using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System.Data.OleDb;


namespace G8FinApp.Disbursing
{
    public class CashCallMain : ObservableCollection<Fiscal.CashCall>
    {
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();
        public void InitList()
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM FiscalCashCallQuery",
                };

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();

                    //Possible Null Values
                    DateTime dtTmPaymentDate = DateTime.MinValue;
                    DateTime dtTmCommitDate = DateTime.MinValue;
                    decimal dcmlPaymentAmount = 0;
                    string strInvoiceNumber = string.Empty;
                    //

                    while (reader.Read())
                    {
                        if (DateTime.TryParse(reader["PaymentDate"].ToString(), out DateTime _dtTmPaymentDate))
                        {
                            dtTmPaymentDate = _dtTmPaymentDate;
                        }

                        if (DateTime.TryParse(reader["CommitDate"].ToString(), out DateTime _dtTmCommitDate))
                        {
                            dtTmCommitDate = _dtTmCommitDate;
                        }

                        if (decimal.TryParse(reader["PaymentAmount"].ToString(), out decimal _dcmlPaymentAmount))
                        {
                            dcmlPaymentAmount = _dcmlPaymentAmount;
                        }


                        Fiscal.CashCall cashCall = new Fiscal.CashCall
                        {
                            ID = reader["ID"].ToString(),
                            FiscalCountriesID = reader["FiscalCountriesID"].ToString(),
                            CountryName = reader["CountryName"].ToString(),
                            Remarks = reader["Remarks"].ToString(),
                            PlannedPayment = decimal.Parse(reader["PlannedPayment"].ToString()),
                            PlannedPaymentDate = DateTime.Parse(reader["PlannedPaymentDate"].ToString()),
                            PaymentDate = dtTmPaymentDate,
                            PaymentAmount = dcmlPaymentAmount,
                            CommitNumber = reader["CommitNumber"].ToString(),
                            CommitDate = dtTmCommitDate,
                            InvoiceNumber = reader["InvoiceNumber"].ToString(),
                        };

                        Add(cashCall);
                    }
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show("CashCallMain:InitList:" + ex.Message);
                    return;
                }
            }
        }

        public bool UpdateData(Fiscal.CashCall cashCall)
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "UPDATE FiscalCashCall SET PaymentDate = ?, PaymentAmount = ?, InvoiceNumber = ? WHERE ID = ?",
                };

                cmd.Parameters.AddWithValue("@PaymentDate", cashCall.PaymentDate);
                cmd.Parameters.AddWithValue("@PaymentAmount", cashCall.PaymentAmount.ToString());
                cmd.Parameters.AddWithValue("@InvoiceNumber", cashCall.InvoiceNumber);
                cmd.Parameters.AddWithValue("@ID", int.Parse(cashCall.ID));

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
                    _ = MessageBox.Show("CashCallMain:UpdateData:" + ex.Message);
                    return false;
                }
            }
            return true;
        }

        public bool DeletePaymentData(Fiscal.CashCall cashCall)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "UPDATE FiscalCashCall SET PaymentDate = ?, PaymentAmount = ?, InvoiceNumber = ? WHERE ID = ?",
                };

                cmd.Parameters.AddWithValue("@PaymentDate", DateTime.MinValue);
                cmd.Parameters.AddWithValue("@PaymentAmount", 0);
                cmd.Parameters.AddWithValue("@InvoiceNumber", string.Empty);
                cmd.Parameters.AddWithValue("@ID", int.Parse(cashCall.ID));

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
                    _ = MessageBox.Show("CashCallMain:UpdateData:" + ex.Message);
                    return false;
                }
            }
            return true;
        }
    }
}
