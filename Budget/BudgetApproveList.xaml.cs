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

namespace G8FinApp.Budget
{
    /// <summary>
    /// Interaction logic for BudgetApproveList.xaml
    /// </summary>
    public partial class BudgetApproveList : Window
    {
        public BudgetApproveList()
        {
            ApproveListMain approveListMain = new ApproveListMain();
            InitializeComponent();
            approveListMain.InitList();
            LstMain.ItemsSource = approveListMain;
        }

        private void BtnOpenSendTo_Click(object sender, RoutedEventArgs e)
        {
            Approve approve;
            EncumbranceSendTo encumbSendTo;

            if (LstMain.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select a record from Main List!");
                return;
            }

            approve = LstMain.SelectedItem as Approve;
            encumbSendTo = new EncumbranceSendTo(approve);
            _ = encumbSendTo.ShowDialog();
            Close();
        }
    }
}
