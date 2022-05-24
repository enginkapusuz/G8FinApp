using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Windows;
using System.Collections.ObjectModel;

namespace G8FinApp.Fiscal
{
    public class CashCallMain : ObservableCollection<CashCall>
    {
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();

        public CashCallMain()
        {

        }

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

                        CashCall cashCall = new CashCall
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
                catch(Exception ex)
                {
                    _ = MessageBox.Show("CashCallMain:InitList:" + ex.Message);
                    return;
                }
            }
        }

        public bool SaveData(CashCall cashCall)
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "INSERT INTO FiscalCashCall (FiscalCountriesID, Remarks, PlannedPayment, PlannedPaymentDate) VALUES(?, ?,	?, ?)",
                };

                cmd.Parameters.AddWithValue("@FiscalCountriesID", cashCall.FiscalCountriesID);
                cmd.Parameters.AddWithValue("@Remarks", cashCall.Remarks);
                cmd.Parameters.AddWithValue("@PlannedPayment", cashCall.PlannedPayment.ToString(prgrmConst.curFormat));
                cmd.Parameters.AddWithValue("@PlannedPaymentDate", cashCall.PlannedPaymentDate.ToString("d"));

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
                    _ = MessageBox.Show("CashCallMain:SaveData:" + ex.Message);
                    return false;
                }
            }
            return true;
        }

        public bool DeleteData(CashCall cashCall)
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "DELETE * FROM FiscalCashCall WHERE ID = ?",
                };

                cmd.Parameters.AddWithValue("@ID", cashCall.ID);

                try
                {
                    con.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter()
                    {
                        DeleteCommand = cmd,
                    };

                    if (!(adapter.DeleteCommand.ExecuteNonQuery() > 0))
                    {
                        return false;
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("CashCallMain:DeleteData:" + ex.Message);
                    return false;
                }
            }

            return true;
        }

        public bool SaveComitNumber(CashCall cashCall)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "UPDATE FiscalCashCall SET CommitDate = ?, CommitNumber = ? WHERE ID = ?",
                };

                cmd.Parameters.AddWithValue("@CommitDate", cashCall.CommitDate.ToString("d"));
                cmd.Parameters.AddWithValue("@CommitNumber", cashCall.CommitNumber);
                cmd.Parameters.AddWithValue("@ID", cashCall.ID);

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
                    _ = MessageBox.Show("CashCallMain:SaveComitNumber:" + ex.Message);
                    return false;
                }
            }
            return true;
        }
    }
}
