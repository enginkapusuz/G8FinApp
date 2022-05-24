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
    public class BudgetMain : ObservableCollection<Budget>
    {
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();
        
        public BudgetMain()
        {
            //InitList();
        }

        public void InitList()
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM BudgetMainCurrentAmount ORDER BY FmNo, CisiCode",
                };

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {

                        decimal inamount = decimal.TryParse(reader["InAmount"].ToString(), out decimal dcmlInAmount) ? dcmlInAmount : 0;
                        decimal outamount = decimal.TryParse(reader["OutAmount"].ToString(), out decimal dcmlOutAmount) ? dcmlOutAmount : 0;
                        decimal curamount = decimal.TryParse(reader["CurrentAmount"].ToString(), out decimal dcmlCurAmount) ? dcmlCurAmount: 0;

                        Budget budget = new Budget()
                        {
                            ID = reader[0].ToString(),
                            FMNO = int.Parse(reader["FmNo"].ToString()),
                            FMNAME = reader["FmName"].ToString(),
                            CISICODE = reader["CisiCode"].ToString(),
                            CISIDESC = reader["CisiDesc"].ToString(),
                            BDGTCURR = reader["BdgtCurr"].ToString(),
                            INAMOUNT = inamount,
                            OUTAMOUNT = outamount,
                            CURRAMOUNT = curamount,
                        };
                        Add(budget);
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("Error:" + ex.Message);
                    return;
                }
            }
        }

        public bool SaveData(Budget bdgt)
        {
            try
            {
                using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
                {
                    OleDbCommand cmdCheck = new OleDbCommand()
                    {
                        Connection = con,
                        CommandType = System.Data.CommandType.Text,
                        CommandText = "SELECT * FROM BudgetMain Where FmNo = ? And FmName = ? And CisiCode = ? And BdgtCurr = ?",
                    };

                    _ = cmdCheck.Parameters.AddWithValue("@FmNo", bdgt.FMNO.ToString());
                    _ = cmdCheck.Parameters.AddWithValue("@FmName", bdgt.FMNAME);
                    _ = cmdCheck.Parameters.AddWithValue("@CisiCode", bdgt.CISICODE);
                    _ = cmdCheck.Parameters.AddWithValue("@BdgtCurr", bdgt.BDGTCURR);

                    con.Open();

                    OleDbDataReader reader = cmdCheck.ExecuteReader();
                    
                    while(reader.Read())
                    {
                        _ = MessageBox.Show("This record with same currency is already in table!");
                        return false;
                    }

                    OleDbCommand cmd = new OleDbCommand()
                    {
                        Connection = con,
                        CommandType = System.Data.CommandType.Text,
                        CommandText = "INSERT INTO BudgetMain (FmNo, FmName, CisiCode, CisiDesc, BdgtCurr) VALUES(?, ?, ?, ?, ?)",
                    };

                    _ = cmd.Parameters.AddWithValue("@FmNo", bdgt.FMNO.ToString());
                    _ = cmd.Parameters.AddWithValue("@FmName", bdgt.FMNAME);
                    _ = cmd.Parameters.AddWithValue("@CisiCode", bdgt.CISICODE);
                    _ = cmd.Parameters.AddWithValue("@CisiDesc", bdgt.CISIDESC);
                    _ = cmd.Parameters.AddWithValue("@BdgtCurr", bdgt.BDGTCURR);
                    
                    OleDbDataAdapter adapter = new OleDbDataAdapter();
                    adapter.InsertCommand = cmd;
                    if (adapter.InsertCommand.ExecuteNonQuery() > 0)
                    {
                        return true;
                    }
                }
            }
            catch(Exception ex)
            {
                _ = MessageBox.Show("Error(BudgetMain-SaveData): " + ex.Message);
                return false;
            }

            return false;
        }

        public bool DeleteData(string id)
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand($"DELETE FROM BudgetMain WHERE ID = @ID;", con);
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
                catch(Exception ex)
                {
                    _ = MessageBox.Show("Error(BudgetMain-DeleteData):" + ex.Message);
                    return false;
                }
            }
            return false;
        }

        public bool DeleteBudget(Budget budget)
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "DELETE FROM BudgetMain WHERE ID = ?",
                };

                cmd.Parameters.AddWithValue("@ID", budget.ID);

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
                    _ = MessageBox.Show("BudgetMain:DeleteBudget:" + ex.Message);
                    return false;
                }
            }
            return true;
        }
    }

}
