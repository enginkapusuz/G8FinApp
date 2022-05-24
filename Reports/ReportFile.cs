using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace G8FinApp.Reports
{
    public class ReportFile
    {
        public string ID { get; set; }
        public string ReportName { get; set; }
        public string ReportPath { get; set; }
        public string TableName {get; private set;}

        public ReportFile TakeFileData(string reportName, string tableName = "")
        {
            ReportFile reportFile = null;
            ReportFileMain reportFileMain;

            if (string.IsNullOrEmpty(reportName))
            {
                return null;
            }

            reportFileMain = new ReportFileMain();
            reportFileMain.InitList();

            foreach(ReportFile rptFile in reportFileMain)
            {
                if (rptFile.ReportName == reportName)
                {
                    reportFile = new ReportFile()
                    {
                        ID = rptFile.ID,
                        ReportName = rptFile.ReportName,
                        ReportPath =  Directory.GetCurrentDirectory() +  rptFile.ReportPath,
                        TableName = tableName,
                    };
                }
            }
            return reportFile;
        }
    }
}
