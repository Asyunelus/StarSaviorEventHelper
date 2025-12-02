using System.Text;
using System.Text.Json.Serialization;

namespace EndoAshu.StarSavior.Core.DataSet
{
    public class CardEventEntry
    {
        [JsonPropertyName("선택지A")]
        public CardEventEffect[] Select1 { get; set; } = new CardEventEffect[0];

        [JsonPropertyName("선택지B")]
        public CardEventEffect[] Select2 { get; set; } = new CardEventEffect[0];

        [JsonPropertyName("선택지C")]
        public CardEventEffect[] Select3 { get; set; } = new CardEventEffect[0];

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
            return builder.ToString();
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