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
    public class MiscellaneousMain : ObservableCollection<Miscellaneous>
    {
        Database.ProgramConsts programConsts = new Database.ProgramConsts();

        public MiscellaneousMain()
        {

        }

        public void InitList()
        {
            using(OleDbConnection con = new OleDbConnection(programConsts.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM FiscalMiscellaneous",
                };

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        Miscellaneous miscellaneous = new Miscellaneous()
                        {
                            ID = reader["ID"].ToString(),
                            EntryType = reader["EntryType"].ToString(),
                            Remarks = reader["Remarks"].ToString(),
                            PlannedPayment = decimal.Parse(reader["PlannedPayment"].ToString()),
                            PaymentCurrency = reader["PaymentCurrency"].ToString(),
                            PlannedPaymentDate = DateTime.Parse(reader["PlannedPaymentDate"].ToString()),
                            CommitNumber = reader["CommitNumber"].ToString(),
                            CommitDate = string.IsNullOrEmpty(reader["CommitDate"].ToString()) ? DateTime.MinValue : DateTime.Parse(reader["CommitDate"].ToString()),
                            PaymentDate = string.IsNullOrEmpty(reader["PaymentDate"].ToString()) ? DateTime.MinValue : DateTime.Parse(reader["PaymentDate"].ToString()),
                            PaymentAmount = string.IsNullOrEmpty(reader["PaymentAmount"].ToString()) ? decimal.Zero : decimal.Parse(reader["PaymentAmount"].ToString()),
                            InvoiceNumber = reader["InvoiceNumber"].ToString(),
                        };

                        Add(miscellaneous);
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("MiscellaneousMain:" + ex.Message);
                    return;
                }
            }
        }

        public bool SaveData(Miscellaneous miscellaneous)
        {
            using(OleDbConnection con = new OleDbConnection(programConsts.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "INSERT INTO FiscalMiscellaneous(EntryType, Remarks, PlannedPayment, PlannedPaymentDate, PaymentCurrency) " +
                    "VALUES(?, ?, ?, ?, ?)",
                };

                cmd.Parameters.AddWithValue("@EntryType", miscellaneous.EntryType);
                cmd.Parameters.AddWithValue("@Remarks", miscellaneous.Remarks);
                cmd.Parameters.AddWithValue("@PlannedPayment", miscellaneous.PlannedPayment.ToString());
                cmd.Parameters.AddWithValue("@PlannedPaymentDate", miscellaneous.PlannedPaymentDate.ToString());
                cmd.Parameters.AddWithValue("@PaymentCurrency", miscellaneous.PaymentCurrency);

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
                    _ = MessageBox.Show("Miscellaneous:" + ex.Message);
                    return false;
                }
            }
            return true;
        }

        public bool DeleteData(Miscellaneous miscellaneous)
        {
            using(OleDbConnection con = new OleDbConnection(programConsts.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "DELETE FROM FiscalMiscellaneous WHERE ID = ?",
                };
                cmd.Parameters.AddWithValue("@ID", miscellaneous.ID);
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
                    _ = MessageBox.Show("MiscellaneousMain:DeleteMain:" + ex.Message);
                    return false;
                }
            }
            return true;
        }

        public bool UpdateDisbursingPayment(Miscellaneous miscellaneous)
        {
            using(OleDbConnection con = new OleDbConnection(programConsts.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "UPDATE FiscalMiscellaneous SET PaymentDate = ?, PaymentAmount = ?, InvoiceNumber = ? " +
                    "WHERE ID = ?",
                };

                cmd.Parameters.AddWithValue("@PaymentDate", miscellaneous.PaymentDate.ToString());
                cmd.Parameters.AddWithValue("@PaymentAmount", miscellaneous.PaymentAmount.ToString());
                cmd.Parameters.AddWithValue("@InvoiceNumber", miscellaneous.InvoiceNumber);
                cmd.Parameters.AddWithValue("@ID", miscellaneous.ID);

                try
                {
                    con.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter()
                    {
                        UpdateCommand = cmd,
                    };

                    if(!(adapter.UpdateCommand.ExecuteNonQuery() > 0))
                    {
                        return false;
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("MiscellaneousMain:UpdateDisbursingPayment:" + ex.Message);
                    return false;
                }
            }
            return true;
        }

        public bool DeleteDataFromDisbursing(Fiscal.Miscellaneous miscellaneous)
        {
            using(OleDbConnection con = new OleDbConnection(programConsts.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "UPDATE FiscalMiscellaneous SET PaymentDate = ?, PaymentAmount = ?, InvoiceNumber = ? " +
                    "WHERE ID = ?",
                };

                cmd.Parameters.AddWithValue("@PaymentDate",DateTime.MinValue.ToString());
                cmd.Parameters.AddWithValue("@PaymentAmount", decimal.Zero.ToString());
                cmd.Parameters.AddWithValue("@InvoiceNumber", string.Empty);
                cmd.Parameters.AddWithValue("@ID", miscellaneous.ID);
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
                    _ = MessageBox.Show("MiscellaneousMain:DeleteDataFromDisbursing:" + ex.Message);
                    return false;
                }
            }
            return true;
        }

        public bool UpdateFiscalCommitNu(Miscellaneous miscellaneous)
        {
            using(OleDbConnection con = new OleDbConnection(programConsts.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "UPDATE FiscalMiscellaneous SET CommitNumber = ?, CommitDate = ? WHERE ID = ?",
                };

                cmd.Parameters.AddWithValue("@CommitNumber", miscellaneous.CommitNumber);
                cmd.Parameters.AddWithValue("@CommitDate", miscellaneous.CommitDate.ToString());
                cmd.Parameters.AddWithValue("@ID", miscellaneous.ID);

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
                    _ = MessageBox.Show("MiscellaneousMain:UpdateFiscalCommitNu:" + ex.Message);
                    return false;
                }
            }
            return true;
        }
    }
}
