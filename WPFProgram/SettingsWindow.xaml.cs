using EndoAshu.StarSavior.Core;
using EndoAshu.StarSavior.Core.Search;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StarSaviorAssistant
{
    /// <summary>
    /// SettingsWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly Window _targetWindow;

        public SettingsWindow(Window targetWindow)
        {
            InitializeComponent();
            _targetWindow = targetWindow;

            ChkTopMost.IsChecked = _targetWindow.Topmost;
            var name = SearchEngine.Current?.Name ?? "<None>";
            TxtSelectedEngine.Text = name;
            Topmost = _targetWindow.Topmost;

            if (Settings.Theme == "Light") CmbThemeSelector.SelectedIndex = 0;
            else if (Settings.Theme == "Dark") CmbThemeSelector.SelectedIndex = 1;
            else if (Settings.Theme == "Blue") CmbThemeSelector.SelectedIndex = 2;
            else if (Settings.Theme == "Purple") CmbThemeSelector.SelectedIndex = 3;
            else CmbThemeSelector.SelectedIndex = 1;
        }
        private void CmbThemeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbThemeSelector.SelectedItem is ComboBoxItem selectedItem)
            {
                string themeTag = selectedItem.Tag.ToString();
                ThemeManager.SetTheme(themeTag);
                Settings.Save();
            }
        }

        private void TopMost_Checked(object sender, RoutedEventArgs e)
        {
            if (_targetWindow != null)
            {
                _targetWindow.Topmost = true;
                this.Topmost = true;
                Settings.TopMost = true;
                Settings.Save();
            }
        }

        private void TopMost_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_targetWindow != null)
            {
                _targetWindow.Topmost = false;
                this.Topmost = false;
                Settings.TopMost = false;
                Settings.Save();
            }
        }

        private void SelectEngine_Click(object sender, RoutedEventArgs e)
        {
            EngineSelectionDialog dialog = new EngineSelectionDialog();

            dialog.Owner = this;
            dialog.Topmost = this.Topmost;

            if (dialog.ShowDialog() == true)
            {
                var name = SearchEngine.Current?.Name ?? "<None>";
                TxtSelectedEngine.Text = name;
                Settings.Save();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}
