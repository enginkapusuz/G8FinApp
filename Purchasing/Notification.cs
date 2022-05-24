using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace G8FinApp.Purchasing
{
    public class Notification : INotifyPropertyChanged
    {

        public string ID { get; set; }
        public DateTime BidDate { get; set; }
        public string BiddingId { get; set; }
        public decimal BidAmount { get; set; }
        public string CompanyId { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropetyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
