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
    public class ActivityCodeMain : ObservableCollection<ActivityCode>
    {
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();

        public ActivityCodeMain()
        {
            InitList();
        }

        private void InitList()
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand com = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM ActivityCodes"
                };

                try
                {
                    con.Open();
                    OleDbDataReader reader = com.ExecuteReader();
                    while(reader.Read())
                    {
                        Add(new ActivityCode(reader[0].ToString(),
                            reader[1].ToString(),
                            reader[2].ToString()));
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("Error:ActivityCodeMain:InitList:" + ex.Message);
                    return;
                }
            }
        }

    }
}
