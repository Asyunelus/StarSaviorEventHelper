using System.Text.Json.Serialization;

namespace EndoAshu.StarSavior.Core.DataSet
{
    public class JourneyData
    {
        [JsonPropertyName("journey")]
        public JourneyDataInner[] Journey { get; set; } = new JourneyDataInner[0];

        [JsonPropertyName("resette")]
        public JourneyDataInner[] Resette { get; set; } = new JourneyDataInner[0];

        [JsonPropertyName("aganon")]
        public JourneyDataInner[] Aganon { get; set; } = new JourneyDataInner[0];

        [JsonPropertyName("flora")]
        public JourneyDataInner[] Flora { get; set; } = new JourneyDataInner[0];

        [JsonPropertyName("kalide")]
        public JourneyDataInner[] Kalide { get; set; } = new JourneyDataInner[0];

        [JsonPropertyName("weather")]
        public JourneyDataInner[] Weather { get; set; } = new JourneyDataInner[0];

        [JsonPropertyName("subjugation")]
        public JourneyDataInner[] SubJugation { get; set; } = new JourneyDataInner[0];
    }
}
