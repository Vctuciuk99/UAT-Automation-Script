/*using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;

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
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    excelFilePath = openFileDialog.FileName;
                    txt_excelFilePath.Text = excelFilePath;
                }
            }
        }

        private void browse_json_file_btn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "JSON Files|*.json";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    jsonFilePath = openFileDialog.FileName;
                    txt_jsonfilePath.Text = jsonFilePath;
                }
            }
        }

        private async void verify_file_btn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(excelFilePath) || string.IsNullOrEmpty(jsonFilePath))
            {
                excel_errorMsgLabel.Text = string.IsNullOrEmpty(excelFilePath) ? "Please upload an Excel File" : "";
                json_errorMsgLabel.Text = string.IsNullOrEmpty(jsonFilePath) ? "Please upload a JSON File" : "";
                return;
            }

            verifyingLoader.Text = "Verifying... This may take a while to process.";
            verify_progressBar.Style = ProgressBarStyle.Marquee;
            verify_progressBar.Visible = true;

            bool verificationFinish = await Task.Run(() => verifyUatScript(excelFilePath, jsonFilePath));

            verify_progressBar.Visible = false;
            verifyingLoader.Text = verificationFinish ? "Verifying Done. File will be downloaded automatically" : "Verification Failed!";
            verifyingLoader.ForeColor = verificationFinish ? System.Drawing.Color.Green : System.Drawing.Color.Red;
        }

        private bool verifyUatScript(string excelFile, string jsonFile)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                FileInfo excelFileInfo = new FileInfo(excelFile);

                using (ExcelPackage package = new ExcelPackage(excelFileInfo))
                {
                    string jsonFileLog = File.ReadAllText(jsonFile);
                    JArray jsonarray = JArray.Parse(jsonFileLog);

                    var uatWorkSheets = package.Workbook.Worksheets;
                    if (uatWorkSheets.Count < 2) return false;

                    for (int uatSheetIndex = 1; uatSheetIndex < uatWorkSheets.Count; uatSheetIndex++)
                    {
                        ExcelWorksheet uatWorkSheet = uatWorkSheets[uatSheetIndex];
                        if (uatWorkSheet.Dimension == null) continue;

                        int rowCount = uatWorkSheet.Dimension.Rows;
                        int signatureColumnIndex = 8, actualResponseColumnIndex = 9, remarksColumnIndex = 12, commentColumnIndex = 13;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            string clientActualSignature = uatWorkSheet.Cells[row, signatureColumnIndex].Text.Trim();
                            string clientActualResponse = uatWorkSheet.Cells[row, actualResponseColumnIndex].Text.Trim();

                            // Check if the whole row is empty (all values in the row are empty)
                            bool isRowEmpty = true;
                            for (int col = 1; col <= uatWorkSheet.Dimension.Columns; col++)
                            {
                                if (!string.IsNullOrWhiteSpace(uatWorkSheet.Cells[row, col].Text))
                                {
                                    isRowEmpty = false;
                                    break;
                                }
                            }

                            if (isRowEmpty)
                            {
                                continue; // Skip writing anything for an empty row
                            }

                            // If both Signature and Actual Response are empty, mark as SKIPPED
                            if (string.IsNullOrEmpty(clientActualSignature) && string.IsNullOrEmpty(clientActualResponse))
                            {
                                uatWorkSheet.Cells[row, remarksColumnIndex].Value = "SKIPPED";
                                uatWorkSheet.Cells[row, commentColumnIndex].Value = "The scenario is skipped by the client";
                                continue; // Skip processing further
                            }


                            clientActualSignature = Regex.Match(clientActualSignature, "'Signature' => '([^']+)'")?.Groups[1].Value ?? clientActualSignature;
                            clientActualResponse = Regex.Match(clientActualResponse, "'response' => '([^']+)'")?.Groups[1].Value ?? clientActualResponse;

                            bool matchFound = false;

                            foreach (JObject logEntry in jsonarray)
                            {
                                string logContent = logEntry["Content"]?.ToString() ?? "";

                                Match logSignatureMatch = Regex.Match(logContent, "- Signature: ([A-Za-z0-9+/=]+)");
                                Match logResponseMatch = Regex.Match(logContent, "- BALANCE result : (\\{[^}]+\\})");

                                string logSignature = logSignatureMatch.Success ? logSignatureMatch.Groups[1].Value : "";
                                string logResponse = logResponseMatch.Success ? logResponseMatch.Groups[1].Value : "";
                                // Log values in the Output window for debugging
                                Console.WriteLine($"Excel Signature: '{clientActualSignature}'");
                                Console.WriteLine($"Excel Response: '{clientActualResponse}'");
                                Console.WriteLine($"JSON Signature: '{logSignature}'");
                                Console.WriteLine($"JSON Response: '{logResponse}'");

                                if (logSignature == clientActualSignature && logResponse == clientActualResponse)
                                {
                                    matchFound = true;
                                    break;
                                }
                            }

                            // If no match is found, log the failure
                            if (!matchFound)
                            {
                                Debug.WriteLine("No match found for the given signature and response.");
                            }

                            uatWorkSheet.Cells[row, remarksColumnIndex].Value = matchFound ? "Passed" : "Failed";
                            uatWorkSheet.Cells[row, commentColumnIndex].Value = matchFound ? "Signature and response matched" : "Signature or response mismatch";
                        }
                    }

                    string newFilePath = Path.Combine(Path.GetDirectoryName(excelFile), "Verified_" + Path.GetFileNameWithoutExtension(excelFile) + ".xlsx");
                    package.SaveAs(new FileInfo(newFilePath));
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Verification Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
*/