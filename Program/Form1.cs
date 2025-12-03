using EndoAshu.StarSavior.Core;
using EndoAshu.StarSavior.Core.Search;

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
            ApplyNone(new SearchResult(SearchResultType.Failed_SearchWait));
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
                var result = await SearchEngine.Current.Search(reader, "스타세이비어");
                if (result.IsFailed)
                {
                    ApplyNone(result);
                }
                else
                {
                    ApplyText(
                        result.Type switch
                        {
                            SearchResultType.Journey => "여정 이벤트",
                            SearchResultType.Arcana => "아르카나 이벤트",
                            _ => "Unknown Error",
                        },
                        result.Type switch
                        {
                            SearchResultType.Journey => result.Journey!.Data!.Name,
                            SearchResultType.Arcana => $"{(result.Arcana!.EventIndex >= 0 ? result.Arcana!.Data!.CardEvents[result.Arcana!.EventIndex].Name : "Undefined")} ({result.Arcana!.Data!.Name})",
                            _ => "Unknown Error",
                        },
                        result.Type switch {
                            SearchResultType.Journey => result.FromJourney(),
                            SearchResultType.Arcana => result.FromArcana(),
                            _ => Array.Empty<ApplyItem>(),
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                ApplyNone(new SearchResult(SearchResultType.Failed_Exception, ex));
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

        public void ApplyNone(SearchResult result)
        {
            if (result.ErrorObject != null)
            {
                labelEventName.Text = $"{result.Type.ToString()} ({result.ErrorObject.ToString()})";
            } else
            {
                labelEventName.Text = result.Type.ToString();
            }
            labelEventType.Text = "Not Detected";
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
            } else
            {
                labelLoading.Text = "화면인식 실패...";
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
