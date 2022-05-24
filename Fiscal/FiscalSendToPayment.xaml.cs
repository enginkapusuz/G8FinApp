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
    /// Interaction logic for FiscalSendToPayment.xaml
    /// </summary>
    public partial class FiscalSendToPayment : Window
    {
        public FiscalSendToPayment()
        {
            InitializeComponent();
        }

        private void OpenTakeInvoice(object sender, RoutedEventArgs e)
        {
            CommitApprove commitapprove;
            CommitApproveNonDisbursign commitApproveNonDisbursign;
            Invoice invoice;
            FiscalSendToTakeInvoice fiscalSendToTakeInvoice;

            if (LstMain.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select a record from Main List!");
                return;
            }
            else
            {
                commitapprove = LstMain.SelectedItem as CommitApprove;
                invoice = new Invoice()
                {
                    COMMITID = commitapprove.CommitId.Trim(),
                    COMMITAMOUNT = commitapprove.CommitAmount,
                    TOTINVAMOUNT = commitapprove.TotInvAmount,
                };

                fiscalSendToTakeInvoice = new FiscalSendToTakeInvoice(invoice);
                fiscalSendToTakeInvoice.ShowDialog();

                commitApproveNonDisbursign = new CommitApproveNonDisbursign();
                LstMain.ItemsSource = commitApproveNonDisbursign;
            }
        }
    }
}
