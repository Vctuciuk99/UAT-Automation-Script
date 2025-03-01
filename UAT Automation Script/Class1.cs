/*using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace UAT_Automation_Script
{
    public partial class Form1 : Form
    {
        private string excelFilePath;
        private string jsonFilePath;

        public Form1()
        {
            InitializeComponent();
        }

        private void browse_excel_fiel_btn_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Excel Files|*.xlsx;*.xls";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                excelFilePath = dialog.FileName;
                txt_excelFilePath.Text = excelFilePath;
            }
        }

        private void browse_json_file_btn_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "JSON Files|*.json";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                jsonFilePath = dialog.FileName;
                txt_jsonfilePath.Text = jsonFilePath;
            }
        }

        private async void verify_file_btn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(excelFilePath) || string.IsNullOrEmpty(jsonFilePath))
            {
                MessageBox.Show("Please select both files", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            verifyingLoader.Text = "Verifying... This may take a while to process.";
            verify_progressBar.Style = ProgressBarStyle.Marquee;
            verify_progressBar.Visible = true;

            bool verificationFinish = await Task.Run(() => VerifyFiles(excelFilePath, jsonFilePath));

            verify_progressBar.Visible = false;
            verifyingLoader.Text = verificationFinish ? "Verifying Done. File will be downloaded automatically" : "Verification Failed!";
            verifyingLoader.ForeColor = verificationFinish ? System.Drawing.Color.Green : System.Drawing.Color.Red;
        }

        private bool VerifyFiles(string excel, string json)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                FileInfo excelFile = new FileInfo(excel);

                using (ExcelPackage package = new ExcelPackage(excelFile))
                {
                    string jsonData = File.ReadAllText(json);
                    JArray logs = JArray.Parse(jsonData);

                    var sheets = package.Workbook.Worksheets;
                    if (sheets.Count < 2) return false;

                    for (int i = 1; i < sheets.Count; i++)
                    {
                        var sheet = sheets[i];
                        if (sheet.Dimension == null) continue;

                        for (int row = 2; row <= sheet.Dimension.Rows; row++)
                        {
                            string excelSig = sheet.Cells[row, 8].Text.Trim();
                            string excelResp = sheet.Cells[row, 9].Text.Trim();

                            if (string.IsNullOrEmpty(excelSig) && string.IsNullOrEmpty(excelResp))
                            {
                                sheet.Cells[row, 12].Value = "SKIPPED";
                                continue;
                            }

                            excelSig = Regex.Match(excelSig, "'Signature' => '([^']+)'")?.Groups[1].Value ?? excelSig;
                            excelResp = Regex.Match(excelResp, "'response' => '([^']+)'")?.Groups[1].Value ?? excelResp;

                            bool matched = false;

                            foreach (JObject log in logs)
                            {
                                string logText = log["Content"]?.ToString() ?? "";
                                string logSig = Regex.Match(logText, "- Signature: ([A-Za-z0-9+/=]+)").Groups[1].Value;
                                string logResp = Regex.Match(logText, "- BALANCE result : (\\{[^}]+\\})").Groups[1].Value;

                                if (logSig == excelSig && logResp == excelResp)
                                {
                                    matched = true;
                                    break;
                                }
                            }

                            sheet.Cells[row, 12].Value = matched ? "Passed" : "Failed";
                        }
                    }

                    string newFile = Path.Combine(Path.GetDirectoryName(excel), "Verified_" + Path.GetFileName(excel));
                    package.SaveAs(new FileInfo(newFile));
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
*/