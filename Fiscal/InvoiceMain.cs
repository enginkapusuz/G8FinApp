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
    public class InvoiceMain : ObservableCollection<Invoice>
    {
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();
        public bool SaveData(Invoice invoice)
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "INSERT INTO FiscalInvoice (CommitID, InvoiceNu, InvoiceDate, InvoiceAmount, Creditor)" +
                    " VALUES(@CommitID, @InvoiceNu, @InvoiceDate, @InvoiceAmount, @Creditor)"
                };

                _ = cmd.Parameters.AddWithValue("@CommitId", invoice.COMMITID.Trim());
                _ = cmd.Parameters.AddWithValue("@InvoiceNu", invoice.INVOICENU.Trim());
                _ = cmd.Parameters.AddWithValue("@InvoiceDate", invoice.INVOICEDATE);
                _ = cmd.Parameters.AddWithValue("@InvoiceAmount", invoice.INVOICEAMOUNT.ToString("N4"));
                _ = cmd.Parameters.AddWithValue("@Creditor", invoice.CREDITOR.Trim());

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
    }
}
