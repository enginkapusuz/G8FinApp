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
    /// Interaction logic for PurchasingContract.xaml
    /// </summary>
    public partial class PurchasingContract : Window
    {
        Bidding bidding = null;
        ContractMain contractmain = new ContractMain();

        private const string curFormat = "#,#.0000";

        public PurchasingContract(Bidding _bidding)
        {
            InitializeComponent();

            contractmain.InitContracts();
            LstMain.ItemsSource = contractmain;

            if (_bidding is null)
            {
                return;
            }

            bidding = _bidding;

            TxtBiddingName.Text = bidding.BiddingName;
            TxtBiddingPrice.Text = bidding.BiddingPrice.ToString("#,#.00");
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Contract contract;
            ContractMain contractMain;

            if (bidding is null)
            {
                return;
            }

            if (string.IsNullOrEmpty(TxtPcDate.Text))
            {
                _ = MessageBox.Show("P&C Date is empty!");
                return;
            }
            if (string.IsNullOrEmpty(TxtPcAmount.Text))
            {
                _ = MessageBox.Show("P&C Amount is empty!");
                return;
            }

            if (!DateTime.TryParse(TxtPcDate.Text, out DateTime dtTmPcDate))
            {
                _ = MessageBox.Show("P&C Date is not proper!");
                return;
            }

            if (!(decimal.TryParse(TxtPcAmount.Text, out decimal dcmlPcAmount) && dcmlPcAmount > 0))
            {
                _ = MessageBox.Show("P&C Amount is not proper!");
                return;
            }

            char contractType = ChckContract.IsChecked == true ? 'C' : 'P';

            string contractNo = bidding.FMNo.PadLeft(2, '0') + "-" + DateTime.Now.ToString("yy") + "-" + contractType + "-" + (bidding.LastContractNu + 1).ToString().PadLeft(4, '0');
            TxtContractNo.Text = contractNo;

            contractMain = new ContractMain();
            contract = new Contract()
            {
                BiddingId = bidding.ID,
                PcDate = dtTmPcDate,
                PcAmount = dcmlPcAmount,
                Company = TxtContractor.Text,
                ContractNo = contractNo,
            };

            if (MessageBox.Show("Do you want to save the Contract?", "Save", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            if (!contractMain.SaveData(contract))
            {
                _ = MessageBox.Show("Data couldn't save!");
                return;
            }

            _ = MessageBox.Show("Data is saved");

            Close();
        }

        private void OpenPurchasingContractItems(object sender, RoutedEventArgs e)
        {
            Contract contract = null;

            if (LstMain.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select a Contract!");
            }

            contract = LstMain.SelectedItem as Contract;
            if(contract is null)
            {
                _ = MessageBox.Show("Please select an item!");
                return;
            }
            PurchasingContractItems purchasingContractItems = new PurchasingContractItems(contract);
            purchasingContractItems.ShowDialog();
        }

        private void OpenPurchasingNotifications(object sender, RoutedEventArgs e)
        {
            Contract contract;
            PurchasingNotfications purchasingNotfications;
            
            if (LstMain.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select an item from List!");
                return;
            }

            contract = LstMain.SelectedItem as Contract;

            purchasingNotfications= new PurchasingNotfications(contract);
            purchasingNotfications.ShowDialog();
        }

        private void OpenContractSendTo(object sender, RoutedEventArgs e)
        {
            PurchasingContractSendTo purchasingContractSendTo = new PurchasingContractSendTo();
            purchasingContractSendTo.ShowDialog();
        }

        private void TxtPcAmount_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtPcAmount.Text))
            {
                return;
            }

            if (!decimal.TryParse(TxtPcAmount.Text, out decimal dcmlPcAmount))
            {
                return;
            }

            TxtPcAmount.Text = dcmlPcAmount.ToString(curFormat);
        }

        private void OpenPurchasingNewDeposit(object sender, RoutedEventArgs e)
        {
            Contract contract;
            Deposit deposit = null;
            PurchasingNewDeposit purchasingNewDeposit;

            if (LstMain.SelectedIndex == -1)
            {
                purchasingNewDeposit = new PurchasingNewDeposit(deposit);
                purchasingNewDeposit.ShowDialog();
                return;
            }

            contract = LstMain.SelectedItem as Contract;

            deposit = new Deposit()
            {
                ContractId = contract.ID,
                BiddingName = contract.BiddingName,
                Company = contract.Company,
                BiddingPrice = contract.PcAmount,
                BiddingCurr = contract.BiddingCurr,
                ContractNo = contract.ContractNo,
            };

            purchasingNewDeposit = new PurchasingNewDeposit(deposit);
            purchasingNewDeposit.ShowDialog();
        }

        private void OpenPurchasingCloseDeposit(object sender, RoutedEventArgs e)
        {
            PurchasingCloseDeposit purchasingCloseDeposit = new PurchasingCloseDeposit();
            purchasingCloseDeposit.ShowDialog();
        }
    }
}
