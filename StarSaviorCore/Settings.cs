using EndoAshu.StarSavior.Core.Search;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EndoAshu.StarSavior.Core
{
    public static class Settings
    {
        private class SaveData
        {
            [JsonPropertyName("engine_after_v0_2_2")]
            [JsonRequired]
            public string Engine { get; set; } = string.Empty;

            [JsonPropertyName("latest_data")]
            [JsonRequired]
            public string LatestDataVersion { get; set; } = string.Empty;

            [JsonPropertyName("top_most")]
            public bool TopMost { get; set; } = false;

            [JsonPropertyName("theme")]
            public string Theme { get; set; } = "Dark";
        }

        private static readonly string saveConfig = "./config.json";

        public static string LatestDataVersion { get; set; } = string.Empty;

        public static bool TopMost { get; set; } = false;
        public static string Theme { get; set; } = "Dark";

        public static void Load()
        {
            if (!File.Exists(saveConfig))
            {
                Save();
            }

            try
            {
                using (Stream fs = File.OpenRead(saveConfig))
                {
                    var data = JsonSerializer.Deserialize<SaveData>(fs)!;

                    SearchEngine.Current = SearchEngine.Items.FirstOrDefault(e => e.Name == data.Engine);
                    LatestDataVersion = data.LatestDataVersion;
                    TopMost = data.TopMost;
                    Theme = data.Theme;
                }
            } catch(Exception)
            {
                Save();
            }
        }

        public static void Save()
        {
            SaveData data = new SaveData();

            data.Engine = SearchEngine.Current == null ? SearchEngine.Items.First().Name : SearchEngine.Current.Name;
            data.LatestDataVersion = LatestDataVersion;
            data.TopMost = TopMost;
            data.Theme = Theme;

            using (Stream fs = File.Create(saveConfig))
            {
                JsonSerializer.Serialize(fs, data);
            }
        }
    }
}
