using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Win8SliderBar
{
    public static class IconHelper
    {
        public static BitmapSource GetExeIcon(string exePath)
        {
            Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(exePath);
            BitmapSource bi = Imaging.CreateBitmapSourceFromHBitmap(icon.ToBitmap().GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            return bi;
        }
    }
}
