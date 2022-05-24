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

namespace G8FinApp.Purchasing
{
    /// <summary>
    /// Interaction logic for PurchasingMain.xaml
    /// </summary>
    public partial class PurchasingMain : Window
    {
        public PurchasingMain()
        {
            ApproveMain approveMain = new ApproveMain();
            InitializeComponent();
            approveMain.InitList();
            LstMain.ItemsSource = approveMain;
        }

        private void OpenPurchasingBidding(object sender, RoutedEventArgs e)
        {
            Approve approve;
            ApproveMain approveMain = new ApproveMain();

            approve = LstMain.SelectedItem as Approve;
            PurchasingBidding purchasingBidding = new PurchasingBidding(approve);
            purchasingBidding.ShowDialog();

            approveMain.InitList();
            LstMain.ItemsSource = approveMain;
        }

        private void LstMain_Delete(object sender, RoutedEventArgs e)
        {
            ApproveMain approveMain;
            Approve approve;

            if (LstMain.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select an item!");
                return;
            }

            approve = LstMain.SelectedItem as Approve;
            if (approve is null)
            {
                return;
            }

            if(MessageBox.Show("Do you want to delete this record!", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }


            approveMain = new ApproveMain();
            approve.ApproveChoice = "Cancelled";
            if(!approveMain.UpdateApprove(approve))
            {
                _ = MessageBox.Show("The Item couldn't deleted!" +
                    Environment.NewLine +
                    "(Purchasing Table)");
                return;
            }

            approve.AppDate = DateTime.Now;
            approve.TableName = "Purchasing";
            approve.ApproveChoice = "Empty";
            approve.SendTo = "Purchasing";
            
            Fiscal.FiscalApproveMain fiscalAproveMain = new Fiscal.FiscalApproveMain();
            if(!fiscalAproveMain.InsertNewFiscalApprove(approve))
            {
                _ = MessageBox.Show("The Item couldn't deleted!" +
                    Environment.NewLine +
                    "(Fiscal Table)");
                return;
            }

            approveMain.InitList();
            LstMain.ItemsSource = approveMain;
        }
    }
}
