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

namespace G8FinApp.Disbursing
{
    /// <summary>
    /// Interaction logic for DisbursingDeposit.xaml
    /// </summary>
    public partial class DisbursingDeposit : Window
    {
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();
        Purchasing.Deposit deposit;

        public DisbursingDeposit()
        {
            Purchasing.DepositMain depositMain = new Purchasing.DepositMain(isInitList:true, userName:"Disbursing");
            InitializeComponent();

            LstMain.ItemsSource = depositMain;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Purchasing.DepositMain depositMain;

            //Deposit Amount Check
            if (string.IsNullOrEmpty(TxtDisbDepositAmount.Text))
            {
                _ = MessageBox.Show("Deposit Amount is not proper-1!");
                return;
            }

            if (!decimal.TryParse(TxtDisbDepositAmount.Text, out decimal dcmlDisbDepositAmount))
            {
                _ = MessageBox.Show("Deposit Amount is not proper-2!");
                return;
            }

            if (!(dcmlDisbDepositAmount > 0) || !(dcmlDisbDepositAmount < deposit.BiddingPrice))
            {
                _ = MessageBox.Show("Deposit Amount is not proper-3!");
                return;
            }
            //Deposit Amount Check

            //Currency Check
            if (string.IsNullOrEmpty(TxtDepositCurrency.Text))
            {
                _ = MessageBox.Show("Deposit Currency is not proper!");
                return;
            }
            //Currency Check

            //Deposit Date Check
            if (!DateTime.TryParse(TxtDepositDate.Text, out DateTime dtTmDepositDate))
            {
                _ = MessageBox.Show("Deposit Date is not proper!");
                return;
            }
            //Deposit Date Check

            deposit.DisbDepositAmount = dcmlDisbDepositAmount;
            deposit.DisbAppDate= dtTmDepositDate;
            deposit.DepositCurrency = TxtDepositCurrency.Text.Split('{')[2];

            if (MessageBox.Show("Do you want to save the Deposit Amount?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                _ = MessageBox.Show("Saving process has been cancelled!");
                return;
            }

            depositMain = new Purchasing.DepositMain();
            if (!depositMain.DisbursingUpdate(deposit))
            {
                _ = MessageBox.Show("Deposit couldn't update!");
                return;
            }

            _ = MessageBox.Show("Deposit is saved.");
            Close();
        }

        private void LstMain_DoubleClick(object sender, MouseButtonEventArgs e)
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

            TxtExRate.Text = (deposit.BiddingPrice / (deposit.PurchasingDepositAmount / (deposit.DepositRate / 100))).ToString(prgrmConst.curFormat);
        }

        private void TxtDisbDepoisitAmount_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtDisbDepositAmount.Text))
            {
                return;
            }

            if (!decimal.TryParse(TxtDisbDepositAmount.Text, out decimal dcmlDisbDepositAmount))
            {
                return;
            }

            if (dcmlDisbDepositAmount <= 0)
            {
                return;
            }

            TxtDisbDepositAmount.Text = dcmlDisbDepositAmount.ToString(prgrmConst.curFormat);
        }
    }
}
