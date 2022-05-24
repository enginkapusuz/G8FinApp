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
    public class BudgetDataMain : ObservableCollection<BudgetData>
    {
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();
        public BudgetDataMain()
        {

        }

        public void InitList()
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand("SELECT * FROM BudgetEncmbData", con);

                try
                { 
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Add(new BudgetData(reader[0].ToString(),
                            reader[1].ToString(),
                            reader[2].ToString(),
                            reader[3].ToString(),
                            reader[4].ToString(),
                            decimal.Parse(reader[5].ToString()),
                            reader[6].ToString(),
                            reader[7].ToString()));
                    }
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show("Error:(BudgetDataMain-InitList):" + ex.Message);
                    return;
                }
            }
        }

        public bool SaveData(BudgetData bdgtData, ref string lstId)
        { 
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand("INSERT INTO BudgetEncmbData (ReqDesc, ReqNum, ReqItemCount, ReqCurr, ReqAmount, ReqDate, DocNu)" +
                    " VALUES(@ReqDesc, @ReqNum, @ReqItemCount, @ReqCurr, @ReqAmont, @ReqDate, @DocNu)", con);


                //cmd.Parameters.AddWithValue("@ID", bdgtData.ID);
                cmd.Parameters.AddWithValue("@ReqDesc", bdgtData.REQDESC);
                cmd.Parameters.AddWithValue("@ReqNum", bdgtData.REQNUM);
                cmd.Parameters.AddWithValue("@ReqItemCount", bdgtData.REQITEMCOUNT);
                cmd.Parameters.AddWithValue("@ReqCurr", bdgtData.REQCURR);
                cmd.Parameters.AddWithValue("@ReqAmount", bdgtData.REQAMOUNT.ToString());
                cmd.Parameters.AddWithValue("@ReqDate", bdgtData.REQDATE);
                cmd.Parameters.AddWithValue("@DocNu", bdgtData.REQDOCNU);

                try
                {
                    con.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter();
                    adapter.InsertCommand = cmd;
                    if (adapter.InsertCommand.ExecuteNonQuery() > 0)
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = "SELECT @@IDENTITY";

                        adapter.SelectCommand = cmd;
                        lstId = adapter.SelectCommand.ExecuteScalar().ToString();
                        return true;
                    }

                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("Error(BudgetDataMain-SaveData):" + ex.Message);
                    return false;
                }

                return false;
            }
        }

        public bool DeleteData(string bdgtEncmbDocNu)
        {
            int bdgtEncumId;

            if (!bdgtEncmbDocNu.Contains("BudgetEncumbrance"))
            {
                return false;
            }
            else
            {
                bdgtEncumId = int.Parse(bdgtEncmbDocNu.Split('/')[1].Split(':')[1]);
                if (!(MessageBox.Show("Id:" + bdgtEncumId, "Test", MessageBoxButton.YesNo) == MessageBoxResult.Yes))
                {
                    return false;
                }
            }
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand("DELETE FROM BudgetEncmbData WHERE ID = @ID", con);
                cmd.CommandType = System.Data.CommandType.Text;
                _ = cmd.Parameters.AddWithValue("@ID", bdgtEncumId.ToString());

                try
                {
                    con.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter();
                    adapter.DeleteCommand = cmd;
                    int dltCount = adapter.DeleteCommand.ExecuteNonQuery();
                    _ = MessageBox.Show(dltCount.ToString());
                    if ( dltCount > 0)
                    {
                        return true;
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("Error:BudgetDataMain-DeleteData:" + ex.Message);
                    return false;
                }
            }
            return false;
        }

        public bool DeleteBudgetData(BudgetData budgetData)
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "DELETE FROM BudgetEncmbData WHERE ID = ? ",
                };

                cmd.Parameters.AddWithValue("@ID", budgetData.ID);

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
                    _ = MessageBox.Show("BudgetDataMain:DeleteBudgetData:" + ex.Message);
                    return false;
                }
            }

            return true;
        }
    }
}
