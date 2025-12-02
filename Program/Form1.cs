using EndoAshu.StarSavior.Core;
using EndoAshu.StarSavior.Core.DataSet;
using System.Text;
using System.Xml.Linq;

namespace Program
{
    public partial class Form1 : Form
    {
        private OcrReader reader;
        private bool isLoading = false;

        public Form1()
        {
            InitializeComponent();
            checkBoxAutoRefresh.Checked = false;
            ApplyNone();
            labelLoading.Text = "대기중...";

            reader = new OcrReader(@"./tdata");
        }
        private void Form1_Load(object sender, EventArgs e)
        {

            new PreloadForm().ShowDialog();

            checkBoxAutoRefresh.Checked = true;

            timer1.Interval = 100;
            timer1.Start();
        }

        ~Form1()
        {
            if (reader != null)
            {
                reader.Dispose();
            }
            reader = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RefreshOCR();
        }

        private async void RefreshOCR()
        {
            if (isLoading) return;

            isLoading = true;
            button1.Enabled = false;
            labelLoading.Text = "화면인식 진행중...";

            await Task.Delay(100);

            try
            {
                IntPtr window = OcrReader.FindTargetStartsWith("스타세이비어");

                if (window != IntPtr.Zero)
                {
                    var task = RefreshOCRInternal(window);
                    var timeoutTask = Task.Delay(4000);

                    var completedTask = await Task.WhenAny(task, timeoutTask);

                    if (completedTask == timeoutTask)
                    {
                        labelLoading.Text = "시간 초과 (강제 재시도)";
                    }
                    else
                    {
                        await task;
                    }
                }
                else
                {
                    ApplyNone();
                }
            }
            catch (Exception ex)
            {
                ApplyNone();
            }
            finally
            {
                if (isLoading)
                {
                    isLoading = false;
                    if (checkBoxAutoRefresh.Checked && !timer1.Enabled)
                    {
                        timer1.Start();
                    }
                    button1.Enabled = true;
                }
            }
        }

        private async Task RefreshOCRInternal(IntPtr window)
        {
            RECT rect = OcrReader.GetRect(window);

            float res = (float)rect.Width / rect.Height;
            ResolutionType resType = res <= 1.9f ? ResolutionType.S16_9 : ResolutionType.S21_9;

            RECT evTypeRect = GetEventTypeRect(resType, rect);
            string evType = reader.Capture(evTypeRect);

            if (
                evType.Contains("이")
                || evType.Contains("벤")
                || evType.Contains("트")
                )
            {

                RECT markRect = GetEventIcon(resType, rect);
                using (Bitmap mark = reader.CaptureBitmap(markRect))
                {
                    int match = ImageMatcher.IsMatch(mark, "./images/detect/na.png");
                    if (match >= 5 || evType.Contains("여정"))
                    {
                        await RefreshJourney(window, resType, rect);
                    }
                    else
                    {
                        await RefreshOCRCard(window, resType, rect);
                    }
                }

            }
            else
            {
                Invoke(() => ApplyNone());
            }
        }

        private async Task RefreshJourney(IntPtr window, ResolutionType resType, RECT rect)
        {
            RECT eventNameRect = GetEventNameRect(resType, rect);
            string eventName = reader.Capture(eventNameRect);
            var found = await EventLoader.FindJourneyEventAsync(eventName);
            if (found != null)
            {
                ApplyItem[] applyItems = found.Choices.Select(e =>
                {
                    string effect = "";

                    if (string.IsNullOrEmpty(e.ResultPositive) && string.IsNullOrEmpty(e.ResultNegative))
                    {
                        effect = e.Result;
                    }
                    else
                    {
                        effect = $"성공 시\n {(string.IsNullOrEmpty(e.ResultPositive) ? "없음" : e.ResultPositive)}\n\n실패 시\n {(string.IsNullOrEmpty(e.ResultNegative) ? "없음" : e.ResultNegative)}";
                    }

                    string text = e.Text;
                    if (!string.IsNullOrEmpty(e.Condition))
                    {
                        text = $"{text} ({e.Condition})";
                    }
                    return new ApplyItem(text, effect);
                }).ToArray();
                Invoke(() => ApplyText(found.Category, found.Name, applyItems));
            }
            else
            {
                Invoke(() => ApplyNone());
            }
        }

        private async Task RefreshOCRCard(IntPtr window, ResolutionType resType, RECT rect)
        {
            RECT eventNameRect = GetEventNameRect(resType, rect);
            string eventName = reader.Capture(eventNameRect);

            RECT select1Rect = GetEventSelect1(resType, rect);
            string select1 = reader.Capture(select1Rect);

            RECT select2Rect = GetEventSelect2(resType, rect);
            string select2 = reader.Capture(select2Rect);

            RECT cardImg = GetCardRect(resType, rect);
            using Bitmap cardBitmap = reader.CaptureBitmap(cardImg, 11);

            var entry = await EventLoader.FindCardEventAsync(cardBitmap, eventName, select1, select2);

            if (entry.Item3 != null && entry.Item4 != null)
            {
                List<CardEventEffect[]> list = new List<CardEventEffect[]>
                            {
                                entry.Item4.Select1,
                                entry.Item4.Select2,
                                entry.Item4.Select3
                            };

                int idx = 0;
                var applyItems = list.Select(e =>
                {
                    ++idx;
                    if (e.Length <= 0)
                    {
                        return new ApplyItem($"선택지 {idx}", "없음");
                    }
                    StringBuilder sb = new StringBuilder();
                    return new ApplyItem($"선택지 {idx}", string.Join(", ", e.Select(e => $"{e.Type} {e.Value}")));
                }).ToArray();
                Invoke(() => ApplyText("아르카나 이벤트", entry.Item3, applyItems));
            }
            else
            {
                Invoke(() => ApplyNone());
            }
        }

        public void ApplyNone()
        {
            labelLoading.Text = "화면인식 실패...";
            labelEventName.Text = "Not Detected";
            labelEventType.Text = "";
            ApplyItemText(null, labelEventSelect1Name, labelEventSelect1Effect);
            ApplyItemText(null, labelEventSelect2Name, labelEventSelect2Effect);
            ApplyItemText(null, labelEventSelect3Name, labelEventSelect3Effect);
            ApplyItemText(null, labelEventSelect4Name, labelEventSelect4Effect);

            button1.Enabled = true;

            if (checkBoxAutoRefresh.Checked)
            {
                labelLoading.Text = "화면인식 실패 (재시도 대기중...)";
                timer1.Interval = 1000;
                timer1.Start();
            }
            isLoading = false;
        }

        private void ApplyText(string type, string name, ApplyItem[] items)
        {
            labelLoading.Text = "화면인식 성공";
            labelEventType.Text = type;
            labelEventName.Text = name;

            ApplyItemText(items.Length >= 1 ? items[0] : null, labelEventSelect1Name, labelEventSelect1Effect);
            ApplyItemText(items.Length >= 2 ? items[1] : null, labelEventSelect2Name, labelEventSelect2Effect);
            ApplyItemText(items.Length >= 3 ? items[2] : null, labelEventSelect3Name, labelEventSelect3Effect);
            ApplyItemText(items.Length >= 4 ? items[3] : null, labelEventSelect4Name, labelEventSelect4Effect);

            button1.Enabled = true;

            if (checkBoxAutoRefresh.Checked)
            {
                labelLoading.Text = "화면인식 성공 (갱신 대기중...)";
                timer1.Interval = 3000;
                timer1.Start();
            }
            isLoading = false;
        }

        private struct ApplyItem
        {
            public string Name { get; set; }
            public string Effect { get; set; }

            public ApplyItem(string name, string effect)
            {
                Name = name;
                Effect = effect;
            }
        }

        private void ApplyItemText(ApplyItem? value, Label nameText, Label effectText)
        {
            if (!value.HasValue)
            {
                nameText.Text = "";
                effectText.Text = "";
            }
            else
            {
                nameText.Text = value.Value.Name;
                effectText.Text = value.Value.Effect;
            }
        }

        private RECT GetCardRect(ResolutionType type, RECT rect)
        {
            RECT res = type switch
            {
                ResolutionType.S21_9 => ResolutionConverter.GetResponsiveRect(new RECT(72, 119, 158, 235), 1580, 677, rect.Width, rect.Height, false),
                _ => ResolutionConverter.GetResponsiveRect(new RECT(64, 116, 152, 226), 1414, 795, rect.Width, rect.Height, false)
            };
            res.AddPos(rect);
            return res;
        }

        private RECT GetEventIcon(ResolutionType type, RECT rect)
        {
            RECT res = type switch
            {
                ResolutionType.S21_9 => ResolutionConverter.GetResponsiveRect(new RECT(72, 145, 158, 208), 1580, 677, rect.Width, rect.Height, false),
                _ => ResolutionConverter.GetResponsiveRect(new RECT(71, 139, 155, 200), 1414, 795, rect.Width, rect.Height, false)
            };
            res.AddPos(rect);
            return res;
        }

        private RECT GetEventTypeRect(ResolutionType type, RECT rect)
        {
            RECT res = type switch
            {
                ResolutionType.S21_9 => ResolutionConverter.GetResponsiveRect(new RECT(180, 149, 336, 171), 1580, 677, rect.Width, rect.Height, false),
                _ => ResolutionConverter.GetResponsiveRect(new RECT(171, 146, 344, 165), 1414, 795, rect.Width, rect.Height, false)
            };
            res.AddPos(rect);
            return res;
        }

        private RECT GetEventNameRect(ResolutionType type, RECT rect)
        {
            RECT res = type switch
            {
                ResolutionType.S21_9 => ResolutionConverter.GetResponsiveRect(new RECT(198, 186, 494, 219), 1715, 735, rect.Width, rect.Height, false),
                _ => ResolutionConverter.GetResponsiveRect(new RECT(174, 165, 431, 192), 1414, 795, rect.Width, rect.Height, false)
            };
            res.AddPos(rect);
            return res;
        }

        private RECT GetEventSelect1(ResolutionType type, RECT rect)
        {
            RECT res = type switch
            {
                ResolutionType.S21_9 => ResolutionConverter.GetResponsiveRect(new RECT(1131, 377, 1536, 406), 1580, 677, rect.Width, rect.Height, false),
                _ => ResolutionConverter.GetResponsiveRect(new RECT(977, 503, 1332, 536), 1414, 795, rect.Width, rect.Height, false)
            };
            res.AddPos(rect);
            return res;
        }

        private RECT GetEventSelect2(ResolutionType type, RECT rect)
        {
            RECT res = type switch
            {
                ResolutionType.S21_9 => ResolutionConverter.GetResponsiveRect(new RECT(1131, 426, 1536, 461), 1580, 677, rect.Width, rect.Height, false),
                _ => ResolutionConverter.GetResponsiveRect(new RECT(977, 555, 1332, 586), 1414, 795, rect.Width, rect.Height, false)
            };
            res.AddPos(rect);
            return res;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            labelLoading.Text = "갱신 시작...";
            RefreshOCR();
            timer1.Stop();
        }

        private void checkBoxAutoRefresh_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAutoRefresh.Checked)
            {
                if (!timer1.Enabled)
                {
                    timer1.Start();
                }
            }
        }
    }
}
