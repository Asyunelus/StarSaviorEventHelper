using EndoAshu.StarSavior.Core.Search.Data;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;

namespace EndoAshu.StarSavior.Core.Search.Defaults
{
    public sealed class V0_2_Beta_SearchEngine : AbstractSearchEngine
    {
        public override bool IsRecommend => DataServer.VERSION_CODE < 0_000_003_00;

        public V0_2_Beta_SearchEngine() : base("v0.2-beta", "아르카나 카드 인식 로직을 일부 최적화하여 더 빠르고 낮은 성능으로 사용할 수 있게 개선한 버전입니다.")
        {
        }

        protected override async Task<SearchResult> InternalSearch(AbstractOCRReader reader, IntPtr window, ResolutionType resType, RECT rect)
        {
            RECT evTypeRect = GetEventTypeRect(resType, rect);
            string evType = reader.Capture(evTypeRect, 90).Replace(" ", "");
            if (evType.Contains("이벤트"))
            {

                RECT markRect = GetEventIcon(resType, rect);
                using (Bitmap mark = reader.CaptureBitmap(markRect))
                {
                    if (evType.Contains("여정"))
                    {
                        return await SearchJourney(reader, window, resType, rect);
                    }
                    else if (evType.Contains("아르카나"))
                    {
                        return await SearchArcana(reader,window, resType, rect);
                    }
                    else
                    {
                        return new SearchResult(SearchResultType.Failed_NotFoundEventType, evType);
                    }
                }
            }
            else
            {
                return new SearchResult(SearchResultType.Failed_NotEventOnScreen);
            }
        }

        protected override Task<SearchResult> FindCardEventAsync(AbstractOCRReader reader, Bitmap cardImage, string eventName, params string[] eventSelect)
        {
            return StaticFindCardEventAsync(reader, cardImage, eventName, eventSelect);
        }

        public static async Task<SearchResult> StaticFindCardEventAsync(AbstractOCRReader reader, Bitmap cardImage, string eventName, params string[] eventSelect)
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

            var searchTasks = searchFirst.Take(5).Select(tuple => ((Bitmap)cardImage.Clone(), tuple)).Select(async item =>
            {
                Bitmap bitmap = item.Item1;
                int idx = item.tuple.idx;
                double nameSim = item.tuple.nameSim;
                CardDataWrapper card = item.tuple.card;
                return await Task.Run(() =>
                {
                    int imageScore = ImageMatcher.IsMatch(bitmap, $"./{card.Image}");
                    bitmap.Dispose();
                    return new CardSearchResult(idx, nameSim, imageScore, card);
                });
            });
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
