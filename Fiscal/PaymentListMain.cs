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
    public class PaymentListMain : ObservableCollection<PaymentList>
    {
        private const string curFormat = "#,#.00";
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();

        public PaymentListMain()
        {
            InitList();
        }

        public PaymentListMain(bool isCollection)
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
                    CommandText = "SELECT * FROM FiscalPaymentList WHERE ListSituation = 'NOTPAID'",
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
                            DisPaymentListId = reader[5].ToString(),
                        };

                        Add(paymentList);
                    }
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show("Error:PaymentListMain:InitList:" + ex.Message);
                    return;
                }
            }
        }

        private bool FiscalCommitUpdate(OleDbConnection con, string tblName, string encumbId, decimal newBdgtAmount)
        {
            string comText;

            if (tblName == "FiscalCommit")
            {
                comText = "UPDATE FiscalCommit SET BdgtAmount = ? WHERE EncumbId = ?";
            }
            else
            {
                comText = "UPDATE BudgetEncumbrance SET Amount = ? WHERE ID = ?";
            }

            OleDbCommand cmd = new OleDbCommand()
            {
                Connection = con,
                CommandType = System.Data.CommandType.Text,
                CommandText = comText
            };

            if (tblName == "FiscalCommit")
            {
                cmd.Parameters.AddWithValue("@BdgtAmount", newBdgtAmount.ToString());
                cmd.Parameters.AddWithValue("@EncumbId", encumbId);
            }
            else
            {
                cmd.Parameters.AddWithValue("@Amount", newBdgtAmount.ToString());
                cmd.Parameters.AddWithValue("@ID", encumbId);
            }

            OleDbDataAdapter adapter = new OleDbDataAdapter()
            {
                UpdateCommand = cmd,
            };

            if (adapter.UpdateCommand.ExecuteNonQuery() > 0)
            {
                return true;
            }

            return false;
        }

        private bool ReadFiscalPaymentApprove(OleDbConnection con, string encumbId)
        {
            OleDbCommand cmd = new OleDbCommand()
            {
                Connection = con,
                CommandType = System.Data.CommandType.Text,
                CommandText = "SELECT EncumbId, NewBdgtAmount FROM FiscalPaymentApprove WHERE EncumbId = ?"
            };

            cmd.Parameters.AddWithValue("@FC.EncumbId", encumbId);

            OleDbDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                FiscalCommitUpdate(con, "FiscalCommit", reader[0].ToString(), decimal.Parse(reader[1].ToString()));
                FiscalCommitUpdate(con, "BudgetEncumbrance", reader[0].ToString(), decimal.Parse(reader[1].ToString()));
            }
           
            return false;
        }

        private bool UpdateFiscalCommitWrap(OleDbConnection con, string disPaymentListID)
        {
            OleDbCommand cmd = new OleDbCommand()
            {
                Connection = con,
                CommandType = System.Data.CommandType.Text,
                CommandText = "SELECT PaymentListId, EncumbID FROM DisPaymentItem WHERE PaymentListId = ?",
            };

            _ = cmd.Parameters.AddWithValue("@PaymentListId", disPaymentListID);

            OleDbDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ReadFiscalPaymentApprove(con, reader[1].ToString());
            }

            return true;
        }

        public bool UpdateData(PaymentList paymenList)
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "UPDATE FiscalPaymentList SET ListSituation = ? WHERE ID = ?",
                };

                cmd.Parameters.AddWithValue("@ListSituation", paymenList.ListSituation);
                cmd.Parameters.AddWithValue("@ID", paymenList.ID);

                try
                {
                    con.Open();

                    _ = UpdateFiscalCommitWrap(con, paymenList.DisPaymentListId);

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
    }
}
