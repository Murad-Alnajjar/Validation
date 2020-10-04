using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Validation.MyLibrary;

namespace Validation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ReportCreation oReportCreation = new ReportCreation();
            DbCon CSDB = new DbCon();
            DataTable myDataset = new DataTable();
            myDataset = CSDB.Execute_Of_Query_Function_dataSet("Exec  sp_tblFiles_Selct");
            if (myDataset.Rows.Count > 0)
            {
                for (int i = 0; i < myDataset.Rows.Count; i++)
                {



                oReportCreation.LoadDataTable(@"" + myDataset.Rows[i]["vchrPath"].ToString() + "", myDataset.Rows[i]["vchrFileName"].ToString(), Convert.ToInt32(myDataset.Rows[i]["intFileSerial"]), Convert.ToInt32(myDataset.Rows[i]["intProjectSerial"]));
                }
            }

        }


        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}









