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
    /// Interaction logic for FiscalPaymentList.xaml
    /// </summary>
    public partial class FiscalPaymentList : Window
    {
        public FiscalPaymentList()
        {
            InitializeComponent();
        }

        private void LstMain_DoubleClick(object sender, MouseButtonEventArgs e)
        {

            PaymentList paymentList;
            PaymentItemMain paymentItemMain;

            if (LstMain.SelectedIndex == -1)
            {
                return;
            }

            paymentList = LstMain.SelectedItem as PaymentList;
            paymentItemMain = new PaymentItemMain();

            paymentItemMain.InitList(paymentList.DisPaymentListId);
            LstPaymetItems.ItemsSource = paymentItemMain;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            PaymentList paymentList;
            PaymentListMain paymentListMain;

            if (LstMain.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select a record from Approve List!");
                return;
            }

            if (LstPaymetItems.Items.Count == 0)
            {
                _ = MessageBox.Show("There is not payment!");
                return;
            }

            if (MessageBox.Show("Do you want to approve the Payment List", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            paymentList = LstMain.SelectedItem as PaymentList;
            paymentList.ListSituation = "PAID";

            paymentListMain = new PaymentListMain();

            if (!paymentListMain.UpdateData(paymentList))
            {
                _ = MessageBox.Show("Payment List couldn't save", "Error");
                return;
            }

            _ = MessageBox.Show("Payment List is being approved");
            Close();
        }
    }
}
