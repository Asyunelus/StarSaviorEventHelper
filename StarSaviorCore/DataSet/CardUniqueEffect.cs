using System.Text.Json.Serialization;

namespace EndoAshu.StarSavior.Core.DataSet
{
    public class CardUniqueEffect
    {
        [JsonPropertyName("이름")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("설명")]
        public string Description { get; set; } = string.Empty;
    }
}