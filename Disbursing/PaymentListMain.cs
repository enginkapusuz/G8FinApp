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
    public class PaymentListMain: ObservableCollection<PaymentList>
    {
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();
        public PaymentListMain()
        {
            InitList();
        }

        public PaymentListMain(bool isUpdate)
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
                    CommandText = "SELECT * FROM DisPaymentList",
                };

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

        public bool SaveData(PaymentList paymentList)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "INSERT INTO DisPaymentList (ListName, ListNu, ListDate, ListSituation) VALUES(?, ?, ?, ?)"
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

        public bool FiscalSaveData(PaymentList paymentList)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "INSERT INTO FiscalPaymentList(ListName, ListNu, ListDate, ListSituation, DisPaymentListId) VALUES(?, ?, ?, ?, ?)"
                };

                _ = cmd.Parameters.AddWithValue("@ListName", paymentList.ListName.Trim());
                _ = cmd.Parameters.AddWithValue("@ListNu", paymentList.ListNu.Trim());
                _ = cmd.Parameters.AddWithValue("@ListDate", paymentList.ListDate);

                _ = cmd.Parameters.AddWithValue("@ListSituation", paymentList.ListSituation.Trim());
                _ = cmd.Parameters.AddWithValue("@DisPaymentListId", paymentList.ID.Trim());

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
                catch (Exception ex)
                {
                    _ = MessageBox.Show("Error:PaymentListMain:FiscalSaveData:" + ex.Message);
                    return false;
                }
            }

            return false;
        }

        public PaymentList TakeNotPaidPaymentList(Invoice invoice)
        {
            PaymentListMain paymentListMain = new PaymentListMain();
            PaymentList paymentList = paymentListMain.Where(pay => pay.ListSituation == "NOTPAID").FirstOrDefault();

            if (paymentList is default(PaymentList))
            {
                if (string.IsNullOrEmpty(invoice.CompanyName))
                {
                    return null;
                }

                paymentList = new PaymentList()
                {
                    ListName = invoice.CompanyName,
                    ListNu = invoice.EncumbId,
                    ListDate = invoice.InvDate,
                    ListSituation = "NOTPAID",
                };

                if (!paymentListMain.SaveData(paymentList))
                {
                    _ = MessageBox.Show("Data couldn't save!");
                    return null;
                }

                paymentListMain = new PaymentListMain();
                paymentList = paymentListMain.Where(pay => pay.ListSituation == "NOTPAID").FirstOrDefault();
            }
            return paymentList;
        }
    }
}
