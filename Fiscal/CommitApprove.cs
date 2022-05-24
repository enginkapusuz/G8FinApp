using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;


namespace G8FinApp.Fiscal
{
    public class CommitApprove : INotifyPropertyChanged
    {
        public string CommitId { get; set; }
        public string MainId { get; set; }
        public string EncumbId { get; set; }
        public string CommitNu { get; set; }
        public DateTime CommitDate { get; set; }
        public decimal CommitAmount { get; set; }
        public string CommitCurr { get; set; }
        public string CommitActCode { get; set; }

        public string ApproveId { get; set; }
        public List<string> ApproveSendTo { get; set; }
        public List<string> ApproveChoice { get; set; }
        public List<DateTime> ApproveDate { get; set; }

        public string FmName { get; set; }
        public string ReqDesc { get; set; }

        public decimal ReqAmount { get; set; }
        public string ReqCurr { get; set; }
        public string BdgtCurr { get; set; }
        public decimal BdgtAmount { get; set; }

        public decimal TotInvAmount { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropetyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
