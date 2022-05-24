using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Windows;



namespace G8FinApp.Budget
{
    public class NotApproveMain: ObservableCollection<NotApprove>
    {
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();

        public NotApproveMain()
        {
            InitList();
        }

        public NotApproveMain(bool IsDeleteOperation = true)
        {

        }
        public void InitList()
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM BudgetNotapprove WHERE ApproveChoice = 'Empty'",
                };

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();

                    while(reader.Read())
                    {
                        NotApprove notApprove = new NotApprove()
                        {
                            ID = reader["ID"].ToString(),
                            MainId = reader["MainId"].ToString(),
                            EncumbId = reader["EncumbId"].ToString(),
                            TableName = reader["TableName"].ToString(),
                            SendTo = reader["SendTo"].ToString(),
                            ApproveChoice = reader["ApproveChoice"].ToString(),
                            AppDate = DateTime.Parse(reader["AppDate"].ToString()),
                            FmName = reader["FmName"].ToString(),
                            ReqDesc = reader["ReqDesc"].ToString(),
                            ReqAmount = decimal.Parse(reader["ReqAmount"].ToString()),
                            ReqCurr = reader["ReqCurr"].ToString(),
                            BdgtCurr = reader["BdgtCurr"].ToString(),
                            BdgtAmount = decimal.Parse(reader["BdgtAmount"].ToString()),
                        };

                        Add(notApprove);
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("NotApproveMain:InitList:" + ex.Message);
                    return;
                }
            }
        }

        private string GetBudgetEncmbData(string BudgetEncumbranceID, OleDbConnection con)
        {
            string budgetEncmbDataID = string.Empty;

            OleDbCommand cmd = new OleDbCommand()
            {
                Connection = con,
                CommandType = System.Data.CommandType.Text,
                CommandText = "SELECT BudgetEncmbDataID FROM BudgetEncumbrance WHERE ID = ?",
            };

            cmd.Parameters.AddWithValue("@ID", BudgetEncumbranceID);

            try
            {
                OleDbDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    budgetEncmbDataID = reader["BudgetEncmbDataID"].ToString();
                }
            }
            catch(Exception ex)
            {
                _ = MessageBox.Show("NotApproveMain:GetBudgetEncmbData:" + ex.Message);
                return "-1";
            }

            return budgetEncmbDataID;
        }

        public bool DeleteData(NotApprove notApprove)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                try
                {
                    con.Open();

                    string resultofGetBudgetEncmbData = GetBudgetEncmbData(notApprove.EncumbId, con);

                    if (resultofGetBudgetEncmbData == string.Empty || resultofGetBudgetEncmbData == "-1")
                    {
                        return false;
                    }

                    OleDbCommand cmd = new OleDbCommand()
                    {
                        Connection = con,
                        CommandType = System.Data.CommandType.Text,
                        CommandText = "DELETE FROM BudgetEncumbrance WHERE ID = ?",
                    };

                    cmd.Parameters.AddWithValue("@ID", notApprove.EncumbId);

                    OleDbDataAdapter adapter = new OleDbDataAdapter()
                    {
                        DeleteCommand = cmd,
                    };

                    if (adapter.DeleteCommand.ExecuteNonQuery() > 0)
                    {
                        cmd.CommandText = "DELETE FROM BudgetEncmbData WHERE ID = ?";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@ID", resultofGetBudgetEncmbData);

                        if (adapter.DeleteCommand.ExecuteNonQuery() > 0)
                        {
                            return true;
                        }
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("NotApproveMain:DeleteData:" + ex.Message);
                    return false;
                }
            }

            return false;
        }

        public bool UpdateData(NotApprove notApprove)
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "UPDATE BudgetNotapprove SET ApproveChoice = ? WHERE ID = ?",
                };

                _ = cmd.Parameters.AddWithValue("@ApproveChoice", notApprove.ApproveChoice);
                _ = cmd.Parameters.AddWithValue("@ID", notApprove.ID);

                try
                {
                    con.Open();

                    OleDbDataAdapter adapter = new OleDbDataAdapter()
                    {
                        UpdateCommand = cmd,
                    };

                    if (!(adapter.UpdateCommand.ExecuteNonQuery() > 0))
                    {
                        return false;
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("NoApproveMain:UpdateData:" + ex.Message);
                    return false;
                }
            }
            return true;
        }
    }
}
