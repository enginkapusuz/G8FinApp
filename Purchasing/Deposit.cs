using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace G8FinApp.Purchasing
{
    public class Deposit : INotifyPropertyChanged
    {
        public int ID { get; set; }
        public int ContractId { get; set; }
        public string BiddingName { get; set; }
        public DateTime DepositDate { get; set; }
        public decimal PurchasingDepositAmount { get; set; }
        public decimal DepositRate { get; set; }
        public string DepositCurrency { get; set; }
        public string Company { get; set; }
        public decimal BiddingPrice { get; set; }
        public string BiddingCurr { get; set; }
        public DateTime DisbAppDate { get; set; }
        public decimal DisbDepositAmount { get; set; }
        public string CommitNu { get; set; }
        public DateTime CommitDate { get; set; }
        public string ContractNo { get; set; }
        public DateTime PCReturnDate { get; set; }
        public decimal PCReturnAmount { get; set; }
        public DateTime DisbReturnDate { get; set; }
        public decimal DisbReturnAmount { get; set; }
        public DateTime FiscalReturnDate { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropetyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
