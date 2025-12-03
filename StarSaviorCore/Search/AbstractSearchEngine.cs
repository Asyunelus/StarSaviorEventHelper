namespace EndoAshu.StarSavior.Core.Search
{
    public abstract class AbstractSearchEngine
    {
        public string Name { get; }

        public AbstractSearchEngine(string engineName)
        {
            Name = engineName;
        }

        public async Task<SearchResult> Search(OcrReader reader, string windowName, int timeoutMilliseconds = 4000)
        {
            IntPtr window = WindowUtil.FindTargetStartsWith(windowName);
            if (window != IntPtr.Zero)
            {
                RECT rect = WindowUtil.GetRect(window);
                float res = (float)rect.Width / rect.Height;
                ResolutionType resType = res <= 1.9f ? ResolutionType.S16_9 : ResolutionType.S21_9;

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

        protected abstract Task<SearchResult> InternalSearch(OcrReader reader, IntPtr window, ResolutionType resolutionType, RECT windowRect);


        protected RECT GetCardRect(ResolutionType type, RECT rect)
        {
            RECT res = type switch
            {
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
                ResolutionType.S21_9 => ResolutionConverter.GetResponsiveRect(new RECT(1131, 426, 1536, 461), 1580, 677, rect.Width, rect.Height, false),
                _ => ResolutionConverter.GetResponsiveRect(new RECT(977, 555, 1332, 586), 1414, 795, rect.Width, rect.Height, false)
            };
            res.AddPos(rect);
            return res;
        }
    }
}
