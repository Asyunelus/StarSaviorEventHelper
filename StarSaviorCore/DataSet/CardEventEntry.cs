using System.Collections.Immutable;
using System.Text;
using System.Text.Json.Serialization;

namespace EndoAshu.StarSavior.Core.DataSet
{
    public class CardEventEntry
    {
        [JsonPropertyName("이름_선택지")]
        public Dictionary<string, string> NameTable { get; set; } = new Dictionary<string, string>();

        [JsonPropertyName("선택지A")]
        public CardEventSelect[] Select1 { get; set; } = Array.Empty<CardEventSelect>();

        [JsonPropertyName("선택지B")]
        public CardEventSelect[] Select2 { get; set; } = Array.Empty<CardEventSelect>();

        [JsonPropertyName("선택지C")]
        public CardEventSelect[] Select3 { get; set; } = Array.Empty<CardEventSelect>();

        [JsonPropertyName("선택지D")]
        public CardEventSelect[] Select4 { get; set; } = Array.Empty<CardEventSelect>();

        [JsonPropertyName("선택지E")]
        public CardEventSelect[] Select5 { get; set; } = Array.Empty<CardEventSelect>();

        [JsonIgnore]
        public IList<string> SelectName
        {
            get
            {
                List<string> result = new List<string>();
                if (!NameTable.TryGetValue("선택지A", out var v1))
                {
                    return result;
                }
                result.Add(v1);
                if (!NameTable.TryGetValue("선택지B", out var v2))
                {
                    return result;
                }
                result.Add(v2);
                if (!NameTable.TryGetValue("선택지C", out var v3))
                {
                    return result;
                }
                result.Add(v3);
                if (!NameTable.TryGetValue("선택지D", out var v4))
                {
                    return result;
                }
                result.Add(v4);
                if (!NameTable.TryGetValue("선택지E", out var v5))
                {
                    return result;
                }
                result.Add(v5);
                return result;
            }
        }

        [JsonIgnore]
        public ICollection<(string, CardEventSelect[])> Selects
        {
            get
            {
                var selectProps = new[] { Select1, Select2, Select3, Select4, Select5 };
                var selName = SelectName ?? new List<string>();
                return selectProps
                    .Where(e => e.Where(i => !i.IsEmpty).Count() > 0)
                    .Take(selName.Count)
                    .Select((prop, index) => (selName[index], prop))
                    .ToList();
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            if (Select1.Length > 0)
            {
                builder.Append("선택지1 : ");
                ToString(builder, Select1);
                builder.AppendLine();
            }
            if (Select2.Length > 0)
            {
                builder.Append("선택지2 : ");
                ToString(builder, Select2);
                builder.AppendLine();
            }
            if (Select3.Length > 0)
            {
                builder.Append("선택지3 : ");
                ToString(builder, Select3);
                builder.AppendLine();
            }
            if (Select4.Length > 0)
            {
                builder.Append("선택지4 : ");
                ToString(builder, Select3);
                builder.AppendLine();
            }
            if (Select5.Length > 0)
            {
                builder.Append("선택지5 : ");
                ToString(builder, Select5);
                builder.AppendLine();
            }
            return builder.ToString();
        }

        private void ToString(StringBuilder builder, CardEventSelect[] select)
        {
            bool first = true;
            foreach (CardEventSelect e in select)
            {
                if (!first)
                {
                    builder.AppendLine();
                }
                builder.Append(e.Type);
                builder.Append(" : ");
                if (e.Effect.Length > 0)
                {
                    ToString(builder, e.Effect);
                } else
                {
                    builder.Append("없음");
                }
            }
        }

        private void ToString(StringBuilder builder, CardEventEffect[] effect)
        {
            bool first = true;
            foreach (CardEventEffect e in effect)
            {
                if (!first)
                {
                    builder.Append(", ");
                }
                first = false;

                builder.Append(e.Type);
                builder.Append(" ");
                builder.Append(e.Value);
            }
        }
    }
}