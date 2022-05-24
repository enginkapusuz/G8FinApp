using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace G8FinApp.Purchasing
{
    public class Contract :INotifyPropertyChanged
    {
        public int ID { get; set; }
        public string BiddingId { get; set; }
        public DateTime PcDate { get; set; }
        public decimal PcAmount { get; set; }
        public string BiddingCurr { get; set; }
        public string Company { get; set; }
        public string ContractNo { get; set; }
        public string BiddingName { get; set; }
        public string PendingNo { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropetyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
