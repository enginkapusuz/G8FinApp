using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Collections.ObjectModel;
using System.Windows;

namespace G8FinApp.Reports
{
    public class ReportFileMain : ObservableCollection<ReportFile>
    {
        Database.ProgramConsts prgrrmConsts = new Database.ProgramConsts();

        public ReportFileMain()
        {

        }

        public void InitList()
        {
            using(OleDbConnection con = new OleDbConnection(prgrrmConsts.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM ReportMain",
                };

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ReportFile reportFile = new ReportFile()
                        {
                            ID = reader["ID"].ToString(),
                            ReportName = reader["ReportName"].ToString(),
                            ReportPath = reader["ReportPath"].ToString(),
                        };

                        Add(reportFile);
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("ReportFileMain:InitList:" + ex.Message);
                    return;
                }
            }
        }
    }
}
