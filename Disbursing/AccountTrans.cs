using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace G8FinApp.Disbursing
{
    public class AccountTrans : INotifyPropertyChanged
    {
        public AccountTrans()
        {

        }
        public string ID { get; set; }
        public string AccountId { get; set; }
        public decimal TransAmount { get; set; }
        public DateTime TransDate { get; set; }
        public string TransRemark { get; set; }
        public string CashBookId { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropetyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
