using EndoAshu.StarSavior.Core.DataSet;

namespace EndoAshu.StarSavior.Core.Search.Data
{
    public class CardDataWrapper
    {
        public enum CardRarity
        {
            SSR,
            SR,
            R,
            Error
        }

        public class EventData
        {
            public string Name { get; set; } = string.Empty;
            public CardEventEntry Entry { get; set; } = new CardEventEntry();

            public EventData(string name, CardEventEntry entry)
            {
                Name = name;
                Entry = entry;
            }
        }

        public readonly Dictionary<int, EventData> CardEvents = new Dictionary<int, EventData>();
        public int Id => _owner.Id;
        public string Name => _owner.Name;
        public string CharacterName => _owner.CharacterName;
        public string RawRarity => _owner.Rarity;
        public CardRarity Rarity { get; }
        public string Image => _owner.Image;
        public CardType Type => _owner.Type;
        public CardUniqueSkill UniqueSkill => _owner.UniqueSkill;
        public CardUniqueEffect UniqueEffect => _owner.UniqueEffect;


        private readonly CardData _owner;

        public CardDataWrapper(CardData data)
        {
            _owner = data;
            for(int i = 0; i < data.Event.Name.Length; ++i)
            {
                CardEvents[i] = new EventData(data.Event.Name[i], i switch
                {
                    0 => data.Event.Event1,
                    1 => data.Event.Event2,
                    2 => data.Event.Event3,
                    _ => new CardEventEntry()
                });
            }

            if (Enum.TryParse(data.Rarity, out CardRarity rare)) {
                Rarity = rare;
            } else
            {
                Rarity = CardRarity.Error;
            }
        }
    }
}
