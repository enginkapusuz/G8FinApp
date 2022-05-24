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
    public class PaymentListMainNotPaid: ObservableCollection<PaymentList>
    {
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();
        public PaymentListMainNotPaid()
        {
            InitList();
        }

        public PaymentListMainNotPaid(bool isUpdate)
        {
            if (isUpdate)
            {
                //stupid!
            }
        }

        public void InitList()
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM DisPaymentList WHERE ListSituation = ?",
                };

                _ = cmd.Parameters.AddWithValue("@ListSituation", "NOTPAID");

                try
                {
                    con.Open();

                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        PaymentList paymentList = new PaymentList()
                        {
                            ID = reader[0].ToString(),
                            ListName = reader[1].ToString(),
                            ListNu = reader[2].ToString(),
                            ListDate = DateTime.Parse(reader[3].ToString()),
                            ListSituation = reader[4].ToString(),
                        };

                        Add(paymentList);
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("Error:PaymentListMain:InitList:" + ex.Message);
                    return;
                }
            }
        }


        public bool UpdateData(string tblName, string paymentListId, string lstSituation)
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    //CommandText = "UPDATE DisPaymentList SET ListSituation = ? WHERE ID = ?",
                    CommandText = "UPDATE " + tblName + " SET ListSituation = ? WHERE ID = ?",
                };

                _ = cmd.Parameters.AddWithValue("@ListSituation", lstSituation);
                _ = cmd.Parameters.AddWithValue("@ID", paymentListId);

                try
                {
                    con.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter()
                    {
                        UpdateCommand = cmd,
                    };

                    if (adapter.UpdateCommand.ExecuteNonQuery() > 0)
                    {
                        return true;
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("Error:PaymentListMain:UpdateData:" + ex.Message);
                    return false;
                }
            }
            return false;
        }

        public bool SaveData(string tblName, PaymentList paymentList)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "INSERT INTO " + tblName + " (ListName, ListNu, ListDate, ListSituation)" +
                    " VALUES(@ListName, @ListNu, @ListDate, @ListSituation)"
                };

                _ = cmd.Parameters.AddWithValue("@ListName", paymentList.ListName.Trim());
                _ = cmd.Parameters.AddWithValue("@ListNu", paymentList.ListNu.Trim());
                _ = cmd.Parameters.AddWithValue("@ListDate", paymentList.ListDate);

                _ = cmd.Parameters.AddWithValue("@ListSituation", paymentList.ListSituation.Trim());

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
                    _ = MessageBox.Show("Error:PaymentListMain:SaveData:" + ex.Message);
                    return false;
                }
            }

            return false;
        }
    }
}
