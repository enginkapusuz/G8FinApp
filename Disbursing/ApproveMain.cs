using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Windows;

namespace G8FinApp.Disbursing
{
    public class ApproveMain : ObservableCollection<Approve>
    {
        private const string curFormat = "##.0000";
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();
        public ApproveMain()
        {
            InitList();
        }

        public void InitList()
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM DisbursingApprove WHERE ApproveChoice = @ApproveChoice"
                };

                cmd.Parameters.AddWithValue("@ApproveChoice", "Empty");

                try
                {
                    con.Open();

                    OleDbDataReader reader = cmd.ExecuteReader();

                    while(reader.Read())
                    {
                        Approve approve = new Approve()
                        {
                            ID = reader[0].ToString(),
                            MainId = reader[1].ToString(),
                            EncumbId = reader[2].ToString(),

                            SendTo = reader[3].ToString(),
                            ApproveChoice = reader[4].ToString(),
                            AppDate = DateTime.Parse(reader[5].ToString()),

                            ReqDesc = reader[6].ToString(),
                            InvNu = reader[7].ToString(),
                            InvAmount = decimal.Parse(reader[8].ToString()),

                            InvDate = DateTime.Parse(reader[9].ToString()),
                            InvCurr = reader[10].ToString(),
                            InvCreditor = reader[11].ToString(),

                            BdgtCurr = reader[12].ToString(),
                            BdgtAmount = decimal.Parse(reader[13].ToString()),
                    };

                        Add(approve);
                    }

                    return;
                }
                catch(Exception ex)
                {

                    Console.WriteLine("Error:ApproveMain:InitList:" + ex.Message);
                    return;
                }
            }
        }

        public bool SaveData(Approve approve)
        {
            
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "INSERT INTO DisbursingApprove (MainId, EncumbId, SendTo, ApproveChoice, AppDate, ReqDesc, InvNu, InvAmount, InvDate, InvCurr, InvCreditor, BdgtCurr, BdgtAmount)" +
                    " VALUES(@MainId, @EncumbId, @SendTo, @ApproveChoice, @AppDate, @ReqDesc, @InvNu, @InvAmount, @InvDate, @InvCurr, @InvCreditor, @BdgtCurr, @BdgtAmount)"
                };

                _ = cmd.Parameters.AddWithValue("@MainId", approve.MainId);
                _ = cmd.Parameters.AddWithValue("@EncumbId", approve.EncumbId);
                _ = cmd.Parameters.AddWithValue("@SendTo", approve.SendTo);

                _ = cmd.Parameters.AddWithValue("@ApproveChoice", approve.ApproveChoice);
                _ = cmd.Parameters.AddWithValue("@AppDate", approve.AppDate);
                _ = cmd.Parameters.AddWithValue("@ReqDesc", approve.ReqDesc);

                _ = cmd.Parameters.AddWithValue("@InvNu", approve.InvNu.ToString());
                _ = cmd.Parameters.AddWithValue("@InvAmount", approve.InvAmount.ToString());
                _ = cmd.Parameters.AddWithValue("@InvDate", approve.InvDate);

                _ = cmd.Parameters.AddWithValue("@InvCurr", approve.InvCurr);
                _ = cmd.Parameters.AddWithValue("@InvCreditor", approve.InvCreditor);

                _ = cmd.Parameters.AddWithValue("@BdgtCurr", approve.BdgtCurr);
                _ = cmd.Parameters.AddWithValue("@BdgtAmount", approve.BdgtAmount.ToString(curFormat));

                try
                {
                    con.Open();

                    if (!string.IsNullOrEmpty(approve.ID))
                    {
                        if(!UpdateData(approve, con))
                        {
                            return false;
                        }
                    }
                    
                    OleDbDataAdapter adapter = new OleDbDataAdapter()
                    {
                        InsertCommand = cmd
                    };

                    if (adapter.InsertCommand.ExecuteNonQuery() > 0)
                    {
                        return true;
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("Error:ApproveMain:SaveData:" + ex.Message);
                    return false;
                }
            }

            return false;
        }

        private bool UpdateData(Approve approve, OleDbConnection con)
        {
            try
            {
                using (OleDbCommand cmd = new OleDbCommand(cmdText: "UPDATE DisbursingApprove SET ApproveChoice = @ApproveChoice WHERE ID = @ID", connection: con))
                {
                    _ = cmd.Parameters.AddWithValue("@ApproveChoice", approve.ApproveChoice);
                    _ = cmd.Parameters.AddWithValue("@ID", approve.ID);

                    OleDbDataAdapter adapter = new OleDbDataAdapter()
                    {
                        UpdateCommand = cmd,
                    };

                    if (adapter.UpdateCommand.ExecuteNonQuery() > 0)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show("Error:AccountMain:UpdateData:" + ex.Message);
                return false;
            }

            return false;
        }
    }
}