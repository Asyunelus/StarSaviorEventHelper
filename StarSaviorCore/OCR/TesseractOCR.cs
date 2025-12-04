using System.Drawing;
using System.Text.RegularExpressions;
using Tesseract;

namespace EndoAshu.StarSavior.Core
{
    public sealed class TesseractOCR : AbstractOcrReader
    {
        private TesseractEngine? engine;
        public TesseractOCR(string dataPath) : base()
        {
            engine = new TesseractEngine(dataPath, "kor+eng", EngineMode.Default);
            engine.DefaultPageSegMode = PageSegMode.SingleLine;
        }

        public override string OnProcess(Bitmap bitmap)
        {
            using (var page = engine.Process(bitmap))
            {
                return FilterOCRNoise(page.GetText());
            }
        }

        public static string FilterOCRNoise(string rawText)
        {
            string cleaned = Regex.Replace(rawText, @"[^가-힣A-Za-z0-9\s]", "");
            cleaned = Regex.Replace(cleaned, @"\s+", " ").Trim();
            return cleaned;
        }

        public override void Dispose()
        {
            base.Dispose();
            if (engine != null)
            {
                engine.Dispose();
            }
            engine = null;
        }
    }
}
