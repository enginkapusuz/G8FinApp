using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.OleDb;

namespace G8FinApp.Admin
{
    public class BudgetCommitExcel
    {
        Database.ProgramConsts programConsts = new Database.ProgramConsts();
        private readonly Reports.ReportExcel reportExcel;
        List<BudgetEncumb> budgetList = new List<BudgetEncumb>();

        private class BudgetEncumb
        {
            public string ReqDesc { get; set; }
            public string ReqNum { get;  set; }
            public int ReqItemCount { get;  set; }
            public string ReqCurr { get;  set; }
            public double ReqAmount { get; set; }
            public string BdgtCurr { get; set; }
            public double BdgtAmount { get; set; }
            public DateTime ReqDate { get; set; }
            public string DocNu { get; set; }
            public string CommitNu { get;  set; }
            public string PendingNu { get; set; }
            public string FmCisi { get; set; }
            public string Branch { get; set; }
            public string FmName { get; set; }
            public int FmNo { get; set; }
            public string CisiCode { get; set; }
            public double SumReqAmount { get; set; }
            public double SumBdgtAmount { get; set; }
            public int BudgetMainID { get; set; }
            public BudgetEncumb()
            {

            }
        }

        public BudgetCommitExcel(Reports.ReportExcel _reportExcel)
        {
            reportExcel = _reportExcel;
        }

        public bool ArrangeValues()
        {
            
            short indx = 0;            
            try
            {
                while (reportExcel.ReportSheet.Range["O2"].Offset[indx, 0].Value != null)
                {
                    BudgetEncumb budgetEncumb = new BudgetEncumb()
                    {
                        ReqDesc = reportExcel.ReportSheet.Range["X2"].Offset[indx, 0].Value ?? "",
                        ReqNum = reportExcel.ReportSheet.Range["AA2"].Offset[indx, 0].Value ?? "",
                        ReqItemCount = 1,
                        ReqCurr = reportExcel.ReportSheet.Range["O2"].Offset[indx, 0].Value ?? "",
                        ReqAmount = reportExcel.ReportSheet.Range["N2"].Offset[indx, 0].Value ?? 0,
                        ReqDate = reportExcel.ReportSheet.Range["L2"].Offset[indx, 0].Value ?? DateTime.MinValue,
                        DocNu = "BudgetMain",
                        CommitNu = reportExcel.ReportSheet.Range["M2"].Offset[indx, 0].Value ?? "",
                        PendingNu = reportExcel.ReportSheet.Range["K2"].Offset[indx, 0].Value ?? "",
                        FmCisi = reportExcel.ReportSheet.Range["E2"].Offset[indx, 0].Value ?? "",
                        Branch = reportExcel.ReportSheet.Range["Z2"].Offset[indx, 0].Value ?? "",
                        BdgtCurr = "EURO",
                        BdgtAmount = reportExcel.ReportSheet.Range["P2"].Offset[indx, 0].Value ?? "",
                    };

                    budgetEncumb.FmName = budgetEncumb.FmCisi.Substring(0, 5) + "-" + budgetEncumb.Branch;
                    budgetEncumb.CisiCode = budgetEncumb.FmCisi.Substring(6, 6);
                    budgetEncumb.FmNo = int.TryParse(budgetEncumb.FmCisi.Substring(4, 1), out int intFmNo) ? intFmNo : -1;

                    budgetList.Add(budgetEncumb);
                    indx++;
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show("BudgetCommitExcel:ArrangeValues:" + ex.Message);
                return false;
            }
            return true;
        }

        public bool SaveData()
        {
            using (OleDbConnection con = new OleDbConnection(programConsts.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "INSERT INTO BudgetEncmbTemplate(ReqDesc, ReqNum, ReqItemCount, " +
                    "ReqCurr, ReqAmount, ReqDate, " +
                    "DocNu, CommitNu, PendingNu, " +
                    "FmCisi, Branch, FmName, " +
                    "FmNo, CisiCode, BdgtCurr, " +
                    "BdgtAmount) " +
                    "VALUES(?, ?, ?, " +
                    "?, ?, ?, " +
                    "?, ?, ?, " +
                    "?, ?, ?, " +
                    "?, ?, ?," +
                    "?)",
                };

                try
                {
                    con.Open();
                    foreach (BudgetEncumb budgetEncumb in budgetList)
                    {
                        cmd.Parameters.AddWithValue("@ReqDesc", budgetEncumb.ReqDesc);
                        cmd.Parameters.AddWithValue("@ReqNum", budgetEncumb.ReqNum);
                        cmd.Parameters.AddWithValue("@ReqItemCount", budgetEncumb.ReqItemCount.ToString());

                        cmd.Parameters.AddWithValue("@ReqCurr", budgetEncumb.ReqCurr);
                        cmd.Parameters.AddWithValue("@ReqAmount", budgetEncumb.ReqAmount.ToString());
                        cmd.Parameters.AddWithValue("@ReqDate", budgetEncumb.ReqDate.ToString());

                        cmd.Parameters.AddWithValue("@DocNu", budgetEncumb.DocNu);
                        cmd.Parameters.AddWithValue("@CommitNu", budgetEncumb.CommitNu);
                        cmd.Parameters.AddWithValue("@PendingNu", budgetEncumb.PendingNu);

                        cmd.Parameters.AddWithValue("@FmCisi", budgetEncumb.FmCisi);
                        cmd.Parameters.AddWithValue("@Branch", budgetEncumb.Branch);
                        cmd.Parameters.AddWithValue("@FmName", budgetEncumb.FmName);
                        cmd.Parameters.AddWithValue("@FmNo", budgetEncumb.FmNo.ToString());

                        cmd.Parameters.AddWithValue("@CisiCode", budgetEncumb.CisiCode);
                        cmd.Parameters.AddWithValue("@BdgtCurr", budgetEncumb.BdgtCurr);
                        cmd.Parameters.AddWithValue("@BdgtAmount", budgetEncumb.BdgtAmount.ToString());

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

                    _ = UpdateBudgetEncmbCancelData(con);
                    _ = UpdateBudgetMainId(con);
                    _ = UpdateNegativeNumbers(con);
                    List<BudgetEncumb> budgetEncumbList = FilterCommitNumber(con);
                    _ = InsertIntoBudgetEncmbData(con, budgetEncumbList);
                    _ = InsertIntoBudgetEncumbrance(con, budgetEncumbList);
                    _ = UpdateBudgetMainID(con);
                }   
                catch (Exception ex)
                {
                    _ = MessageBox.Show("BudgetMainExcel:SaveData:" + ex.Message);
                    return false;
                }
            }

            return true;
        }

        private bool UpdateBudgetEncmbCancelData(OleDbConnection con)
        {
            using(OleDbCommand cmd = new OleDbCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "UPDATE BudgetEncmbTemplate AS BET, " +
                    "(SELECT BudgetEncmbTemplateID, ReqDesc, ReqNum, ReqItemCount, ReqCurr, ReqAmount, BdgtCurr, BdgtAmount, ReqDate, Branch FROM BudgetEncmbTemplateCancelled) AS BETC " +
                    "SET BET.ReqDesc = BETC.ReqDesc, BET.ReqNum = BETC.ReqNum, BET.ReqItemCount = BETC.ReqItemCount, BET.ReqCurr = BETC.ReqCurr, BET.ReqAmount = BETC.ReqAmount, BET.BdgtCurr = BETC.BdgtCurr," +
                    " BET.BdgtAmount = BETC.BdgtAmount, BET.ReqDate = BETC.ReqDate, BET.Branch = BETC.Branch, BET.FmName = (BET.FmName & BETC.Branch) " +
                    "WHERE BET.ID =  BETC.BudgetEncmbTemplateID";

                OleDbDataAdapter adapter = new OleDbDataAdapter()
                {
                    UpdateCommand = cmd,
                };

                if (!(adapter.UpdateCommand.ExecuteNonQuery() > 0))
                {
                    return false;
                }
            }

            return true;
        }

        private bool UpdateBudgetMainId(OleDbConnection con)
        {
            using (OleDbCommand cmd = new OleDbCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "UPDATE BudgetEncmbTemplate AS BET, (SELECT * FROM BudgetMain) AS BM " +
                    "SET BET.BudgetMainID = BM.ID, BET.DocNu = ('BudgetMain:' & BM.ID & '/')" +
                    "WHERE BET.FmName = BM.FmName AND BET.CisiCode = BM.CisiCode";

                OleDbDataAdapter adapter = new OleDbDataAdapter()
                {
                    UpdateCommand = cmd,
                };

                if (!(adapter.UpdateCommand.ExecuteNonQuery() > 0))
                {
                    return false;
                }
            }
            return true;
        }

        private bool UpdateNegativeNumbers(OleDbConnection con)
        {
            using (OleDbCommand cmd = new OleDbCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "UPDATE BudgetEncmbTemplate AS BET, (SELECT * FROM BudgetEncmbTemplate WHERE ReqAmount < 0) AS SUB_BET " +
                    "SET BET.ReqAmount = 0, BET.BdgtAmount = 0 " +
                    "WHERE BET.ID = SUB_BET.ID";

                OleDbDataAdapter adapter = new OleDbDataAdapter()
                {
                    UpdateCommand = cmd,
                };

                if (!(adapter.UpdateCommand.ExecuteNonQuery() > 0))
                {
                    return false;
                }
            }
            return true;
        }

        private bool InsertIntoBudgetEncmbData(OleDbConnection con, List<BudgetEncumb> budgetEncumbList)
        {
            MessageBox.Show("One");
            using (OleDbCommand cmd = new OleDbCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "INSERT INTO BudgetEncmbData(ReqDesc, ReqNum, ReqItemCount, ReqCurr, ReqAmount, ReqDate, DocNu) VALUES(?, ?, ?, ?, ?, ?, ?)";

                foreach (BudgetEncumb bdgtEncumb in budgetEncumbList)
                {
                    cmd.Parameters.AddWithValue("@ReqDesc", bdgtEncumb.ReqDesc);
                    cmd.Parameters.AddWithValue("@ReqNum", bdgtEncumb.ReqNum);
                    cmd.Parameters.AddWithValue("@ReqItemCount", bdgtEncumb.ReqItemCount.ToString());
                    cmd.Parameters.AddWithValue("@ReqCurr", bdgtEncumb.ReqCurr);
                    cmd.Parameters.AddWithValue("@ReqAmount", bdgtEncumb.SumReqAmount.ToString());
                    cmd.Parameters.AddWithValue("@ReqDate", bdgtEncumb.ReqDate.ToString());
                    cmd.Parameters.AddWithValue("@DocNu", bdgtEncumb.DocNu);

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

            return true;
        }

        private bool InsertIntoBudgetEncumbrance(OleDbConnection con, List<BudgetEncumb> budgetEncumbList)
        {
            MessageBox.Show("Two");
            using (OleDbCommand cmd = new OleDbCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "INSERT INTO BudgetEncumbrance(FmNo, FmName, CisiCode, BdgtCurr, Amount, TDate, DocNu, BudgetMainID) " +
                    "VALUES(?, ?, ?, ?, ?, ?, ?, ?)";
                foreach (BudgetEncumb bdgtEncumb in budgetEncumbList)
                {
                    cmd.Parameters.AddWithValue("@FmNo", bdgtEncumb.FmNo.ToString());
                    cmd.Parameters.AddWithValue("@FmName", bdgtEncumb.FmName);
                    cmd.Parameters.AddWithValue("@CisiCode", bdgtEncumb.CisiCode);
                    cmd.Parameters.AddWithValue("@BdgtCurr", bdgtEncumb.BdgtCurr);
                    cmd.Parameters.AddWithValue("@Amount", bdgtEncumb.SumBdgtAmount.ToString());
                    cmd.Parameters.AddWithValue("@TDate", bdgtEncumb.ReqDate.ToString());
                    cmd.Parameters.AddWithValue("@DocNu", bdgtEncumb.DocNu);
                    //cmd.Parameters.AddWithValue("@BudgetEncmbDataId", "BudgetEncumbrance");
                    cmd.Parameters.AddWithValue("@BudgetMainID", bdgtEncumb.BudgetMainID);
                    
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
            return true;
        }

        private List<BudgetEncumb> GetBudgetEncumb(OleDbConnection con)
        {
            List<BudgetEncumb> budgetEncumbList = new List<BudgetEncumb>();

            using (OleDbCommand cmd = new OleDbCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "SELECT * FROM BudgetEncmbTemplateGroupQuery";

                OleDbDataReader reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    BudgetEncumb budgetEncumb = new BudgetEncumb()
                    {
                        CommitNu = reader["NewCommitNu"].ToString(),
                        ReqDesc = reader["ReqDesc"].ToString(),
                        ReqNum = reader["ReqNum"].ToString(),
                        ReqCurr = reader["ReqCurr"].ToString(),
                        ReqAmount = double.Parse(reader["ReqAmount"].ToString()),
                        BdgtCurr = reader["BdgtCurr"].ToString(),
                        BdgtAmount = double.Parse(reader["BdgtAmount"].ToString()),
                        ReqDate = DateTime.Parse(reader["ReqDate"].ToString()),
                        DocNu = reader["ReqDate"].ToString(),
                        PendingNu = reader["PendingNu"].ToString(),
                        FmCisi = reader["FmCisi"].ToString(),
                        FmName = reader["FmName"].ToString(),
                        FmNo = int.Parse(reader["FmNo"].ToString()),
                        CisiCode = reader["CisiCode"].ToString(),
                        BudgetMainID = int.Parse(reader["BudgetMainID"].ToString()),
                        SumReqAmount = double.Parse(reader["SumReqAmount"].ToString()),
                        SumBdgtAmount = double.Parse(reader["SumBdgtAmount"].ToString()),
                    };

                    budgetEncumbList.Add(budgetEncumb);
                }
            }

            return budgetEncumbList;
        }

        private List<BudgetEncumb> FilterCommitNumber(OleDbConnection con)
        {
            List<BudgetEncumb> listBudgetEncumb = GetBudgetEncumb(con);
            List<BudgetEncumb> retListBudgetEncumb = new List<BudgetEncumb>();

            foreach(BudgetEncumb outerBdgt in listBudgetEncumb)
            {
                bool isExist = false;

                foreach(BudgetEncumb innerBdgt in retListBudgetEncumb)
                {
                    if (innerBdgt.CommitNu == outerBdgt.CommitNu)
                    {
                        isExist = true;
                    }
                }

                if (!isExist)
                {
                    retListBudgetEncumb.Add(outerBdgt);
                }
            }
            return retListBudgetEncumb;
        }

        private bool UpdateBudgetMainID(OleDbConnection con)
        {
            using(OleDbCommand cmd = new OleDbCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "UPDATE BudgetEnCumbrance AS BE, (SELECT ID FROM BudgetEncumbrance) AS SUB_BE " +
                    "SET BE.BudgetEncmbDataID = SUB_BE.ID WHERE BE.ID = SUB_BE.ID";

                OleDbDataAdapter adapter = new OleDbDataAdapter()
                {
                    UpdateCommand = cmd,
                };

                if (!(adapter.UpdateCommand.ExecuteNonQuery() > 0))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
