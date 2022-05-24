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

namespace G8FinApp.Admin
{
    /// <summary>
    /// Interaction logic for AdminMain.xaml
    /// </summary>
    public partial class AdminMain : Window
    {
        public AdminMain()
        {
            InitializeComponent();
        }

        private void DeleteBudgetTables(object sender, RoutedEventArgs e)
        {
            Deletion deletion = new Deletion();

            if (MessageBox.Show("Do you want to delete Budget Tables?", "Delete", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                _ = MessageBox.Show("Delete process has been cancelled!");
                return;
            }

            if (!deletion.DeleteTableGroup("BudgetTablesForDeletion"))
            {
                _ = MessageBox.Show("Deletion proceess has failed!");
                return;
            }
        }

        private void DeleteFiscalTables(object sender, RoutedEventArgs e)
        {
            Deletion deletion = new Deletion();

            if (MessageBox.Show("Do you want to delete Fiscal Tables?", "Delete", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                _ = MessageBox.Show("Delete process has been cancelled!");
                return;
            }

            if (!deletion.DeleteTableGroup("FiscalTablesForDeletion"))
            {
                _ = MessageBox.Show("Deletion proceess has failed!");
                return;
            }
        }

        private void DeleteFinconTables(object sender, RoutedEventArgs e)
        {
            Deletion deletion = new Deletion();

            if (MessageBox.Show("Do you want to delete Fincon Tables?", "Delete", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                _ = MessageBox.Show("Delete process has been cancelled!");
                return;
            }

            if (!deletion.DeleteTableGroup("FinconTablesForDeletion"))
            {
                _ = MessageBox.Show("Deletion proceess has failed!");
                return;
            }
        }

        private void DeletePurchasingTables(object sender, RoutedEventArgs e)
        {
            Deletion deletion = new Deletion();

            if (MessageBox.Show("Do you want to delete P&C Tables?", "Delete", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                _ = MessageBox.Show("Delete process has been cancelled!");
                return;
            }

            if (!deletion.DeleteTableGroup("PurchasingTablesForDeletion"))
            {
                _ = MessageBox.Show("Deletion proceess has failed!");
                return;
            }
        }

        private void DeleteDisbursingTables(object sender, RoutedEventArgs e)
        {
            Deletion deletion = new Deletion();

            if (MessageBox.Show("Do you want to delete Disbursing Tables?", "Delete", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                _ = MessageBox.Show("Delete process has been cancelled!");
                return;
            }

            if (!deletion.DeleteTableGroup("DisbursingTablesForDeletion"))
            {
                _ = MessageBox.Show("Deletion proceess has failed!");
                return;
            }

        }

        private void LoadBudgetData(object sender, RoutedEventArgs e)
        {
            Reports.ReportFile reportFile = new Reports.ReportFile().TakeFileData("BACKUP ALLOCATIONS2022", "BudgetMain");
            if (reportFile is null)
            {
                _ = MessageBox.Show("Report File is null!");
                return;
            }

            Reports.ReportExcel reportExcelBudget = new Reports.ReportExcel().OpenTemplateFile(reportFile, "BUDGET");
            if (reportExcelBudget is null)
            {
                _ = MessageBox.Show("BUDGET is null!");
                return;
            }

            AdminFundManager adminFundManager = new AdminFundManager();
            List<AdminFundManager> adminFundManagers = adminFundManager.LoadAdminFundManagersFromExcel(reportExcelBudget);
            if(adminFundManagers is null)
            {
                _ = MessageBox.Show("adminFundManagers is null!");
                return;
            }

            foreach(AdminFundManager adm in adminFundManagers)
            {
                foreach(FundManagerBudget fnBdgt in adm.fundManagerBudgets)
                {
                    Budget.BudgetNew budgetNew = new Budget.BudgetNew(adm, fnBdgt);
                    budgetNew.BtnSave.Focus();
                    budgetNew.ShowDialog();

                    Budget.BudgetMain budgetMain = new Budget.BudgetMain();
                    budgetMain.InitList();

                    var budgetMainId = budgetMain.Where(bd => bd.FMNO == adm.fundManager.FundManagerNo &&
                    bd.FMNAME == adm.fundManager.FundManagerName &&
                    bd.CISICODE == fnBdgt.CisiCode);
                    string bdgtCurr = string.Empty;

                    foreach (var idN in budgetMainId)
                    {
                        adm.fundManager.FundManagerID = int.Parse(idN.ID);
                        bdgtCurr = idN.BDGTCURR;
                        break;
                    }

                    if (fnBdgt.InitialBudget != 0)
                    {
                        Budget.BudgetEnterAmount budgetEnterAmount = new Budget.BudgetEnterAmount(adm, fnBdgt, bdgtCurr);
                        budgetEnterAmount.txtDate.Text = "01/01/" + DateTime.Now.ToString("yy");
                        budgetEnterAmount.BtnSave.Focus();
                        budgetEnterAmount.ShowDialog();
                    }

                    if (MessageBox.Show("Do you want to continue","Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    {
                        return;
                    }
                }
            }
        }

        private void LoadBudgetEncumbrance(object sender, RoutedEventArgs e)
        {
            Reports.ReportFile reportFile = new Reports.ReportFile().TakeFileData("BACKUP ALLOCATIONS2022");
            if (reportFile is null)
            {
                _ = MessageBox.Show("Report File is null!");
                return;
            }

            Reports.ReportExcel reportExcel = new Reports.ReportExcel().OpenTemplateFile(reportFile, "COMMIT");
            if (reportExcel is null)
            {
                _ = MessageBox.Show("Report Excel is null!");
                return;
            }

            List<FundManagerEncumbrance> adminEncumbrances = new AdminEncumbrance().LoadExcelCommitData(reportExcel);

            foreach (FundManagerEncumbrance fnEncumb in adminEncumbrances)
            {
                MessageBox.Show("CommitNu:" + fnEncumb.CommitNu + Environment.NewLine +
                    "ReqDesc:" + fnEncumb.ReqDesc + Environment.NewLine +
                    "ReqAmount:" + fnEncumb.ReqAmount.ToString() + Environment.NewLine + 
                    "FmNo:" + fnEncumb.FmNo + Environment.NewLine +
                    "FmName:" + fnEncumb.FmName + Environment.NewLine +
                    "CommitEuro:" + fnEncumb.CommitEuro);

                Budget.BudgetDataMain budgetDataMain = new Budget.BudgetDataMain();
                budgetDataMain.InitList();

                var budgetDataCheck = (from bdgtData in budgetDataMain
                                      where fnEncumb.ReqNum.Trim() == bdgtData.REQNUM.Trim() &&
                                      fnEncumb.ReqDesc.Trim() == bdgtData.REQDESC.Trim()
                                      select bdgtData).ToList();

                if (budgetDataCheck.Count > 0)
                {
                    MessageBox.Show(fnEncumb.CommitNu);
                    continue;
                }

                Budget.BudgetMain budgetMain = new Budget.BudgetMain();
                budgetMain.InitList();

                var bdgtMainQuery = budgetMain.Where(bd =>
                bd.FMNO.ToString().Trim() == fnEncumb.FmNo.Trim() &&
                bd.CISICODE.Trim() == fnEncumb.CisiCode.Trim() &&
                bd.FMNAME.Trim() == fnEncumb.FmName.Trim());

                foreach(var bdgt in bdgtMainQuery)
                {
                    fnEncumb.FundManagerID = int.Parse(bdgt.ID);
                    fnEncumb.FundBudgetAmount = (double)bdgt.CURRAMOUNT;
                    fnEncumb.FundBudgetCurr = bdgt.BDGTCURR;
                }

                Budget.BudgetEncumbrance budgetEncumbrance = new Budget.BudgetEncumbrance(fnEncumb);
                budgetEncumbrance.ShowDialog();

                if (MessageBox.Show("Do you want to continue", "aaa", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    break;
                }
                MessageBox.Show(fnEncumb.CommitNu);
            }
        }

        private void CheckBudgetMainandExcelBudget(object sender, RoutedEventArgs e)
        {
            Reports.ReportFile reportFile = new Reports.ReportFile().TakeFileData("BACKUP ALLOCATIONS2022", "BudgetMain");
            if (reportFile is null)
            {
                _ = MessageBox.Show("Report File is null!");
                return;
            }

            Reports.ReportExcel reportExcelBudget = new Reports.ReportExcel().OpenTemplateFile(reportFile, "FUND_MANAGER_ALL_BRANCH");
            if (reportExcelBudget is null)
            {
                _ = MessageBox.Show("BUDGET is null!");
                return;
            }

            List<BudgetExcel> budgetExcels = new BudgetExcel().LoadBudgetExcel(reportExcelBudget);

            if(budgetExcels is null)
            {
                _ = MessageBox.Show("Budget Excel is null!");
                return;
            }

            Budget.BudgetMain budgetMain = new Budget.BudgetMain();
            budgetMain.InitList();

            foreach (Budget.Budget bdgt in budgetMain)
            {
                var bdgtExcelCheck = budgetExcels
                    .Where(bdEx => bdEx.FmNo == bdgt.FMNO &&
                    bdEx.FmName.Trim() == bdgt.FMNAME.Trim() &&
                    bdEx.CisiCode.Trim() == bdgt.CISICODE.Trim()).ToList();

                if (bdgtExcelCheck.Count == 0)
                {
                    _ = MessageBox.Show("Budget Main item is not in Excel:" + Environment.NewLine +
                        bdgt.FMNO + Environment.NewLine +
                        bdgt.FMNAME + Environment.NewLine +
                        bdgt.CISICODE);

                    continue;
                }

                string budgetPart = string.Empty;
                string bdgtMainLine = string.Empty;
                string bdgtExcelLine = string.Empty;

                if (bdgtExcelCheck[0].InitialBudget.ToString("N4") != ((double)bdgt.INAMOUNT).ToString("N4"))
                {
                    budgetPart = "Initial is not equal!";
                    bdgtMainLine = "Initial BudgetMain:" + bdgt.INAMOUNT.ToString();
                    bdgtExcelLine = "Initial BudgetExcel:" + bdgtExcelCheck[0].InitialBudget.ToString();
                    MessageBoxCheckExcelBudget(bdgt, budgetPart, bdgtMainLine, bdgtExcelLine);
                }

                if (bdgtExcelCheck[0].OutBudget.ToString("N4") != ((double)bdgt.OUTAMOUNT).ToString("N4"))
                {
                    budgetPart = "Out is not equal!";
                    bdgtMainLine = "Out BudgetMain:" + bdgt.OUTAMOUNT.ToString();
                    bdgtExcelLine = "Out BudgetExcel:" + bdgtExcelCheck[0].OutBudget.ToString();
                    MessageBoxCheckExcelBudget(bdgt, budgetPart, bdgtMainLine, bdgtExcelLine);
                }

                if (bdgtExcelCheck[0].CurrentBudget.ToString("N4") != ((double)bdgt.CURRAMOUNT).ToString("N4"))
                {
                    budgetPart = "Current Amount is not equal!";
                    bdgtMainLine = "Current BudgetMain:" + bdgt.CURRAMOUNT.ToString();
                    bdgtExcelLine = "Current BudgetExcel:" + bdgtExcelCheck[0].CurrentBudget.ToString();
                    MessageBoxCheckExcelBudget(bdgt, budgetPart, bdgtMainLine, bdgtExcelLine);

                    continue;
                }
            }
        }

        private void MessageBoxCheckExcelBudget(Budget.Budget bdgt, string budgetPart, string bdgtMainLine, string bdgtExcelLine)
        {
            _ = MessageBox.Show(budgetPart + Environment.NewLine +
                bdgt.FMNO + Environment.NewLine +
                bdgt.FMNAME + Environment.NewLine +
                bdgt.CISICODE + Environment.NewLine +
                bdgtMainLine + Environment.NewLine +
                bdgtExcelLine);
        }

        private void BudgetEncumbranceSendTo(object sender, RoutedEventArgs e)
        {
            Budget.ApproveListMain approveListMain = new Budget.ApproveListMain();
            approveListMain.InitList();

            foreach(Budget.Approve approve in approveListMain)
            {
                Budget.EncumbranceSendTo encumbranceSendTo = new Budget.EncumbranceSendTo(approve);
                encumbranceSendTo.txtApproveChoice.SelectedIndex = 0;
                encumbranceSendTo.txtSendUser.SelectedIndex = 0;
                encumbranceSendTo.txtDate.Text = approve.APPDATE;
                encumbranceSendTo.btnSave.Focus();
                encumbranceSendTo.ShowDialog();
            }
        }

        private void FinconEncumranceSendTo(object sender, RoutedEventArgs e)
        {
            Fincon.FinconApproveMain finconApproveMain = new Fincon.FinconApproveMain();
            finconApproveMain.InitList();

            foreach(Fincon.FinconApprove finconApprove in finconApproveMain)
            {
                Fincon.FinEncumbranceSendTo finEncumbranceSendTo = new Fincon.FinEncumbranceSendTo(finconApprove);
                finEncumbranceSendTo.txtApprove.SelectedIndex = 0;
                finEncumbranceSendTo.txtSendTo.SelectedIndex = 0;
                finEncumbranceSendTo.txtDate.Text = finconApprove.APPDATE.ToString();
                finEncumbranceSendTo.BtnSave.Focus();
                finEncumbranceSendTo.ShowDialog();
            }
        }

        private void FiscalSendToPending(object sender, RoutedEventArgs e)
        {
            Fiscal.PendingMain pendingMain = new Fiscal.PendingMain();

            Reports.ReportFile reportFile = new Reports.ReportFile().TakeFileData("BACKUP ALLOCATIONS2022");
            if (reportFile is null)
            {
                _ = MessageBox.Show("Report File is null!");
                return;
            }

            Reports.ReportExcel reportExcel = new Reports.ReportExcel().OpenTemplateFile(reportFile, "COMMIT");
            if (reportExcel is null)
            {
                _ = MessageBox.Show("Report Excel is null!");
                return;
            }

            List<FundManagerEncumbrance> adminEncumbrances = new AdminEncumbrance().LoadExcelCommitData(reportExcel);

            Fiscal.FiscalApproveMain fiscalApproveMain = new Fiscal.FiscalApproveMain();
            fiscalApproveMain.InitList();

            var adminEncumbPendings = from cmt in adminEncumbrances
                                      where cmt.PendingNo != string.Empty
                                      select cmt;

            foreach(FundManagerEncumbrance fndEncumb in  adminEncumbPendings)
            {
                var fsclApproveCheck = (from apr in fiscalApproveMain
                                       where apr.REQDESC.Trim() == fndEncumb.ReqDesc.Trim() &&
                                       apr.FMNO == fndEncumb.FmNo &&
                                       apr.CISICODE.Trim() == fndEncumb.CisiCode.Trim() &&
                                       apr.REQAMOUNT.ToString("N4") == fndEncumb.ReqAmount.ToString("N4")
                                       select apr).ToList();

                if (fsclApproveCheck.Count > 0)
                {
                    Fiscal.Pending pending = new Fiscal.Pending()
                    {
                        APPROVEID = fsclApproveCheck[0].ID,
                        MAINID = fsclApproveCheck[0].MAININD,
                        ENCUMID = fsclApproveCheck[0].ENCUMID,
                        REQDESC = fsclApproveCheck[0].REQDESC,
                        FMNAME = fsclApproveCheck[0].FMNAME,
                        REQAMOUNT = fsclApproveCheck[0].REQAMOUNT,
                        REQCURR = fsclApproveCheck[0].REQCURR,
                        BDGTCURR = fsclApproveCheck[0].BDGTCURR,
                        BDGTAMOUNT = fsclApproveCheck[0].BDGTAMOUNT,
                        APPDATE = fsclApproveCheck[0].APPDATE,
                    };

                    if (pendingMain.InitPendingData(ref pending))
                    {
                        Fiscal.FiscalPending fiscalPending = new Fiscal.FiscalPending(pending);
                        fiscalPending.txtTransAmount.Text = pending.REQAMOUNT.ToString("N4");
                        fiscalPending.txtDate.Text = pending.APPDATE.ToString();
                        fiscalPending.txtActCode.SelectedIndex = fndEncumb.ActCode;
                        fiscalPending.BtnSave.Focus();

                        MessageBox.Show("Pending No:" + fndEncumb.PendingNo);

                        fiscalPending.ShowDialog();
                    }
                }
            }
        }

        private void LoadBiddings(object sender, RoutedEventArgs e)
        {
            Reports.ReportFile reportBackup = new Reports.ReportFile().TakeFileData("BACKUP ALLOCATIONS2022");
            if (reportBackup is null)
            {
                _ = MessageBox.Show("ReportBackup File is null!");
                return;
            }
            Reports.ReportExcel reportCommit = new Reports.ReportExcel().OpenTemplateFile(reportBackup, "COMMIT");
            if (reportCommit is null)
            {
                _ = MessageBox.Show("Report Commit is null!");
                return;
            }
            List<FundManagerEncumbrance> adminEncumbrances = new AdminEncumbrance().LoadExcelCommitData(reportCommit);

            if (adminEncumbrances is null)
            {
                _ = MessageBox.Show("adminEncumbrance is null!");
                return;
            }

            Reports.ReportFile reportProcurement = new Reports.ReportFile().TakeFileData("PROCUREMENT_SHEET");
            if (reportProcurement is null)
            {
                _ = MessageBox.Show("ReportProcurement File is null!");
                return;
            }            
            Reports.ReportExcel reportIhale = new Reports.ReportExcel().OpenTemplateFile(reportProcurement, "IHALE");
            if (reportIhale is null)
            {
                _ = MessageBox.Show("Report Ihale is null!");
                return;
            }
            List<AdminProcurementSheet> adminProcurementSheets = new AdminProcurementSheet().LoadProcurementSheet(reportIhale);

            if (adminProcurementSheets is null)
            {
                _ = MessageBox.Show("adminProcurementSheets is null!");
                return;
            }

            Purchasing.ApproveMain approveMain = new Purchasing.ApproveMain();
            approveMain.InitList();
            Fiscal.PendingMain pendingMain = new Fiscal.PendingMain();

            foreach (Purchasing.Approve approve in approveMain)
            {
                var encumbCheck = adminEncumbrances.Where(fmEncumb => fmEncumb.PendingNo == approve.PendingNo);
                var procureCheck = adminProcurementSheets.Where(procure => procure.PendingNo == approve.PendingNo);

                List<FundManagerEncumbrance> encumbList = null;
                List<AdminProcurementSheet> procureList = null;

                foreach(var encumb in encumbCheck)
                {
                    if (encumbList is null)
                    {
                        encumbList = new List<FundManagerEncumbrance>();
                    }

                    encumbList.Add(encumb);
                }

                foreach(var procure in procureCheck)
                {
                    if (procureList is null)
                    {
                        procureList = new List<AdminProcurementSheet>();
                    }

                    procureList.Add(procure);
                }


                if (encumbList is null)
                {
                    _ = MessageBox.Show("EncumbList is null!");
                    continue;
                }

                if (procureList is null)
                {
                    _ = MessageBox.Show("ProcureList is null!");
                    continue;
                }

                Purchasing.PurchasingBidding purchasingBidding = new Purchasing.PurchasingBidding(approve);
                purchasingBidding.TxtPointOfContact.Text = encumbList[0].PofContactRank + " " + encumbList[0].PofContact;
                purchasingBidding.TxtBiddingOpenDate.Text = procureList[0].BiddingOpenDate.ToString();
                purchasingBidding.TxtBiddingCloseDate.Text = procureList[0].BiddingCloseDate.ToString();
                purchasingBidding.TxtLines.Text = "1";
                purchasingBidding.TxtPofConPhone.Text = procureList[0].PendingNo;
                purchasingBidding.BtnSaveBidding.Focus();
                purchasingBidding.ShowDialog();

                if (MessageBox.Show("Do you want to continue", "Confirmation!", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    break;
                }
            }
        }

        private void LoadBiddingsItems(object sender, RoutedEventArgs e)
        {
            List<AdminPurchaseItem> adminPurchaseItems = new AdminPurchaseItem().LoadAdminPurhaseItems();
            if (adminPurchaseItems is null)
            {
                _ = MessageBox.Show("adminPurchaseItems is null");
                return;
            }

            Purchasing.BiddingMain biddings = new Purchasing.BiddingMain();
            biddings.InitList();

            foreach (Purchasing.Bidding bid in biddings)
            {
                Purchasing.PurchasingItems purchasingItems = new Purchasing.PurchasingItems(bid);

                var adminPurchaseItemCheck = adminPurchaseItems.Where(itm => itm.PendingNu == bid.PendingNo);
                int itmNu = 1;
                foreach(var admItm in adminPurchaseItemCheck)
                {
                    Purchasing.Item item = new Purchasing.Item()
                    {
                        BiddingId = bid.ID,
                        Brand = string.Empty,
                        Description = admItm.ItemDesc,
                        ItemNu = itmNu.ToString(),
                        Quantity = (float)admItm.ItemQuantity,
                        Unit = admItm.ItemUnit,
                        UnitPrice = (decimal)admItm.ItemUnitPrice,
                        TotalAmount = (decimal)(admItm.ItemUnitPrice * admItm.ItemQuantity),
                    };
                    itmNu++;
                    purchasingItems.itemsMain.Add(item);
                }

                if (purchasingItems.itemsMain.Count == 0)
                {
                    continue;
                }

                purchasingItems.LstMain.ItemsSource = purchasingItems.itemsMain;
                purchasingItems.BtnSaveList.Focus();
                purchasingItems.ShowDialog();

                if (MessageBox.Show("Do you want to continue?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    break;
                }
            }
        }

        private void LoadContracts(object sender, RoutedEventArgs e)
        {
            List<AdminPurchaseContract> adminPurchaseContracts = new AdminPurchaseContract().LoadAdminPurchaseContract();
            if (adminPurchaseContracts is null)
            {
                _ = MessageBox.Show("AdminPurchaseContract is null");
                return;
            }

            Purchasing.BiddingMain biddingMain = new Purchasing.BiddingMain();
            biddingMain.InitList();

            foreach(AdminPurchaseContract admPurCont in adminPurchaseContracts)
            {
                Purchasing.Bidding bidding = null;
                var biddingCheck = biddingMain.Where(bid => bid.PendingNo.Trim() == admPurCont.PendingNo.Trim());

                foreach(Purchasing.Bidding bid in biddingCheck)
                {
                    bidding = bid;
                    break;
                }

                if (bidding is null)
                {
                    _ = MessageBox.Show("Bidding is not found!" + admPurCont.PendingNo);
                    continue;
                }

                bidding.LastContractNu = adminPurchaseContracts.IndexOf(admPurCont);

                Purchasing.PurchasingContract purchasingContract = new Purchasing.PurchasingContract(bidding);
                purchasingContract.TxtContractor.Text = admPurCont.Contractor;
                purchasingContract.TxtContractNo.Text = admPurCont.ContractNo;
                //This control for fist data upload due to commit amount is different beacause of Bank Charges?
                purchasingContract.TxtPcAmount.Text = bidding.BiddingPrice > admPurCont.PcAmount ? bidding.BiddingPrice.ToString() : admPurCont.PcAmount.ToString();
                purchasingContract.TxtPcDate.Text = admPurCont.PurchasingDate.ToString();
                purchasingContract.ButtonSave.Focus();
                purchasingContract.ShowDialog();

                if (MessageBox.Show("Do you want to continue?", "Confirmation!", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    break;
                }
            }
        }

        private void LoadNotifications(object sender, RoutedEventArgs e)
        {
            List<AdminContractNotification> adminContractNotifications = new AdminContractNotification().LoadAdminContractNotifications();
            if (adminContractNotifications is null)
            {
                _ = MessageBox.Show("AdminContractNotifications is null!");
                return;
            }

            Purchasing.ContractMain contractMain = new Purchasing.ContractMain();
            contractMain.InitContracts();

            foreach(Purchasing.Contract cnt in contractMain)
            {
                var admContNotCheck = adminContractNotifications.Where(adm => adm.ContractNo.Substring(5, 4) == cnt.ContractNo.Substring(8, 4));
                Purchasing.PurchasingNotfications purchasingNotfications = new Purchasing.PurchasingNotfications(cnt);

                int itmNo = 1;
                foreach (AdminContractNotification admContNotif in admContNotCheck)
                {
                    Purchasing.Notification notification = new Purchasing.Notification()
                    {
                        ID = itmNo.ToString(),
                        BidAmount = admContNotif.BidAmount,
                        CompanyId = admContNotif.CompanyId,
                        BidDate = cnt.PcDate,
                        BiddingId = cnt.BiddingId,
                    };
                    itmNo++;
                    purchasingNotfications.notificationMain.Add(notification);
                }
                purchasingNotfications.LstMain.ItemsSource = purchasingNotfications.notificationMain;
                purchasingNotfications.BtnSave.Focus();
                purchasingNotfications.ShowDialog();

                if(MessageBox.Show("Do you want to continue?","Confirmation!", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }
            }
        }

        private void SendContractstoFincon(object sender, RoutedEventArgs e)
        {
            Purchasing.ContractMain contractmain = new Purchasing.ContractMain();
            contractmain.InitContractsNotSendTo();

            foreach(Purchasing.Contract contract in contractmain)
            {
                if (MessageBox.Show("Do you want to send to this file to Fincon?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }

                if (!contractmain.SendToFincon(contract))
                {
                    _ = MessageBox.Show("Data couldn't save!");
                    return;
                }
            }
        }

        private void FinconSendContractstoFiscal(object sender, RoutedEventArgs e)
        {
            Fincon.FinconApproveMain finconApproveMain = new Fincon.FinconApproveMain();
            finconApproveMain.InitList();

            foreach(Fincon.FinconApprove finconApprove in finconApproveMain)
            {
                Fincon.FinEncumbranceSendTo finEncumbranceSendTo = new Fincon.FinEncumbranceSendTo(finconApprove);
                finEncumbranceSendTo.txtDate.Text = finconApprove.APPDATE.ToString();
                finEncumbranceSendTo.txtApprove.SelectedIndex = 0;
                finEncumbranceSendTo.txtSendTo.SelectedIndex = 0;
                finEncumbranceSendTo.BtnSave.Focus();
                finEncumbranceSendTo.ShowDialog();
            }
        }

        private void FiscalTakeCommitNumbers(object sender, RoutedEventArgs e)
        {
            Reports.ReportFile reportFile = new Reports.ReportFile().TakeFileData("BACKUP ALLOCATIONS2022");
            if (reportFile is null)
            {
                _ = MessageBox.Show("Report File is null!");
                return;
            }

            Reports.ReportExcel reportExcel = new Reports.ReportExcel().OpenTemplateFile(reportFile, "COMMIT");
            if (reportExcel is null)
            {
                _ = MessageBox.Show("Report Excel is null!");
                return;
            }

            List<FundManagerEncumbrance> adminEncumbrances = new AdminEncumbrance().LoadExcelCommitData(reportExcel);

            var commitOrdetAdminEncumbrances = from fundManagerEncumbrance in adminEncumbrances
                                               where fundManagerEncumbrance.CommitNu.Length > 4
                                               orderby fundManagerEncumbrance.CommitNu.Substring(9, 4)
                                               select fundManagerEncumbrance;

            var activityCodes = new Fiscal.ActivityCodeMain();


            foreach (FundManagerEncumbrance fundManagerEncumbrance in commitOrdetAdminEncumbrances)
            {
                Fiscal.FiscalApproveMain fiscalApproveMain = new Fiscal.FiscalApproveMain();
                fiscalApproveMain.InitList();

                Fiscal.FiscalApprove fiscalApprove = (from approve in fiscalApproveMain
                                                      where approve.REQDESC.Trim() == fundManagerEncumbrance.ReqDesc.Trim()
                                                      select approve).FirstOrDefault();

                if (!(fiscalApprove is default(Fiscal.FiscalApprove)))
                {
                    _ = MessageBox.Show("ReqDesc:" + fiscalApprove.REQDESC + Environment.NewLine +
                        "CommitNu:" + fundManagerEncumbrance.CommitNu + Environment.NewLine + 
                        "CommitAmount:" + fundManagerEncumbrance.ReqAmount.ToString("N4"));

                    Fiscal.FiscalCommit fiscalCommit = new Fiscal.FiscalCommit(fiscalApprove);
                    Fiscal.ActivityCode actCode = activityCodes.Where(act=> act.ACTCODE == fundManagerEncumbrance.CommitNu.Substring(3, 2))
                        .Select (act => act).FirstOrDefault();

                    if (actCode is default(Fiscal.ActivityCode))
                    {
                        _ = MessageBox.Show("Activity Code:" + fundManagerEncumbrance.CommitNu.Substring(3, 2) + " is not found!" + Environment.NewLine +
                            "Therefore first activity code is selected!");
                        actCode = activityCodes.First();
                    }

                    if (fundManagerEncumbrance.ReqAmount.ToString("N4") != fiscalApprove.REQAMOUNT.ToString("N4"))
                    {
                        _ = MessageBox.Show("ReqAmounts are not equal!" + Environment.NewLine +
                            "BudgetAllocate Req Amount is " + fundManagerEncumbrance.ReqAmount.ToString() + Environment.NewLine +
                            "FiscalApprove Req Amount is " + fiscalApprove.REQAMOUNT.ToString());
                    }

                    fiscalCommit.txtActCode.SelectedIndex = activityCodes.IndexOf(actCode);

                    fiscalCommit.txtDate.Text = fiscalApprove.APPDATE.ToString("d");
                    fiscalCommit.txtTransAmount.Text = fiscalApprove.REQAMOUNT.ToString();
                    fiscalCommit.BtnSave.Focus();
                    fiscalCommit.ShowDialog();
                }

                if (MessageBox.Show("Do you want to continue?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    break;
                }
            }
        }

        private void TakeInvoices(object sender, RoutedEventArgs e)
        {
            List<Invoice> invoices = new AdminInvoice().LoadInvoices();

            if( invoices is null)
            {
                _ = MessageBox.Show("AdminInvoice list is null!");
                return;
            }

            foreach(Invoice inv in invoices)
            {
                Fiscal.CommitApproveNonDisbursign commitApproves = new Fiscal.CommitApproveNonDisbursign();
                Fiscal.CommitApprove commitApprove = commitApproves.FirstOrDefault(comapprove => comapprove.CommitNu.Substring(9, 4) == inv.CommitNo.Substring(6, 4));

                if (commitApprove is default(Fiscal.CommitApprove))
                {
                    _ = MessageBox.Show(inv.CommitNo + " is not find in CommitApproveNonDisbursing!");
                    continue;
                }

                Fiscal.Invoice fiscalInvoice = new Fiscal.Invoice()
                {
                    COMMITID = commitApprove.CommitId,
                    COMMITAMOUNT = commitApprove.CommitAmount,
                    INVOICEAMOUNT = inv.InvoiceAmount,
                    INVOICENU = inv.InvoiceReferance,
                    CREDITOR = inv.FiscalVendorName,
                    INVOICEDATE = inv.InvoiceDate,
                    TOTINVAMOUNT = commitApprove.TotInvAmount,
                };

                _ = MessageBox.Show("Commit Number:" + "\t" + commitApprove.CommitNu);

                Fiscal.FiscalSendToTakeInvoice fiscalSendToTakeInvoice = new Fiscal.FiscalSendToTakeInvoice(fiscalInvoice);
                fiscalSendToTakeInvoice.txtInvAmount.Text = fiscalInvoice.INVOICEAMOUNT.ToString();
                fiscalSendToTakeInvoice.txtInvoiceNu.Text = fiscalInvoice.INVOICENU;
                fiscalSendToTakeInvoice.txtDate.Text = fiscalInvoice.INVOICEDATE.ToString();
                fiscalSendToTakeInvoice.txtCreditor.Text = fiscalInvoice.CREDITOR;
                fiscalSendToTakeInvoice.btnSave.Focus();
                fiscalSendToTakeInvoice.ShowDialog();

                if (MessageBox.Show("Do you want to continue?", "Confirmation!", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    break;
                }
            }
        }
    }
}
