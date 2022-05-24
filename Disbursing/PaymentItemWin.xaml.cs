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
    /// Interaction logic for PaymentItemWin.xaml
    /// </summary>
    public partial class PaymentItemWin : Window
    {
        private readonly string ApproveId;
        private readonly Invoice _invoice;
        private const string curFormat = "#,#.0000";
        public PaymentItemWin(Invoice invoice)
        {
            InitializeComponent();

            _invoice = invoice;

            ApproveId = invoice.PaymentListId;

            txtCompanyName.Text = invoice.CompanyName.Trim();
            txtInvNu.Text = invoice.InvNu.Trim();
            txtInDate.Text = invoice.InvDate.ToString("d");
            txtPayAmount.Text = invoice.PayAmount.ToString(curFormat);
           

            int indx = 0;

            foreach(object cr in txtCurrency.Items)
            {
                if (cr.ToString().Contains(invoice.PayCurr))
                {
                    txtCurrency.SelectedIndex = indx;
                    break;
                }

                indx++;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Invoice invoice;
            InvoiceMain invoiceMain;

            ApproveMain approveMain;
            Approve approve = null;

            DateTime _dtTmDate;
            decimal _dcmlPayAmount;

            if(string.IsNullOrEmpty(txtCompanyName.Text))
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
                _ = MessageBox.Show("IBAN Nu is empty!");
                return;
            }

            if (string.IsNullOrEmpty(txtInvNu.Text))
            {
                _ = MessageBox.Show("Invoice Number is empty!");
                return;
            }

            if (!DateTime.TryParse(txtInDate.Text, out DateTime dtTmDate))
            {
                _ = MessageBox.Show("Date is not proper!");
                return;
            }
            else
            {
                _dtTmDate = dtTmDate;
            }

            if (!decimal.TryParse(txtPayAmount.Text, out decimal dcmlPayAmount))
            {
                _ = MessageBox.Show("Pay amount is not proper!");
                return;
            }
            else
            {
                _dcmlPayAmount = dcmlPayAmount;
            }

            if (_dcmlPayAmount <= 0)
            {
                _ = MessageBox.Show("Pay amount should be positive!");
                return;
            }

            if (string.IsNullOrEmpty(txtCurrency.Text))
            {
                _ = MessageBox.Show("Currency is empty!");
                return;
            }

            invoice = new Invoice()
            {
                //PaymentListId = paymentList.ID,
                CompanyName = txtCompanyName.Text.Trim(),
                CompanyAddress = txtCompanyAddress.Text.Trim(),
                BankName = txtBankName.Text.Trim(),
                IBANNu = txtIBANNu.Text.Trim(),
                InvNu = txtInvNu.Text.Trim(),
                InvDate = _dtTmDate,
                PayAmount = _dcmlPayAmount,
                PayCurr = txtCurrency.Text.Trim().Split('{')[2],

                BdgtCurr = _invoice.BdgtCurr,
                BdgtAmount = _invoice.BdgtAmount,
                MainID = _invoice.MainID,
                EncumbId = _invoice.EncumbId,
            };

            invoiceMain = new InvoiceMain(invoice);

            #region approveMatch
            approveMain = new ApproveMain();

            foreach(Approve apr in approveMain)
            {
                if (apr.ID == ApproveId)
                {
                    approve = apr;
                }
            }
            
            if (approve == null)
            {
                _ = MessageBox.Show("Error: Approve Id couldn't find");
                return;
            }

            approve.ApproveChoice = "InPayList";
            approve.AppDate = DateTime.Parse(DateTime.Now.ToString("d"));
            approve.SendTo = "Disbursing";

            #endregion

            if (!invoiceMain.SaveData(invoice))
            {
                _ = MessageBox.Show("Data couldn't save!(Invoice Table)");
                return;
            }

            if(!approveMain.SaveData(approve))
            {
                _ = MessageBox.Show("Data couldn't save!(Approve Table)");
                return;
            }

            _ = MessageBox.Show("Data is saved!");

            Close();
        }
    }
}
