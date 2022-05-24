using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G8FinApp.Disbursing
{
    public class AccountInSaving
    {
        public readonly AccountInMain accounInMain = new AccountInMain();

        public bool SaveAccountInData(AccountTrans accountTrans)
        {
            if (!accounInMain.SaveData(accountTrans))
            {
                return false;
            }
            return true;
        }
    }
}
