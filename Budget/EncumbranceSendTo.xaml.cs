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
    /// Interaction logic for EncumbranceSendTo.xaml
    /// </summary>
    public partial class EncumbranceSendTo : Window
    {
        private Approve approve = null;

        public EncumbranceSendTo(Approve _approve)
        {
            InitializeComponent();
            approve = _approve;

            txtId.Text = approve.ENCUMID.Trim();
            txtFmNo.Text = approve.FMNO.Trim();
            txtFmName.Text = approve.FMNAME.Trim();
            txtCisiCode.Text = approve.CISICODE.Trim();
            txtBdgtCurr.Text = approve.BDGTCURR.Trim();
            txtBdgtAmount.Text = approve.BDGTAMOUNT.ToString("#,#.0000");
            txtReqAmount.Text = approve.REQAMOUNT.ToString("#,#.0000");
            txtReqCurr.Text = approve.REQCURR.Trim();
            txtReqDesc.Text = approve.REQDESC.Trim();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Approve finapprove;
            ApproveMain approveMain;

            if (approve is null)
            {
                _ = MessageBox.Show("Main List is empty!");
                return;
            }

            string sendToTable = string.Empty;

            finapprove = new Approve()
            {
                MAININD = approve.MAININD,
                ENCUMID = approve.ENCUMID,
                TABLENAME = approve.TABLENAME,
                FMNAME = approve.FMNAME,
                REQDESC = approve.REQDESC,
                REQAMOUNT = approve.REQAMOUNT,
                REQCURR = approve.REQCURR,
                BDGTCURR = approve.BDGTCURR,
                BDGTAMOUNT = approve.BDGTAMOUNT,
            };

            if (string.IsNullOrEmpty(txtSendUser.Text))
            {
                _ = MessageBox.Show("Please select a user!");
                return;
            }

            finapprove.SENDTO = txtSendUser.Text.Trim();

            if (string.IsNullOrEmpty(txtApproveChoice.Text))
            {
                _ = MessageBox.Show("Please Approve choice!");
                return;
            }

            finapprove.APPROVECHOICE = txtApproveChoice.Text;

            if (!DateTime.TryParse(txtDate.Text, out DateTime dtTmDate))
            {
                _ = MessageBox.Show("Please select a date!");
                return;
            }

            finapprove.APPDATE = dtTmDate.ToString("d");

            if (finapprove.SENDTO == "Fincon")
            {
                sendToTable = "FinconApprove";
            }

            if (finapprove.SENDTO == "Fiscal")
            {
                sendToTable = "FiscalApprove";
            }


            finapprove.SENDTO = "Budget";
            finapprove.APPROVECHOICE = "Empty";

            approveMain = new ApproveMain();


            if (!approveMain.SaveSendToData(sendToTable, finapprove))
            {
                _ = MessageBox.Show("Data couldn't send to Fincon!");
                return;
            }

            finapprove.SENDTO = txtSendUser.Text.Trim();
            finapprove.APPROVECHOICE = txtApproveChoice.Text;


            if (!approveMain.SaveData("BudgetApprove", finapprove))
            {
                _ = MessageBox.Show("Data couldn't saved!");
                return;
            }
            _ = MessageBox.Show("Data is saved!");
            Close();
        }
    }
}
