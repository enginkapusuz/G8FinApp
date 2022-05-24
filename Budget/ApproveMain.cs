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
    public class ApproveMain : ObservableCollection<Approve>
    {
        private const string curFormat = "N4";
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();
        public ApproveMain()
        {

        }

        public void InitList()
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand("SELECT * FROM BudgetApprove", con)
                {
                    CommandType = System.Data.CommandType.Text
                };
                
                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Add(new Approve(reader[1].ToString(),
                            reader[2].ToString(),
                            reader[3].ToString(),
                            reader[4].ToString(),
                            reader[5].ToString(),
                            reader[6].ToString()));
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("Error:ApproveMain:InitList:" + ex.Message);
                    return;
                }
            }
        }
        private bool IsDataExist(Approve approve, string tblName, OleDbConnection con)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT MainId, EncumbId FROM " + tblName + " WHERE MainId = @MainId AND EncumbId = @EncumbId;", con);
            _ = cmd.Parameters.AddWithValue("@MainId", approve.MAININD);
            _ = cmd.Parameters.AddWithValue("@EncumbId", approve.ENCUMID);

            try
            {
                if (cmd.ExecuteScalar() != null)
                {
                    return true;
                }
            }
            catch(Exception ex)
            {
                _ = MessageBox.Show("Error-ApproveMain-IsDataExist:" + ex.Message);
                throw new ArgumentNullException(nameof(cmd));
            }

            return false;
        }

        public bool SaveSendToData(string tblName, Approve approve)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand("INSERT INTO " + tblName +
                    " (MainId, EncumbId, TableName, SendTo, ApproveChoice, AppDate, FmName, ReqDesc, ReqAmount, ReqCurr, BdgtCurr, BdgtAmount)" +
                    " VALUES(@MainId, @EncumbId, @TableName, @SendTo, @ApproveChoice, @AppDate, @FmName, @ReqDesc, @ReqAmount, @ReqCurr, @BdgtCurr, @BdgtAmount);", con);

                _ = cmd.CommandType = System.Data.CommandType.Text;

                _ = cmd.Parameters.AddWithValue("@MainId", approve.MAININD);
                _ = cmd.Parameters.AddWithValue("EncumbId", approve.ENCUMID);
                _ = cmd.Parameters.AddWithValue("@TableName", approve.TABLENAME);
                _ = cmd.Parameters.AddWithValue("@SendTo", approve.SENDTO);
                _ = cmd.Parameters.AddWithValue("@ApproveChoice", approve.APPROVECHOICE);
                _ = cmd.Parameters.AddWithValue("@AppDate", approve.APPDATE);

                _ = cmd.Parameters.AddWithValue("@FmName", approve.FMNAME);
                _ = cmd.Parameters.AddWithValue("@ReqDesc", approve.REQDESC);
                _ = cmd.Parameters.AddWithValue("@ReqAmount", approve.REQAMOUNT.ToString(curFormat));
                _ = cmd.Parameters.AddWithValue("@ReqCurr", approve.REQCURR);

                _ = cmd.Parameters.AddWithValue("@BdgtCurr", approve.BDGTCURR);
                _ = cmd.Parameters.AddWithValue("@BdgtAmount", approve.BDGTAMOUNT.ToString(curFormat));

                try
                {
                    con.Open();

                    OleDbDataAdapter adapter = new OleDbDataAdapter();
                    _ = adapter.InsertCommand = cmd;
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

        public bool SaveData(string tblName, Approve approve)
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
              
                OleDbCommand cmd = new OleDbCommand("INSERT INTO "+ tblName  + 
                    " (MainId, EncumbId, TableName, SendTo, ApproveChoice, AppDate)" +
                    " VALUES(@MainId, @EncumbId, @TableName, @SendTo, @ApproveChoice, @AppDate);", con);

                _ = cmd.CommandType = System.Data.CommandType.Text;

                _ = cmd.Parameters.AddWithValue("@MainId", approve.MAININD);
                _ = cmd.Parameters.AddWithValue("EncumbId", approve.ENCUMID);
                _ = cmd.Parameters.AddWithValue("@TableName", approve.TABLENAME);
                _ = cmd.Parameters.AddWithValue("@SendTo", approve.SENDTO);
                _ = cmd.Parameters.AddWithValue("@ApproveChoice", approve.APPROVECHOICE);
                _ = cmd.Parameters.AddWithValue("@AppDate", approve.APPDATE);

                try
                {
                    con.Open();

                    if (IsDataExist(approve, tblName, con))
                    {
                        return false;
                    }
                    
                    OleDbDataAdapter adapter = new OleDbDataAdapter();
                    _ = adapter.InsertCommand = cmd;
                    if (adapter.InsertCommand.ExecuteNonQuery() > 0)
                    {
                        return true;
                    }
                }
                catch(ArgumentNullException ex)
                {
                    _ = MessageBox.Show("Null exception!" + ex.Message);
                    return false;
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("Error:ApproveMain:SaveData:" + ex.Message);
                    return false;
                }
            }

            return false;
        }

    }
}
