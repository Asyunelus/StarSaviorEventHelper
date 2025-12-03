using EndoAshu.StarSavior.Core;
using EndoAshu.StarSavior.Core.Search;

namespace Program
{
    public partial class SettingForm : Form
    {

        public SettingForm()
        {
            InitializeComponent();


            comboBox1.SelectedIndexChanged -= comboBox1_SelectedIndexChanged;
            RefreshComboBox();
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
        }

        private void RefreshComboBox()
        {
            comboBox1.DataSource = SearchEngine.Items.ToList();
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "Name";
            if (SearchEngine.Current != null)
            {
                comboBox1.SelectedValue = SearchEngine.Current.Name;
            }
            else
            {
                comboBox1.SelectedIndex = -1;
            }

            label1.Text = SearchEngine.Current != null ? SearchEngine.Current.Name : "선택된 모델 없음";
            label2.Text = SearchEngine.Current != null ? SearchEngine.Current.Description : "위 콤보박스에서 모델을 선택해주세요.";
        }

        private void comboBox1_SelectedIndexChanged(object? sender, EventArgs e)
        {
            SearchEngine.Current = comboBox1.SelectedItem as AbstractSearchEngine;

            label1.Text = SearchEngine.Current != null ? SearchEngine.Current.Name : "선택된 모델 없음";
            label2.Text = SearchEngine.Current != null ? SearchEngine.Current.Description : "위 콤보박스에서 모델을 선택해주세요.";

            Settings.Save();
        }
    }
}
