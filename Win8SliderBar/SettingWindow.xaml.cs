using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace Win8SliderBar
{
    /// <summary>
    /// SettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingWindow 
    {

        public ShortcutsCollection sc;
        private string DataPath;

        public SettingWindow(string path)
        {
            DataPath = path;
            InitializeComponent();
            LoadData();
            this.BottomMouseDown += Bottom_MouseDown;
            this.TitleBarMouseDown += TitleBar_MouseDown;
            if (IsAutoRun("Win8SliderBar")) cbAutoRun.IsChecked = true;
            lvShortcuts.ItemsSource = sc;
        }


        private void TitleBar_MouseDown(object obj, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                this.DragMove();
            }

        }

        private void Bottom_MouseDown(object obj, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                Process.Start("http://www.7fenx.com/win8sliderbar.html");
            }

        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            switch(combType.SelectedIndex)
            {
                case 0:
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter = "所有文件(*.*)|*.*";
                    if (ofd.ShowDialog() == true)
                    {
                        tbPath.Text = ofd.FileName;
                    }
                    break;
                case 1:
                    System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
                    if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        tbPath.Text = fbd.SelectedPath;
                    }
                    break;
                case 2:
                    tbPath.Text = "http://请在此处输入网址";
                    tbPath.IsReadOnly = false;
                    break;
            }
            
            
        }

        private void combType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbPath!=null&&combType.SelectedIndex == 2)
            {
                tbPath.IsReadOnly = false;
                tbPath.Text = "http://";
            }
            else if(tbPath!=null)
            {
                tbPath.Text = string.Empty;
                tbPath.IsReadOnly = true;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (tbName.Text != string.Empty && tbPath.Text != string.Empty)
            {
                if (combType.SelectedIndex == 2 && !IsUrl(tbPath.Text))
                {
                    MessageBox.Show("你输入的网址不正确，请重新输入","错误",MessageBoxButton.OKCancel, MessageBoxImage.Error);
                }
                else
                {
                    sc.Add(new Shortcuts(tbName.Text,GetType(combType.SelectedIndex),tbPath.Text));
                }
            }
        }

        public bool IsUrl(string str_url) 
        {
            return System.Text.RegularExpressions.Regex.IsMatch(str_url, @"(\w+:\/\/)?([^\.]+)(\.[^/:]+)(:\d*)?([^# ]*)");
        }

        public string GetType(int index)
        {
            string type = string.Empty;
            switch (index)
            {
                case 0:
                    type= "文件";
                    break;
                case 1:
                    type= "文件夹";
                    break;
                case 2:
                    type= "网址";
                    break;
            }
            return type;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void LoadData()
        {
            if (File.Exists(DataPath) == true)
            {
                sc = ShortcutsCollection.LoadDataFromFile(DataPath);
            }
            else
            {
                sc = new ShortcutsCollection();
            }

        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            if(sc!=null)
                sc.Clear();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            int index = lvShortcuts.SelectedIndex;
            if(index!=-1)
            {
                sc.RemoveAt(index);
            }
        }

        private void cbAutoRun_Click(object sender, RoutedEventArgs e)
        {
            if (cbAutoRun.IsChecked == true)
            {
                SetAutoRun("Win8SliderBar", System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
            else
            {
                DeleteAutoRun("Win8SliderBar");
            }
        }

        #region 设置/取消/检测开机启动
        public static bool SetAutoRun(string keyName,string filePath)
        {
            try
            {

            RegistryKey runKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

                runKey.SetValue(keyName,filePath);

                runKey.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool DeleteAutoRun(string keyName)
        {

            try
            {

                RegistryKey runKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

                runKey.DeleteValue(keyName);
                runKey.Close();

            }

            catch
            {

                return false;

            }

            return true;

        }

        public static bool IsAutoRun(string keyName)
        {
            try
            {

                RegistryKey runKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

                object obj = runKey.GetValue(keyName);

                runKey.Close();

                if (obj != null)
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false ;
        }
        #endregion
    }
}
