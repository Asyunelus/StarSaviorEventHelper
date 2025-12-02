using System.Drawing;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace EndoAshu.StarSavior.Core
{

    public static class ImageMatcher
    {
        private static readonly Dictionary<string, Mat> _targets = new Dictionary<string, Mat>();

        public static int IsMatchLegacy(Bitmap screenBitmap, string targetFilePath)
        {
            if (screenBitmap == null || string.IsNullOrEmpty(targetFilePath)) return -1;
            if (!System.IO.File.Exists(targetFilePath)) return -1;

            using var detector = AKAZE.Create();
            using var matcher = new BFMatcher(NormTypes.Hamming);

            using var matScreenRaw = screenBitmap.ToMat();
            using var matScreen = new Mat();
            if (matScreenRaw.Channels() > 1)
                Cv2.CvtColor(matScreenRaw, matScreen, ColorConversionCodes.BGR2GRAY);
            else
                matScreenRaw.CopyTo(matScreen);

            if (!_targets.ContainsKey(targetFilePath))
            {

                using var matTarget = Cv2.ImRead(targetFilePath, ImreadModes.Grayscale);
                if (matTarget.Empty()) return -1;
                var st = ResizeMat(matTarget, 600);
                _targets[targetFilePath] = st;
            }

            var smallTarget = _targets[targetFilePath];
            using var smallScreen = ResizeMat(matScreen, 600);

            using var descriptorsTarget = new Mat();
            using var descriptorsScreen = new Mat();
            KeyPoint[] kpsTarget, kpsScreen;

            detector.DetectAndCompute(smallTarget, null, out kpsTarget, descriptorsTarget);
            detector.DetectAndCompute(smallScreen, null, out kpsScreen, descriptorsScreen);

            if (descriptorsTarget.Empty() || descriptorsScreen.Empty() || kpsTarget.Length < 5 || kpsScreen.Length < 5)
                return -1;

            var matches = matcher.KnnMatch(descriptorsTarget, descriptorsScreen, k: 2);

            int goodMatchCount = matches.Count(x => x.Length >= 2 && x[0].Distance < 0.75 * x[1].Distance);

            return goodMatchCount;
        }

        public static int IsMatch(Bitmap screenBitmap, string targetFilePath)
        {
            if (screenBitmap == null || string.IsNullOrEmpty(targetFilePath)) return -1;
            if (!System.IO.File.Exists(targetFilePath)) return -1;

            using var detector = AKAZE.Create();
            using var matcher = new BFMatcher(NormTypes.Hamming);

            using var matTarget = Cv2.ImRead(targetFilePath, ImreadModes.Grayscale);
            if (matTarget.Empty()) return -1;

            using var matScreenRaw = screenBitmap.ToMat();
            using var matScreen = new Mat();
            if (matScreenRaw.Channels() > 1)
                Cv2.CvtColor(matScreenRaw, matScreen, ColorConversionCodes.BGR2GRAY);
            else
                matScreenRaw.CopyTo(matScreen);

            using var smallTarget = ResizeMat(matTarget, 600);
            using var smallScreen = ResizeMat(matScreen, 600);

            using var desTarget = new Mat();
            using var desScreen = new Mat();
            KeyPoint[] kpsTarget, kpsScreen;

            detector.DetectAndCompute(smallTarget, null, out kpsTarget, desTarget);
            detector.DetectAndCompute(smallScreen, null, out kpsScreen, desScreen);

            if (desTarget.Empty() || desScreen.Empty() || kpsTarget.Length < 4 || kpsScreen.Length < 4)
                return -1;

            var matches = matcher.KnnMatch(desTarget, desScreen, k: 2);

            var goodMatches = matches
                .Where(x => x.Length >= 2 && x[0].Distance < 0.75 * x[1].Distance)
                .Select(x => x[0])
                .ToList();

            if (goodMatches.Count < 4) return -1;

            var srcPoints = goodMatches.Select(m => kpsTarget[m.QueryIdx].Pt).ToArray();
            var dstPoints = goodMatches.Select(m => kpsScreen[m.TrainIdx].Pt).ToArray();

            using var mask = new Mat();
            Cv2.FindHomography(
                InputArray.Create(srcPoints),
                InputArray.Create(dstPoints),
                HomographyMethods.Ransac,
                ransacReprojThreshold: 5.0,
                mask: mask
            );

            int finalScore = Cv2.CountNonZero(mask);
            return finalScore;
        }

        private static Mat ResizeMat(Mat src, int maxWidth)
        {
            if (src.Width <= maxWidth) return src.Clone();

            double scale = (double)maxWidth / src.Width;
            var newSize = new OpenCvSharp.Size(src.Width * scale, src.Height * scale);

            var dst = new Mat();
            Cv2.Resize(src, dst, newSize, 0, 0, InterpolationFlags.Linear);
            return dst;
        }
    }
}
