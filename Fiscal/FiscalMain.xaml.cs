using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace G8FinApp.Fiscal
{
    /// <summary>
    /// Interaction logic for FiscalMain.xaml
    /// </summary>
    public partial class FiscalMain : Window
    {

        public FiscalMain()
        {
            InitializeComponent();
            LoadLstMain();
        }

        private void LoadLstMain()
        {
            FiscalApproveMain fiscalApproveMain = new FiscalApproveMain();
            fiscalApproveMain.InitList();
            LstMain.ItemsSource = fiscalApproveMain;
        }

        private void LoadLstCommit()
        {
            CommitApproveNonDisbursign commitApproveNonDisbursign = new CommitApproveNonDisbursign();
            LstCommit.ItemsSource = commitApproveNonDisbursign;
        }

        private void OpenFiscalApprove(object sender, RoutedEventArgs e)
        {

        }

        private void OpenCommitForm(object sender, RoutedEventArgs e)
        {
            FiscalApprove fiscalApprove;
            FiscalCommit fiscalCommit;

            if (LstMain.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select an item from Main List!");
                return;
            }

            fiscalApprove = LstMain.SelectedItem as FiscalApprove;
            fiscalCommit = new FiscalCommit(fiscalApprove);
            fiscalCommit.ShowDialog();

            LoadLstMain();
            LoadLstCommit();
        }

        private void OpenPendingForm(object sender, RoutedEventArgs e)
        {
            FiscalApprove fsclApprove;
            FiscalApproveMain fiscalApproveMain;

            Pending pending = new Pending();
            PendingMain pendingMain = new PendingMain();
            FiscalPending fsclPending;

            if (LstMain.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select a record from Main List!");
                return;
            }
            else
            {
                fsclApprove = LstMain.SelectedItem as FiscalApprove;

                pending.APPROVEID = fsclApprove.ID;
                pending.MAINID = fsclApprove.MAININD;
                pending.ENCUMID = fsclApprove.ENCUMID;
                pending.REQDESC = fsclApprove.REQDESC;
                pending.FMNAME = fsclApprove.FMNAME;
                pending.REQAMOUNT = fsclApprove.REQAMOUNT;
                pending.REQCURR = fsclApprove.REQCURR;

                pending.BDGTCURR = fsclApprove.BDGTCURR;
                pending.BDGTAMOUNT = fsclApprove.BDGTAMOUNT;

                if (pendingMain.InitPendingData(ref pending))
                {
                    fsclPending = new FiscalPending(pending);
                    fsclPending.ShowDialog();
                    fiscalApproveMain = new FiscalApproveMain();
                    fiscalApproveMain.InitList();
                    LstMain.ItemsSource = fiscalApproveMain;
                }
                else
                {
                    _ = MessageBox.Show("Error-Pending data couldn't take!");
                    return;
                }
            }
        }

        private void OpenSendToPayment(object sender, RoutedEventArgs e)
        {
            FiscalSendToPayment fiscalSendToPayment = new FiscalSendToPayment();
            fiscalSendToPayment.ShowDialog();
        }

        private void OpenFiscalPaymentList(object sender, RoutedEventArgs e)
        {
            FiscalPaymentList fisccalPaymentList = new FiscalPaymentList();
            fisccalPaymentList.ShowDialog();
        }

        private void OpenApproveNewDeposit(object sender, RoutedEventArgs e)
        {
            FiscalNewDeposit fiscalNewDeposit = new FiscalNewDeposit();
            fiscalNewDeposit.ShowDialog();
        }

        private void OpenFiscalCloseDeposit(object sender, RoutedEventArgs e)
        {
            FiscalCloseDeposit fiscalCloseDeposit = new FiscalCloseDeposit();
            fiscalCloseDeposit.ShowDialog();
        }

        private void EnterCashCall(object sender, RoutedEventArgs e)
        {
            FiscalCashCallEnter fiscalCashCallEnter = new FiscalCashCallEnter();
            fiscalCashCallEnter.ShowDialog();
        }

        private void LstCommit_CancelCommit(object sender, RoutedEventArgs e)
        {
            MainCommitMain mainCommitMain;
            CommitMain commitMain;
            FiscalApproveMain fiscalApproveMain;

            CommitApprove commitApprove = TakeLstCommitNumber();
            if (commitApprove is null)
            {
                _ = MessageBox.Show("Please select a commit!");
                return;
            }

            if (MessageBox.Show("Do you want to cancel the Commit Number!" + Environment.NewLine +
                "You can't take same Commit Number again!", 
                "Commit Number Cancellation", 
                MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            bool isCanCommitBeCanceled = IsCanCommitBeCancelled(commitApprove);
            if (!isCanCommitBeCanceled)
            {
                _ = MessageBox.Show("The Commit has Invoice Amount!" + Environment.NewLine + "There you can't cancel the Commit!");
                return;
            }

            mainCommitMain = new MainCommitMain();
            if (!mainCommitMain.UpdateData(commitApprove))
            {
                _ = MessageBox.Show("Cancellation of the Commit Number is unsuccessful-1!");
                return;
            }

            fiscalApproveMain = new FiscalApproveMain();
            if (!fiscalApproveMain.UpdateAndDeleteFiscalApprove(commitApprove))
            {
                _ = MessageBox.Show("Cancellation of the Commit Number is unsuccessful-2!");
                return;
            }

            commitMain = new CommitMain();
            if(commitMain.UpdateFiscalCommitAndInserCommitCancel(commitApprove))
            {
                _ = MessageBox.Show("Cancellation of the Commit Number is unsuccessful-3!");
                return;
            }

            _ = MessageBox.Show("Cancellation of the Commit Number is successful");

            LoadLstMain();
            LoadLstCommit();

        }

        private bool IsCanCommitBeCancelled(CommitApprove commitApprove)
        {
            if (commitApprove.TotInvAmount > 0)
            {
                return false;
            }

            return true;
        }
        private CommitApprove TakeLstCommitNumber()
        {
            if (LstCommit.SelectedIndex == -1)
            {
                return null;
            }

            return LstCommit.SelectedItem as CommitApprove;
        }

        private void LstMain_CancelApproveReturnSender(object sender, RoutedEventArgs e)
        {
            FiscalApprove fiscalApprove;
            Fincon.FinconApprove finconApprove;
            FiscalApproveMain fiscalApproveMain;
            Fincon.FinconApproveMain finconApproveMain;
            Pending pending;
            PendingMain pendingMain;

            if(MessageBox.Show("Do you want to send to this file?",
                "Cancel", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            fiscalApprove = TakeLstMainApprove();
            if (fiscalApprove is null)
            {
                _ = MessageBox.Show("Please select an item from list!");
                return;
            }

            fiscalApproveMain = new FiscalApproveMain();
            fiscalApprove.APPROVECHOICE = "Cancelled";
            fiscalApprove.APPDATE = DateTime.Now;

            if (!fiscalApproveMain.UpdateData(fiscalApprove.ID, fiscalApprove.APPROVECHOICE))
            {
                _ = MessageBox.Show("Cancellation is unsuccessful(Fiscal Table)!");
                return;
            }

            finconApprove = new Fincon.FinconApprove()
            {
                MAININD = fiscalApprove.MAININD,
                ENCUMID = fiscalApprove.ENCUMID,
                APPDATE = DateTime.Now,
                APPROVECHOICE = "Empty",
                SENDTO = "Fiscal",

            };

            finconApproveMain = new Fincon.FinconApproveMain();
            if (!finconApproveMain.UpdateData(finconApprove))
            {
                _ = MessageBox.Show("Cancellation is unsuccessful(Fincon Table)!)");
                return;
            }

            if (!string.IsNullOrEmpty(fiscalApprove.PENDINGNO))
            {
                pendingMain = new PendingMain();
                pending = new Pending()
                {
                    PENDINGNO = fiscalApprove.PENDINGNO,
                    MAINID = fiscalApprove.MAININD,
                    ENCUMID = fiscalApprove.ENCUMID,
                    APPDATE = DateTime.Now,
                    REQAMOUNT = fiscalApprove.REQAMOUNT,
                };

                if (!pendingMain.PendingCancel(pending))
                {
                    _ = MessageBox.Show("Cancellation is unsuccessful(Fisal Pending Table!");
                    return;
                }
            }

            _ = MessageBox.Show("Cancellation is successful");

            LoadLstMain();
        }

        private FiscalApprove TakeLstMainApprove()
        {
            if (LstMain.SelectedIndex == -1)
            {
                return null;
            }

            return LstMain.SelectedItem as FiscalApprove;
        }

        private void OpenPendingsList(object sender, RoutedEventArgs e)
        {
            FiscalPendingsList fiscalPendingsList = new FiscalPendingsList();
            fiscalPendingsList.ShowDialog();
        }

        private void OpenFiscalBudget(object sender, RoutedEventArgs e)
        {
            FiscalBudget fiscalBudget = new FiscalBudget();
            fiscalBudget.ShowDialog();
        }

        private void OpenNewMiscellaneous(object sender, RoutedEventArgs e)
        {
            FiscalMiscellaneus fiscalMiscellaneus = new FiscalMiscellaneus();
            fiscalMiscellaneus.ShowDialog();
        }

        private void AscendOrderCommitAmount(object sender, RoutedEventArgs e)
        {
            CommitApproveNonDisbursign commitApproves = new CommitApproveNonDisbursign();
            LstCommit.ItemsSource = commitApproves.OrderBy(app => app.CommitAmount);
        }

        private void DescendOrderCommitAmount(object sender, RoutedEventArgs e)
        {
            CommitApproveNonDisbursign commitApproves = new CommitApproveNonDisbursign();
            LstCommit.ItemsSource = commitApproves.OrderByDescending(app => app.CommitAmount);
        }
    }
}
