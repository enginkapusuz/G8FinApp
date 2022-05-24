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
    /// Interaction logic for PaymentCashBook.xaml
    /// </summary>
    /// 

    public partial class PaymentCashBook : Window
    {
        PaymentListWithItems _paymentListWithItems;
        CashBookMain cashbookMain;
        List<Account> accountList;
        
        public PaymentCashBook(PaymentListWithItems paymentListWithItems, InvoiceAccountMain invoiceAccontMain)
        {
            InitializeComponent();
            cashbookMain = new CashBookMain();
            cashbookMain.TakeAccountsTotal(invoiceAccontMain);
            LstAddedAccount.ItemsSource = cashbookMain;
            _paymentListWithItems = paymentListWithItems;
        }

        private void txtAccountNumberOne_SelectionChange(object sender, SelectionChangedEventArgs e)
        {
            Account account = txtAccountNumberOne.SelectedValue as Account;
            txtCurrencyOne.Text = account.AccountCurr;
        }

        private void txtAmountOne_LostFocus(object sender, RoutedEventArgs e)
        {
            decimal _dcmlAmount;

            if (string.IsNullOrEmpty(txtAmountOne.Text))
            {
                return;
            }

            if (!decimal.TryParse(txtAmountOne.Text, out decimal dcmlAmount))
            {
                txtAmountOne.Text = "";
                return;
            }
            else
            {
                _dcmlAmount = dcmlAmount;
            }

            txtAmountOne.Text = _dcmlAmount.ToString("N4");
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            Account account;
            AccountTransaction accountTransaction;

            if (string.IsNullOrEmpty(txtAccountNumberOne.Text))
            {
                _ = MessageBox.Show("Account is Empty!");
                return;
            }
            if(string.IsNullOrEmpty(txtAmountOne.Text))
            {
                _ = MessageBox.Show("Amount is Empty!");
                return;
            }

            account = txtAccountNumberOne.SelectedValue as Account;

            accountTransaction = new AccountTransaction()
            {
                AccountId = account.ID,
                AccountNu = account.AccountNu,
                AccountCurr = account.AccountCurr,
                AccInTotal = account.AccInTotal,
                TransAmount = decimal.Parse(txtAmountOne.Text),
                AccountName = account.AccountName,
                TransRemark = txtTransactionOne.Text,
            };
            cashbookMain.Add(accountTransaction);
            LstAddedAccount.ItemsSource = cashbookMain;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            BookCash bookCash;
            BookCashMain bookCashMain;
            AccountInMain accountInMain;
            AccountTrans accountTrans;
            AccountOutMain accountOutMain;

            if (string.IsNullOrEmpty(txtDescription.Text))
            {
                _ = MessageBox.Show("Description is empty!");
                _ = txtDescription.Focus();
                return;
            }

            if (accountList is null)
            {
                _ = MessageBox.Show("Balance Table is empty!");
                return;
            }

            if (!accountList.All(acc => acc.AccInTotal >= 0))
            {
                _ = MessageBox.Show("There is negative account");
                return;
            }

            if (!DateTime.TryParse(txtCashBookDate.Text, out DateTime dtTmDate))
            {
                _ = MessageBox.Show("Date is not proper!");
                return;
            }

            if (MessageBox.Show("Do you want to save Cash Book Data?", "Approve!", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                _ = MessageBox.Show("Saving process is cancelled!");
                return;
            }

            bookCash = new BookCash()
            {
                Description = txtDescription.Text,
                CashBookDate = dtTmDate,
            };

            bookCashMain = new BookCashMain();

            if (!bookCashMain.SaveData(bookCash))
            {
                return;
            }

            #region DisbursingInTransactionsSavings

            if (!bookCashMain.TakeLastIdNumber())
            {
                return;
            }

            IEnumerable<AccountTransaction> incomeTrans = from trans in cashbookMain
                                                          where trans.TransRemark == "TransferIn" || trans.TransRemark == "Received"
                                                          select trans;

            accountInMain = new AccountInMain(isCollectionEmpty: true);

            foreach (var trans in incomeTrans)
            {
                accountTrans = new AccountTrans()
                {
                    AccountId = trans.AccountId,
                    TransAmount = trans.TransAmount,
                    TransDate = trans.TransDate,
                    TransRemark = trans.TransRemark,
                    CashBookId = bookCashMain.LastIdNu,
                };


                if (!accountInMain.SaveData(accountTrans))
                {
                    return;
                }
            }
            #endregion

            #region DisbursingOutTransactionsSavings

            IEnumerable<AccountTransaction> outComeTrans = from trans in cashbookMain
                                                           where trans.TransRemark == "TransferOut" || trans.TransRemark == "Payment"
                                                           select trans;

            accountOutMain = new AccountOutMain(isCollectionEmpty: true);

            foreach (var trans in outComeTrans)
            {
                accountTrans = new AccountTrans()
                {
                    AccountId = trans.AccountId,
                    TransAmount = trans.TransAmount,
                    TransDate = trans.TransDate,
                    TransRemark = trans.TransRemark,
                    CashBookId = bookCashMain.LastIdNu,
                };

                if (!accountOutMain.SaveData(accountTrans))
                {
                    return;
                }
            }
            #endregion

            _ = MessageBox.Show("CashBook is Saved!");
            _paymentListWithItems.isCashBookSaved = true;
            _paymentListWithItems.cashBookDate = dtTmDate;
            _paymentListWithItems.bookCashLastId = bookCashMain.LastIdNu;
            Close();
        }

        private void CalcBtn_Click(object sender, RoutedEventArgs e)
        {
            CashCurrentAmount cashCurrentAmount = new CashCurrentAmount();
            accountList = cashCurrentAmount.CalculateCurrrentAmount(cashbookMain);
            LstAccountBalance.ItemsSource = accountList;
        }
    }
}
