
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

    public class AccountInMain : ObservableCollection<AccountTrans>
    {
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();

        public AccountInMain()
        {
            InitList();
        }

        public AccountInMain(bool isCollectionEmpty)
        {

        }

        private void InitList()
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM DisbursingAccountIn"
                };

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        AccountTrans accountTrans = new AccountTrans()
                        {
                            ID = reader[0].ToString(),
                            AccountId = reader[1].ToString(),
                            TransAmount = decimal.Parse(reader[2].ToString()),
                            TransDate = DateTime.Parse(reader[3].ToString()),
                            CashBookId = reader[4].ToString(),
                        };

                        Add(accountTrans);

                    }

                    return;
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("Error:AccountInMain:InitList:" + ex.Message);
                    return;
                }
            }
        }

        public bool IsOpenExistAccountIn(string accountId, string transRemark, OleDbConnection con)
        {
            OleDbCommand cmd = new OleDbCommand()
            {
                Connection = con,
                CommandType = System.Data.CommandType.Text,
                CommandText = "SELECT AccountId, InRemarks FROM DisbursingAccountIn WHERE AccountId = @AccountId AND InRemarks = @InRemarks"
            };

            _ = cmd.Parameters.AddWithValue("@AccountIn", accountId);
            _ = cmd.Parameters.AddWithValue("@InRemarks", transRemark);

            try
            {
                OleDbDataReader reader = cmd.ExecuteReader();

                while(reader.Read())
                {
                    if (reader[0].ToString() == accountId)
                    {
                        if (reader[1].ToString() == transRemark)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
            catch(Exception ex)
            {
                _ = MessageBox.Show("Error:AccountInMain:IsOpenExistAcoountIn:" + ex.Message);
                return false;
            }
        }

        public bool SaveData(AccountTrans accountTrans)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "INSERT INTO DisbursingAccountIn (AccountId, InAmount, InDate, InRemarks, CashBookId) VALUES(?, ?, ?, ?, ?)",
                };

                _ = cmd.Parameters.AddWithValue("@AccountId", accountTrans.AccountId.Trim());
                _ = cmd.Parameters.AddWithValue("@InAmount", accountTrans.TransAmount.ToString());
                _ = cmd.Parameters.AddWithValue("@InDate", accountTrans.TransDate.ToString("d"));

                _ = cmd.Parameters.AddWithValue("@InRemarks", accountTrans.TransRemark.Trim());
                _ = cmd.Parameters.AddWithValue("@CashBookId", accountTrans.CashBookId.Trim());

                try
                {
                    con.Open();

                    if (IsOpenExistAccountIn(accountTrans.AccountId.Trim(), accountTrans.TransRemark.Trim(), con))
                    {
                        return false;
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
                    _ = MessageBox.Show("Error:AccountInMain:SaveData:" + ex.Message);
                    return false;
                }
            }

            return false;
        }

        public decimal GetAccountInTotal(string accountId)
        {
            IEnumerable<decimal> amountLst = from acc in this
                                           where acc.ID == accountId
                                           select acc.TransAmount;
            return amountLst.Sum();
        }
    }
}
