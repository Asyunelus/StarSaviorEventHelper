using EndoAshu.StarSavior.Core;
using System.Windows;
using System.Windows.Controls;

namespace StarSaviorAssistant
{
    public partial class Preload : Window
    {
        public Preload()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Settings.Load();
            ThemeManager.SetTheme(Settings.Theme);
            PreloadLogic.StartLoad(UpdateUI, ShowYesNo, ShowError, () =>
            {
                new MainWindow().Show();
                Close();
            }, () =>
            {
                Application.Current.Shutdown();
            });
        }

        private void ShowError(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private bool ShowYesNo(string title, string message)
        {
            return MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes;
        }

        private void UpdateUI(int totalIdx, int totalMax, string what, int current, string currentWhat)
        {
            PbTotal.Value = (int)(totalIdx * 100.0f / totalMax);
            PbDetail.Value = current;
            TxtMainPercent.Text = $"{PbTotal.Value}%";
            TxtDetailPercent.Text = $"{current}%";

            TxtMainStatus.Text = $"{what}";
            TxtDetailStatus.Text = $"{currentWhat}";
        }
    }
}
