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
    public class AccountOutMain : ObservableCollection<AccountTrans>
    {
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();

        public AccountOutMain()
        {
            InitList();
        }

        public AccountOutMain(bool isCollectionEmpty)
        {

        }

        private void InitList()
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM DisbursingAccountOut"
                };

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
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
                catch (Exception ex)
                {
                    _ = MessageBox.Show("Error:AccountOutMain:InitList:" + ex.Message);
                    return;
                }
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
                    CommandText = "INSERT INTO DisbursingAccountOut (AccountId, OutAmount, OutDate, OutRemarks, CashBookId) VALUES(?, ?, ?, ?, ?)",
                };

                _ = cmd.Parameters.AddWithValue("@AccountId", accountTrans.AccountId.Trim());
                _ = cmd.Parameters.AddWithValue("@OutAmount", accountTrans.TransAmount.ToString());
                _ = cmd.Parameters.AddWithValue("@OutDate", accountTrans.TransDate.ToString("d"));

                _ = cmd.Parameters.AddWithValue("@OutRemarks", accountTrans.TransRemark.Trim());
                _ = cmd.Parameters.AddWithValue("@CashBookId", accountTrans.CashBookId.Trim());

                try
                {
                    con.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter()
                    {
                        InsertCommand = cmd,
                    };

                    if (adapter.InsertCommand.ExecuteNonQuery() > 0)
                    {
                        return true;
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("Error:AccountOutMain:SaveData:" + ex.Message);
                    return false;
                }
            }
            _ = MessageBox.Show("AccountOutMain:SaveData:" + "Data couldn't save!");
            return false;
        }
    }
}
