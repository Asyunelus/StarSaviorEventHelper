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

            [JsonPropertyName("data_version")]
            [JsonRequired]
            public string DataVersion { get; set; } = string.Empty;

            [JsonPropertyName("data_files")]
            [JsonRequired]
            public List<string> DataJsonFiles { get; set; } = new List<string>();
        }
        public static readonly string BASE_PATH = "https://raw.githubusercontent.com/Asyunelus/StarSaviorEventHelperDB/refs/heads/main/";

        public static readonly string VERSION = "v0.2.2-beta";

        /// <summary>
        /// Version 코드 작성법
        /// 
        /// 베타 버전일 경우 0_버전
        /// 정식 버전일 경우 1_버전
        /// 버전 작성 시 다음과 같이 작성
        /// 0.2.1일 경우 000_002_01
        /// 0.2일 경우 000_002_00
        /// 1.3.4일 경우 001_003_04
        /// 
        /// </summary>
        public static readonly long VERSION_CODE = 0_000_002_02;
    }
}
