using EndoAshu.StarSavior.Core.Search.Data;
using System.Drawing;

namespace EndoAshu.StarSavior.Core.Search.Defaults
{
    public sealed class V0_1_Beta_SearchEngine : AbstractSearchEngine
    {
        public V0_1_Beta_SearchEngine() : base("v0.1-beta")
        {
        }

        protected override async Task<SearchResult> InternalSearch(OcrReader reader, IntPtr window, ResolutionType resType, RECT rect)
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

        private async Task<SearchResult> SearchJourney(OcrReader reader, IntPtr window, ResolutionType resType, RECT rect)
        {
            RECT eventNameRect = GetEventNameRect(resType, rect);
            string eventName = reader.Capture(eventNameRect);
            var found = await FindJourneyEventAsync(eventName);
            if (found != null)
            {
                return new SearchResult(found);
            }
            else
            {
                return new SearchResult(SearchResultType.Failed_NotFoundJourneyData, eventName);
            }
        }


        public static async Task<JourneySearchResult?> FindJourneyEventAsync(string eventName)
        {
            return await Task.Run(() =>
            {
                var search = EventLoader.JourneyDatas.Select(dat =>
                {
                    double sim = HangulCompare.GetHangulSimilarity(eventName, dat.Name);
                    return (sim, dat);
                }).OrderByDescending(e => e.sim).ToList();

                if (search.Count > 0)
                {
                    if (search[0].sim > 0.7)
                    {
                        return new JourneySearchResult(search[0].sim, search[0].dat);
                    }
                }

                return null;
            });
        }

        private async Task<SearchResult> SearchArcana(OcrReader reader, IntPtr window, ResolutionType resType, RECT rect)
        {
            RECT eventNameRect = GetEventNameRect(resType, rect);
            string eventName = reader.Capture(eventNameRect);

            RECT select1Rect = GetEventSelect1(resType, rect);
            string select1 = reader.Capture(select1Rect);

            RECT select2Rect = GetEventSelect2(resType, rect);
            string select2 = reader.Capture(select2Rect);

            RECT cardImg = GetCardRect(resType, rect);
            using Bitmap cardBitmap = reader.CaptureBitmap(cardImg, 11);

            return await FindCardEventAsync(cardBitmap, eventName, select1, select2);
        }

        public static async Task<SearchResult> FindCardEventAsync(Bitmap cardImage, string eventName, params string[] eventSelect)
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
