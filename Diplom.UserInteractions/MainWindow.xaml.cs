using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
using Diplom.BLEInteractions.Interfaces;
using Diplom.MyMath;

namespace Diplom.UserInteractions
{
    public partial class MainWindow : Window
    {
        private BaseBLEWatcher Watcher;
        private IBLEConnector Connector;

        bool isMap;

        private bool isDragging;
        private Point clickPosition;
        private TranslateTransform originTT;

        public MainWindow(BaseBLEWatcher Watcher, IBLEConnector Connector)
        {
            this.Watcher = Watcher;
            this.Connector = Connector;

            this.Watcher.Removed += DeviceRemoved;
            this.Watcher.Added += DeviceAdded;

            InitializeComponent();

            StopButton.IsEnabled = false;
            isMap = false;
        }

        private void DeviceAdded(string Id)
        {
            Visible.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => Visible.Text += Id + "\n"));
        }

        private void DeviceRemoved(string Id)
        {
            Visible.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => Visible.Text = Visible.Text.Replace(Id + "\n", "")));
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Visible.Text = "";
            Watcher.StartWatcher();
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            Watcher.StopWatcher();
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
        }

        private async void Connect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (await Connector.ConnectTo(DeviceID_C.Text))
                    Connected.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => Connected.Text += DeviceID_C.Text + "\n"));
            }
            catch(ArgumentException) {}
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string Name = Connector.GetName(DeviceID_D.Text);
                if (Connector.DisconnectFrom(DeviceID_D.Text))
                {
                    Connect.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => Connected.Text = Connected.Text.Replace(DeviceID_D.Text + "\n", "")));
                    foreach (UIElement item in RealCanvas.Children)
                        if (item.Uid == Name)
                        {
                            RealCanvas.Children.Remove(item);
                            break;
                        }
                }
            }
            catch (ArgumentException) { }
        }

        private void ShowMap_Click(object sender, RoutedEventArgs e)
        {
            if (!isMap)
            {
                Time.Visibility = Visibility.Visible;
                Clear.Visibility = Visibility.Visible;
                Connected.Visibility = Visibility.Collapsed;
                Disconnect.Visibility = Visibility.Collapsed;
                StartButton.Visibility = Visibility.Collapsed;
                StopButton.Visibility = Visibility.Collapsed;
                DeviceID_C.Visibility = Visibility.Collapsed;
                DeviceID_D.Visibility = Visibility.Collapsed;
                Visible.Visibility = Visibility.Collapsed;
                Connected.Visibility = Visibility.Collapsed;
                Connect.Visibility = Visibility.Collapsed;
                Text1.Visibility = Visibility.Collapsed;
                Text2.Visibility = Visibility.Collapsed;
                MyCanvas.Visibility = Visibility.Visible;
                WriteToDevices.Visibility = Visibility.Visible;
                ToWrite.Visibility = Visibility.Visible;
                UpdateTime.Visibility = Visibility.Visible;
                StartCalculation.Visibility = Visibility.Visible;
                ShowMap.Content = "Hide map";
            }
            else
            {
                Time.Visibility = Visibility.Collapsed;
                Clear.Visibility = Visibility.Collapsed;
                Connected.Visibility = Visibility.Visible;
                Disconnect.Visibility = Visibility.Visible;
                StartButton.Visibility = Visibility.Visible;
                StopButton.Visibility = Visibility.Visible;
                DeviceID_C.Visibility = Visibility.Visible;
                DeviceID_D.Visibility = Visibility.Visible;
                Visible.Visibility = Visibility.Visible;
                Connected.Visibility = Visibility.Visible;
                Connect.Visibility = Visibility.Visible;
                Text1.Visibility = Visibility.Visible;
                Text2.Visibility = Visibility.Visible;
                MyCanvas.Visibility = Visibility.Collapsed;
                WriteToDevices.Visibility = Visibility.Collapsed;
                ToWrite.Visibility = Visibility.Collapsed;
                UpdateTime.Visibility = Visibility.Collapsed;
                StartCalculation.Visibility = Visibility.Collapsed;
                ShowMap.Content = "Open Map";
            }
            isMap = !isMap;
        }

        private void WriteToDevices_Click(object sender, RoutedEventArgs e)
        {
            List<Task> tasks = new List<Task>();
            string a = ToWrite.Text;
            bool ass = false;
            foreach (string item in Connector)
            {
                ass = false;
                tasks.Add(new Task<bool>(() => { return Connector.GetDeviceInteractor(item).Write(a, 2, 1).Result; }));
                foreach (UIElement v in RealCanvas.Children)
                {
                    if (v.Uid == item)
                        ass = true;
                }
                if(!ass)
                    circle(item, 10, 10, 10, 10, RealCanvas);
            }

            tasks.ForEach(x => x.Start());
            Task.WaitAll(tasks.ToArray());
        }

        private void StartCalculation_Click(object sender, RoutedEventArgs e)
        {
            (string id, double X, double Y, List<int> values)[] ps = new (string name, double X, double Y, List<int> values)[Connector.ConnectedDevices];
            int counter = 0;

            Stopwatch watch = new Stopwatch();


            foreach (string id in Connector)
            {
                ps[counter].id = id;
                ps[counter].values = new List<int>();
                foreach (UIElement item in RealCanvas.Children)
                {
                    if(item.Uid == id)
                    {
                        ps[counter].X = Convert.ToInt32( item.GetValue(Canvas.LeftProperty));
                        ps[counter].Y = MyCanvas.Height - Convert.ToInt32(item.GetValue(Canvas.TopProperty));
                        break;
                    }
                }
                counter++;
            }

            watch.Start();

            while (watch.ElapsedMilliseconds < Convert.ToInt32(Time.Text)*1000)
            { 
                List<Task> tasks = new List<Task>();
                string b = "";
                for (int i = 0; i < ps.Length; i++)
                {
                    int a = i;
                    tasks.Add(new Task(() => {
                        try
                        {
                            ps[a].values.Add(Convert.ToInt32(Connector.GetDeviceInteractor(ps[a].id).Read(2, 0).Result));
                        }
                        catch(Exception)
                        {

                        }
                    }));
                }

                tasks.ForEach(x => x.Start());

                Task.WaitAll(tasks.ToArray());
            }
            watch.Stop();

            foreach (var item in ps)
            {
                item.values.RemoveAll(x => x == 0);
            }
            try
            {
                ps.Where(z => z.values.Count>=8).ToList().ForEach(z => z.values.RemoveAll(x => x < z.values.Average() - 2 * Math.Sqrt(z.values.Sum(y => (double)(y - z.values.Average()) * (y - z.values.Average())) / z.values.Count) || x > z.values.Average() + 2 * Math.Sqrt(z.values.Sum(y => (double)(y - z.values.Average()) * (y - z.values.Average())) / z.values.Count)));
                (double x, double y) point = PositionCalculator.LS(ps);
                circle("a", (int)point.x, (int)point.y, 5, 5, RealCanvas);
                File.AppendAllText(@"C:\Users\proto\Desktop\Main.txt", point.x +" "+point.y+"\n");
            }
            catch(Exception)
            {

            }
        }


        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var draggableControl = sender as Shape;
            originTT = draggableControl.RenderTransform as TranslateTransform ?? new TranslateTransform();
            isDragging = true;
            clickPosition = e.GetPosition(this);
            draggableControl.CaptureMouse();
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            string a = Connector.GetName((sender as UIElement).Uid);
            if (UpdateTime.Text != a)
                UpdateTime.Text = a;
            isDragging = false;
            var draggable = sender as Shape;
            draggable.ReleaseMouseCapture();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var draggableControl = sender as Shape;
            if (isDragging && draggableControl != null)
            {
                Point currentPosition = e.GetPosition(this);
                var transform = draggableControl.RenderTransform as TranslateTransform ?? new TranslateTransform();
                transform.X = originTT.X + (currentPosition.X - clickPosition.X);
                (sender as UIElement).SetValue(Canvas.LeftProperty, transform.X);
                transform.Y = originTT.Y + (currentPosition.Y - clickPosition.Y);
                (sender as UIElement).SetValue(Canvas.TopProperty, transform.Y);
                //draggableControl.RenderTransform = new TranslateTransform(transform.X, transform.Y);
            }
        }
        public void circle(string name,int x, int y, int width, int height, Canvas cv)
        {
            Ellipse circle = new Ellipse()
            {
                Uid = name,
                Width = width,
                Height = height,
                Stroke = Brushes.Red,
                StrokeThickness = 6
            };

            circle.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
            circle.MouseMove += Canvas_MouseMove;
            circle.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;

            cv.Children.Add(circle);

            circle.SetValue(Canvas.LeftProperty, (double)x);
            circle.SetValue(Canvas.TopProperty, 340 - (double)y);
        }
        public void circleg(string name, int x, int y, int width, int height, Canvas cv)
        {
            Ellipse circle = new Ellipse()
            {
                Uid = name,
                Width = width,
                Height = height,
                Stroke = Brushes.Green,
                StrokeThickness = 6
            };

            circle.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
            circle.MouseMove += Canvas_MouseMove;
            circle.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;

            cv.Children.Add(circle);

            circle.SetValue(Canvas.LeftProperty, (double)x);
            circle.SetValue(Canvas.TopProperty, (double)y);
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < RealCanvas.Children.Count;i ++)
            {
                if(RealCanvas.Children[i].Uid == "a")
                {
                    RealCanvas.Children.Remove(RealCanvas.Children[i]);
                }
            }
        }
    }
}
