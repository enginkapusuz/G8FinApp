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

namespace G8FinApp.Fiscal
{
    /// <summary>
    /// Interaction logic for FiscalCloseDeposit.xaml
    /// </summary>
    public partial class FiscalCloseDeposit : Window
    {
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();
        Purchasing.Deposit deposit;
        public FiscalCloseDeposit()
        {
            Purchasing.DepositMain depositMain = new Purchasing.DepositMain(isInitList: true, userName: "FiscalClose");
            InitializeComponent();
            LstMain.ItemsSource = depositMain;
            //To understand User choiced an item from LstMain
            BtnSave.IsEnabled = false;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Purchasing.DepositMain depositMain;

            if (LstMain.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select an item from Main List!");
                return;
            }

            //Deposit Date Check
            if (!DateTime.TryParse(TxtDepositDate.Text, out DateTime dtTmFisReturnDepositDate))
            {
                _ = MessageBox.Show("Deposit Date is not proper!");
                return;
            }
            //Deposit Date Check

            if (MessageBox.Show("Do you want to approve the Deposit?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                _ = MessageBox.Show("Saving process has been cancelled!");
                return;
            }

            depositMain = new Purchasing.DepositMain();

            deposit.FiscalReturnDate = dtTmFisReturnDepositDate;

            if (!depositMain.FiscalCloseUpdate(deposit))
            {
                _ = MessageBox.Show("Data couldn't save!");
                return;
            }

            _ = MessageBox.Show("Data is saved");

            LstMain.ItemsSource = new Purchasing.DepositMain(isInitList: true, userName: "FiscalClose");
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

            TxtDepositAmount.Text = deposit.DisbReturnAmount.ToString(prgrmConst.curFormat);
          
            //To understand User choiced an item from LstMain
            BtnSave.IsEnabled = true;
        }
    }
}
