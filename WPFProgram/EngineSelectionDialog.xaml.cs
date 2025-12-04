using EndoAshu.StarSavior.Core;
using EndoAshu.StarSavior.Core.Search;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StarSaviorAssistant
{
    public partial class EngineSelectionDialog : Window
    {
        public EngineSelectionDialog()
        {
            InitializeComponent();
            LoadEngines();
        }
        private void LoadEngines()
        {
            EngineList.ItemsSource = SearchEngine.Items;
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            var select = EngineList.SelectedItem as AbstractSearchEngine;

            if (select == null)
            {
                MessageBox.Show("검색 엔진을 선택해 주세요.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            SearchEngine.Current = select;

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
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
