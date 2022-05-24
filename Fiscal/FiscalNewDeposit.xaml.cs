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
    /// Interaction logic for FiscalNewDeposit.xaml
    /// </summary>
    public partial class FiscalNewDeposit : Window
    {
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();
        Purchasing.Deposit deposit;
        public FiscalNewDeposit()
        {
            Purchasing.DepositMain depositMain = new Purchasing.DepositMain(isInitList:true, userName: "Fiscal");
            InitializeComponent();
            LstMain.ItemsSource = depositMain;
        }

        private void LstMain_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
            if (LstMain.SelectedIndex == -1)
            {
                return;
            }

            deposit = LstMain.SelectedItem as Purchasing.Deposit;

            TxtContractNo.Text = deposit.ContractNo;
            TxtBiddingName.Text = deposit.BiddingName;
            TxtCompany.Text = deposit.Company;

            TxtPcAmount.Text = deposit.BiddingPrice.ToString(prgrmConst.curFormat);
            TxtCurrency.Text = deposit.BiddingCurr;
            TxtDepositRate.Text = deposit.DepositRate.ToString(prgrmConst.curFormat);

            TxtDisbDepositAmount.Text = deposit.DisbDepositAmount.ToString(prgrmConst.curFormat);
            TxtDepositCurrency.Text = deposit.DepositCurrency;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            MainCommit mainCommit;
            MissCommitMain missCommitMain = new MissCommitMain();
            missCommitMain.InitList();

            int lstComId = 0;
            var commits = missCommitMain.Select(cmt => cmt.ID);
            foreach(var cmt in commits)
            {
                lstComId = int.Parse(cmt);
            }

            Purchasing.DepositMain depositMain = new Purchasing.DepositMain();

            if (!DateTime.TryParse(TxtDepositDate.Text, out DateTime dtTmDepositDate))
            {
                _ = MessageBox.Show("Deposit date is not proper!");
                return;
            }

            if (MessageBox.Show("Do you want to approve the Deposit?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                _ = MessageBox.Show("Saving process has been cancelled!");
                return;
            }

            deposit.CommitDate = dtTmDepositDate;
            deposit.CommitNu = "88-00-00-" + (lstComId + 1).ToString().PadLeft(4,'0');

            if (MessageBox.Show("The Commit Nu:" + deposit.CommitNu, "Confirmation", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                _ = MessageBox.Show("Saving process has been cancelled!");
                return;
            }

            if (!depositMain.FiscalUpdate(deposit))
            {
                _ = MessageBox.Show("Data couldn't save!");
                return;
            }

            mainCommit = new MainCommit()
            {
                CommitNu = deposit.CommitNu,
                CommitDate = deposit.CommitDate,
                TableName = "Deposit",
            };

            if (!missCommitMain.SaveData(mainCommit))
            {
                _ = MessageBox.Show("Data couldn't save!");
                return;
            }

            _ = MessageBox.Show("Data is saved.");

            Close();
        }
    }
}
