using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace G8FinApp.Fiscal
{
    public class CashCall : INotifyPropertyChanged
    {
        public string ID { get; set; }
        public string FiscalCountriesID { get; set; }
        public string CountryName { get; set; }
        public string Remarks { get; set; }
        public decimal PlannedPayment { get; set; }
        public DateTime PlannedPaymentDate { get; set; }
        public string CommitNumber { get; set; }
        public DateTime CommitDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
        public string InvoiceNumber { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropetyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
