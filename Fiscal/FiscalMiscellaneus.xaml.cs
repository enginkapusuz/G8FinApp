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
    /// Interaction logic for FiscalMiscellaneus.xaml
    /// </summary>
    public partial class FiscalMiscellaneus : Window
    {
        Database.ProgramConsts programConsts = new Database.ProgramConsts();
        public FiscalMiscellaneus()
        {
            InitializeComponent();
            MiscellaneousMain miscellaneousMain = new MiscellaneousMain();
            miscellaneousMain.InitList();
            LstMain.ItemsSource = miscellaneousMain;
            SumLstMain();

            Budget.CurrencyMain currencyMain = new Budget.CurrencyMain();
            TxtCurrencies.ItemsSource = currencyMain;
        }

        private void TxtPlannedPayment_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!decimal.TryParse(TxtPlannedPayment.Text, out decimal dcmlPlannedPayment))
            {
                return;
            }
            TxtPlannedPayment.Text = decimal.Parse(TxtPlannedPayment.Text).ToString(programConsts.curFormat);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Miscellaneous miscellaneous;
            MiscellaneousMain miscellaneousMain;

            if (string.IsNullOrEmpty(TxtEntryType.Text))
            {
                _ = MessageBox.Show("Entry Type is not proper!");
                return;
            }
            if (!decimal.TryParse(TxtPlannedPayment.Text, out decimal dcmlPlannedPayment) || !(dcmlPlannedPayment >= 0))
            {
                _ = MessageBox.Show("Planned payment is not proper!");
                return;
            }
            if (!DateTime.TryParse(TxtPlannedPaymentDate.Text, out DateTime dtTmPlannedPaymentDate))
            {
                _ = MessageBox.Show("Planned Payment Date is not proper!");
                return;
            }
            if (string.IsNullOrEmpty(TxtRemarks.Text))
            {
                _ = MessageBox.Show("Remarks is not proper!");
                return;
            }
            if(string.IsNullOrEmpty(TxtCurrencies.Text))
            {
                _ = MessageBox.Show("Currency is not proper!");
                return;
            }
            miscellaneous = new Miscellaneous()
            {
                EntryType = TxtEntryType.Text,
                PlannedPayment = dcmlPlannedPayment,
                PlannedPaymentDate = dtTmPlannedPaymentDate,
                Remarks = TxtRemarks.Text.Trim(),
                PaymentCurrency = (TxtCurrencies.SelectedItem as Budget.Currency).CURRCODE,
            };

            miscellaneousMain = new MiscellaneousMain();
            if (!miscellaneousMain.SaveData(miscellaneous))
            {
                _ = MessageBox.Show("Data couldn't save!");
                return;
            }
            _ = MessageBox.Show("Data is saved.");
            miscellaneousMain = new MiscellaneousMain();
            miscellaneousMain.InitList();
            LstMain.ItemsSource = miscellaneousMain;
            SumLstMain();
            ClearFields();
        }

        private void LstMain_Delete(object sender, RoutedEventArgs e)
        {
            Miscellaneous miscellaneous;
            MiscellaneousMain misellaneousMain;

            if (LstMain.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select an item from List!");
                return;
            }

            miscellaneous = LstMain.SelectedItem as Miscellaneous;

            if (miscellaneous.PaymentAmount > 0)
            {
                _ = MessageBox.Show("Since Payment Amount is greater than 0, you cannot delete this record!");
                return;
            }

            if(MessageBox.Show("Do you want to delete this record?","Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            misellaneousMain = new MiscellaneousMain();
            if (!misellaneousMain.DeleteData(miscellaneous))
            {
                _ = MessageBox.Show("The data couldn't be deleted!");
                return;
            }

            MessageBox.Show("Data is deleted");
            misellaneousMain.Clear();
            misellaneousMain.InitList();
            LstMain.ItemsSource = misellaneousMain;
            SumLstMain();
            ClearFields();
        }

        private void SumLstMain()
        {
            MiscellaneousMain miscellaneousMain = LstMain.ItemsSource as  MiscellaneousMain;
            TxtLstMainPlannedPaymentTotal.Text = miscellaneousMain.Select(missCell => missCell.PlannedPayment).Sum().ToString(programConsts.curFormat);
            TxtLstMainPaymentAmountTotal.Text = miscellaneousMain.Select(missCell => missCell.PaymentAmount).Sum().ToString(programConsts.curFormat);
        }

        private void ClearFields()
        {
            TxtEntryType.SelectedIndex = 0;
            TxtPlannedPayment.Text = "";
            TxtRemarks.Text = "";
        }

        private void BtnCommit_Click(object sender, RoutedEventArgs e)
        {
            MainCommit mainCommit;
            MissCommitMain missCommitMain;
            MiscellaneousCommitNu miscellaneousCommitNu = new MiscellaneousCommitNu();
            MiscellaneousMain miscellaneousMain = new MiscellaneousMain();

            Miscellaneous miscellaneous = LstMain.SelectedItem as Miscellaneous;

            if(!string.IsNullOrEmpty(miscellaneous.CommitNumber))
            {
                _ = MessageBox.Show("The commit number for the item is already given!");
                return;
            }

            if (string.IsNullOrEmpty(miscellaneous.InvoiceNumber))
            {
                _ = MessageBox.Show("You cannot take commit number which is not paid! ");
                return;
            }

            if (!DateTime.TryParse(TxtPlannedPaymentDate.Text, out DateTime dtTmPlannedPayment))
            {
                _ = MessageBox.Show("Commit Date is not proper!");
                return;
            }
            mainCommit = new MainCommit()
            {
                CommitDate = dtTmPlannedPayment,
                CommitNu = miscellaneous.CommitNumber,
                TableName = "Miscellaneous",
            };

            missCommitMain = new MissCommitMain();
            if (!missCommitMain.SaveData(mainCommit))
            {
                _ = MessageBox.Show("Data couldn't save!");
                return;
            }

            int lstComId = miscellaneousCommitNu.TakeLastCommitId();
            miscellaneous.CommitNumber = "88-00-00-" + (lstComId + 1).ToString().PadLeft(4, '0');
            miscellaneous.CommitDate = dtTmPlannedPayment;

            if (!miscellaneousMain.UpdateFiscalCommitNu(miscellaneous))
            {
                _ = MessageBox.Show("Data couldn't saved!");
                return;
            }

            _ = MessageBox.Show("Data is saved.");
            miscellaneousMain = new MiscellaneousMain();
            miscellaneousMain.InitList();
            LstMain.ItemsSource = miscellaneousMain;
            SumLstMain();
            ClearFields();
        }
    }
}
