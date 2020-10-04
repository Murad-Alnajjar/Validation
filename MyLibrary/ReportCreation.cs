using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Collections;
using System.Linq;

namespace Validation.MyLibrary
{
    public class ReportCreation
    {
        DataTable dataTable = null;
        static List<KeyValuePair<string, ResultModule>> report = new List<KeyValuePair<string, ResultModule>>();
        ResultFactory factory = new ResultFactory(report);
        int totalNumberOfRowsHaveErrors = 0;
        int RowsCount = 0;
        string Message = "";


        public void LoadDataTable(string filePath, string FileName, int FileSerial, int ProjectSerial)
        {
            string fileExtension = Path.GetExtension(filePath);
            switch (fileExtension.ToLower())
            {
                case ".xlsx":
                    ConvertExcelToDataTable(filePath, true, FileSerial);
                    break;
                case ".xls":
                    ConvertExcelToDataTable(filePath,false, FileSerial);
                    break;
                case ".csv":
                    ConvertCsvToDataTable(filePath, FileSerial);
                    break;
                case ".txt":
                    ConvertTextDocumentToDataTable(filePath, FileSerial);
                    break;
            }
            ValidtionDatatable(dataTable, FileName, DateTime.Now, FileSerial, ProjectSerial);
            DataValidationAPI validation = new DataValidationAPI();
        }
        public void ConvertExcelToDataTable(string filePath, bool isXlsx,int FileSerial)
        {
            FileStream stream = null;
            IExcelDataReader excelReader = null;
            DataTable dataTables = null;
            if (File.Exists(filePath))
            {
                stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                excelReader = isXlsx ? ExcelReaderFactory.CreateOpenXmlReader(stream) : ExcelReaderFactory.CreateBinaryReader(stream);
                DataSet result = excelReader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });



                if (result != null && result.Tables.Count > 0)
                    dataTables = result.Tables[0];
                List<string> columnNameList = new List<string>();


                foreach (DataColumn dataColumn in dataTables.Columns)
                {
                    columnNameList.Add(dataColumn.ColumnName);

                }
                DataValidationAPI ob = new DataValidationAPI();

                string[] headers = columnNameList.ToArray();


                if (ob.ChekCol(headers, FileSerial) == true)
                {
                    ArrayList rows = new ArrayList();

                    foreach (DataRow dataRow in dataTables.Rows)
                        rows.Add(dataRow.ItemArray.Select(item => item.ToString()));

                    if (rows.Count > 0)
                    {
                        dataTable = dataTables;

                    }
                    else
                    {
                        Message = "DataEmpty";
                    }


                }
                else
                {
                    Message = "Missing Column";

                }
            }
            else
            {
                Message = "File_not_Exists";


            }
        }
        public void ConvertCsvToDataTable(string filePath,int FileSerial)
        {
            DataTable dt = new DataTable();

            if (File.Exists(filePath))
            {
                using (StreamReader sr = new StreamReader(filePath, Encoding.GetEncoding(1251)))
                {
                    var line = sr.ReadLine();
                    if (null != line)
                    {
                        string[] headers = line.Split(',');
                        DataValidationAPI ob = new DataValidationAPI();
                        if (ob.ChekCol(headers, FileSerial) == true)
                        {
                            foreach (string header in headers)
                            {
                                dt.Columns.Add(header);
                            }
                            while (!sr.EndOfStream)
                            {
                                string[] rows = sr.ReadLine().Split(',');
                                DataRow dr = dt.NewRow();
                                for (int i = 0; i < headers.Length; i++)
                                {
                                    dr[i] = rows[i];
                                }
                                dt.Rows.Add(dr);
                            }
                            if (sr.EndOfStream == true)
                            {
                                dataTable = dt;
                            }
                            else
                            {
                                Message = "DataEmpty";
                            }
                        }
                        else
                        {
                            Message = "Missing Column";
                        }
                    }
                    else
                    {
                        Message = "DataEmpty";
                    }
                }
            }
            else
            {

                Message = "File_not_Exists";
            }
        }
        public void ConvertTextDocumentToDataTable(string filePath,int FileSerial)
        {

            DataTable dt = new DataTable();

            if (File.Exists(filePath))
            {
                using (StreamReader sr = new StreamReader(filePath, Encoding.GetEncoding(1251)))
                {
                    var line = sr.ReadLine();
                    if (null != line)
                    {
                        string[] headers = line.Split('|');
                        DataValidationAPI ob = new DataValidationAPI();
                        if (ob.ChekCol(headers, FileSerial) == true)
                        {




                            foreach (string header in headers)
                            {
                                dt.Columns.Add(header);
                            }
                            while (!sr.EndOfStream)
                            {
                                string[] rows = sr.ReadLine().Split('|');
                                DataRow dr = dt.NewRow();
                                for (int i = 0; i < headers.Length; i++)
                                {
                                    dr[i] = rows[i];
                                }
                                dt.Rows.Add(dr);
                            }
                            if (sr.EndOfStream == true)
                            {
                                dataTable = dt;
                            }
                            else
                            {
                                Message = "DataEmpty";

                            }
                        }
                        else
                        {
                            Message = "Missing Column";
                        }
                    }
                    else
                    {
                        Message = "Missing Column";
                    }
                }
            }
            else
            {
                Message = "File_not_Exists";
            }
        }

        public void ValidtionDatatable(DataTable DT, string FileName, DateTime date, int FileSerial, int ProjectSerial)
        {
            try
            {
                DbCon CSDB = new DbCon();
                List<SqlParameter> mySqlParameter_list = new List<SqlParameter>();

                if (DT != null)
                {
                    int errorFlag = 0;
                    DataValidationAPI validation = new DataValidationAPI();
                    DataTable myDataTable = new DataTable();
                   
                    myDataTable = CSDB.Execute_Of_Query_Function_dataSet("Select * From tblFileFields");
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        foreach (DataColumn dc in DT.Columns)
                        {
                            var ColumnName_Document = dc.ColumnName.Trim().ToString();
                            DataRow[] dr = myDataTable.Select("vchrFieldName = '" + ColumnName_Document + "'");
                            var value = DT.Rows[i]["" + ColumnName_Document + ""].ToString();
                            foreach (DataRow row in dr)
                            {
                                var serial = Convert.ToInt32(row.ItemArray[0]);
                                var type = row.ItemArray[2].ToString();
                                var columnName = row.ItemArray[5].ToString();
                                var AllowNull = row.ItemArray[8].ToString();
                                var dcMaxValue = Convert.ToInt32(row.ItemArray[9]);
                                var Length = Convert.ToInt32(row.ItemArray[10]);
                                var vchrFormat = row.ItemArray[14].ToString();
                                var scale = Convert.ToInt32(row.ItemArray[4]);
                                var precision = Convert.ToInt32(row.ItemArray[3]);
                                var Negative = row.ItemArray[11].ToString();
                                var btLookup = row.ItemArray[12].ToString();


                                if (type == "1")
                                {
                                    if (btLookup == "True")
                                    {
                                        if (validation.IsLockUp(value, serial) == false)
                                        {
                                            errorFlag++;
                                            factory.AddToReportList(columnName, "Value doesn't exist in lockup");
                                        }

                                    }
                                    if (AllowNull == "True")
                                    {
                                        if (validation.NotEmptyOrNull(value) == false)
                                        {
                                            errorFlag++;
                                            factory.AddToReportList(columnName, "Null or Empty");
                                        }
                                    }
                                    if (Length > 0)
                                    {
                                        if (validation.Number_length(value, Length) == false)
                                        {
                                            errorFlag++;
                                            factory.AddToReportList(columnName, "length Dose Not Match");
                                        }
                                    }

                                    if (validation.Number(value) == false)
                                    {
                                        errorFlag++;
                                        factory.AddToReportList(columnName, "Number");
                                    }

                                    if (Negative.ToLower() == "true")
                                    {
                                        if (validation.Number_Negative(value) == false)
                                        {
                                            errorFlag++;
                                            factory.AddToReportList(columnName, "Negative Number");
                                        }
                                    }

                                    if (dcMaxValue > 0)
                                    {
                                        if (validation.Number_MaxValue(value, dcMaxValue) == false)
                                        {
                                            errorFlag++;
                                            factory.AddToReportList(columnName, "MaxValue Dose Not Match");
                                        }
                                    }
                                }



                                else if (type == "2")
                                {
                                    if (validation.Arabic(value))
                                    {
                                        if (AllowNull == "True")
                                        {
                                            if (validation.NotEmptyOrNull(value) == false)
                                            {
                                                errorFlag++;
                                                factory.AddToReportList(columnName, "Null or Empty");
                                            }
                                        }

                                        if (btLookup == "True")
                                        {
                                            if (validation.IsLockUp(value, serial) == false)
                                            {
                                                errorFlag++;
                                                factory.AddToReportList(columnName, "Value doesn't exist in lockup");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (btLookup == "True")
                                        {
                                            if (validation.IsLockUp(value, serial) == false)
                                            {
                                                errorFlag++;
                                                factory.AddToReportList(columnName, "Value doesn't exist in lockup");
                                            }
                                        }


                                        if (AllowNull == "True")
                                        {
                                            if (validation.NotEmptyOrNull(value) == false)
                                            {
                                                errorFlag++;
                                                factory.AddToReportList(columnName, "Null or Empty");
                                            }
                                        }
                                        if (vchrFormat == "Combined")
                                        {
                                            if (validation.Text(value, "Combined") == false)
                                            {
                                                errorFlag++;
                                                factory.AddToReportList(columnName, "Not valid text");
                                            }
                                        }
                                        else if (vchrFormat == "Letters")
                                        {
                                            if (validation.Text(value, "Letters") == false)
                                            {
                                                errorFlag++;
                                                factory.AddToReportList(columnName, "Not valid text");
                                            }
                                        }
                                        else
                                        {
                                            if (validation.Text(value, "Numbers") == false)
                                            {
                                                errorFlag++;
                                                factory.AddToReportList(columnName, "Text");
                                            }
                                        }
                                    }
                                }

                                else if (type == "3")
                                {
                                    if (btLookup == "True")
                                    {
                                        if (validation.IsLockUp(value, serial) == false)
                                        {
                                            errorFlag++;
                                            factory.AddToReportList(columnName, "Value doesn't exist in lockup");
                                        }
                                    }
                                    if (AllowNull == "True")
                                    {
                                        if (validation.NotEmptyOrNull(value) == false)
                                        {
                                            errorFlag++;
                                            factory.AddToReportList(columnName, "Null or Empty");
                                        }
                                    }
                                    if (validation.Date(value, vchrFormat) == false)
                                    {
                                        errorFlag++;
                                        factory.AddToReportList(columnName, "Invalid Date Foramt");
                                    }
                                }

                                else if (type == "4")
                                {
                                    if (btLookup == "True")
                                    {
                                        if (validation.IsLockUp(value, serial) == false)
                                        {
                                            errorFlag++;
                                            factory.AddToReportList(columnName, "Value doesn't exist in lockup");
                                        }
                                    }
                                    if (AllowNull == "True")
                                    {
                                        if (validation.NotEmptyOrNull(value) == false)
                                        {
                                            errorFlag++;
                                            factory.AddToReportList(columnName, "Null or Empty");
                                        }


                                    }
                                    if (validation.Bit(value, vchrFormat) == false)
                                    {
                                        if (vchrFormat == "true/false")
                                        {
                                            errorFlag++;
                                            factory.AddToReportList(columnName, "Not True or False");
                                        }
                                        else if (vchrFormat == "yes/no")
                                        {
                                            errorFlag++;
                                            factory.AddToReportList(columnName, "Not Yes or No");
                                        }
                                        else
                                        {
                                            errorFlag++;
                                            factory.AddToReportList(columnName, "Not 1 or 0");
                                        }
                                    }
                                }

                                else//Decimal
                                {
                                    if (btLookup == "True")
                                    {
                                        if (validation.IsLockUp(value, serial) == false)
                                        {
                                            errorFlag++;
                                            factory.AddToReportList(columnName, "Value doesn't exist in lockup");
                                        }
                                    }
                                    if (AllowNull == "True")
                                    {
                                        if (validation.NotEmptyOrNull(value) == false)
                                        {
                                            errorFlag++;
                                            factory.AddToReportList(columnName, "Null or Empty");
                                        }
                                    }
                                    if (Negative.ToLower() == "true")
                                    {
                                        if (validation.Number_Negative(value) == false)
                                        {
                                            errorFlag++;
                                            factory.AddToReportList(columnName, "Negative Number");
                                        }
                                    }

                                    if (validation.Decimal(value, precision, scale) == false)
                                    {
                                        errorFlag++;
                                        factory.AddToReportList(columnName, "Not valid decimal");
                                    }
                                }
                            }
                        }
                        if (errorFlag > 0)
                        {
                            totalNumberOfRowsHaveErrors++;
                        }
                        RowsCount = DT.Rows.Count-1;
                    }
                    InsertEmail(FileName, date, FileSerial, ProjectSerial, Message);
                }
                else
                {
                    InsertEmail(FileName, date, FileSerial, ProjectSerial, Message);
                }
            }
            catch (Exception ex)
            { }
        }
        public void InsertEmail(string FileName, DateTime date, int FileSerial, int ProjectSerial, string Message)
        {
            DbCon CSDB = new DbCon();
            List<SqlParameter> mySqlParameter_list = new List<SqlParameter>();
            DataTable myDataTable2 = new DataTable();
            mySqlParameter_list.Add(new SqlParameter("@P1", SqlDbType.Int));
            mySqlParameter_list[mySqlParameter_list.Count - 1].Value = FileSerial;
            //---------------------------------------------------------------------
            mySqlParameter_list.Add(new SqlParameter("@P2", SqlDbType.Int));
            mySqlParameter_list[mySqlParameter_list.Count - 1].Value = ProjectSerial;
            myDataTable2 = CSDB.Execute_Of_Query_Function_dataSet("Exec sp_Emailbox_Select @P1, @P2", mySqlParameter_list);

            if (myDataTable2.Rows[0]["Max_Number"].ToString() == "")
            {

                SendEmail(FileName, date, FileSerial, ProjectSerial, 1, Message);
            }
            else
            {
                var Max_Number = Convert.ToInt32(myDataTable2.Rows[0]["Max_Number"]);
                myDataTable2.Clear();
                myDataTable2 = CSDB.Execute_Of_Query_Function_dataSet("Exec sp_EmailSetting_Select");
                DataRow[] dr_Number = myDataTable2.Select("setting_name = 'Email_Max_Send'");
                var Number = Convert.ToInt32(dr_Number[0].ItemArray[1]);
                if (Max_Number < Number)
                {
                    Max_Number++;
                    SendEmail(FileName, date, FileSerial, ProjectSerial, Max_Number, Message);
                }
                else
                {
                    string table = factory.TableResult(report).table;
                    string totalNumberOfErrors = factory.TableResult(report).totalNumberOfErrors.ToString();
                    string totalForEachTypeError = factory.TableResult(report).totalForEachTypeError.ToString();
                    Max_Number++;
                    Emailbox_Insert(Message, "SendMailSubject", date, "vchrProjectName", FileName, table, false, Max_Number, 11, 11, FileSerial, ProjectSerial);
                }
            }
        }
        public void SendEmail(string FileName, DateTime date, int FileSerial, int ProjectSerial, int Max_Number, string Message2)
        {
            try
            {
                DbCon CSDB = new DbCon();
                var Email_Display_Name = "";
                var Email_Domain = "";
                var Email_From = "";
                var Templet_Email = "";
                var Email_Password = "";
                var Email_Port = 0;
                var Email_Smtp_Server = "";
                var Email_Special_Body = "";
                var Email_Ssl = "";
                var Email_To = "";
                var Email_Username = "";
                var DataEmpty = "";
                var Subject = "";
                var MissingColumn = "";
                var File_not_Exists = "";
                var Success = "";
                var Error = "";
                DataTable myDataTable2 = new DataTable();
                myDataTable2 = CSDB.Execute_Of_Query_Function_dataSet("Exec sp_EmailSetting_Select");
                DataRow[] dr_Email_Display_Name = myDataTable2.Select("setting_name = 'Email_Display_Name'");
                DataRow[] dr_Email_Domain = myDataTable2.Select("setting_name = 'Email_Domain'");
                DataRow[] dr_Email_From = myDataTable2.Select("setting_name = 'Email_From'");
                DataRow[] dr_Templet_Email = myDataTable2.Select("setting_name = 'Email_Message_Html_Body'");
                DataRow[] dr_Email_Password = myDataTable2.Select("setting_name = 'Email_Password'");
                DataRow[] dr_Email_Port = myDataTable2.Select("setting_name = 'Email_Port'");
                DataRow[] dr_Email_Smtp_Server = myDataTable2.Select("setting_name = 'Email_Smtp_Server'");
                DataRow[] dr_Email_Special_Body = myDataTable2.Select("setting_name = 'Email_Special_Body'");
                DataRow[] dr_Email_Ssl = myDataTable2.Select("setting_name = 'Email_Ssl'");
                DataRow[] dr_Email_To = myDataTable2.Select("setting_name = 'Email_To'");
                DataRow[] dr_Email_Username = myDataTable2.Select("setting_name = 'Email_Username'");
                DataRow[] dr_DataEmpty = myDataTable2.Select("setting_name = 'DataEmpty'");
                DataRow[] dr_Subject = myDataTable2.Select("setting_name = 'Subject'");
                DataRow[] dr_MissingColumn = myDataTable2.Select("setting_name = 'Missing Column'");
                DataRow[] dr_File_not_Exists = myDataTable2.Select("setting_name = 'File_not_Exists'");
                DataRow[] dr_Success = myDataTable2.Select("setting_name = 'Success'");
                DataRow[] dr_Error = myDataTable2.Select("setting_name = 'Error'");

                DataEmpty = dr_DataEmpty[0].ItemArray[1].ToString();
                Success = dr_Success[0].ItemArray[1].ToString();
                MissingColumn = dr_MissingColumn[0].ItemArray[1].ToString();
                File_not_Exists = dr_File_not_Exists[0].ItemArray[1].ToString();
                Subject = dr_Subject[0].ItemArray[1].ToString();
                Error = dr_Error[0].ItemArray[1].ToString();



                Email_Display_Name = dr_Email_Display_Name[0].ItemArray[1].ToString();
                Email_Domain = dr_Email_Domain[0].ItemArray[1].ToString();
                Email_From = dr_Email_From[0].ItemArray[1].ToString();
                Templet_Email = dr_Templet_Email[0].ItemArray[1].ToString();
                Email_Password = dr_Email_Password[0].ItemArray[1].ToString();
                Email_Port = Convert.ToInt32(dr_Email_Port[0].ItemArray[1]);
                Email_Smtp_Server = dr_Email_Smtp_Server[0].ItemArray[1].ToString();
                Email_Special_Body = dr_Email_Special_Body[0].ItemArray[1].ToString();
                Email_Ssl = dr_Email_Ssl[0].ItemArray[1].ToString();
                Email_To = dr_Email_To[0].ItemArray[1].ToString();
                Email_Username = dr_Email_Username[0].ItemArray[1].ToString();
                MailMessage mail = new MailMessage();

                string table = factory.TableResult(report).table;
                string totalNumberOfErrors = factory.TableResult(report).totalNumberOfErrors.ToString();
                string totalForEachTypeError = factory.TableResult(report).totalForEachTypeError.ToString();
                string NumberOfRowsContainErrors = totalNumberOfRowsHaveErrors.ToString();
                string RowsCountForValidatedFile = RowsCount.ToString();
                int SuccessRecords = RowsCount - totalNumberOfRowsHaveErrors;
                DateTime aDate = DateTime.Now;
                Templet_Email = Templet_Email.Replace("ETL system", " " + Email_Display_Name + " ");
                Templet_Email = Templet_Email.Replace("***_INSERT_No.records_***", " " + RowsCountForValidatedFile + " ");
                Templet_Email = Templet_Email.Replace("***_INSERT_No.successfully_***", " " + SuccessRecords.ToString() + " ");
                Templet_Email = Templet_Email.Replace("***_INSERT_No.discarded_***", " " + totalNumberOfErrors+ " ");


                if (Message2 == "File_not_Exists") 
                {
                    Templet_Email = Templet_Email.Replace("***_PACKAGE_STATUS_HERE_***", "File not Exists");
                    Templet_Email = Templet_Email.Replace("***_INSERT_MESSAGE_HERE_***", File_not_Exists);
                }
               else if (Message2 == "Missing Column")
                {
                    Templet_Email = Templet_Email.Replace("***_PACKAGE_STATUS_HERE_***", " Missing Column");
                    Templet_Email = Templet_Email.Replace("***_INSERT_MESSAGE_HERE_***", MissingColumn);
                }
                else if (Message2 == "DataEmpty")
                {
                    Templet_Email = Templet_Email.Replace("***_PACKAGE_STATUS_HERE_***", " Data Empty");
                    Templet_Email = Templet_Email.Replace("***_INSERT_MESSAGE_HERE_***", DataEmpty);
                }
                else if (Message2 == "Success")
                {
                    Templet_Email = Templet_Email.Replace("***_PACKAGE_STATUS_HERE_***", " Success ");
                    Templet_Email = Templet_Email.Replace("***_INSERT_MESSAGE_HERE_***", Success);
                }
                else 
                {
                    Templet_Email = Templet_Email.Replace("***_PACKAGE_STATUS_HERE_***", " Error ");
                    Templet_Email = Templet_Email.Replace("***_INSERT_MESSAGE_HERE_***", Error);
                    Templet_Email = Templet_Email.Replace("TableError", "" + table + "");
                }
                Templet_Email = Templet_Email.Replace("***_INSERT_DATE_HERE_***", "" + aDate.ToString("dddd, dd MMMM yyyy") + "");
                Templet_Email = Templet_Email.Replace("Module", "File Name");
                Templet_Email = Templet_Email.Replace("***_INSERT_MODULE_NAME_HERE_***", "" + FileName + "");
                



                //-------------------------------------------------------------------------------------------------------------------------
          
                DataTable myDataTable3 = new DataTable();
                List<SqlParameter> mySqlParameter_list3 = new List<SqlParameter>();
                mySqlParameter_list3.Add(new SqlParameter("@P1", SqlDbType.Int));
                mySqlParameter_list3[mySqlParameter_list3.Count - 1].Value = FileSerial;
                //---------------------------------------------------------------------
                mySqlParameter_list3.Add(new SqlParameter("@P2", SqlDbType.Int));
                mySqlParameter_list3[mySqlParameter_list3.Count - 1].Value = ProjectSerial;
                myDataTable3 = CSDB.Execute_Of_Query_Function_dataSet("Exec sp_select_Email_To @P1, @P2", mySqlParameter_list3);
                //===========================================
                mail.To.Add("" + myDataTable3.Rows[0]["Email_To"].ToString() + "");
                mail.Subject = Subject;
                mail.From = new System.Net.Mail.MailAddress("" + Email_From + "");
                mail.Body = Templet_Email.ToString();
                mail.BodyEncoding = System.Text.Encoding.UTF8;
                mail.IsBodyHtml = true;
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("" + Email_Smtp_Server + "", Email_Port);
                smtp.EnableSsl = false;
                smtp.Credentials = new NetworkCredential("" + Email_To + "", "" + Email_Password + "");
                smtp.Send(mail);
                Emailbox_Insert(Message2, "SendMailSubject", date, "vchrProjectName", FileName, table, true, Max_Number, 11, 11, FileSerial, ProjectSerial);
            }
            catch (Exception ex) { }

        }
        public void Emailbox_Insert(string EmailType, string SendMailSubject, DateTime date, string ProjectName, string FileName, string table, bool Y_N, int SendNumber, int RecordsNumber, int FailureNumber, int FileSerial, int ProjectSerial)
        {
            try
            {
                DbCon CSDB = new DbCon();
                List<SqlParameter> mySqlParameter_list = new List<SqlParameter>();
                mySqlParameter_list.Add(new SqlParameter("@P1", SqlDbType.NVarChar));
                mySqlParameter_list[mySqlParameter_list.Count - 1].Value = EmailType;
                //---------------------------------------------------------------------
                mySqlParameter_list.Add(new SqlParameter("@P2", SqlDbType.NVarChar));
                mySqlParameter_list[mySqlParameter_list.Count - 1].Value = SendMailSubject;
                //---------------------------------------------------------------------
                mySqlParameter_list.Add(new SqlParameter("@P3", SqlDbType.DateTime));
                mySqlParameter_list[mySqlParameter_list.Count - 1].Value = date;
                //---------------------------------------------------------------------
                mySqlParameter_list.Add(new SqlParameter("@P4", SqlDbType.NVarChar));
                mySqlParameter_list[mySqlParameter_list.Count - 1].Value = ProjectName;
                //---------------------------------------------------------------------
                mySqlParameter_list.Add(new SqlParameter("@P5", SqlDbType.NVarChar));
                mySqlParameter_list[mySqlParameter_list.Count - 1].Value = FileName;
                //---------------------------------------------------------------------
                mySqlParameter_list.Add(new SqlParameter("@P6", SqlDbType.NVarChar));
                mySqlParameter_list[mySqlParameter_list.Count - 1].Value = table;
                //---------------------------------------------------------------------
                mySqlParameter_list.Add(new SqlParameter("@P7", SqlDbType.Bit));
                mySqlParameter_list[mySqlParameter_list.Count - 1].Value = Y_N;
                //---------------------------------------------------------------------
                mySqlParameter_list.Add(new SqlParameter("@P8", SqlDbType.Int));
                mySqlParameter_list[mySqlParameter_list.Count - 1].Value = SendNumber;
                //---------------------------------------------------------------------
                mySqlParameter_list.Add(new SqlParameter("@P9", SqlDbType.BigInt));
                mySqlParameter_list[mySqlParameter_list.Count - 1].Value = RecordsNumber;
                //---------------------------------------------------------------------
                mySqlParameter_list.Add(new SqlParameter("@P10", SqlDbType.BigInt));
                mySqlParameter_list[mySqlParameter_list.Count - 1].Value = FailureNumber;

                mySqlParameter_list.Add(new SqlParameter("@P11", SqlDbType.Int));
                mySqlParameter_list[mySqlParameter_list.Count - 1].Value = FileSerial;
                //---------------------------------------------------------------------
                mySqlParameter_list.Add(new SqlParameter("@P12", SqlDbType.Int));
                mySqlParameter_list[mySqlParameter_list.Count - 1].Value = ProjectSerial;

                CSDB.Execute_Of_Query_Function_retOneItem("Exec sp_Emailbox_Insert @P1,@P2,@P3,@P4,@P5,@P6,@P7,@P8,@P9,@P10,@P11,@P12", mySqlParameter_list);
            }
            catch (Exception ex)
            {
            }


        }
    }
}
