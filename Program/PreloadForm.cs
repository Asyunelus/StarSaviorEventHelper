using EndoAshu.StarSavior.Core;
using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Program
{
    public partial class PreloadForm : Form
    {
        private bool _loaded = false;

        public PreloadForm()
        {
            InitializeComponent();

            UpdateUI(0, 1, "시작 준비중...", 0, "대기");
        }

        private void PreloadForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_loaded)
            {
                Application.Exit();
            }
        }

        private void PreloadForm_Load(object sender, EventArgs e)
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

            progressBar1.Maximum = 100;
            progressBar2.Maximum = 100;

            StartLoad();
        }

        private async void StartLoad()
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true,
                NoStore = true
            };

            const int total = 6;
            UpdateUI(1, total, "프로그램 업데이트가 있는지 확인하는중...", 0, "Fetch Github Server...");

            string pubJson = await FetchStringWithProgressAsync(client, $"{DataServer.BASE_PATH}publish.json?cache_buster={Guid.NewGuid().ToString()}", progress =>
            {
                UpdateUI(1, total, "프로그램 업데이트가 있는지 확인하는중...", (int)(progress * 100.0f), "Fetch Github Server...");
            });
            DataServer.PublishJson? pubData = JsonSerializer.Deserialize<DataServer.PublishJson>(pubJson);

            if (pubData == null)
            {
                MessageBox.Show("프로그램 업데이트를 확인할 수 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            if (pubData.Version != DataServer.VERSION)
            {
                DialogResult res = MessageBox.Show($"현재 버전 : {DataServer.VERSION}\n새로운 버전 : {pubData.Version}\n\n업데이트를 진행해야 실행이 가능합니다.\n진행하시겠습니까?", "업데이트 존재", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (res == DialogResult.Yes)
                {
                    UpdateUI(1, total, "프로그램 업데이트를 받는 중...", 0, "Download Update File...");
                    byte[] updateZip = await FetchBytesWithProgressAsync(client, pubData.DownloadPath, progress =>
                    {
                        UpdateUI(1, total, "프로그램 업데이트를 받는 중...", (int)(progress * 100.0f), "Download Update File...");
                    });

                    string tempPath = Path.GetFullPath("./temp_update.zip");
                    File.WriteAllBytes(tempPath, updateZip);
                    Process.Start(new ProcessStartInfo(Path.GetFullPath("./Update.exe"))
                    {
                        Arguments = tempPath
                    });
                    Close();
                } else { 
                    Close();
                }
                return;
            }

            UpdateUI(2, total, "새로운 선택지가 있는지 확인하는중...", 0, "Fetch Github Server...");

            await Task.Delay(100);

            string cards = await FetchStringWithProgressAsync(client, $"{DataServer.BASE_PATH}data/cards.json", progress =>
            {
                UpdateUI(2, total, "새로운 선택지가 있는지 확인하는중...", (int)(progress * 33.0f), "Fetch cards.json");
            });

            await Task.Delay(50);
            string journeyData = await FetchStringWithProgressAsync(client, $"{DataServer.BASE_PATH}data/journey_data.json", progress =>
            {
                UpdateUI(2, total, "새로운 선택지가 있는지 확인하는중...", (int)(progress * 33.0f) + 33, "Fetch journey_data.json");
            });

            await Task.Delay(50);
            string potentials = await FetchStringWithProgressAsync(client, $"{DataServer.BASE_PATH}data/potentials.json", progress =>
            {
                UpdateUI(2, total, "새로운 선택지가 있는지 확인하는중...", (int)(progress * 33.0f) + 33, "Fetch potentials.json");
            });

            await Task.Delay(50);
            string na = await FetchStringWithProgressAsync(client, $"{DataServer.BASE_PATH}images/detect/na.png", progress =>
            {
                UpdateUI(2, total, "새로운 선택지가 있는지 확인하는중...", (int)(progress * 1.0f) + 99, "Fetch na.png");
            });

            await Task.Delay(50);
            UpdateUI(2, total, "새로운 선택지가 있는지 확인하는중...", 100, "데이터 교체 여부 확인중");

            bool saveCards = CalculateSha256Hash(cards) != await CalculateSha256HashWithPathAsync("./data/cards.json");
            bool saveJourney = CalculateSha256Hash(journeyData) != await CalculateSha256HashWithPathAsync("./data/journey_data.json");
            bool savePotentials = CalculateSha256Hash(potentials) != await CalculateSha256HashWithPathAsync("./data/potentials.json");
            bool saveNaImage = CalculateSha256Hash(na) != await CalculateSha256HashWithPathAsync("./images/detect/na.png");

            if (saveJourney || saveCards || savePotentials || saveNaImage)
            {
                UpdateUI(3, total, "새로운 선택지 데이터를 저장하는중...", 0, "Save Server Data...");

                if (saveCards)
                {
                    File.WriteAllText("./data/cards.json", cards);
                }
                if (saveJourney)
                {
                    File.WriteAllText("./data/journey_data.json", journeyData);
                }
                if (savePotentials)
                {
                    File.WriteAllText("./data/potentials.json", potentials);
                }
                if (saveNaImage)
                {
                    File.WriteAllText("./images/detect/na.png", na);
                }

                await Task.Delay(50);
                UpdateUI(3, total, "새로운 선택지 데이터를 저장하는중...", 100, "Save Server Data...");
            }
            await Task.Delay(50);

            UpdateUI(4, total, "선택지 데이터를 불러오는중...", 0, "선택지 데이터 파일을 읽는중...");
            await EventLoader.Load();
            await Task.Delay(50);
            UpdateUI(4, total, "선택지 데이터를 불러오는중...", 100, "선택지 데이터 파일을 읽는중...");
            await Task.Delay(50);

            UpdateUI(5, total, "아르카나 카드 이미지가 있는지 확인하는중...", 0, "대기중...");

            var card = EventLoader.ArcanaCards;
            int idx = 0;
            foreach(var c in card)
            {
                if (string.IsNullOrEmpty(c.Image)) continue;
                UpdateUI(5, total, "아르카나 카드 이미지가 있는지 확인하는중...", (int)(idx * 100.0f / card.Count), $"Check File Exists : \"{c.Image}\"");

                string saveImgPath = $"./{c.Image}";

                if (!File.Exists(saveImgPath))
                {
                    byte[] imageBytes = await FetchBytesWithProgressAsync(client, $"{DataServer.BASE_PATH}{c.Image}");
                    File.WriteAllBytes(saveImgPath, imageBytes);
                }
                ++idx;
            }
            UpdateUI(5, total, "아르카나 카드 이미지가 있는지 확인하는중...", 100, "완료");

            await Task.Delay(50);

            UpdateUI(6, total, "업데이트 완료", 100, "완료");

            _loaded = true;
            await Task.Delay(500);
            Close();
        }

        private void UpdateUI(int totalIdx, int totalMax, string what, int current, string currentWhat)
        {
            progressBar1.Value = (int)(totalIdx * 100.0f / totalMax);
            progressBar2.Value = current;

            label1.Text = $"{what} ({totalIdx}/{totalMax})";
            label2.Text = $"{currentWhat} ({current}%)";
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
            catch (ArgumentException) {}

            using var reader = new StreamReader(memoryStream, encoding);
            return await reader.ReadToEndAsync();
        }

        private static async Task<string> CalculateSha256HashWithPathAsync(string filePath) {
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
