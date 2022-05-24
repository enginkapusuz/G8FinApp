using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace G8FinApp.Disbursing
{
    public class BookPayment : INotifyPropertyChanged
    {
        public string ID { get; set; }
        public string Description { get; set; }
        public DateTime PaymentBookDate { get; set; }

        public string BookCashID { get; set; }

        public string DisPaymentListId { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropetyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
