using System.Text.Json.Serialization;

namespace EndoAshu.StarSavior.Core.DataSet
{
    public class CardEventSelect
    {
        [JsonPropertyName("여부")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("획득")]
        public CardEventEffect[] Effect { get; set; } = Array.Empty<CardEventEffect>();
    }
}