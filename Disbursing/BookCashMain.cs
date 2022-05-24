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
    public class BookCashMain : ObservableCollection<BookCash>
    {
        //Class variable for LastIdNu
        public string LastIdNu { get; private set; }
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();

        public BookCashMain()
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
                    CommandText = "SELECT * FROM BookCash",
                };

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        Add(new BookCash
                        {
                            ID = reader[0].ToString(),
                            Description = reader[1].ToString(),
                            CashBookDate = DateTime.Parse(reader[2].ToString())
                        });
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("Error:BookCashMain:" + ex.Message);
                    return;
                }
            }
        }

        public bool TakeLastIdNumber()
        {
            string lastIdNu = string.Empty;

            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT ID FROM BookCash",
                };

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        lastIdNu = reader[0].ToString();
                    }

                    if (string.IsNullOrEmpty(lastIdNu))
                    {
                        return false;
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("Error:BookCashMain:TakeLastId:" + ex.Message);
                    return false;
                }
            }
            LastIdNu = lastIdNu;
            return true;
        }
        public bool SaveData(BookCash bookCash)
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "INSERT INTO BookCash (Description, CashBookDate) VALUES(?, ?)",
                };

                cmd.Parameters.AddWithValue("@Description", bookCash.Description);
                cmd.Parameters.AddWithValue("@CashBookDate", bookCash.CashBookDate.ToString("d"));

                try
                {
                    con.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter()
                    {
                        InsertCommand = cmd,
                    };
                    adapter.InsertCommand = cmd;

                    if (adapter.InsertCommand.ExecuteNonQuery() > 0)
                    {
                        return true;
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("Error:BookCashMain:SaveData:" + ex.Message);
                    return false;
                }
            }
            return false;
        }
    }
}
