using EndoAshu.StarSavior.Core.Search.Data;
using System.Drawing;

namespace EndoAshu.StarSavior.Core.Search.Defaults
{
    public sealed class V0_2_Beta_Powered_V0_1_2_SearchEngine : AbstractSearchEngine
    {
        public override bool IsRecommend => DataServer.VERSION_CODE < 0_000_003_00;

        public V0_2_Beta_Powered_V0_1_2_SearchEngine() : base("v0.2-beta-powered-v0.1.2-beta", "v0.1.2-beta를 기반으로 수정한 검색 엔진입니다.\n아르카나 카드 인식 로직을 일부 최적화하여 더 빠르고 낮은 성능으로 사용할 수 있게 개선한 버전입니다.")
        {
        }

        protected override async Task<SearchResult> InternalSearch(AbstractOCRReader reader, IntPtr window, ResolutionType resType, RECT rect)
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

        protected override Task<SearchResult> FindCardEventAsync(AbstractOCRReader reader, Bitmap cardImage, string eventName, params string[] eventSelect)
        {
            return V0_2_Beta_SearchEngine.StaticFindCardEventAsync(reader, cardImage, eventName, eventSelect);
        }
    }
}
