using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.OleDb;

namespace G8FinApp.Admin
{
    public class AdminDepositOpen
    {
        public string Year { get; private set; }
        public string CommitNo { get; private set; }
        public string InvoiceNu { get; private set; }
        public decimal InvoiceAmount { get; private set; }
        public List<FundManagerEncumbrance> DepositEncumbrances { get; private set; }

        public List<AdminDepositOpen> LoadAdminDepositOpen(string databaseName, string year)
        {
            string connectString = "Provider=Microsoft.Jet.Oledb.4.0; Data Source = DatabasesForFirstInit/" + databaseName;

            List<AdminDepositOpen> adminDepositOpens = null;

            using(OleDbConnection con = new OleDbConnection(connectString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM SubFiscal " +
                    "WHERE CommitNo LIKE '88%' AND (InvoiceReferance LIKE 'DP%' OR InvoiceReferance LIKE 'RT%') ;",
                };

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        AdminDepositOpen adminDepositOpen = new AdminDepositOpen()
                        {
                            Year = year,
                            CommitNo = reader["CommitNo"].ToString(),
                            InvoiceAmount = decimal.Parse(reader["InvoiceAmount"].ToString()),
                            InvoiceNu = reader["InvoiceReferance"].ToString(),
                        };

                        if (adminDepositOpens is null)
                        {
                            adminDepositOpens = new List<AdminDepositOpen>();
                        }
                        adminDepositOpens.Add(adminDepositOpen);
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("AdminDepositOpen:" + ex.Message);
                    return null;
                }
            }
            return adminDepositOpens;
        }

        public List<AdminDepositOpen> SelectNotPaidDeposist(List<AdminDepositOpen> adminDepositOpens)
        {
            var inDeposits = adminDepositOpens.Where(dp => dp.InvoiceNu.Contains("DP"))
                .Select (dp => new AdminDepositOpen { Year  = dp.Year, CommitNo = dp.CommitNo, InvoiceAmount = dp.InvoiceAmount * -1,  InvoiceNu  = dp.InvoiceNu });

            var outDepositsAmounts = adminDepositOpens.Where(dp => dp.InvoiceNu.Contains("RT"))
               .Select(dp => dp.InvoiceAmount).ToList();

            var notRetDeposits = from dp in inDeposits
                                 where !outDepositsAmounts.Contains(dp.InvoiceAmount)
                                 select dp;

            List < AdminDepositOpen > retAdminDepositOpens = null;

            foreach(var dp in notRetDeposits)
            {
                if (retAdminDepositOpens is null)
                {
                    retAdminDepositOpens = new List<AdminDepositOpen>();
                }

                retAdminDepositOpens.Add(dp);
            }

            return retAdminDepositOpens;
        }
    }
}
