using System.Text.Json.Serialization;

namespace EndoAshu.StarSavior.Core.DataSet
{
    public class CardData
    {
        [JsonPropertyName("아이디")]
        [JsonRequired]
        public int Id { get; set; } = -1;

        [JsonPropertyName("이름")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("캐릭터")]
        public string CharacterName { get; set; } = string.Empty;

        [JsonPropertyName("레어도")]
        public string Rarity { get; set; } = string.Empty;

        [JsonPropertyName("이미지")]
        public string Image { get; set; } = string.Empty;

        [JsonPropertyName("타입")]
        public CardType Type { get; set; } = new CardType();

        [JsonPropertyName("고유잠재")]
        public CardUniqueSkill UniqueSkill { get; set; } = new CardUniqueSkill();

        [JsonPropertyName("고유효과")]
        public CardUniqueEffect UniqueEffect { get; set; } = new CardUniqueEffect();

        [JsonPropertyName("이벤트")]
        public CardEvent Event { get; set; } = new CardEvent();
    }
}
