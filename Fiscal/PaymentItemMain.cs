using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Windows;

namespace G8FinApp.Fiscal
{
    public class PaymentItemMain : ObservableCollection<PaymentItem>
    {
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();
        public PaymentItemMain()
        {

        }

        public void InitList(string paymentListId)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM DisPaymentItem WHERE PaymentListId = ?"
                };

                _ = cmd.Parameters.AddWithValue("@PaymentListId", paymentListId);

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        PaymentItem paymentItem = new PaymentItem()
                        {
                            ID = reader[0].ToString(),
                            PaymentListId = reader[1].ToString(),
                            CompanyName = reader[2].ToString(),

                            CompanyAddress = reader[3].ToString(),
                            BankName = reader[4].ToString(),
                            IBANNu = reader[5].ToString(),

                            InvNu = reader[6].ToString(),
                            InvDate = DateTime.Parse(reader[7].ToString()),
                            PayAmount = decimal.Parse(reader[8].ToString()),
                            PayCurr = reader[9].ToString(),
                        };

                        Add(paymentItem);
                    }

                    return;
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show("Error:PaymentItemMain:InitList:" + ex.Message);
                    return;
                }
            }
        }
    }
}
