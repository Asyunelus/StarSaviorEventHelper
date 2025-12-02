using System.Text.Json.Serialization;

namespace EndoAshu.StarSavior.Core.DataSet
{
    public class CardUniqueSkill
    {

        [JsonPropertyName("이름")]
        public string Name { get; set; } = string.Empty;
    }
}