using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace G8FinApp.Budget
{
    public class BudgetData : INotifyPropertyChanged
    {
        private string _id;
        private string _reqDesc;
        private string _reqNum;
        private string _reqItemCount;
        private string _reqCurr;
        private decimal _reqAmount;
        private string _reqDate;
        private string _docNu;

        public BudgetData()
        {

        }

        public BudgetData(string id, string reqDesc, string reqNum,
            string reqItemCount, string reqCurr, decimal reqAmount,
            string reqDate, string docNu)
        {
            _id = id;
            _reqDesc = reqDesc;
            _reqNum = reqNum;
            _reqItemCount = reqItemCount;
            _reqCurr = reqCurr;
            _reqAmount = reqAmount;
            _reqDate = reqDate;
            _docNu = docNu;
        }

        public string ID
        {
            get { return _id; }
            set { _id = value; OnPropetyChanged("Id"); }
        }
        public string REQDESC
        {
            get { return _reqDesc; }
            set { _reqDesc = value;OnPropetyChanged("ReqDesc");}
        }
        public string REQNUM
        {
            get { return _reqNum; }
            set { _reqNum = value; OnPropetyChanged("ReqNum"); }
        }
        public string REQITEMCOUNT
        {
            get { return _reqItemCount; }
            set { _reqItemCount = value; OnPropetyChanged("ReqItemCount"); }
        }

        public string REQCURR
        {
            get { return _reqCurr; }
            set { _reqCurr = value; OnPropetyChanged("ReqCurr"); }
        }
        public decimal REQAMOUNT
        {
            get { return _reqAmount; }
            set { _reqAmount = value; OnPropetyChanged("ReqAmount"); }
        }
        public string REQDATE
        {
            get { return _reqDate; }
            set { _reqDate = value; OnPropetyChanged("Date"); }
        }
        public string REQDOCNU
        {
            get { return _docNu; }
            set { _docNu = value; OnPropetyChanged("DocNu"); }
        }

        public override string ToString() => $"{REQNUM}";

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropetyChanged(string info)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
