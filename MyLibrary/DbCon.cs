using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
namespace Validation.MyLibrary
{
    public class DbCon
    {
        private int CommandTimeout = 0; private string ConnectionSrting = "";
        
        public DbCon()
        {
            ConnectionSrting = "Data Source=192.168.0.8; Initial Catalog = DataScopeDB; User ID = sa; Password=P@$$w0rd123";
        }

        public Int64 Execute_Of_noneQuery_Function_retNum(string query, List<SqlParameter> list = null)
        {
            // ------------------------------------------------------------------
            Int64 num_row_effected = 0;
            // ------------------------------------------------------------------
            SqlConnection myConnection = new SqlConnection(ConnectionSrting);
            SqlCommand myCommand = new SqlCommand();
            myCommand.Connection = myConnection;
            myCommand.CommandText = query;
            // -----------------------------------------------------------------------------
            myCommand.CommandTimeout = CommandTimeout;
            // -----------------------------------------------------------------------------
            // Adding the parameters to the database. 
            // Remember that the order in which the parameters are added is VERY important!
            // -----------------------------------------------------------------------------
            if (list != null)
            {
                for (int i = 0; i <= list.Count - 1; i++)
                    myCommand.Parameters.Add(list[i]);
            }
            // -----------------------------------------------------------------------------
            try
            {
                myConnection.Open();
                num_row_effected = myCommand.ExecuteNonQuery();
                myConnection.Close();
            }
            catch (Exception ex)
            {
            }
            // ------------------------------------------------------------------
            query = null;
            myCommand.Dispose();
            myConnection.Dispose();
            // ------------------------------------------------------------------ 
            return num_row_effected;
        }
        public void Execute_Of_BulkQuery_Function(string TableName, DataTable TempTable1)
        {
            // ------------------------------------------------------------------
            Int64 num_row_effected = 0;
            // ------------------------------------------------------------------
            SqlConnection myConnection = new SqlConnection(ConnectionSrting);
            SqlBulkCopy objbulk = new SqlBulkCopy(myConnection);
            // -----------------------------------------------------------------------------
            // Adding the parameters to the database. 
            // Remember that the order in which the parameters are added is VERY important!
            // -----------------------------------------------------------------------------
            if (TempTable1 != null)
            {
                for (int ColCount = 0; ColCount < TempTable1.Columns.Count; ColCount++)
                {
                    objbulk.ColumnMappings.Add(TempTable1.Columns[ColCount].ColumnName, TempTable1.Columns[ColCount].ColumnName);
                }
            }
            // -----------------------------------------------------------------------------
            try
            {
                myConnection.Open();
                objbulk.DestinationTableName = "dbo." + TableName;
                objbulk.WriteToServer(TempTable1);
                myConnection.Close();
            }
            catch (Exception ex)
            {
            }
            // ------------------------------------------------------------------
            objbulk = null;
            myConnection.Dispose();
            // ------------------------------------------------------------------ 
        }

        public Int64 Execute_Of_Query_Function_retNum(string query)
        {
            Int64 num_row_effected = 0;
            // ------------------------------------------------------------------  
            try
            {
                SqlConnection myConnection = new SqlConnection(ConnectionSrting);
                myConnection.Open();
                SqlDataReader myReader = null;
                SqlCommand myCommand = new SqlCommand(query, myConnection);
                // ------------------------------------------------------------------
                myCommand.CommandTimeout = CommandTimeout;
                // ------------------------------------------------------------------
                myReader = myCommand.ExecuteReader();
                // ------------------------------------------------------------------
                while ((myReader.Read()))
                    // ------------------------------------------------------------------ 
                    num_row_effected = num_row_effected + 1;
                myConnection.Close();
            }
            catch (Exception ex)
            {
            }
            // ------------------------------------------------------------------ 
            return num_row_effected;
        }

        public List<List<string>> Execute_Of_Query_Function_retList_OneColumn(string query)
        {
            List<string> Temp_list = new List<string>();
            List<List<string>> Temp_list_OfLists = new List<List<string>>();
            // ------------------------------------------------------------------ 
            if ((ConnectionSrting != ""))
            {
                // ------------------------------------------------------------------ 
                try
                {
                    SqlConnection myConnection = new SqlConnection(ConnectionSrting);
                    myConnection.Open();
                    SqlDataReader myReader = null;
                    SqlCommand myCommand = new SqlCommand(query, myConnection);
                    // ------------------------------------------------------------------
                    myCommand.CommandTimeout = CommandTimeout;
                    // ------------------------------------------------------------------
                    myReader = myCommand.ExecuteReader();
                    while ((myReader.Read()))
                    {
                        // ------------------------------------------------------------------ 
                        Temp_list.Add(myReader[0].ToString());
                        // ------------------------------------------------------------------ 
                        Temp_list_OfLists.Add(Temp_list);
                    }
                    myConnection.Close();
                }
                catch (Exception ex)
                {
                }
            }
            // ------------------------------------------------------------------ 
            return Temp_list_OfLists;
        }

        public List<List<string>> Execute_Of_Query_Function_retList(string query)
        {
            List<string> Temp_list = new List<string>();
            List<List<string>> Temp_list_OfLists = new List<List<string>>();
            // ------------------------------------------------------------------ 
            if ((ConnectionSrting != ""))
            {
                // ------------------------------------------------------------------ 
                try
                {
                    SqlConnection myConnection = new SqlConnection(ConnectionSrting);
                    myConnection.Open();
                    SqlDataReader myReader = null;
                    SqlCommand myCommand = new SqlCommand(query, myConnection);
                    myCommand.CommandTimeout = CommandTimeout;
                    // ------------------------------------------------------------------
                    myReader = myCommand.ExecuteReader();
                    // ------------------------------------------------------------------
                    while ((myReader.Read()))
                    {
                        // ------------------------------------------------------------------ 
                        for (int i = 0; i <= myReader.FieldCount - 1; i++)
                            Temp_list.Add(myReader[i].ToString());
                        // ------------------------------------------------------------------ 
                        Temp_list_OfLists.Add(Temp_list);
                    }
                    myConnection.Close();
                }
                catch (Exception ex)
                {
                }
            }
            // ------------------------------------------------------------------ 
            return Temp_list_OfLists;
        }

        public string Execute_Of_Query_Function_retOneItem(string query, List<SqlParameter> list = null)
        {
            // ----------------------------------------------------------------------
            string value = "";
            // ---------------------------------------------------------------------- 
            SqlDataReader myReader = null;
            // ----------------------------------------------------------------------
            if ((ConnectionSrting != ""))
            {
                // ------------------------------------------------------------------ 
                SqlConnection myConnection = new SqlConnection(ConnectionSrting);
                SqlCommand myCommand = new SqlCommand(query, myConnection);
                myCommand.CommandTimeout = CommandTimeout;
                // -----------------------------------------------------------------------------
                // Adding the parameters to the database. 
                // Remember that the order in which the parameters are added is VERY important!
                // -----------------------------------------------------------------------------
                if (list != null)
                {
                    for (int i = 0; i <= list.Count - 1; i++)
                        myCommand.Parameters.Add(list[i]);
                }
                // ----------------------------------------------------------------------    
                try
                {
                    myConnection.Open();
                    myReader = myCommand.ExecuteReader();
                    while ((myReader.Read()))
                        value = myReader[0].ToString().Trim();
                    myConnection.Close();
                }
                catch (Exception ex)
                {
                }
                // ------------------------------------------------------------------
                list = null;
                query = null;
                myCommand.Dispose();
                myConnection.Dispose();
            }
            // ------------------------------------------------------------------ 
            return value;
        }


        public DataTable Execute_Of_Query_Function_dataSet(string query, List<SqlParameter> list = null)
        {
            // -----------------------------------------------------------------------------
            DataTable Temp_dataSet = new DataTable();
            SqlConnection myConnection = new SqlConnection(ConnectionSrting);
            SqlCommand myCommand = new SqlCommand(query, myConnection);
            myCommand.Connection = myConnection;
            myCommand.CommandText = query;
            myCommand.CommandTimeout = CommandTimeout;
            // -----------------------------------------------------------------------------
            // Adding the parameters to the database. 
            // Remember that the order in which the parameters are added is VERY important!
            // -----------------------------------------------------------------------------
            if (list != null)
            {
                for (int i = 0; i <= list.Count - 1; i++)
                    myCommand.Parameters.Add(list[i]);
            }
            // -----------------------------------------------------------------------------
            try
            {
                myConnection.Open();
                myCommand.CommandType = CommandType.Text;
                SqlDataAdapter Temp_dataAdapter = new SqlDataAdapter(myCommand);
                Temp_dataAdapter.Fill(Temp_dataSet);
                Temp_dataAdapter.Dispose();
                myConnection.Close();
            }
            catch (Exception ex)
            {
            }
            // -----------------------------------------------------------------------------
            list = null;
            query = null;
            myCommand.Dispose();
            myConnection.Dispose();
            // ----------------------------------------------------------------------------- 
            return Temp_dataSet;
        }

        public void Execute_Of_Query_ByRef_DataSet(string query, ref DataTable Temp_dataSet, List<SqlParameter> list = null)
        {
            // -----------------------------------------------------------------------------
            SqlConnection myConnection = new SqlConnection(ConnectionSrting);
            SqlCommand myCommand = new SqlCommand(query, myConnection);
            myCommand.CommandTimeout = CommandTimeout;
            // -----------------------------------------------------------------------------
            // Adding the parameters to the database. 
            // Remember that the order in which the parameters are added is VERY important!
            // -----------------------------------------------------------------------------
            if (list != null)
            {
                for (int i = 0; i <= list.Count - 1; i++)
                    myCommand.Parameters.Add(list[i]);
            }
            // ----------------------------------------------------------------------
            try
            {
                myConnection.Open();
                SqlDataAdapter Temp_dataAdapter = new SqlDataAdapter(myCommand);
                Temp_dataAdapter.Fill(Temp_dataSet);
                Temp_dataAdapter.Dispose();
                myConnection.Close();
                Temp_dataAdapter.Dispose();
            }
            catch (Exception ex)
            {
            }
            // ------------------------------------------------------------------
            list = null;
            query = null;
            myCommand.Dispose();
            myConnection.Dispose();
        }
    }
}
