using System.Text.Json.Serialization;

namespace EndoAshu.StarSavior.Core.DataSet
{
    public class CardType
    {
        [JsonPropertyName("훈련")]
        public string Training { get; set; } = string.Empty;

        [JsonPropertyName("보조1")]
        public string SubType1 { get; set; } = string.Empty;

        [JsonPropertyName("보조2")]
        public string SubType2 { get; set; } = string.Empty;
    }
}