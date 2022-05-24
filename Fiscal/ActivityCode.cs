using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace G8FinApp.Fiscal
{
    public class ActivityCode : INotifyPropertyChanged
    {
        public ActivityCode()
        {

        }

        public ActivityCode(string id, string actcode, string actdesc)
        {
            ID = id;
            ACTCODE = actcode;
            ACTDESC = actdesc;
        }

        public string ID { get; set; }
        public string ACTCODE { get; set; }
        public string ACTDESC { get; set; }

        public override string ToString() => ACTCODE;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropetyChanged(string info)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
