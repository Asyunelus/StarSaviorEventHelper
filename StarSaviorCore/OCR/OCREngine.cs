namespace EndoAshu.StarSavior.Core.OCR
{
    public static class OCREngine
    {
        public static AbstractOCRReader? CurrentReader { get; private set; }
        private static string? _currentEngineId;

        // 엔진 교체 메서드
        public static async Task UpdateEngineAsync(string engineId)
        {
            if (_currentEngineId == engineId && CurrentReader != null)
            {
                return;
            }
            if (CurrentReader != null)
            {
                CurrentReader.Dispose();
                CurrentReader = null;
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            await Task.Delay(50);

            CurrentReader = await InternalLoad(engineId);
            _currentEngineId = engineId;
        }

        private static async Task<AbstractOCRReader?> InternalLoad(string id)
        {
            await Task.Delay(1);
            return id switch
            {
                TesseractOCR.OCR_ID => new TesseractOCR("./tdata"),
                //PaddleOCR.OCR_ID => new TesseractOCR("./tdata"),
                PaddleOCR.OCR_ID => new PaddleOCR("./pdata"),
                _ => null
            };
        }
    }
}