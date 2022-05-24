using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G8FinApp.Fiscal
{
    public class MiscellaneousCommitNu
    {
        private readonly MissCommitMain missCommitMain = new MissCommitMain();
        public int TakeLastCommitId()
        {
            missCommitMain.InitList();
            string commits = missCommitMain.Select(cmt => cmt.ID).LastOrDefault();

            if (string.IsNullOrEmpty(commits))
            {
                return 0;
            }
            return int.Parse(commits);
        }
    }
}
