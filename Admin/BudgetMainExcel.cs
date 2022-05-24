using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.OleDb;


namespace G8FinApp.Admin
{
    public class BudgetMainExcel
    {
        Database.ProgramConsts programConsts = new Database.ProgramConsts();

        private readonly Reports.ReportExcel reportExcel;
        List<Budget> budgetList = new List<Budget>();

        private class Budget
        {
            public string FmNo { get; private set; }
            public string FmName { get; private set; }
            public string CisiCode { get; private set; }
            public string CisiDesc { get; private set; }
            public string BdgtCurr { get; private set; }

            public Budget(string fmNo, string fmName, string branches, string cisiCode, string cisiDesc, string bdgtCurr)
            {
                FmNo = fmNo.Substring(4, 1);
                FmName = fmName.Substring(0, 5) + "-" + branches.Trim();
                CisiCode = cisiCode.Substring(6, 6);
                CisiDesc = cisiDesc.Trim();
                BdgtCurr = bdgtCurr;
            }
        }
        public BudgetMainExcel(Reports.ReportExcel _reportExcel)
        {
            reportExcel = _reportExcel;
        }

        public bool ArrangeValues()
        {
            
            short indx = 0;
            try
            {
                while (reportExcel.ReportSheet.Range["B2"].Offset[indx, 0].Value != null)
                {
                    string fmNoText = reportExcel.ReportSheet.Range["B2"].Offset[indx, 0].Value;
                    string fmNametext = reportExcel.ReportSheet.Range["B2"].Offset[indx, 0].Value;
                    string branchesText = reportExcel.ReportSheet.Range["A2"].Offset[indx, 0].Value;
                    string cisiCodeText = reportExcel.ReportSheet.Range["B2"].Offset[indx, 0].Value;
                    string cisiDescText = reportExcel.ReportSheet.Range["C2"].Offset[indx, 0].Value;
                    string bdgtCurrText = "EURO";

                    Budget budget = new Budget(fmNoText, fmNametext, branchesText, cisiCodeText, cisiDescText, bdgtCurrText);
                    budgetList.Add(budget);
                    indx++;
                }
            }
            catch(Exception ex)
            {
                _ = MessageBox.Show("BudgetMainExcel:WriteHeaders:" + ex.Message);
                return false;
            }
            return true;
        }

        public bool SaveData()
        {
            using(OleDbConnection con = new OleDbConnection(programConsts.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "INSERT INTO BudgetMain(FmNo, FmName, CisiCode, CisiDesc, BdgtCurr) VALUES(?, ?, ?, ?, ?)",
                };

                try
                {
                    con.Open();
                    foreach (Budget budget in budgetList)
                    {
                        cmd.Parameters.AddWithValue("@FmNo", budget.FmNo);
                        cmd.Parameters.AddWithValue("@FmName", budget.FmName);
                        cmd.Parameters.AddWithValue("@CisiCode", budget.CisiCode);
                        cmd.Parameters.AddWithValue("@CisiDesc", budget.CisiDesc);
                        cmd.Parameters.AddWithValue("@BdgtCurr", budget.BdgtCurr);

                        OleDbDataAdapter adapter = new OleDbDataAdapter()
                        {
                            InsertCommand = cmd,
                        };

                        if (!(adapter.InsertCommand.ExecuteNonQuery() > 0))
                        {
                            return false;
                        }

                        cmd.Parameters.Clear();
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("BudgetMainExcel:SaveData:" + ex.Message);
                    return false;
                }
            }

            return true;
        }
    }
}
