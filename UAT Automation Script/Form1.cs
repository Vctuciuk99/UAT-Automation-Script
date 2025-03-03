using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Globalization;
using System.Text;


//TODO: pag REST API dat gagana rin to
//TODO: print all failed scenario
//TODO: if endpoint is receipt PASSED as long as signature is available otherwise failed
namespace UAT_Automation_Script
{
    public partial class Form1 : Form
    {
        private string excelFilePath;
        private string jsonFilePath;
        private string newFilePath;

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
            verify_file_btn.Enabled = false;

            bool verificationFinish = await Task.Run(() => verifyUatScript(excelFilePath, jsonFilePath));

            verify_progressBar.Visible = false;
            verifyingLoader.Text = verificationFinish ? "Verifying Done. File will be downloaded automatically" : "Verification Failed!";
            verifyingLoader.ForeColor = verificationFinish ? System.Drawing.Color.Green : System.Drawing.Color.Red;
            lbl_filePath.Text = ("File saved at: " + newFilePath);
            verify_file_btn.Enabled = true;
            jsonFilePath = null;
            excelFilePath = null;
            txt_excelFilePath.Text = "";
            txt_jsonfilePath.Text = "";


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
                        int endpointColumn = 1;
                        int expectedResultSheet = 6;
                        int signatureColumnIndex = 8;
                        int actualResponseColumnIndex = 9;
                        int remarksColumnIndex = 12;
                        int commentColumnIndex = 13;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            string clientActualSignature = uatWorkSheet.Cells[row, signatureColumnIndex].Text.Trim();
                            string clientActualResponse = uatWorkSheet.Cells[row, actualResponseColumnIndex].Text.Trim();
                            string endpoint = uatWorkSheet.Cells[row, endpointColumn].Text.Trim();
                            string expectedResult = uatWorkSheet.Cells[row, expectedResultSheet].Text.Trim();

                            // Check if the whole row is empty
                            bool isRowEmpty = true;
                            for (int col = 1; col <= uatWorkSheet.Dimension.Columns; col++)
                            {
                                if (!string.IsNullOrWhiteSpace(uatWorkSheet.Cells[row, col].Text))
                                {
                                    isRowEmpty = false;
                                    break;
                                }
                            }
                            if (isRowEmpty) continue;

                            // para ma gaya format ng json logs
                            string formatActualResponse = clientActualResponse
                                .Replace("\n", "")
                                .Replace("\r", "")
                                .Replace("  ", "")  
                                .Trim();

                            formatActualResponse = Regex.Replace(formatActualResponse, @"\s*:\s*", ":");
                            formatActualResponse = Regex.Replace(formatActualResponse, @"\s*,\s*", ",");  
                            string finalFormattedResponse = formatActualResponse.Replace("\"", "\\\"");

                            
                            //Console.WriteLine(expectedResult);
                            var expectedCodeRemarks = JsonConvert.DeserializeObject<dynamic>(expectedResult);
                            var actualCodeRemarks = JsonConvert.DeserializeObject<dynamic>(clientActualResponse);

                            string expectedresponseCode = "200";
                            string expectedRemarks = expectedCodeRemarks.remarks;
                            string actualresponseCode = actualCodeRemarks.responseCode;
                            string actualRemarks = actualCodeRemarks.remarks;

                            Console.WriteLine($"Expected Response Code: \"{expectedresponseCode}\"");
                            Console.WriteLine($"Expected Remarks: \"{expectedRemarks}\"");
                            Console.WriteLine($"Actual Response Code: \"{actualresponseCode}\"");
                            Console.WriteLine($"ActualRemarks: \"{actualRemarks}\"");

                            Console.WriteLine($"Curent Sheet: \"{uatWorkSheet.Name}\"");
                            Console.WriteLine($"Current Endpoint: \"{endpoint}\"");
                            Console.WriteLine($"Column: \"{signatureColumnIndex}\" Row: \"{row}\"");
                            Console.WriteLine($"Excel Signature: \"{clientActualSignature}\"");
                            Console.WriteLine($"Excel Actual Response: \"{finalFormattedResponse}\"");


                            //STEP 1: Check of scenario is skipped
                            if (string.IsNullOrEmpty(clientActualSignature) && string.IsNullOrEmpty(clientActualResponse))
                            {
                                uatWorkSheet.Cells[row, remarksColumnIndex].Value = "SKIPPED";
                                uatWorkSheet.Cells[row, commentColumnIndex].Value = "The scenario is skipped by the client";
                                continue;
                            }

                            //STEP 2: check if expected api response code == actual api response code
                            if (expectedresponseCode == actualresponseCode && expectedRemarks == actualRemarks)
                            {
                                Console.WriteLine("STEP 2 PASSED!!!");
                                continue;
                            }
                            else
                            {
                                uatWorkSheet.Cells[row, remarksColumnIndex].Value = "FAILED";
                                uatWorkSheet.Cells[row, commentColumnIndex].Value = "Expected response code and remarks did not meet";
                                break;
                            }

                            JArray jsonObjects = JArray.Parse(jsonFileLog);
                            bool matchFound = false;

                            //Step 3: check if existing on logs
                            foreach (JObject jsonObject in jsonObjects)
                            {
                                string jsonLog = jsonObject.ToString();

                                if (jsonLog.Contains(clientActualSignature) && jsonLog.Contains(finalFormattedResponse))
                                {

                                    matchFound = true;
                                    string dateEntry = jsonObject["dateEntry"]["$date"].ToString();
                                    DateTime parsedDate = DateTime.Parse(dateEntry, null, System.Globalization.DateTimeStyles.RoundtripKind);
                                    string formattedDateEntry = parsedDate.ToString("yyyy-MM-ddTHH:mm:ss");
                                    Console.WriteLine($"Match Found! Date Entry: {formattedDateEntry}");
                                    Console.WriteLine("==================================================================================================================================");
                                    //Console.WriteLine("Remarks: Passed");
                                    uatWorkSheet.Cells[row, remarksColumnIndex].Value = "PASSED";
                                    uatWorkSheet.Cells[row, commentColumnIndex].Value = "Scenario is verified, log date entry was: " + formattedDateEntry;
                                    break;

                                }
                                
                            }

                            if (!matchFound)
                            {
                                //Console.WriteLine("Remarks: Failed");
                                uatWorkSheet.Cells[row, remarksColumnIndex].Value = "FAILED";
                                uatWorkSheet.Cells[row, commentColumnIndex].Value = "Verification failed. No logs found containing the signature or actual response";
                            }
                            
                        }

                    }
                    // **SAVE UPDATED FILE**
                    string directory = Path.GetDirectoryName(excelFile);
                    string newFileName = Path.Combine(directory, $"Verified_{Path.GetFileName(excelFile)}");
                    package.SaveAs(new FileInfo(newFileName));

                    newFilePath = newFileName; // Store the new file path for display

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
