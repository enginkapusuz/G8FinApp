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
    /// Interaction logic for DisbursingMiscellaneous.xaml
    /// </summary>
    public partial class DisbursingMiscellaneous : Window
    {
        Database.ProgramConsts programConsts = new Database.ProgramConsts();
        public DisbursingMiscellaneous()
        {
            InitializeComponent();
            Fiscal.MiscellaneousMain miscellaneousMain = new Fiscal.MiscellaneousMain();
            miscellaneousMain.InitList();
            LstMain.ItemsSource = miscellaneousMain;
            SumLstMain();

            AccountMain accountMain = new AccountMain();
            TxtAccount.ItemsSource = accountMain;
        }

        private void TxtPaymentAmount_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtPaymentAmount.Text))
            {
                return;
            }
            if (!decimal.TryParse(TxtPaymentAmount.Text, out decimal dcmlPyamentAmount))
            {
                return;
            }

            TxtPaymentAmount.Text = dcmlPyamentAmount.ToString(programConsts.curFormat);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(TxtPaymentAmount.Text, out decimal dcmlPaymentAmount))
            {
                _ = MessageBox.Show("Payment amount is not proper!");
                return;
            }

            if (!DateTime.TryParse(TxtPaymentDate.Text, out DateTime dttmPaymentDate))
            {
                _ = MessageBox.Show("Payment Date is not proper!");
                return;
            }
            if (string.IsNullOrEmpty(TxtInvoiceNo.Text))
            {
                _ = MessageBox.Show("Invoice No is not proper!");
                return;
            }
            if(string.IsNullOrEmpty(TxtAccount.Text))
            {
                _ = MessageBox.Show("Account number is not proper!");
                return;
            }

            if (LstMain.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select an item from Main List!");
                return;
            }
            
            Fiscal.Miscellaneous miscellaneous = LstMain.SelectedItem as Fiscal.Miscellaneous;

            if (!string.IsNullOrEmpty(miscellaneous.InvoiceNumber))
            {
                _ = MessageBox.Show("The item is already paid!");
                return;
            }

            miscellaneous.PaymentAmount = dcmlPaymentAmount;
            miscellaneous.PaymentDate = dttmPaymentDate;
            miscellaneous.InvoiceNumber = TxtInvoiceNo.Text.Trim();

            if (MessageBox.Show("Do you want to save?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            BookCash bookCash = new BookCash()
            {
                CashBookDate = dttmPaymentDate,
                Description = TxtCashBookDesc.Text.Trim(),
            };

            Account account = TxtAccount.SelectedItem as Account;

            if (account.AccountCurr != miscellaneous.PaymentCurrency)
            {
                _ = MessageBox.Show("Account currency is different from item currency!");
                return;
            }

            AccountTrans accountTrans = new AccountTrans()
            {
                AccountId = account.ID,
                
                CashBookId = TxtIsNewCashBook.IsChecked == true ? new CashBookNewID().TakeBookCashLastId(bookCash, true) :
                new CashBookNewID().TakeBookCashLastId(bookCash, false),

                TransAmount = dcmlPaymentAmount,
                TransDate = dttmPaymentDate,
                TransRemark = TxtRemarks.Text.Trim(),
            };

            if (miscellaneous.EntryType == "OutGoing")
            {
                AccountOutSaving accountOutSaving = new AccountOutSaving();

                if (!accountOutSaving.IsAccountAmountEnough(accountTrans))
                {
                    _ = MessageBox.Show("Account is not enough for this amount!");
                    return;
                }

                if (!accountOutSaving.SaveAccountOutData(accountTrans))
                {
                    _ = MessageBox.Show("Account out data couldn't save!");
                    return;
                }
            }

            Fiscal.MiscellaneousMain miscellaneousMain = new Fiscal.MiscellaneousMain();
            if (!miscellaneousMain.UpdateDisbursingPayment(miscellaneous))
            {
                _ = MessageBox.Show("Data couldn't saved!");
                return;
            }

            if (miscellaneous.EntryType == "InComing")
            {
                AccountInSaving accountInSaving = new AccountInSaving();
                if (!accountInSaving.SaveAccountInData(accountTrans))
                {
                    _ = MessageBox.Show("Acccount in data couldn't save!");
                    return;
                }
            }

            _ = MessageBox.Show("Data is saved!");
            miscellaneousMain.Clear();
            miscellaneousMain.InitList();
            LstMain.ItemsSource = miscellaneousMain;
            SumLstMain();
            ClearFields();
        }

        private void SumLstMain()
        {
            Fiscal.MiscellaneousMain miscellaneousMain = LstMain.ItemsSource as Fiscal.MiscellaneousMain;

            decimal outgoingPlannedTotal = miscellaneousMain.Where(misCell => misCell.EntryType == "OutGoing")
                .Select(misCell => misCell.PlannedPayment).Sum();

            decimal incomingPlannedTotal = miscellaneousMain.Where(misCell => misCell.EntryType == "InComing")
                .Select(misCell => misCell.PlannedPayment).Sum();

            decimal outgoingPaymentTotal = miscellaneousMain.Where(misCell => misCell.EntryType == "OutGoing")
                .Select(misCell => misCell.PaymentAmount).Sum();      
            
            decimal incomingPaymentTotal = miscellaneousMain.Where(misCell => misCell.EntryType == "InComing")
                .Select(misCell => misCell.PaymentAmount).Sum();


            TxtLstMainPlannedPaymentTotal.Text = (incomingPlannedTotal - outgoingPlannedTotal).ToString(programConsts.curFormat);
            TxtLstMainPaymentAmountTotal.Text = (incomingPaymentTotal - outgoingPaymentTotal).ToString(programConsts.curFormat);
        }
        private void ClearFields()
        {
            TxtInvoiceNo.Text = "";
            TxtPaymentAmount.Text = "";
            TxtRemarks.Text = "";
            TxtCashBookDesc.Text = "";
        }

        private void LstMain_Delete(object sender, RoutedEventArgs e)
        {
            if (LstMain.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select an item from Main List!");
                return;
            }

            Fiscal.Miscellaneous miscellaneous = LstMain.SelectedItem as Fiscal.Miscellaneous;
            if (!string.IsNullOrEmpty(miscellaneous.CommitNumber))
            {
                _ = MessageBox.Show("The item has taken commit number therefor you cannot delete it!" + Environment.NewLine +
                    "Please inform Fiscal Officer'");
                return;
            }

            Fiscal.MiscellaneousMain miscellaneousMain = new Fiscal.MiscellaneousMain();
            if (!miscellaneousMain.DeleteDataFromDisbursing(miscellaneous))
            {
                _ = MessageBox.Show("The data couldn't delete!");
                return;
            }

            _ = MessageBox.Show("Data is removed!");
            miscellaneousMain.Clear();
            miscellaneousMain.InitList();
            LstMain.ItemsSource = miscellaneousMain;
            SumLstMain();
        }

        private void SelectItemForSaving(object sender, RoutedEventArgs e)
        {
            if (LstMain.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select an item!");
                return;
            }

            Fiscal.Miscellaneous miscellaneous = LstMain.SelectedItem as Fiscal.Miscellaneous;

            if (!string.IsNullOrEmpty(miscellaneous.InvoiceNumber))
            {
                _ = MessageBox.Show("The item is already paid!");
                return;
            }

            TxtPaymentAmount.Text = miscellaneous.PlannedPayment.ToString();
            TxtRemarks.Text = miscellaneous.Remarks;
        }
    }
}
