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
    public class InvoiceAccountMain : ObservableCollection<InvoiceAccount>
    {
        public InvoiceAccountMain()
        {

        }

        public void AddItem(InvoiceAccount invoiceAccount)
        {
            Add(invoiceAccount);
        }
    }
}
