using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;


namespace G8FinApp.Fiscal
{
    public class CommitApproveNonDisbursign : ObservableCollection<CommitApprove>
    {
        CommitApproveMain commitApproveMain;
        public CommitApproveNonDisbursign()
        {
            InitList();
        }

        private void InitList()
        {
            commitApproveMain = new CommitApproveMain();

            foreach (CommitApprove cmpApp in commitApproveMain)
            {
                Add(cmpApp);
            }
        }
    }
}
