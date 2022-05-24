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

namespace G8FinApp.Fiscal
{
    /// <summary>
    /// Interaction logic for FiscalBudget.xaml
    /// </summary>
    public partial class FiscalBudget : Window
    {
        public FiscalBudget()
        {
            InitializeComponent();
            LoadLstMain();
        }

        private void LoadLstMain()
        {
            Budget.BudgetMain budgetMain = new Budget.BudgetMain();
            budgetMain.InitList();
            LstMain.ItemsSource = budgetMain;
        }

        private void LstMainSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Budget.Budget budget = new Budget.Budget();
            Budget.BudgetDetailMain budgetDetailMain = new Budget.BudgetDetailMain();

            if (LstMain.SelectedIndex == -1)
            {
                return;
            }

            budgetDetailMain.Clear();
            budgetDetailMain.InitList();

            budget = LstMain.SelectedItem as Budget.Budget;
            LstDetailIn.Items.Clear();
            LstDetailOut.Items.Clear();

            foreach (var bdgtDtl in budgetDetailMain)
            {
                if (bdgtDtl.BUDGETMAINID == budget.ID && bdgtDtl.AMOUNT > 0)
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

        private void LstDetailOutSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Budget.BudgetData budgetData;
            Budget.BudgetDetail budgetDetail;
            Budget.BudgetDataMain budgetDataMain = new Budget.BudgetDataMain();

            if (LstDetailOut.Items.Count == 0)
            {
                return;
            }

            budgetDetail = LstDetailOut.SelectedItem as Budget.BudgetDetail;

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

        private void ClearTextBlocks()
        {
            txtBlckReqDesc.Text = "";
            txtBlckReqNum.Text = "";
            txtBlckReqItmCount.Text = "";
            txtBlckReqCurr.Text = "";
            txtBlckReqAmount.Text = "";
        }
    }
}
