using EndoAshu.StarSavior.Core.Search.Defaults;

namespace EndoAshu.StarSavior.Core.Search
{
    public class SearchEngine
    {
        public static AbstractSearchEngine? Current = new V0_2_Fast_Alpha_SearchEngine();

        private static readonly Dictionary<string, AbstractSearchEngine> _engines = new Dictionary<string, AbstractSearchEngine>();

        public static ICollection<AbstractSearchEngine> Items => _engines.Values;

        static SearchEngine()
        {
            Register(new V0_2_Fast_Alpha_SearchEngine());
            Register(new V0_2_Beta_SearchEngine());
            Register(new V0_2_Beta_Powered_V0_1_2_SearchEngine());
            Register(new V0_2_Beta_Powered_V0_1_SearchEngine());
            Register(new V0_1_2_Beta_SearchEngine());
            Register(new V0_1_Beta_SearchEngine());
        }

        private static void Register(AbstractSearchEngine engine) {
            _engines[engine.Name] = engine;
        }
    }
}
