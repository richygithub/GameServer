using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace UseLibuv
{




    class Program
    {

        static string test = "abcdefg";

        unsafe static void change(string s)
        {
            fixed( char* p =s ){
                p[0] = '1';

            }

        }
        static void work_cb(IntPtr watcher)
        {
            change(test);
            Console.WriteLine(test);

        }
        static void Main(string[] args)
        {

            /*
            Task.Run(() =>
            {
                IntPtr loop = Libuv.CreateLoop();

                Libuv.uv_loop_init(loop);

                IntPtr timer = Libuv.Allocate(Libuv.uv_handle_type.UV_TIMER);
                Libuv.uv_timer_init(loop, timer);
                Libuv.uv_timer_start(timer, work_cb, 1000, 1000);

                Libuv.uv_run(loop, Libuv.uv_run_mode.UV_RUN_DEFAULT);
            });
            */
            //AreaServer as

            Console.WriteLine("Hello World!");

            //AreaServer area = new AreaServer();
            AreaServer.instance.Start();

            //Server s = new Server(11240);
            //s.Start();



            /*


                        IntPtr loop = Libuv.CreateLoop();

                        Libuv.uv_loop_init(loop);

                        IntPtr timer = Libuv.Allocate(Libuv.uv_handle_type.UV_TIMER);
                        Libuv.uv_timer_init(loop, timer);
                        Libuv.uv_timer_start(timer, work_cb, 1000, 1000);

                        Libuv.uv_run(loop, Libuv.uv_run_mode.UV_RUN_DEFAULT);
                    */
        }
    }
}
