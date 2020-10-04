using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

namespace Validation.MyLibrary
{
    public class DataValidationAPI
    {
        private readonly int fileSerial = 0;
        private readonly DataTable myDataTable1 = new DataTable();
        public DataValidationAPI() { }
        public DataTable getSettingsTable()
        {
            return this.myDataTable1;
        }
        public bool ChekCol(string[] headers, int fileSerial)
        {
            try {
                DbCon CSDB = new DbCon();
                DataTable myDataTable1 = new DataTable();
                List<SqlParameter> mySqlParameter_list = new List<SqlParameter>();
                mySqlParameter_list.Clear();
                mySqlParameter_list.Add(new SqlParameter("@P1", SqlDbType.Int));
                mySqlParameter_list[mySqlParameter_list.Count - 1].Value = fileSerial;
                myDataTable1 = CSDB.Execute_Of_Query_Function_dataSet("sp_tblFileFields_GetColumns @P1", mySqlParameter_list);
                List<string> rows = new List<string>();
                foreach (DataRow dataRow in myDataTable1.Rows)
                {          
                    rows.Add(dataRow.ItemArray.FirstOrDefault().ToString());
                }
                bool areEqual = headers.SequenceEqual((IEnumerable<string>)rows);
                return areEqual;
            }
            catch(Exception ex) { return false; }
            }
        public bool File_not_Exists(string file)
        {
            //File not Exists in location
            return true;
        }
        public bool Data_not_Exists(string file)
        {
            //File not Exists in location
            return true;
        }
        public bool NotEmptyOrNull(string value)
        {
            if (value == "" || value == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool Text(string value, string format)
        {
            Regex Text;
            if (format.ToLower() == "combined")
            {
                Text = new Regex("^$|^[a-zA-Z0-9_@]*$");
            }
            else if (format.ToLower() == "numbers")
            {
                Text = new Regex("^$|^[+-]?[0-9]*$");
            }
            else
            {
                Text = new Regex("^$|[a-zA-Z]");//Letters

            }
            if (!Text.IsMatch(value))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool IsLockUp(string value, int serial)
        {
            DbCon CSDB = new DbCon();
            DataTable myDataTable1 = new DataTable();
            List<SqlParameter> mySqlParameter_list = new List<SqlParameter>();
            mySqlParameter_list.Clear();
            mySqlParameter_list.Add(new SqlParameter("@P1", SqlDbType.Int));
            mySqlParameter_list[mySqlParameter_list.Count - 1].Value = serial;
            mySqlParameter_list.Add(new SqlParameter("@P2", SqlDbType.VarChar));
            mySqlParameter_list[mySqlParameter_list.Count - 1].Value = value;
            myDataTable1 = CSDB.Execute_Of_Query_Function_dataSet("sp_tblLookups_GetItems @P1,@P2", mySqlParameter_list);
            var list = new List<string>();
            foreach (DataRow item in myDataTable1.Rows)
            {
                list.Add(item.ToString());
            }
            if (list.Contains(value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool Number(string value)
        {
            Regex Number = new Regex("^$|^[+-]?[0-9]*$");

            if (!Number.IsMatch(value))
            {
                return false;
            }
            {
                return true;
            }
        }   
        public bool Arabic(string value)
        {
            Regex Number = new Regex("^[\u0621-\u064A\u0660-\u0669 ]+$");

            if (!Number.IsMatch(value))
            {
                return false;
            }
            {
                return true;
            }
        }
        public bool Number_length(string value, int length)
        {
            if (value.Length != length && value != "")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool Number_Negative(string value)
        {
            Regex NegativeNumber = new Regex(@"^$|^[-]\d*$");

            if (NegativeNumber.IsMatch(value) && value != "")
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        public bool Bit(string value, string format)
        {
            if (format.ToLower() == "true/false")
            {
                if (value.ToLower() != "true" && value.ToLower() != "false" && value.ToLower() != "")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            if (format == "1/0")
            {
                if (value.ToLower() != "1" && value.ToLower() != "0" && value.ToLower() != "")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            if (format.ToLower() == "yes/no")
            {
                if (value.ToLower() != "yes" && value.ToLower() != "no" && value.ToLower() != "")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
        public bool Decimal(string value, int precision, int scale)
        {
            Regex Decimal = new Regex(@"^[+-]?\d{0," + precision + @"}$|^[+-]?\d{0," + precision + @"}(\.\d{0," + scale + "})$");

            if (!Decimal.IsMatch(value))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool Number_MaxValue(string value, int MaxValue)
        {
            if (Convert.ToInt32(value) > MaxValue && value != "")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool Date(string value, string format)
        {
            //type1 dd-mm-yyyy
            //type2 yyyy-mm-dd
            //type3 ddmmyyyy
            //type4 yyyymmdd
            //type5 mm/dd/yyyy
            //type6 mmddyyyy
            Regex type1 = new Regex(@"^$|^(?:(?:31(\/|-|\.)(?:0?[13578]|1[02]))\1|(?:(?:29|30)(\/|-|\.)(?:0?[13-9]|1[0-2])\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\/|-|\.)0?2\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/|-|\.)(?:(?:0?[1-9])|(?:1[0-2]))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$");
            Regex type2 = new Regex(@"^$|^(20\d{2}|19\d{2}|0(?!0)\d|[1-9]\d)(.|-|\/)(0?[1-9]|1[012])(.|-|\/)(0?[1-9]|[12][0-9]|3[01])$");
            Regex type3 = new Regex(@"^$|^(0?[1-9]|[12][0-9]|3[01])(0?[1-9]|1[012])(20\d{2}|19\d{2}|0(?!0)\d|[1-9]\d)$");
            Regex type4 = new Regex(@"^$|^(20\d{2}|19\d{2}|0(?!0)\d|[1-9]\d)(0?[1-9]|1[012])(0?[1-9]|[12][0-9]|3[01])$");
            Regex type5 = new Regex(@"^$|^(0?[1-9]|1[012])(.|-|\/)(0?[1-9]|[12][0-9]|3[01])(.|-|\/)(20\d{2}|19\d{2}|0(?!0)\d|[1-9]\d)$");
            Regex type6 = new Regex(@"^$|^(0?[1-9]|1[012])(0?[1-9]|[12][0-9]|3[01])(20\d{2}|19\d{2}|0(?!0)\d|[1-9]\d)$");

            if (format.ToLower() == "dd-mm-yyyy" || format.ToLower() == "dd.mm.yyyy" || format.ToLower() == "dd/mm/yyyy")
            {
                if (!type1.IsMatch(value))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            if (format.ToLower() == "yyyy-mm-dd" || format.ToLower() == "yyyy.mm.dd" || format.ToLower() == "yyyy/mm/dd")
            {
                if (!type2.IsMatch(value))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            if (format.ToLower() == "ddmmyyyy")
            {
                if (!type3.IsMatch(value))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            if (format.ToLower() == "yyyymmdd")
            {
                if (!type4.IsMatch(value))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            if (format.ToLower() == "mm-dd-yyyy" || format.ToLower() == "mm.dd.yyyy" || format.ToLower() == "mm/dd/yyyy")
            {
                if (!type5.IsMatch(value))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            if (format.ToLower() == "mmddyyyy")
            {
                if (!type6.IsMatch(value))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
    }
}
