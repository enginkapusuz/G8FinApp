using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.OleDb;


namespace G8FinApp.Admin
{
    public class AdminPurchaseContract
    {
        public string ContractNo { get; private set; }
        public string Contractor { get; private set; }
        public string POCurrency { get; private set; }
        public decimal PcAmount { get; private set; }

        public string PendingNo { get; private set; }

        public DateTime PurchasingDate { get; private set; }


        public List<AdminPurchaseContract> LoadAdminPurchaseContract()
        {
            string connectString = new AdminDatabase().ConnectStringSRD2022;

            List<AdminPurchaseContract> adminPurchaseContracts = null;

            using (OleDbConnection con = new OleDbConnection(connectString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM PC, " +
                    "(SELECT CommitNo, PendingNo FROM Fiscal) AS FSC " +
                    "WHERE FSC.CommitNo = PC.PCCommitNo " +
                    "ORDER BY MID(ContractNo, 6, 4);",
                };

                //0122C0011

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        AdminPurchaseContract adminPurchaseContract = new AdminPurchaseContract()
                        {
                            ContractNo = reader["ContractNo"].ToString(),
                            Contractor = reader["Contractor"].ToString(),
                            PcAmount = decimal.Parse(reader["PcAmount"].ToString()),
                            PendingNo = reader["PendingNo"].ToString(),
                            POCurrency = reader["POCurrency"].ToString(),
                            PurchasingDate = DateTime.Parse(reader["P&CDate"].ToString()),
                        };

                        if (adminPurchaseContracts is null)
                        {
                            adminPurchaseContracts = new List<AdminPurchaseContract>();
                        }

                        adminPurchaseContracts.Add(adminPurchaseContract);
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("AdminPurchaseContracts:" + ex.Message);
                    return null;
                }
            }
            return adminPurchaseContracts;
        }
    }
}
