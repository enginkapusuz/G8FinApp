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
    public class BiddingMain : ObservableCollection<Bidding>
    {
        private const string curFormat = "#,#.0000";
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();

        public BiddingMain()
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
                    CommandText = "SELECT * FROM PurchasingBiddingQuery",
                };

                try
                {
                    con.Open();

                    OleDbDataReader reader = cmd.ExecuteReader();

                    while(reader.Read())
                    {
                        Bidding bidding = new Bidding()
                        {
                            ID = reader["PB.ID"].ToString(),
                            ApproveId = reader["ApproveId"].ToString(),
                            BiddingName = reader["BiddingName"].ToString(),
                            Lines = reader["Lines"].ToString(),

                            BiddingOpenDate = DateTime.Parse(reader["BiddingOpenDate"].ToString()),
                            BiddingCloseDate = DateTime.Parse(reader["BiddingCloseDate"].ToString()),

                            PointOfContact = reader["PointOfContact"].ToString(),
                            PofConPhone = reader["PofConPhone"].ToString(),

                            BiddingPrice = decimal.Parse(reader["BiddingPrice"].ToString()),
                            BiddingCurr = reader["BiddingCurr"].ToString(),

                            FMNo = reader["FmNo"].ToString(),
                            LastContractNu = int.Parse(reader["LastContractNu"].ToString()),

                            CisiCode = reader["CisiCode"].ToString(),
                            PendingNo = reader["PendingNo"].ToString(),
                        };

                        Add(bidding);
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("BiddingMain:InitList:" + ex.Message);
                    return;
                }
            }
        }

        public bool SaveData(Bidding bidding)
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "INSERT INTO PurchasingBidding (ApproveId, BiddingName, Lines, BiddingOpenDate," +
                    "BiddingCloseDate, BiddingPrice, BiddingCurr, PointOfContact, PofConPhone) " +
                    "VALUES(?, ?, ?," +
                    "?, ?, ?," +
                    "?, ?, ?)",
                };

                cmd.Parameters.AddWithValue("@ApproveId", bidding.ApproveId);
                cmd.Parameters.AddWithValue("@BiddingName", bidding.BiddingName.ToUpper());
                cmd.Parameters.AddWithValue("@Lines", bidding.Lines);

                cmd.Parameters.AddWithValue("@BiddingOpenDate", bidding.BiddingOpenDate);
                cmd.Parameters.AddWithValue("@BiddingCloseDate", bidding.BiddingCloseDate);

                cmd.Parameters.AddWithValue("@BiddingPrice", bidding.BiddingPrice.ToString(curFormat));
                cmd.Parameters.AddWithValue("@BiddingCurr", bidding.BiddingCurr);
                cmd.Parameters.AddWithValue("@PointOfContact", bidding.PointOfContact);

                cmd.Parameters.AddWithValue("@PofConPhone", bidding.PofConPhone);

                try
                {
                    con.Open();

                    OleDbDataAdapter adapter = new OleDbDataAdapter()
                    {
                        InsertCommand = cmd,
                    };

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        return true;
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("BiddingMain:SaveData:" + ex.Message);
                    return false;
                }
            }

            return true;
        }

        public bool DeleteBidding(Bidding bidding)
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "DELETE FROM PurchasingBidding WHERE ID = ?",
                };

                cmd.Parameters.AddWithValue("@ID", bidding.ID);

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
                    _ = MessageBox.Show("BiddingMain:DeleteBidding:" + ex.Message);
                    return false;
                }
            }

            return true;
        }
    }
}
