using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Windows;

namespace G8FinApp.Purchasing
{
    public class NotificationMain : ObservableCollection<Notification>
    {
        private const string curFormat = "##.00";
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();
        public NotificationMain()
        {

        }

        public bool InitBiddingList(string biddingId)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM PurchasingNotification WHERE BiddingId = ?",
                };

                cmd.Parameters.AddWithValue("@BiddingId", biddingId);

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        Notification notification = new Notification()
                        {
                            ID = reader["ID"].ToString(),
                            BidDate = DateTime.Parse(reader["BidDate"].ToString()),
                            BiddingId = reader["BiddingId"].ToString(),
                            BidAmount = decimal.Parse(reader["BidAmount"].ToString()),
                            CompanyId = reader["CompanyId"].ToString(),
                        };

                        Add(notification);
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("NotificationMain:InitBiddingList:" + ex.Message);
                    return false;
                }
            }
            return true;
        }

        public bool DeleteData(string biddingId)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "DELETE FROM PurchasingNotification WHERE BiddingId = ?",
                };

                cmd.Parameters.AddWithValue("@BiddingId", biddingId);

                try
                {
                    con.Open();

                    OleDbDataAdapter adapter = new OleDbDataAdapter()
                    {
                        DeleteCommand = cmd,
                    };

                    adapter.DeleteCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show("NotificationMain:DeleteData:" + ex.Message);
                    return false;
                }

                return true;
            }
        }

        public bool SaveData()
        {
            using (OleDbConnection con  = new OleDbConnection(prgrmConst.connectionString))
            {
                try
                {
                    con.Open();

                    foreach(Notification ntf in Items)
                    {
                        OleDbCommand cmd = new OleDbCommand()
                        {
                            Connection = con,
                            CommandType = System.Data.CommandType.Text,
                            CommandText = "INSERT INTO PurchasingNotification (BidDate, BiddingId, BidAmount, CompanyId) VALUES(?, ?, ?, ?)",
                        };

                        cmd.Parameters.AddWithValue("@BidDate", ntf.BidDate);
                        cmd.Parameters.AddWithValue("@BiddingId", ntf.BiddingId);
                        cmd.Parameters.AddWithValue("@BidAmount", ntf.BidAmount.ToString());
                        cmd.Parameters.AddWithValue("@CompanyId", ntf.CompanyId.ToUpper());

                        OleDbDataAdapter adapter = new OleDbDataAdapter()
                        {
                            InsertCommand = cmd,
                        };

                        adapter.InsertCommand.ExecuteNonQuery();
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("NotificationMain:SaveData:" + ex.Message);
                    return false;
                }
            }

            return true;
        }

    }
}
