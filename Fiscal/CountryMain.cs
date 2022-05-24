using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Windows;
using System.Collections.ObjectModel;


namespace G8FinApp.Fiscal
{
    public class CountryMain : ObservableCollection<Country>
    {
        Database.ProgramConsts prgrmConsts = new Database.ProgramConsts();

        public void InitList()
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConsts.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM FiscalCountries",
                };

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        Country country = new Country
                        {
                            ID = reader["ID"].ToString(),
                            CountryName = reader["CountryName"].ToString(),
                        };

                        Add(country);
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("CountryMain:InitList:" + ex.Message);
                    return;
                }
            }
        }
    }
}
