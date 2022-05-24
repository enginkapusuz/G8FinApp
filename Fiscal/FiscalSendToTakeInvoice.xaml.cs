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
    /// Interaction logic for FiscalSendToTakeInvoice.xaml
    /// </summary>
    public partial class FiscalSendToTakeInvoice : Window
    {
        Disbursing.Approve disbursingApprove;
        Disbursing.ApproveMain disbursingApproveMain;
        private const string currFormat = "#,#.0000";

        CommitApproveMain commitApproveMain;

        private Invoice INVOICE
        {
            get;
            set;
        }
        public FiscalSendToTakeInvoice(Invoice invoice)
        {
            InitializeComponent();

            INVOICE = invoice;
            txtCommitID.Text = invoice.COMMITID;
            txtCommitAmount.Text = invoice.COMMITAMOUNT.ToString(currFormat);
            txtTotInvAmount.Text = invoice.TOTINVAMOUNT.ToString(currFormat);
            //txtInvAmount.Text = (invoice.COMMITAMOUNT - invoice.TOTINVAMOUNT).ToString(currFormat);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            InvoiceMain invoiceMain;
            FiscalApproveMain fiscalApproveMain;

            if (string.IsNullOrEmpty(txtInvoiceNu.Text))
            {
                _ = MessageBox.Show("Please enter Invoice Number!");
                return;
            }

            if (!DateTime.TryParse(txtDate.Text, out DateTime dtTmDate))
            {
                _ = MessageBox.Show("Please enter Invoice Date!");
                return;
            }

            if(!decimal.TryParse(txtInvAmount.Text, out decimal dcmlInvAmount))
            {
                _ = MessageBox.Show("Please enter Invoice Amount!");
                return;
            }
            else if(dcmlInvAmount <= 0)
            {
                _ = MessageBox.Show("Invoice Amount should be positive!");
                return;
            }
            else if(dcmlInvAmount > INVOICE.COMMITAMOUNT)
            {
                _ = MessageBox.Show("Invoice Amount is greater than Commit Amount!");
                return;
            }
            else if(dcmlInvAmount + INVOICE.TOTINVAMOUNT > INVOICE.COMMITAMOUNT)
            {
                _ = MessageBox.Show("Invoice Amount + Total Paid Amount (>) greater than Commit Amount!");
                return;
            }
            else
            {
                txtInvAmount.Text = dcmlInvAmount.ToString(currFormat);
            }


            if (string.IsNullOrEmpty(txtCreditor.Text))
            {
                _ = MessageBox.Show("Please enter Creditor Name");
                return;
            }

            INVOICE.COMMITID = txtCommitID.Text.Trim();
            INVOICE.INVOICENU = txtInvoiceNu.Text.Trim();
            INVOICE.INVOICEDATE = dtTmDate;
            INVOICE.INVOICEAMOUNT = dcmlInvAmount;
            INVOICE.CREDITOR = txtCreditor.Text.Trim();

            invoiceMain = new InvoiceMain();

            if (!invoiceMain.SaveData(INVOICE)) 
            {
                _ = MessageBox.Show("Data couldn't save!");
                return;
            }

            fiscalApproveMain = new FiscalApproveMain();

            if(!fiscalApproveMain.GetFiscalApproveData(INVOICE.COMMITID))
            {
                _ = MessageBox.Show("Halloua!");
            }

            disbursingApprove = new Disbursing.Approve();
            disbursingApproveMain = new Disbursing.ApproveMain();

            commitApproveMain = new CommitApproveMain();

            foreach(CommitApprove cmtApp in commitApproveMain)
            {
                if (cmtApp.CommitId == INVOICE.COMMITID)
                {
                    disbursingApprove.MainId = cmtApp.MainId;
                    disbursingApprove.EncumbId = cmtApp.EncumbId;
                    disbursingApprove.ReqDesc = cmtApp.ReqDesc;

                    disbursingApprove.InvCurr = cmtApp.ReqCurr;

                    disbursingApprove.BdgtCurr = cmtApp.BdgtCurr;
                    disbursingApprove.BdgtAmount = cmtApp.BdgtAmount;
                    break;
                }
            }

            disbursingApprove.SendTo = "Fiscal";
            disbursingApprove.ApproveChoice = "Empty";
            disbursingApprove.AppDate = DateTime.Parse(DateTime.Now.ToString("d"));

            disbursingApprove.InvNu = INVOICE.INVOICENU;
            disbursingApprove.InvAmount = INVOICE.INVOICEAMOUNT;
            disbursingApprove.InvDate = INVOICE.INVOICEDATE;
            disbursingApprove.InvCreditor = INVOICE.CREDITOR;

            
            //Invoice Portion calculation
            disbursingApprove.BdgtAmount = disbursingApprove.InvAmount / INVOICE.COMMITAMOUNT * disbursingApprove.BdgtAmount;

            if (!disbursingApproveMain.SaveData(disbursingApprove))
            {
                _ = MessageBox.Show("Disbursing data couldn't save!");
                return;
            }

            Close();
        }
    }
}
