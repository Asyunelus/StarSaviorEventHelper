using System.Text.Json.Serialization;

namespace EndoAshu.StarSavior.Core
{
    public static class DataServer
    {
        public class PublishJson
        {
            [JsonPropertyName("version")]
            [JsonRequired]
            public string Version { get; set; } = string.Empty;

            [JsonPropertyName("downloadPath")]
            [JsonRequired]
            public string DownloadPath { get; set; } = string.Empty;
        }
        public static readonly string BASE_PATH = "https://raw.githubusercontent.com/Asyunelus/StarSaviorEventHelperDB/refs/heads/main/";

        public static readonly string VERSION = "v0.2-beta";
    }
}
