using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Win8SliderBar
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Win32Api声明
        [DllImport("user32")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        //获取窗口句柄传递键盘消息
        [DllImport("USER32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        #endregion

        private DispatcherTimer showTimer;
        private DispatcherTimer hideTimer;
        private double screenWidth;
        private Point mousePos;
        private ShortcutsCollection sc;
        private string dataPath = string.Empty;

        public MainWindow()
        {
            dataPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\config.inf";
            InitializeComponent();
            this.SetPosition();
            this.InitializeTimer();
            LoadData();
            InitializePanel(sc);
        }

        #region 设置显示，隐藏动画

        public void InitializeTimer()
        {
            
            showTimer = new DispatcherTimer();
            showTimer.Interval = TimeSpan.FromMilliseconds(10);
            showTimer.Tick += ShowTimer_Tick;

            hideTimer = new DispatcherTimer();
            hideTimer.Interval = TimeSpan.FromMilliseconds(10);
            hideTimer.Tick += hideTimer_Tick;
            
        }
        void hideTimer_Tick(object sender, EventArgs e)
        {
            if (this.Left < screenWidth - 6 )
            {
                this.Left += 10;
            }
            else
            {
                this.Left = screenWidth - 2;
                hideTimer.Stop();
            }
        }

        void ShowTimer_Tick(object sender, EventArgs e)
        {
            if (this.Left >= screenWidth - this.Width+6)
            {
                this.Left -= 10;
            }
            else
            {
                this.Left = screenWidth - this.Width + 1;
                showTimer.Stop();
            }

        }
        #endregion

        #region 设置窗口宽度及位置
        public void SetPosition()
        {
            try
            {
                screenWidth = SystemParameters.WorkArea.Width;
                this.Height = SystemParameters.VirtualScreenHeight + 1;
                //this.Height = SystemParameters.WorkArea.Height;
                this.Top = -1;
                this.Left = screenWidth - 2;
                //this.Left = screenWidth - this.Width + 1;
                this.Topmost = true;
                this.ShowInTaskbar = false;
                IntPtr hDeskTop = FindWindow("Progman", "Program Manager");
                SetParent(new WindowInteropHelper(this).Handle, hDeskTop); //windows+D不会隐藏
            }
            catch
            {
            }
        }
        #endregion

        #region 延时函数
        static public void WaitFor(int ms)
        {
            DateTime time = DateTime.Now;
            while (true)
            {
                DoEvents();  //wpf 直接调用Doevents，定义在下面
                TimeSpan span = DateTime.Now - time;
                if (span.TotalMilliseconds > ms) break;
            }
        }

        static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame(true);
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, (SendOrPostCallback)delegate(object arg)
            {
                DispatcherFrame fr = arg as DispatcherFrame;
                fr.Continue = false;
            }, frame);
            Dispatcher.PushFrame(frame);
        }
        #endregion

        #region 初始化Panel
        private void InitializePanel(ShortcutsCollection sc)
        {
            appPanel.Children.Clear();
            directoryPanel.Children.Clear();
            urlPanel.Children.Clear();
            foreach (Shortcuts s in sc)
            {
                Panel p=null;
                UIElement ue=null;
                switch (s.Type) 
                {
                    case "文件":
                        p = appPanel;
                        try
                        {
                            ue = new Image()
                            {
                                Source = IconHelper.GetExeIcon(s.FilePath),
                                Width = 32,
                                Height = 32,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center
                            };
                        }
                        catch
                        {
                            p = null;
                            ue = null;
                        }
                        break;
                    case "文件夹":
                        if (System.IO.Directory.Exists(s.FilePath))
                        {
                            p = directoryPanel;
                            Path path = new Path();
                            Path fileIcon = (Path)this.FindResource("FileIcon");
                            path.Data = fileIcon.Data;
                            path.Fill = fileIcon.Fill;
                            path.Width = fileIcon.Width; path.Height = fileIcon.Height;
                            path.HorizontalAlignment = HorizontalAlignment.Center; path.VerticalAlignment = VerticalAlignment.Center;
                            ue = path;
                        }
                        else
                        {
                            p = null;
                            ue = null;
                        }
                        break;
                    case "网址":
                        if (System.Text.RegularExpressions.Regex.IsMatch(s.FilePath, @"(\w+:\/\/)?([^\.]+)(\.[^/:]+)(:\d*)?([^# ]*)"))
                        {
                            p = urlPanel;
                            Path uPath = new Path();
                            Path urlIcon = (Path)this.FindResource("IeIcon");
                            uPath.Data = urlIcon.Data;
                            uPath.Fill = urlIcon.Fill;
                            uPath.Width = urlIcon.Width; uPath.Height = urlIcon.Height;
                            uPath.HorizontalAlignment = HorizontalAlignment.Center; uPath.VerticalAlignment = VerticalAlignment.Center;
                            ue = uPath;
                        }
                        else
                        {
                            p = null;
                            ue = null;
                        }
                        break;
                }
                if (p != null && ue != null)
                {
                    Button btn = MetroButton.Creat(ue, s.Name, s.FilePath);
                    //btn.HorizontalAlignment = HorizontalAlignment.Center;
                    //btn.VerticalAlignment = VerticalAlignment.Center;                    
                    p.Children.Add(btn);
                }
            }
        }
        #endregion

        #region 加载数据

        private void LoadData()
        {
            if (System.IO.File.Exists(dataPath) == true)
            {
                sc = ShortcutsCollection.LoadDataFromFile(dataPath);
            }
            else
            {
                sc = new ShortcutsCollection();
            }

        }
        #endregion

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            //if (sc != null && sc.Count > 0)
            //{
            //    sc.SaveData("config.inf");
            //}
            this.Close();
            Application.Current.Shutdown();
        }

        private void window_MouseEnter(object sender, MouseEventArgs e)
        {
            mousePos = e.GetPosition(this);
            if (!hideTimer.IsEnabled && mousePos.Y <= 1 && mousePos.X >= 0)
            {
                WaitFor(500);
                if (e.GetPosition(this) == mousePos)
                {
                    showTimer.Start();
                    this.Activate();
                }
            }
        }

        private void window_MouseLeave(object sender, MouseEventArgs e)
        {
            if(!showTimer.IsEnabled)
                hideTimer.Start();
        }

        private void btnOption_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow setwindow = new SettingWindow(dataPath);
            if (setwindow.ShowDialog() == true)
            {
                sc = setwindow.sc;
                InitializePanel(sc);
                sc.SaveData(dataPath);
            }
        }

        private void btnShutDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("shutdown.exe", "-s -t 0");
            }
            catch
            {
            }
        }

        public static bool IsInAdministrator()
        {

            System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();

            System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);

            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);

        }

        private void window_Deactivated(object sender, EventArgs e)
        {
            if(this.Left != screenWidth - 2&&(!showTimer.IsEnabled))
                hideTimer.Start();
        }
    }
}
