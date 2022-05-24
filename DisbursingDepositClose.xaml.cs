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

namespace G8FinApp
{
    /// <summary>
    /// Interaction logic for DisbursingDepositClose.xaml
    /// </summary>
    public partial class DisbursingDepositClose : Window
    {
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();
        Purchasing.Deposit deposit;
        public DisbursingDepositClose()
        {
            Purchasing.DepositMain depositMain = new Purchasing.DepositMain(isInitList: true, "DisbursingClose");
            InitializeComponent();
            LstMain.ItemsSource = depositMain;
            //To understand User choiced an item from LstMain
            BtnSave.IsEnabled = false;
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

        private void LstMain_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (LstMain.SelectedIndex == -1)
            {
                return;
            }

            deposit = LstMain.SelectedItem as Purchasing.Deposit;

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
            Purchasing.DepositMain depositMain;

            //Deposit Amount Check
            if (!decimal.TryParse(TxtDepositAmount.Text, out decimal dcmlDisbRetDepositAmount))
            {
                _ = MessageBox.Show("Deposit amount is not proper!");
                return;
            }

            if (!(dcmlDisbRetDepositAmount > 0 && dcmlDisbRetDepositAmount <= deposit.PCReturnAmount))
            {
                MessageBox.Show("dcmlDisbRetDepositAmount:" + dcmlDisbRetDepositAmount + '\n' + "deposit.PCReturnAmount:" + deposit.PCReturnAmount);
                _ = MessageBox.Show("Deposit amout is not proper for return amount!");
                return;
            }

            if (MessageBox.Show("Deposit Amount:" + dcmlDisbRetDepositAmount.ToString(prgrmConst.curFormat), "Confirmation", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                _ = MessageBox.Show("Saving process has been cancelled!");
                return;
            }
            //Deposit Amount Check

            //Deposit Date Check
            if (!DateTime.TryParse(TxtDepositDate.Text, out DateTime dtTmDisbReturnDepositDate))
            {
                _ = MessageBox.Show("Deposit Date is not proper!");
                return;
            }
            //Deposit Date Check

            deposit.DisbReturnAmount = dcmlDisbRetDepositAmount;
            deposit.DisbReturnDate = dtTmDisbReturnDepositDate;

            depositMain = new Purchasing.DepositMain();

            if (!depositMain.DisbursingCloseUpdate(deposit))
            {
                _ = MessageBox.Show("Deposit return couldn't save!");
                return;
            }

            LstMain.ItemsSource = new Purchasing.DepositMain(isInitList: true, "DisbursingClose");

            _ = MessageBox.Show("Data is saved");
        }
    }
}
