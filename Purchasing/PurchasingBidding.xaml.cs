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
    /// Interaction logic for PurchasingBidding.xaml
    /// </summary>
    public partial class PurchasingBidding : Window
    {
        Fiscal.Pending pending;
        Approve approve = null;
        public PurchasingBidding(Approve _approve)
        {
            BiddingMain biddingMain = new BiddingMain(); 

            InitializeComponent();

            biddingMain.InitList();
            LstMain.ItemsSource = biddingMain;

            if (_approve is null)
            {
                return;
            }

            approve = _approve;
            TxtBiddingName.Text = approve.ReqDesc;
            TxtBiddingPrice.Text = approve.ReqAmount.ToString("#,#.0000");
            TxtBiddingCurr.Text = approve.ReqCurr;
        }

        private void BtnBiddingSave_Click(object sender, RoutedEventArgs e)
        {
            if (approve is null)
            {
                _ = MessageBox.Show("You have to select an item from Main List!");
                return;
            }
            
            BiddingMain biddingMain = new BiddingMain();
            ApproveMain approveMain = new ApproveMain();

            if (string.IsNullOrEmpty(TxtBiddingName.Text))
            {
                _ = MessageBox.Show("Bidding name is empty!");
                return;
            }

            if (!int.TryParse(TxtLines.Text, out int intLines))
            {
                _ = MessageBox.Show("Lines is not proper!");
                return;
            }

            if (!DateTime.TryParse(TxtBiddingOpenDate.Text, out DateTime dtTmBiddingOpenDate))
            {
                _ = MessageBox.Show("Open Date is not proper!");
                return;
            }

            if (!DateTime.TryParse(TxtBiddingCloseDate.Text, out DateTime dtTmBiddingCloseDate))
            {
                _ = MessageBox.Show("Close Date is not proper!");
                return;
            }

            if (dtTmBiddingCloseDate <= dtTmBiddingOpenDate)
            {
                _ = MessageBox.Show("Close date is less than open date!");
                return;
            }

            if (string.IsNullOrEmpty(TxtPointOfContact.Text))
            {
                _ = MessageBox.Show("Point of Contact is empty!");
                return;
            }

            Bidding bidding = new Bidding()
            {
                ApproveId = approve.ID,
                BiddingName = TxtBiddingName.Text,
                Lines = intLines.ToString(),
                BiddingOpenDate = DateTime.Parse(TxtBiddingOpenDate.Text),
                BiddingCloseDate = DateTime.Parse(TxtBiddingCloseDate.Text),
                //BiddingPrice = decimal.Parse(TxtBiddingPrice.Text),
                BiddingPrice = approve.ReqAmount,
                BiddingCurr = TxtBiddingCurr.Text,
                PointOfContact = TxtPointOfContact.Text,
                PofConPhone = TxtPofConPhone.Text,
            };

            if (!biddingMain.SaveData(bidding))
            {
                _ = MessageBox.Show("Bidding data couldn't saved!");
                return;
            }


            pending = new Fiscal.Pending();
            pending.APPROVECHOICE = "BiddingOpen";
            pending.ID = approve.ID;

            if(!approveMain.UpdateData(pending))
            {
                _ = MessageBox.Show("Approve data couldn't saved!");
                return;
            }

            _ = MessageBox.Show("Data is saved!");
            biddingMain.InitList();
            LstMain.ItemsSource = biddingMain;

            approve = null;
            TxtBiddingName.Text = "";
            TxtLines.Text = "";
            TxtBiddingOpenDate.Text = "";
            TxtBiddingCloseDate.Text = "";
            TxtBiddingPrice.Text = "";
            TxtBiddingCurr.Text = "";
            TxtPointOfContact.Text = "";
            TxtPofConPhone.Text = "";

            Close();
        }

        private void OpenPurchasingItems(object sender, RoutedEventArgs e)
        {
            Bidding bidding;
            PurchasingItems purchasingItems;

            if (LstMain.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select Tender from List!");
                return;
            }

            bidding = LstMain.SelectedItem as Bidding;
            purchasingItems = new PurchasingItems(bidding);
            purchasingItems.ShowDialog();
        }

        private void OpenPurchasingContract(object sender, RoutedEventArgs e)
        {
            Bidding bidding = null;
            BiddingMain biddingMain = new BiddingMain();

            PurchasingContract purchasingContract;

            if (!(LstMain.SelectedIndex == -1))
            {
                bidding = LstMain.SelectedItem as Bidding;
            }

            purchasingContract = new PurchasingContract(bidding);
            purchasingContract.ShowDialog();

            biddingMain.InitList();
            LstMain.ItemsSource = biddingMain;
        }

        private void PrintFileCover01(object sender, RoutedEventArgs e)
        {
            const string fileName = "01-ContractFileCover";

            Bidding bidding = TakeBidding();
            if (bidding is null)
            {
                _ = MessageBox.Show("Please select a bidding!");
                return;
            }

            WriteReportFile(fileName, InitDictReport(bidding));
        }

        private void PrintFileCover02(object sender, RoutedEventArgs e)
        {
            const string fileName = "02-ContractFileCover";

            Bidding bidding = TakeBidding();
            if (bidding is null)
            {
                _ = MessageBox.Show("Please select a bidding!");
                return;
            }

            WriteReportFile(fileName, InitDictReport(bidding));
        }

        private Dictionary<string, string> InitDictReport(Bidding bidding)
        {
            Dictionary<string, string> dictReport = new Dictionary<string, string>();
            dictReport["BiddingName"] = bidding.BiddingName;
            dictReport["FmNo"] = "'" + bidding.FMNo.PadLeft(2, '0');
            dictReport["Year-01"] = "'" + DateTime.Now.ToString("yy");
            dictReport["Year-02"] = "'" + DateTime.Now.ToString("dd").PadLeft(2, '0');
            dictReport["Month-01"] = "'" + DateTime.Now.ToString("MM").PadLeft(2, '0');
            dictReport["Year-03"] = "'" + DateTime.Now.ToString("yyyy");
            dictReport["CisiCode-01"] = "'" + bidding.CisiCode;
            dictReport["Company-01"] = "";
            dictReport["PendingNo-01"] = bidding.PendingNo;

            return dictReport;
        }

        private void WriteReportFile(string fileName, Dictionary<string, string> dictReport)
        {
            Reports.ReportFile reportFile;
            Reports.ReportExcel reportExcel;
            Reports.ReportRange reportRange;

            reportFile = new Reports.ReportFile().TakeFileData(fileName);
            if (reportFile is null)
            {
                _ = MessageBox.Show("File couldn't be found!");
                return;
            }

            reportExcel = new Reports.ReportExcel().OpenTemplateFile(reportFile);
            if (reportExcel is null)
            {
                _ = MessageBox.Show("Excel couldn't open the template!");
                return;
            }

            reportRange = new Reports.ReportRange().TakeRange(reportExcel, reportFile);
            if (reportRange is null)
            {
                _ = MessageBox.Show("Sheet range couldn't take!");
                return;
            }

            if (!reportRange.WriteRange(dictReport))
            {
                _ = MessageBox.Show("Data couldn't write to range!");
                return;
            }
        }

        private Bidding TakeBidding()
        {
            if (LstMain.SelectedIndex == -1)
            {
                return null;
            }

            return LstMain.SelectedItem as Bidding;
        }

        private void CancelBidding(object sender, RoutedEventArgs e)
        {
            Bidding bidding;
            BiddingMain biddingMain;
            Approve approve;
            ApproveMain approveMain = new ApproveMain();

            if (LstMain.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select an item!");
                return;
            }

            bidding = LstMain.SelectedItem as Bidding;

            biddingMain = new BiddingMain();

            if (!biddingMain.DeleteBidding(bidding))
            {
                _ = MessageBox.Show("Deletion is unsuccessful!(Purchasing Bidding Table)");
                return;
            }

            approve = new Approve()
            {
                ID = bidding.ApproveId,
                ApproveChoice = "Empty",
            };

            if (!approveMain.UpdateApprove(approve))
            {
                _ = MessageBox.Show("Deletion is unsuccessful!(Purchasing ApproveTable)");
                return;
            }

            biddingMain.InitList();
            LstMain.ItemsSource = biddingMain;

            MessageBox.Show("Cancellation is successful!");
        }

        private void OpenNewNotification(object sender, RoutedEventArgs e)
        {
            Contract contract;
            Bidding bidding;
            PurchasingNotfications purchasingNotfications;



            if (LstMain.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select an item!");
                return;
            }
            
            bidding = LstMain.SelectedItem as Bidding;
            contract = new Contract()
            {
                BiddingName = bidding.BiddingName,
                PcAmount = bidding.BiddingPrice,
                BiddingId = bidding.ID,
            };

            purchasingNotfications = new PurchasingNotfications(contract);
            purchasingNotfications.ShowDialog();
        }
    }
}
