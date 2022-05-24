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
    /// Interaction logic for BudgetNew.xaml
    /// </summary>
    public partial class BudgetNew : Window
    {
        CISICodeMain csdCodeMain;

        public BudgetNew()
        {
            InitializeComponent();
            LoadTxtFmName();
        }

        public BudgetNew(Admin.AdminFundManager adminFundManagers, Admin.FundManagerBudget fundManagerBudget)
        {
            InitializeComponent();
            LoadTxtFmName();
            txtFmNo.Text = adminFundManagers.fundManager.FundManagerNo.ToString();
            txtFmName.Text = adminFundManagers.fundManager.FundManagerName.ToString();
            txtCisiCode.Text = fundManagerBudget.CisiCode;
        }

        private void LoadTxtFmName()
        {
            BudgetMain budgetMain = new BudgetMain();
            budgetMain.InitList();
            foreach(var fmName in budgetMain.Select(bdgt => bdgt.FMNAME).Distinct())
            {
                txtFmName.Items.Add(fmName);
            }
        }
        private void Window_Initialized(object sender, EventArgs e)
        {
            csdCodeMain = new CISICodeMain();
            foreach(var cisi in csdCodeMain)
            {
                _  = txtCisiCode.Items.Add(cisi.CISICODE);
            }
        }

        private void txtCisiCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var cisiDescLst = from cisi in csdCodeMain
                              where cisi.CISICODE == txtCisiCode.SelectedItem.ToString()
                              select cisi.CISIDESC;

            foreach(string i in cisiDescLst)
            {
                txtCisiDesc.Text = i;
            }

        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            BudgetMain bdgtMain;

            if (string.IsNullOrEmpty(txtFmNo.Text))
            {
                _ = MessageBox.Show("Fm No field is empty!");
                _ = txtFmNo.Focus();
                return;
            }

            if (!int.TryParse(txtFmNo.Text, out int intFmNo))
            {
                _ = MessageBox.Show("Fm No should be a number like 1, 2,...!");
                return;
            }

            if(string.IsNullOrEmpty(txtFmName.Text))
            {
                _ = MessageBox.Show("Fm Name is empty!");
                _ = txtFmName.Focus();
                return;
            }
            if(string.IsNullOrEmpty(txtCisiCode.Text))
            {
                _ = MessageBox.Show("Cisi Code is empty!");
                return;
            }
            if(string.IsNullOrEmpty(txtBdgtCurr.Text))
            {
                _ = MessageBox.Show("Currency is empty!");
                _ = txtBdgtCurr.Focus();
                return;
            }
            
            Budget bdgt = new Budget();
            bdgtMain = new BudgetMain();
            bdgtMain.InitList();

            bdgt.FMNO = int.Parse(txtFmNo.Text);
            bdgt.FMNAME = txtFmName.Text.Trim();
            bdgt.CISICODE = txtCisiCode.Text.Trim();
            bdgt.CISIDESC = txtCisiDesc.Text.Trim();
            bdgt.BDGTCURR = txtBdgtCurr.Text.Split('{', '{')[2].Trim();

            if (!bdgtMain.SaveData(bdgt))
            {
                _ = MessageBox.Show("Data can't saved!");
                return;
            }
            _ = MessageBox.Show("Data is saved!");
            Close();
            
        }
    }
}
