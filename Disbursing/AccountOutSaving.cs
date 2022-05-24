using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace G8FinApp.Disbursing
{
    public class AccountOutSaving
    {
        private readonly AccountMain accountMain = new AccountMain();

        public bool IsAccountAmountEnough(AccountTrans accountTrans)
        {
            decimal accCurrentAmount = accountMain.Where(acc => acc.ID == accountTrans.AccountId)
                .Select(acc => acc.AccInTotal).FirstOrDefault();

            if (accountTrans.TransAmount > accCurrentAmount)
            {
                return false;
            }
            return true;
        }

        public bool SaveAccountOutData(AccountTrans accountTrans)
        {
            AccountOutMain accountOutMain = new AccountOutMain();

            if (!accountOutMain.SaveData(accountTrans))
            {
                _ = MessageBox.Show("Data couldn't save to AccountOut Table!");
                return false;
            }

            return true;
        }

    }
}
