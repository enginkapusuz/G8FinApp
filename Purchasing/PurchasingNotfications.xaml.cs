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
    /// Interaction logic for PurchasingNotfications.xaml
    /// </summary>
    public partial class PurchasingNotfications : Window
    {
        Contract contract;
        public NotificationMain notificationMain = new NotificationMain();
        public PurchasingNotfications(Contract _contract)
        {
            InitializeComponent();

            contract = _contract;
            TxtBiddingName.Text = contract.BiddingName;
            TxtPcAmount.Text = contract.PcAmount.ToString("#,#.00");

            notificationMain.InitBiddingList(contract.BiddingId);
            OrderMainList();
            LstMain.ItemsSource = notificationMain;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            Notification notification;

            if (string.IsNullOrEmpty(TxtItemNu.Text))
            {
                _ = MessageBox.Show("Item Nu is empty!");
                return;
            }

            if (string.IsNullOrEmpty(TxtBidDate.Text))
            {
                _ = MessageBox.Show("Bid Date is empty!");
                return;
            }

            if (string.IsNullOrEmpty(TxtBidAmount.Text))
            {
                _ = MessageBox.Show("Bid Amount is empty!");
                return;
            }

            if (string.IsNullOrEmpty(TxtCompanyId.Text))
            {
                _ = MessageBox.Show("Company is empty!");
                return;
            }

            if (!DateTime.TryParse(TxtBidDate.Text, out DateTime dtTimeBidDate))
            {
                _ = MessageBox.Show("Bid Date is not proper!");
                return;
            }

            if (!decimal.TryParse(TxtBidAmount.Text, out decimal dcmlBidAmount) || !(dcmlBidAmount > 0))
            {
                _ = MessageBox.Show("Bid Amount is not proper!");
                return;
            }


            notification = new Notification()
            {
                ID = TxtItemNu.Text,
                BidDate = dtTimeBidDate,
                BiddingId = contract.BiddingId,
                BidAmount = dcmlBidAmount,
                CompanyId = TxtCompanyId.Text.Trim(),
            };

            List<Notification> isCmpExitst = notificationMain.Where(ntf => ntf.CompanyId.ToUpper() == notification.CompanyId.ToUpper()).ToList();

            if (isCmpExitst.Count() > 0)
            {
                _ = MessageBox.Show("The Company is already in the list!");
                return;
            }

            notificationMain.Add(notification);
            OrderMainList();
        }

        private void LstMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (LstMain.Items.Count == 0)
            {
                return;
            }

            if (e.Key != Key.Delete)
            {
                return;
            }

            Notification selNotification = LstMain.SelectedItem as Notification;
            int indx = notificationMain.IndexOf(selNotification);
            if (indx == -1)
            {
                return;
            }

            notificationMain.Remove(selNotification);
            OrderMainList();
            LstMain.ItemsSource = notificationMain;
        }

        private void OrderMainList()
        {
            int indx = 1;
            var newNotificationMain = notificationMain.OrderBy(ntf => ntf.BidAmount)
                .Select(ntf => new Notification { ID = indx++.ToString(), BiddingId = ntf.BiddingId, BidDate = ntf.BidDate, BidAmount = ntf.BidAmount, CompanyId = ntf.CompanyId });

            LstMain.ItemsSource = newNotificationMain;
            notificationMain = new NotificationMain();
            foreach (var ntf in LstMain.Items)
            {
                notificationMain.Add(ntf as Notification);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (LstMain.Items.Count == 0)
            {
                _ = MessageBox.Show("The List is empty!");
                return;
            }

            if (!notificationMain.DeleteData(contract.BiddingId))
            {
                return;
            }

            if (MessageBox.Show("Do you want to save Notifications!", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                _ = MessageBox.Show("The process has been cancelled!");
                return;
            }

            if (!notificationMain.SaveData())
            {
                return;
            }

            _ = MessageBox.Show("Data is saved!");

            Close();
        }
    }
}
