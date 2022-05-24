using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.Windows;

namespace G8FinApp.Fincon
{
    public class FinconApproveMain : ObservableCollection<FinconApprove>
    {
        private string curFormat = "#,#.0000";
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();
        public FinconApproveMain()
        {

        }

        public FinconApproveMain(bool loadInitList = false)
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

                OleDbDataReader reader = null;
                cmd.Parameters.AddWithValue("@ApproveChoice", "Empty");

                try
                {
                    con.Open();
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        FinconApprove finconApprove = new FinconApprove
                        {
                            ID = reader["ID"].ToString(),
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
                            PENDINGNO = reader["PendingNo"].ToString(),
                        };

                     Add(finconApprove);
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("Error:ApproveMain:InitList:" + ex.Message);
                    return;
                }
            }
        }

        public bool DeleteData()
        {
            
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT * FROM FinconApprove",
                };

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();

                    DataTable schemaTable = reader.GetSchemaTable();

                    for (int i = 0; i < schemaTable.Rows.Count; i++)
                    {
                        DataRow dtRow = schemaTable.Rows[i];
                        string columnName = dtRow["ColumnName"].ToString();
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("FinconApproveMain:DeleteData:" + ex.Message);
                    return false;
                }
            }

            return true;
        }

        public bool UpdateData(FinconApprove finApprove)
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand("UPDATE FinconApprove " +
                    " SET SendTo = @SendTo, ApproveChoice = @ApproveChoice, AppDate = @AppDate " +
                    "Where MainId = @MainId And EncumbId = @EncumId;", con);

                _ = cmd.Parameters.AddWithValue("@SendTo", finApprove.SENDTO);
                _ = cmd.Parameters.AddWithValue("@ApproveChoice", finApprove.APPROVECHOICE);
                _ = cmd.Parameters.AddWithValue("@AppDate", finApprove.APPDATE.ToString("d"));

                _ = cmd.Parameters.AddWithValue("@MainId", finApprove.MAININD.ToString());
                _ = cmd.Parameters.AddWithValue("@EncumbId", finApprove.ENCUMID.ToString());

                try
                {
                    con.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter
                    {
                        UpdateCommand = cmd
                    };
                    if (adapter.UpdateCommand.ExecuteNonQuery() > 0)
                    {
                        return true;
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("Error-FinconApproveMain-UpdateDate:" + ex.Message);
                    return false;
                }
            }

            return false;
        }

        public bool SaveDataFiscalWithPendingNo(FinconApprove finconApprove)
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = CommandType.Text,
                    CommandText = "INSERT INTO FiscalApprove (MainId, EncumbId, TableName, SendTo, ApproveChoice, AppDate, FmName, ReqDesc, ReqAmount, ReqCurr, BdgtCurr, BdgtAmount, PendingNo) VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
                };
                _ = cmd.Parameters.AddWithValue("@MainId", finconApprove.MAININD);
                _ = cmd.Parameters.AddWithValue("@EncumbId", finconApprove.ENCUMID);
                _ = cmd.Parameters.AddWithValue("@TableName", finconApprove.TABLENAME);

                _ = cmd.Parameters.AddWithValue("@SendTo", finconApprove.SENDTO);
                _ = cmd.Parameters.AddWithValue("@ApproveChoice", finconApprove.APPROVECHOICE);
                _ = cmd.Parameters.AddWithValue("@AppDate", finconApprove.APPDATE);

                _ = cmd.Parameters.AddWithValue("@FmName", finconApprove.FMNAME);
                _ = cmd.Parameters.AddWithValue("@ReqDesc", finconApprove.REQDESC);
                _ = cmd.Parameters.AddWithValue("@ReqAmount", finconApprove.REQAMOUNT.ToString("N4"));

                _ = cmd.Parameters.AddWithValue("@ReqCurr", finconApprove.REQCURR);

                _ = cmd.Parameters.AddWithValue("@BdgtCurr", finconApprove.BDGTCURR);
                _ = cmd.Parameters.AddWithValue("@BdgtAmount", finconApprove.BDGTAMOUNT.ToString("N4"));
                _ = cmd.Parameters.AddWithValue("@PendingNo", finconApprove.PENDINGNO);

                try
                {
                    con.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter()
                    {
                        InsertCommand = cmd,
                    };

                    if (!(adapter.InsertCommand.ExecuteNonQuery() > 0))
                    {
                        return false;
                    }

                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("FinconApproveMain:SaveDataFiscalWithPending:" + ex.Message);
                    return false;
                }
            }
            return true;
        }

        public bool SaveDataSendTo(string tblName, FinconApprove finApprove)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {

                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = CommandType.Text,
                    CommandText = "INSERT INTO " + tblName + " (MainId, EncumbId, TableName, " +
                    "SendTo, ApproveChoice, AppDate, " +
                    "FmName, ReqDesc, ReqAmount, " +
                    "ReqCurr, BdgtCurr, BdgtAmount)" +
                    " VALUES(?, ?, ?, " +
                    "?, ?, ?, " +
                    "?, ?, ?, " +
                    "?, ?, ?)",
                };

                _ = cmd.Parameters.AddWithValue("@MainId", finApprove.MAININD);
                _ = cmd.Parameters.AddWithValue("@EncumbId", finApprove.ENCUMID);
                _ = cmd.Parameters.AddWithValue("@TableName", finApprove.TABLENAME);

                _ = cmd.Parameters.AddWithValue("@SendTo", finApprove.SENDTO);
                _ = cmd.Parameters.AddWithValue("@ApproveChoice", finApprove.APPROVECHOICE);
                _ = cmd.Parameters.AddWithValue("@AppDate", finApprove.APPDATE);

                _ = cmd.Parameters.AddWithValue("@FmName", finApprove.FMNAME);
                _ = cmd.Parameters.AddWithValue("@ReqDesc", finApprove.REQDESC);
                _ = cmd.Parameters.AddWithValue("@ReqAmount", finApprove.REQAMOUNT.ToString("N4"));

                _ = cmd.Parameters.AddWithValue("@ReqCurr", finApprove.REQCURR);

                _ = cmd.Parameters.AddWithValue("@BdgtCurr", finApprove.BDGTCURR);
                _ = cmd.Parameters.AddWithValue("@BdgtAmount", finApprove.BDGTAMOUNT.ToString("N4"));

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
                catch(Exception ex)
                {
                    _ = MessageBox.Show("Error-FinconApproveMain-SaveDataSendTo:" + ex.Message);
                    return false;
                }
            }

            return false;
        }

        public bool SaveData(FinconApprove finApprove)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "INSERT INTO FinconApprove (MainId, EncumbId, TableName, SendTo, ApproveChoice, AppDate, FmName, ReqDesc, ReqAmount, ReqCurr, BdgtCurr, BdgtAmount, PendingNo) " +
                    "VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
                };

                cmd.Parameters.AddWithValue("@MainId", finApprove.MAININD);
                cmd.Parameters.AddWithValue("@EncumbId", finApprove.ENCUMID);
                cmd.Parameters.AddWithValue("@TableName", finApprove.TABLENAME);
                cmd.Parameters.AddWithValue("@SendTo", finApprove.SENDTO);
                cmd.Parameters.AddWithValue("@ApproveChoice", finApprove.APPROVECHOICE);
                cmd.Parameters.AddWithValue("@AppDate", finApprove.APPDATE);
                cmd.Parameters.AddWithValue("@FmName", finApprove.FMNAME);
                cmd.Parameters.AddWithValue("@ReqDesc", finApprove.REQDESC);
                cmd.Parameters.AddWithValue("@ReqAmount", finApprove.REQAMOUNT.ToString(curFormat));
                cmd.Parameters.AddWithValue("@ReqCurr", finApprove.REQCURR);
                cmd.Parameters.AddWithValue("@BdgtCurr", finApprove.BDGTCURR);
                cmd.Parameters.AddWithValue("@BdgtAmount", finApprove.BDGTAMOUNT.ToString(curFormat));
                cmd.Parameters.AddWithValue("@PendingNo", finApprove.PENDINGNO);

                try
                {
                    con.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter()
                    {
                        InsertCommand = cmd,
                    };

                    if (!(adapter.InsertCommand.ExecuteNonQuery() > 0))
                    {
                        return false;
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("FinconApproveMain:SaveData:" + ex.Message);
                    return false;
                }
            }
            return true;
        }

        public bool DeleteApproveFromMainList(FinconApprove finconApprove)
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "DELETE FROM FinconApprove WHERE ID = ?",
                };

                cmd.Parameters.AddWithValue("@ID", finconApprove.ID);

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
                    _ = MessageBox.Show("FinconApproveMain:DeleteApproveFromMainList:" + ex.Message);
                    return false;
                }
            }
            return true;
        }
    }
}
