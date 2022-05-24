using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace G8FinApp.Fincon
{
    public class FinconApprove : INotifyPropertyChanged
    {
        private string _id;
        private string _mainId;
        private string _encumbId;
        private string _tableName;
        private string _sendTo;
        private string _approveChoice;
        private DateTime _appDate;

        private string _fmName;
        private string _reqDesc;
        private decimal _reqAmount;
        private string _reqCurr;

        private string _bdgtCurr;
        private decimal _bdgtAmount;
        private string _pendingNo;

        public FinconApprove()
        {

        }

        public string ID
        {
            get => _id;
            set => _id = value;
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

        public string PENDINGNO
        {
            get => _pendingNo;
            set => _pendingNo = value;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropetyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
