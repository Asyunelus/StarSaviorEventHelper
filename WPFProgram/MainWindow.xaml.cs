using EndoAshu.StarSavior.Core;
using EndoAshu.StarSavior.Core.Search;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StarSaviorAssistant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private enum RefreshState
        {
            Wait,
            Search,
            Success,
            Failure
        }

        public class ChoiceItem
        {
            public string Title { get; set; } = string.Empty;
            public string FixedOutcome { get; set; } = string.Empty;
            public string SuccessOutcome { get; set; } = string.Empty;
            public string FailureOutcome { get; set; } = string.Empty;

            public ChoiceItem()
            {

            }

            public ChoiceItem(string title, string fixedOutcome, string successOutcome, string failureOutcome)
            {
                Title = title;
                FixedOutcome = fixedOutcome;
                SuccessOutcome = successOutcome;
                FailureOutcome = failureOutcome;
            }
        }

        private CancellationTokenSource? _cts;
        private bool _isAutoRefreshEnabled = false;
        private volatile bool _isRefreshing = false;
        private RefreshState _refreshState = RefreshState.Wait;

        public MainWindow()
        {
            InitializeComponent();

            Topmost = Settings.TopMost;
            UpdateUIState();

            this.Closed += MainWindow_Closed;

            ChoicesListBox.ItemsSource = new List<ChoiceItem>();
            /*
            {
                new ChoiceItem("선택지A", "스태미나 +10, 공격의 성흔", string.Empty, string.Empty),
                new ChoiceItem("선택지B", "스태미나 +10", "힘+20", string.Empty),
                new ChoiceItem("선택지C", "스태미나 +10", string.Empty, "힘-30"),
                new ChoiceItem("선택지D", "스태미나 +10", "힘+20", "힘-30")
            };*/
        }

        private void MainWindow_Closed(object? sender, System.EventArgs e)
        {
            _cts?.Cancel();
        }

        private void AutoRefresh_Checked(object sender, RoutedEventArgs e)
        {
            _isAutoRefreshEnabled = true;
            _cts = new CancellationTokenSource();
            Task.Run(() => AutoRefreshLoop(_cts.Token));
        }

        private void AutoRefresh_Unchecked(object sender, RoutedEventArgs e)
        {
            _isAutoRefreshEnabled = false;
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        private void ManualRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (_isRefreshing)
            {
                return;
            }

            _cts?.Cancel();

            SearchRefresh().ContinueWith(t =>
            {
                if (_isAutoRefreshEnabled)
                {
                    _cts?.Dispose();
                    _cts = new CancellationTokenSource();
#pragma warning disable CS4014
                    Task.Run(() => AutoRefreshLoop(_cts.Token));
#pragma warning restore CS4014
                }
            });
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWin = new SettingsWindow(this);
            settingsWin.ShowDialog();
        }
        private void UpdateUIState()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                BtnManualRefresh.IsEnabled = !_isRefreshing;

                TxtTitle.Text = $"스타세이비어 어시스턴트 {DataServer.VERSION}";
                string statusColor = _refreshState switch
                {
                    RefreshState.Success => "FixedColor_Success_Color",
                    RefreshState.Failure => "FixedColor_Failure_Color",
                    _ => ThemeManager.MAIN_FG_KEY
                };
                
                if (Application.Current.Resources[statusColor] is SolidColorBrush existingBrush) {
                    TxtStatus.Foreground = existingBrush;
                }
            });
        }

        private async Task AutoRefreshLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                bool success = await SearchRefresh();
                int delayTime = success ? 3000 : 1000;

                try
                {
                    await Task.Delay(delayTime, token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

        private async Task<bool> SearchRefresh()
        {
            _isRefreshing = true;
            _refreshState = RefreshState.Search;
            UpdateUIState();

            if (SearchEngine.Current != null)
            {
                SearchResult res = await SearchEngine.Current.Search("스타세이비어");

                if (res.IsFailed)
                {
                    _refreshState = RefreshState.Failure;
                    Application.Current.Dispatcher.Invoke(() => {
                        ChoicesListBox.ItemsSource = new List<ChoiceItem>();
                        TxtEventType.Text = "Not Detected!";
                        TxtTargetName.Text = res.Type.ToString();
                        TxtEventName.Text = $"DebugText : {(res.ErrorObject?.ToString() ?? "<none>")}";
                        CardImage.Source = null;
                    });
                }
                else
                {
                    _refreshState = RefreshState.Success;
                    var newItems = new List<ChoiceItem>();

                    if (res.IsJourney)
                    {
                        var data = res.Journey!.Data!;
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            TxtEventType.Text = "여정 이벤트";
                            TxtTargetName.Text = data.Name;
                            CardImage.Source = null;

                            var arr = data.Choices.Select(e =>
                            {
                                return new ChoiceItem()
                                {
                                    Title = e.Text,
                                    FixedOutcome = e.Result,
                                    SuccessOutcome = e.ResultPositive,
                                    FailureOutcome = e.ResultNegative
                                };
                            }).ToArray();

                            newItems.AddRange(arr);
                        });
                    }
                    else if (res.IsArcana)
                    {
                        var data = res.Arcana!.Data!;
                        int eventIdx = res.Arcana!.EventIndex;

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            TxtEventType.Text = "아르카나 이벤트";
                            TxtTargetName.Text = data.Name;
                        });

                        if (eventIdx >= 0 && eventIdx < data.CardEvents.Count)
                        {
                            var ev = data.CardEvents[eventIdx];

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                TxtEventName.Text = ev.Name;
                            });

                            var arr = ev.Entry.Selects.Select(e =>
                            {
                                string selectName = e.Item1;

                                var fix = e.Item2.FirstOrDefault(e => e.Type == "고정");
                                var succ = e.Item2.FirstOrDefault(e => e.Type == "성공");
                                var fail = e.Item2.FirstOrDefault(e => e.Type == "실패");

                                return new ChoiceItem()
                                {
                                    Title = selectName,
                                    FixedOutcome = fix != null ? string.Join(", ", fix.Effect.Select(e => e.ToString())) : string.Empty,
                                    SuccessOutcome = succ != null ? string.Join(", ", succ.Effect.Select(e => e.ToString())) : string.Empty,
                                    FailureOutcome = fail != null ? string.Join(", ", fail.Effect.Select(e => e.ToString())) : string.Empty
                                };
                            }).ToArray();
                            newItems.AddRange(arr);
                        }
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ChoicesListBox.ItemsSource = newItems;
                        CardImage.Source = res.IsArcana ? BitmapFromFile(res.Arcana!.Data!.Image) : null;
                    });
                }
            }

            _isRefreshing = false;
            UpdateUIState();
            return _refreshState == RefreshState.Success;
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private BitmapImage? BitmapFromFile(string filePath)
        {
            if (!File.Exists(filePath)) return null;

            var bitmap = new BitmapImage();
            bitmap.BeginInit();

            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bitmap.UriSource = new Uri(Path.GetFullPath(filePath), UriKind.Absolute);

            bitmap.EndInit();
            bitmap.Freeze();

            return bitmap;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}