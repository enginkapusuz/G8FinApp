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
    public class DepositMain:ObservableCollection<Deposit>
    {
        Database.ProgramConsts prgrmConsts = new Database.ProgramConsts();
        string _userName;
        public DepositMain()
        {

        }

        public DepositMain(bool isInitList, string userName)
        {
            _userName = userName;
            InitList();
        }

        private void InitList()
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConsts.connectionString))
            {
                string purchasingCommandText = "SELECT * FROM PurchasingDepositQuery";
                string disbursingCommandText = "SELECT * FROM PurchasingDepositQuery WHERE DisbDepositAmount = 0";
                string fiscalCommandText = "SELECT * FROM PurchasingDepositQuery WHERE CommitNu IS NULL";
                
                string purchasingCloseCommandText = "SELECT * FROM PurchasingDepositQuery WHERE CommitNu IS NOT NULL AND PCReturnDate IS NULL";
                string disbursingCloseDepositCommandText = "SELECT * FROM PurchasingDepositQuery WHERE PCReturnDate IS NOT NULL AND DisbReturnDate IS NULL";

                string fiscalCloseCommandText = "SELECT * FROM PurchasingDepositQuery WHERE DisbReturnDate IS NOT NULL AND FiscalReturnDate IS NULL";


                string cmdText = string.Empty;

                if (_userName == "Purchasing")
                {
                    cmdText = purchasingCommandText;
                }

                if (_userName == "Disbursing")
                {
                    cmdText = disbursingCommandText;
                }

                if(_userName == "Fiscal")
                {
                    cmdText = fiscalCommandText;
                }

                if (_userName == "PurchasingClose")
                {
                    cmdText = purchasingCloseCommandText;
                }
                if (_userName == "DisbursingClose")
                {
                    cmdText = disbursingCloseDepositCommandText;
                }

                if (_userName == "FiscalClose")
                {
                    cmdText = fiscalCloseCommandText;
                }


                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = cmdText,
                };


                try
                {
                    con.Open();

                    OleDbDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        DateTime _dtTimeDisApp = DateTime.MinValue;
                        DateTime _dtTmCommitDate = DateTime.MinValue;
                        DateTime _dtTmPCReturnDate = DateTime.MinValue;
                        DateTime _dtTmDisbReturnDate = DateTime.MinValue;
                        DateTime _dtTmFisReturnDate = DateTime.MinValue;

                        decimal _dcmlDisbDepositAmount = 0;
                        decimal _dcmlPCReturnAmount = 0;
                        decimal _dcmlDisbReturnAmount = 0;
                        string commitNu = string.Empty;

                        if (DateTime.TryParse(reader["DisbAppDate"].ToString(), out DateTime dtTmDisbApp))
                        {
                            _dtTimeDisApp = dtTmDisbApp;
                        }

                        if(!string.IsNullOrEmpty(reader["CommitNu"].ToString()))
                        {
                            commitNu = reader["CommitNu"].ToString();
                        }
                        if(DateTime.TryParse(reader["CommitDate"].ToString(), out DateTime dtTmCommitDate))
                        {
                            _dtTmCommitDate = dtTmCommitDate;
                        }

                        if (decimal.TryParse(reader["DisbDepositAmount"].ToString(), out decimal dcmlDisbDepositAmount))
                        {
                            _dcmlDisbDepositAmount = dcmlDisbDepositAmount;
                        }

                        if (DateTime.TryParse(reader["PCReturnDate"].ToString(), out DateTime dtTmPCReturnDate))
                        {
                            _dtTmPCReturnDate = dtTmPCReturnDate;
                        }

                        
                        if (decimal.TryParse(reader["PCReturnAmount"].ToString(), out decimal dcmlPCReturnAmount))
                        {
                            _dcmlPCReturnAmount = dcmlPCReturnAmount;
                        }

                        if (DateTime.TryParse(reader["DisbReturnDate"].ToString(), out DateTime dtTmDisbReturnDate))
                        {
                            _dtTmDisbReturnDate = dtTmPCReturnDate;
                        }

                        if (decimal.TryParse(reader["DisbReturnAmount"].ToString(), out decimal dcmlDisbReturnAmount))
                        {
                            _dcmlDisbReturnAmount = dcmlPCReturnAmount;
                        }

                        if (DateTime.TryParse(reader["FiscalReturnDate"].ToString(), out DateTime dtTmDFisReturnDate))
                        {
                            _dtTmFisReturnDate = dtTmDFisReturnDate;
                        }

                        Deposit deposit = new Deposit()
                        {
                            ID = int.Parse(reader["ID"].ToString()),
                            ContractId = int.Parse(reader["ContractId"].ToString()),
                            ContractNo = reader["ContractNo"].ToString(),

                            BiddingName = reader["BiddingName"].ToString(),
                            DepositRate = decimal.Parse(reader["DepositRate"].ToString()),
                            DepositDate = DateTime.Parse(reader["DepositDate"].ToString()),

                            PurchasingDepositAmount = decimal.Parse(reader["PurchasingDepositAmount"].ToString()),
                            DepositCurrency = reader["DepositCurrency"].ToString(),
                            Company = reader["Company"].ToString(),

                            BiddingPrice = decimal.Parse(reader["BiddingPrice"].ToString()),
                            BiddingCurr = reader["BiddingCurr"].ToString(),
                            DisbAppDate = _dtTimeDisApp,

                            DisbDepositAmount = _dcmlDisbDepositAmount,
                            CommitNu = commitNu,
                            CommitDate = _dtTmCommitDate,

                            PCReturnDate = _dtTmPCReturnDate,
                            PCReturnAmount = _dcmlPCReturnAmount,

                            DisbReturnDate = _dtTmDisbReturnDate,
                            DisbReturnAmount = _dcmlDisbReturnAmount,

                            FiscalReturnDate = _dtTmFisReturnDate,
                        };

                        Add(deposit);
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("DepositMain:InitList:" + ex.Message);
                    return;
                }
            }
        }

        public bool SaveData(Deposit deposit)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConsts.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "INSERT INTO PurchasingDeposit " +
                    "(ContractId, DepositDate, PurchasingDepositAmount, " +
                    "DepositRate, DepositCurrency, Company, " +
                    "BiddingPrice, BiddingCurr) " +
                    "VALUES" +
                    "(?, ?, ?, " +
                    "?, ?, ?, " +
                    "?, ?)",
                };

                cmd.Parameters.AddWithValue("@ContractId", deposit.ContractId);
                cmd.Parameters.AddWithValue("@DepositDate", deposit.DepositDate);
                cmd.Parameters.AddWithValue("@PurchasingDepositAmount", deposit.PurchasingDepositAmount);

                cmd.Parameters.AddWithValue("@DepositRate", deposit.DepositRate);
                cmd.Parameters.AddWithValue("@DepositCurrency", deposit.DepositCurrency);
                cmd.Parameters.AddWithValue("@Company", deposit.Company);

                cmd.Parameters.AddWithValue("@BiddingPrice", deposit.BiddingPrice.ToString(prgrmConsts.curFormat));
                cmd.Parameters.AddWithValue("@BiddingCurr", deposit.BiddingCurr);

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
                    _ = MessageBox.Show("DepositMain:SaveData:" + ex.Message);
                    return false;
                }
            }
            return true;
        }

        public bool DisbursingUpdate(Deposit deposit)
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConsts.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "UPDATE PurchasingDeposit SET DisbAppDate = ?, DisbDepositAmount = ?, DepositCurrency = ? WHERE ID = ?",
                };

                cmd.Parameters.AddWithValue("@DisbAppDate", deposit.DisbAppDate.ToString());
                cmd.Parameters.AddWithValue("@DisbDepositAmount", deposit.DisbDepositAmount.ToString(prgrmConsts.curFormat));
                cmd.Parameters.AddWithValue("@DepositCurrency", deposit.DepositCurrency);
                cmd.Parameters.AddWithValue("@ID", deposit.ID);

                try
                {
                    con.Open();

                    OleDbDataAdapter adapter = new OleDbDataAdapter()
                    {
                        UpdateCommand = cmd,
                    };

                    if (!(adapter.UpdateCommand.ExecuteNonQuery() > 0))
                    {
                        return false;
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("DepositMain:DisbursingUpdate:" + ex.Message);
                    return false;
                }
            }
            return true;
        }

        public bool FiscalUpdate(Deposit deposit)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConsts.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "UPDATE PurchasingDeposit SET CommitDate = ?, CommitNu = ? WHERE ID = ?",
                };

                cmd.Parameters.AddWithValue("@CommitDate", deposit.CommitDate.ToString("d"));
                cmd.Parameters.AddWithValue("@CommitNu", deposit.CommitNu);
                cmd.Parameters.AddWithValue("@ID", deposit.ID);

                try
                {
                    con.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter()
                    {
                        UpdateCommand = cmd,
                    };

                    if (!(adapter.UpdateCommand.ExecuteNonQuery() > 0))
                    {
                        return false;
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("DepositMain:FiscalUpdate:" + ex.Message);
                    return false;
                }
            }
            return true;
        }

        public bool PurchasingCloseUpdate(Deposit deposit)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConsts.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "UPDATE PurchasingDeposit SET PCReturnDate = ?, PCReturnAmount = ? WHERE ID = ?",
                };

                cmd.Parameters.AddWithValue("@PCReturnDate", deposit.PCReturnDate.ToString("d"));
                cmd.Parameters.AddWithValue("@PCReturnAmount", deposit.PCReturnAmount.ToString(prgrmConsts.curFormat));
                cmd.Parameters.AddWithValue("@ID", deposit.ID);

                try
                {
                    con.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter()
                    {
                        UpdateCommand = cmd,
                    };

                    if (!(adapter.UpdateCommand.ExecuteNonQuery() > 0))
                    {
                        return false;
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("DepositMain:PurchasingCloseUpdate:" + ex.Message);
                    return false;
                }
            }

            return true;
        }

        public bool DisbursingCloseUpdate(Deposit deposit)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConsts.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "UPDATE PurchasingDeposit SET DisbReturnDate = ?, DisbReturnAmount = ? WHERE ID = ?",
                };

                cmd.Parameters.AddWithValue("@DisbReturnDate", deposit.PCReturnDate.ToString("d"));
                cmd.Parameters.AddWithValue("@DisbReturnAmount", deposit.PCReturnAmount.ToString(prgrmConsts.curFormat));
                cmd.Parameters.AddWithValue("@ID", deposit.ID);

                try
                {
                    con.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter()
                    {
                        UpdateCommand = cmd,
                    };

                    if (!(adapter.UpdateCommand.ExecuteNonQuery() > 0))
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show("DepositMain:DisbursingCloseUpdate:" + ex.Message);
                    return false;
                }
            }

            return true;
        }

        public bool FiscalCloseUpdate(Deposit deposit)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConsts.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "UPDATE PurchasingDeposit SET FiscalReturnDate = ? WHERE ID = ?",
                };

                cmd.Parameters.AddWithValue("@FiscalReturnDate", deposit.FiscalReturnDate.ToString("d"));
                cmd.Parameters.AddWithValue("@ID", deposit.ID);

                try
                {
                    con.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter()
                    {
                        UpdateCommand = cmd,
                    };

                    if (!(adapter.UpdateCommand.ExecuteNonQuery() > 0))
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show("DepositMain:FiscalCloseUpdate:" + ex.Message);
                    return false;
                }
            }

            return true;
        }
    }
}
