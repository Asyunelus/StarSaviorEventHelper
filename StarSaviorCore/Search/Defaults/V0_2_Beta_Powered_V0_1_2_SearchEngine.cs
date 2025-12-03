using EndoAshu.StarSavior.Core.Search.Data;
using System.Drawing;

namespace EndoAshu.StarSavior.Core.Search.Defaults
{
    public sealed class V0_2_Beta_Powered_V0_1_2_SearchEngine : AbstractSearchEngine
    {
        public V0_2_Beta_Powered_V0_1_2_SearchEngine() : base("v0.2-beta-powered-v0.1.2-beta", "v0.1.2-beta를 기반으로 수정한 검색 엔진입니다.\n아르카나 카드 인식 로직을 일부 최적화하여 더 빠르고 낮은 성능으로 사용할 수 있게 개선한 버전입니다.")
        {
        }

        protected override async Task<SearchResult> InternalSearch(OcrReader reader, IntPtr window, ResolutionType resType, RECT rect)
        {
            RECT evTypeRect = GetEventTypeRect(resType, rect);
            string evType = reader.Capture(evTypeRect, 150).Replace(" ", "");
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
                        return await SearchArcana(reader, window, resType, rect);
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

            return await V0_2_Beta_SearchEngine.FindCardEventAsync(cardBitmap, eventName, select1, select2);
        }
    }
}
