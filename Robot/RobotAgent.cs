using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using Robot.Test;
using System.Threading.Tasks;
using SharedLib;

namespace Robot
{

    public class RobotBtn
    {
        static public BitmapImage idleImg   = new BitmapImage(new Uri("res/robotStop.png", UriKind.Relative));
        static public BitmapImage errorImg  = new BitmapImage(new Uri("res/robotError.png", UriKind.Relative));
        static public BitmapImage workImg = new BitmapImage(new Uri("res/robotWork.png", UriKind.Relative));
 
        public enum State
        {
            Idle=0,
            Work,
            Error

        }

        public const int width  = 50;
        public const int height = 50;
        public const int margin = 5;

        Button _btn;
        public Button Btn => _btn;

        public RobotBtn(int id,int row,int col)
        {
            int left =  col * ( width +margin);
            int top =  row * (height +margin);
            _btn = new Button()
            {
                // Name = $"btn{id}",
                Tag = id,
                Height = height,
                Width = width,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(left, top, 0, 0),
                Content = new Image()
                {
                    Source = idleImg,
                    VerticalAlignment = VerticalAlignment.Center
                } 
            };
        }

        public void SetState(State state)
        {
            switch (state)
            {
                case State.Idle:
                    Btn.Content = new Image()
                    {
                        Source = idleImg,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    break;
                case State.Work:
                    Btn.Content = new Image()
                    {
                        Source = workImg,
                        VerticalAlignment = VerticalAlignment.Center
                    };


                    break;
                case State.Error:
                    Btn.Content = new Image()
                    {
                        Source = errorImg,
                        VerticalAlignment = VerticalAlignment.Center
                    };


                    break;
                default:
                    break;



            }

        }





    }

    public class RobotAgent
    {
        RobotBtn _rbtn;
        public Button Btn => _rbtn.Btn;
        int _id;
        public int Id => _id;

        public RobotAgent(int id, int row, int col) {
            _rbtn = new RobotBtn(id, row, col);
            _id = id;

            //_test = new TcpTest();
        }
        bool _connecting = false;
        Socket _s;
        TestCase _test;


        byte[] _recvBuf = new byte[1024*64];
        public void connectCB (IAsyncResult ar){

            //ar.AsyncState;
            Trace.WriteLine($"Connect CB !{_s.Connected}");
            
            _connecting = false;
            if (_s.Connected)
            {
                _rbtn.SetState(RobotBtn.State.Work);
            }
            else
            {
                _rbtn.SetState(RobotBtn.State.Error);
 
            }

        }
        public bool Send(Byte[] msg)
        {
            if(_s!=null && _s.Connected)
            {
                _s.SendAsync(msg,SocketFlags.None);
                return true;
            }
            return false;

        }
        public void Stop()
        {
            if (_s!=null && _s.Connected)
            {
                _s.Close();
                _s = null;

                _rbtn.SetState(RobotBtn.State.Idle);
            }
 
        }
        public void Update()
        {
            _test?.Update(this);
        }

        PacketRead pr = new PacketRead();
        void RecvCB(IAsyncResult r)
        {
            try
            {
                RobotAgent agent = (RobotAgent)r.AsyncState;
                int length = agent._s.EndReceive(r);

                List<Packet> plist = new List<Packet>();
                pr.process(_recvBuf, 0, length, plist);

                foreach(var p in plist)
                {
                    string s = pr.ReadString(p);
                    Trace.WriteLine($"Receive:{length},con:{s}");
                }

                _s.BeginReceive(_recvBuf, 0, _recvBuf.Length, SocketFlags.None, RecvCB, this);

            }
            catch (Exception e)
            {
                Trace.WriteLine($"Exception:{e}");
            }
       }

        public async void Start(Type caseType )
        {
            Trace.WriteLine("Agent Start Connct !");
            _test = Activator.CreateInstance(caseType) as TestCase;

            if ( _s == null)
                _s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(Setting.Host), Setting.Port);


            if( _connecting == false && !_s.Connected)
            {
                _connecting = true;
                //_s.BeginConnect(ip, connectCB, _s);

                try
                {

                    await _s.ConnectAsync(ip);
                }catch(Exception e)
                {

                }
                finally
                {
                    _connecting = false;

                    if (_s.Connected)
                    {
                        _s.BeginReceive(_recvBuf,0,_recvBuf.Length, SocketFlags.None, RecvCB,this);

                    }

                    _rbtn.SetState( _s.Connected?RobotBtn.State.Work:RobotBtn.State.Error);
 
                }


            }
            /*
            SocketAsyncEventArgs e = new SocketAsyncEventArgs();
            e.Completed+= 
            s.ConnectAsync(SocketType.Stream, ProtocolType.Tcp, SocketAsyncEventArgs e);
            s.BeginConnect(ip,connectCB);
            */


        }

        public Action work;


    }
}
