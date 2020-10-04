using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Validation.MyLibrary
{
    public class ResultFactory
    {
        private readonly List<KeyValuePair<string, ResultModule>> report;
        public ResultFactory()
        {

        }
        public ResultFactory(List<KeyValuePair<string, ResultModule>> report)
        {
            this.report = report;
        }

        public string Warning()
        {
            var DesignWarning = "<div class='w3-container w3-yellow' style='background-color:#ffeb3b!important;width: 394px;height: 145px;'><span>&#9888;Warning! </span>  <p>Yellow</p></div>";
            return DesignWarning;
        }
        public void AddToReportList(string columnName, string TypeOfError)
        {
            ResultModule result = new ResultModule();
            result.errors++;
            result.type = columnName + TypeOfError;
            report.Add(new KeyValuePair<string, ResultModule>(columnName, result));
        }

        public ResultModule TableResult(List<KeyValuePair<string, ResultModule>> report)
        {
            if (report.Count > 0) 
            {
                ResultModule result = new ResultModule();
                List<string> TotalErrors = new List<string>();
                var LiKey = report.Select(x => x.Key).Distinct();
                var Table = "";
                int NotvalidText = 0;
                int NotNumber = 0;
                int NlengthDoseNotMatch = 0;
                int NegativeNumber = 0;
                int InvalidDateForamt = 0;
                int MaxValueDoseNotMatch = 0;
                int NotValidDecimal = 0;
                int NotTrueOrFalse = 0;
                int NotYesOrNo = 0;
                int Not1Or0 = 0;
                int NullOrEmpty = 0;
                int NotMatchLockUp = 0;
                List<string> liErrors = new List<string>();

                foreach (var key in LiKey)
                {
                    if (report.Where(x => x.Key == key && x.Value.type.Contains("Null or Empty")).Count() != 0)
                    {
                        if (Table == "")
                        {
                            Table = key + " : Null or Empty : " + report.Where(x => x.Key == key && x.Value.type.Contains("Null or Empty")).Count();
                        }
                        else
                        {
                            Table = Table + ";" + key + " : Null or Empty : " + report.Where(x => x.Key == key && x.Value.type.Contains("Null or Empty")).Count();
                        }
                        NullOrEmpty++;
                    }

                    if (report.Where(x => x.Key == key && x.Value.type.Contains("Not valid text")).Count() != 0)
                    {
                        if (Table == "")
                        {
                            Table = key + " : Not valid text : " + report.Where(x => x.Key == key && x.Value.type.Contains("Not valid text")).Count();
                        }
                        else
                        {
                            Table = Table + ";" + key + " : Not valid text : " + report.Where(x => x.Key == key && x.Value.type.Contains("Not valid text")).Count();
                        }
                        NotvalidText++;
                    }

                    if (report.Where(x => x.Key == key && x.Value.type.Contains("Not Number")).Count() != 0)
                    {
                        if (Table == "")
                        {
                            Table = key + " : Not Number : " + report.Where(x => x.Key == key && x.Value.type.Contains("Not Number")).Count();
                        }
                        else
                        {
                            Table = Table + ";" + key + " : Not Number : " + report.Where(x => x.Key == key && x.Value.type.Contains("Not Number")).Count();
                        }
                        NotNumber++;
                    }

                    if (report.Where(x => x.Key == key && x.Value.type.Contains("length Dose Not Match")).Count() != 0)
                    {
                        if (Table == "")
                        {
                            Table = key + " : length Dose Not Match : " + report.Where(x => x.Key == key && x.Value.type.Contains("length Dose Not Match")).Count();
                        }
                        else
                        {
                            Table = Table + ";" + key + " : length Dose Not Match : " + report.Where(x => x.Key == key && x.Value.type.Contains("length Dose Not Match")).Count();
                        }
                        NlengthDoseNotMatch++;
                    }

                    if (report.Where(x => x.Key == key && x.Value.type.Contains("Negative Number")).Count() != 0)
                    {
                        if (Table == "")
                        {
                            Table = key + " : Negative Number : " + report.Where(x => x.Key == key && x.Value.type.Contains("Negative Number")).Count();
                        }
                        else
                        {
                            Table = Table + ";" + key + " : Negative Number : " + report.Where(x => x.Key == key && x.Value.type.Contains("Negative Number")).Count();
                        }
                        NegativeNumber++;
                    }

                    if (report.Where(x => x.Key == key && x.Value.type.Contains("Invalid Date Foramt")).Count() != 0)
                    {
                        if (Table == "")
                        {
                            Table = key + " : Invalid Date Foramt : " + report.Where(x => x.Key == key && x.Value.type.Contains("Invalid Date Foramt")).Count();
                        }
                        else
                        {
                            Table = Table + ";" + key + " : Invalid Date Foramt : " + report.Where(x => x.Key == key && x.Value.type.Contains("Invalid Date Foramt")).Count();
                        }
                        InvalidDateForamt++;
                    }

                    if (report.Where(x => x.Key == key && x.Value.type.Contains("MaxValue Dose Not Match")).Count() != 0)
                    {
                        if (Table == "")
                        {
                            Table = key + " : MaxValue Dose Not Match : " + report.Where(x => x.Key == key && x.Value.type.Contains("MaxValue Dose Not Match")).Count();
                        }
                        else
                        {
                            Table = Table + ";" + key + " : MaxValue Dose Not Match : " + report.Where(x => x.Key == key && x.Value.type.Contains("MaxValue Dose Not Match")).Count();
                        }
                        MaxValueDoseNotMatch++;
                    }

                    if (report.Where(x => x.Key == key && x.Value.type.Contains("Not valid decimal")).Count() != 0)
                    {
                        if (Table == "")
                        {
                            Table = key + " : Not valid decimal : " + report.Where(x => x.Key == key && x.Value.type.Contains("Not valid decimal")).Count();
                        }
                        else
                        {
                            Table = Table + ";" + key + " : Not valid decimal : " + report.Where(x => x.Key == key && x.Value.type.Contains("Not valid decimal")).Count();
                        }
                        NotValidDecimal++;
                    }

                    if (report.Where(x => x.Key == key && x.Value.type.Contains("Not True or False")).Count() != 0)
                    {
                        if (Table == "")
                        {
                            Table = key + " : Not True or False : " + report.Where(x => x.Key == key && x.Value.type.Contains("Not True or False")).Count();
                        }
                        else
                        {
                            Table = Table + ";" + key + " : Not True or False : " + report.Where(x => x.Key == key && x.Value.type.Contains("Not True or False")).Count();
                        }
                        NotTrueOrFalse++;
                    }

                    if (report.Where(x => x.Key == key && x.Value.type.Contains("Not Yes or No")).Count() != 0)
                    {
                        if (Table == "")
                        {
                            Table = key + " : Not Yes or No : " + report.Where(x => x.Key == key && x.Value.type.Contains("Not Yes or No")).Count();
                        }
                        else
                        {
                            Table = Table + ";" + key + " : Not Yes or No : " + report.Where(x => x.Key == key && x.Value.type.Contains("Not Yes or No")).Count();
                        }
                        NotYesOrNo++;
                    }

                    if (report.Where(x => x.Key == key && x.Value.type.Contains("Not 1 or 0")).Count() != 0)
                    {
                        if (Table == "")
                        {
                            Table = key + " : Not 1 or 0 : " + report.Where(x => x.Key == key && x.Value.type.Contains("Not 1 or 0")).Count();
                        }
                        else
                        {
                            Table = Table + ";" + key + " : Not 1 or 0 : " + report.Where(x => x.Key == key && x.Value.type.Contains("Not 1 or 0")).Count();
                        }

                        Not1Or0++;
                    }

                    if (report.Where(x => x.Key == key && x.Value.type.Contains("Value doesn't exist in lockup")).Count() != 0)
                    {
                        if (Table == "")
                        {
                            Table = key + " : Value doesn't exist in lockup : " + report.Where(x => x.Key == key && x.Value.type.Contains("Value doesn't exist in lockup")).Count();
                        }
                        else
                        {
                            Table = Table + ";" + key + " : Value doesn't exist in lockup : " + report.Where(x => x.Key == key && x.Value.type.Contains("Value doesn't exist in lockup")).Count();
                        }

                        NotMatchLockUp++;
                    }
                }

                string Table_Header = "<table style='border: 1px solid black !important;text-align: left;border-collapse: inherit !important ;width: 66%;'><th>Column Name</th><th>Erorr Massage</th><th>Error Count </th><tr><td>";
                Table_Header = Table_Header.Replace("<table>", " <table style='border: 1px solid black;border-collapse: collapse;'>");
                Table_Header = Table_Header.Replace("<th>", "<th style=' border: 1px solid black;border-collapse: collapse;background-color:#C0BBBA ;'>");
                Table_Header = Table_Header.Replace("<td>", " <td style='border: 1px solid black;border-collapse: collapse;'>");
                string Content_Table = Table;
                Content_Table = Content_Table.Replace(":", "</td><td>");
                Content_Table = Content_Table.Replace(";", "</td></tr><td>");
                Content_Table = Content_Table.Replace("<td>", " <td style=' border: 1px solid black;border-collapse: collapse;'>");
                string Table_Footer = "</td></tr></table>";
                Content_Table = Table_Header + Content_Table + Table_Footer;

                TotalErrors.Add(NotvalidText.ToString());
                TotalErrors.Add(NotNumber.ToString());
                TotalErrors.Add(NlengthDoseNotMatch.ToString());
                TotalErrors.Add(NegativeNumber.ToString());
                TotalErrors.Add(InvalidDateForamt.ToString());
                TotalErrors.Add(MaxValueDoseNotMatch.ToString());
                TotalErrors.Add(NotValidDecimal.ToString());
                TotalErrors.Add(NotTrueOrFalse.ToString());
                TotalErrors.Add(NotYesOrNo.ToString());
                TotalErrors.Add(Not1Or0.ToString());
                TotalErrors.Add(NullOrEmpty.ToString());
                TotalErrors.Add(NotMatchLockUp.ToString());
                result.totalNumberOfErrors = NotvalidText + NotNumber + NlengthDoseNotMatch + NegativeNumber + InvalidDateForamt + MaxValueDoseNotMatch + NotValidDecimal + NotTrueOrFalse + NotYesOrNo + Not1Or0 + NullOrEmpty;
                result.totalForEachTypeError = TotalErrors;
                result.table = Content_Table;

                return result;
            }
            else
            {
                ResultModule result = new ResultModule
                {
                    table = "Success"
                };
                return result;
            }

        }
    }
}
