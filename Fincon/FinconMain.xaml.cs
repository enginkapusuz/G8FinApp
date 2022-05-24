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
using System.Windows.Shapes;

namespace G8FinApp.Fincon
{
    /// <summary>
    /// Interaction logic for FinconMain.xaml
    /// </summary>
    public partial class FinconMain : Window
    {
        public FinconMain()
        {
            InitializeComponent();
            LoadLstMainApproved();
            LoadLstMain();
        }

        private void OpenFinconSendTo(object sender, RoutedEventArgs e)
        {
            FinEncumbranceSendTo fnEncmbSendTo;
            FinconApprove finApprove;

            if (LstMain.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select a record from Main List!");
                return;
            }
            else
            {
                finApprove = LstMain.SelectedItem as FinconApprove;

                fnEncmbSendTo = new FinEncumbranceSendTo(finApprove);
                fnEncmbSendTo.ShowDialog();

                LoadLstMain();
                LoadLstMainApproved();
            }
        }

        private void LoadLstMainApproved()
        {
            FinconApprovedMain finconApprovedMain = new FinconApprovedMain();
            finconApprovedMain.InitList();
            LstMainApproved.ItemsSource = finconApprovedMain;
        }

        private void LoadLstMain()
        {
            FinconApproveMain finconApproveMain = new FinconApproveMain();
            finconApproveMain.InitList();
            LstMain.ItemsSource = finconApproveMain;
        }

        private void LstMain_Delete(object sender, RoutedEventArgs e)
        {
            Budget.Approve budgetApprove;
            Budget.ApproveListMain approveListMain;
            FinconApproveMain finconApproveMain;
            FinconApprove finconApprove = TakeLstMainSelect();

            if(finconApprove is null)
            {
                _ = MessageBox.Show("Please select an item!");
                return;
            }

            if (MessageBox.Show("Do you want to delete this item?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            finconApproveMain = new FinconApproveMain();
            if(!finconApproveMain.DeleteApproveFromMainList(finconApprove))
            {
                _ = MessageBox.Show("Deletion is not successful!");
                return;
            }

            approveListMain = new Budget.ApproveListMain();
            budgetApprove = new Budget.Approve()
            {
                MAININD = finconApprove.MAININD,
                ENCUMID = finconApprove.ENCUMID,
            };

            if (!(approveListMain.DeleteData(budgetApprove)))
            {
                return;
            }

            LoadLstMain();
            LoadLstMainApproved();
        }

        private FinconApprove TakeLstMainSelect()
        {
            if (LstMain.SelectedIndex == -1)
            {
                return null;
            }
            return LstMain.SelectedItem as FinconApprove;
        }

        private void OpenPendingList(object sender, RoutedEventArgs e)
        {
            Fiscal.FiscalPendingsList fiscalPendingsList = new Fiscal.FiscalPendingsList();
            fiscalPendingsList.ShowDialog();
        }

        private void OpenFiscalBudget(object sender, RoutedEventArgs e)
        {
            Fiscal.FiscalBudget fiscalBudget = new Fiscal.FiscalBudget();
            fiscalBudget.ShowDialog();
        }
    }
}
