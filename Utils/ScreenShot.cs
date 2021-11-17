using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace D2RMuler.Utils
{
    internal class ScreenShot
    {
        public static string CaptureApplication(string procName)
        {
            var processArray = Process.GetProcessesByName(procName);
            if (processArray == null || processArray.Length == 0)
            {
                //TODO: Throw exception
                return "";
            }
            else
            {
                var proc = Process.GetProcessesByName(procName)[0];
                var rect = new User32.Rect();
                IntPtr intPtr = User32.GetWindowRect(proc.MainWindowHandle, ref rect);

                int width = rect.right - rect.left;
                int height = rect.bottom - rect.top;

                var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                using (Graphics graphics = Graphics.FromImage(bmp))
                {
                    graphics.DrawLine(new Pen(Color.FromArgb(255, Color.Red), 3), new Point(rect.left, rect.top), new Point(rect.right, rect.top));
                    graphics.CopyFromScreen(rect.left, rect.top, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
                }

                string name = Guid.NewGuid().ToString().Replace("-", "") + ".png";

                if (!Directory.Exists(Const.dir + "/test"))
                {
                    Directory.CreateDirectory(Const.dir + "/test");
                }
                bmp.Save(string.Format(Const.dir + "/test/{0}", name), ImageFormat.Png);
                bmp.Dispose();

                Debug.WriteLine(String.Format("Was file '{0}' saved?", name) + File.Exists(string.Format(AppDomain.CurrentDomain.BaseDirectory + "/test/{0}", name)));

                return AppDomain.CurrentDomain.BaseDirectory + "/test/" + name;
            }
        }

        private class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct Rect
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }

            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);
        }
    }
}
