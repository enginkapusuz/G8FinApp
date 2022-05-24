using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace G8FinApp.Disbursing
{
    public class AccountTransaction : INotifyPropertyChanged
    {
        public AccountTransaction()
        {

        }

        #region TransactionFields
        public string ID { get; set; }        
        public decimal TransAmount { get; set; }
        public DateTime TransDate { get; set; }
        public string TransRemark { get; set; }

        #endregion


        #region AccountFields

        public string AccountId { get; set; }
        public string AccountName { get; set; }
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
