using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Windows;

namespace G8FinApp.Fiscal
{
    public class CommitApproveMain : ObservableCollection<CommitApprove>
    {
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();

        public CommitApproveMain()
        {
            InitList();
        }


        private void InitList()
        {
            CommitApprove commitApprove;

            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM FiscalCommitAppMainDistinct WHERE FC.ID IS NOT NULL ORDER BY FC.ID"
                };

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        commitApprove = new CommitApprove();

                        commitApprove.ApproveSendTo = new List<string>();
                        commitApprove.ApproveChoice = new List<string>();
                        commitApprove.ApproveDate = new List<DateTime>();

                        commitApprove.CommitId = reader[0].ToString();
                        commitApprove.MainId = reader[1].ToString();
                        commitApprove.EncumbId = reader[2].ToString();

                        commitApprove.CommitNu = reader[3].ToString();
                        commitApprove.CommitDate = DateTime.Parse(reader[4].ToString());
                        commitApprove.CommitAmount = decimal.Parse(reader[5].ToString());

                        commitApprove.CommitCurr = reader[6].ToString();
                        commitApprove.CommitActCode = reader[7].ToString();
                        commitApprove.BdgtCurr = reader[8].ToString();

                        commitApprove.BdgtAmount = decimal.Parse(reader[9].ToString());
                        commitApprove.ApproveId = reader[10].ToString();
                        commitApprove.ApproveSendTo.Add(reader[11].ToString());

                        commitApprove.ApproveChoice.Add(reader[12].ToString());
                        commitApprove.ApproveDate.Add(DateTime.Parse(reader[13].ToString()));
                        commitApprove.FmName = reader[14].ToString();

                        commitApprove.ReqDesc = reader[15].ToString();
                        commitApprove.ReqAmount = decimal.Parse(reader[16].ToString());
                        commitApprove.ReqCurr = reader[17].ToString();

                        commitApprove.TotInvAmount = decimal.Parse(reader[18].ToString());

                        Add(commitApprove);
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("Error:CommitApprove:InitList:" + ex.Message);
                    return;
                }

            }
        }
    }
}
