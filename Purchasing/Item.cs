using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace G8FinApp.Purchasing
{
    public class Item: INotifyPropertyChanged
    {

        public string ID { get; set; }
        public string BiddingId { get; set; }
        public string ItemNu { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public float Quantity { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropetyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
