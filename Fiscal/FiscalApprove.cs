using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace G8FinApp.Fiscal
{
    public class FiscalApprove : INotifyPropertyChanged
    {
        public FiscalApprove()
        {

        }

        public string ID { get; set; }
        public string REQCURR { get; set; }
        public decimal REQAMOUNT { get; set; }
        public string REQDESC { get; set; }
        public string FMNO { get; set; }
        public string FMNAME { get; set; }
        public string CISICODE { get; set; }
        public string CISIDESC { get; set; }
        public string MAININD { get; set; }
        public string ENCUMID { get; set; }
        public string TABLENAME { get; set; }
        public string SENDTO { get; set; }
        public string APPROVECHOICE { get; set; }
        public DateTime APPDATE { get; set; }
        public string BDGTCURR { get; set; }
        public decimal BDGTAMOUNT { get; set; }
        public int LASTCOMID { get; set; }

        public string PENDINGNO { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropetyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
