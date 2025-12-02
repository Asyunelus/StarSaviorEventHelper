using System.Text.Json.Serialization;

namespace EndoAshu.StarSavior.Core.DataSet
{
    public class CardEvent
    {
        [JsonPropertyName("이름")]
        public string[] Name { get; set; } = new string[0];

        [JsonPropertyName("1단계")]
        public CardEventEntry Event1 { get; set; } = new CardEventEntry();

        [JsonPropertyName("2단계")]
        public CardEventEntry Event2 { get; set; } = new CardEventEntry();

        [JsonPropertyName("3단계")]
        public CardEventEntry Event3 { get; set; } = new CardEventEntry();
    }
}