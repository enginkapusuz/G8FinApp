using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace G8FinApp.Budget
{
    public class BudgetDetail : INotifyPropertyChanged
    {
        private string _id;
        private string _budgetmainid;
        private string _fmNo;
        private string _fmName;
        private string _cisiCode;
        private string _bdgtCurr;
        private decimal _amount;
        private string _tdate;
        private string _docnu;
        private string _tblname;
        private string _budgetEncmbDataID;

        public BudgetDetail()
        {

        }

        public BudgetDetail(string id, string fmno, string fmname,
            string cisicode, string bdgtcurr, decimal amount,
            string tdate, string docnu, string tblname)
        {
            _id = id;
            _fmNo = fmno;
            _fmName = fmname;
            _cisiCode = cisicode;
            _bdgtCurr = bdgtcurr;
            _amount = amount;
            _tdate = tdate;
            _docnu = docnu;
            _tblname = tblname;
        }

        public string ID
        {
            get { return _id; }
            set { _id = value; OnPropetyChanged("Id"); }
        }

        public string BUDGETMAINID
        {
            get { return _budgetmainid; }
            set { _budgetmainid = value; }
        }

        public string DETAILID
        {
            get;
            set;
        }

        public string FMNO
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

        public string BDGTCURR
        {
            get { return _bdgtCurr; }
            set { _bdgtCurr = value; OnPropetyChanged("BudgetCurr"); }
        }

        public decimal AMOUNT
        {
            get { return _amount; }
            set { _amount = value; OnPropetyChanged("Amount"); }
        }
        public string TDATE
        {
            get { return _tdate; }
            set { _tdate = value; OnPropetyChanged("TDate"); }
        }

        public string DOCNU
        {
            get { return _docnu; }
            set { _docnu = value; OnPropetyChanged("Docnu"); }
        }

        public string TABLENAME
        {
            get { return _tblname; }
            set { _tblname = value; OnPropetyChanged("Docnu"); }
        }

        public string BDGTENCMBDATAID
        {
            get { return _budgetEncmbDataID; }
            set { _budgetEncmbDataID = value; }
        }

        public override string ToString() => $"{_id} {{ {_fmNo} {{ {_fmName} {{ {_cisiCode}  {{ {_bdgtCurr}  {{ {_tblname}";

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropetyChanged(string info)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
