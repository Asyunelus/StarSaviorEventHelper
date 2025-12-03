using EndoAshu.StarSavior.Core.DataSet;
using EndoAshu.StarSavior.Core.Search.Data;
using OpenCvSharp;
using System.Drawing;
using System.Runtime.InteropServices.Marshalling;
using System.Text.Json;

namespace EndoAshu.StarSavior.Core
{
    public static class EventLoader
    {

        private static readonly Dictionary<int, CardDataWrapper> _arcana = new Dictionary<int, CardDataWrapper>();
        private static readonly List<JourneyDataWrapper> _journey = new List<JourneyDataWrapper>();

        public static ICollection<CardDataWrapper> ArcanaCards => _arcana.Values;

        public static ICollection<JourneyDataWrapper> JourneyDatas => _journey;

        public static async Task Load()
        {
            _journey.Clear();
            using (Stream stream = File.OpenRead("./data/journey_data.json"))
            {
                List<JourneyDataInner> datas = new List<JourneyDataInner>();
                JourneyData data = JsonSerializer.Deserialize<JourneyData>(stream)!;
                datas.AddRange(data.Journey);
                datas.AddRange(data.Resette);
                datas.AddRange(data.Aganon);
                datas.AddRange(data.Flora);
                datas.AddRange(data.Kalide);
                datas.AddRange(data.Weather);
                datas.AddRange(data.SubJugation);

                _journey.AddRange(datas.Select(e => new JourneyDataWrapper(e)).ToArray());
            }

            _arcana.Clear();

            using (Stream stream = File.OpenRead("./data/cards.json"))
            {
                CardData[] data = JsonSerializer.Deserialize<CardData[]>(stream)!;

                foreach (var d in data)
                {
                    _arcana[d.Id] = new CardDataWrapper(d);
                }
            }
        }
    }
}