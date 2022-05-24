using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Windows;

namespace G8FinApp.Budget
{
    public class CisiCodeUpdateMain
    {
        Database.ProgramConsts programConsts = new Database.ProgramConsts();

        public CisiCodeUpdateMain()
        {

        }

        public bool UpdateCisiCode(Budget budget)
        {
            using(OleDbConnection con = new OleDbConnection(programConsts.connectionString))
            {
                try
                {
                    con.Open();

                    if (!UpdateBudgetMain(con, budget))
                    {
                        return false;
                    }

                    List<string> tableNames = new List<string>()
                    {
                        "BudgetEncumbrance",
                        "BudgetIN",
                        "BudgetTransferIn",
                        "BudgetTransferOut",
                        "BudgetRevise",
                    };

                    foreach(string tbl in tableNames)
                    {
                        UpdateTables(con, tbl, budget);
                    }

                    UpdatePurchaseAprrove(con, budget);
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("CisiCodeUpdateMain:" + ex.Message);
                    return false;
                }

                return true;
            }
        }

        private bool UpdateBudgetMain(OleDbConnection con, Budget budget)
        {
            using (OleDbCommand cmd = new OleDbCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.CommandText = "UPDATE BudgetMain SET CisiCode = ?, CisiDesc = ? WHERE ID = ?";

                cmd.Parameters.AddWithValue("@CisiCode", budget.CISICODE);
                cmd.Parameters.AddWithValue("@CisiDesc", TakeCisiDesc(con, budget));
                cmd.Parameters.AddWithValue("@ID", budget.ID);

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

        private string TakeCisiDesc(OleDbConnection con, Budget budget)
        {
            using(OleDbCommand cmd = new OleDbCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "SELECT * FROM CISICodes WHERE CISINo = ?";

                cmd.Parameters.AddWithValue("@CISINo", budget.CISICODE);

                OleDbDataReader reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    return reader["CISIDesc"].ToString();
                }
            }

            return string.Empty;
        }

        private void UpdateTables( OleDbConnection con, string tbl, Budget budget)
        {
            using(OleDbCommand cmd = new OleDbCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "UPDATE " + tbl + " SET CisiCode = ? WHERE BudgetMainID = ?";

                cmd.Parameters.AddWithValue("@CisiCode", budget.CISICODE);
                cmd.Parameters.AddWithValue("@BudgetMainID", budget.ID);
                    
                OleDbDataAdapter adapter = new OleDbDataAdapter()
                {
                    UpdateCommand = cmd,
                };
                _ = adapter.UpdateCommand.ExecuteNonQuery();
            }
        }

        private void UpdatePurchaseAprrove(OleDbConnection con, Budget budget)
        {
            using (OleDbCommand cmd = new OleDbCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "UPDATE PurchasingApprove SET CisiCode = ? WHERE MainId = ?";

                cmd.Parameters.AddWithValue("@CisiCode", budget.CISICODE);
                cmd.Parameters.AddWithValue("@MainId", budget.ID);

                OleDbDataAdapter adapter = new OleDbDataAdapter()
                {
                    UpdateCommand = cmd,
                };
                _ = adapter.UpdateCommand.ExecuteNonQuery();
            }
        }
    }
}
