using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows;

namespace G8FinApp.Reports
{

    public class ReportRange
    {
        public Dictionary<string, Excel.Range> excelRange { get; private set; }
        public ReportRange()
        {

        }

        public ReportRange TakeRange(ReportExcel _reportExcel, ReportFile reportFile)
        {
            ReportRangeMain reportRangeMain = new ReportRangeMain();
            ReportRange reportRange = new ReportRange();

            reportRange.excelRange = reportRangeMain.LoadRangeData(_reportExcel, reportFile);

            if (reportRange.excelRange is null)
            {
                return null;
            }

            return reportRange;
        }
        public bool WriteRange(Dictionary<string, string> dictReport)
        {
            try
            {
                foreach (string dKey in dictReport.Keys)
                {
                    excelRange[dKey].Value = dictReport[dKey];
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show("ReportRange:WriteRange:Not Found:" + ex.Message);
                return false;
            }
            return true;
        }
    }
}