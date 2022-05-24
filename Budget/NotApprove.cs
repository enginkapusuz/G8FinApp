using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace G8FinApp.Budget
{
    public class NotApprove: INotifyPropertyChanged
    {
        public string ID { get; set; }
        public string MainId { get; set; }
        public string EncumbId { get; set; }
        public string TableName { get; set; }
        public string SendTo { get; set; }
        public string ApproveChoice { get; set; }
        public DateTime AppDate { get; set; }
        public string FmName { get; set; }
        public string ReqDesc { get; set; }

        public decimal ReqAmount { get; set; }
        public string ReqCurr { get; set; }
        public string BdgtCurr { get; set; }
        public decimal BdgtAmount { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropetyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
