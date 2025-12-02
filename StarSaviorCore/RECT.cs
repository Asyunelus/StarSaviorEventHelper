using System.Runtime.InteropServices;

namespace EndoAshu.StarSavior.Core
{

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public int Width => Right - Left;
        public int Height => Bottom - Top;

        public int X => Left;
        public int Y => Top;

        public RECT(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public RECT() : this(0, 0, 0, 0)
        {

        }

        public static RECT operator +(RECT p1, RECT p2)
        {
            return new RECT(
                p1.Left + p2.Left,
                p1.Top + p2.Top,
                p1.Right + p2.Right,
                p1.Bottom + p2.Bottom
            );
        }

        public void AddPos(RECT rect)
        {
            Left += rect.X;
            Right += rect.X;
            Top += rect.Y;
            Bottom += rect.Y;
        }
    }
}
