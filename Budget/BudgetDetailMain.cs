
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
    public class BudgetDetailMain : ObservableCollection<BudgetDetail>
    {
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();

        public BudgetDetailMain()
        {

        }

        public void InitList()
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = con;

                try
                {
                    con.Open();
                    // I removed this tables from query list since: "BudgetCommitIn","BudgetAccrual", "BudgetPending" 
                    string[] tblLst = new string[] { "BudgetIn", "BudgetTransferIn", "BudgetTransferOut", "BudgetEncumbrance", "BudgetRevise" };

                    foreach (string tbl in tblLst)
                    {

                        cmd.CommandText = $"SELECT * FROM {tbl}";

                        OleDbDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            BudgetDetail budgetDetail = new BudgetDetail()
                            {
                                ID = reader["ID"].ToString(),
                                FMNO = reader["FmNo"].ToString(),
                                FMNAME = reader["FmName"].ToString(),
                                CISICODE = reader["CisiCode"].ToString(),
                                BDGTCURR = reader["BdgtCurr"].ToString(),
                                AMOUNT = decimal.Parse(reader["Amount"].ToString()),
                                TDATE = DateTime.Parse(reader["TDate"].ToString()).ToString("d"),
                                DOCNU = reader["DocNu"].ToString(),
                                TABLENAME = tbl,
                                BDGTENCMBDATAID = reader["BudgetEncmbDataID"].ToString(),
                                BUDGETMAINID = reader["BudgetMainID"].ToString(),
                            };
                            Add(budgetDetail);
                        }

                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show("Error(BudgetDetailMain):" + ex.Message);
                    return;
                }
            }
        }

        public bool DeleteData(string tblName, string id)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand($"DELETE FROM {tblName} WHERE ID = @ID", con);
                cmd.CommandType = System.Data.CommandType.Text;
                _ = cmd.Parameters.AddWithValue("@ID", id);

                try
                {
                    con.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter();
                    adapter.DeleteCommand = cmd;
                    if (adapter.DeleteCommand.ExecuteNonQuery() > 0)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show("Error(BudgetDetail:DeleteData):" + ex.Message);
                    return false;
                }
                return false;
            }
        }

        public bool SaveDataEncumbrance(BudgetDetail bdgtDetail)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "INSERT INTO BudgetEncumbrance (FmNo, FmName, CisiCode, BdgtCurr, Amount, TDate, DocNu, BudgetEncmbDataID, BudgetMainID) VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?);",
                };

                _ = cmd.Parameters.AddWithValue("@FmNo", bdgtDetail.FMNO);
                _ = cmd.Parameters.AddWithValue("@FmName", bdgtDetail.FMNAME);
                _ = cmd.Parameters.AddWithValue("@CisiCode", bdgtDetail.CISICODE);

                _ = cmd.Parameters.AddWithValue("@BdgtCurr", bdgtDetail.BDGTCURR);
                _ = cmd.Parameters.AddWithValue("@Amount", bdgtDetail.AMOUNT.ToString());
                _ = cmd.Parameters.AddWithValue("@TDate", bdgtDetail.TDATE);

                _ = cmd.Parameters.AddWithValue("@DocNu", bdgtDetail.DOCNU);
                _ = cmd.Parameters.AddWithValue("@BudgetEncmbDataID", bdgtDetail.BDGTENCMBDATAID);
                _ = cmd.Parameters.AddWithValue("@BudgetMainID", bdgtDetail.BUDGETMAINID);

                try
                {
                    con.Open();

                    OleDbDataAdapter adapter = new OleDbDataAdapter()
                    {
                        InsertCommand = cmd,
                    };

                    if (adapter.InsertCommand.ExecuteNonQuery() > 0)
                    {
                        return true;
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("BudgetDetailMain:SaveDataEncumbrance:" + ex.Message);
                    return false;
                }
            }

            return false;
        }

        public bool SaveData(string tblName, BudgetDetail bdgtDetail)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand($"INSERT INTO " + tblName + " (FmNo, FmName, CisiCode, BdgtCurr, Amount, TDate, DocNu, BudgetMainID, BudgetEncmbDataID) " +
                    "VALUES(@FmNo, @FmName, @CisiCode, @BdgtCurr, @Amount, @TDate, @DocNu, @BudgetMainID, @BudgetEncmbDataID);", con);

                cmd.CommandType = System.Data.CommandType.Text;

                _ = cmd.Parameters.AddWithValue("@FmNo", bdgtDetail.FMNO);
                _ = cmd.Parameters.AddWithValue("@FmName", bdgtDetail.FMNAME);
                _ = cmd.Parameters.AddWithValue("@CisiCode", bdgtDetail.CISICODE);
                _ = cmd.Parameters.AddWithValue("@BdgtCurr", bdgtDetail.BDGTCURR);
                _ = cmd.Parameters.AddWithValue("@Amount", bdgtDetail.AMOUNT.ToString());
                _ = cmd.Parameters.AddWithValue("@TDate", bdgtDetail.TDATE);
                _ = cmd.Parameters.AddWithValue("@DocNu", bdgtDetail.DOCNU);
                _ = cmd.Parameters.AddWithValue("@BudgetMainID", bdgtDetail.BUDGETMAINID);
                _ = cmd.Parameters.AddWithValue("@BudgetEncmbDataID", "0");

                try
                {
                    con.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter();
                    adapter.InsertCommand = cmd;
                    if (adapter.InsertCommand.ExecuteNonQuery() > 0)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show("Error(BudgetDetail-SaveData):" + ex.Message);
                    return false;
                }
            }

            return false;
        }

        private bool LookForBudgetEncumbrance(OleDbConnection con, BudgetDetail budgetDetail)
        {
            using (OleDbCommand cmd = new OleDbCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "SELECT * FROM BudgetApprove WHERE EncumbId = ? ";

                cmd.Parameters.AddWithValue("@EncumbId", budgetDetail.ID);

                try
                {
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        _ = MessageBox.Show("This file has been sent to " +  reader["SendTo"] + Environment.NewLine + "Please inform " + reader["SendTo"] +"!");
                        return true;
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("BudgetDetailMain:LookForBudgetEncumbrance:" + ex.Message);
                    return false;
                }
            }

            return false;
        }

        public bool DeleteBudgetEncumbrance(BudgetDetail budgetDetail)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "DELETE FROM BudgetEncumbrance WHERE ID = ?",
                };

                cmd.Parameters.AddWithValue("@EncumbId", budgetDetail.ID);

                try
                {
                    con.Open();

                    if (LookForBudgetEncumbrance(con, budgetDetail))
                    {
                        return false;
                    }

                    OleDbDataAdapter adapter = new OleDbDataAdapter()
                    {
                        DeleteCommand = cmd,
                    };

                    if (!(adapter.DeleteCommand.ExecuteNonQuery() > 0))
                    {
                        return false;
                    }

                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("BudgetDetailMain:DeleteBudgetDetail:" + ex.Message);
                    return false;
                }
            }
            return true;
        }

        public bool DeleteBudgetRevise(BudgetDetail budgetDetail)
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "DELETE FROM BudgetRevise WHERE ID = ?",
                };

                cmd.Parameters.AddWithValue("@ID", budgetDetail.ID);

                try
                {
                    con.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter()
                    {
                        DeleteCommand = cmd,
                    };

                    if (!(adapter.DeleteCommand.ExecuteNonQuery() > 0))
                    {
                        return false;

                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("BudgetDetailMain:DeleteBudgetRevise:" + ex.Message);
                    return false;
                }
            }

            return true;
        }

        public bool DeleteTransfer(BudgetDetail budgetDetailOut)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                try
                {
                    con.Open();

                    if (!DeleteTransfer(con, budgetDetailOut, "BudgetTransferIn"))
                    {
                        return false;
                    }

                    if (!DeleteTransfer(con, budgetDetailOut, "BudgetTransferOut"))
                    {
                        return false;
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("BudgetDetailMain:DeleteTransfer:" + ex.Message);
                    return false;
                }
            }

            return true;
        }

        private bool DeleteTransfer(OleDbConnection con, BudgetDetail budgetDetailOut, string tableName)
        {
            using(OleDbCommand cmd = new OleDbCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "DELETE FROM "+ tableName +" WHERE ID = ?";

                cmd.Parameters.AddWithValue("@ID", budgetDetailOut.ID);

                OleDbDataAdapter adapter = new OleDbDataAdapter()
                {
                    DeleteCommand = cmd,
                };

                if (!(adapter.DeleteCommand.ExecuteNonQuery() > 0))
                {
                    return false;
                }
            }

            return true;
        }

        public bool DeleteBudgetIn(BudgetDetail budgetDetail)
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "DELETE FROM BudgetIN WHERE ID = ?",
                };

                cmd.Parameters.AddWithValue("@ID", budgetDetail.ID);

                try
                {
                    con.Open();

                    OleDbDataAdapter adapter = new OleDbDataAdapter()
                    {
                        DeleteCommand = cmd,
                    };

                    if (!(adapter.DeleteCommand.ExecuteNonQuery() > 0))
                    {
                        return false;
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("BudgetDetailMain:DeleteBudgetIn:" + ex.Message);
                    return false;
                }
            }

            return true;
        }
    }
}
