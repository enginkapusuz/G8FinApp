using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace G8FinApp.Fiscal
{
    public class Pending : INotifyPropertyChanged
    {
        private string _id;
        private string _approveId;
        private string _fmNo;

        private string _sendto;
        private string _cisiCode;
        private string _mainId;
        private string _encumbId;
        private string _reqDesc;

        private string _fmName;
        private decimal _reqAmount;
        private string _reqCurr;

        private DateTime _appDate;
        private string _pendingNo;
        private string _actCode;

        private string _approveChoice;
        private string _bdgtcurr;
        private decimal _bdgtamount;

        public Pending()
        {

        }

        public decimal BDGTAMOUNT
        {
            get => _bdgtamount;
            set => _bdgtamount = value;
        }

        public string BDGTCURR
        {
            get => _bdgtcurr;
            set => _bdgtcurr = value;
        }

        public string SENDTO
        {
            get => _sendto;
            set => _sendto = value;
        }
        public string APPROVECHOICE
        {
            get => _approveChoice;
            set => _approveChoice = value;
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
        public string PENDINGNO
        {
            get => _pendingNo;
            set => _pendingNo = value;
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

        public string MAINID
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
