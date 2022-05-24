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
    /// Interaction logic for BudgetTransfer.xaml
    /// </summary>
    /// 

    public partial class BudgetTransfer : Window
    {
        Budget transferOut;
        public BudgetTransfer(Budget slctdBudget)
        {
            InitializeComponent();
            transferOut = slctdBudget;
            InitTextFields();
        }

        private void InitTextFields()
        {
            txtId.Text = transferOut.ID.Trim();
            txtFmNo.Text = transferOut.FMNO.ToString();
            txtFmName.Text = transferOut.FMNAME.Trim();
            txtCisiCode.Text = transferOut.CISICODE.Trim();
            txtCisiDesc.Text = transferOut.CISIDESC.Trim();
            txtBdgtCurr.Text = transferOut.BDGTCURR.Trim();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (LstBdgtDetail.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select a record from list!");
                return;
            }
            else if (string.IsNullOrEmpty(txtTransAmount.Text))
            {
                _ = MessageBox.Show("Transfer amount is empty!");
                return;
            }
            else if(!decimal.TryParse(txtTransAmount.Text, out decimal decmlTransAmount))
            {
                _ = MessageBox.Show("Transfer Amount is not decimal!");
                return;
            }
            else if (decmlTransAmount > decimal.Parse(txtAmount.Text))
            {
                _ = MessageBox.Show("Transfer amount is greater than available amount!");
                return;
            }
            else if(!DateTime.TryParse(txtDate.Text, out DateTime dtTmDate))
            {
                _ = MessageBox.Show("Date is not correct!");
                return;
            }
            else
            {

                BudgetDetail bdgtDetailOut = new BudgetDetail();
                BudgetDetail bdgtDetailIn = new BudgetDetail();

                Budget trnsInBdgt = LstBdgtDetail.SelectedItem as Budget;
                BudgetDetailMain bdgtDtlMain = new BudgetDetailMain();

                if (trnsInBdgt == null)
                {
                    _ = MessageBox.Show("nuull");
                    return;
                }

               
                bdgtDetailOut.ID = txtId.Text.Trim();
                bdgtDetailOut.BUDGETMAINID = txtId.Text.Trim();
                bdgtDetailOut.FMNO = txtFmNo.Text.Trim();
                bdgtDetailOut.FMNAME = txtFmName.Text.Trim();
                bdgtDetailOut.CISICODE = txtCisiCode.Text.Trim();
                bdgtDetailOut.BDGTCURR = txtBdgtCurr.Text.Trim();
                bdgtDetailOut.TDATE = dtTmDate.ToString("d");
                bdgtDetailOut.AMOUNT = decimal.Parse(decmlTransAmount.ToString());
                bdgtDetailOut.DOCNU = "BudgetMain:" + bdgtDetailOut.ID + "/";


                bdgtDetailIn.ID = trnsInBdgt.ID;
                bdgtDetailIn.BUDGETMAINID = trnsInBdgt.ID;
                bdgtDetailIn.FMNO = trnsInBdgt.FMNO.ToString();
                bdgtDetailIn.FMNAME = trnsInBdgt.FMNAME;
                bdgtDetailIn.CISICODE = trnsInBdgt.CISICODE;
                bdgtDetailIn.BDGTCURR = trnsInBdgt.BDGTCURR;
                bdgtDetailIn.TDATE = bdgtDetailOut.TDATE;
                bdgtDetailIn.AMOUNT = bdgtDetailOut.AMOUNT;
                bdgtDetailIn.DOCNU = "BudgetMain:" + trnsInBdgt.ID + "/";


                if (bdgtDetailOut.BDGTCURR != bdgtDetailIn.BDGTCURR)
                {
                    _ = MessageBox.Show("Both budget record currency should be equal!");
                    return;
                }

                if (bdgtDtlMain.SaveData("BudgetTransferOut", bdgtDetailOut))
                {


                    if (bdgtDtlMain.SaveData("BudgetTransferIn", bdgtDetailIn))
                    {
                        _ = MessageBox.Show("Transfer is successful.");
                        Close();
                        return;
                    }
                }

                _ = MessageBox.Show("Transfer is not successful!");
            }
        }

        private void TxtTransAmount_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtTransAmount.Text))
            {
                if (decimal.TryParse(txtTransAmount.Text, out decimal dcmlTransAmount))
                {
                    txtTransAmount.Text = dcmlTransAmount.ToString("N4");
                }
            }
        }
    }
}
