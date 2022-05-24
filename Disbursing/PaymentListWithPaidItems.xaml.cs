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
    /// Interaction logic for PaymentListWithPaidItems.xaml
    /// </summary>
    public partial class PaymentListWithPaidItems : Window
    {
        private PaymentList _paymentList;
        IEnumerable<Invoice> notPaidInvoices;
        InvoiceAccountMain invoiceAccountMain;
        public PaymentListWithPaidItems(PaymentList paymentList)
        {
            InitializeComponent();
            _paymentList = paymentList;
            InitListMain();
            invoiceAccountMain = new InvoiceAccountMain();
        }
        public void InitListMain()
        {
            InvoiceMain invoiceMain = new InvoiceMain();
            invoiceMain.InitList();

            notPaidInvoices = from inv in invoiceMain
                              where inv.PaymentListId == _paymentList.ID
                              select inv;

            LstMain.ItemsSource = notPaidInvoices;
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            PaymentItemsPrint paymentItemsPrint = new PaymentItemsPrint(notPaidInvoices);
            paymentItemsPrint.ShowDialog();
        }
    }
}
