
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
    /// Interaction logic for BudgetEncumbrance.xaml
    /// </summary>
    public partial class BudgetEncumbrance : Window
    {
        private Admin.FundManagerEncumbrance fundManagerEncumbrance;

        private const string curFormat = "#,#.0000";
        Budget bdgtSelected;
        public BudgetEncumbrance(Budget bdgtSlctd)
        {
            InitializeComponent();
            bdgtSelected = bdgtSlctd;
            InitTextFields();
        }

        public BudgetEncumbrance(Admin.FundManagerEncumbrance fundManagerEncumb)
        {
            InitializeComponent();

            fundManagerEncumbrance = fundManagerEncumb;

            txtId.Text = fundManagerEncumb.FundManagerID.ToString();
            txtFmNo.Text = fundManagerEncumb.FmNo.ToString();
            txtFmName.Text = fundManagerEncumb.FmName;
            txtCisiCode.Text = fundManagerEncumb.CisiCode;
            txtBdgtCurr.Text = fundManagerEncumb.FundBudgetCurr;
            txtDate.Text = fundManagerEncumb.ReqDate.ToString();
            txtAmount.Text = fundManagerEncumb.FundBudgetAmount.ToString();

            BtnSave.Focus();
        }

        private void InitTextFields()
        {
            txtId.Text = bdgtSelected.ID.Trim();
            txtFmNo.Text = bdgtSelected.FMNO.ToString();
            txtFmName.Text = bdgtSelected.FMNAME.Trim();
            txtCisiCode.Text = bdgtSelected.CISICODE.Trim();
            txtCisiDesc.Text = bdgtSelected.CISIDESC.Trim();
            txtBdgtCurr.Text = bdgtSelected.BDGTCURR.Trim();
        }

        private void TxtTransAmount_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtTransAmount.Text))
            {
                if (decimal.TryParse(txtTransAmount.Text, out decimal dcmlTransAmount))
                {
                    txtTransAmount.Text = dcmlTransAmount.ToString("#,#.0000");
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            BudgetEncumbData bdgtEncumbData;

            if (!DateTime.TryParse(txtDate.Text, out DateTime dtTmDate))
            {
                _ = MessageBox.Show("Date is not correct!");
                return;
            }
            else
            {                
                bdgtEncumbData =  fundManagerEncumbrance is null ? new BudgetEncumbData(this) : 
                    new BudgetEncumbData(this, fundManagerEncumbrance);

                bdgtEncumbData.txtId.Text = txtId.Text.Trim();
                bdgtEncumbData.txtBdgtCurr.Text = txtBdgtCurr.Text.Trim();
                bdgtEncumbData.ShowDialog();

                if (string.IsNullOrEmpty(txtEncumbId.Text))
                {
                    _ = MessageBox.Show("Encumbrance data couldn't save!");
                    return;
                }

                SaveData(dtTmDate);
            }
        }

        private void SaveData(DateTime dtTmDate)
        {
            BudgetDetailMain bdgtDtlMain = new BudgetDetailMain();
            BudgetDetail bdgtEncumbrance;

            decimal decmlTransAmount = decimal.Parse(decimal.Parse(txtTransAmount.Text).ToString(curFormat));

            bdgtEncumbrance = new BudgetDetail
            {
                ID = txtId.Text.Trim(),
                BUDGETMAINID = txtId.Text.Trim(),
                FMNO = txtFmNo.Text.Trim(),
                FMNAME = txtFmName.Text.Trim(),
                CISICODE = txtCisiCode.Text.Trim(),
                BDGTCURR = txtBdgtCurr.Text.Trim(),
                TDATE = dtTmDate.ToString("d"),
                AMOUNT = decmlTransAmount,
                DOCNU = "BudgetMain:" + txtId.Text.Trim() + "/" + "BudgetEncmbData:" + txtEncumbId.Text.Trim() + "/",
                BDGTENCMBDATAID = txtEncumbId.Text.Trim(),
            };


            if (bdgtDtlMain.SaveDataEncumbrance(bdgtEncumbrance))
            {
                _ = MessageBox.Show("Encumbrance is successful.");
                Close();
            }
            else
            {
                _ = MessageBox.Show("Encumbrance is not successful!");
            }
        }
    }
}
