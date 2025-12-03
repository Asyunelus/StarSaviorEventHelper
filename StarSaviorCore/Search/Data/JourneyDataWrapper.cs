using EndoAshu.StarSavior.Core.DataSet;
using System.Text.Json.Serialization;

namespace EndoAshu.StarSavior.Core.Search.Data
{
    public class JourneyDataWrapper
    {
        private readonly JourneyDataInner _data;

        public string Category => _data.Category;
        public string Name => _data.Name;
        public string Timing => _data.Timing;
        public string Condition => _data.Condition;
        public JourneyChoice[] Choices => _data.Choices;

        public JourneyDataWrapper(JourneyDataInner data)
        {
            _data = data;
        }
    }
}
