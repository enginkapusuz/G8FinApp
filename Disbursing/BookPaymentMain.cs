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
    public class BookPaymentMain : ObservableCollection<BookPayment>
    {
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();
        public BookPaymentMain()
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
                    CommandText = "SELECT * FROM BookPayment",
                };

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Add(new BookPayment
                        {
                            ID = reader[0].ToString(),
                            Description = reader[1].ToString(),
                            PaymentBookDate = DateTime.Parse(reader[2].ToString()),
                            BookCashID = reader[3].ToString(),
                            DisPaymentListId = reader[4].ToString(),
                        });
                    }
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show("Error:BookPaymentMain:" + ex.Message);
                    return;
                }
            }
        }

        public bool SaveData(BookPayment bookPayment)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "INSERT INTO BookPayment (Description, PaymentDate, BookCashID, DisPaymentListId) VALUES(?, ?, ?, ?)",
                };

                cmd.Parameters.AddWithValue("@Description", bookPayment.Description);
                cmd.Parameters.AddWithValue("@PaymentDate", bookPayment.PaymentBookDate.ToString("d"));
                cmd.Parameters.AddWithValue("@BookCashID", bookPayment.BookCashID);
                cmd.Parameters.AddWithValue("@DisPaymentListId", bookPayment.DisPaymentListId);

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
                catch (Exception ex)
                {
                    _ = MessageBox.Show("Error:BookPaymentMain:SaveData:" + ex.Message);
                    return false;
                }
            }
            return false;
        }
    }
}
