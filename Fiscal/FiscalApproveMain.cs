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
    public class FiscalApproveMain : ObservableCollection<FiscalApprove>
    {
        private const string curFormat = "##.0000";
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();

        Purchasing.ApproveMain purchasingApproveMain;

        public FiscalApproveMain()
        {

        }

        public void InitList()
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM FiscalApproveQuery",
                };

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();

                    int _lastComId = 0;

                    while (reader.Read())
                    {
                        if (int.TryParse(reader["LastComId"].ToString(), out int lastComId))
                        {
                            _lastComId = lastComId;
                        }

                        FiscalApprove fiscalapprove = new FiscalApprove()
                        {
                            ID = reader["ID"].ToString(),
                            MAININD = reader["MainId"].ToString(),
                            ENCUMID = reader["EncumbId"].ToString(),
                            TABLENAME = reader["TableName"].ToString(),
                            SENDTO = reader["SendTo"].ToString(),
                            APPROVECHOICE = reader["ApproveChoice"].ToString(),
                            APPDATE = DateTime.Parse(reader["AppDate"].ToString()),
                            FMNO = reader["FmNo"].ToString(),
                            FMNAME = reader["FmName"].ToString(),
                            CISICODE = reader["CisiCode"].ToString(),
                            CISIDESC = reader["CISIDesc"].ToString(),
                            REQDESC = reader["ReqDesc"].ToString(),
                            REQAMOUNT = decimal.Parse(reader["ReqAmount"].ToString()),
                            REQCURR = reader["ReqCurr"].ToString(),
                            BDGTCURR = reader["BdgtCurr"].ToString(),
                            BDGTAMOUNT = decimal.Parse(reader["BdgtAmount"].ToString()),
                            LASTCOMID = _lastComId,
                            PENDINGNO = reader["PendingNo"].ToString(),
                        };

                        Add(fiscalapprove);
                    }
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show("Error:FiscalApproveMain:InitList:" + ex.Message);
                    return;
                }
            }
        }

        public bool SaveData(FiscalApprove fiscalApprove)
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "INSERT INTO FiscalApprove (MainId, EncumbId, TableName, SendTo, ApproveChoice, AppDate, FmName, ReqDesc,ReqAmount, ReqCurr, BdgtCurr, BdgtAmount) " +
                    "VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
                };

                _ = cmd.Parameters.AddWithValue("@MainId", fiscalApprove.MAININD.Trim());
                _ = cmd.Parameters.AddWithValue("@EncumbId", fiscalApprove.ENCUMID.Trim());
                _ = cmd.Parameters.AddWithValue("@TableName", fiscalApprove.TABLENAME.Trim());

                _ = cmd.Parameters.AddWithValue("@SendTo", fiscalApprove.SENDTO.Trim());
                _ = cmd.Parameters.AddWithValue("@ApproveChoice", fiscalApprove.APPROVECHOICE.Trim());
                _ = cmd.Parameters.AddWithValue("@AppDate", fiscalApprove.APPDATE);

                _ = cmd.Parameters.AddWithValue("@FmName", fiscalApprove.FMNAME.Trim());
                _ = cmd.Parameters.AddWithValue("@ReqDesc", fiscalApprove.REQDESC.Trim());
                _ = cmd.Parameters.AddWithValue("@ReqAmount", fiscalApprove.REQAMOUNT.ToString("N4"));

                _ = cmd.Parameters.AddWithValue("@ReqCurr", fiscalApprove.REQCURR.Trim());
                
                _ = cmd.Parameters.AddWithValue("@BdgtCurr", fiscalApprove.BDGTCURR.Trim());
                _ = cmd.Parameters.AddWithValue("BdgtAmount", fiscalApprove.BDGTAMOUNT.ToString(curFormat));

                try
                {
                    con.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter()
                    {
                        InsertCommand = cmd,
                    };

                    if (adapter.InsertCommand.ExecuteNonQuery() > 0)
                    {
                        return true;
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("Error:FiscalApproveMain:SaveData:" + ex.Message);
                    return false;
                }
            }

            return false;
        }
        public bool UpdateDataPurchasing(Pending pendingApprove)
        {
            purchasingApproveMain = new Purchasing.ApproveMain();
            if (!purchasingApproveMain.SaveData(pendingApprove))
            {
                return false;
            }
            
            return true;
        }
        public bool UpdateData(string approveID, string approveChoice)
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "UPDATE FiscalApprove SET ApproveChoice = @ApproveChoice WHERE ID = @ID"
                };

                _ = cmd.Parameters.AddWithValue("@ApproveChoice", approveChoice);
                _ = cmd.Parameters.AddWithValue("@ID", approveID);

                try
                {
                    con.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter()
                    {
                        UpdateCommand = cmd
                    };
                    if (!(adapter.UpdateCommand.ExecuteNonQuery() > 0))
                    {
                        return false;
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("Error:FiscalApproveMain:UpdateData:" + ex.Message);
                    return false;
                }
            }
            return true;
        }

        public bool UpdateDataPending(Pending pending)
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "UPDATE FiscalApprove SET ApproveChoice = ?, PendingNo = ? WHERE ID = @ID",
                };

                cmd.Parameters.AddWithValue("@ApproveChoice", pending.APPROVECHOICE);
                cmd.Parameters.AddWithValue("@pendingNo", pending.PENDINGNO);
                cmd.Parameters.AddWithValue("@ID", pending.APPROVEID);

                try
                {
                    con.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter()
                    {
                        UpdateCommand = cmd,
                    };

                    if (!(adapter.UpdateCommand.ExecuteNonQuery() > 0))
                    {
                        return false;
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("FiscalApproveMain:UpdateDataPending:" + ex.Message);
                    return false;
                }
            }
            return true;
        }

        public bool GetFiscalApproveData(string commitId)
        {
            FiscalApprove fiscalApprove;

            string mainId = null;
            string encumbId = null;

            string fmName = null;
            string reqDesc = null;
            decimal reqAmount = 0;

            string reqCurr = null;

            string bdgtCurr = null;
            decimal bdgtAmount = 0;

            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT FmName, ReqDesc, ReqAmount, ReqCurr, BdgtCurr, BdgtAmount FROM FiscalApprove WHERE MainId = @MainId AND EncumbId = @EncumbId",
                };

                try
                {
                    con.Open();

                    if (!GetMainAndEncumbId(commitId, ref mainId, ref encumbId, con))
                    {
                        return false;
                    }

                    _ = cmd.Parameters.AddWithValue("@MainId", mainId);
                    _ = cmd.Parameters.AddWithValue("EncumbId", encumbId);

                    OleDbDataReader reader = cmd.ExecuteReader();

                    while(reader.Read())
                    {
                        fmName = reader[0].ToString();
                        reqDesc = reader[1].ToString();
                        reqAmount = decimal.Parse(reader[2].ToString());

                        reqCurr = reader[3].ToString();

                        bdgtCurr = reader[4].ToString();
                        bdgtAmount = decimal.Parse(reader[5].ToString());
                    }

                    if (string.IsNullOrEmpty(fmName))
                    {
                        return false;
                    }

                    fiscalApprove = new FiscalApprove();

                    fiscalApprove.MAININD = mainId;
                    fiscalApprove.ENCUMID = encumbId;
                    fiscalApprove.TABLENAME = "BudgetEncumbrance";

                    fiscalApprove.SENDTO = "Disbursing";
                    fiscalApprove.APPROVECHOICE = "Committed";
                    fiscalApprove.APPDATE = DateTime.Parse(DateTime.Now.ToString("d"));

                    fiscalApprove.FMNAME = fmName;
                    fiscalApprove.REQDESC = reqDesc;
                    fiscalApprove.REQAMOUNT = reqAmount;

                    fiscalApprove.REQCURR = reqCurr;

                    fiscalApprove.BDGTCURR = bdgtCurr;
                    fiscalApprove.BDGTAMOUNT = bdgtAmount;

                    if (!SaveData(fiscalApprove))
                    {
                        return false;
                    }

                    return true;
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("Error:FiscalApproveMain:GetFiscalApproveMain:" + ex.Message);
                    return false;
                }
            }
        }

        private bool GetMainAndEncumbId(string commitId, ref string mainId, ref string encumbId, OleDbConnection con)
        {
            
            OleDbCommand cmd = new OleDbCommand()
            {
                Connection = con,
                CommandType = System.Data.CommandType.Text,
                CommandText = "SELECT ID, MainId, EncumbId FROM FiscalCommit WHERE ID = @ID",
            };

            _ = cmd.Parameters.AddWithValue("@ID", commitId);

            try
            {
                OleDbDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    mainId = reader[1].ToString();
                    encumbId = reader[2].ToString();
                }

                if (string.IsNullOrEmpty(mainId))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch(Exception ex)
            {
                _ = MessageBox.Show("Error:FiscalApproveMain:GetMainAndEncumbId:" + ex.Message);
                return false;
            }
        }

        public bool UpdateAndDeleteFiscalApprove(CommitApprove commitApprove)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                try
                {
                    con.Open();

                    bool isFiscalApproveDelete = DeleteFiscalApprove(con, commitApprove);
                    if (!isFiscalApproveDelete)
                    {
                        return false;
                    }

                    bool isFiscalApproveUpdate = UpdateFiscalApprove(con, commitApprove);
                    if (!isFiscalApproveUpdate)
                    {
                        return false;
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("FiscalApproveMain:UpdateAndDeleteFiscalApprove:" + ex.Message);
                    return false;
                }
            }

            return true;
        }

        private bool DeleteFiscalApprove( OleDbConnection con, CommitApprove commitApprove)
        {
            using(OleDbCommand cmd = new OleDbCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "DELETE FROM FiscalApprove WHERE MainId = ? AND EncumbId = ? AND SendTo = ?";

                cmd.Parameters.AddWithValue("@MainId", commitApprove.MainId);
                cmd.Parameters.AddWithValue("@EncumbId", commitApprove.EncumbId);
                cmd.Parameters.AddWithValue("SendTo", "Fiscal");

                OleDbDataAdapter adapter = new OleDbDataAdapter()
                {
                    DeleteCommand = cmd,
                };

                if (!(adapter.DeleteCommand.ExecuteNonQuery() > 0))
                {
                    return false;
                }
                return true;
            }
        }

        private bool UpdateFiscalApprove(OleDbConnection con, CommitApprove commitApprove)
        {
            using(OleDbCommand cmd = new OleDbCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "UPDATE FiscalApprove SET ApproveChoice = ? WHERE MainId = ? AND EncumbId = ?";

                cmd.Parameters.AddWithValue("@ApproveChoice", "Empty");
                cmd.Parameters.AddWithValue("@MainId", commitApprove.MainId);
                cmd.Parameters.AddWithValue("@EncumbId", commitApprove.EncumbId);

                OleDbDataAdapter adapter = new OleDbDataAdapter()
                {
                    UpdateCommand = cmd,
                };

                if (!(adapter.UpdateCommand.ExecuteNonQuery() > 0))
                {
                    return false;
                }
                return true;
            }
        }

        public bool InsertNewFiscalApprove(Purchasing.Approve approve)
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "INSERT INTO FiscalApprove (MainId, EncumbId, TableName, SendTo, ApproveChoice, AppDate, FmName, ReqDesc, ReqAmount, ReqCurr, BdgtCurr, BdgtAmount, PendingNo) " +
                    "VALUES(?, ?, ?,  ?, ?, ?,  ?, ?, ?,  ?, ?, ?, ?)",
                };

                cmd.Parameters.AddWithValue("@MainId", approve.MainId);
                cmd.Parameters.AddWithValue("@EncumbId", approve.EncumbId);
                cmd.Parameters.AddWithValue("TableName", approve.TableName);
                cmd.Parameters.AddWithValue("@SendTo", approve.SendTo);
                cmd.Parameters.AddWithValue("@ApproveChoice", approve.ApproveChoice);
                cmd.Parameters.AddWithValue("@AppDate", approve.AppDate.ToString("d"));
                cmd.Parameters.AddWithValue("@FmName", approve.FmName);
                cmd.Parameters.AddWithValue("@ReqDesc", approve.ReqDesc);
                cmd.Parameters.AddWithValue("@ReqAmount", approve.ReqAmount.ToString());
                cmd.Parameters.AddWithValue("@ReqCurr", approve.ReqCurr);
                cmd.Parameters.AddWithValue("@BdgtCurr", approve.BdgtCurr);
                cmd.Parameters.AddWithValue("@BdgtAmount", approve.BdgtAmount.ToString());
                cmd.Parameters.AddWithValue("@PendingNo", approve.PendingNo);

                try
                {
                    con.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter()
                    {
                        InsertCommand = cmd,
                    };

                    if(!(adapter.InsertCommand.ExecuteNonQuery() > 0))
                    {
                        return false;
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("FiscalApproveMain:InsertNewFiscalApprove:" + ex.Message);
                    return false;
                }
            }
            return true;
        }
    }
}
