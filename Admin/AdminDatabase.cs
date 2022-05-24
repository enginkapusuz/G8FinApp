using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G8FinApp.Admin
{
    public class AdminDatabase
    {
        public string ConnectStringSRD2022 {get; private set;}
        public AdminDatabase()
        {
            ConnectStringSRD2022 = "Provider=Microsoft.Jet.Oledb.4.0; Data Source = DatabasesForFirstInit/SRD2022-01.mdb";
        }
    }
}
