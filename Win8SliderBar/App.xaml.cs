using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows;
using Microsoft.TeamFoundation.Common.Internal;

namespace Win8SliderBar
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        System.Threading.Mutex mutex;



        public App()
        {

            this.Startup += new StartupEventHandler(App_Startup);

        }



        void App_Startup(object sender, StartupEventArgs e)
        {

            bool ret;

            mutex = new System.Threading.Mutex(true, "WpfMuerterrrterterttex", out ret);



            if (!ret)

                Environment.Exit(0);

        }
    }
}
