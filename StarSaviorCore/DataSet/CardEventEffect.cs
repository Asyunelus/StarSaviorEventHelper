using System.Text.Json.Serialization;

namespace EndoAshu.StarSavior.Core.DataSet
{
    public class CardEventEffect
    {
        [JsonPropertyName("타입")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("수치")]
        public string Value { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{Type} {Value}";
        }
    }
}