using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace G8FinApp.Fiscal
{
    public class Invoice : INotifyPropertyChanged
    {
        public Invoice()
        {
        }
        public string ID { get; set; }
        public string COMMITID { get; set; }

        public decimal COMMITAMOUNT { get; set; }
        public string INVOICENU { get; set; }
        public DateTime INVOICEDATE { get; set; }
        public decimal INVOICEAMOUNT { get; set; }
        public string CREDITOR { get; set; }

        public decimal TOTINVAMOUNT { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropetyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
