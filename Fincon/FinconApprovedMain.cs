using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Collections.ObjectModel;
using System.Windows;

namespace G8FinApp.Fincon
{
    public class FinconApprovedMain: ObservableCollection<FinconApprove>
    {
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();

        public FinconApprovedMain()
        {

        }

        public void InitList()
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM FinconApprove WHERE ApproveChoice = ?",
                };

                cmd.Parameters.AddWithValue("@ApproveChoice", "Approve");

                try
                {
                    con.Open();

                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        FinconApprove finconApprove = new FinconApprove
                        {
                            MAININD = reader["MainId"].ToString(),
                            ENCUMID = reader["EncumbId"].ToString(),
                            TABLENAME = reader["TableName"].ToString(),
                            SENDTO = reader["SendTo"].ToString(),
                            APPROVECHOICE = reader["ApproveChoice"].ToString(),
                            APPDATE = DateTime.Parse(reader["AppDate"].ToString()),
                            FMNAME = reader["FmName"].ToString(),
                            REQDESC = reader["ReqDesc"].ToString(),
                            REQAMOUNT = decimal.Parse(reader["ReqAmount"].ToString()),
                            BDGTCURR = reader["BdgtCurr"].ToString(),
                            REQCURR = reader["ReqCurr"].ToString(),
                            BDGTAMOUNT = decimal.Parse(reader["BdgtAmount"].ToString()),
                        };

                        Add(finconApprove);
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("FinconApprovedMain:InitList:" + ex.Message);
                    return;
                }
            }
        }
    }
}
