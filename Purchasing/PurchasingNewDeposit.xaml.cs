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
    /// Interaction logic for PurchasingNewDeposit.xaml
    /// </summary>
    public partial class PurchasingNewDeposit : Window
    {
        Deposit deposit;
        Database.ProgramConsts prgrmConsts = new Database.ProgramConsts();
        public PurchasingNewDeposit(Deposit _deposit)
        {
            DepositMain depositMain;
            InitializeComponent();

            depositMain = new DepositMain(isInitList: true, userName:"Purchasing");
            LstMain.ItemsSource = depositMain;

            if (_deposit is null)
            {
                EnableDisableBtnSave();
                return;
            }

            deposit = _deposit;

            var isDepositInDepositMain = depositMain.Where(dp => dp.ContractId == deposit.ContractId).FirstOrDefault();
            
            if(!(isDepositInDepositMain is null))
            {
                EnableDisableBtnSave();
                return;
            }

            InitTextFields();
        }

        private void InitTextFields()
        {
            TxtBiddingName.Text = deposit.BiddingName;
            TxtContractNo.Text = deposit.ContractNo;
            TxtCurrency.Text = deposit.BiddingCurr;
            TxtCompany.Text = deposit.Company;
            TxtPcAmount.Text = deposit.BiddingPrice.ToString(prgrmConsts.curFormat);
        }

        private void EnableDisableBtnSave()
        {
            if (BtnSave.IsEnabled)
            {
                BtnSave.IsEnabled = false;
                return;
            }

            BtnSave.IsEnabled = true;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            DepositMain depositMain;

            if (string.IsNullOrEmpty(TxtDepositRate.Text))
            {
                _ = MessageBox.Show("Deposit Rate is empty!");
                return;
            }

            if (!decimal.TryParse(TxtDepositRate.Text, out decimal dcmlDepositRate) || !(dcmlDepositRate > 0) || !(dcmlDepositRate <= 100))
            {
                _ = MessageBox.Show("Deposit Rate is not proper!");
                return;
            }

            if (string.IsNullOrEmpty(TxtExRate.Text))
            {
                _ = MessageBox.Show("Exchange Rate is empty!");
                return;
            }

            if (!decimal.TryParse(TxtExRate.Text, out decimal dcmlExRate) || !(dcmlExRate > 0))
            {
                _ = MessageBox.Show("Exhange Rate is not proper!");
                return;
            }

            if (MessageBox.Show("Are those figures correct!" + '\n' + "Exchange Rate:"  + dcmlExRate.ToString() + '\n' + 
                "Deposit Rate:" + dcmlDepositRate.ToString() , "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            if (string.IsNullOrEmpty(TxtDepositDate.Text) || !DateTime.TryParse(TxtDepositDate.Text, out DateTime dtTmDepositDate))
            {
                _ = MessageBox.Show("Date is not proper!");
                return;
            }

            TxtDepositAmount.Text = (deposit.BiddingPrice * dcmlExRate * dcmlDepositRate / 100).ToString(prgrmConsts.curFormat);

            if (!decimal.TryParse(TxtDepositAmount.Text, out decimal dcmlDepositAmount))
            {
                _ = MessageBox.Show("Deposit Amount is not proper!");
                return;
            }
            deposit.DepositDate = dtTmDepositDate;
            deposit.PurchasingDepositAmount = dcmlDepositAmount;
            deposit.DepositRate = dcmlDepositRate;
            deposit.DepositCurrency = TxtDepositCurrency.Text.Split('{')[2];

            depositMain = new DepositMain();

            if (MessageBox.Show("Do you want to save deposit?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                _ = MessageBox.Show("Deposit saving process has been cancelled!");
                return;
            }

            if (!depositMain.SaveData(deposit))
            {
                _ = MessageBox.Show("Deposit couldn't save!");
                return;
            }

            _ = MessageBox.Show("Deposit is saved!");

            Close();
        }
    }
}
