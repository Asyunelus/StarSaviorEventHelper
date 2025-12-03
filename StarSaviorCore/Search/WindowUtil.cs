using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace EndoAshu.StarSavior.Core.Search
{
    public static class WindowUtil
    {
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        public static IntPtr FindTargetStartsWith(string startWithTitle)
        {
            IntPtr foundHwnd = IntPtr.Zero;

            EnumWindows((hWnd, lParam) =>
            {
                StringBuilder sb = new StringBuilder(256);

                if (GetWindowText(hWnd, sb, 256) > 0)
                {
                    string windowTitle = sb.ToString();

                    if (windowTitle.StartsWith(startWithTitle, StringComparison.OrdinalIgnoreCase))
                    {
                        foundHwnd = hWnd;
                        return false;
                    }
                }
                return true;
            }, IntPtr.Zero);

            return foundHwnd;
        }

        public static IntPtr FindTarget(string name)
        {
            return FindWindow(null!, name);
        }

        public static RECT GetRect(IntPtr window)
        {
            RECT currentClientRect;
            GetClientRect(window, out currentClientRect);

            int currentW = currentClientRect.Width;
            int currentH = currentClientRect.Height;

            Point clientOrigin = new Point(0, 0);
            ClientToScreen(window, ref clientOrigin);

            return new RECT(clientOrigin.X, clientOrigin.Y, clientOrigin.X + currentW, clientOrigin.Y + currentH);
        }
    }
}
