using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows;

namespace G8FinApp.Reports
{
    public class ReportExcel
    {
        public Excel._Worksheet ReportSheet { get; private set; }
        public ReportExcel()
        {

        }

        public ReportExcel OpenTemplateFile(ReportFile reportFile, string sheetName = "1")
        {
            Excel.Application excelApplication;
            Excel._Workbook excelWorkBook;
            Excel._Worksheet excelWorkSheet;
            ReportExcel reportExcel = null;

            if (reportFile is null)
            {
                return null;
            }

            try
            {
                excelApplication = new Excel.Application();
                excelApplication.Visible = true;
                excelWorkBook = excelApplication.Workbooks.Open(reportFile.ReportPath, null, true);
                excelWorkSheet = sheetName == "1" ? excelWorkBook.Sheets[int.Parse(sheetName)] : excelWorkBook.Sheets[sheetName];
                reportExcel = new ReportExcel()
                {
                    ReportSheet = excelWorkSheet,
                };
            }
            catch(Exception ex)
            {
                _ = MessageBox.Show("ReportExcel:OpenTemplateFile:" + ex.Message);
                return null;
            }
            return reportExcel;
        }
    }
}
