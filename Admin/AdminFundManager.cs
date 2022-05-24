using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows;

namespace G8FinApp.Admin
{
    public struct FundManager
    {
        public int FundManagerID;
        public string FundManagerName;
        public int FundManagerNo;
    }
    
    public struct FundManagerBudget
    {
        public int FundManagerNo;
        public string BudgetCurrency;
        public string CisiCode;
        public double InitialBudget;
        public double RevisedBudget;
        public double TransferBudget;
        public double SpentBudget;
        public double EncumbranceBudget;
    }

    public class AdminFundManager
    {
        public FundManager fundManager = new FundManager();
        public List<FundManagerBudget> fundManagerBudgets = new List<FundManagerBudget>();
        public string fundManagerCisiCode;

        public List<AdminFundManager> LoadAdminFundManagersFromExcel(Reports.ReportExcel reportExcel)
        {

            List<AdminFundManager> adminFundManagersName = TakeAdminFundManagers(reportExcel);

            if (adminFundManagersName is null)
            {
                return null;
            }

            List<AdminFundManager> adminFundManagersNamesCisiCodes = LoadCisiCodeAndBudgets(adminFundManagersName, reportExcel);

            if (adminFundManagersNamesCisiCodes is null)
            {
                return null;
            }

            return adminFundManagersNamesCisiCodes;
        }

        private List<AdminFundManager> TakeAdminFundManagers(Reports.ReportExcel reportExcel)
        {
            List<AdminFundManager> adminFundManagers = null;

            List<string> allFundManagerNames = TakeFundManagerName(reportExcel);

            if (allFundManagerNames is null)
            {
                return null;
            }

            foreach(string fmName in allFundManagerNames)
            {
               if (!int.TryParse(fmName.Substring(4, 1), out int intFmNo))
               {
                    continue;
               }

                FundManager fundManager = new FundManager()
                {
                    FundManagerName = fmName.Trim(),
                    FundManagerNo = intFmNo,
                };

                AdminFundManager adminFundManager = new AdminFundManager()
                {
                    fundManager = fundManager,
                };

                if (adminFundManagers is null)
                {
                    adminFundManagers = new List<AdminFundManager>();
                }

                adminFundManagers.Add(adminFundManager);
            }

            return adminFundManagers;
        }

        private List<AdminFundManager> LoadCisiCodeAndBudgets(List<AdminFundManager> adminFundManagers, Reports.ReportExcel reportExcel)
        {
            List<AdminFundManager> retAdminFundManager = null;

            foreach(AdminFundManager adm in adminFundManagers)
            {
                AdminFundManager adminFundManager = LoadCisiCodeForOne(adm, reportExcel);

                if (adminFundManager is null)
                {
                    return null;
                }

                if (retAdminFundManager is null)
                {
                    retAdminFundManager = new List<AdminFundManager>();
                }
                retAdminFundManager.Add(adminFundManager);
            }

            return retAdminFundManager;
        }

        private List<string> TakeFundManagerName(Reports.ReportExcel reportExcel)
        {
            int lastRow = reportExcel.ReportSheet.Cells[reportExcel.ReportSheet.Rows.Count, 1].End[Excel.XlDirection.xlUp].Row;
            List<string> fmNames = null;

            foreach(Excel.Range rng in reportExcel.ReportSheet.Range["B2:B" + lastRow.ToString()])
            {
                if (fmNames is null)
                {
                    fmNames = new List<string>();
                }

                if (rng.Value is null || rng.Value == "")
                {
                    continue;
                }

                fmNames.Add(rng.Value);
            }
            return fmNames is null ? null : fmNames.Select(fm => fm).Distinct().ToList();
        }

        private AdminFundManager LoadCisiCodeForOne(AdminFundManager adminFundManager, Reports.ReportExcel reportExcel)
        {
            int lastRow = reportExcel.ReportSheet.Cells[reportExcel.ReportSheet.Rows.Count, 1].End[Excel.XlDirection.xlUp].Row;

            if (lastRow <= 2)
            {
                _ = MessageBox.Show("Last row is less than 3 so there is no data!");
                return null;
            }

            foreach(Excel.Range rng in reportExcel.ReportSheet.Range["C2:C" + lastRow])
            {
                if (rng.Offset[0, -1].Value != adminFundManager.fundManager.FundManagerName)
                {
                    continue;
                }

                rng.Value = "'" + rng.Value;

                FundManagerBudget fundManagerBudget = new FundManagerBudget()
                {
                    CisiCode = rng.Value,
                    InitialBudget = rng.Offset[0, 2].Value2,
                    RevisedBudget = rng.Offset[0, 4].Value2,
                    TransferBudget = rng.Offset[0, 5].Value2
                };

                adminFundManager.fundManagerBudgets.Add(fundManagerBudget);
            }
            return adminFundManager;
        }
    }
}
