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
    public class ApproveListMain: ObservableCollection<Approve>
    {
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();
        private const string curFormat = "#,#.0000";
        public ApproveListMain()
        {

        }

        public void InitList()
        {
            Approve approve;

            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM BudgetSendToQuery",
                };

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();

                    while(reader.Read())
                    {
                        approve = new Approve()
                        {
                            MAININD = reader["MainID"].ToString(),
                            ENCUMID = reader["EncumbId"].ToString(),
                            CISICODE = reader["CisiCode"].ToString(),
                            FMNO = reader["FmNo"].ToString(),
                            TABLENAME = reader["TableName"].ToString(),
                            FMNAME = reader["FmName"].ToString(),
                            REQDESC = reader["ReqDesc"].ToString(),
                            REQAMOUNT = decimal.Parse(reader["ReqAmount"].ToString()),
                            REQCURR = reader["ReqCurr"].ToString(),
                            BDGTCURR = reader["BdgtCurr"].ToString(),
                            BDGTAMOUNT = decimal.Parse(reader["BdgtAmount"].ToString()),
                            APPDATE = reader["TDate"].ToString(),
                        };

                        Add(approve);
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("ApproveListMain:InitList:" + ex.Message);
                    return;
                }
            }
        }

        public bool DeleteData(Approve approve)
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "DELETE FROM BudgetApprove WHERE MainId = ? AND EncumbId = ?",
                };

                cmd.Parameters.AddWithValue("@MainId", approve.MAININD);
                cmd.Parameters.AddWithValue("@EncumbId", approve.ENCUMID);

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
                    _ = MessageBox.Show("ApproveListMain:DeleteData:" + ex.Message);
                    return false;
                }
            }
            return true;
        }
    }
}
