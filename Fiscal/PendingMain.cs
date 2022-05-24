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
    public class PendingMain : ObservableCollection<Pending>
    {
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();

        public PendingMain()
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
                    CommandText = "SELECT * FROM FiscalPendingList",
                };

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        Pending pending = new Pending()
                        {

                            PENDINGNO = reader["PendingNu"].ToString(),
                            REQDESC = reader["ReqDesc"].ToString(),
                            REQAMOUNT = decimal.Parse(reader["ReqAmount"].ToString()),
                            REQCURR = reader["ReqCurr"].ToString(),
                        };

                        Add(pending);
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("PendingMain:InitList:" + ex.Message);
                    return;
                }
            }
        }

        private string TakeLastIdN(OleDbConnection con)
        {
            string tmpIdNu = string.Empty;

            try
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT Count(*) FROM FiscalPendingCancelQuery"

                };

                OleDbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tmpIdNu = reader[0].ToString();
                }

                return tmpIdNu != string.Empty ? tmpIdNu : "0";
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show("Error:PendingMain:TakeLastIdNu" + ex.Message);
                return string.Empty;
            }
        }

        public bool InitPendingData(ref Pending pending)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT ID, FmNo, CisiCode FROM BudgetEncumbrance WHERE ID = @ID",
                };

                _ = cmd.Parameters.AddWithValue("@ID", pending.ENCUMID);

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader[0].ToString() == pending.ENCUMID)
                        {
                            pending.ID = TakeLastIdN(con);
                            pending.FMNO = reader[1].ToString();
                            pending.CISICODE = reader[2].ToString();
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show("PendingMain-InitCommitData:" + ex.Message);
                    return false;
                }
            }

            return false;
        }

        public bool SaveData(Pending pending)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "INSERT INTO FiscalPending (MainId, EncumbId, PendingNu, PendingDate, PendingAmount, ActCode) " +
                    "VALUES(@MainId, @EncumbId, @PendingNu, @PendingDate, @PendingAmount, @ActCode)"
                };

                _ = cmd.Parameters.AddWithValue("@MainId", pending.MAINID.Trim());
                _ = cmd.Parameters.AddWithValue("@EncumbId", pending.ENCUMID.Trim());
                _ = cmd.Parameters.AddWithValue("@PendingNu", pending.PENDINGNO.Trim());

                _ = cmd.Parameters.AddWithValue("@PendingDate", pending.APPDATE.ToString());
                _ = cmd.Parameters.AddWithValue("@PendingAmount", pending.REQAMOUNT.ToString("N4"));
                _ = cmd.Parameters.AddWithValue("@ActCode", pending.ACTCODE.Trim());

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
                catch (Exception ex)
                {
                    _ = MessageBox.Show("Error:PendingMain:SaveData" + ex.Message);
                    return false;
                }
            }
            return false;
        }

        public bool PendingCancel(Pending pending)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "INSERT INTO FiscalPendingCancel (MainId, EncumbId, PendingNu, PendingDate, PendingAmount) VALUES(?, ?, ?, ?, ?)"
                };
                cmd.Parameters.AddWithValue("@MainId", pending.MAINID);
                cmd.Parameters.AddWithValue("@EncumbId", pending.ENCUMID);
                cmd.Parameters.AddWithValue("@PendingNu", pending.PENDINGNO);
                cmd.Parameters.AddWithValue("@PendingDate", pending.APPDATE.ToString("d"));
                cmd.Parameters.AddWithValue("@PendingAmount", pending.REQAMOUNT.ToString());

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

                    adapter = null;

                    cmd.CommandText = "DELETE FROM FiscalPending WHERE PendingNu = ?";
                    adapter = new OleDbDataAdapter()
                    {
                        DeleteCommand = cmd,
                    };

                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@PendingNu", pending.PENDINGNO);

                    if (!(adapter.DeleteCommand.ExecuteNonQuery() > 0))
                    {
                        return false;
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("PendingMain:PendingCancel:" + ex.Message);
                    return false;
                }
            }
            return true;
        }
    }
}
