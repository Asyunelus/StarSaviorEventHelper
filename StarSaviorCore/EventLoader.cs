using EndoAshu.StarSavior.Core.DataSet;
using OpenCvSharp;
using System.Drawing;
using System.Runtime.InteropServices.Marshalling;
using System.Text.Json;

namespace EndoAshu.StarSavior.Core
{
    public static class EventLoader
    {
        private static readonly Dictionary<int, CardData> _arcana = new Dictionary<int, CardData>();
        private static readonly List<JourneyDataInner> _journey = new List<JourneyDataInner>();

        public static ICollection<CardData> ArcanaCards => _arcana.Values;

        public static async Task Load()
        {
            _journey.Clear();
            using (Stream stream = File.OpenRead("./data/journey_data.json"))
            {
                JourneyData data = JsonSerializer.Deserialize<JourneyData>(stream)!;
                _journey.AddRange(data.Journey);
                _journey.AddRange(data.Resette);
                _journey.AddRange(data.Aganon);
                _journey.AddRange(data.Flora);
                _journey.AddRange(data.Kalide);
                _journey.AddRange(data.Weather);
                _journey.AddRange(data.SubJugation);
            }

            _arcana.Clear();

            using (Stream stream = File.OpenRead("./data/cards.json"))
            {
                CardData[] data = JsonSerializer.Deserialize<CardData[]>(stream)!;

                foreach (var d in data)
                {
                    _arcana[d.Id] = d;
                }
            }
        }

        private class CardSearchData
        {
            public double NameMatch { get; set; }
            public int ImageMatch { get; set; }

            public CardData Data { get; set; }

            public CardSearchData(double name, int image, CardData data)
            {
                NameMatch = name;
                ImageMatch = image;
                Data = data;
            }
        }

        public static async Task<JourneyDataInner?> FindJourneyEventAsync(string eventName)
        {
            return await Task.Run(() =>
            {
                var search = _journey.Select(dat =>
                {
                    double sim = HangulCompare.GetHangulSimilarity(eventName, dat.Name);
                    return (sim, dat);
                }).OrderByDescending(e => e.sim).ToList();

                if (search.Count > 0)
                {
                    if (search[0].sim > 0.7)
                    {
                        return search[0].dat;
                    }
                }

                return null;
            });
        }


        public static async Task<(double, int, string?, CardEventEntry?)> FindCardEventAsync(Bitmap cardImage, string eventName, params string[] eventSelect)
        {
#pragma warning disable CA1416
            var searchTasks = _arcana.Values.Where(e => !string.IsNullOrEmpty(e.Image)).Select(e => ((Bitmap)cardImage.Clone(), e)).Select(async dat =>
            {
                Bitmap bitmap = dat.Item1;
                CardData e = dat.e;
                return await Task.Run(() =>
                {
                    double nameSim = double.MinValue;
                    foreach (var str in e.Event.Name)
                    {
                        nameSim = Math.Max(nameSim, HangulCompare.GetHangulSimilarity(eventName, str));
                    }

                    int imageScore = ImageMatcher.IsMatch(bitmap, $"./{e.Image}");

                    bitmap.Dispose();

                    return new CardSearchData(nameSim, imageScore, e);
                });
            });
#pragma warning restore CA1416

            var allResults = await Task.WhenAll(searchTasks);

            List<CardSearchData> searchData = allResults
                                                .Where(e => e.ImageMatch > 10)
                                                .ToList();

            if (searchData.Count <= 0)
            {
                return (-1, -1, null, null);
            }

            float max = (float)searchData.Max(e => e.ImageMatch);

            List<CardSearchData> data = searchData
                                        .OrderByDescending(e => e.ImageMatch / max + e.NameMatch)
                                        .ToList();

            if (data.Count <= 0)
            {
                return (-1, -1, null, null);
            }

            CardData first = data[0].Data;

            double t = -1;
            int idx = -1;

            for (int i = 0; i < first.Event.Name.Length; ++i)
            {
                string str = first.Event.Name[i];
                double cur = HangulCompare.GetHangulSimilarity(eventName, str);

                if (cur > t)
                {
                    t = cur;
                    idx = i;
                }
            }

            return (t, data[0].ImageMatch, first.Event.Name[idx], idx switch
            {
                0 => first.Event.Event1,
                1 => first.Event.Event2,
                2 => first.Event.Event3,
                _ => null
            });
        }

        public static (double, int, string?, CardEventEntry?) FindCardEvent(Bitmap cardImage, string eventName, params string[] eventSelect)
        {
            List<CardSearchData> searchData = _arcana.Values.Select(e =>
            {
                double name = double.MinValue;
                foreach (var str in e.Event.Name)
                {
                    name = Math.Max(name, HangulCompare.GetHangulSimilarity(eventName, str));
                }

                int image = ImageMatcher.IsMatch(cardImage, $"./{e.Image}");

                return new CardSearchData(name, image, e);
            }).Where(e => e.ImageMatch > 10).ToList();

            if (searchData.Count <= 0)
            {
                return (-1, -1, null, null);
            }

            float max = (float)searchData.Max(e => e.ImageMatch);

            List<CardSearchData> data = searchData.OrderByDescending(e => e.ImageMatch / max + e.NameMatch).ToList();

            if (data.Count <= 0)
            {
                return (-1, -1, null, null);
            }

            CardData first = data[0].Data;

            double t = -1;
            int idx = -1;

            for (int i = 0; i < first.Event.Name.Length; ++i)
            {
                string str = first.Event.Name[i];
                double cur = HangulCompare.GetHangulSimilarity(eventName, str);

                if (cur > t)
                {
                    t = cur;
                    idx = i;
                }
            }

            return (t, data[0].ImageMatch, first.Event.Name[idx], idx switch
            {
                0 => first.Event.Event1,
                1 => first.Event.Event2,
                2 => first.Event.Event3,
                _ => null
            });
        }
    }
}