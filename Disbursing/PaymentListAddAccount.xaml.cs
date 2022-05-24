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
    /// Interaction logic for PaymentListAddAccount.xaml
    /// </summary>
    public partial class PaymentListAddAccount : Window
    {
        Invoice _invoice;
        PaymentListWithItems _paymentListWithItems;
        InvoiceAccount _invoiceAccount;

        public PaymentListAddAccount(Invoice invoice, PaymentListWithItems paymentListWithItems, ref InvoiceAccount invoiceAccount)
        {
            InitializeComponent();
            _invoice = invoice;
            _paymentListWithItems = paymentListWithItems;
            _invoiceAccount = invoiceAccount;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Account account;

            if (LstMain.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select a record from Main List!");
                return;
            }

            account = LstMain.SelectedItem as Account;

            if (_invoice.PayCurr != account.AccountCurr)
            {
                _ = MessageBox.Show("Account currency is not same with Invoice Currency!");
                return;
            }

            //if (_invoice.PayAmount > account.AccInTotal)
            //{
            //    _ = MessageBox.Show("Account amount is not enough for Invoice Amount!");
            //    return;
            //}

            _invoiceAccount.AccountId = account.ID;
            _invoiceAccount.AccountCurr = account.AccountCurr;
            _invoiceAccount.AccountNu = account.AccountNu;
            _invoiceAccount.AccInTotal = account.AccInTotal;

            _paymentListWithItems.isAccountAdded = true;

            Close();
        }

        private void LstView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BtnSave_Click(sender, e);
        }
    }
}
