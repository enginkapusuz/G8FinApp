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
    /// Interaction logic for FiscalPending.xaml
    /// </summary>
    public partial class FiscalPending : Window
    {
        Pending pndngApprove;
        public FiscalPending(Pending pendingApprove)
        {
            InitializeComponent();
            pndngApprove = pendingApprove;
            InitTextBoxes();

        }

        private void InitTextBoxes()
        {
            txtFmName.Text = pndngApprove.FMNAME.Trim();

            txtCisiDesc.Text = pndngApprove.REQDESC.Trim();
            txtAmount.Text = pndngApprove.REQAMOUNT.ToString("#,#.0000");
            txtBdgtCurr.Text = pndngApprove.REQCURR;

            txtFmNo.Text = pndngApprove.FMNO.Trim();
            txtCisiCode.Text = pndngApprove.CISICODE.Trim();

            txtPendingNu.Text = pndngApprove.ID.Trim();
        }

        private void TxtTransAmount_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtTransAmount.Text))
            {
                return;
            }
            else
            {
                if (decimal.TryParse(txtTransAmount.Text, out decimal dcmlAmount))
                {
                    txtTransAmount.Text = dcmlAmount.ToString("N4");
                }
            }
        }

        private void CmbActCode_SelectedChange(object sender, SelectionChangedEventArgs e)
        {
            ActivityCodeMain actCodeMain = new ActivityCodeMain();

            IEnumerable<ActivityCode> descLst = from ActivityCode actCode in actCodeMain
                                                where actCode.ACTCODE == txtActCode.SelectedItem.ToString()
                                                select actCode;

            foreach (var i in descLst)
            {
                txtActDesc.Text = i.ACTDESC.ToString();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            PendingMain pendingMain;
            FiscalApproveMain fiscalApproveMain;

            if (string.IsNullOrEmpty(txtActCode.Text))
            {
                _ = MessageBox.Show("Please select an activity code!");
                return;
            }

            if (!DateTime.TryParse(txtDate.Text, out DateTime dtTmDate))
            {
                _ = MessageBox.Show("Please enter a valid date!");
                return;
            }

            if (string.IsNullOrEmpty(txtTransAmount.Text))
            {
                _ = MessageBox.Show("Please enter a commit amount!");
                return;
            }

            if (!decimal.TryParse(txtTransAmount.Text, out decimal dcmlPendingAmount))
            {
                _ = MessageBox.Show("Pending Amount is not proper!");
                return;
            }

            if (dcmlPendingAmount > pndngApprove.REQAMOUNT)
            {
                _ = MessageBox.Show("Pending Amount is greater than BudgetAmount!");
                return;
            }

            if (dcmlPendingAmount < 0)
            {
                _ = MessageBox.Show("Pending Amount should be equal or greater than 0!");
                return;
            }

            if (MessageBox.Show("Do you want to save pending number?", "Pending Save", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            txtPendingNu.Text = "P" + (int.Parse(pndngApprove.ID) + 1).ToString().PadLeft(4, '0');

            pendingMain = new PendingMain();
            fiscalApproveMain = new FiscalApproveMain();

            pndngApprove.REQAMOUNT = dcmlPendingAmount;
            pndngApprove.ACTCODE = txtActCode.Text.Trim();
            pndngApprove.PENDINGNO = txtPendingNu.Text.Trim();
            pndngApprove.APPDATE = dtTmDate;

            if (!pendingMain.SaveData(pndngApprove))
            {
                _ = MessageBox.Show("Data couldn't saved!");
                return;
            }

            pndngApprove.APPROVECHOICE = "Pending";
            if (!fiscalApproveMain.UpdateDataPending(pndngApprove))
            {
                _ = MessageBox.Show("Data couldn't save!");
                return;
            }

            pndngApprove.APPROVECHOICE = "Empty";
            pndngApprove.APPDATE = DateTime.Parse(DateTime.Now.ToString("d"));
            pndngApprove.SENDTO = "Fiscal";

            if (!fiscalApproveMain.UpdateDataPurchasing(pndngApprove))
            {
                _ = MessageBox.Show("Data couldn't saved!");
                return;
            }
            
            _ = MessageBox.Show("Data is saved!");

            Close();
        }
    }
}
