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
    /// Interaction logic for PaymentListWithItemUpdate.xaml
    /// </summary>
    public partial class PaymentListWithItemUpdate : Window
    {
        private const string curFormat = "#,#.0000";
        private readonly Invoice _invoice;
        private readonly PaymentListWithItems _paymentListWithItems;
        public PaymentListWithItemUpdate(Invoice invoice, PaymentListWithItems paymentListWithItems)
        {
            InitializeComponent();
            _invoice = invoice;
            _paymentListWithItems = paymentListWithItems;
            InitForm();
        }

        private void InitForm()
        {
            txtId.Text = _invoice.ID.Trim();
            txtCompanyName.Text = _invoice.CompanyName.Trim();
            txtCompanyAddress.Text = _invoice.CompanyAddress.Trim();

            txtBankName.Text = _invoice.BankName.Trim();
            txtIBANNu.Text = _invoice.IBANNu.Trim();
            txtInvNu.Text = _invoice.InvNu.Trim();

            txtInvDate.SelectedDate = _invoice.InvDate;
            txtInvAmount.Text = _invoice.PayAmount.ToString(curFormat);

            txtBlckExRate.Text = "Exchange Rate(" + _invoice.BdgtCurr + ")";
            txtExRate.Text = (_invoice.PayAmount / _invoice.BdgtAmount).ToString(curFormat);
            txtBdgtAmount.Text = _invoice.BdgtAmount.ToString(curFormat);

            int indx = 0;

            foreach (var cr in txtCurrency.Items)
            {
                txtCurrency.SelectedIndex = cr.ToString().Contains(_invoice.PayCurr) ? indx : -1;

                if (txtCurrency.SelectedIndex != -1)
                {
                    break;
                }
                indx++;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InvoiceMain invoiceMain;
            DateTime _dtTmDate;
            decimal _dcmlPayAmount;
            

            if (string.IsNullOrEmpty(txtCompanyName.Text))
            {
                _ = MessageBox.Show("Company Name is empty!");
                return;
            }

            if (string.IsNullOrEmpty(txtCompanyAddress.Text))
            {
                _ = MessageBox.Show("Company Address is empty!");
                return;
            }

            if (string.IsNullOrEmpty(txtBankName.Text))
            {
                _ = MessageBox.Show("Bank Name is empty!");
                return;
            }

            if (string.IsNullOrEmpty(txtIBANNu.Text))
            {
                _ = MessageBox.Show("IBAN Number is empty!");
                return;
            }

            if (string.IsNullOrEmpty(txtInvNu.Text))
            {
                _ = MessageBox.Show("Invoice Number is empty!");
                return;
            }

            if (!DateTime.TryParse(txtInvDate.Text, out DateTime dtTmDate))
            {
                _ = MessageBox.Show("Date is not correct!");
                return;
            }
            else
            {
                _dtTmDate = dtTmDate;
            }

            if (!decimal.TryParse(txtInvAmount.Text, out decimal dcmlPayAmount))
            {
                _ = MessageBox.Show("Invoice Amount is not correct!");
                return;
            }
            else
            {
                _dcmlPayAmount = dcmlPayAmount;
                txtInvAmount.Text = _dcmlPayAmount.ToString("N4");
            }

            if (string.IsNullOrEmpty(txtCurrency.Text))
            {
                _ = MessageBox.Show("Invoice Currency is empty!");
                return;
            }

            if (string.IsNullOrEmpty(txtBlckExRate.Text))
            {
                _ = MessageBox.Show("Exchange Rate is empty!");
                return;
            }

            if (!decimal.TryParse(txtExRate.Text, out decimal dcmlExRate))
            {
                _ = MessageBox.Show("Exchange Rate is not proper!");
                return;
            }

          
            decimal newBdgtAmount = decimal.Parse((_dcmlPayAmount / dcmlExRate).ToString(curFormat));

            Invoice invoice = new Invoice()
            {
                ID = txtId.Text.Trim(),
                CompanyName = txtCompanyName.Text.Trim(),
                CompanyAddress = txtCompanyAddress.Text.Trim(),
                BankName = txtBankName.Text.Trim(),
                IBANNu = txtIBANNu.Text.Trim(),
                InvNu = txtInvNu.Text.Trim(),
                InvDate = _dtTmDate,
                PayAmount = _dcmlPayAmount,
                PayCurr = txtCurrency.Text.Trim().Split('{')[2],

                BdgtCurr = _invoice.BdgtCurr,
                BdgtAmount = newBdgtAmount,

                MainID = _invoice.MainID,
                EncumbId = _invoice.EncumbId,
                ExRate = dcmlExRate,
            };

            invoiceMain = new InvoiceMain(invoice);

            if (invoiceMain.UpdateData())
            {
                _ = MessageBox.Show("Data is saved!");
                _paymentListWithItems.InitListMain();
                Close();
            }
            else
            {
                _ = MessageBox.Show("Data couldn't update!'");
                Close();
            }
        }

        private void TxtExRate_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(txtExRate.Text, out decimal dcmlExRate))
            {
                return;
            }

            if (txtCurrency.Text.Contains(_invoice.BdgtCurr) && dcmlExRate != 1)
            {
                _ = MessageBox.Show("Since Currencies are same Exchange Rate should be 1!");
                txtExRate.Text = "1";
                dcmlExRate = 1;
            }

            txtExRate.Text = dcmlExRate.ToString(curFormat);

            if(txtCurrency.Text.Contains(_invoice.PayCurr))
            {
                txtBdgtAmount.Text = (_invoice.PayAmount / dcmlExRate).ToString(curFormat);
                return;
            }

            if (!txtCurrency.Text.Contains(_invoice.PayCurr))
            {
                txtInvAmount.Text = (_invoice.BdgtAmount * dcmlExRate).ToString();
                return;
            }
        }
    }
}
