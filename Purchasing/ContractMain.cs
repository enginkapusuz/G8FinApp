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
    public class ContractMain : ObservableCollection<Contract>
    {
        private const string curFormat = "##.00";
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();

        public ContractMain()
        {

        }

        public bool InitContracts()
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM PurchasingContractMainQuery",
                };

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        Contract contract = new Contract()
                        {
                            ID = int.Parse(reader["ID"].ToString()),
                            BiddingId = reader["BiddingId"].ToString(),
                            PcDate = DateTime.Parse(reader["PccDate"].ToString()),
                            PcAmount = decimal.Parse(reader["PcAmount"].ToString()),
                            BiddingCurr = reader["BiddingCurr"].ToString(),
                            Company = reader["Company"].ToString(),
                            ContractNo = reader["ContractNo"].ToString(),
                            BiddingName = reader["BiddingName"].ToString(),
                            PendingNo = reader["PendingNo"].ToString(),
                        };

                        Add(contract);
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("ContractMain:InitContracts:" + ex.Message);
                    return false;
                }
            }
            return true;
        }

        public bool InitContractsNotSendTo()
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM PurchasingContractNotSendTo",
                };

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Contract contract = new Contract()
                        {
                            ID = int.Parse(reader["ID"].ToString()),
                            BiddingId = reader["BiddingId"].ToString(),
                            PcDate = DateTime.Parse(reader["PcDate"].ToString()),
                            PcAmount = decimal.Parse(reader["PcAmount"].ToString()),
                            Company = reader["Company"].ToString(),
                            ContractNo = reader["ContractNo"].ToString(),
                            BiddingName = reader["BiddingName"].ToString(),
                        };

                        Add(contract);
                    }
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show("ContractMain:InitContractsNotSendTo:" + ex.Message);
                    return false;
                }
            }

            return true;
        }

        public bool SaveData(Contract contract)
        {
            using(OleDbConnection con  = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "INSERT INTO PurchasingContract (BiddingId, PcDate, PcAmount, Company, ContractNo) VALUES(?, ?, ?, ?, ?)",
                };

                cmd.Parameters.AddWithValue("@BiddingId", contract.BiddingId);
                cmd.Parameters.AddWithValue("@PcDate", contract.PcDate);
                cmd.Parameters.AddWithValue("@PcAmount", contract.PcAmount.ToString());
                cmd.Parameters.AddWithValue("@Company", contract.Company);
                cmd.Parameters.AddWithValue("@ContractNo", contract.ContractNo);

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
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("ContractMain:SaveData:" + ex.Message);
                    return false;
                }
            }
            return true;
        }

        public bool SendToFincon(Contract contract)
        {
            Fincon.FinconApprove finconApprove;
            Fincon.FinconApproveMain finconApproveMain = new Fincon.FinconApproveMain(loadInitList:false);

            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM PurchasingContractSendToFincon WHERE ID = ?",
                };

                cmd.Parameters.AddWithValue("@ID", contract.ID);

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        finconApprove = new Fincon.FinconApprove()
                        {
                            MAININD = reader["MainId"].ToString(),
                            ENCUMID = reader["EncumbId"].ToString(),
                            TABLENAME = reader["TableName"].ToString(),
                            SENDTO = reader["SendTo"].ToString(),
                            APPROVECHOICE = reader["ApproveChoice"].ToString(),
                            APPDATE = DateTime.Parse(reader["AppDate"].ToString()),
                            FMNAME = reader["FmName"].ToString(),
                            REQDESC = reader["ReqDesc"].ToString(),
                            REQAMOUNT = decimal.Parse(reader["PcAmount"].ToString()),
                            REQCURR = reader["ReqCurr"].ToString(),
                            BDGTCURR = reader["BdgtCurr"].ToString(),
                            BDGTAMOUNT = decimal.Parse(reader["BdgtAmount"].ToString()),
                            PENDINGNO = reader["PendingNo"].ToString(),
                        };

                        if (finconApprove is null)
                        {
                            _ = MessageBox.Show("Approve is null!");
                            return false;
                        }

                        if (!finconApproveMain.SaveData(finconApprove))
                        {
                            _ = MessageBox.Show("Error: ContractMain:SendToFincon");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show("ContractMain:SendToFincon:" + ex.Message);
                    return false;
                }
            }

            return true;
        }
    }
}
