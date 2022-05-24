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
    /// Interaction logic for PaidOrNotPaid.xaml
    /// </summary>
    public partial class PaidOrNotPaid : Window
    {
        PaymentListMain paymentListMain;
        public PaidOrNotPaid()
        {
            InitializeComponent();
            paymentListMain = new PaymentListMain();
        }

        private void CmbPaymentSitiation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtPaymentNumber.ItemsSource = null;

            if (txtPaymentSitiuation.SelectedItem.ToString().Contains("Not Paid"))
            {
                IEnumerable<PaymentList> notPaidList = from pdLst in paymentListMain
                                                       where pdLst.ListSituation == "NOTPAID"
                                                       select pdLst;

                txtPaymentNumber.ItemsSource = notPaidList;

            }
            else if(txtPaymentSitiuation.SelectedItem.ToString().Contains("Paid"))
            {
                IEnumerable<PaymentList> notPaidList = from pdLst in paymentListMain
                                                       where pdLst.ListSituation == "PAID"
                                                       select pdLst;

                txtPaymentNumber.ItemsSource = notPaidList;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            PaymentList paymentList;

            PaymentListWithItems paymentListWithItems;
            PaymentListWithPaidItems paymentListWithPaidItems;

            if (txtPaymentNumber.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select a Payment List Number! ");
                return;
            }

            paymentList = txtPaymentNumber.SelectedItem as PaymentList;

            if (txtPaymentSitiuation.SelectedItem.ToString().Contains("Not Paid"))
            {
                paymentListWithItems = new PaymentListWithItems(paymentList);
                paymentListWithItems.ShowDialog();
                Close();
            }
            else if (txtPaymentSitiuation.SelectedItem.ToString().Contains("Paid"))
            {
                paymentListWithPaidItems = new PaymentListWithPaidItems(paymentList);
                paymentListWithPaidItems.ShowDialog();
                Close();
            }
        }
    }
}
