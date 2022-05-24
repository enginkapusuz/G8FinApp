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
    /// Interaction logic for NewOpenAmount.xaml
    /// </summary>
    public partial class NewOpenAmount : Window
    {
        public NewOpenAmount(string accountId)
        {
            InitializeComponent();
            txtAccountId.Text = accountId;
            txtRemark.Text = "OPEN";
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            AccountTrans accountTrans;
            AccountInMain accountInMain;

            decimal _dcmlAmount;
            DateTime _dtTmDate;

            if (string.IsNullOrEmpty(txtOpenAmount.Text))
            {
                _ = MessageBox.Show("Amount is empty!");
                return;
            }

            if (!decimal.TryParse(txtOpenAmount.Text, out decimal dcmlAmount))
            {
                _ = MessageBox.Show("Amount is not proper!");
                return;
            }
            else
            {
                _dcmlAmount = dcmlAmount;
            }

            if(!DateTime.TryParse(txtDate.Text, out DateTime dtTmDate))
            {
                _ = MessageBox.Show("Date is empty!");
                return;
            }
            else
            {
                _dtTmDate = dtTmDate;
            }

            accountTrans = new AccountTrans()
            {
                AccountId = txtAccountId.Text.Trim().ToUpper(),
                TransAmount = _dcmlAmount,
                TransDate = _dtTmDate,
                TransRemark = txtRemark.Text.Trim().ToUpper(),
                CashBookId = "0",
            };

            accountInMain = new AccountInMain(isCollectionEmpty:true);

            if (!accountInMain.SaveData(accountTrans))
            {
                _ = MessageBox.Show("Data couldn't saved");
                Close();
                return;
            }
            _ = MessageBox.Show("Data is saved");

            Close();
        }

        private void txtOpenAmount_LostFocus(object sender, RoutedEventArgs e)
        {
            decimal _dcmlAmount = 0;

            if(!decimal.TryParse(txtOpenAmount.Text, out decimal dcmlAmount))
            {
                return;
            }
            else
            {
                _dcmlAmount = dcmlAmount;
            }

            txtOpenAmount.Text = _dcmlAmount.ToString("N4");
        }
    }
}
