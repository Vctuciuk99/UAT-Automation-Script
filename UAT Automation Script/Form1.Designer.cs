namespace UAT_Automation_Script
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
            label1 = new Label();
            label2 = new Label();
            txt_jsonfilePath = new TextBox();
            label3 = new Label();
            verify_file_btn = new Button();
            txt_excelfilePath = new TextBox();
            browse_excel_fiel_btn = new Button();
            browse_json_file_btn = new Button();
            excel_errorMsgLabel = new Label();
            json_errorMsgLabel = new Label();
            verifyingLoader = new Label();
            verify_progressBar = new ProgressBar();
            lbl_filePath = new Label();
            apiDropdown = new ComboBox();
            label4 = new Label();
            failedScenariosListBox = new ListBox();
            api_errorMsgLabel = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(309, 9);
            label1.Name = "label1";
            label1.Size = new Size(258, 38);
            label1.TabIndex = 2;
            label1.Text = "UAT Script Verifier";
            label1.TextAlign = ContentAlignment.TopCenter;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(12, 152);
            label2.Name = "label2";
            label2.Size = new Size(164, 28);
            label2.TabIndex = 3;
            label2.Text = "Upload Excel File:";
            // 
            // txt_jsonfilePath
            // 
            txt_jsonfilePath.BackColor = SystemColors.ControlLightLight;
            txt_jsonfilePath.Font = new Font("Segoe UI", 12F);
            txt_jsonfilePath.Location = new Point(225, 222);
            txt_jsonfilePath.Name = "txt_jsonfilePath";
            txt_jsonfilePath.ReadOnly = true;
            txt_jsonfilePath.Size = new Size(551, 34);
            txt_jsonfilePath.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(12, 228);
            label3.Name = "label3";
            label3.Size = new Size(207, 28);
            label3.TabIndex = 6;
            label3.Text = "Upload JSON Log File:";
            // 
            // verify_file_btn
            // 
            verify_file_btn.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            verify_file_btn.Location = new Point(309, 288);
            verify_file_btn.Name = "verify_file_btn";
            verify_file_btn.Size = new Size(250, 55);
            verify_file_btn.TabIndex = 7;
            verify_file_btn.Text = "Verify Data";
            verify_file_btn.UseVisualStyleBackColor = true;
            verify_file_btn.Click += verify_file_btn_Click;
            // 
            // txt_excelfilePath
            // 
            txt_excelfilePath.BackColor = SystemColors.ControlLightLight;
            txt_excelfilePath.Font = new Font("Segoe UI", 12F);
            txt_excelfilePath.Location = new Point(182, 146);
            txt_excelfilePath.Name = "txt_excelfilePath";
            txt_excelfilePath.ReadOnly = true;
            txt_excelfilePath.Size = new Size(594, 34);
            txt_excelfilePath.TabIndex = 11;
            // 
            // browse_excel_fiel_btn
            // 
            browse_excel_fiel_btn.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            browse_excel_fiel_btn.Location = new Point(782, 146);
            browse_excel_fiel_btn.Name = "browse_excel_fiel_btn";
            browse_excel_fiel_btn.Size = new Size(83, 34);
            browse_excel_fiel_btn.TabIndex = 12;
            browse_excel_fiel_btn.Text = "Browse";
            browse_excel_fiel_btn.UseVisualStyleBackColor = true;
            browse_excel_fiel_btn.Click += browse_excel_fiel_btn_Click;
            // 
            // browse_json_file_btn
            // 
            browse_json_file_btn.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            browse_json_file_btn.Location = new Point(782, 222);
            browse_json_file_btn.Name = "browse_json_file_btn";
            browse_json_file_btn.Size = new Size(83, 34);
            browse_json_file_btn.TabIndex = 13;
            browse_json_file_btn.Text = "Browse";
            browse_json_file_btn.UseVisualStyleBackColor = true;
            browse_json_file_btn.Click += browse_json_file_btn_Click;
            // 
            // excel_errorMsgLabel
            // 
            excel_errorMsgLabel.AutoSize = true;
            excel_errorMsgLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            excel_errorMsgLabel.ForeColor = Color.Red;
            excel_errorMsgLabel.Location = new Point(182, 183);
            excel_errorMsgLabel.Name = "excel_errorMsgLabel";
            excel_errorMsgLabel.Size = new Size(0, 20);
            excel_errorMsgLabel.TabIndex = 14;
            // 
            // json_errorMsgLabel
            // 
            json_errorMsgLabel.AutoSize = true;
            json_errorMsgLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            json_errorMsgLabel.ForeColor = Color.Red;
            json_errorMsgLabel.Location = new Point(225, 259);
            json_errorMsgLabel.Name = "json_errorMsgLabel";
            json_errorMsgLabel.Size = new Size(0, 20);
            json_errorMsgLabel.TabIndex = 15;
            // 
            // verifyingLoader
            // 
            verifyingLoader.AutoSize = true;
            verifyingLoader.Font = new Font("Segoe UI", 12F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            verifyingLoader.Location = new Point(12, 357);
            verifyingLoader.Name = "verifyingLoader";
            verifyingLoader.Size = new Size(0, 28);
            verifyingLoader.TabIndex = 16;
            // 
            // verify_progressBar
            // 
            verify_progressBar.Location = new Point(12, 388);
            verify_progressBar.Name = "verify_progressBar";
            verify_progressBar.Size = new Size(853, 29);
            verify_progressBar.TabIndex = 17;
            // 
            // lbl_filePath
            // 
            lbl_filePath.AutoSize = true;
            lbl_filePath.Location = new Point(12, 420);
            lbl_filePath.Name = "lbl_filePath";
            lbl_filePath.Size = new Size(0, 20);
            lbl_filePath.TabIndex = 18;
            // 
            // apiDropdown
            // 
            apiDropdown.FormattingEnabled = true;
            apiDropdown.Items.AddRange(new object[] { "Unified API", "Rest API" });
            apiDropdown.Location = new Point(12, 94);
            apiDropdown.Name = "apiDropdown";
            apiDropdown.Size = new Size(151, 28);
            apiDropdown.TabIndex = 19;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F);
            label4.Location = new Point(12, 63);
            label4.Name = "label4";
            label4.Size = new Size(41, 28);
            label4.TabIndex = 20;
            label4.Text = "API";
            // 
            // failedScenariosListBox
            // 
            failedScenariosListBox.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            failedScenariosListBox.FormattingEnabled = true;
            failedScenariosListBox.ItemHeight = 28;
            failedScenariosListBox.Location = new Point(12, 443);
            failedScenariosListBox.Name = "failedScenariosListBox";
            failedScenariosListBox.Size = new Size(853, 340);
            failedScenariosListBox.TabIndex = 21;
            // 
            // api_errorMsgLabel
            // 
            api_errorMsgLabel.AutoSize = true;
            api_errorMsgLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            api_errorMsgLabel.ForeColor = Color.Red;
            api_errorMsgLabel.Location = new Point(169, 97);
            api_errorMsgLabel.Name = "api_errorMsgLabel";
            api_errorMsgLabel.Size = new Size(0, 20);
            api_errorMsgLabel.TabIndex = 22;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(877, 821);
            Controls.Add(api_errorMsgLabel);
            Controls.Add(failedScenariosListBox);
            Controls.Add(label4);
            Controls.Add(apiDropdown);
            Controls.Add(lbl_filePath);
            Controls.Add(verify_progressBar);
            Controls.Add(verifyingLoader);
            Controls.Add(json_errorMsgLabel);
            Controls.Add(excel_errorMsgLabel);
            Controls.Add(browse_json_file_btn);
            Controls.Add(browse_excel_fiel_btn);
            Controls.Add(txt_excelfilePath);
            Controls.Add(verify_file_btn);
            Controls.Add(label3);
            Controls.Add(txt_jsonfilePath);
            Controls.Add(label2);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            Text = "UAT Script Varifier";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label label1;
        private Label label2;
        private TextBox txt_jsonfilePath;
        private Label label3;
        private Button verify_file_btn;
        private TextBox txt_excelfilePath;
        private Button browse_excel_fiel_btn;
        private Button browse_json_file_btn;
        private Label excel_errorMsgLabel;
        private Label json_errorMsgLabel;
        private Label verifyingLoader;
        private ProgressBar verify_progressBar;
        private Label lbl_filePath;
        private ComboBox apiDropdown;
        private Label label4;
        private ListBox failedScenariosListBox;
        private Label api_errorMsgLabel;
    }
}
