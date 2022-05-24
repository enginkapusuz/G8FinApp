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

namespace G8FinApp
{
    /// <summary>
    /// Interaction logic for UserLoginWin.xaml
    /// </summary>
    public partial class UserLoginWin : Window
    {
        public UserLoginWin()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {

            UserMain usrMain;
            Budget.BudgetMainWin bdgtMain;
            Fincon.FinconMain finconMainWin;
            Fiscal.FiscalMain fiscalMainWin;
            Disbursing.DisbursingMain disbursingMain;
            Purchasing.PurchasingMain purchasingMain;
            Admin.AdminMain adminMain;

            if (txtUsrRole.Text == "Fincon")
            {
                txtUsrName.Text = "Fincon";
                txtUsrPassword.Password = "Fincon";
            }
            else if(txtUsrRole.Text == "Budget")
            {
                txtUsrName.Text = "Budget";
                txtUsrPassword.Password = "Budget";
            }

            else if (txtUsrRole.Text == "Fiscal")
            {
                txtUsrName.Text = "Fiscal";
                txtUsrPassword.Password = "Fiscal";
            }

            else if (txtUsrRole.Text == "Disbursing")
            {
                txtUsrName.Text = "Disbursing";
                txtUsrPassword.Password = "Disbursing";
            }

            else if (txtUsrRole.Text == "PC")
            {
                txtUsrName.Text = "PC";
                txtUsrPassword.Password = "PC";
            }

            else if(txtUsrRole.Text == "Admin")
            {
                txtUsrName.Text = "Admin";
                txtUsrPassword.Password = "Admin";
            }

            if (string.IsNullOrEmpty(txtUsrName.Text))
            {
                _ = MessageBox.Show("User Name can't be empty!");
                txtUsrName.Focus();
                return;
            }
            else if(string.IsNullOrEmpty(txtUsrPassword.ToString()))
            {
                _ = MessageBox.Show("User Password can't be empty!");
                txtUsrPassword.Focus();
                return;
            }
            else
            { 
                usrMain = new UserMain(txtUsrName.Text.Trim(), txtUsrPassword.Password.ToString().Trim(), txtUsrRole.Text);

                Close();

                if (usrMain.UserCheck() &&
                    txtUsrRole.Text == "Budget")
                {
                    bdgtMain = new Budget.BudgetMainWin();
                    _ = bdgtMain.ShowDialog();
                }
                else if (usrMain.UserCheck() &&
                    txtUsrRole.Text == "Fincon")
                {
                    finconMainWin = new Fincon.FinconMain();
                    finconMainWin.ShowDialog();
                }
                else if (usrMain.UserCheck() &&
                    txtUsrRole.Text == "Fiscal")
                {
                    fiscalMainWin = new Fiscal.FiscalMain();
                    fiscalMainWin.ShowDialog();
                }
                else if (usrMain.UserCheck() &&
                    txtUsrRole.Text == "Disbursing")
                {
                    disbursingMain = new Disbursing.DisbursingMain();
                    disbursingMain.ShowDialog();
                }
                else if (usrMain.UserCheck() &&
                    txtUsrRole.Text == "PC")
                {
                    purchasingMain = new Purchasing.PurchasingMain();
                    purchasingMain.ShowDialog();
                }
                else if (usrMain.UserCheck() &&
                    txtUsrRole.Text == "Admin")
                {
                    adminMain = new Admin.AdminMain();
                    adminMain.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Login isn't correct");
                }
            }
        }
    }
}
