using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace G8FinApp.Disbursing
{
    public class InvoiceAccount : INotifyPropertyChanged
    {
        #region IDs
        
        public string ID { get; set; }
        public string InvoiceId { get; set; }
        public string PaymentListId { get; set; }
        public string AccountId { get; set; }

        #endregion
        
        #region InvoiceFields

        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string InvNu { get; set; }
        public DateTime InvDate { get; set; }
        public decimal PayAmount { get; set; }
        public string PayCurr { get; set; }

        #endregion


        #region AcountFields

        public string AccountNu { get; set; }
        public string AccountCurr { get; set; }
        public decimal AccInTotal { get; set; }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropetyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }

    }
}
