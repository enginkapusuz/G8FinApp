using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace G8FinApp.Disbursing
{
    public class CashCurrentAmount
    {

        public List<Account> CalculateCurrrentAmount(CashBookMain cashBookMain)
        {
            List<Account> accountList = new List<Account>();

            var income = from trans in cashBookMain
                         where trans.TransRemark == "TransferIn" || trans.TransRemark == "Received"
                         group trans by trans.AccountNu into g
                         select new { AccountNu = g.Key, AccountTotal = g.Select(trans => trans.AccInTotal).First(), AccountIncome = g.Sum(trans => trans.TransAmount) };

            var outcome = from trans in cashBookMain
                          where trans.TransRemark == "TransferOut" || trans.TransRemark == "Payment"
                          group trans by trans.AccountNu into g
                          select new { AccountNu = g.Key, AccountTotal = g.Select(trans => trans.AccInTotal).First(), AccountOutcome = g.Sum(trans => trans.TransAmount) };

            foreach(var trans in cashBookMain.Select(trans => trans.AccountNu).Distinct())
            {
                decimal accountBalance = income
                    .Where(inTrans => inTrans.AccountNu == trans)
                    .Select(inTrans => inTrans.AccountTotal)
                    .FirstOrDefault();

                if (accountBalance == 0)
                {
                    accountBalance = outcome
                        .Where(outTrans => outTrans.AccountNu == trans)
                        .Select(outTrans => outTrans.AccountTotal)
                        .FirstOrDefault();
                }

                accountBalance += income
                    .Where(inTrans => inTrans.AccountNu == trans)
                    .Select(inTrans => inTrans.AccountIncome).FirstOrDefault();

                accountBalance -= outcome
                    .Where(inTrans => inTrans.AccountNu == trans)
                    .Select(inTrans => inTrans.AccountOutcome).FirstOrDefault();

                accountList.Add(new Account { AccInTotal = accountBalance, AccountNu = trans });
            }

            return accountList;
        }
    }
}
