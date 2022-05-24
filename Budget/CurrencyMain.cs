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
    public class CurrencyMain : ObservableCollection<Currency>
    {
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();

        public CurrencyMain()
        {
            InitList();
        }

        public void InitList()
        {
            string tblName = "BudgetCurrencies";

            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "SELECT * FROM " + tblName + " ORDER BY ID;";
                cmd.Connection = con;

                try
                {
                    con.Open();

                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Add(new Currency(reader[0].ToString(),
                            reader[1].ToString(),
                            reader[2].ToString(),
                            reader[3].ToString()));
                    }

                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show("Error:" + ex.Message);
                    return;
                }
            }
        }
    }

}
