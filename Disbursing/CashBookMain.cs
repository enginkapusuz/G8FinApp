using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Windows;

namespace G8FinApp.Disbursing
{
    public class CashBookMain : ObservableCollection<AccountTransaction>
    {
        public CashBookMain()
        {

        }

        public void TakeAccountsTotal(InvoiceAccountMain invoiceAccountMain)
        {
            AccountTransaction accountTransaction;

            var accountGroup = from invAcc in invoiceAccountMain
                               group invAcc by invAcc.AccountNu into g
                               let accTotal = g.Sum(ac => ac.PayAmount)
                               let accCurr = g.Select(ac => ac.PayCurr).First()
                               let accId = g.Select(ac => ac.AccountId).First()
                               let accCurrentAmount = g.Select(ac => ac.AccInTotal).First()
                               select new { AccountNu = g.Key, AccTotal = accTotal, AccCurr = accCurr, AccId = accId, AccCurrentAmount = accCurrentAmount};

            foreach(var g in accountGroup)
            {
                accountTransaction = new AccountTransaction()
                {
                    AccountId = g.AccId,
                    AccountNu = g.AccountNu,
                    AccountCurr = g.AccCurr,
                    TransAmount = g.AccTotal,
                    AccInTotal = g.AccCurrentAmount,
                    TransRemark = "Payment",
                };
                Add(accountTransaction);
            }
        }
    }
}
