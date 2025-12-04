using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace EndoAshu.StarSavior.Core
{
    public static class PreloadLogic
    {
        public delegate void UpdateUIFunction(int totalIdx, int totalMax, string what, int current, string currentWhat);
        public delegate bool YesNoDialog(string title, string message);
        public delegate void ErrorDialog(string title, string message);
        public delegate void Shutdown();

        const int TOTAL_LOAD_STEP = 6;

        public static async void StartLoad(UpdateUIFunction updateUI, YesNoDialog showYesNo, ErrorDialog showError, Action onSuccess, Action onClose)
        {
            if (!Directory.Exists("./data"))
            {
                Directory.CreateDirectory("./data");
            }
            if (!Directory.Exists("./images"))
            {
                Directory.CreateDirectory("./images");
            }
            if (!Directory.Exists("./images/cards"))
            {
                Directory.CreateDirectory("./images/cards");
            }
            if (!Directory.Exists("./images/detect"))
            {
                Directory.CreateDirectory("./images/detect");
            }

            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true,
                NoStore = true
            };
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("ko-KR,ko;q=0.9,en-US;q=0.8,en;q=0.7");
            client.DefaultRequestHeaders.Add("Referer", "https://arca.live/b/starsavior");
            client.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8");

            updateUI(1, TOTAL_LOAD_STEP, "프로그램 업데이트가 있는지 확인하는중...", 0, "Fetch Github Server...");

            string pubJson = await FetchStringWithProgressAsync(client, $"{DataServer.BASE_PATH}publish.json?cache_buster={Guid.NewGuid().ToString()}", progress =>
            {
                updateUI(1, TOTAL_LOAD_STEP, "프로그램 업데이트가 있는지 확인하는중...", (int)(progress * 100.0f), "Fetch Github Server...");
            });
            DataServer.PublishJson? pubData = JsonSerializer.Deserialize<DataServer.PublishJson>(pubJson);

            if (pubData == null)
            {
                showError("오류", "프로그램 업데이트를 확인할 수 없습니다.");
                onClose();
                return;
            }

            if (pubData.Version != DataServer.VERSION)
            {
                if (showYesNo("업데이트 존재", $"현재 버전 : {DataServer.VERSION}\n새로운 버전 : {pubData.Version}\n\n업데이트를 진행해야 실행이 가능합니다.\n진행하시겠습니까?"))
                {
                    updateUI(1, TOTAL_LOAD_STEP, "프로그램 업데이트를 받는 중...", 0, "Download Update File...");
                    byte[] updateZip = await FetchBytesWithProgressAsync(client, pubData.DownloadPath, progress =>
                    {
                        updateUI(1, TOTAL_LOAD_STEP, "프로그램 업데이트를 받는 중...", (int)(progress * 100.0f), "Download Update File...");
                    });

                    string tempPath = Path.GetFullPath("./temp_update.zip");
                    File.WriteAllBytes(tempPath, updateZip);
                    Process.Start(new ProcessStartInfo(Path.GetFullPath("./Update.exe"))
                    {
                        Arguments = tempPath
                    });
                    onClose();
                }
                else
                {
                    onClose();
                }
                return;
            }

            updateUI(2, TOTAL_LOAD_STEP, "새로운 선택지가 있는지 확인하는중...", 0, "Fetch Github Server...");

            await Task.Delay(100);

            updateUI(3, TOTAL_LOAD_STEP, "데이터를 비교하는중...", 0, "Compare Data Hash...");
            await Task.Delay(100);

            int idx = 0;

            foreach (string filePath in pubData.DataJsonFiles)
            {
                await FetchUpdate(client, pubData.DataVersion, filePath, ++idx, pubData.DataJsonFiles.Count, updateUI);
            }
            await Task.Delay(50);

            Settings.LatestDataVersion = pubData.DataVersion;
            Settings.Save();

            updateUI(4, TOTAL_LOAD_STEP, "선택지 데이터를 불러오는중...", 0, "선택지 데이터 파일을 읽는중...");
            await EventLoader.Load();
            await Task.Delay(50);
            updateUI(4, TOTAL_LOAD_STEP, "선택지 데이터를 불러오는중...", 100, "선택지 데이터 파일을 읽는중...");
            await Task.Delay(50);

            updateUI(5, TOTAL_LOAD_STEP, "아르카나 카드 이미지가 있는지 확인하는중...", 0, "대기중...");

            var card = EventLoader.ArcanaCards;
            idx = 0;
            foreach (var c in card)
            {
                if (string.IsNullOrEmpty(c.Image)) continue;
                updateUI(5, TOTAL_LOAD_STEP, "아르카나 카드 이미지가 있는지 확인하는중...", (int)(idx * 100.0f / card.Count), $"Check File Exists : \"{c.Image}\"");

                string saveImgPath = $"./{c.Image}";

                if (!File.Exists(saveImgPath))
                {
                    byte[] imageBytes = await FetchBytesWithProgressAsync(client, $"{DataServer.BASE_PATH}{c.Image}");
                    File.WriteAllBytes(saveImgPath, imageBytes);
                }
                ++idx;
            }
            updateUI(5, TOTAL_LOAD_STEP, "아르카나 카드 이미지가 있는지 확인하는중...", 100, "완료");

            await Task.Delay(50);

            updateUI(6, TOTAL_LOAD_STEP, "업데이트 완료", 100, "완료");

            await Task.Delay(500);
            onSuccess();
        }

        private static async Task FetchUpdate(HttpClient client, string dataVer, string filePath, int idx, int maxIdx, UpdateUIFunction updateUI)
        {
            if (!File.Exists(filePath) || Settings.LatestDataVersion != dataVer)
            {
                var baseUri = new Uri(DataServer.BASE_PATH);
                var result = new Uri(baseUri, filePath);

                string res = await FetchStringWithProgressAsync(client, result.ToString(), progress =>
                {
                    updateUI(3, TOTAL_LOAD_STEP, "데이터를 비교하는중...", (int)(progress * 100.0f), $"Download File : {Path.GetFileName(filePath)} ({idx}/{maxIdx})");
                });

                updateUI(3, TOTAL_LOAD_STEP, "데이터를 저장하는중...", 100, $"Save File : {Path.GetFileName(filePath)} ({idx}/{maxIdx})");
                File.WriteAllText(filePath, res);
            }
            else
            {
                updateUI(3, TOTAL_LOAD_STEP, "데이터를 비교하는중...", 100, $"{Path.GetFileName(filePath)} file is the latest version. Skip... ({idx}/{maxIdx})");
            }
            await Task.Delay(100);
        }

        public static async Task<byte[]> FetchBytesWithProgressAsync(HttpClient client, string url, Action<float>? progressReporter = null)
        {
            using var response = await client.GetAsync(
                url,
                HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            long? totalLength = response.Content.Headers.ContentLength;

            if (!totalLength.HasValue)
            {
                progressReporter?.Invoke(1.0f);
                return await response.Content.ReadAsByteArrayAsync();
            }

            using var contentStream = await response.Content.ReadAsStreamAsync();
            using var memoryStream = new MemoryStream();

            var buffer = new byte[8192];
            long totalBytesRead = 0;
            int bytesRead;

            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await memoryStream.WriteAsync(buffer.AsMemory(0, bytesRead));
                totalBytesRead += bytesRead;

                float percentage = (float)totalBytesRead / totalLength.Value;
                progressReporter?.Invoke(percentage);
            }

            progressReporter?.Invoke(1.0f);

            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream.ToArray();
        }

        public static async Task<string> FetchStringWithProgressAsync(HttpClient client, string url, Action<float>? progressReporter = null)
        {
            using var response = await client.GetAsync(
                url,
                HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            long? totalLength = response.Content.Headers.ContentLength;

            if (!totalLength.HasValue)
            {
                progressReporter?.Invoke(1.0f);
                return await response.Content.ReadAsStringAsync();
            }

            using var contentStream = await response.Content.ReadAsStreamAsync();
            using var memoryStream = new MemoryStream();

            var buffer = new byte[8192];
            long totalBytesRead = 0;
            int bytesRead;

            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await memoryStream.WriteAsync(buffer.AsMemory(0, bytesRead));
                totalBytesRead += bytesRead;

                float percentage = (float)totalBytesRead / totalLength.Value;
                progressReporter?.Invoke(percentage);
            }

            progressReporter?.Invoke(1.0f);

            memoryStream.Seek(0, SeekOrigin.Begin);

            Encoding encoding = Encoding.UTF8;
            try
            {
                var charSet = response.Content.Headers.ContentType?.CharSet;
                if (charSet != null)
                {
                    encoding = Encoding.GetEncoding(charSet);
                }
            }
            catch (ArgumentException) { }

            using var reader = new StreamReader(memoryStream, encoding);
            return await reader.ReadToEndAsync();
        }

        private static async Task<string> CalculateSha256HashWithPathAsync(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
                {
                    return await CalculateSha256HashAsync(fileStream);
                }
            }
            return "empty";
        }

        private static async Task<string> CalculateSha256HashAsync(Stream stream)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = await Task.Run(() => sha256.ComputeHash(stream));

                StringBuilder builder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
        private static string CalculateSha256Hash(string rawData)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(rawData);
                byte[] hashBytes = sha256.ComputeHash(bytes);

                StringBuilder builder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
