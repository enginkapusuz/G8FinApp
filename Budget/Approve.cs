using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace G8FinApp.Budget
{
    public class Approve : INotifyPropertyChanged
    {
        private string _mainId;
        private string _encumbId;
        private string _cisicode;
        private string _fmno;
        private string _tableName;
        private string _sendTo;
        private string _approveChoice;
        private string _appDate;


        private string _fmName;
        private string _reqDesc;
        private decimal _reqAmount;
        private string _reqCurr;

        private string _bdgtCurr;
        private decimal _bdgtAmount;


        public Approve()
        {

        }

        public Approve(string mainid, string encumbid, string tablename,
            string sendto, string approvechoice, string appdate)
        {
            _mainId = mainid;
            _encumbId = encumbid;
            _tableName = tablename;
            _sendTo = sendto;
            _approveChoice = approvechoice;
            _appDate = appdate;

        }
        public string CISICODE
        {
            get => _cisicode;
            set => _cisicode = value;
        }

        public string FMNO
        {
            get => _fmno;
            set => _fmno = value;
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

        public string TABLENAME
        {
            get => _tableName;
            set => _tableName = value;
        }

        public string SENDTO
        {
            get => _sendTo;
            set => _sendTo = value;
        }

        public string APPROVECHOICE
        {
            get => _approveChoice;
            set => _approveChoice = value;
        }
        public string APPDATE
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropetyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
