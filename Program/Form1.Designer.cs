namespace Program
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            button1 = new Button();
            panel1 = new Panel();
            labelEventSelect1Effect = new Label();
            labelEventSelect1Name = new Label();
            labelEventName = new Label();
            labelEventType = new Label();
            panel2 = new Panel();
            labelEventSelect2Effect = new Label();
            labelEventSelect2Name = new Label();
            panel3 = new Panel();
            labelEventSelect3Effect = new Label();
            labelEventSelect3Name = new Label();
            panel4 = new Panel();
            labelEventSelect4Effect = new Label();
            labelEventSelect4Name = new Label();
            labelLoading = new Label();
            timer1 = new System.Windows.Forms.Timer(components);
            checkBoxAutoRefresh = new CheckBox();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            panel4.SuspendLayout();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(321, 44);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 0;
            button1.Text = "수동 갱신";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(labelEventSelect1Effect);
            panel1.Controls.Add(labelEventSelect1Name);
            panel1.Location = new Point(12, 87);
            panel1.Name = "panel1";
            panel1.Size = new Size(384, 176);
            panel1.TabIndex = 1;
            // 
            // labelEventSelect1Effect
            // 
            labelEventSelect1Effect.AutoSize = true;
            labelEventSelect1Effect.Location = new Point(7, 48);
            labelEventSelect1Effect.MaximumSize = new Size(370, 0);
            labelEventSelect1Effect.Name = "labelEventSelect1Effect";
            labelEventSelect1Effect.Size = new Size(39, 15);
            labelEventSelect1Effect.TabIndex = 1;
            labelEventSelect1Effect.Text = "label2";
            // 
            // labelEventSelect1Name
            // 
            labelEventSelect1Name.AutoSize = true;
            labelEventSelect1Name.Font = new Font("맑은 고딕", 13F);
            labelEventSelect1Name.Location = new Point(7, 9);
            labelEventSelect1Name.MaximumSize = new Size(370, 0);
            labelEventSelect1Name.Name = "labelEventSelect1Name";
            labelEventSelect1Name.Size = new Size(60, 25);
            labelEventSelect1Name.TabIndex = 0;
            labelEventSelect1Name.Text = "label1";
            // 
            // labelEventName
            // 
            labelEventName.AutoSize = true;
            labelEventName.Font = new Font("맑은 고딕", 13F);
            labelEventName.Location = new Point(12, 44);
            labelEventName.Name = "labelEventName";
            labelEventName.Size = new Size(60, 25);
            labelEventName.TabIndex = 2;
            labelEventName.Text = "label3";
            // 
            // labelEventType
            // 
            labelEventType.AutoSize = true;
            labelEventType.Font = new Font("맑은 고딕", 15F);
            labelEventType.Location = new Point(12, 9);
            labelEventType.Name = "labelEventType";
            labelEventType.Size = new Size(66, 28);
            labelEventType.TabIndex = 3;
            labelEventType.Text = "label3";
            // 
            // panel2
            // 
            panel2.Controls.Add(labelEventSelect2Effect);
            panel2.Controls.Add(labelEventSelect2Name);
            panel2.Location = new Point(12, 269);
            panel2.Name = "panel2";
            panel2.Size = new Size(384, 176);
            panel2.TabIndex = 2;
            // 
            // labelEventSelect2Effect
            // 
            labelEventSelect2Effect.AutoSize = true;
            labelEventSelect2Effect.Location = new Point(7, 48);
            labelEventSelect2Effect.MaximumSize = new Size(370, 0);
            labelEventSelect2Effect.Name = "labelEventSelect2Effect";
            labelEventSelect2Effect.Size = new Size(39, 15);
            labelEventSelect2Effect.TabIndex = 1;
            labelEventSelect2Effect.Text = "label2";
            // 
            // labelEventSelect2Name
            // 
            labelEventSelect2Name.AutoSize = true;
            labelEventSelect2Name.Font = new Font("맑은 고딕", 13F);
            labelEventSelect2Name.Location = new Point(7, 9);
            labelEventSelect2Name.MaximumSize = new Size(370, 0);
            labelEventSelect2Name.Name = "labelEventSelect2Name";
            labelEventSelect2Name.Size = new Size(60, 25);
            labelEventSelect2Name.TabIndex = 0;
            labelEventSelect2Name.Text = "label1";
            // 
            // panel3
            // 
            panel3.Controls.Add(labelEventSelect3Effect);
            panel3.Controls.Add(labelEventSelect3Name);
            panel3.Location = new Point(12, 451);
            panel3.Name = "panel3";
            panel3.Size = new Size(384, 176);
            panel3.TabIndex = 2;
            // 
            // labelEventSelect3Effect
            // 
            labelEventSelect3Effect.AutoSize = true;
            labelEventSelect3Effect.Location = new Point(7, 48);
            labelEventSelect3Effect.MaximumSize = new Size(370, 0);
            labelEventSelect3Effect.Name = "labelEventSelect3Effect";
            labelEventSelect3Effect.Size = new Size(39, 15);
            labelEventSelect3Effect.TabIndex = 1;
            labelEventSelect3Effect.Text = "label2";
            // 
            // labelEventSelect3Name
            // 
            labelEventSelect3Name.AutoSize = true;
            labelEventSelect3Name.Font = new Font("맑은 고딕", 13F);
            labelEventSelect3Name.Location = new Point(7, 9);
            labelEventSelect3Name.MaximumSize = new Size(370, 0);
            labelEventSelect3Name.Name = "labelEventSelect3Name";
            labelEventSelect3Name.Size = new Size(60, 25);
            labelEventSelect3Name.TabIndex = 0;
            labelEventSelect3Name.Text = "label1";
            // 
            // panel4
            // 
            panel4.Controls.Add(labelEventSelect4Effect);
            panel4.Controls.Add(labelEventSelect4Name);
            panel4.Location = new Point(12, 633);
            panel4.Name = "panel4";
            panel4.Size = new Size(384, 176);
            panel4.TabIndex = 4;
            // 
            // labelEventSelect4Effect
            // 
            labelEventSelect4Effect.AutoSize = true;
            labelEventSelect4Effect.Location = new Point(7, 48);
            labelEventSelect4Effect.MaximumSize = new Size(370, 0);
            labelEventSelect4Effect.Name = "labelEventSelect4Effect";
            labelEventSelect4Effect.Size = new Size(39, 15);
            labelEventSelect4Effect.TabIndex = 1;
            labelEventSelect4Effect.Text = "label2";
            // 
            // labelEventSelect4Name
            // 
            labelEventSelect4Name.AutoSize = true;
            labelEventSelect4Name.Font = new Font("맑은 고딕", 13F);
            labelEventSelect4Name.Location = new Point(7, 9);
            labelEventSelect4Name.MaximumSize = new Size(370, 0);
            labelEventSelect4Name.Name = "labelEventSelect4Name";
            labelEventSelect4Name.Size = new Size(60, 25);
            labelEventSelect4Name.TabIndex = 0;
            labelEventSelect4Name.Text = "label1";
            // 
            // labelLoading
            // 
            labelLoading.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelLoading.Font = new Font("맑은 고딕", 8F);
            labelLoading.Location = new Point(199, 9);
            labelLoading.Name = "labelLoading";
            labelLoading.Size = new Size(197, 28);
            labelLoading.TabIndex = 5;
            labelLoading.Text = "labelLoading";
            labelLoading.TextAlign = ContentAlignment.TopRight;
            // 
            // timer1
            // 
            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick;
            // 
            // checkBoxAutoRefresh
            // 
            checkBoxAutoRefresh.Location = new Point(234, 43);
            checkBoxAutoRefresh.Name = "checkBoxAutoRefresh";
            checkBoxAutoRefresh.Size = new Size(81, 24);
            checkBoxAutoRefresh.TabIndex = 6;
            checkBoxAutoRefresh.Text = "자동 갱신";
            checkBoxAutoRefresh.UseVisualStyleBackColor = true;
            checkBoxAutoRefresh.CheckedChanged += checkBoxAutoRefresh_CheckedChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(408, 826);
            Controls.Add(checkBoxAutoRefresh);
            Controls.Add(labelLoading);
            Controls.Add(panel4);
            Controls.Add(panel3);
            Controls.Add(panel2);
            Controls.Add(labelEventType);
            Controls.Add(labelEventName);
            Controls.Add(panel1);
            Controls.Add(button1);
            MaximizeBox = false;
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Panel panel1;
        private Label labelEventSelect1Effect;
        private Label labelEventSelect1Name;
        private Label labelEventName;
        private Label labelEventType;
        private Panel panel2;
        private Label labelEventSelect2Effect;
        private Label labelEventSelect2Name;
        private Panel panel3;
        private Label labelEventSelect3Effect;
        private Label labelEventSelect3Name;
        private Panel panel4;
        private Label labelEventSelect4Effect;
        private Label labelEventSelect4Name;
        private Label labelLoading;
        private System.Windows.Forms.Timer timer1;
        private CheckBox checkBoxAutoRefresh;
    }
}
