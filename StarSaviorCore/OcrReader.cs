using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Tesseract;

namespace EndoAshu.StarSavior.Core {

#pragma warning disable CA1416
    public class OcrReader : IDisposable
    {
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

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);


        private TesseractEngine? engine;

        public OcrReader(string dataPath)
        {
            engine = new TesseractEngine(dataPath, "kor+eng", EngineMode.Default);
            engine.DefaultPageSegMode = PageSegMode.SingleLine;
        }

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

        public Bitmap CaptureBitmap(RECT rect)
        {
            int targetW = rect.Right - rect.Left;
            int targetH = rect.Bottom - rect.Top;
            Bitmap rawBmp = new Bitmap(targetW, targetH);
            using (Graphics g = Graphics.FromImage(rawBmp))
            {
                g.CopyFromScreen(rect.Left, rect.Top, 0, 0, rawBmp.Size);
            }

            return rawBmp;
        }
        public Bitmap CaptureBitmap(RECT rect, float angle, int targetW = 600)
        {
            int w = rect.Right - rect.Left;
            int h = rect.Bottom - rect.Top;

            using (Bitmap raw = new Bitmap(w, h))
            {
                using (Graphics g = Graphics.FromImage(raw))
                {
                    g.CopyFromScreen(rect.Left, rect.Top, 0, 0, raw.Size);
                }

                float scale = (float)targetW / w;
                int targetH = (int)(h * scale);

                using (Bitmap resized = new Bitmap(targetW, targetH))
                {
                    resized.SetResolution(raw.HorizontalResolution, raw.VerticalResolution);

                    using (Graphics g = Graphics.FromImage(resized))
                    {
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(raw, 0, 0, targetW, targetH);
                    }

                    Bitmap rotated = new Bitmap(targetW, targetH);
                    rotated.SetResolution(resized.HorizontalResolution, resized.VerticalResolution);

                    using (Graphics g = Graphics.FromImage(rotated))
                    {
                        g.TranslateTransform(targetW / 2, targetH / 2);
                        g.RotateTransform(angle);
                        g.TranslateTransform(-targetW / 2, -targetH / 2);

                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(resized, new Point(0, 0));
                    }

                    return rotated;
                }
            }
        }
        public string Capture(RECT rect, Action<Bitmap>? debug = null)
        {
            using (Bitmap rawBmp = CaptureBitmap(rect))
            {
                float targetHeight = 60.0f;
                float scaleFactor = targetHeight / rawBmp.Height;

                int scale = (int)Math.Ceiling(scaleFactor);
                if (scale < 1) scale = 1;

                using (Bitmap resizeBmp = ResizeImage(rawBmp, scale))
                using (Bitmap bmp = MakeGrayscale(resizeBmp))
                {
                    Binarize(bmp, 90);

                    debug?.Invoke(bmp);

                    using (var page = engine.Process(bmp))
                    {
                        return FilterOCRNoise(page.GetText());
                    }
                }
            }
        }

        public static string FilterOCRNoise(string rawText)
        {
            string cleaned = Regex.Replace(rawText, @"[^가-힣A-Za-z0-9\s]", "");
            cleaned = Regex.Replace(cleaned, @"\s+", " ").Trim();
            return cleaned;
        }

        public static Bitmap MakeGrayscale(Bitmap original)
        {
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);
            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                ColorMatrix colorMatrix = new ColorMatrix(
                    new float[][]
                    {
                        new float[] {-1,  0,  0,  0,  0},
                        new float[] { 0, -1,  0,  0,  0},
                        new float[] { 0,  0, -1,  0,  0},
                        new float[] { 0,  0,  0,  1,  0},
                        new float[] { 1,  1,  1,  0,  1} 
                    }
                );

                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(colorMatrix);

                g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                    0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
            }
            return newBitmap;
        }

        public static Bitmap ResizeImage(Bitmap original, int scaleFactor)
        {
            int w = original.Width * scaleFactor;
            int h = original.Height * scaleFactor;
            Bitmap newBitmap = new Bitmap(w, h);

            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(original, 0, 0, w, h);
            }
            return newBitmap;
        }

        public static void Binarize(Bitmap bmp, int threshold)
        {
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            unsafe
            {
                byte* ptr = (byte*)bmpData.Scan0;
                int bytes = Math.Abs(bmpData.Stride) * bmp.Height;

                for (int i = 0; i < bytes; i += 4)
                {
                    byte gray = (byte)(ptr[i] * 0.11 + ptr[i + 1] * 0.59 + ptr[i + 2] * 0.3);
                    byte binary = (byte)(gray > threshold ? 255 : 0);

                    ptr[i] = binary;     
                    ptr[i + 1] = binary; 
                    ptr[i + 2] = binary; 
                    ptr[i + 3] = 255;    
                }
            }
            bmp.UnlockBits(bmpData);
        }

        public void Dispose()
        {
            if (engine != null)
            {
                engine.Dispose();
            }
            engine = null;
        }
    }
#pragma warning restore CA1416
}
