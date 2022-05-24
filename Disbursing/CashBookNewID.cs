using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G8FinApp.Disbursing
{
    public class CashBookNewID
    {
        private readonly BookCashMain bookCashMain = new BookCashMain();
        public string TakeBookCashLastId(BookCash bookCash, bool IsNewId)
        {
            if(IsNewId)
            {
                if (!bookCashMain.SaveData(bookCash))
                {
                    return string.Empty;
                }
            }
            if (!bookCashMain.TakeLastIdNumber())
            {
                return string.Empty;
            }
            return bookCashMain.LastIdNu;
        }
    }
}
