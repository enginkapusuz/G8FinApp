using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.OleDb;

namespace G8FinApp.Admin
{
    public class AdminPurchaseItem
    {
        public string ContractNo { get; private set; }
        public string ItemDesc { get; private set; }
        public double ItemQuantity { get; private set; }
        public string ItemUnit { get; private set; }
        public double ItemUnitPrice { get; private set; }
        public string CommitNu { get; private set;}
        public string PendingNu { get; private set;  }

        public List<AdminPurchaseItem> LoadAdminPurhaseItems()
        {
            string connectString = "Provider=Microsoft.Jet.Oledb.4.0; Data Source = DatabasesForFirstInit/SRD2022-01.mdb";

            List<AdminPurchaseItem> adminPurchaseItems = null;

            using (OleDbConnection con = new OleDbConnection(connectString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM Items," +
                    "(SELECT ContractNo, PCCommitNo, Fiscal.PendingNo FROM PC, " +
                    "(SELECT CommitNo, PendingNo FROM Fiscal) AS Fiscal " +
                    "WHERE Fiscal.CommitNo = PC.PCCommitNo) AS PC " +
                    "WHERE PC.ContractNo = Items.MlzContractNo;",
                };

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        AdminPurchaseItem adminPurchaseItem = new AdminPurchaseItem()
                        {
                            ContractNo = reader["MlzContractNo"].ToString(),
                            ItemDesc = reader["ItemName"].ToString(),
                            ItemQuantity = double.Parse(reader["Quantity"].ToString()),
                            ItemUnit = reader["Unit"].ToString(),
                            ItemUnitPrice = double.Parse(reader["UnitPrice"].ToString()),
                            CommitNu = reader["PCCommitNo"].ToString(),
                            PendingNu = reader["PendingNo"].ToString(),
                        };

                        if (adminPurchaseItems is null)
                        {
                            adminPurchaseItems = new List<AdminPurchaseItem>();
                        }

                        adminPurchaseItems.Add(adminPurchaseItem);
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("AdminPurchaseItems:" + ex.Message);
                    return null;
                }
            }

            return adminPurchaseItems;
        }
    }
}
