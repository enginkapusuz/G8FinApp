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
using System.Globalization;
using System.Data.OleDb;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;

namespace G8FinApp.Budget
{
    /// <summary>
    /// Interaction logic for BudgetMainWin.xaml
    /// </summary>
    /// 

    public partial class BudgetMainWin : Window
    {
        Budget bdgt;
        BudgetMain bdgtMain;
        BudgetDetailMain bdgtDtlMain = new BudgetDetailMain();
        BudgetData budgetData;

        private const string curFor = "#,#.00";
        public BudgetMainWin()
        {
            BudgetMain budgetMain = new BudgetMain();
            InitializeComponent();
            budgetMain.InitList();
            LstMain.ItemsSource = budgetMain;
        }

        private void LstMainSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstMain.SelectedIndex == -1)
            {
                return;
            }


            ClearTextBlocks();

            bdgtDtlMain.Clear();
            bdgtDtlMain.InitList();

            bdgt = LstMain.SelectedItem as Budget;
            LstDetailIn.Items.Clear();
            LstDetailOut.Items.Clear();

            foreach (var bdgtDtl in bdgtDtlMain)
            {
                if (bdgtDtl.BUDGETMAINID == bdgt.ID && bdgtDtl.AMOUNT > 0)
                {
                    if (bdgtDtl.TABLENAME == "BudgetIn" || bdgtDtl.TABLENAME == "BudgetTransferIn")
                    {
                        _ = LstDetailIn.Items.Add(bdgtDtl);
                    }
                    else
                    {
                        _ = LstDetailOut.Items.Add(bdgtDtl);
                    }
                }
            }
        }

        private void OpenNewBudget(object sender, RoutedEventArgs e)
        {
            BudgetNew budgetNew = new BudgetNew();
            _ = budgetNew.ShowDialog();

            bdgtMain = new BudgetMain();
            bdgtMain.Clear();
            bdgtMain.InitList();
            LstMain.ItemsSource = bdgtMain;
        }

        private void OpenEnterBudgetAmount(object sender, RoutedEventArgs e)
        {
            if (LstMain.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select a record from Main List!");
                return;
            }
            else
            {
                bdgt = LstMain.SelectedItem as Budget;

                foreach (BudgetDetail bdgtDtl in LstDetailIn.Items)
                {

                    if (bdgtDtl.TABLENAME == "BudgetIn")
                    {
                        _ = MessageBox.Show("There is Budget Enter for this record!");
                        return;
                    }
                }

                BudgetEnterAmount budgetEnterAmount = new BudgetEnterAmount(bdgt);
                _ = budgetEnterAmount.ShowDialog();

                bdgtMain = new BudgetMain();
                bdgtMain.Clear();
                bdgtMain.InitList();
                LstMain.ItemsSource = bdgtMain;
                if (LstMain.Items.Count > 0)
                {
                    LstMain.SelectedIndex = 0;
                }

                bdgtDtlMain.Clear();

            }
        }

        private void OpenEnterBudgetTransfer(object sender, RoutedEventArgs e)
        {
            Budget slctdBudget;
            BudgetTransfer budgetTransfer;

            if (LstMain.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select a record!");
                return;
            }
            else if (LstMain.Items.Count < 2)
            {
                _ = MessageBox.Show("At least two budget items are needed for the transfer!");
                return;
            }
            else
            {
                slctdBudget = LstMain.SelectedItem as Budget;
                budgetTransfer = new BudgetTransfer(slctdBudget);

                foreach (Budget bdgt in LstMain.Items)
                {
                    if (bdgt.ID != slctdBudget.ID)
                    {
                        _ = budgetTransfer.LstBdgtDetail.Items.Add(bdgt);
                    }
                }

                decimal inAmount = 0;
                decimal outAmount = 0;
                decimal availAmount;

                foreach (BudgetDetail bdgtDtl in LstDetailIn.Items)
                {
                    inAmount += bdgtDtl.AMOUNT;
                }

                foreach (BudgetDetail bdgtDtl in LstDetailOut.Items)
                {
                    outAmount += bdgtDtl.AMOUNT;
                }

                availAmount = inAmount - outAmount;

                if (availAmount <= 0)
                {
                    _ = MessageBox.Show("Budget is not enough!");
                    return;
                }

                budgetTransfer.txtAmount.Text = availAmount.ToString("N2");
                _ = budgetTransfer.ShowDialog();

                bdgtMain = new BudgetMain();
                bdgtMain.Clear();
                bdgtMain.InitList();
                LstMain.ItemsSource = bdgtMain;
                if (LstMain.Items.Count > 0)
                {
                    LstMain.SelectedIndex = 0;
                }

                bdgtDtlMain.Clear();
            }
        }

        private void OpenEnterNewEncumbrance(object sender, RoutedEventArgs e)
        {
            Budget slctdBudget;
            BudgetEncumbrance budgetEncumbrance;

            if (LstMain.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select a record!");
                return;
            }

            slctdBudget = LstMain.SelectedItem as Budget;
            budgetEncumbrance = new BudgetEncumbrance(slctdBudget);

            decimal inAmount = 0;
            decimal outAmount = 0;
            decimal availAmount;

            foreach (BudgetDetail bdgtDtl in LstDetailIn.Items)
            {
                inAmount += bdgtDtl.AMOUNT;
            }

            foreach (BudgetDetail bdgtDtl in LstDetailOut.Items)
            {
                outAmount += bdgtDtl.AMOUNT;
            }

            availAmount = inAmount - outAmount;

            if (availAmount < 0)
            {
                _ = MessageBox.Show("Budget is not enough!");
                return;
            }

            budgetEncumbrance.txtAmount.Text = availAmount.ToString(curFor);
            budgetEncumbrance.ShowDialog();

            bdgtMain = new BudgetMain();
            bdgtMain.Clear();
            bdgtMain.InitList();
            LstMain.ItemsSource = bdgtMain;

            //if (LstMain.Items.Count > 0)
            //{
            //    LstMain.SelectedIndex = 0;
            //}

            bdgtDtlMain.Clear();
        }
        private void ClearTextBlocks()
        {
            txtBlckReqDesc.Text = "";
            txtBlckReqNum.Text = "";
            txtBlckReqItmCount.Text = "";
            txtBlckReqCurr.Text = "";
            txtBlckReqAmount.Text = "";
        }
        private void LstDetailOutSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BudgetDetail budgetDetail;
            BudgetDataMain budgetDataMain = new BudgetDataMain();

            if (LstDetailOut.Items.Count == 0)
            {
                return;
            }

            budgetDetail = LstDetailOut.SelectedItem as BudgetDetail;

            if (budgetDetail.TABLENAME != "BudgetEncumbrance")
            {
                ClearTextBlocks();
                return;
            }
            
            budgetDataMain.InitList();
            try
            {
                budgetData = budgetDataMain.Where(bd => budgetDetail.BDGTENCMBDATAID == bd.ID).First();
                txtBlckReqDesc.Text = budgetData.REQDESC;
                txtBlckReqNum.Text = budgetData.REQNUM;
                txtBlckReqNum.Text = budgetData.REQNUM;
                txtBlckReqItmCount.Text = budgetData.REQITEMCOUNT;
                txtBlckReqCurr.Text = budgetData.REQCURR;
                txtBlckReqAmount.Text = budgetData.REQAMOUNT.ToString("#,#.0000");
            }
            catch
            { }
        }

        private void OpenSendToEncumbrance(object sender, RoutedEventArgs e)
        {
            BudgetApproveList budgetApproveList = new BudgetApproveList();
            budgetApproveList.ShowDialog();
        }

        private void OpenEnterBudgetRevise(object sender, RoutedEventArgs e)
        {
            Budget budget;
            BudgetMain budgetMain;
            BudgetRevise budgetRevise;

            if (LstMain.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select an item from Main List!");
                return;
            }

            budget = LstMain.SelectedItem as Budget;

            if (budget.CURRAMOUNT == 0)
            {
                _ = MessageBox.Show("Current Amount is not enough!");
                return;
            }

            budgetRevise = new BudgetRevise(budget);
            budgetRevise.ShowDialog();

            bdgtDtlMain = new BudgetDetailMain();
            budgetMain = new BudgetMain();
            budgetMain.InitList();
            LstMain.ItemsSource = budgetMain;
            LstDetailIn.Items.Clear();
            LstDetailOut.Items.Clear();
        }

        private void OpenBudgetNotApprove(object sender, RoutedEventArgs e)
        {
            BudgetNotApprove budgetNotApprove = new BudgetNotApprove();
            budgetNotApprove.ShowDialog();
        }

        private void OpenMainListtoExcel(object sender, RoutedEventArgs e)
        {
            Excel.Application oXL;
            Excel._Workbook oWB;
            Excel._Worksheet oSheet;
            Excel.Range oRange;

            try
            {
                oXL = new Excel.Application();
                oXL.Visible = true;


                oWB = (Excel._Workbook)oXL.Workbooks.Add();
                oSheet = (Excel._Worksheet)oWB.ActiveSheet;

                oSheet.Cells[1, 1] = "ID";
                oSheet.Cells[1, 2] = "Fund Mang.No";
                oSheet.Cells[1, 3] = "Fund Mang Name";
                oSheet.Cells[1, 4] = "CISI Code";
                oSheet.Cells[1, 5] = "Currency";
                oSheet.Cells[1, 6] = "Description";
                oSheet.Cells[1, 7] = "In Amount";
                oSheet.Cells[1, 8] = "Out Amount";
                oSheet.Cells[1, 9] = "Current Amount";

                var lstLstMain = LstMain.ItemsSource as BudgetMain;
                int rw = 2;

                foreach(var bdgt in lstLstMain)
                {
                    oSheet.Cells[rw, 1] = bdgt.ID;
                    oSheet.Cells[rw, 2] = bdgt.FMNO;
                    oSheet.Cells[rw, 3] = bdgt.FMNAME;
                    oSheet.Cells[rw, 4] = bdgt.CISICODE;
                    oSheet.Cells[rw, 5] = bdgt.BDGTCURR;
                    oSheet.Cells[rw, 6] = bdgt.CISIDESC;
                    oSheet.Cells[rw, 7] = bdgt.INAMOUNT;
                    oSheet.Cells[rw, 8] = bdgt.OUTAMOUNT;
                    oSheet.Cells[rw, 9] = bdgt.CURRAMOUNT;
                    rw++;
                }

                oRange = oSheet.Range["A1:I1"];
                oRange.ColumnWidth = 14;
                oRange.Interior.ColorIndex = 15;

                oSheet.Range["G:G"].NumberFormat = "#,#.00";
                oSheet.Range["H:H"].NumberFormat = "#,#.00";
                oSheet.Range["I:I"].NumberFormat = "#,#.00";
            }
            catch(Exception ex)
            {
                _ = MessageBox.Show(ex.Message);
            }
        }

        private void LstDetailOut_Delete(object sender, RoutedEventArgs e)
        {
            BudgetDetail budgetDetail;

            if (LstDetailOut.SelectedIndex == -1)
            {
                return;
            }

            if (MessageBox.Show("Do you want to delete this record?","Deletion Confirmation!", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            budgetDetail = LstDetailOut.SelectedItem as BudgetDetail;

            if (budgetDetail.TABLENAME == "BudgetEncumbrance")
            {
                DeleteBudgetEncumbrance();
            }

            if(budgetDetail.TABLENAME == "BudgetRevise")
            {
                DeleteBudgetRevise();
            }

            if(budgetDetail.TABLENAME == "BudgetTransferOut")
            {
                DeleteBudgetTransferOut();
            }
        }

        private void DeleteBudgetEncumbrance()
        {
            Budget budget;
            BudgetDetailMain budgetDetailMain;
            BudgetDataMain budgetDataMain;
            BudgetDetail budgetDetail;
            BudgetData budgetData = null;

            budgetDetail = LstDetailOut.SelectedItem as BudgetDetail;
            budgetDetailMain = new BudgetDetailMain();
            budgetDataMain = new BudgetDataMain();
            budget = LstMain.SelectedItem as Budget;

            budgetDataMain.InitList();
            var budgetDataQuery = budgetDataMain.Where(bd => budgetDetail.ID == bd.ID);

            foreach (var bd in budgetDataQuery)
            {
                budgetData = bd;
            }

            if (budgetData is null)
            {
                _ = MessageBox.Show("BudgetData couldn't found!");
                return;
            }

            if (!budgetDetailMain.DeleteBudgetEncumbrance(budgetDetail))
            {
                _ = MessageBox.Show("Deletion process has been failed in BudgetDetail!");
                return;
            }

            if (!budgetDataMain.DeleteBudgetData(budgetData))
            {
                _ = MessageBox.Show("Deletion process has been failed in BudgetData!");
                return;
            }

            ReloadLstDetail();
            ReloadLstMain();
        }

        private void DeleteBudgetRevise()
        {
            BudgetDetail budgetDetail;
            BudgetDetailMain budgetDetailMain = new BudgetDetailMain();

            budgetDetail = LstDetailOut.SelectedItem as BudgetDetail;
            if (!budgetDetailMain.DeleteBudgetRevise(budgetDetail))
            {
                _ = MessageBox.Show("Data couldn't delete!");
                return;
            }

            _ = MessageBox.Show("Revise amount has been deleted");
            ReloadLstDetail();
            ReloadLstMain();
        }

        private void DeleteBudgetTransferOut()
        {
            BudgetDetailMain budgetDetailMain;

            BudgetDetail budgetDetailOut = FindSelectedBudgetDetailOut();
            if (budgetDetailOut is null)
            {
                _ = MessageBox.Show("Please select a record!");
                return;
            }

            BudgetDetail budgetDetailIn = FindSelectedBudgetDetailIn(budgetDetailOut);
            if (budgetDetailIn is null)
            {
                _ = MessageBox.Show("BudgetDetailIn is empty!");
                return;
            }

            Budget budgetTransferIn = FindSelectedBudget(budgetDetailIn);
            if (budgetTransferIn is null)
            {
                _ = MessageBox.Show("Budget Main is empty!");
                return;
            }

            if (budgetTransferIn.CURRAMOUNT - budgetDetailIn.AMOUNT < 0)
            {
                _ = MessageBox.Show("Current amount is not enough for this!");
                return;
            }

            budgetDetailMain = new BudgetDetailMain();

            if (!budgetDetailMain.DeleteTransfer(budgetDetailOut))
            {
                _ = MessageBox.Show("Data couldn't remove!");
                return;
            }

            MessageBox.Show("Deletion is successfull");

            ReloadLstDetail();
            ReloadLstMain();
        }

        private BudgetDetail FindSelectedBudgetDetailOut()
        {
            if (LstDetailOut.SelectedIndex == -1)
            {
                return null;
            }

            return LstDetailOut.SelectedItem as BudgetDetail;
        }
        private BudgetDetail FindSelectedBudgetDetailIn(BudgetDetail budgetDetailOut)
        {
            BudgetDetailMain budgetDetailMain = new BudgetDetailMain();
            budgetDetailMain.InitList();

            BudgetDetail budgetDetailIn = (from bdgDtl in budgetDetailMain
                                 where bdgDtl.TABLENAME == "BudgetTransferIn" && bdgDtl.ID == budgetDetailOut.ID
                                 select bdgDtl).First();

            return budgetDetailIn;
        }

        private Budget FindSelectedBudget(BudgetDetail budgetDetailIn)
        {
            var budget = (from bdgt in LstMain.ItemsSource as BudgetMain
                          where bdgt.ID == budgetDetailIn.BUDGETMAINID
                          select bdgt).First();
            return budget;
        }

        private void ReloadLstMain()
        {
            BudgetMain budgetMain;
            budgetMain = new BudgetMain();

            budgetMain.Clear();
            budgetMain.InitList();
            LstMain.ItemsSource = budgetMain;
        }

        private void ReloadLstDetail()
        {
            Budget budget;
            BudgetDetailMain budgetDetailMain = new BudgetDetailMain();


            LstDetailIn.Items.Clear();
            LstDetailOut.Items.Clear();

            if (LstMain.SelectedIndex == -1)
            {
                return;
            }

            budget = LstMain.SelectedItem as Budget;

            budgetDetailMain.Clear();
            budgetDetailMain.InitList();
            var budgetDetailQuery = budgetDetailMain.Where(bd => bd.BUDGETMAINID == budget.ID);

            LstDetailIn.Items.Clear();
            LstDetailOut.Items.Clear();

            foreach (var bd in budgetDetailQuery)
            {
                if (bd.TABLENAME != "BudgetTransferIn" && bd.TABLENAME != "BudgetIn")
                {
                    LstDetailOut.Items.Add(bd);
                }
                else
                {
                    LstDetailIn.Items.Add(bd);
                }
            }
        }

        private void LstDetailIn_Delete(object sender, RoutedEventArgs e)
        {
            BudgetDetailMain budgetDetailMain;

            BudgetDetail budgetDetailIn = TakeBudgetDetailIn();
            if (budgetDetailIn is null)
            {
                _ = MessageBox.Show("Budget Detail is empty!");
                return;
            }

            Budget budget = FindSelectedBudget(budgetDetailIn);
            if (budget is null)
            {
                _ = MessageBox.Show("Budget is empty!");
                return;
            }

            if (budget.CURRAMOUNT - budgetDetailIn.AMOUNT < 0)
            {
                _ = MessageBox.Show("Current Budget is not enough!");
                return;
            }

            budgetDetailMain = new BudgetDetailMain();
            if (!budgetDetailMain.DeleteBudgetIn(budgetDetailIn))
            {
                _ = MessageBox.Show("Deletion is not successful!");
                return;
            }

            _ = MessageBox.Show("Deletion is successfull");

            ReloadLstDetail();
            ReloadLstMain();
        }

        private BudgetDetail TakeBudgetDetailIn()
        {
            BudgetDetail budgetDetail;

            if (LstDetailIn.SelectedIndex == -1)
            {
                return null;
            }

            budgetDetail = LstDetailIn.SelectedItem as BudgetDetail;

            if (budgetDetail.TABLENAME != "BudgetIn")
            {
                return null;
            }

            return budgetDetail;
        }

        private void LstMain_Delete(object sender, RoutedEventArgs e)
        {
            Budget budget;
            BudgetMain budgetMain;

            if (LstMain.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select an item!");
                return;
            }

            budget = LstMain.SelectedItem as Budget;

            if (budget is null)
            {
                return;
            }

            if (budget.OUTAMOUNT > 0)
            {
                _ = MessageBox.Show("Outamount is greater than zero!" + Environment.NewLine + "Therefore you can't delete this budget!");
                return;
            }

            if (MessageBox.Show("Do you want to delete this record!", "Confirmation",MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            budgetMain = new BudgetMain();
            if (!budgetMain.DeleteBudget(budget))
            {
                _ = MessageBox.Show("Deletion is not successful!");
                return;
            }

            MessageBox.Show("Deletion is successful");

            ReloadLstDetail();
            ReloadLstMain();
        }

        private void UpdateCisiCode(object sender, RoutedEventArgs e)
        {
            if (LstMain.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select an item for update!");
                return;
            }

            Budget budget = LstMain.SelectedItem as Budget;
            if (budget is null)
            {
                _ = MessageBox.Show("Budget couldn't taken");
                return;
            }

            if (MessageBox.Show("Do you want to update the CisiCode:" + budget.CISICODE, "Update", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            CISICodeMain cisiCodeMain = new CISICodeMain();

            InputBoxForm inputBoxForm = new InputBoxForm(cisiCodeMain, budget);
            inputBoxForm.ShowDialog();

            if(inputBoxForm.newBudget is null)
            {
                _ = MessageBox.Show("The new CisiCode couldn't take!");
                return;
            }

            CisiCodeUpdateMain cisiCodeUpdateMain = new CisiCodeUpdateMain();
            if (!cisiCodeUpdateMain.UpdateCisiCode(inputBoxForm.newBudget))
            {
                _ = MessageBox.Show("Update process failed!");
                return;
            }

            _ = MessageBox.Show("Update process is successful");
            ReloadLstMain();
            ReloadLstDetail();
            return;
        }
    }
}
