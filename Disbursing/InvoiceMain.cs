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
    public class InvoiceMain : ObservableCollection<Invoice>
    {
        private const string curFormat = "##.0000";
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();
        private readonly Invoice _invoice;
        public InvoiceMain()
        {
        }

        public InvoiceMain(Invoice invoice)
        {
            _invoice = invoice;
        }

        public void InitList()
        {
            using(OleDbConnection con  = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM DisPaymentItem"
                };
                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Invoice invoice = new Invoice()
                        {
                            ID = reader["ID"].ToString(),
                            PaymentListId = reader["PaymentListId"].ToString(),
                            CompanyName = reader["CompanyName"].ToString(),

                            CompanyAddress = reader["CompanyAddress"].ToString(),
                            BankName = reader["BankName"].ToString(),
                            IBANNu = reader["IBANNu"].ToString(),

                            InvNu = reader["InvNu"].ToString(),
                            InvDate = DateTime.Parse(reader["InvDate"].ToString()),
                            PayAmount = decimal.Parse(reader["PayAmount"].ToString()),
                            PayCurr = reader["PayCurr"].ToString(),

                            BdgtCurr = reader["BdgtCurr"].ToString(),
                            BdgtAmount = decimal.Parse(reader["BdgtAmount"].ToString()),

                            MainID = reader["MainID"].ToString(),
                            EncumbId = reader["EncumbID"].ToString(),
                        };

                        Add(invoice);
                    }

                    return;
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("Error:InvoiceMain:InitList:" + ex.Message);
                    return;
                }

            }
        }

        private bool IsBudgetEnough(string mainId, decimal budgetAmount, decimal exRate, decimal reqAmount, OleDbConnection con)
        {
            OleDbCommand cmd = new OleDbCommand()
            {
                Connection = con,
                CommandType = System.Data.CommandType.Text,
                CommandText = "SELECT CurrentAmount FROM BudgetCurrentAmount WHERE ID = ?",
            };

            cmd.Parameters.AddWithValue("ID", mainId);

            OleDbDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                //To find current budget after subtract encumbrance amount

                decimal currBudgetMinusBdgtAmount = decimal.Parse(reader[0].ToString()) + budgetAmount;
                decimal newBudgetAmount = reqAmount / exRate;

                if (currBudgetMinusBdgtAmount < newBudgetAmount)
                {
                    return false; ;
                }
            }
            return true;
        }

        private void InitOleCommand(ref OleDbCommand cmd, in Invoice invoice)
        {
            _ = cmd.Parameters.AddWithValue("@CompanyName", invoice.CompanyName.Trim());
            _ = cmd.Parameters.AddWithValue("@CompanyAddress", invoice.CompanyAddress.Trim());

            _ = cmd.Parameters.AddWithValue("@BankName", invoice.BankName.Trim());
            _ = cmd.Parameters.AddWithValue("@IBANNu", invoice.IBANNu.Trim());
            _ = cmd.Parameters.AddWithValue("@InvNu", invoice.InvNu.Trim());

            _ = cmd.Parameters.AddWithValue("@InvDate", invoice.InvDate.ToString("d"));
            _ = cmd.Parameters.AddWithValue("@PayAmount", invoice.PayAmount.ToString());
            _ = cmd.Parameters.AddWithValue("@PayCurr", invoice.PayCurr.Trim());

            _ = cmd.Parameters.AddWithValue("BdgtCurr", invoice.BdgtCurr.Trim());
            _ = cmd.Parameters.AddWithValue("BdgtAmount", invoice.BdgtAmount.ToString(curFormat));

            _ = cmd.Parameters.AddWithValue("@MainID", invoice.MainID.Trim());
        }

        public bool SaveData(Invoice _invoice)
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "INSERT INTO DisPaymentItem (PaymentListId, CompanyName, CompanyAddress, BankName, IBANNu, InvNu, InvDate, PayAmount, PayCurr, BdgtCurr, BdgtAmount, MainID, EncumbID)" +
                    " VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
                };

                _ = cmd.Parameters.AddWithValue("@PaymentListId", _invoice.PaymentListId.Trim());
                InitOleCommand(ref cmd, in _invoice);
                _ = cmd.Parameters.AddWithValue("@EncumbID", _invoice.EncumbId.Trim());

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
                    _ = MessageBox.Show("Error:InvoiceMain:SaveData:" + ex.Message);
                    return false;
                }
            }

            return false;
        }


        public bool UpdateData()
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "UPDATE DisPaymentItem SET CompanyName = ?, CompanyAddress = ?, BankName = ?, IBANNu = ?, " +
                    "InvNu = ?, InvDate = ?, PayAmount = ?, PayCurr = ?, BdgtCurr = ?, BdgtAmount = ?, MainID = ?  WHERE ID = ?",
                };

                #region InitCommonParameters

                InitOleCommand(ref cmd, in _invoice);
                _ = cmd.Parameters.AddWithValue("@ID", _invoice.ID.Trim());

                #endregion

                try
                {
                    con.Open();

                    if (!IsBudgetEnough(_invoice.MainID, _invoice.BdgtAmount, _invoice.ExRate, _invoice.PayAmount, con))
                    {
                        _ = MessageBox.Show("Budget Amount is not enough!");
                        return false;
                    }

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
                    _ = MessageBox.Show("Error:InvoiceMain:UpdateData:" + ex.Message);
                    return false;
                }
            }
            return false;
        }

        public bool DeleteData(Invoice invoice)
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "DELETE FROM DisPaymentItem WHERE ID = ?",
                };

                cmd.Parameters.AddWithValue("@ID", invoice.ID);

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
                    _ = MessageBox.Show("InvoiceMain:DeleteData:" + ex.Message);
                    return false;
                }
            }
            return true;
        }

        public bool DeleteFromDisbursingApprove(Invoice invoice)
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                using (OleDbCommand cmd = new OleDbCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "DELETE FROM DisbursingApprove WHERE InvCreditor = ? AND InvNu = ? AND InvAmount = ? AND SendTo = 'Disbursing'";

                    cmd.Parameters.AddWithValue("@ReqDesc", invoice.CompanyName);
                    cmd.Parameters.AddWithValue("@InvNu", invoice.InvNu);
                    cmd.Parameters.AddWithValue("@InvAmount", invoice.PayAmount.ToString());

                    OleDbDataAdapter adapter = new OleDbDataAdapter()
                    {
                        DeleteCommand = cmd,
                    };

                    con.Open();

                    if (!(adapter.DeleteCommand.ExecuteNonQuery() > 0))
                    {
                        return false;
                    }
                }

                using (OleDbCommand cmd = new OleDbCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "UPDATE DisbursingApprove SET ApproveChoice = 'Empty' WHERE InvCreditor = ? AND InvNu = ? AND InvAmount = ? AND SendTo = 'Fiscal'";

                    cmd.Parameters.AddWithValue("@ReqDesc", invoice.CompanyName);
                    cmd.Parameters.AddWithValue("@InvNu", invoice.InvNu);
                    cmd.Parameters.AddWithValue("@InvAmount", invoice.PayAmount.ToString());

                    OleDbDataAdapter adapter = new OleDbDataAdapter()
                    {
                        UpdateCommand = cmd,
                    };

                    if (!(adapter.UpdateCommand.ExecuteNonQuery() > 0))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
