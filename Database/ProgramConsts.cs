using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G8FinApp.Database
{
    public class ProgramConsts
    {
        public readonly string connectionString = "Provider=Microsoft.Jet.Oledb.4.0; " +
            "Data Source=data/Fin8Database.mdb;User Id=admin; " +
            "Jet OLEDB:Database Password=eng*1978*3037";

        public readonly string curFormat = "#,#.0000";
    }
}
