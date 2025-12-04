using EndoAshu.StarSavior.Core;
using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Tesseract;

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

            progressBar1.Maximum = 100;
            progressBar2.Maximum = 100;

            PreloadLogic.StartLoad(UpdateUI, ShowYesNo, ShowError, () =>
            {
                _loaded = true;
                Close();
            }, Close);
        }

        private void ShowError(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private bool ShowYesNo(string title, string message)
        {
            return MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes;
        }

        private void UpdateUI(int totalIdx, int totalMax, string what, int current, string currentWhat)
        {
            progressBar1.Value = (int)(totalIdx * 100.0f / totalMax);
            progressBar2.Value = current;

            label1.Text = $"{what} ({totalIdx}/{totalMax})";
            label2.Text = $"{currentWhat} ({current}%)";
        }
    }
}
