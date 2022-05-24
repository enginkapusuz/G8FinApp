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
    /// Interaction logic for PurchasingContractSendTo.xaml
    /// </summary>
    public partial class PurchasingContractSendTo : Window
    {
        ContractMain contractMain = new ContractMain();

        public PurchasingContractSendTo()
        {
            InitializeComponent();

            contractMain.InitContractsNotSendTo();
            LstMain.ItemsSource = contractMain;
        }

        private void BtnSendTo_Click(object sender, RoutedEventArgs e)
        {
            Contract contract;

            if (LstMain.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select a Contract from List!");
                return;
            }
            contract = LstMain.SelectedItem as Contract;

            if (MessageBox.Show("Do you want to send the Contract to Fincon?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                _ = MessageBox.Show("Sending process has been cancelled!");
                return;
            }
            if (!contractMain.SendToFincon(contract))
            {
                MessageBox.Show("Error in PurchasingContract.xaml.cs!");
                return;
            }
            _ = MessageBox.Show("The Contract has been send to Fincon.");

            Close();
        }
    }
}
