using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using G8FinApp.Reports;
using G8FinApp.Budget;

namespace G8FinApp.Admin
{
    public class AdminCisiCode
    {
        public List<CISICode> ListCisiCode { get; private set; }
        
        public AdminCisiCode LoadBudgetData(ReportExcel budgetSheet)
        {
            AdminCisiCode adminCisiCode = new AdminCisiCode();
            adminCisiCode.ListCisiCode = new List<CISICode>();
            return adminCisiCode;
        }

    }
}
