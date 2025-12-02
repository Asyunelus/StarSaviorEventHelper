using System.Text.Json.Serialization;

namespace EndoAshu.StarSavior.Core.DataSet
{
    public class JourneyChoice
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("condition")]
        public string Condition { get; set; } = string.Empty;

        [JsonPropertyName("result")]
        public string Result { get; set; } = string.Empty;

        [JsonPropertyName("result_positive")]
        public string ResultPositive { get; set; } = string.Empty;

        [JsonPropertyName("result_negative")]
        public string ResultNegative { get; set; } = string.Empty;
    }
}