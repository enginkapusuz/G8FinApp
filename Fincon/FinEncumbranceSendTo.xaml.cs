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

namespace G8FinApp.Fincon
{
    /// <summary>
    /// Interaction logic for FinEncumbranceSendTo.xaml
    /// </summary>
    public partial class FinEncumbranceSendTo : Window
    {
        FinconApprove finApprove;
        public FinEncumbranceSendTo(FinconApprove fnApprove)
        {
            InitializeComponent();
            finApprove = fnApprove;
            InitTextBoxes();
        }

        private void InitTextBoxes()
        {
            //txtId.Text = finApprove.MAININD;
            //txtEncumId.Text = finApprove.ENCUMID;
            txtReqDesc.Text = finApprove.REQDESC;

            txtSender.Text = finApprove.SENDTO;
            txtFmName.Text = finApprove.FMNAME;
            txtReqAmount.Text = finApprove.REQAMOUNT.ToString("#,#.00");
            
            txtReqCurr.SelectedText = finApprove.REQCURR;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            FinconApproveMain finconAppMain;

            if (!DateTime.TryParse(txtDate.Text, out DateTime dtTmDate))
            {
                _ = MessageBox.Show("Please select a date!");
                return;
            }
            
            if(string.IsNullOrEmpty(txtApprove.Text))
            {
                _ = MessageBox.Show("Please select an approve choice!");
                return;
            }

            if(string.IsNullOrEmpty(txtSendTo.Text))
            {
                _ = MessageBox.Show("Please select a send to!");
                return;
            }

            if(MessageBox.Show("Do you want to send this file!", "Send To file", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            finApprove.APPDATE = dtTmDate;
            finApprove.APPROVECHOICE = txtApprove.Text.Trim();
            finApprove.SENDTO = txtSendTo.Text.Trim();
            
            finconAppMain = new FinconApproveMain();

            if (!finconAppMain.UpdateData(finApprove))
            {
                _ = MessageBox.Show("Data couldn't save!");
                return;
            }

            StringBuilder sendToTable = new StringBuilder();

            if (finApprove.SENDTO == "Fiscal" && !string.IsNullOrEmpty(finApprove.PENDINGNO))
            {
                finApprove.APPROVECHOICE = "Empty";

                if (!finconAppMain.SaveDataFiscalWithPendingNo(finApprove))
                {
                    _ = MessageBox.Show("Data couldn't save!");
                    return;
                }
                _ = MessageBox.Show("Approval is saved!");
                Close();
                return;
            }

            if (finApprove.SENDTO == "Fiscal")
            {
                sendToTable.Append("FiscalApprove");
            }

            if (finApprove.SENDTO == "Budget" && finApprove.APPROVECHOICE == "Not Approve")
            {
                sendToTable.Append("BudgetNotapprove");
            }

            finApprove.SENDTO = "Fincon";
            finApprove.APPROVECHOICE = "Empty";
            
            if (!finconAppMain.SaveDataSendTo(sendToTable.ToString(), finApprove))
            {
                _ = MessageBox.Show("Data couldn't send!");
                return;
            }
            _ = MessageBox.Show("Approval is saved!");
            Close();
        }
    }
}
