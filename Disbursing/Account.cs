using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace G8FinApp.Disbursing
{
    public class Account : INotifyPropertyChanged
    {
        public string ID { get; set; }
        public string AccountName { get; set; }
        public string AccountNu { get; set; }
        public string AccountCurr { get; set; }
        public decimal AccInTotal { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public string AccountString
        {
            get { return AccountNu + "-" + AccountName + "-" + AccountCurr; }
        }

        protected void OnPropetyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
