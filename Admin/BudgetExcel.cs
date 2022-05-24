using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows;

namespace G8FinApp.Admin
{
    public class BudgetExcel
    {
        public string FmCisiCode { get; private set; }
        public int FmNo { get; private set; }
        public string FmName { get; private set; }
        public string CisiCode { get; private set; }

        public double InitialBudget { get; private set; }
        public double OutBudget { get; private set; }
        public double CurrentBudget { get; private set; }

        public List<BudgetExcel> LoadBudgetExcel(Reports.ReportExcel reportExcel)
        {
            int lastRow = reportExcel.ReportSheet.Cells[reportExcel.ReportSheet.Rows.Count, 1].End[Excel.XlDirection.xlUp].Row;
            
            MessageBox.Show("Last Row:" + lastRow.ToString());
            MessageBox.Show("Sheet Name:" + reportExcel.ReportSheet.Name);

            List<BudgetExcel> budgetExcels = null;

            reportExcel.ReportSheet.Application.ScreenUpdating = false;
            reportExcel.ReportSheet.Application.Calculation = Excel.XlCalculation.xlCalculationManual;

            int indx = 0;
            try
            {
                foreach (Excel.Range rng in reportExcel.ReportSheet.Range["B2:B" + lastRow.ToString()])
                {
                    if (rng.Value is null || rng.Value2 is null || rng.Value == "")
                    {
                        break;
                    }

                    BudgetExcel bdgtExcel = new BudgetExcel()
                    {
                        FmCisiCode = reportExcel.ReportSheet.Range["B2"].Offset[indx, 0].Value,
                        InitialBudget = reportExcel.ReportSheet.Range["D2"].Offset[indx, 0].Value,
                        OutBudget = reportExcel.ReportSheet.Range["F2"].Offset[indx, 0].Value,
                        CurrentBudget = reportExcel.ReportSheet.Range["G2"].Offset[indx, 0].Value,
                    };

                    indx++;

                    bdgtExcel.CisiCode = bdgtExcel.FmCisiCode.Substring(6, 6);
                    bdgtExcel.FmNo = int.Parse(bdgtExcel.FmCisiCode.Substring(4, 1));
                    bdgtExcel.FmName = bdgtExcel.FmCisiCode.Substring(0, 5);

                    if (budgetExcels is null)
                    {
                        budgetExcels = new List<BudgetExcel>();
                    }

                    budgetExcels.Add(bdgtExcel);
                }

                reportExcel.ReportSheet.Application.ScreenUpdating = true;
                reportExcel.ReportSheet.Application.Calculation = Excel.XlCalculation.xlCalculationAutomatic;
                reportExcel.ReportSheet.Application.ActiveWorkbook.Close(Excel.XlSaveAction.xlDoNotSaveChanges, null, null);

                return DistinctBudgetExcels(budgetExcels);

            }
            catch(Exception ex)
            {
                _ = MessageBox.Show("Exception:" + ex.Message + Environment.NewLine +
                    "indx:" + indx + Environment.NewLine +
                    "Value:" + reportExcel.ReportSheet.Cells[indx, 2].Value);

                return null;
            }
        }

        private List<BudgetExcel> DistinctBudgetExcels(List<BudgetExcel> budgetExcels)
        {
            var distinctBdgtExcels = 
                from bdgt in budgetExcels
                group bdgt by bdgt.FmCisiCode into g
                select new BudgetExcel
                {
                    FmName = g.ElementAt(0).FmName,
                    FmNo = g.ElementAt(0).FmNo,
                    FmCisiCode = g.Key,
                    CisiCode = g.ElementAt(0).CisiCode,
                    CurrentBudget = g.Select(bd => bd.CurrentBudget).Sum(),
                    OutBudget = g.Select(bd => bd.OutBudget).Sum(),
                    InitialBudget = g.Select(bd => bd.InitialBudget).Sum(),
                };

            return distinctBdgtExcels.ToList();
        }
    }
}
