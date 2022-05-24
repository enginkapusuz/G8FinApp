using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Windows;

namespace G8FinApp.Admin
{
    public class BackupContract
    {
        public string Remarks { get; private set; }
        public string ContractNo { get; private set; }
        public double DepositIn { get; private set; }

        Database.ProgramConsts programConsts = new Database.ProgramConsts();

        public List<BackupContract> LoadBackupContracts()
        {
            List<BackupContract> backupContracts = null;
            using (OleDbConnection con = new OleDbConnection(programConsts.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM PurchasingBackupDepositContractNoAddress",
                };

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        BackupContract backupContract = new BackupContract()
                        {
                            Remarks = reader["Remarks"].ToString(),
                            ContractNo = reader["ContractNo"].ToString(),
                        };

                        if (backupContracts is null)
                        {
                            backupContracts = new List<BackupContract>();
                        }

                        backupContracts.Add(backupContract);
                    }
                    return backupContracts;
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show("BackupContracts:LoadBackupContracts:" + ex.Message);
                    return null;
                }
            }
        }

        public List<BackupContract> LoadBackupContractsDeposit(List<BackupContract> backupContracts, Reports.ReportExcel reportExcel)
        {
            return new List<BackupContract>();
        }
    }
}
