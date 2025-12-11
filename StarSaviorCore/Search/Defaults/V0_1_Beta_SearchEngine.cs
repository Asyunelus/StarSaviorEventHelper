using EndoAshu.StarSavior.Core.Search.Data;
using System.Drawing;

namespace EndoAshu.StarSavior.Core.Search.Defaults
{
    public sealed class V0_1_Beta_SearchEngine : AbstractSearchEngine
    {
        public V0_1_Beta_SearchEngine() : base("v0.1-beta")
        {
        }

        protected override async Task<SearchResult> InternalSearch(AbstractOCRReader reader, IntPtr window, ResolutionType resType, RECT rect)
        {
            RECT evTypeRect = GetEventTypeRect(resType, rect);
            string evType = reader.Capture(evTypeRect);
            if (
                evType.Contains("이")
                || evType.Contains("벤")
                || evType.Contains("트")
            )
            {

                RECT markRect = GetEventIcon(resType, rect);
                using (Bitmap mark = reader.CaptureBitmap(markRect))
                {
                    int match = ImageMatcher.IsMatch(mark, "./images/detect/na.png");
                    if (match >= 5 || evType.Contains("여정"))
                    {
                        return await SearchJourney(reader, window, resType, rect);
                    }
                    else
                    {
                        return await SearchArcana(reader, window, resType, rect);
                    }
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
            var searchTasks = EventLoader.ArcanaCards.Where(e => !string.IsNullOrEmpty(e.Image)).Select(e => ((Bitmap)cardImage.Clone(), e)).Select(async dat =>
            {
                Bitmap bitmap = dat.Item1;
                CardDataWrapper card = dat.e;
                return await Task.Run(() =>
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
                    int imageScore = ImageMatcher.IsMatch(bitmap, $"./{card.Image}");
                    bitmap.Dispose();
                    return new CardSearchResult(idx, nameSim, imageScore, dat.e);
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
