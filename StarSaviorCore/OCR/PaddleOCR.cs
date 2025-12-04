using PaddleOCRSharp;
using System.Drawing;

namespace EndoAshu.StarSavior.Core
{
    public class PaddleOCR : AbstractOcrReader
    {
        private PaddleOCREngine? engine;

        public PaddleOCR(string modelPathroot)
        {
            OCRModelConfig config = new OCRModelConfig();
            config.det_infer = Path.GetFullPath(Path.Combine(modelPathroot, "det_infer"));
            config.cls_infer = Path.GetFullPath(Path.Combine(modelPathroot, "cls_infer"));
            config.rec_infer = Path.GetFullPath(Path.Combine(modelPathroot, "rec_infer_kor"));
            OCRParameter oCRParameter = new OCRParameter();
            engine = new PaddleOCREngine(config, oCRParameter);
        }

        public override string OnProcess(Bitmap bitmap)
        {
            if (engine != null)
            {
                var res = engine!.DetectText(bitmap);
                return res.Text;
            }
            return string.Empty;
        }

        public override void Dispose()
        {
            base.Dispose();
            engine?.Dispose();
            engine = null;
        }
    }
}
