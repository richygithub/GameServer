﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace UseLibuv
{

    enum uv_handle_type
    {
        UV_UNKNOWN_HANDLE = 0,
        UV_ASYNC,
        UV_CHECK,
        UV_FS_EVENT,
        UV_FS_POLL,
        UV_HANDLE,
        UV_IDLE,
        UV_NAMED_PIPE,
        UV_POLL,
        UV_PREPARE,
        UV_PROCESS,
        UV_STREAM,
        UV_TCP,
        UV_TIMER,
        UV_TTY,
        UV_UDP,
        UV_SIGNAL,
        UV_FILE,
        UV_HANDLE_TYPE_MAX

    }
    [StructLayout(LayoutKind.Sequential)]
    struct uv_handle_t
    {
        public IntPtr data;
        public IntPtr loop;
        public uv_handle_type type;
        public IntPtr close_cb;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct uv_buf_t
    {
        static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        static readonly int Size = IntPtr.Size;

        /*
           Windows 
           public int length;
           public IntPtr data;

           Unix
           public IntPtr data;
           public IntPtr length;
        */

        // ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
        readonly IntPtr first;
        readonly IntPtr second;
        // ReSharper restore PrivateFieldCanBeConvertedToLocalVariable

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void InitMemory(IntPtr buf, IntPtr memory, int length)
        {
            var len = (IntPtr)length;
            if (IsWindows)
            {
                *(IntPtr*)buf = len;
                *(IntPtr*)(buf + Size) = memory;
            }
            else
            {
                *(IntPtr*)buf = memory;
                *(IntPtr*)(buf + Size) = len;
            }
        }

        public uv_buf_t(IntPtr memory, int length)
        {
            if (IsWindows)
            {
                this.first = (IntPtr)length;
                this.second = memory;
            }
            else
            {
                this.first = memory;
                this.second = (IntPtr)length;
            }
        }

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct sockaddr
    {
        static readonly bool IsMacOS = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        // this type represents native memory occupied by sockaddr struct
        // https://msdn.microsoft.com/en-us/library/windows/desktop/ms740496(v=vs.85).aspx
        // although the c/c++ header defines it as a 2-byte short followed by a 14-byte array,
        // the simplest way to reserve the same size in c# is with four nameless long values
        public long field0;
        public long field1;
        public long field2;
        public long field3;

        // ReSharper disable once UnusedParameter.Local
        public sockaddr(long ignored)
        {
            this.field0 = 0;
            this.field1 = 0;
            this.field2 = 0;
            this.field3 = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe IPEndPoint GetIPEndPoint()
        {
            // The bytes are represented in network byte order.
            //
            // Example 1: [2001:4898:e0:391:b9ef:1124:9d3e:a354]:39179
            //
            // 0000 0000 0b99 0017  => The third and fourth bytes 990B is the actual port
            // 9103 e000 9848 0120  => IPv6 address is represented in the 128bit field1 and field2.
            // 54a3 3e9d 2411 efb9     Read these two 64-bit long from right to left byte by byte.
            // 0000 0000 0000 0000
            //
            // Example 2: 10.135.34.141:39178 when adopt dual-stack sockets, IPv4 is mapped to IPv6
            //
            // 0000 0000 0a99 0017  => The port representation are the same
            // 0000 0000 0000 0000
            // 8d22 870a ffff 0000  => IPv4 occupies the last 32 bit: 0A.87.22.8d is the actual address.
            // 0000 0000 0000 0000
            //
            // Example 3: 10.135.34.141:12804, not dual-stack sockets
            //
            // 8d22 870a fd31 0002  => sa_family == AF_INET (02)
            // 0000 0000 0000 0000
            // 0000 0000 0000 0000
            // 0000 0000 0000 0000
            //
            // Example 4: 127.0.0.1:52798, on a Mac OS
            //
            // 0100 007F 3ECE 0210  => sa_family == AF_INET (02) Note that struct sockaddr on mac use
            // 0000 0000 0000 0000     the second unint8 field for sa family type
            // 0000 0000 0000 0000     http://www.opensource.apple.com/source/xnu/xnu-1456.1.26/bsd/sys/socket.h
            // 0000 0000 0000 0000
            //
            // Reference:
            //  - Windows: https://msdn.microsoft.com/en-us/library/windows/desktop/ms740506(v=vs.85).aspx
            //  - Linux: https://github.com/torvalds/linux/blob/6a13feb9c82803e2b815eca72fa7a9f5561d7861/include/linux/socket.h
            //  - Apple: http://www.opensource.apple.com/source/xnu/xnu-1456.1.26/bsd/sys/socket.h

            // Quick calculate the port by mask the field and locate the byte 3 and byte 4
            // and then shift them to correct place to form a int.
            int port = ((int)(this.field0 & 0x00FF0000) >> 8) | (int)((this.field0 & 0xFF000000) >> 24);

            int family = (int)this.field0;
            if (IsMacOS)
            {
                // see explaination in example 4
                family = family >> 8;
            }
            family = family & 0xFF;

            if (family == 2)
            {
                // AF_INET => IPv4
                return new IPEndPoint(new IPAddress((this.field0 >> 32) & 0xFFFFFFFF), port);
            }
            else if (this.IsIPv4MappedToIPv6())
            {
                long ipv4bits = (this.field2 >> 32) & 0x00000000FFFFFFFF;
                return new IPEndPoint(new IPAddress(ipv4bits), port);
            }
            else
            {
                // otherwise IPv6
                var bytes = new byte[16];
                fixed (byte* b = bytes)
                {
                    *((long*)b) = this.field1;
                    *((long*)(b + 8)) = this.field2;
                }

                return new IPEndPoint(new IPAddress(bytes), port);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IsIPv4MappedToIPv6()
        {
            // If the IPAddress is an IPv4 mapped to IPv6, return the IPv4 representation instead.
            // For example [::FFFF:127.0.0.1] will be transform to IPAddress of 127.0.0.1
            if (this.field1 != 0)
            {
                return false;
            }

            return (this.field2 & 0xFFFFFFFF) == 0xFFFF0000;
        }
    }



    public unsafe static class Libuv
    {
        public enum uv_run_mode
        {
            UV_RUN_DEFAULT = 0,
            UV_RUN_ONCE,
            UV_RUN_NOWAIT
        };
        public enum uv_handle_type
        {
            UV_UNKNOWN_HANDLE = 0,
            UV_ASYNC,
            UV_CHECK,
            UV_FS_EVENT,
            UV_FS_POLL,
            UV_HANDLE,
            UV_IDLE,
            UV_NAMED_PIPE,
            UV_POLL,
            UV_PREPARE,
            UV_PROCESS,
            UV_STREAM,
            UV_TCP,
            UV_TIMER,
            UV_TTY,
            UV_UDP,
            UV_SIGNAL,
            UV_FILE,
            UV_HANDLE_TYPE_MAX
        }

        private const string LibraryName = "libuv";



        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void uv_work_cb(IntPtr watcher);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void uv_watcher_cb(IntPtr watcher, int status);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void uv_close_cb(IntPtr conn);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void uv_alloc_cb(IntPtr handle, IntPtr suggested_size, out uv_buf_t buf);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void uv_read_cb(IntPtr handle, IntPtr nread, ref uv_buf_t buf);






        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int uv_run(IntPtr handle, uv_run_mode mode);

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr uv_loop_size();


        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int uv_loop_init(IntPtr handle);


        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int uv_timer_init(IntPtr loopHandle, IntPtr handle);

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int uv_timer_start(IntPtr handle, uv_work_cb work_cb, long timeout, long repeat);

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int uv_timer_stop(IntPtr handle);

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr uv_handle_size(uv_handle_type handleType);


        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int uv_listen(IntPtr handle, int backlog, uv_watcher_cb connection_cb);


        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int uv_accept(IntPtr server, IntPtr client);


        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int uv_read_start(IntPtr handle, uv_alloc_cb alloc_cb, uv_read_cb read_cb);



        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int uv_ip4_addr(string ip, int port, out sockaddr address);

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int uv_ip6_addr(string ip, int port, out sockaddr address);

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int uv_tcp_init(IntPtr loopHandle, IntPtr handle);

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int uv_tcp_bind(IntPtr handle, ref sockaddr sockaddr, uint flags);


        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void uv_close(IntPtr handle, uv_close_cb close_cb);


        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int uv_is_closing(IntPtr handle);


        public static int GetSize(uv_handle_type handleType) => uv_handle_size(handleType).ToInt32();
        public static IntPtr Allocate(uv_handle_type handleType)
        {
            int size = GetSize(handleType);
            return Allocate(size);
        }

        public static IntPtr Allocate(int size) => Marshal.AllocCoTaskMem(size);

        public static void FreeMemory(IntPtr ptr) => Marshal.FreeCoTaskMem(ptr);

        public static IntPtr CreateLoop() => Allocate( uv_loop_size().ToInt32() );


        public static void GetSocketAddress(IPEndPoint endPoint, out sockaddr addr)
        {
            Debug.Assert(endPoint != null);

            string ip = endPoint.Address.ToString();
            int result;
            switch (endPoint.AddressFamily)
            {
                case AddressFamily.InterNetwork:
                    result = uv_ip4_addr(ip, endPoint.Port, out addr);
                    break;
                case AddressFamily.InterNetworkV6:
                    result = uv_ip6_addr(ip, endPoint.Port, out addr);
                    break;
                default:
                    throw new NotSupportedException($"End point {endPoint} is not supported, expecting InterNetwork/InterNetworkV6.");
            }
        }

    }
}