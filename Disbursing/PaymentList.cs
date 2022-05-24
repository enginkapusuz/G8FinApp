using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace G8FinApp.Disbursing
{
    public class PaymentList : INotifyPropertyChanged
    {
        public string ID { get; set; }
        public string ListName { get; set; }
        public string ListNu { get; set; }
        public DateTime ListDate { get; set; }
        public string ListSituation { get; set; }

        public override string ToString() => ListName + "-" + ListNu + "-" + ListDate.ToString("d") ;
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropetyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
