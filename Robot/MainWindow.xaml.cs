using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Robot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>


    public class RobotView
    {
        Grid _grid;
        public RobotView(Grid grid)
        {
            _grid = grid;
            update();
        }
        int _count = 0;

        int _maxRow = 0;
        int _maxCol = 0;

        Dictionary<int, RobotAgent> _robots = new Dictionary<int, RobotAgent>();
        void update()
        {
            _maxRow = (int)_grid.Height/ ( RobotBtn.height+ RobotBtn.margin );
            _maxCol = (int)_grid.Width / ( RobotBtn.width + RobotBtn.margin );
        }
        public void CreateRobot(int num)
        {
            for(int idx = 0; idx < num; idx++)
            {
                CreateRobot();
            }

        }
        void CreateRobot()
        {
            if(_count >= _maxCol * _maxRow)
            {
                Console.WriteLine($"too many robot.limit:{_maxRow*_maxCol}");
                return;
            }
            int row = _count / _maxCol;
            int col = _count % _maxCol;

            RobotAgent r = new RobotAgent(_count, row, col);
            _robots.Add(_count, r);
            _count++;
            _grid.Children.Add(r.Btn);
        }
        public void Start(Type caseType)
        {


            foreach(var kv in _robots)
            {
                var robot = kv.Value;
                //robot?.work();
                robot.Start(caseType);

            }
        }
        public void Update()
        {
            foreach(var kv in _robots)
            {
                var robot = kv.Value;
                //robot?.work();
                robot.Update();

            }
 
        }


        public void Stop()
        {
            foreach (var kv in _robots)
            {
                var robot = kv.Value;
                //robot?.work();
                robot.Stop();

            }
        }
    }




    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            robotView = new RobotView( MainGrid);

            var cases = Setting.TestCases;

            testCase.ItemsSource = cases;
            testCase.SelectedIndex = 0;
            /*
            foreach(var c in cases)
            {
            }
            */
            Assembly a = typeof(Robot.Test.TestCase).Assembly;
            Trace.WriteLine($"testcase:{a.FullName}");

            Assembly a1 = typeof(RobotView).Assembly;
            Trace.WriteLine($"RobotView:{a1.FullName}");


            foreach (Assembly b in AppDomain.CurrentDomain.GetAssemblies())
            {
                Trace.WriteLine( $"Assembly: {b.GetName()}");

            }

            var aa = Assembly.Load("Robot");
            Type t = aa.GetType("Robot.Test.TcpTest");
            Activator.CreateInstance(t);

                InitTimer();
        }

        DispatcherTimer  timer;
        void InitTimer()
        {
            int interval = 100;
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0,0,0,0,interval);
            timer.Tick += timerUp;
            timer.Start();


        }
        void timerUp(object sender, EventArgs e)
        {
            //TimerTxt.Content = $"heart beat:{count++}";
            robotView.Update();
        }

        RobotView robotView;
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            //Grid.SetRow(btn, count++);
            int num=int.Parse(Num.Text);

            robotView.CreateRobot(num);

        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            var casename = testCase.SelectedItem as string;
            //var assembly = Assembly.Load($"Robot.Test.{casename}");
            var assembly = Assembly.Load("Robot");
            Type t = assembly.GetType($"Robot.Test.{casename}");
            robotView.Start(t);
        }
        private static readonly Regex _regex = new Regex("[^0-9.-]+");
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _regex.IsMatch(e.Text);
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            robotView.Stop();
        }
    }
}
