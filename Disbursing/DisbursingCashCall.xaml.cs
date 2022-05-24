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
    /// Interaction logic for DisbursingCashCall.xaml
    /// </summary>
    public partial class DisbursingCashCall : Window
    {
        Database.ProgramConsts prgrmConsts = new Database.ProgramConsts();
        public DisbursingCashCall()
        {
            CashCallMain cashCallMain;
            InitializeComponent();
            cashCallMain = new CashCallMain();
            cashCallMain.InitList();
            LstMain.ItemsSource = cashCallMain;
            SumLstMain();
        }

        private void LstMain_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Fiscal.CashCall cashCall;
            if (LstMain.SelectedIndex == -1)
            {
                return;
            }

            cashCall = LstMain.SelectedItem as Fiscal.CashCall;
            if (cashCall.PaymentAmount > 0)
            {
                _ = MessageBox.Show("The Item is already paid!");
                return;
            }

            InitFields(cashCall);
        }


        private void TxtPaymentAmount_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!decimal.TryParse(TxtPaymentAmount.Text, out decimal dcmlPaymentAmount))
            {
                return;
            }

            TxtPaymentAmount.Text = dcmlPaymentAmount.ToString(prgrmConsts.curFormat);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            CashCallMain cashCallMain;
            Fiscal.CashCall cashCall;

            if(!DateTime.TryParse(TxtPaymentDate.Text, out DateTime dtTmPaymentDate))
            {
                _ = MessageBox.Show("Payment Date is empty!");
                return;
            }

            if (!decimal.TryParse(TxtPaymentAmount.Text, out decimal dcmlPaymentAmount))
            {
                _ = MessageBox.Show("Payment Amount is not proper!");
                return;
            }

            if (dcmlPaymentAmount > decimal.Parse(TxtPlannedPayment.Text))
            {
                if (MessageBox.Show("Payment Amount is greater than Planned Payment!" +  
                    Environment.NewLine + "Do you want to save the Payment?", 
                    "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }
            }

            if (string.IsNullOrEmpty(TxtInvoiceNumber.Text))
            {
                _ = MessageBox.Show("Invoice Number is empty!");
                return;
            }

            cashCall = LstMain.SelectedItem as Fiscal.CashCall;
            cashCall.PaymentDate = dtTmPaymentDate;
            cashCall.PaymentAmount = dcmlPaymentAmount;
            cashCall.InvoiceNumber = TxtInvoiceNumber.Text;

            cashCallMain = new CashCallMain();
            if (!cashCallMain.UpdateData(cashCall))
            {
                _ = MessageBox.Show("Data couldn't update!");
                return;
            }

            _ = MessageBox.Show("Data is updated!");

            cashCallMain.InitList();
            LstMain.ItemsSource = cashCallMain;
            ClearFields();
            SumLstMain();
        }

        private void LstMain_Delete(object sender, RoutedEventArgs e)
        {
            Fiscal.CashCall cashCall;
            CashCallMain cashCallMain;

            if (LstMain.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select an Item!");
                return;
            }

            cashCall = LstMain.SelectedItem as Fiscal.CashCall;

            if (!string.IsNullOrEmpty(cashCall.CommitNumber))
            {
                _ = MessageBox.Show("The Payment has been committed!");
                return;
            }

            cashCallMain = new CashCallMain();

            if (!cashCallMain.DeletePaymentData(cashCall))
            {
                _ = MessageBox.Show("The Payment couldn't removed!");
                return;
            }

            _ = MessageBox.Show("The Payment is successful.");

            cashCallMain.InitList();
            LstMain.ItemsSource = cashCallMain;
            SumLstMain();
        }

        public void ClearFields()
        {
            TxtCountries.Text = "";
            TxtPlannedPayment.Text = "";
            TxtPaymentDate.Text = "";
            TxtRemarks.Text = "";
            TxtPaymentAmount.Text = "";
            TxtInvoiceNumber.Text = "";
        }

        private void SumLstMain()
        {
            CashCallMain cashCallMain = LstMain.ItemsSource as CashCallMain;
            TxtLstMainPlannedPaymentTotal.Text = cashCallMain.Select(cshCall => cshCall.PlannedPayment).Sum().ToString(prgrmConsts.curFormat);
            TxtLstMainPaymentAmountTotal.Text = cashCallMain.Select(cshCall => cshCall.PaymentAmount).Sum().ToString(prgrmConsts.curFormat);
        }

        private void InitFields(Fiscal.CashCall cashCall)
        {
            TxtCountries.Text = cashCall.CountryName;
            TxtPlannedPayment.Text = cashCall.PlannedPayment.ToString(prgrmConsts.curFormat);
            TxtRemarks.Text = cashCall.Remarks;
        }
    }
}
