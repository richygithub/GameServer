using System;
using System.Collections.Generic;
using System.Text;
using SharedLib;

namespace Robot.Test
{
    interface TestCase
    {
        void Update(RobotAgent robot); 
    }
    public class TcpTest:TestCase
    {
        public TcpTest()
        {
            pw = new PacketWrite();
        }
        PacketWrite pw;
        public void Update(RobotAgent robot)
        {
            var msg = pw.Write($"It is robot {robot.Id}");
            robot.Send(msg);
        }
    }
}
