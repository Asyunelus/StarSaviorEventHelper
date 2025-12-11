using EndoAshu.StarSavior.Core.OCR;
using System.Drawing;
using System.Reflection.PortableExecutable;

namespace EndoAshu.StarSavior.Core.Search
{
    public abstract class AbstractSearchEngine
    {
        public virtual bool IsRecommend => false;
        public virtual bool IsExperimental => false;
        public virtual string OCREngineId => TesseractOCR.OCR_ID;

        public string Name { get; }
        public string Description { get; }

        public AbstractSearchEngine(string engineName) : this(engineName, $"\"{engineName}\" 버전 검색 엔진입니다.")
        {

        }

        public AbstractSearchEngine(string engineName, string description)
        {
            Name = engineName;
            Description = description;
        }

        public async Task<SearchResult> Search(int timeoutMilliseconds, params string[] windowNames)
        {
            var reader = OCREngine.CurrentReader;

            if (reader == null)
            {
                return new SearchResult(SearchResultType.Failed_OCREngineNotSet);
            }

            IntPtr window = IntPtr.Zero;
            foreach (string wnName in windowNames)
            {
                window = WindowUtil.FindTargetStartsWith(wnName);
                if (window != IntPtr.Zero)
                {
                    break;
                }
            }

            if (window != IntPtr.Zero)
            {
                RECT rect = WindowUtil.GetRect(window);
                float res = (float)rect.Width / rect.Height;
                ResolutionType resType = res <= 1.9f ? (res <= 1.61f ? ResolutionType.S16_10 : ResolutionType.S16_9) : ResolutionType.S21_9;

                var task = InternalSearch(reader, window, resType, rect);
                var timeoutTask = Task.Delay(timeoutMilliseconds);

                var completedTask = await Task.WhenAny(task, timeoutTask);

                if (completedTask == timeoutTask)
                {
                    return new SearchResult(SearchResultType.Failed_Timeout);
                }
                else
                {
                    return await task;
                }
            }
            else
            {
                return new SearchResult(SearchResultType.Failed_NotFoundWindow);
            }
        }

        protected abstract Task<SearchResult> InternalSearch(AbstractOCRReader reader, IntPtr window, ResolutionType resolutionType, RECT windowRect);
        protected virtual async Task<SearchResult> SearchJourney(AbstractOCRReader reader, IntPtr window, ResolutionType resType, RECT rect)
        {
            RECT eventNameRect = GetEventNameRect(resType, rect);
            string eventName = reader.Capture(eventNameRect);
            var found = await FindJourneyEventAsync(reader, eventName);
            if (found != null)
            {
                return new SearchResult(found);
            }
            else
            {
                return new SearchResult(SearchResultType.Failed_NotFoundJourneyData, eventName);
            }
        }


        protected virtual async Task<JourneySearchResult?> FindJourneyEventAsync(AbstractOCRReader reader, string eventName)
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

        protected virtual async Task<SearchResult> SearchArcana(AbstractOCRReader reader, IntPtr window, ResolutionType resType, RECT rect)
        {
            RECT eventNameRect = GetEventNameRect(resType, rect);
            string eventName = reader.Capture(eventNameRect);

            RECT select1Rect = GetEventSelect1(resType, rect);
            string select1 = reader.Capture(select1Rect);

            RECT select2Rect = GetEventSelect2(resType, rect);
            string select2 = reader.Capture(select2Rect);

            RECT cardImg = GetCardRect(resType, rect);
            using Bitmap cardBitmap = reader.CaptureBitmap(cardImg, 11);

            return await FindCardEventAsync(reader, cardBitmap, eventName, select1, select2);
        }

        protected abstract Task<SearchResult> FindCardEventAsync(AbstractOCRReader reader, Bitmap cardImage, string eventName, params string[] eventSelect);


        protected RECT GetCardRect(ResolutionType type, RECT rect)
        {
            RECT res = type switch
            {
                ResolutionType.S16_10 => ResolutionConverter.GetResponsiveRect(new RECT(77, 139, 188, 287), 1766, 1106, rect.Width, rect.Height, false),
                ResolutionType.S21_9 => ResolutionConverter.GetResponsiveRect(new RECT(72, 119, 158, 235), 1580, 677, rect.Width, rect.Height, false),
                _ => ResolutionConverter.GetResponsiveRect(new RECT(64, 116, 152, 226), 1414, 795, rect.Width, rect.Height, false)
            };
            res.AddPos(rect);
            return res;
        }

        protected RECT GetEventIcon(ResolutionType type, RECT rect)
        {
            RECT res = type switch
            {
                ResolutionType.S16_10 => ResolutionConverter.GetResponsiveRect(new RECT(81, 168, 191, 250), 1766, 1106, rect.Width, rect.Height, false),
                ResolutionType.S21_9 => ResolutionConverter.GetResponsiveRect(new RECT(72, 145, 158, 208), 1580, 677, rect.Width, rect.Height, false),
                _ => ResolutionConverter.GetResponsiveRect(new RECT(71, 139, 155, 200), 1414, 795, rect.Width, rect.Height, false)
            };
            res.AddPos(rect);
            return res;
        }

        protected RECT GetEventTypeRect(ResolutionType type, RECT rect)
        {
            RECT res = type switch
            {
                ResolutionType.S16_10 => ResolutionConverter.GetResponsiveRect(new RECT(216, 175, 395, 205), 1766, 1106, rect.Width, rect.Height, false),
                ResolutionType.S21_9 => ResolutionConverter.GetResponsiveRect(new RECT(180, 149, 336, 171), 1580, 677, rect.Width, rect.Height, false),
                _ => ResolutionConverter.GetResponsiveRect(new RECT(171, 146, 344, 165), 1414, 795, rect.Width, rect.Height, false)
            };
            res.AddPos(rect);
            return res;
        }

        protected RECT GetEventNameRect(ResolutionType type, RECT rect)
        {
            RECT res = type switch
            {
                ResolutionType.S16_10 => ResolutionConverter.GetResponsiveRect(new RECT(212, 208, 501, 247), 1766, 1106, rect.Width, rect.Height, false),
                ResolutionType.S21_9 => ResolutionConverter.GetResponsiveRect(new RECT(198, 186, 494, 219), 1715, 735, rect.Width, rect.Height, false),
                _ => ResolutionConverter.GetResponsiveRect(new RECT(174, 165, 431, 192), 1414, 795, rect.Width, rect.Height, false)
            };
            res.AddPos(rect);
            return res;
        }

        protected RECT GetEventSelect1(ResolutionType type, RECT rect)
        {
            RECT res = type switch
            {
                ResolutionType.S16_10 => ResolutionConverter.GetResponsiveRect(new RECT(1220, 737, 1681, 777), 1766, 1106, rect.Width, rect.Height, false),
                ResolutionType.S21_9 => ResolutionConverter.GetResponsiveRect(new RECT(1131, 377, 1536, 406), 1580, 677, rect.Width, rect.Height, false),
                _ => ResolutionConverter.GetResponsiveRect(new RECT(977, 503, 1332, 536), 1414, 795, rect.Width, rect.Height, false)
            };
            res.AddPos(rect);
            return res;
        }

        protected RECT GetEventSelect2(ResolutionType type, RECT rect)
        {
            RECT res = type switch
            {
                ResolutionType.S16_10 => ResolutionConverter.GetResponsiveRect(new RECT(1220, 807, 1681, 844), 1766, 1106, rect.Width, rect.Height, false),
                ResolutionType.S21_9 => ResolutionConverter.GetResponsiveRect(new RECT(1131, 426, 1536, 461), 1580, 677, rect.Width, rect.Height, false),
                _ => ResolutionConverter.GetResponsiveRect(new RECT(977, 555, 1332, 586), 1414, 795, rect.Width, rect.Height, false)
            };
            res.AddPos(rect);
            return res;
        }
    }
}
