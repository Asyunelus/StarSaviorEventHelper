using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndoAshu.StarSavior.Core
{
    using System.Drawing;

    public static class ResolutionConverter
    {
        public static RECT GetResponsiveRect(RECT originalRect, int baseWidth, int baseHeight, int currentW, int currentH, bool isUiCentered = false)
        {
            float scale = (float)currentH / baseHeight;

            int newX = (int)(originalRect.X * scale);
            int newY = (int)(originalRect.Y * scale);
            int newWidth = (int)(originalRect.Width * scale);
            int newHeight = (int)(originalRect.Height * scale);

            return new RECT(newX, newY, newWidth + newX, newHeight + newY);
        }
    }
}
