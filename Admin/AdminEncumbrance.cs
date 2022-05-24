using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows;
using System.Data.OleDb;


namespace G8FinApp.Admin
{
    public class FundManagerEncumbrance
    {
        public int FundManagerID;
        public double FundBudgetAmount;
        public string FundBudgetCurr;
        public string FmNoCisiCode;
        public string FmName;
        public string FmNo;
        public string CisiCode;
        public string CommitNu;
        public string ReqDesc;
        public string ReqNum;
        public int ReqItemCount;
        public string ReqCurr;
        public double ReqAmount;
        public double CommitEuro;
        public DateTime ReqDate;
        public DateTime CommitDate;
        public string PendingNo;
        public DateTime PendingDate;
        public int ActCode;
        public string PofContact;
        public string PofContactRank;
        public string InvoiceNu;
        public double InvoiceAmount;
        public string Creditor;
    }

    public class AdminEncumbrance
    {
        public FundManager fundManager { get; private set; }
        public FundManagerBudget fundManagerBudget { get; private set; }
        public List<FundManagerEncumbrance> fundManagerEncumbrance { get; private set; }

        public AdminEncumbrance()
        {

        }

        public List<FundManagerEncumbrance> LoadExcelCommitData(Reports.ReportExcel reportExcel, bool isInvoice = false)
        {
            int lastRow = reportExcel.ReportSheet.Cells[reportExcel.ReportSheet.Rows.Count, 5].End[Excel.XlDirection.xlUp].Row;

            reportExcel.ReportSheet.Application.ScreenUpdating = false;
            reportExcel.ReportSheet.Application.Calculation = Excel.XlCalculation.xlCalculationManual;

            List<FundManagerEncumbrance> fundManagerEncumbrances = null;
            
            int indx = 0;
            foreach(Excel.Range rng in reportExcel.ReportSheet.Range["E2:E" + lastRow])
            {
                FundManagerEncumbrance fundManagerEncumbrance = new FundManagerEncumbrance()
                {
                    FmNoCisiCode = reportExcel.ReportSheet.Range["E2"].Offset[indx, 0].Value ?? string.Empty,
                    CommitNu = reportExcel.ReportSheet.Range["M2"].Offset[indx, 0].Value ?? string.Empty,
                    ReqDesc = reportExcel.ReportSheet.Range["X2"].Offset[indx, 0].Value ?? string.Empty,
                    ReqAmount = reportExcel.ReportSheet.Range["N2"].Offset[indx, 0].Value ?? double.MinValue,
                    ReqCurr = reportExcel.ReportSheet.Range["O2"].Offset[indx, 0].Value ?? string.Empty,
                    CommitEuro = reportExcel.ReportSheet.Range["P2"].Offset[indx, 0].Value ?? double.MinValue,
                    ReqNum = reportExcel.ReportSheet.Range["AA2"].Offset[indx, 0].Value ?? string.Empty,
                    CommitDate = reportExcel.ReportSheet.Range["L2"].Offset[indx, 0].Value ?? DateTime.MinValue,
                    PendingNo = reportExcel.ReportSheet.Range["K2"].Offset[indx, 0].Value ?? string.Empty,
                    PendingDate = reportExcel.ReportSheet.Range["J2"].Offset[indx, 0].Value ?? DateTime.MinValue,
                    PofContact = reportExcel.ReportSheet.Range["AB2"].Offset[indx, 0].Value ?? string.Empty,
                    PofContactRank = reportExcel.ReportSheet.Range["AC2"].Offset[indx, 0].Value ?? string.Empty,
                    InvoiceNu = reportExcel.ReportSheet.Range["F2"].Offset[indx, 0].Text ?? string.Empty,
                    InvoiceAmount = reportExcel.ReportSheet.Range["R2"].Offset[indx, 0].Value ?? string.Empty,
                    Creditor = reportExcel.ReportSheet.Range["W2"].Offset[indx, 0].Text ?? string.Empty,
                };

                indx++;

                fundManagerEncumbrance.FmName = fundManagerEncumbrance.FmNoCisiCode == string.Empty ? 
                    string.Empty : fundManagerEncumbrance.FmNoCisiCode.Substring(0, 5);

                fundManagerEncumbrance.FmNo = fundManagerEncumbrance.FmName.Last().ToString();

                fundManagerEncumbrance.CisiCode = fundManagerEncumbrance.FmNoCisiCode == string.Empty ? 
                    string.Empty : fundManagerEncumbrance.FmNoCisiCode.Substring(6, 6);

                fundManagerEncumbrance.ReqAmount = fundManagerEncumbrance.ReqAmount < 0 ? 0 : fundManagerEncumbrance.ReqAmount;

                fundManagerEncumbrance.ReqDate = fundManagerEncumbrance.PendingDate == DateTime.MinValue ? fundManagerEncumbrance.CommitDate 
                    : fundManagerEncumbrance.PendingDate;

                if (fundManagerEncumbrances is null)
                {
                    fundManagerEncumbrances = new List<FundManagerEncumbrance>();
                }
                
                fundManagerEncumbrances.Add(fundManagerEncumbrance);
            }

            reportExcel.ReportSheet.Application.ScreenUpdating = true;
            reportExcel.ReportSheet.Application.Calculation = Excel.XlCalculation.xlCalculationAutomatic;
            reportExcel.ReportSheet.Application.ActiveWorkbook.Saved = true;
            reportExcel.ReportSheet.Application.ActiveWorkbook.Close(Excel.XlSaveAction.xlDoNotSaveChanges);

            return isInvoice ? fundManagerEncumbrances : LoadActivityCodes(SelectDistinctCommitNu(fundManagerEncumbrances));
        }

        public List<FundManagerEncumbrance> SelectDistinctCommitNu(List<FundManagerEncumbrance> inFundManagerEncumbrances)
        {
            int indx = 1;
            var commitFundManagerEncumbrances =
                 from fnM in inFundManagerEncumbrances
                 select new FundManagerEncumbrance
                 {
                     CisiCode = fnM.CisiCode,
                     CommitDate = fnM.CommitDate,
                     CommitEuro = fnM.CommitEuro,
                     CommitNu = fnM.CommitNu == string.Empty ? indx++.ToString() : fnM.CommitNu,
                     FmNo = fnM.FmNo,
                     FmName = fnM.FmName,
                     FmNoCisiCode = fnM.FmNoCisiCode,
                     PendingNo = fnM.PendingNo,
                     PendingDate = fnM.PendingDate,
                     ReqAmount = fnM.ReqAmount,
                     ReqCurr = fnM.ReqCurr,
                     ReqDate = fnM.ReqDate,
                     ReqDesc = fnM.ReqDesc,
                     ReqItemCount = fnM.ReqItemCount,
                     ReqNum = fnM.ReqNum,
                     PofContact = fnM.PofContact,
                     PofContactRank = fnM.PofContactRank,
                 };

            List<FundManagerEncumbrance> groupFundManagerEncumbrances = (from fnM in commitFundManagerEncumbrances
                group fnM by fnM.CommitNu into g
                select new FundManagerEncumbrance
                {
                    CommitNu = g.Key,
                    CisiCode = g.ElementAt(0).CisiCode,
                    CommitDate = g.ElementAt(0).CommitDate,
                    CommitEuro = g.Select(fn => fn.CommitEuro).ToList().Sum() < 0 ? 0 : g.Select(fn => fn.CommitEuro).ToList().Sum(),
                    FmNo = g.ElementAt(0).FmNo,
                    FmName = g.ElementAt(0).FmName,
                    FmNoCisiCode = g.ElementAt(0).FmNoCisiCode,
                    FundManagerID = g.ElementAt(0).FundManagerID,
                    PendingNo = g.ElementAt(0).PendingNo,
                    PendingDate = g.ElementAt(0).PendingDate,
                    ReqAmount = g.Select(fn => fn.ReqAmount).ToList().Sum() < 0 ? 0 : g.Select(fn => fn.ReqAmount).ToList().Sum(),
                    ReqCurr = g.ElementAt(0).ReqCurr,
                    ReqDate = g.ElementAt(0).ReqDate,
                    ReqDesc = g.ElementAt(0).ReqDesc,
                    ReqItemCount = g.ElementAt(0).ReqItemCount,
                    ReqNum = g.ElementAt(0).ReqNum,
                    PofContact = g.ElementAt(0).PofContact,
                    PofContactRank = g.ElementAt(0).PofContactRank,
                }).ToList();

            return groupFundManagerEncumbrances;
        }

        private List<FundManagerEncumbrance> LoadActivityCodes(List<FundManagerEncumbrance> fundManagerEncumbrances)
        {

            string connectString = new AdminDatabase().ConnectStringSRD2022;

            using (OleDbConnection con = new OleDbConnection(connectString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM Fiscal ORDER BY MID(CommitNo, 9, 4)",
                };

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        int? actCode = null;
                        foreach (FundManagerEncumbrance fnd in fundManagerEncumbrances)
                        {
                            if (fnd.CommitNu.Trim() == reader["CommitNo"].ToString().Trim())
                            {
                                actCode = int.Parse(reader["FiscalActivityCode"].ToString().Substring(1, 1));
                                fnd.ActCode = (int)actCode > 0 ? (int)actCode - 1 : 0;
                                break;
                            }

                            if(fnd.PendingNo.Trim() == reader["PendingNo"].ToString().Trim())
                            {
                                actCode = int.Parse(reader["FiscalActivityCode"].ToString().Substring(1, 1));
                                fnd.ActCode = (int)actCode > 0 ? (int)actCode - 1 : 0;
                                break;
                            }
                        }

                        if (actCode is null)
                        {
                            MessageBox.Show(reader["FiscalDescription"].ToString() + ":" + "is not found!");
                            continue;
                        }
                    }

                    return fundManagerEncumbrances;
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("Exception:" + ex.Message);
                    return null;
                }
            }
        }
    }
}
