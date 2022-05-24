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
    /// Interaction logic for NewAccount.xaml
    /// </summary>
    public partial class NewAccount : Window
    {
        public NewAccount()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Account account;
            AccountMain accountMain;

            if (string.IsNullOrEmpty(txtAccountName.Text))
            {
                _ = MessageBox.Show("Account name is empty!");
                return;
            }

            if (string.IsNullOrEmpty(txtAccountNu.Text))
            {
                _ = MessageBox.Show("Account number is empty!");
                return;
            }

            if (string.IsNullOrEmpty(txtCurr.Text))
            {
                _ = MessageBox.Show("Currency is empty!");
                return;
            }

            account = new Account();
            accountMain = new AccountMain();

            account.AccountName = txtAccountName.Text.Trim().ToUpper();
            account.AccountNu = txtAccountNu.Text.Trim().ToUpper();
            account.AccountCurr = txtCurr.Text.Split('{')[2];

            foreach(Account acc in accountMain)
            {
                if (acc.AccountNu == account.AccountNu)
                {
                    _ = MessageBox.Show("Account is already in table!");
                    return;
                }
            }

            if (accountMain.SaveData(account))
            {
                _ = MessageBox.Show("Data is saved!");
                accountMain = new AccountMain();
                LstMain.ItemsSource = accountMain;
                
                txtAccountName.Text = "";
                txtAccountNu.Text = "";
                txtCurr.Text = "";

                return;
            }
        }

        private void OpenNewAmount(object sender, RoutedEventArgs e)
        {
            Account account;
            NewOpenAmount newOpenAmount;

            if (LstMain.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select a record from Main List!");
                return;
            }

            account = LstMain.SelectedItem as Account;
            newOpenAmount = new NewOpenAmount(account.ID);
            newOpenAmount.ShowDialog();
            LstMain.ItemsSource = new AccountMain();
        }
    }
}
