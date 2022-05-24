using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.OleDb;

namespace G8FinApp.Admin
{
    public class AdminContractNotification
    {
        public decimal BidAmount { get; private set; }
        public string CompanyId { get; private set; }
        public DateTime BidDate { get; private set; }
        public string BiddingId { get; private set; }
        public string ContractNo { get; private set; }

        public List<AdminContractNotification> LoadAdminContractNotifications()
        {
            string connectString = "Provider=Microsoft.Jet.Oledb.4.0; Data Source = DatabasesForFirstInit/SRD2022-01.mdb";

            List<AdminContractNotification> adminContractNotifications = null;

            using(OleDbConnection con =  new OleDbConnection(connectString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM Notifications",
                };

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        AdminContractNotification adminContractNotification = new AdminContractNotification()
                        {
                            BidAmount = decimal.Parse(reader["BidAmount"].ToString()),
                            CompanyId = reader["Bidder"].ToString(),
                            ContractNo = reader["NotificationPO"].ToString(),
                        };
                        if (adminContractNotifications is null)
                        {
                            adminContractNotifications = new List<AdminContractNotification>();
                        }

                        //MessageBox.Show("BidAmount:" + adminContractNotification.BidAmount.ToString() + Environment.NewLine +
                        //    "Company:" + adminContractNotification.CompanyId + Environment.NewLine +
                        //    "Contract No:" + adminContractNotification.ContractNo + Environment.NewLine + 
                        //    "Length of Contract No:" + adminContractNotification.ContractNo.Length);

                        adminContractNotifications.Add(adminContractNotification);
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("AdminContractNotifications:" + ex.Message);
                    return null;
                }
            }
            return adminContractNotifications;
        }
    }
}
