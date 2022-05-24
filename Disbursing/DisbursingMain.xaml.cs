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
    /// Interaction logic for DisbursingMain.xaml
    /// </summary>
    public partial class DisbursingMain : Window
    {
        public DisbursingMain()
        {
            InitializeComponent();
        }

        private void OpenNewAccount(object sender, RoutedEventArgs e)
        {
            NewAccount newAccount = new NewAccount();
            _ = newAccount.ShowDialog();
        }

        //private void OpenAddPaymentList(object sender, RoutedEventArgs e)
        //{
        //    Approve approve;
        //    Invoice invoice;
        //    PaymentItemWin paymentItemWin;

        //    if (LstMain.SelectedIndex == -1)
        //    {
        //        _ = MessageBox.Show("Please select a record from Main List!");
        //        return;
        //    }

        //    approve = LstMain.SelectedItem as Approve;

        //    invoice = new Invoice()
        //    {
        //        PaymentListId = approve.ID,
        //        CompanyName = approve.InvCreditor,
        //        InvNu = approve.InvNu,
        //        InvDate = approve.InvDate,
        //        PayAmount = approve.InvAmount,
        //        PayCurr = approve.InvCurr,
        //        BdgtCurr = approve.BdgtCurr,
        //        BdgtAmount = approve.BdgtAmount,
        //        MainID = approve.MainId,
        //        EncumbId = approve.EncumbId,
        //    };
            
        //    paymentItemWin = new PaymentItemWin(invoice);

        //    if (paymentItemWin.ShowDialog() == null)
        //    {
        //        return;
        //    }

        //    LstMain.ItemsSource = new ApproveMain();
        //}

        private void OpenPaymentList(object sender, RoutedEventArgs e)
        {
            PaidOrNotPaid paidOrNotPaid = new PaidOrNotPaid();
            paidOrNotPaid.ShowDialog();
        }

        private void OpenNewDeposit(object sender, RoutedEventArgs e)
        {
            DisbursingDeposit disbursingDeposit = new DisbursingDeposit();
            disbursingDeposit.ShowDialog();
        }

        private void OpenReturnDepoist(object sender, RoutedEventArgs e)
        {

            DisbursingDepositClose disbursingDepositClose = new DisbursingDepositClose();
            disbursingDepositClose.ShowDialog();
        }

        private void OpenNewCashCall(object sender, RoutedEventArgs e)
        {
            DisbursingCashCall disbursingCashCall = new DisbursingCashCall();
            disbursingCashCall.ShowDialog();
        }

        private void OpenNewMiscellaneous(object sender, RoutedEventArgs e)
        {
            DisbursingMiscellaneous disbursingMiscellaneous = new DisbursingMiscellaneous();
            disbursingMiscellaneous.ShowDialog();
        }

        private void AddPaymentList(object sender, RoutedEventArgs e)
        {

            if (LstMain.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select a record from Main List!");
                return;
            }

            Approve approve = LstMain.SelectedItem as Approve;

            Invoice invoice = new Invoice()
            {
                CompanyName = approve.InvCreditor,
                InvNu = approve.InvNu,
                InvDate = approve.InvDate,
                PayAmount = approve.InvAmount,
                PayCurr = approve.InvCurr,
                BdgtCurr = approve.BdgtCurr,
                BdgtAmount = approve.BdgtAmount,
                MainID = approve.MainId,
                EncumbId = approve.EncumbId,

                CompanyAddress = string.Empty,
                BankName = string.Empty,
                IBANNu = string.Empty,
                ID = string.Empty,
                ExRate = decimal.Zero,
            };

            PaymentListMain paymentListMain = new PaymentListMain();
            PaymentList paymentList = paymentListMain.TakeNotPaidPaymentList(invoice);
            invoice.PaymentListId = paymentList is default(PaymentList) ? string.Empty : paymentList.ID;

            if (string.IsNullOrEmpty(invoice.PaymentListId))
            {
                _ = MessageBox.Show("PaymentListid couldn't be taken!");
            }

            if (MessageBox.Show("Do you want to add Payment List?" + Environment.NewLine + 
                "Company:" + "\t" + invoice.CompanyName + Environment.NewLine +
                "Invoice Nu:" + "\t" + invoice.InvNu + Environment.NewLine +
                "Invoice Date:" + "\t" + invoice.InvDate.ToShortDateString() + Environment.NewLine + 
                "Payment Amount:" + "\t" + invoice.PayAmount + Environment.NewLine+
                "Payment Curr:" + "\t" + invoice.PayCurr, "Invoice Data", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            InvoiceMain invoiceMain = new InvoiceMain();
            if (!invoiceMain.SaveData(invoice))
            {
                _ = MessageBox.Show("Data couldn't save!");
                return;
            }
            approve.ApproveChoice = "InPayList";
            approve.AppDate = DateTime.Parse(DateTime.Now.ToString("d"));
            approve.SendTo = "Disbursing";

            ApproveMain approveMain = new ApproveMain();

            if (!approveMain.SaveData(approve))
            {
                _ = MessageBox.Show("Data couldn't save!(Approve Table)");
                return;
            }

            _ = MessageBox.Show("Data is saved!");

            LstMain.ItemsSource = new ApproveMain();
        }

        private void OpenPaymentListNew(object sender, RoutedEventArgs e)
        {
            Invoice invoice = new Invoice();

            PaymentListMain paymentListMain = new PaymentListMain();
            PaymentList paymentList = paymentListMain.TakeNotPaidPaymentList(invoice);

            invoice.PaymentListId = paymentList is null ? string.Empty : paymentList.ID;

            if (string.IsNullOrEmpty(invoice.PaymentListId))
            {
                _ = MessageBox.Show("PaymentListid couldn't be taken!");
                return;
            }

            PaymentListWithItems paymentListWithItems = new PaymentListWithItems(paymentList);
            paymentListWithItems.ShowDialog();

            LstMain.ItemsSource = new ApproveMain();
        }

        private void AscendInvoiceAmount(object sender, RoutedEventArgs e)
        {
            ApproveMain approves = new ApproveMain();
            LstMain.ItemsSource = approves.OrderBy(app => app.InvAmount);
        }

        private void AscendCreditor(object sender, RoutedEventArgs e)
        {
            ApproveMain approves = new ApproveMain();
            LstMain.ItemsSource = approves.OrderBy(app => app.InvCreditor);
        }

        private void AscendCurrency(object sender, RoutedEventArgs e)
        {
            ApproveMain approves = new ApproveMain();
            LstMain.ItemsSource = approves.OrderBy(app => app.InvCurr);
        }

        private void AscendInvoiceDate(object sender, RoutedEventArgs e)
        {
            ApproveMain approves = new ApproveMain();
            LstMain.ItemsSource = approves.OrderBy(app => app.InvDate);
        }

        private void AscendInvoiceNo(object sender, RoutedEventArgs e)
        {
            ApproveMain approves = new ApproveMain();
            LstMain.ItemsSource = approves.OrderBy(app => app.InvNu);
        }

        private void DescendInvoiceAmount(object sender, RoutedEventArgs e)
        {
            ApproveMain approves = new ApproveMain();
            LstMain.ItemsSource = approves.OrderByDescending(app => app.InvAmount);
        }

        private void DescendCreditor(object sender, RoutedEventArgs e)
        {
            ApproveMain approves = new ApproveMain();
            LstMain.ItemsSource = approves.OrderByDescending(app => app.InvCreditor);
        }

        private void DescendInvoiceDate(object sender, RoutedEventArgs e)
        {
            ApproveMain approves = new ApproveMain();
            LstMain.ItemsSource = approves.OrderByDescending(app => app.InvDate);
        }

        private void DescendInvoiceNo(object sender, RoutedEventArgs e)
        {
            ApproveMain approves = new ApproveMain();
            LstMain.ItemsSource = approves.OrderByDescending(app => app.InvNu);
        }

        private void DescendCurrency(object sender, RoutedEventArgs e)
        {
            ApproveMain approves = new ApproveMain();
            LstMain.ItemsSource = approves.OrderByDescending(app => app.InvCurr);
        }

        private void AscendId(object sender, RoutedEventArgs e)
        {
            ApproveMain approves = new ApproveMain();
            LstMain.ItemsSource = approves.OrderBy(app => int.Parse(app.ID));
        }

        private void DescendId(object sender, RoutedEventArgs e)
        {
            ApproveMain approves = new ApproveMain();
            LstMain.ItemsSource = approves.OrderByDescending(app => int.Parse(app.ID));
        }
    }
}
