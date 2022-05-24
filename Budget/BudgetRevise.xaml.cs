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
    /// Interaction logic for BudgetRevise.xaml
    /// </summary>
    public partial class BudgetRevise : Window
    {
        Budget budget;
        public BudgetRevise(Budget _budget)
        {
            InitializeComponent();
            budget = _budget;

            TxtId.Text = budget.ID.Trim();
            TxtFmNo.Text = budget.FMNO.ToString();
            TxtFmName.Text = budget.FMNAME.Trim();
            TxtCisiCode.Text = budget.CISICODE.Trim();
            TxtCisiDesc.Text = budget.CISIDESC.Trim();
            TxtCurr.Text = budget.BDGTCURR.Trim();
            TxtAvailAmount.Text = budget.CURRAMOUNT.ToString("#,#.00");
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            BudgetDetail budgetDetail;
            BudgetDetailMain budgetDetailMain;

            if (string.IsNullOrEmpty(TxtReviseAmount.Text))
            {
                _ = MessageBox.Show("Revise amount is empty!");
                return;
            }

            if (!decimal.TryParse(TxtReviseAmount.Text, out decimal dcmlReviseAmount) || !(dcmlReviseAmount > 0))
            {
                _ = MessageBox.Show("Revise amount is not proper!");
                return;
            }

            if (dcmlReviseAmount > budget.CURRAMOUNT)
            {
                _ = MessageBox.Show("Revise amount is greater than Current Amount!");
                return;
            }

            if (!DateTime.TryParse(TxtDate.Text, out DateTime dtTmDate))
            {
                _ = MessageBox.Show("Date is not proper!");
                return;
            }

            budgetDetail = new BudgetDetail()
            {
                FMNO = budget.FMNO.ToString(),
                FMNAME = budget.FMNAME,
                CISICODE = budget.CISICODE,
                BDGTCURR = budget.BDGTCURR,
                AMOUNT = dcmlReviseAmount,
                TDATE = dtTmDate.ToString("d"),
                DOCNU = "BudgetMain:" + budget.ID + "/",
                BUDGETMAINID = budget.ID,
            };

            budgetDetailMain = new BudgetDetailMain();
            
            if (!budgetDetailMain.SaveData("BudgetRevise", budgetDetail))
            {
                _ = MessageBox.Show("Data couldn'ts save!");
                return;
            }

            _ = MessageBox.Show("Data is saved.");
            Close();
        }
    }
}
