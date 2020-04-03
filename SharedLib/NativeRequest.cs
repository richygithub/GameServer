using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace UseLibuv 
{

    abstract public unsafe class NativeRequest : IDisposable
    {

        readonly uv_req_type requestType;
        protected internal IntPtr Handle;

        protected NativeRequest(uv_req_type requestType, int size)
        {
            IntPtr handle = Libuv.Allocate(requestType );

            GCHandle gcHandle = GCHandle.Alloc(this, GCHandleType.Normal);
            *(IntPtr*)handle = GCHandle.ToIntPtr(gcHandle);

            this.Handle = handle;
            this.requestType = requestType;
        }

        protected bool IsValid
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.Handle != IntPtr.Zero;
        }

        protected void CloseHandle()
        {
            IntPtr handle = this.Handle;
            if (handle == IntPtr.Zero)
            {
                return;
            }

            IntPtr pHandle = ((uv_req_t*)handle)->data;

            // Free GCHandle
            if (pHandle != IntPtr.Zero)
            {
                GCHandle nativeHandle = GCHandle.FromIntPtr(pHandle);
                if (nativeHandle.IsAllocated)
                {
                    nativeHandle.Free();
                    ((uv_req_t*)handle)->data = IntPtr.Zero;
                }
            }

            // Release memory
            Libuv.FreeMemory(handle);
            this.Handle = IntPtr.Zero;
        }

        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (this.IsValid)
                {
                    this.CloseHandle();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{this.requestType} {this.Handle} error whilst closing handle.{exception}" );

                // For finalizer, we cannot allow this to escape.
                if (disposing) throw;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~NativeRequest() => this.Dispose(false);

        internal static T GetDataFromHandle<T>(IntPtr handle)
        {
            Debug.Assert(handle != IntPtr.Zero);

            IntPtr internalHandle = ((uv_req_t*)handle)->data;
            if (internalHandle != IntPtr.Zero)
            {
                GCHandle gcHandle = GCHandle.FromIntPtr(internalHandle);
                if (gcHandle.IsAllocated)
                {
                    return (T)gcHandle.Target;
                }
            }
            return default(T);
        }
    }

}
