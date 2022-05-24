﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace G8FinApp.Disbursing
{
    public class Invoice : INotifyPropertyChanged
    {
        public Invoice()
        {

        }

        public string ID { get; set; }
        public string PaymentListId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string BankName { get; set; }
        public string IBANNu { get; set; }
        public string InvNu { get; set; }
        public DateTime InvDate { get; set; }
        public decimal PayAmount { get; set; }
        public string PayCurr { get; set; }
        public string BdgtCurr { get; set; }
        public decimal BdgtAmount { get; set; }

        public string MainID { get; set; }
        public string EncumbId { get; set; }

        public decimal ExRate { get; set; }

        public override string ToString()
        {
            return PaymentListId + "-" + CompanyName + "-" + CompanyAddress + "-" + BankName + "-" + IBANNu + "-" + InvNu + "-" + InvDate + "-" + PayAmount + "-" + PayCurr;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropetyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
