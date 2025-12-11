using EndoAshu.StarSavior.Core.Search.Data;
using OpenCvSharp;
using System.Drawing;

namespace EndoAshu.StarSavior.Core.Search.Defaults
{
    public class V0_3_Beta_SearchEngine : AbstractSearchEngine
    {
        public override bool IsRecommend => false;
        public override bool IsExperimental => true;
        public override string OCREngineId => PaddleOCR.OCR_ID;

        public V0_3_Beta_SearchEngine() : base("v0.3-beta", "v0.2-fast-beta에서 개선된 엔진입니다. 불안정할 수 있습니다.")
        {
        }

        protected override async Task<SearchResult> InternalSearch(AbstractOCRReader reader, IntPtr window, ResolutionType resType, RECT rect)
        {
            RECT evTypeRect = GetEventTypeRect(resType, rect);
            string evType = reader.Capture(evTypeRect, 150).Replace(" ", "");
            if (evType.Contains("이벤트"))
            {
                if (evType.Contains("여정") || evType.Contains("날씨") || evType.Contains("토벌") || evType.Contains("원정"))
                {
                    return await SearchJourney(reader, window, resType, rect);
                }
                else if (evType.Contains("아르카나"))
                {
                    return await SearchArcana(reader, window, resType, rect);
                }
                else
                {
                    return new SearchResult(SearchResultType.Failed_NotFoundEventType, evType);
                }
            }
            else
            {
                return new SearchResult(SearchResultType.Failed_NotEventOnScreen);
            }
        }

        protected override async Task<SearchResult> FindCardEventAsync(AbstractOCRReader reader, Bitmap cardImage, string eventName, params string[] eventSelect)
        {
#pragma warning disable CA1416
            var searchFirst = EventLoader.ArcanaCards.Where(e => !string.IsNullOrEmpty(e.Image)).Select(card =>
            {
                int idx = -1;
                double nameSim = double.MinValue;
                foreach (var entry in card.CardEvents)
                {
                    var ev = entry.Value;
                    double current = HangulCompare.GetHangulSimilarity(eventName, ev.Name);
                    if (current > nameSim)
                    {
                        nameSim = current;
                        idx = entry.Key;
                    }
                }

                return (idx, nameSim, card);
            }).OrderByDescending(e => e.nameSim).ToList();

            if (searchFirst.Count <= 0)
            {
                return new SearchResult(SearchResultType.Failed_NotFoundArcanaData, eventName);
            }

            using Mat mat = ImageMatcher.PrepareScreenMat(cardImage)!;

            var searchTasks = searchFirst.Take(3).Select(tuple => Task.Run(() => {
                int idx = tuple.idx;
                double nameSim = tuple.nameSim;
                CardDataWrapper card = tuple.card;

                int imageScore = ImageMatcher.IsMatchMat(mat, $"./{card.Image}");
                return new CardSearchResult(idx, nameSim, imageScore, card);
            }));
#pragma warning restore CA1416

            var allResults = await Task.WhenAll(searchTasks);

            List<CardSearchResult> searchData = allResults.Where(e => e.ImageMatch > 10).ToList();

            if (searchData.Count <= 0)
            {
                return new SearchResult(SearchResultType.Failed_NotFoundArcanaData, eventName);
            }

            float max = (float)searchData.Max(e => e.ImageMatch);

            List<CardSearchResult> data = searchData.OrderByDescending(e => e.ImageMatch / max + e.NameMatch).ToList();
            return new SearchResult(data[0]);
        }
    }
}
