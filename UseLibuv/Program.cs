using Google.Protobuf;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Tutorial;

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
            AddressBook addrbook = new AddressBook();
            addrbook.People.Add(new Person() {Name="abc",Id=10,Email="abc@163.com" });
            addrbook.People.Add(new Person() {Name="abcd",Id=11,Email="abcd@163.com" });



            byte[] buffs = new byte[1024];
            var gstream = new CodedOutputStream(buffs);
            addrbook.WriteTo(gstream);
            int i=addrbook.CalculateSize();
            Console.WriteLine($"c size:{i},buf:{gstream.Position}");

            var istream = new CodedInputStream(buffs,0,i);

            AddressBook book2 = new AddressBook();
            book2.MergeFrom(istream);
            Console.WriteLine($"{book2.People[0].Name}");

            
            //book2.



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
