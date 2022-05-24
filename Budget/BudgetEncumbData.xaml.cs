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
    /// Interaction logic for BudgetEncumbData.xaml
    /// </summary>
    public partial class BudgetEncumbData : Window
    {
        private const string curFormat = "#,#.0000";
        BudgetEncumbrance bdgtEncumWin;
        
        public BudgetEncumbData(BudgetEncumbrance budgetEncumbrance, Admin.FundManagerEncumbrance fundManagerEncumbrance)
        {
            InitializeComponent();
            bdgtEncumWin = budgetEncumbrance;

            txtId.Text = fundManagerEncumbrance.FundManagerID.ToString();
            txtBdgtCurr.Text = fundManagerEncumbrance.ReqCurr;
            txtDate.Text = fundManagerEncumbrance.CommitDate.ToString();
            txtExchngRate.Text = fundManagerEncumbrance.ReqAmount == 0 ? "1" :
                (fundManagerEncumbrance.ReqAmount / fundManagerEncumbrance.CommitEuro).ToString("N4");

            txtReqAmount.Text = fundManagerEncumbrance.ReqAmount.ToString();
            txtReqDesc.Text = fundManagerEncumbrance.ReqDesc;
            txtReqNu.Text = fundManagerEncumbrance.ReqNum;
            txtReqItemCount.Text = "1";

            var currIndx = (txtReqCurr.ItemsSource as CurrencyMain).Select(curr => curr.CURRCODE).ToList().IndexOf(fundManagerEncumbrance.ReqCurr);

            if (currIndx == -1)
            {
                _ = MessageBox.Show("Request Currency is not in the List!" + Environment.NewLine +
                    "Currency is" + fundManagerEncumbrance.ReqCurr);
                return;
            }

            txtReqCurr.SelectedIndex = currIndx;

            BtnSave.Focus();
        }

        public BudgetEncumbData(BudgetEncumbrance budgetEncumbrance)
        {
            InitializeComponent();
            bdgtEncumWin = budgetEncumbrance;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            BudgetData budgetData;
            BudgetDataMain budgetDataMain;

            if (string.IsNullOrEmpty(txtReqDesc.Text))
            {
                _ = MessageBox.Show("Request Description is empty!");
                return;
            }

            if (string.IsNullOrEmpty(txtReqNu.Text))
            {
                _ = MessageBox.Show("Request Nu is empty!");
                return;
            }

            if (string.IsNullOrEmpty(txtReqItemCount.Text))
            {
                _ = MessageBox.Show("Request Count is empty!");
                return;
            }

            if (string.IsNullOrEmpty(txtReqCurr.Text))
            {
                _ = MessageBox.Show("Currency is empty!");
                return;
            }

            if (string.IsNullOrEmpty(txtReqAmount.Text))
            {
                _ = MessageBox.Show("Request Amount is empty!");
                return;
            }

            if (string.IsNullOrEmpty(txtExchngRate.Text))
            {
                _ = MessageBox.Show("Exchange Rate is empty!");
                return;
            }

            if (string.IsNullOrEmpty(txtDate.Text))
            {
                _ = MessageBox.Show("Date is empty!");
                return;
            }

            if (!int.TryParse(txtReqItemCount.Text, out int intReqItemCount) || !(intReqItemCount > 0))
            {
                _ = MessageBox.Show("Req Item Count is not proper!");
                return;
            }

            if (!decimal.TryParse(txtReqAmount.Text, out decimal dcmlReqAmount) || !(dcmlReqAmount >= 0))
            {
                _ = MessageBox.Show("Request Amount is not proper!");
                return;
            }

            if (!decimal.TryParse(txtExchngRate.Text, out decimal dcmlExchngRate) || !(dcmlExchngRate > 0))
            {
                _ = MessageBox.Show("Exchange Rate is not proper!");
                return;
            }

            if (!DateTime.TryParse(txtDate.Text, out DateTime dtTmDate))
            {
                _ = MessageBox.Show("Data is not proper!");
                return;
            }

            if (txtReqCurr.Text.Contains(txtBdgtCurr.Text) && dcmlExchngRate != 1)
            {
                MessageBox.Show("Exchange Rate should be 1 since Budget Currency is same with Request Currency!");
                txtExchngRate.Text = "1";
                dcmlExchngRate = 1;
            }

            decimal bdgtTransAmount = dcmlReqAmount == 0 ? dcmlReqAmount : decimal.Parse((dcmlReqAmount / dcmlExchngRate).ToString(curFormat));

            if (bdgtTransAmount < 0)
            //if (bdgtTransAmount <= 0)
            {
                _ = MessageBox.Show("Budget amount cannot be less than 0");
                return;
            }

            if (bdgtTransAmount > decimal.Parse(bdgtEncumWin.txtAmount.Text))
            {
                _ = MessageBox.Show("Budget amount is not enough for this!");
                return;
            }

            budgetDataMain = new BudgetDataMain();
            budgetData = new BudgetData()
            {
                ID = txtId.Text,
                REQDESC = txtReqDesc.Text.Trim(),
                REQNUM = txtReqNu.Text.Trim(),
                REQITEMCOUNT = intReqItemCount.ToString(),
                REQCURR = txtReqCurr.Text.Split('{', '{')[2].Trim(),
                REQAMOUNT = dcmlReqAmount,
                REQDATE = dtTmDate.ToString("d"),
                REQDOCNU = "BudgetMain:" + txtId.Text + "/" + "BudgetEncmbData:" + bdgtEncumWin.txtEncumbId.Text.Trim(),
            };


            string lstId = string.Empty;

            if (!budgetDataMain.SaveData(budgetData, ref lstId))
            {
                _ = MessageBox.Show("Data couldn't saved!");
                return;
            }

            _ = MessageBox.Show("Data is saved!");
            bdgtEncumWin.txtEncumbId.Text = lstId;
            bdgtEncumWin.txtTransAmount.Text = bdgtTransAmount.ToString(curFormat);
            Close();
        }

        private void TxtReqAmount_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtReqAmount.Text))
            {
                return;
            }

            if (!decimal.TryParse(txtReqAmount.Text, out decimal dcmlReqAmount))
            {
                return;
            }

            txtReqAmount.Text = dcmlReqAmount.ToString(curFormat);
        }

        private void CmbReqCurrency_LostFocus(object sender, RoutedEventArgs e)
        {
            
        }

        private void TxtExchngRate_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtExchngRate.Text.Contains("Please"))
            {
                txtExchngRate.Text = "";
            }
        }

        private void TxtExchngRate_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtExchngRate.Text))
            {
                return;
            }

            if (!decimal.TryParse(txtExchngRate.Text, out decimal dcmlExchngRate))
            {
                return;
            }

            txtExchngRate.Text = dcmlExchngRate.ToString("#,#.0000");
        }
    }
}
