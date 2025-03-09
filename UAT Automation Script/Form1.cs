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
                    txt_excelfilePath.Text = excelFilePath;
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
            if (string.IsNullOrEmpty(apiDropdown.Text))
            {
                api_errorMsgLabel.Text = string.IsNullOrEmpty(apiDropdown.Text) ? "Please choose API" : "";
                return;
            }
            else if (string.IsNullOrEmpty(excelFilePath) || string.IsNullOrEmpty(jsonFilePath))
            {
                excel_errorMsgLabel.Text = string.IsNullOrEmpty(excelFilePath) ? "Please upload an Excel File" : "";
                json_errorMsgLabel.Text = string.IsNullOrEmpty(jsonFilePath) ? "Please upload a JSON File" : "";
                return;
            }

            verifyingLoader.Text = "Verifying... This may take a while to process.";
            verify_progressBar.Style = ProgressBarStyle.Marquee;
            verify_progressBar.Visible = true;
            verify_file_btn.Enabled = false;

            if (apiDropdown.Text == "Unified API")
            {
                //Console.WriteLine("YEY");
                bool verificationFinishUnifiedAPI = await Task.Run(() => verifyUatScriptUnifiedAPI(excelFilePath, jsonFilePath));

                verify_progressBar.Visible = false;
                verifyingLoader.Text = verificationFinishUnifiedAPI ? "Verifying Done. File will be downloaded automatically" : "Verification Failed!";
                verifyingLoader.ForeColor = verificationFinishUnifiedAPI ? System.Drawing.Color.Green : System.Drawing.Color.Red;
                lbl_filePath.Text = ("File saved at: " + newFilePath);
                verify_file_btn.Enabled = true;
                jsonFilePath = null;
                excelFilePath = null;
                txt_excelfilePath.Text = "";
                txt_jsonfilePath.Text = "";
                api_errorMsgLabel.Text = "";
            }
            else
            {
                bool verificationFinishRestAPI = await Task.Run(() => verifyUatScriptRestAPI(excelFilePath, jsonFilePath));

                verify_progressBar.Visible = false;
                verifyingLoader.Text = verificationFinishRestAPI ? "Verifying Done. File will be downloaded automatically" : "Verification Failed!";
                verifyingLoader.ForeColor = verificationFinishRestAPI ? System.Drawing.Color.Green : System.Drawing.Color.Red;
                lbl_filePath.Text = ("File saved at: " + newFilePath);
                verify_file_btn.Enabled = true;
                jsonFilePath = null;
                excelFilePath = null;
                txt_excelfilePath.Text = "";
                txt_jsonfilePath.Text = "";
                api_errorMsgLabel.Text = "";
            }

        }

        private bool verifyUatScriptRestAPI(string excelFile, string jsonFile)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                FileInfo excelFileInfo = new FileInfo(excelFile);

                List<string> failedScenarios = new List<string>(); // Store failed scenarios

                using (ExcelPackage package = new ExcelPackage(excelFileInfo))
                {
                    string jsonFileLog = File.ReadAllText(jsonFile);
                    JArray jsonarray = JArray.Parse(jsonFileLog);

                    var uatWorkSheets = package.Workbook.Worksheets;
                    if (uatWorkSheets.Count < 2) return false;

                    //where sheet the script will start
                    for (int uatSheetIndex = 2; uatSheetIndex < uatWorkSheets.Count; uatSheetIndex++)
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
                            try
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

                                //STEP 1: Check if scenario is skipped
                                if (string.IsNullOrEmpty(clientActualSignature) && string.IsNullOrEmpty(clientActualResponse))
                                {
                                    uatWorkSheet.Cells[row, remarksColumnIndex].Value = "SKIPPED";
                                    uatWorkSheet.Cells[row, commentColumnIndex].Value = "The scenario is skipped by the client";
                                    continue;
                                }

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
                                string pattern = @"\{[\s\S]*\}"; // Matches everything inside the outermost curly braces
                                Match match = Regex.Match(expectedResult, pattern);//testing
                                string extractedJson = match.Value.Trim();
                                Console.WriteLine(extractedJson);


                                var expectedCodeRemarks = JsonConvert.DeserializeObject<dynamic>(extractedJson);
                                var actualCodeRemarks = JsonConvert.DeserializeObject<dynamic>(clientActualResponse);


                                string expectedResponseCode = expectedCodeRemarks.responseCode.ToString();
                                string expectedRemarks = expectedCodeRemarks.remarks.ToString();
                                string actualResponseCode = actualCodeRemarks.responseCode.ToString();
                                string actualRemarks = actualCodeRemarks.remarks.ToString();

                                Console.WriteLine($"Expected Response Code: \"{expectedResponseCode}\"");
                                Console.WriteLine($"Expected Remarks: \"{expectedRemarks}\"");
                                Console.WriteLine($"Actual Response Code: \"{actualResponseCode}\"");
                                Console.WriteLine($"ActualRemarks: \"{actualRemarks}\"");

                                Console.WriteLine($"Curent Sheet: \"{uatWorkSheet.Name}\"");
                                Console.WriteLine($"Current Endpoint: \"{endpoint}\"");
                                Console.WriteLine($"Column: \"{signatureColumnIndex}\" Row: \"{row}\"");
                                Console.WriteLine($"Excel Signature: \"{clientActualSignature}\"");
                                Console.WriteLine($"Excel Actual Response: \"{finalFormattedResponse}\"");

                                if (endpoint.Contains("List of Products"))
                                {
                                    if (clientActualResponse.Contains(expectedRemarks) && clientActualResponse.Contains(expectedResponseCode))
                                    {
                                        uatWorkSheet.Cells[row, remarksColumnIndex].Value = "PASSED";
                                    }
                                    else
                                    {
                                        uatWorkSheet.Cells[row, remarksColumnIndex].Value = "FAILED";
                                        uatWorkSheet.Cells[row, commentColumnIndex].Value = "Expected response code and remarks did not meet";
                                    }
                                }



                                //STEP 2: check if expected api response code == actual api response code
                                if (expectedResponseCode == actualResponseCode && expectedRemarks == actualRemarks)
                                {
                                    Console.WriteLine("STEP 2 PASSED!!!");
                                    //continue;
                                }
                                else
                                {
                                    uatWorkSheet.Cells[row, remarksColumnIndex].Value = "FAILED";
                                    uatWorkSheet.Cells[row, commentColumnIndex].Value = "Expected response code and remarks did not meet";
                                    failedScenarios.Add($"Sheet: {uatWorkSheet.Name}, Row: {row}, Endpoint: {endpoint}, Error: Response Code or Remarks Mismatch");
                                    Console.WriteLine("STEP 2 FAILED!!!");
                                    //break;
                                    continue;
                                }

                                JArray jsonObjects = JArray.Parse(jsonFileLog);
                                bool matchFound = false;

                                //Step 3: check if existing on logs
                                foreach (JObject jsonObject in jsonObjects)
                                {
                                    string jsonLog = jsonObject.ToString();

                                    if (finalFormattedResponse.Contains("\\\"data\\\":null}"))
                                    {
                                        //Console.WriteLine("\"data\":null}");
                                        if (jsonLog.Contains(clientActualSignature) && jsonLog.Contains(actualResponseCode) && jsonLog.Contains(actualRemarks))
                                        {
                                            matchFound = true;
                                            string dateEntry = jsonObject["dateEntry"]["$date"].ToString();
                                            DateTime parsedDate = DateTime.Parse(dateEntry, null, System.Globalization.DateTimeStyles.RoundtripKind);
                                            string formattedDateEntry = parsedDate.ToString("yyyy-MM-ddTHH:mm:ss");
                                            Console.WriteLine($"Match Found! Date Entry: {formattedDateEntry}");
                                            Console.WriteLine("STEP 3 PASSED!!!");
                                            Console.WriteLine("==================================================================================================================================");
                                            //Console.WriteLine("Remarks: Passed");
                                            uatWorkSheet.Cells[row, remarksColumnIndex].Value = "PASSED";
                                            uatWorkSheet.Cells[row, commentColumnIndex].Value = "Scenario is verified, log date entry was: " + formattedDateEntry;                                           
                                            break;
                                        }
                                    }
                                    else if (jsonLog.Contains(clientActualSignature) && jsonLog.Contains(finalFormattedResponse))
                                    {
                                        matchFound = true;
                                        string dateEntry = jsonObject["dateEntry"]["$date"].ToString();
                                        DateTime parsedDate = DateTime.Parse(dateEntry, null, System.Globalization.DateTimeStyles.RoundtripKind);
                                        string formattedDateEntry = parsedDate.ToString("yyyy-MM-ddTHH:mm:ss");
                                        Console.WriteLine($"Match Found! Date Entry: {formattedDateEntry}");
                                        Console.WriteLine("STEP 3 PASSED!!!");
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
                                    uatWorkSheet.Cells[row, commentColumnIndex].Value = "Verification failed. No matching log entry.";
                                    failedScenarios.Add($"Sheet: {uatWorkSheet.Name}, Row: {row}, Endpoint: {endpoint}, Error: No matching log entry found");
                                    Console.WriteLine("STEP 3 FAILED!!!");
                                }
                            }
                            catch (Exception ex)
                            {
                                // Log error and continue
                                Console.WriteLine($"Error processing sheet {uatWorkSheet}: {ex.Message}");
                                Console.WriteLine($"Error processing row {row}: {ex.Message}");
                                uatWorkSheet.Cells[row, remarksColumnIndex].Value = "FAILED";
                                //uatWorkSheet.Cells[row, commentColumnIndex].Value = $"Error: {ex.Message}";
                                failedScenarios.Add($"Sheet: {uatWorkSheet.Name}, Row: {row}, Error: {ex.Message}");
                                continue;
                            }



                        }

                    }
                    // **SAVE UPDATED FILE**
                    string directory = Path.GetDirectoryName(excelFile);
                    string newFileName = Path.Combine(directory, $"Verified_{Path.GetFileName(excelFile)}");
                    package.SaveAs(new FileInfo(newFileName));

                    newFilePath = newFileName; // Store the new file path for display

                }

                // Update UI with failed scenarios
                Invoke(new Action(() =>
                {
                    failedScenariosListBox.Items.Clear();
                    if (failedScenarios.Count > 0)
                    {
                        foreach (var item in failedScenarios)
                        {
                            failedScenariosListBox.Items.Add(item);
                        }
                    }
                    else
                    {
                        failedScenariosListBox.Items.Add("No failures detected!");
                    }
                }));
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Verification Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        private bool verifyUatScriptUnifiedAPI(string excelFile, string jsonFile)
        {

            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                FileInfo excelFileInfo = new FileInfo(excelFile);

                List<string> failedScenarios = new List<string>(); // Store failed scenarios

                using (ExcelPackage package = new ExcelPackage(excelFileInfo))
                {
                    string jsonFileLog = File.ReadAllText(jsonFile);
                    JArray jsonarray = JArray.Parse(jsonFileLog);

                    var uatWorkSheets = package.Workbook.Worksheets;
                    if (uatWorkSheets.Count < 2) return false;

                    //where sheet the script will start
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
                            try
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

                                //STEP 1: Check if scenario is skipped
                                if (string.IsNullOrEmpty(clientActualSignature) && string.IsNullOrEmpty(clientActualResponse))
                                {
                                    uatWorkSheet.Cells[row, remarksColumnIndex].Value = "SKIPPED";
                                    uatWorkSheet.Cells[row, commentColumnIndex].Value = "The scenario is skipped by the client";
                                    continue;
                                }

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
                                string pattern = @"\{[\s\S]*\}"; // Matches everything inside the outermost curly braces
                                Match match = Regex.Match(expectedResult, pattern);//testing
                                string extractedJson = match.Value.Trim();
                                Console.WriteLine(extractedJson);


                                var expectedCodeRemarks = JsonConvert.DeserializeObject<dynamic>(extractedJson);
                                var actualCodeRemarks = JsonConvert.DeserializeObject<dynamic>(clientActualResponse);


                                string expectedResponseCode = expectedCodeRemarks.responseCode.ToString();
                                string expectedRemarks = expectedCodeRemarks.remarks.ToString();
                                string actualResponseCode = actualCodeRemarks.responseCode.ToString();
                                string actualRemarks = actualCodeRemarks.remarks.ToString();

                                Console.WriteLine($"Expected Response Code: \"{expectedResponseCode}\"");
                                Console.WriteLine($"Expected Remarks: \"{expectedRemarks}\"");
                                Console.WriteLine($"Actual Response Code: \"{actualResponseCode}\"");
                                Console.WriteLine($"ActualRemarks: \"{actualRemarks}\"");

                                Console.WriteLine($"Curent Sheet: \"{uatWorkSheet.Name}\"");
                                Console.WriteLine($"Current Endpoint: \"{endpoint}\"");
                                Console.WriteLine($"Column: \"{signatureColumnIndex}\" Row: \"{row}\"");
                                Console.WriteLine($"Excel Signature: \"{clientActualSignature}\"");
                                Console.WriteLine($"Excel Actual Response: \"{finalFormattedResponse}\"");

                                if (endpoint.Contains("Get Biller") || endpoint.Contains("billerdata"))
                                {
                                    if (clientActualResponse.Contains(expectedRemarks) && clientActualResponse.Contains(expectedResponseCode))
                                    {
                                        uatWorkSheet.Cells[row, remarksColumnIndex].Value = "PASSED";
                                    }
                                    else
                                    {
                                        uatWorkSheet.Cells[row, remarksColumnIndex].Value = "FAILED";
                                        uatWorkSheet.Cells[row, commentColumnIndex].Value = "Expected response code and remarks did not meet";
                                    }
                                }



                                //STEP 2: check if expected api response code == actual api response code
                                if (expectedResponseCode == actualResponseCode && expectedRemarks == actualRemarks)
                                {
                                    Console.WriteLine("STEP 2 PASSED!!!");
                                    //continue;
                                }
                                else
                                {
                                    uatWorkSheet.Cells[row, remarksColumnIndex].Value = "FAILED";
                                    uatWorkSheet.Cells[row, commentColumnIndex].Value = "Expected response code and remarks did not meet";
                                    failedScenarios.Add($"Sheet: {uatWorkSheet.Name}, Row: {row}, Endpoint: {endpoint}, Error: Response Code or Remarks Mismatch");
                                    Console.WriteLine("STEP 2 FAILED!!!");
                                    //break;
                                    continue;
                                }

                                JArray jsonObjects = JArray.Parse(jsonFileLog);
                                bool matchFound = false;

                                //Step 3: check if existing on logs
                                foreach (JObject jsonObject in jsonObjects)
                                {
                                    string jsonLog = jsonObject.ToString();

                                    if (finalFormattedResponse.Contains("\\\"data\\\":null}"))
                                    {
                                        //Console.WriteLine("\"data\":null}");
                                        if (jsonLog.Contains(clientActualSignature) && jsonLog.Contains(actualResponseCode) && jsonLog.Contains(actualRemarks))
                                        {
                                            matchFound = true;
                                            string dateEntry = jsonObject["dateEntry"]["$date"].ToString();
                                            DateTime parsedDate = DateTime.Parse(dateEntry, null, System.Globalization.DateTimeStyles.RoundtripKind);
                                            string formattedDateEntry = parsedDate.ToString("yyyy-MM-ddTHH:mm:ss");
                                            Console.WriteLine($"Match Found! Date Entry: {formattedDateEntry}");
                                            Console.WriteLine("STEP 3 PASSED!!!");
                                            Console.WriteLine("==================================================================================================================================");
                                            //Console.WriteLine("Remarks: Passed");
                                            uatWorkSheet.Cells[row, remarksColumnIndex].Value = "PASSED";
                                            uatWorkSheet.Cells[row, commentColumnIndex].Value = "Scenario is verified, log date entry was: " + formattedDateEntry;  
                                            break;
                                        }
                                    }
                                    else if (jsonLog.Contains(clientActualSignature) && jsonLog.Contains(finalFormattedResponse))
                                    {
                                        matchFound = true;
                                        string dateEntry = jsonObject["dateEntry"]["$date"].ToString();
                                        DateTime parsedDate = DateTime.Parse(dateEntry, null, System.Globalization.DateTimeStyles.RoundtripKind);
                                        string formattedDateEntry = parsedDate.ToString("yyyy-MM-ddTHH:mm:ss");
                                        Console.WriteLine($"Match Found! Date Entry: {formattedDateEntry}");
                                        Console.WriteLine("STEP 3 PASSED!!!");
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
                                    uatWorkSheet.Cells[row, commentColumnIndex].Value = "Verification failed. No matching log entry.";
                                    failedScenarios.Add($"Sheet: {uatWorkSheet.Name}, Row: {row}, Endpoint: {endpoint}, Error: No matching log entry found");
                                    Console.WriteLine("STEP 3 FAILED!!!");
                                }
                            }
                            catch (Exception ex)
                            {
                                // Log error and continue
                                Console.WriteLine($"Error processing sheet {uatWorkSheet}: {ex.Message}");
                                Console.WriteLine($"Error processing row {row}: {ex.Message}");
                                uatWorkSheet.Cells[row, remarksColumnIndex].Value = "FAILED";
                                //uatWorkSheet.Cells[row, commentColumnIndex].Value = $"Error: {ex.Message}";
                                failedScenarios.Add($"Sheet: {uatWorkSheet.Name}, Row: {row}, Error: {ex.Message}");
                                continue;
                            }



                        }

                    }
                    // **SAVE UPDATED FILE**
                    string directory = Path.GetDirectoryName(excelFile);
                    string newFileName = Path.Combine(directory, $"Verified_{Path.GetFileName(excelFile)}");
                    package.SaveAs(new FileInfo(newFileName));

                    newFilePath = newFileName; // Store the new file path for display

                }

                // Update UI with failed scenarios
                Invoke(new Action(() =>
                {
                    failedScenariosListBox.Items.Clear();
                    if (failedScenarios.Count > 0)
                    {
                        foreach (var item in failedScenarios)
                        {
                            failedScenariosListBox.Items.Add(item);
                        }
                    }
                    else
                    {
                        failedScenariosListBox.Items.Add("No failures detected!");
                    }
                }));
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
