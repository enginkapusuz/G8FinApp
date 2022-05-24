using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Windows;

namespace G8FinApp.Fiscal
{
    public class CommitMain : ObservableCollection<Commit>
    {
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();

        public CommitMain()
        {
            InitList();
        }

        public void InitList()
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT ID, CommitNu, CommitDate, CommitAmount, CommitCurr FROM FiscalCommit"
                };

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Add(new Commit(reader[0].ToString(),
                            reader[1].ToString(),
                            DateTime.Parse(reader[2].ToString()),
                            decimal.Parse(reader[3].ToString()),
                            reader[4].ToString()));
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Error:CommitMain:InitList:" + ex.Message);
                    return;
                }
            }

        }

        private string TakeLastIdN(OleDbConnection con)
        {
            string tmpIdNu = string.Empty;

            try
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT ID FROM FiscalCommit"

                };

                OleDbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tmpIdNu = reader[0].ToString();
                }

                return tmpIdNu != string.Empty ? tmpIdNu : "0";
            }
            catch(Exception ex)
            {
                _ = MessageBox.Show("Error:CommitMain:TakeLastIdNu" + ex.Message);
                return string.Empty;
            }
        }

        public bool InitCommitData(ref Commit commit)
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT ID, FmNo, CisiCode FROM BudgetEncumbrance WHERE ID = @ID",
                };

                _ = cmd.Parameters.AddWithValue("@ID", commit.ENCUMID);

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        if (reader[0].ToString() == commit.ENCUMID)
                        {
                            commit.ID = TakeLastIdN(con);
                            commit.FMNO = reader[1].ToString();
                            commit.CISICODE = reader[2].ToString();
                            return true;
                        }
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("CommitMain-InitCommitData:" + ex.Message);
                    return false;
                }
            }

            return false;
        }
        
        private bool IsBudgetEnough(string mainId, decimal budgetAmount, decimal exRate, decimal reqAmount, OleDbConnection con)
        {
            OleDbCommand cmd = new OleDbCommand()
            {
                Connection = con,
                CommandType = System.Data.CommandType.Text,
                CommandText = "SELECT CurrentAmount FROM BudgetCurrentAmount WHERE ID = ?",
            };

            cmd.Parameters.AddWithValue("ID", mainId);

            OleDbDataReader reader = cmd.ExecuteReader();

            while(reader.Read())
            {
                //To find current budget after subtract encumbrance amount

                decimal currBudgetMinusBdgtAmount = decimal.Parse(reader[0].ToString()) + budgetAmount;
                decimal newBudgetAmount = decimal.Parse((reqAmount / exRate).ToString("N4"));

                MessageBox.Show(nameof(budgetAmount) + ":" + budgetAmount.ToString("N4"));
                MessageBox.Show(nameof(currBudgetMinusBdgtAmount) + ":" + currBudgetMinusBdgtAmount.ToString("N4"));
                MessageBox.Show(nameof(newBudgetAmount) + ":" + newBudgetAmount.ToString("N4"));

                if (currBudgetMinusBdgtAmount < newBudgetAmount)
                {
                    return false; ;
                }
            }
            return true;
        }
        private bool UpdateEncmbData(string encumbId, decimal reqAmount, OleDbConnection con)
        {
            OleDbCommand cmd = new OleDbCommand()
            {
                Connection = con,
                CommandType = System.Data.CommandType.Text,
                CommandText = "UPDATE BudgetEncmbData SET ReqAmount = ? WHERE ID = ?",
            };

            OleDbDataAdapter adapter = new OleDbDataAdapter()
            {
                UpdateCommand = cmd,
            };

            cmd.Parameters.AddWithValue("@ReqAmount", reqAmount.ToString(prgrmConst.curFormat));
            cmd.Parameters.AddWithValue("@ID", encumbId);

            if (adapter.UpdateCommand.ExecuteNonQuery() > 0)
            {
                return true;
            }

            return false;
        }
        private bool UpdateEncumbrance(string encumbId, decimal bdgtAmount, OleDbConnection con)
        {
            OleDbCommand cmd = new OleDbCommand()
            {
                Connection = con,
                CommandType = System.Data.CommandType.Text,
                CommandText = "UPDATE BudgetEncumbrance SET Amount = ? WHERE ID = ?",
            };

            cmd.Parameters.AddWithValue("@Amount", bdgtAmount.ToString(prgrmConst.curFormat));
            cmd.Parameters.AddWithValue("@ID", encumbId);

            OleDbDataAdapter adapter = new OleDbDataAdapter()
            {
                UpdateCommand = cmd,
            };

            if (adapter.UpdateCommand.ExecuteNonQuery() > 0)
            {
                return true;
            }

            return false;
        }

        public bool SaveData(Commit commit)
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "INSERT INTO FiscalCommit (MainId, EncumbId, CommitNu, CommitDate, CommitAmount, " +
                    "CommitCurr, ActCode, BdgtCurr, BdgtAmount) VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?)"
                };

                _ = cmd.Parameters.AddWithValue("@MainId", commit.MAININD.Trim());
                _ = cmd.Parameters.AddWithValue("@EncumbId", commit.ENCUMID.Trim());
                _ = cmd.Parameters.AddWithValue("@CommitNu", commit.COMMITNO.Trim());

                _ = cmd.Parameters.AddWithValue("@CommitDate", commit.APPDATE.ToString());
                _ = cmd.Parameters.AddWithValue("@CommitAmount", commit.REQAMOUNT.ToString("N4"));
                _ = cmd.Parameters.AddWithValue("@CommitCurr", commit.REQCURR.Trim());

                _ = cmd.Parameters.AddWithValue("@ActCode", commit.ACTCODE.Trim());
                _ = cmd.Parameters.AddWithValue("@BdgtCurr", commit.BDGTCURR);
            

                try
                {
                    con.Open();

                    if (!IsBudgetEnough(commit.MAININD, commit.BDGTAMOUNT, commit.EXRATE, commit.REQAMOUNT, con))
                    {
                        _ = MessageBox.Show("Budget Amount is not enough for this commit!");
                        return false;
                    }


                    decimal newBudgetAmount = commit.REQAMOUNT / commit.EXRATE;

                    if (!UpdateEncumbrance(commit.ENCUMID, newBudgetAmount, con))
                    {
                        _ = MessageBox.Show("Budget Table coldn't update!");
                        return false;
                    }

                    if (!UpdateEncmbData(commit.ENCUMID, commit.REQAMOUNT, con))
                    {
                        _ = MessageBox.Show("Budget EncumbData couldn't update!");
                        return false;
                    }

                    _ = cmd.Parameters.AddWithValue("@BdgtAmount", newBudgetAmount.ToString(prgrmConst.curFormat));

                    OleDbDataAdapter adapter = new OleDbDataAdapter();
                    adapter.InsertCommand = cmd;
                    if (adapter.InsertCommand.ExecuteNonQuery() > 0)
                    {
                        return true;
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("Error:CommitMain:SaveData" + ex.Message);
                    return false;
                }
            }
            return false;
        }

        public bool UpdateFiscalCommitAndInserCommitCancel(CommitApprove commitApprove)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {

                try
                {
                    con.Open();

                    bool isCommitDeleted = UpdateCommit(con, commitApprove);
                    if (!isCommitDeleted)
                    {
                        return false;
                    }

                    bool isCommitInsertToCommitCancel = InsertCommitToCancelTable(con, commitApprove);
                    if (isCommitInsertToCommitCancel)
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show("CommitMain:DeleteData:" + ex.Message);
                    return false;
                }
            }
            return true;
        }

        private bool UpdateCommit(OleDbConnection con, CommitApprove commitApprove)
        {
            using(OleDbCommand cmd = new OleDbCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "UPDATE FiscalCommit SET CommitNu = ? WHERE CommitNu = ?";

                cmd.Parameters.AddWithValue("@CommitNu", "Cancelled");
                cmd.Parameters.AddWithValue("@CommitNu", commitApprove.CommitNu.ToString());

                OleDbDataAdapter adapter = new OleDbDataAdapter()
                {
                    UpdateCommand = cmd,
                };

                if (!(adapter.UpdateCommand.ExecuteNonQuery() > 0))
                {
                    return false;
                }
                return true;
            }
        }

        private bool InsertCommitToCancelTable(OleDbConnection con, CommitApprove commitApprove)
        {
            using(OleDbCommand cmd = new OleDbCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "INSERT INTO FiscalCommitCancelled (MainId, EncumbId, CommitNu, CommitDate, CommitAmount, " +
                    "CommitCurr, ActCode, BdgtCurr, BdgtAmount) VALUES(?, ?, ?,  ?, ?, ?,  ?, ?, ?)";

                MessageBox.Show("MainId:" + commitApprove.MainId + Environment.NewLine +
                    "EncumbId:" + commitApprove.EncumbId + Environment.NewLine +
                    "CommitNu:" + commitApprove.CommitNu + Environment.NewLine +
                    "CommitDate:" + commitApprove.ApproveDate.ToString() + Environment.NewLine +
                    "CommitAmount:" + commitApprove.CommitAmount.ToString() + Environment.NewLine +
                    "CommitCurr:" + commitApprove.CommitCurr + Environment.NewLine +
                    "ActCode:" + commitApprove.CommitActCode + Environment.NewLine +
                    "BdgtCurr:" + commitApprove.BdgtCurr + Environment.NewLine +
                    "BdgtAmount:" + commitApprove.BdgtAmount.ToString());


                _ = cmd.Parameters.AddWithValue("@MainId", commitApprove.MainId.Trim());
                _ = cmd.Parameters.AddWithValue("@EncumbId", commitApprove.EncumbId.Trim());
                _ = cmd.Parameters.AddWithValue("@CommitNu", commitApprove.CommitNu.Trim());

                _ = cmd.Parameters.AddWithValue("@CommitDate", commitApprove.CommitDate.ToString("d"));
                _ = cmd.Parameters.AddWithValue("@CommitAmount", commitApprove.CommitAmount.ToString("N4"));
                _ = cmd.Parameters.AddWithValue("@CommitCurr", commitApprove.CommitCurr.Trim());

                _ = cmd.Parameters.AddWithValue("@ActCode", commitApprove.CommitActCode.Trim());
                _ = cmd.Parameters.AddWithValue("@BdgtCurr", commitApprove.BdgtCurr);
                _ = cmd.Parameters.AddWithValue("@BdgtAmount", commitApprove.BdgtAmount.ToString("N4"));

                OleDbDataAdapter adapter = new OleDbDataAdapter()
                {
                    InsertCommand = cmd,
                };

                if (!(adapter.InsertCommand.ExecuteNonQuery() > 0))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
