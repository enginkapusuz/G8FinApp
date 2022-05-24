using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace G8FinApp.Budget
{
    public class Currency : INotifyPropertyChanged
    {
        public Currency(string id, string currname, string currcode, string currsym)
        {
            ID = id;
            CURRNAME = currname;
            CURRCODE = currcode;
            CURRSYM = currsym;
            UNIONOFCURR = $"{CURRCODE} - {CURRSYM} -{CURRNAME}";
        }

        public string ID { get; set; }
        public string CURRNAME { get; set; }
        public string CURRCODE { get; set; }
        public string CURRSYM { get; set; }

        public string UNIONOFCURR { get; set; }

        public override string ToString() => $"{ID} {{ {CURRNAME} {{ {CURRCODE} {{ {CURRSYM}";

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropetyChanged(string info)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
