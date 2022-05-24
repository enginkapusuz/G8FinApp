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

namespace G8FinApp.Purchasing
{
    /// <summary>
    /// Interaction logic for PurchasingCloseDeposit.xaml
    /// </summary>
    public partial class PurchasingCloseDeposit : Window
    {
        Deposit deposit;
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();
        public PurchasingCloseDeposit()
        {
            DepositMain depositMain = new DepositMain(isInitList: true, userName: "PurchasingClose");
            InitializeComponent();
            LstMain.ItemsSource = depositMain;

            //To understand User choiced an item from LstMain
            BtnSave.IsEnabled = false;
        }

        private void LstMain_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (LstMain.SelectedIndex == -1)
            {
                return;
            }

            deposit = LstMain.SelectedItem as Deposit;

            TxtContractNo.Text = deposit.ContractNo;
            TxtBiddingName.Text = deposit.BiddingName;
            TxtCompany.Text = deposit.Company;

            TxtPcAmount.Text = deposit.BiddingPrice.ToString(prgrmConst.curFormat);
            TxtCurrency.Text = deposit.BiddingCurr;
            TxtDepositRate.Text = deposit.DepositRate.ToString(prgrmConst.curFormat);

            TxtDepositCurrency.Text = deposit.DepositCurrency;

            //To understand User choiced an item from LstMain
            BtnSave.IsEnabled = true;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            DepositMain depositMain;

            //Deposit Amount Check
            if (!decimal.TryParse(TxtDepositAmount.Text, out decimal dcmlPCRetDepositAmount))
            {
                _ = MessageBox.Show("Deposit amount is not proper!");
                return;
            }

            if (!(dcmlPCRetDepositAmount > 0 && dcmlPCRetDepositAmount <= deposit.DisbDepositAmount))
            {
                _ = MessageBox.Show("Deposit amout is not proper for return amount!");
                return;
            }

            if(MessageBox.Show("Deposit Amount:" + dcmlPCRetDepositAmount.ToString(prgrmConst.curFormat), "Confirmation", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                _ = MessageBox.Show("Saving process has been cancelled!");
                return;
            }
            //Deposit Amount Check

            //Deposit Date Check
            if (!DateTime.TryParse(TxtDepositDate.Text, out DateTime dtTmPCReturnDepositDate))
            {
                _ = MessageBox.Show("Deposit Date is not proper!");
                return;
            }
            //Deposit Date Check

            deposit.PCReturnAmount = dcmlPCRetDepositAmount;
            deposit.PCReturnDate = dtTmPCReturnDepositDate;

            depositMain = new DepositMain();

            if (!depositMain.PurchasingCloseUpdate(deposit))
            {
                _ = MessageBox.Show("Deposit return couldn't save!");
                return;
            }

            LstMain.ItemsSource = new DepositMain(isInitList: true, "PurchasingClose");
        }

        private void TxtDepositAmount_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtDepositAmount.Text))
            {
                return;
            }

            if (!decimal.TryParse(TxtDepositAmount.Text, out decimal dcmlDepositAmount))
            {
                return;
            }

            TxtDepositAmount.Text = dcmlDepositAmount.ToString(prgrmConst.curFormat);
        }

    }
}
