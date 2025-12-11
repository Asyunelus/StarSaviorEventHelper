using EndoAshu.StarSavior.Core.OCR;
using EndoAshu.StarSavior.Core.Search.Defaults;

namespace EndoAshu.StarSavior.Core.Search
{
    public class SearchEngine
    {
        public static AbstractSearchEngine? _current;
        public static AbstractSearchEngine? Current
        {
            get => _current;
            set
            {
                if (_current != value)
                {
                    _current = value;
                    if (_current != null)
                    {
                        _ = LoadOCREngineSafe(_current.OCREngineId);
                    }
                }
            }
        }
        private static async Task LoadOCREngineSafe(string ocrId)
        {
            try
            {
                await OCREngine.UpdateEngineAsync(ocrId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OCR Engine Load Failed: {ex.Message}");
            }
        }

        private static readonly Dictionary<string, AbstractSearchEngine> _engines = new Dictionary<string, AbstractSearchEngine>();

        public static ICollection<AbstractSearchEngine> Items => _engines.Values;

        static SearchEngine()
        {
        }

        public static void Initialize() { 
            
            var defaultRecommend = new V0_2_Fast_Beta_SearchEngine();

            Register(defaultRecommend);
            Register(new V0_2_Fast_Alpha_SearchEngine());
            Register(new V0_3_Beta_SearchEngine());
            Register(new V0_2_Beta_SearchEngine());
            Register(new V0_2_Beta_Powered_V0_1_2_SearchEngine());
            Register(new V0_2_Beta_Powered_V0_1_SearchEngine());
            Register(new V0_1_2_Beta_SearchEngine());
            Register(new V0_1_Beta_SearchEngine());

            Current = Settings.Engine ?? defaultRecommend;
        }

        ~SearchEngine()
        {
        }

        private static void Register(AbstractSearchEngine engine) {
            _engines[engine.Name] = engine;
        }
    }
}
