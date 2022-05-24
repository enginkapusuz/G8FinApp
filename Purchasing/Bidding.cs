using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace G8FinApp.Purchasing
{
    public class Bidding : INotifyPropertyChanged
    {
        public string ID { get; set; }
        public string ApproveId { get; set; }
        public string BiddingName { get; set; }
        public string CisiCode { get; set; }
        public string PendingNo { get; set; }
        public string Lines { get; set; }
        public DateTime BiddingOpenDate { get; set; }
        public DateTime BiddingCloseDate { get; set; }
        public decimal BiddingPrice { get; set; }
        public string BiddingCurr { get; set; }
        public string PointOfContact { get; set; }
        public string PofConPhone { get; set; }
        public string FMNo { get; set; }
        public int LastContractNu { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropetyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
