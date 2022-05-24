using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;

namespace G8FinApp.Admin
{
    public class AdminProcurementSheet
    {
        public string BiddingId { get; private set; }
        public string BiddingName { get; private set; }
        public string PendingNo { get; private set; }
        public DateTime BiddingOpenDate { get; private set; }
        public DateTime BiddingCloseDate { get; private set; }
        public DateTime DeliveryOpenDate { get; private set; }
        public DateTime DeliveryCloseDate { get; private set; }

        public List<AdminProcurementSheet> LoadProcurementSheet(Reports.ReportExcel reportExcel)
        {
            List<AdminProcurementSheet> adminProcurementSheets = null;

            int lstRowNum = reportExcel.ReportSheet.Cells[reportExcel.ReportSheet.Rows.Count, 2].End[Excel.XlDirection.xlUp].Row;

            int indx = 0;
            try
            {
                foreach (Excel.Range rng in reportExcel.ReportSheet.Range["B4:B" + lstRowNum])
                {
                    if (rng.Value == "")
                    {
                        break;
                    }

                    AdminProcurementSheet adminProcurementSheet = new AdminProcurementSheet()
                    {
                        BiddingId = reportExcel.ReportSheet.Range["B4"].Offset[indx, 0].Value ?? string.Empty,
                        BiddingName = reportExcel.ReportSheet.Range["C4"].Offset[indx, 0].Value ?? string.Empty,
                        PendingNo = reportExcel.ReportSheet.Range["L4"].Offset[indx, 0].Value ?? string.Empty,
                        BiddingOpenDate = reportExcel.ReportSheet.Range["P4"].Offset[indx, 0].Value ?? DateTime.MinValue,
                        BiddingCloseDate = reportExcel.ReportSheet.Range["Q4"].Offset[indx, 0].Value ?? DateTime.MinValue,
                        DeliveryOpenDate = reportExcel.ReportSheet.Range["V4"].Offset[indx, 0].Value ?? DateTime.MinValue,
                        DeliveryCloseDate = reportExcel.ReportSheet.Range["W4"].Offset[indx, 0].Value ?? DateTime.MinValue,
                    };

                    if (adminProcurementSheets is null)
                    {
                        adminProcurementSheets = new List<AdminProcurementSheet>();
                    }
                    adminProcurementSheets.Add(adminProcurementSheet);
                    indx++;
                }
            }
            catch(Exception ex)
            {
                _ = MessageBox.Show(ex.Message);
                return null;
            }

            return adminProcurementSheets;
        }
    }
}
