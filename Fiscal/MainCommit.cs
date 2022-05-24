using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace G8FinApp.Fiscal
{
    public class MainCommit : INotifyPropertyChanged
    {
        public string ID { get; set; }
        public string CommitNu { get; set; }
        public DateTime CommitDate { get; set; }
        public string TableName { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropetyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
