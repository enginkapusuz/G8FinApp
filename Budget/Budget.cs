using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace G8FinApp.Budget
{
    public class Budget : INotifyPropertyChanged
    {
        private string _id;
        private int _fmNo;
        private string _fmName;
        private string _cisiCode;
        private string _cisiDesc;
        private string _bdgtCurr;
        private decimal _inAmount;
        private decimal _outAmount;
        private decimal _currAmount;

        public Budget()
        {

        }
        public string ID
        {
            get { return _id; }
            set { _id = value; OnPropetyChanged("Id"); }
        }

        public int FMNO
        {
            get { return _fmNo; }
            set { _fmNo = value; OnPropetyChanged("Fm No"); }
        }

        public string FMNAME
        {
            get { return _fmName; }
            set { _fmName = value; OnPropetyChanged("Fm Name"); }
        }

        public string CISICODE
        {
            get { return _cisiCode; }
            set { _cisiCode = value; OnPropetyChanged("Cisi Code"); }
        }
        public string CISIDESC
        {
            get { return _cisiDesc; }
            set { _cisiDesc = value; OnPropetyChanged("Cisi Desc"); }
        }

        public string BDGTCURR
        {
            get { return _bdgtCurr; }
            set { _bdgtCurr = value; OnPropetyChanged("BudgetCurr"); }
        }

        public decimal CURRAMOUNT
        {
            get { return _currAmount; }
            set { _currAmount = value; }
        }

        public decimal INAMOUNT
        {
            get { return _inAmount; }
            set { _inAmount = value; }
        }

        public decimal OUTAMOUNT
        {
            get { return _outAmount; }
            set { _outAmount = value; }
        }

        public override string ToString() => $"{_id} {{ {_fmNo} {{ {_fmName} {{ {_cisiCode} {{ {_cisiDesc} {{ {_bdgtCurr}";

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropetyChanged(string info)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
