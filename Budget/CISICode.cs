using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace G8FinApp.Budget
{
    public class CISICode : INotifyPropertyChanged
    {
        public CISICode(string id, string cisicode, string cisidesc)
        {
            ID = id;
            CISICODE = cisicode;
            CISIDESC = cisidesc;
        }

        public string ID { get; set; }
        public string CISICODE { get; set; }
        public string CISIDESC { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropetyChanged(string info)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
