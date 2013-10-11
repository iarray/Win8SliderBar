using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Win8SliderBar
{
    class MetroButton
    {
        /// <summary>
        /// 创建一个Metro风格的按钮
        /// </summary>
        /// <param name="Ue">按钮图标</param>
        /// <param name="text">按钮内容</param>
        /// <returns></returns>
        public static Button Creat(UIElement Ue,string text,string path)
        {
            Button btn = new Button();
            btn.Tag = path;
            btn.Click += btn_Click;
            StackPanel spanel = new StackPanel() { Orientation = Orientation.Vertical};
            spanel.Children.Add(Ue);
            spanel.Children.Add(new TextBlock()
            {
                Text=text,
                Margin=new Thickness(5,3,0,0),
                HorizontalAlignment=HorizontalAlignment.Center,
                VerticalAlignment=VerticalAlignment.Center
            }
            );
            btn.Content = spanel;
            return btn;
        }

        static void btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = ((Button)sender).Tag as string;
                if (path != null)
                    Process.Start(path);
            }
            catch
            {
            }
        }

    }
}
