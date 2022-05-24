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
    /// Interaction logic for FiscalCommit.xaml
    /// </summary>
    public partial class FiscalCommit : Window
    {
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();
        private readonly FiscalApprove _fiscalApprove;
        public FiscalCommit(FiscalApprove fiscalApprove)
        {
            _fiscalApprove = fiscalApprove;
            InitializeComponent();
            InitTextBoxes();
        }

        private void InitTextBoxes()
        {
            txtFmNo.Text = _fiscalApprove.FMNO;
            txtFmName.Text = _fiscalApprove.FMNAME.Trim();
            txtCisiCode.Text = _fiscalApprove.CISICODE.Trim();
            txtCisiDesc.Text = _fiscalApprove.CISIDESC.Trim();

            txtBdgtAmount.Text = _fiscalApprove.BDGTAMOUNT.ToString(prgrmConst.curFormat);
            TxtBdgtCurr.Text = _fiscalApprove.BDGTCURR.Trim();

            txtReqCurr.Text = _fiscalApprove.REQCURR;
            txtReqAmount.Text = _fiscalApprove.REQAMOUNT.ToString(prgrmConst.curFormat);
            txtExRate.Text = _fiscalApprove.REQAMOUNT <= 0 ? "0" :
                (_fiscalApprove.REQAMOUNT / _fiscalApprove.BDGTAMOUNT).ToString(prgrmConst.curFormat);

            txtCommitNu.Text = _fiscalApprove.LASTCOMID.ToString();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Commit fsclCommit;
            MainCommit mainCommit;
            CommitMain commitMain;
            MainCommitMain mainCommitMain;
            FiscalApprove fiscalApprove;
            FiscalApproveMain fiscalApproveMain;
            
            decimal _dcmlExRate;

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

            if (!decimal.TryParse(txtExRate.Text, out decimal dcmlExRate))
            {
                _ = MessageBox.Show("Please enter valid Exchange Rate!");
                return;
            }
            else
            {
                _dcmlExRate = dcmlExRate;
            }
            
            if(_fiscalApprove.BDGTCURR == _fiscalApprove.REQCURR && _dcmlExRate != 1)
            {
                _ = MessageBox.Show("Currencies are same therefore Exchange Rate should be 1!");
                return;
            }

            if (string.IsNullOrEmpty(txtTransAmount.Text))
            {
                _ = MessageBox.Show("Please enter a commit amount!");
                return;
            }

            if (!decimal.TryParse(txtTransAmount.Text, out decimal dcmlCommitAmount))
            {
                _ = MessageBox.Show("Please enter valid Commit Amount!");
                return;

            }
            else if(dcmlCommitAmount < 0)
            {
                _ = MessageBox.Show("Commit Amount should be greater or equal to 0!");
                return;
            }
            else if(dcmlCommitAmount > decimal.Parse(txtReqAmount.Text))
            {
                _ = MessageBox.Show("Commit Amount is greater than Budget Amount!");
                return;
            }

            txtCommitNu.Text = DateTime.Now.ToString("yy") +
                "-" + txtFmNo.Text.Trim().PadLeft(2,'0') +
                "-" + txtActCode.Text.Trim() +
                "-" + (int.Parse(txtCommitNu.Text) + 1).ToString().PadLeft(4, '0');


            if (MessageBox.Show("Do you want to save commit number?", "Commit Save", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                Close();
                return;
            }

            fsclCommit = new Commit();
            mainCommit = new MainCommit();

            commitMain = new CommitMain();
            mainCommitMain = new MainCommitMain();

            fiscalApprove = new FiscalApprove();
            fiscalApproveMain = new FiscalApproveMain();

            fsclCommit.MAININD = _fiscalApprove.MAININD;
            fsclCommit.ENCUMID = _fiscalApprove.ENCUMID;
            fsclCommit.COMMITNO = txtCommitNu.Text.Trim();

            fsclCommit.APPDATE = dtTmDate;
            fsclCommit.REQAMOUNT = dcmlCommitAmount;
            fsclCommit.REQCURR = _fiscalApprove.REQCURR;
            
            fsclCommit.ACTCODE = txtActCode.Text.Trim();
            fsclCommit.BDGTAMOUNT = _fiscalApprove.BDGTAMOUNT;
            fsclCommit.BDGTCURR = _fiscalApprove.BDGTCURR;
            fsclCommit.EXRATE = _dcmlExRate == 0 ? 1 : _dcmlExRate;

            fiscalApprove.MAININD = fsclCommit.MAININD.Trim();
            fiscalApprove.ENCUMID = fsclCommit.ENCUMID.Trim();
            fiscalApprove.TABLENAME = "BudgetEncumbrance";

            fiscalApprove.SENDTO = "Fiscal";
            fiscalApprove.APPROVECHOICE = "Committed";
            fiscalApprove.APPDATE = dtTmDate;
            fiscalApprove.FMNAME = txtFmName.Text.Trim();

            fiscalApprove.REQDESC = _fiscalApprove.REQDESC;
            fiscalApprove.REQAMOUNT = _fiscalApprove.REQAMOUNT; ;
            fiscalApprove.REQCURR = _fiscalApprove.REQCURR;

            fiscalApprove.BDGTCURR = fsclCommit.BDGTCURR;
            fiscalApprove.BDGTAMOUNT = fsclCommit.REQAMOUNT / fsclCommit.EXRATE;

            mainCommit.CommitDate = fsclCommit.APPDATE;
            mainCommit.CommitNu = fsclCommit.COMMITNO;
            mainCommit.TableName = "FiscalCommit";

            if (!fiscalApproveMain.SaveData(fiscalApprove))
            {
                _ = MessageBox.Show("Data couldn't save");
                return;
            }

            if (!fiscalApproveMain.UpdateData(_fiscalApprove.ID, "Committed"))
            {
                _ = MessageBox.Show("Data couldn't save");
                return;
            }

            if (!mainCommitMain.SaveData(mainCommit))
            {
                _ = MessageBox.Show("Data couldn't save!");
                return;
            }

            if (!commitMain.SaveData(fsclCommit))
            {
                _ = MessageBox.Show("Data couldn't save!");
                return;
            }
            _ = MessageBox.Show("Data is saved");
            Close();
        }

        private void CmbActCode_SelectedChange(object sender, SelectionChangedEventArgs e)
        {
            ActivityCodeMain actCodeMain = new ActivityCodeMain();

            IEnumerable<ActivityCode> descLst = from ActivityCode actCode in actCodeMain
                                                where actCode.ACTCODE == txtActCode.SelectedItem.ToString()
                                                select actCode;

            foreach(var i in descLst)
            {
                txtActDesc.Text = i.ACTDESC.ToString();
            }
        }

        private void TxtCommitNu_GotFocus(object sender, RoutedEventArgs e)
        {
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
                    txtTransAmount.Text = dcmlAmount.ToString(prgrmConst.curFormat);
                }
            }
        }
    }
}
