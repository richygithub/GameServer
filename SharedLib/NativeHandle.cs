using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace UseLibuv
{
    public unsafe class NativeHandle : IDisposable
    {



        static readonly uv_close_cb CloseCallback = OnCloseHandle;
        protected IntPtr _handle;
        public IntPtr Handle => _handle;

        bool _disposed = false;
        public NativeHandle()
        {
        }
        ~NativeHandle() => Dispose(false);

        public void Dispose()
        {
            //throw new NotImplementedException();
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public static IntPtr GetLoopFromHandle(IntPtr handle)
        {
            return ((uv_handle_t*)handle)->loop;
        }

        public static T GetDataFromHandle<T>(IntPtr handle)
        {
            IntPtr inernalHandle = ((uv_handle_t*)handle)->data;
            if(inernalHandle != IntPtr.Zero )
            {
                GCHandle gcHandle = GCHandle.FromIntPtr(inernalHandle);
                if (gcHandle.IsAllocated)
                {
                    return (T)gcHandle.Target;
                }
            }
            return default(T);
        }
        internal void CloseHandle()
        {
            IntPtr handle = this._handle;
            if (handle == IntPtr.Zero)
            {
                return;
            }

            int result = Libuv.uv_is_closing(handle);
            if (result == 0)
            {
                Libuv.uv_close(handle, CloseCallback);
            }
        }

        static void OnCloseHandle(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
            {
                return;
            }

            NativeHandle nativeHandle = null;

            // Get gc handle first
            IntPtr pHandle = ((uv_handle_t*)handle)->data;
            if (pHandle != IntPtr.Zero)
            {
                GCHandle gcHandle = GCHandle.FromIntPtr(pHandle);
                if (gcHandle.IsAllocated)
                {
                    nativeHandle = gcHandle.Target as NativeHandle;
                    //Console.WriteLine("gcHandle free");
                    gcHandle.Free();

                    ((uv_handle_t*)handle)->data = IntPtr.Zero;
                }
            }


            // Release memory
            Libuv.FreeMemory(handle);
            nativeHandle?.OnClosed();
        }
        void Dispose(bool disposing)
        {

            if( !_disposed)
            {
                CloseHandle();
            }
            _disposed = true;

        }
        protected virtual void OnClosed()
        {
            _handle = IntPtr.Zero;
        }

    }
}
