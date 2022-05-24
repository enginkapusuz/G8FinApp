using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Windows;
using G8FinApp.Fiscal;

namespace G8FinApp.Purchasing
{
    
    public class ApproveMain: ObservableCollection<Approve>
    {
        private const string curFormat = "##.00";
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();

        public ApproveMain()
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
                    CommandText = "SELECT * FROM PurchasingApprove WHERE ApproveChoice = ?"
                    //CommandText = "SELECT * FROM PurchasingApprove"
                };

                cmd.Parameters.AddWithValue("@ApproveChoice", "Empty");

                try
                {
                    con.Open();

                    OleDbDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Approve appove = new Approve()
                        {
                            ID = reader[0].ToString(),
                            MainId = reader[1].ToString(),
                            EncumbId = reader[2].ToString(),

                            SendTo = reader[3].ToString(),
                            ApproveChoice = reader[4].ToString(),
                            AppDate = DateTime.Parse(reader[5].ToString()),

                            CisiCode = reader[6].ToString(),
                            FmName = reader[7].ToString(),
                            ReqDesc = reader[8].ToString(),

                            ReqAmount = decimal.Parse(reader[9].ToString()),
                            ReqCurr = reader[10].ToString(),
                            BdgtCurr = reader[11].ToString(),

                            BdgtAmount = decimal.Parse(reader[12].ToString()),
                            PendingNo = reader[13].ToString(),
                        };

                        Add(appove);
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Error:ApproveMain:InitList:" + ex.Message);
                    return;
                }
            }
        }

        public bool SaveData(Pending pending)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "INSERT INTO PurchasingApprove (MainId, EncumbId, SendTo, " +
                    "ApproveChoice, AppDate, CisiCode, " +
                    "FmName, ReqDesc, ReqAmount, " +
                    "ReqCurr, BdgtCurr, BdgtAmount, " +
                    "PendingNo)" +
                    " VALUES(?, ?, ?," +
                    "?, ?, ?," +
                    "?, ?, ?," +
                    "?, ?, ?," +
                    "?)"
                };

                cmd.Parameters.AddWithValue("@MainId", pending.MAINID);
                cmd.Parameters.AddWithValue("@EncumbId", pending.ENCUMID);
                cmd.Parameters.AddWithValue("@SendTo", pending.SENDTO);

                cmd.Parameters.AddWithValue("@ApproveChoice", pending.APPROVECHOICE);
                cmd.Parameters.AddWithValue("AppDate", pending.APPDATE);
                cmd.Parameters.AddWithValue("CisiCode", pending.CISICODE);

                cmd.Parameters.AddWithValue("@FmName", pending.FMNAME);
                cmd.Parameters.AddWithValue("@ReqDesc", pending.REQDESC);
                cmd.Parameters.AddWithValue("@ReqAmount", pending.REQAMOUNT.ToString());

                cmd.Parameters.AddWithValue("@ReqCurr", pending.REQCURR);
                cmd.Parameters.AddWithValue("@BdgtCurr", pending.BDGTCURR);
                cmd.Parameters.AddWithValue("@BdgtAmount", pending.BDGTAMOUNT.ToString());

                cmd.Parameters.AddWithValue("PendingNo", pending.PENDINGNO);

                try
                {
                    con.Open();

                    OleDbDataAdapter adapter = new OleDbDataAdapter()
                    {
                        InsertCommand = cmd
                    };

                    if (adapter.InsertCommand.ExecuteNonQuery() > 0)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show("Error:ApproveMain:SaveData:" + ex.Message);
                    return false;
                }
            }

            return false;
        }

        public bool UpdateData(Pending pending)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "UPDATE PurchasingApprove SET ApproveChoice = ? WHERE ID = ?",
                };

                _ = cmd.Parameters.AddWithValue("@ApproveChoice", pending.APPROVECHOICE);
                _ = cmd.Parameters.AddWithValue("@ID", pending.ID);

                try
                {
                    con.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter()
                    {
                        UpdateCommand = cmd,
                    };

                    if (adapter.UpdateCommand.ExecuteNonQuery() > 0)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show("Error:AccountMain:UpdateData:" + ex.Message);
                    return false;
                }
            }

            return false;
        }

        public bool UpdateApprove(Approve approve)
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "UPDATE PurchasingApprove SET ApproveChoice = ? WHERE ID = ?",
                };
                cmd.Parameters.AddWithValue("@ApproveChoice", approve.ApproveChoice);
                cmd.Parameters.AddWithValue("@ID", approve.ID);

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
                    _ = MessageBox.Show("ApproveMain:UpdateApprove:" + ex.Message);
                    return false;
                }
            }
            return true;
        }
    }
}
