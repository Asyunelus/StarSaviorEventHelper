using EndoAshu.StarSavior.Core.Search.Defaults;

namespace EndoAshu.StarSavior.Core.Search
{
    public class SearchEngine
    {
        public static AbstractSearchEngine? Current;

        private static readonly Dictionary<string, AbstractSearchEngine> _engines = new Dictionary<string, AbstractSearchEngine>();

        public static ICollection<AbstractSearchEngine> Items => _engines.Values;
        private static TesseractOCR? _tesseractOCR;
        private static PaddleOCR? _paddleOCR;


        static SearchEngine()
        {
        }

        public static void Initialize() { 
            _tesseractOCR = new TesseractOCR("./tdata");
            _paddleOCR = new PaddleOCR("./pdata");
            
            var defaultRecommend = new V0_2_Fast_Beta_SearchEngine(_paddleOCR);

            Register(defaultRecommend);
            Register(new V0_2_Fast_Alpha_SearchEngine(_tesseractOCR));
            Register(new V0_2_Beta_SearchEngine(_tesseractOCR));
            Register(new V0_2_Beta_Powered_V0_1_2_SearchEngine(_tesseractOCR));
            Register(new V0_2_Beta_Powered_V0_1_SearchEngine(_tesseractOCR));
            Register(new V0_1_2_Beta_SearchEngine(_tesseractOCR));
            Register(new V0_1_Beta_SearchEngine(_tesseractOCR));

            Current = Settings.Engine ?? defaultRecommend;
        }

        ~SearchEngine()
        {
            _tesseractOCR?.Dispose();
            _tesseractOCR = null;
        }

        private static void Register(AbstractSearchEngine engine) {
            _engines[engine.Name] = engine;
        }
    }
}
