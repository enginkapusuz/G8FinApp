using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Windows;


namespace G8FinApp.Admin
{
    public class Invoice
    {
        public string CommitNo { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceReferance { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string Currency { get; set; }
        public decimal InvoiceAmount { get; set; }
        public string ActualCurrency { get; set; }
        public decimal ActualAmount { get; set; }
        public string FiscalVendorName { get; set; }
    }
    public class AdminInvoice
    {
        public AdminInvoice()
        {

        }

        public List<Invoice> LoadInvoices()
        {
            List<Invoice> invoices = null;

            string connectString = new AdminDatabase().ConnectStringSRD2022;
            using(OleDbConnection con = new OleDbConnection(connectString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM SubFiscal WHERE ActualCurrency IS NOT NULL AND ActualAmount > 0 AND LEFT(CommitNo, 2) = '22' ORDER BY RIGHT(CommitNo, 4)",
                };

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        Invoice invoice = new Invoice()
                        {
                            CommitNo = reader["CommitNo"].ToString(),
                            InvoiceReferance = reader["InvoiceReferance"].ToString(),
                            InvoiceNumber = reader["InvoiceNumber"].ToString(),
                            Currency = reader["Currency"].ToString(),
                            ActualCurrency = reader["ActualCurrency"].ToString(),
                            ActualAmount = decimal.Parse(reader["ActualAmount"].ToString()),
                            InvoiceAmount = decimal.Parse(reader["InvoiceAmount"].ToString()),
                            FiscalVendorName = reader["FiscalVendorName"].ToString(),
                            InvoiceDate = DateTime.Parse(reader["InvoiceDate"].ToString()),
                        };

                        if (invoices is null)
                        {
                            invoices = new List<Invoice>();
                        }

                        invoices.Add(invoice);
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("AdminInvoices:LoadInvoices:" + ex.Message);
                    return null;
                }
            }
            return invoices;
        }
    }
}
