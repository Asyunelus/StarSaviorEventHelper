using EndoAshu.StarSavior.Core.DataSet;
using EndoAshu.StarSavior.Core.Search.Data;
using System.Text;
namespace EndoAshu.StarSavior.Core.Search
{
    public enum SearchResultType
    {
        Arcana,
        Journey,
        Failed_Timeout,
        Failed_NotFoundEventType,
        Failed_NotFoundWindow,
        Failed_NotEventOnScreen,
        Failed_NotFoundJourneyData,
        Failed_NotFoundArcanaData,
        Failed_Exception,
        Failed_SearchWait,
        Failed_EngineNotSet,
        Failed_Unknown
    }

    public struct ApplyItem
    {
        public string Name { get; set; }
        public string Effect { get; set; }

        public ApplyItem(string name, string effect)
        {
            Name = name;
            Effect = effect;
        }
    }

    public class SearchResult
    {
        public bool IsArcana => Type == SearchResultType.Arcana;
        public bool IsJourney => Type == SearchResultType.Journey;
        public bool IsFailed => !IsArcana && !IsJourney;
        public SearchResultType Type { get; set; } = SearchResultType.Failed_Unknown;

        public JourneySearchResult? Journey { get; set; }
        public CardSearchResult? Arcana { get; set; }

        public object? ErrorObject { get; }

        public SearchResult(CardSearchResult data)
        {
            Type = SearchResultType.Arcana;
            Arcana = data;
            ErrorObject = null;
        }

        public SearchResult(JourneySearchResult data)
        {
            Type = SearchResultType.Journey;
            Journey = data;
            ErrorObject = null;
        }

        public SearchResult() : this(SearchResultType.Failed_Unknown, null)
        {
            
        }

        public SearchResult(SearchResultType type, object? errorObject = null)
        {
            Type = type;
            if (!IsFailed)
            {
                throw new ArgumentException("SearchResult(ResultType)은 오류 코드만 받을 수 있습니다.");
            }
            ErrorObject = errorObject;
        }

        public ApplyItem[] FromJourney()
        {
            if (!IsJourney) return Array.Empty<ApplyItem>();

            return Journey!.Data!.Choices.Select(e =>
            {
                string effect = "";

                if (string.IsNullOrEmpty(e.ResultPositive) && string.IsNullOrEmpty(e.ResultNegative))
                {
                    effect = e.Result;
                }
                else
                {
                    effect = $"성공 시\n {(string.IsNullOrEmpty(e.ResultPositive) ? "없음" : e.ResultPositive)}\n\n실패 시\n {(string.IsNullOrEmpty(e.ResultNegative) ? "없음" : e.ResultNegative)}";
                }

                string text = e.Text;
                if (!string.IsNullOrEmpty(e.Condition))
                {
                    text = $"{text} ({e.Condition})";
                }
                return new ApplyItem(text, effect);
            }).ToArray();
        }

        public ApplyItem[] FromArcana()
        {
            if (!IsArcana) return Array.Empty<ApplyItem>();
            if (Arcana!.EventIndex < 0) return Array.Empty<ApplyItem>();
            int idx = 0;
            CardDataWrapper.EventData ev = Arcana!.Data!.CardEvents[Arcana!.EventIndex];
            return ev.Entry.Selects.Select(entry => {
                string name = entry.Item1;
                CardEventSelect[] selects = entry.Item2;
                ++idx;
                if (string.IsNullOrEmpty(name))
                {
                    name = $"선택지 {idx}";
                }
                StringBuilder sb = new StringBuilder();

                foreach (CardEventSelect select in selects)
                {
                    sb.Append($"[{select.Type}]");
                    sb.AppendLine();
                    if (select.Effect.Length > 0)
                    {
                        sb.Append(string.Join(", ", select.Effect.Select(e => e.ToString())));
                        sb.AppendLine();
                    }
                    sb.AppendLine();
                }

                return new ApplyItem(name, sb.ToString());
            }).ToArray();
        }
    }
}
