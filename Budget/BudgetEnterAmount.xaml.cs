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
    /// Interaction logic for BudgetEnterAmount.xaml
    /// </summary>
    public partial class BudgetEnterAmount : Window
    {
        private const string curFormat = "#,#.00";
        Budget BdgtSelected;
        public BudgetEnterAmount(Budget bdgtSelected)
        {
            InitializeComponent();
            BdgtSelected = bdgtSelected;
            InitFields();
        }

        public BudgetEnterAmount(Admin.AdminFundManager adminFundManager, Admin.FundManagerBudget fundManagerBudget, string bdgtCurr)
        {
            InitializeComponent();
            txtId.Text = adminFundManager.fundManager.FundManagerID.ToString();
            txtFmNo.Text = adminFundManager.fundManager.FundManagerNo.ToString();
            txtFmName.Text = adminFundManager.fundManager.FundManagerName;
            txtCisiCode.Text = fundManagerBudget.CisiCode;
            txtbdgtCurr.Text = bdgtCurr;
            txtAmount.Text = fundManagerBudget.InitialBudget.ToString();
        }

        private void InitFields()
        {
            txtId.Text = BdgtSelected.ID.Trim();
            txtFmNo.Text = BdgtSelected.FMNO.ToString();
            txtFmName.Text = BdgtSelected.FMNAME.Trim();
            txtCisiCode.Text = BdgtSelected.CISICODE.Trim();
            txtbdgtCurr.Text = BdgtSelected.BDGTCURR.Trim();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtAmount.Text))
            {
                _ = MessageBox.Show("Amount is empty!");
                _ = txtAmount.Focus();
                return;
            }
            else if(!decimal.TryParse(txtAmount.Text, out decimal dcmlAmount))
            {
                _ = MessageBox.Show("Amount is not true!");
                _ = txtAmount.Focus();
                return;
            }
            else if(dcmlAmount <= 0)
            {
                _ = MessageBox.Show("Amount should be positive decimal!");
                _ = txtAmount.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtDate.SelectedDate.ToString()))
            {
                _ = MessageBox.Show("Date is empty!");
                txtDate.SelectedDate = DateTime.Now;
                _ = txtDate.Focus();
                return;
            }
            else if (!DateTime.TryParse(txtDate.SelectedDate.ToString(), out DateTime dttmDate))
            {
                _ = MessageBox.Show("Date is not correct!");
                txtDate.SelectedDate = DateTime.Now;
                _ = txtDate.Focus();
                return;
            }

            BudgetDetail budgetDetail = new BudgetDetail();
            budgetDetail.ID = txtId.Text.Trim();
            budgetDetail.BUDGETMAINID = txtId.Text.Trim();
            budgetDetail.FMNO = txtFmNo.Text.Trim();
            budgetDetail.FMNAME = txtFmName.Text.Trim();
            budgetDetail.CISICODE = txtCisiCode.Text.Trim();
            budgetDetail.BDGTCURR = txtbdgtCurr.Text.Trim();
            budgetDetail.AMOUNT = decimal.Parse(txtAmount.Text);
            budgetDetail.TDATE = txtDate.SelectedDate.Value.ToString("d");
            budgetDetail.DOCNU = "BudgetMain:" + budgetDetail.ID + "/";

            BudgetDetailMain bdgtDetailMain = new BudgetDetailMain();
            if (bdgtDetailMain.SaveData("BudgetIn", budgetDetail))
            {
                _ = MessageBox.Show("Data is saved");
                Close();
            }
            else
            {
                _ = MessageBox.Show("Data couldn't saved!");
                Close();
            }
        }

        private void txtAmount_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtAmount.Text))
            {
                return;
            }

            if (!decimal.TryParse(txtAmount.Text, out decimal dcmlAmount))
            {
                _ = MessageBox.Show("Amount is not proper!");
                txtAmount.Text = "";
                return;
            }

            txtAmount.Text = dcmlAmount.ToString(curFormat);
        }
    }
}
