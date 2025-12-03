using EndoAshu.StarSavior.Core.Search.Data;

namespace EndoAshu.StarSavior.Core.Search
{
    public class CardSearchResult
    {
        public int EventIndex { get; set; }
        public double NameMatch { get; }
        public int ImageMatch { get; }
        public CardDataWrapper? Data { get; }

        public CardSearchResult(int eventIdx, double nameMatch, int imageMatch, CardDataWrapper? data)
        {
            EventIndex = eventIdx;
            NameMatch = nameMatch;
            ImageMatch = imageMatch;
            Data = data;
        }
    }

    public class JourneySearchResult
    {
        public double NameMatch { get; }
        public JourneyDataWrapper? Data { get; }

        public JourneySearchResult(double nameMatch, JourneyDataWrapper? data)
        {
            NameMatch = nameMatch;
            Data = data;
        }
    }
}
