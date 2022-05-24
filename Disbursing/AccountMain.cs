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
    public class AccountMain : ObservableCollection<Account>
    {
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();

        public AccountMain()
        {
            InitList();
        }

        private decimal GetAccountInTotal(string ID, OleDbConnection con)
        {
            OleDbCommand cmd = new OleDbCommand()
            {
                Connection = con,
                CommandType = System.Data.CommandType.Text,
                CommandText = "SELECT * FROM DisbursingAccountInSum"
            };

            try
            {
                OleDbDataReader reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    if (ID == reader[1].ToString())
                    {
                        return decimal.Parse(reader[0].ToString());
                    }
                }
            }
            catch(Exception ex)
            {
                _ = MessageBox.Show("Error:AccountMain:GetAccountTotal:" + ex.Message);
                return decimal.MinValue;
            }

            return decimal.MinValue;
        }

        private decimal GetAccountOutTotal(string ID, OleDbConnection con)
        {
            OleDbCommand cmd = new OleDbCommand()
            {
                Connection = con,
                CommandType = System.Data.CommandType.Text,
                CommandText = "SELECT * FROM DisbursingAccountOutSum"
            };

            try
            {
                OleDbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (ID == reader[1].ToString())
                    {
                        return decimal.Parse(reader[0].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show("Error:AccountMain:GetAccountOutTotal:" + ex.Message);
                return decimal.MinValue;
            }

            return decimal.MinValue;
        }
        private void InitList()
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM DisbursingAccount"
                };

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        Account account = new Account()
                        {
                            ID = reader[0].ToString(),
                            AccountName = reader[1].ToString(),
                            AccountNu = reader[2].ToString(),
                            AccountCurr = reader[3].ToString(),
                        };

                        decimal accountOutTotal = GetAccountOutTotal(account.ID, con);
                        accountOutTotal = accountOutTotal == decimal.MinValue ? 0 : accountOutTotal;

                        account.AccInTotal = GetAccountInTotal(account.ID, con);
                        account.AccInTotal = account.AccInTotal == decimal.MinValue ? 0 : account.AccInTotal;

                        account.AccInTotal = account.AccInTotal - accountOutTotal;

                        Add(account);
                    }

                    return;
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("Error:AccountMain:InitList:" + ex.Message);
                    return;
                }
            }
        }

        public bool SaveData(Account account)
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "INSERT INTO DisbursingAccount (AccountName, AccountNu, AccountCurr)" +
                    "VALUES(@AccountName, @AccountNu, @AccountCurr)"
                };

                _ = cmd.Parameters.AddWithValue("@AccountName", account.AccountName.Trim());
                _ = cmd.Parameters.AddWithValue("@AccountNu", account.AccountNu.Trim());
                _ = cmd.Parameters.AddWithValue("@AccountCurr", account.AccountCurr.Trim());

                try
                {
                    con.Open();

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
                    _ = MessageBox.Show("Error:AccountMain:SaveData:" + ex.Message);
                    return false;
                }
            }

            return false;
        }
    }
}
