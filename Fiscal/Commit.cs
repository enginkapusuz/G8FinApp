using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace G8FinApp.Fiscal
{
    public class Commit : INotifyPropertyChanged
    {
        private string _id;
        private string _approveId;
        private string _fmNo;

        private string _cisiCode;
        private string _mainId;
        private string _encumbId;
        private string _reqDesc;

        private string _fmName;
        private decimal _reqAmount;
        private string _reqCurr;

        private DateTime _appDate;
        private string _commitNo;
        private string _actCode;

        private string _bdgtCurr;
        private decimal _bdgtAmount;

        public Commit()
        {

        }

        public Commit(string id, string commitnu, DateTime appdate, 
            decimal reqamount, string reqcurr)
        {
            _id = id;
            _commitNo = commitnu;
            _appDate = appdate;

            _reqAmount = reqamount;
            _reqCurr = reqcurr;
        }

        public string APPROVEID
        {
            get => _approveId;
            set => _approveId = value;
        }
        public string ACTCODE
        {
            get => _actCode;
            set => _actCode = value;
        }
        public string ID
        {
            get => _id;
            set => _id = value;
        }
        public string CISICODE
        {
            get => _cisiCode;
            set => _cisiCode = value;
        }
        public string FMNO
        {
            get => _fmNo;
            set => _fmNo = value;
        }
        public string COMMITNO
        {
            get => _commitNo;
            set => _commitNo = value;
        }

        public string REQCURR
        {
            get => _reqCurr;
            set => _reqCurr = value;
        }
        public decimal REQAMOUNT
        {
            get => _reqAmount;
            set => _reqAmount = value;
        }
        public string REQDESC
        {
            get => _reqDesc;
            set => _reqDesc = value;
        }
        public string FMNAME
        {
            get => _fmName;
            set => _fmName = value;
        }

        public string MAININD
        {
            get => _mainId;
            set => _mainId = value;
        }

        public string ENCUMID
        {
            get => _encumbId;
            set => _encumbId = value;
        }
        public DateTime APPDATE
        {
            get => _appDate;
            set => _appDate = value;
        }

        public string BDGTCURR
        {
            get => _bdgtCurr;
            set => _bdgtCurr = value;
        }

        public decimal BDGTAMOUNT
        {
            get => _bdgtAmount;
            set => _bdgtAmount = value;
        }

        public decimal EXRATE { get; set; }

        public override string ToString()
        {
            return FMNAME + "-" + REQDESC + "-" + REQAMOUNT + "-" + REQCURR;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropetyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
