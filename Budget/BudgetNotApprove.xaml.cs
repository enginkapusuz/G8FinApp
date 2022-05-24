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
    /// Interaction logic for BugdetApprove.xaml
    /// </summary>
    public partial class BudgetNotApprove : Window
    {
        public BudgetNotApprove()
        {
            InitializeComponent();
        }

        private void CancelBidding(object sender, RoutedEventArgs e)
        {
            NotApprove notApprove;
            NotApproveMain notApproveMain = new NotApproveMain(true);

            if (MessageBox.Show("Do you want to cancel this Request!", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            if (LstMain.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select an record from Main List!");
                return;
            }

            notApprove = LstMain.SelectedItem as NotApprove;

            notApprove.ApproveChoice = "Cancelled";

            if (!notApproveMain.UpdateData(notApprove))
            {
                _ = MessageBox.Show("Cancel operation is not successful!");
                return;
            }

            if (!notApproveMain.DeleteData(notApprove))
            {

                _ = MessageBox.Show("Cancel operation is not successful!");
                return;
            }

            _ = MessageBox.Show("Cancellation is successful");

            LstMain.ItemsSource = new NotApproveMain();

            return;
        }
    }
}
