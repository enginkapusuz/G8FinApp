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
    /// Interaction logic for FiscalCashCallEnter.xaml
    /// </summary>
    public partial class FiscalCashCallEnter : Window
    {
        Database.ProgramConsts prgrmConsts = new Database.ProgramConsts();
        CountryMain countryMain;
        public FiscalCashCallEnter()
        {
            CashCallMain cashCallMain = new CashCallMain();

            countryMain = new CountryMain();
            countryMain.InitList();
            InitializeComponent();

            TxtCountries.ItemsSource = countryMain;
            TxtPlannedPaymentDate.Text = DateTime.Now.Date.ToString("d");
            cashCallMain.InitList();
            LstMain.ItemsSource = cashCallMain;
            SumLstMain();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            CashCall cashCall;
            CashCallMain cashCallMain;

            if (TxtCountries.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select a country!");
                return;
            }

            if (!decimal.TryParse(TxtPlannedPayment.Text, out var dcmlPlannedPayment))
            {
                _ = MessageBox.Show("Planned Payment is not proper!");
                return;
            }

            if(string.IsNullOrEmpty(TxtRemarks.Text))
            {
                _ = MessageBox.Show("Remarks is empty!");
                return;
            }

            if (!DateTime.TryParse(TxtPlannedPaymentDate.Text, out DateTime dtTmPlannedPaymentDate))
            {
                _ = MessageBox.Show("Planned Payment Date is not proper!");
                return;
            }

            var selectCountry = TxtCountries.SelectedItem as Country;

            cashCall = new CashCall
            {
                FiscalCountriesID = selectCountry.ID,
                Remarks = TxtRemarks.Text.Trim(),
                PlannedPayment = dcmlPlannedPayment,
                PlannedPaymentDate = dtTmPlannedPaymentDate,
            };

            if (IsItemExistInLstMain(cashCall))
            {
                if (MessageBox.Show("There is a record with same amount! Do you want to save this payment!","Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }
            }

            cashCallMain = new CashCallMain();

            if (!cashCallMain.SaveData(cashCall))
            {
                return;
            }

            cashCallMain.InitList();
            LstMain.ItemsSource = cashCallMain;
            SumLstMain();
            ClearFields();
        }

        private void TxtPlannedPayment_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!decimal.TryParse(TxtPlannedPayment.Text, out decimal dcmlPlannedPayment))
            {
                return;
            }

            TxtPlannedPayment.Text = decimal.Parse(TxtPlannedPayment.Text).ToString(prgrmConsts.curFormat);
        }

        private void TxtCountries_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TxtCountries.SelectedIndex == -1)
            {
                return;
            }

            var selectedCountry = TxtCountries.SelectedItem as Country;

            var countryCount = (LstMain.ItemsSource as CashCallMain).Where(cshCall => cshCall.FiscalCountriesID == selectedCountry.ID).ToList();

            TxtRemarks.Text = (countryCount.Count + 1).ToString() +  "  Cash Call of " + selectedCountry.CountryName;
            TxtPlannedPayment.Text = "";
        }

        private void LstMain_Delete(object sender, RoutedEventArgs e)
        {
            CashCallMain cashCallMain;

            if(LstMain.SelectedIndex == -1)
            {
                return;
            }

            var cashCall = LstMain.SelectedItem as CashCall;

            if (cashCall.PaymentAmount > 0)
            {
                _ = MessageBox.Show("The Cash Call Amout is paid." +
                    Environment.NewLine + "You can't delete this record!" +
                    Environment.NewLine + "If you want to delete this, please inform Disbursing Off.!","Unauthorized Deletion", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            
            
            if (MessageBox.Show("Do you want to delete this item?","Delete", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            cashCallMain = new CashCallMain();
            if (!cashCallMain.DeleteData(cashCall))
            {
                _ = MessageBox.Show("Data couldn't save!");
                return;
            }

            _ = MessageBox.Show("Data is removed.");
            cashCallMain.InitList();
            LstMain.ItemsSource = cashCallMain;
            SumLstMain();
        }

        private bool IsItemExistInLstMain(CashCall cashCall)
        {
            var cashCallMain = LstMain.ItemsSource as CashCallMain;
            var countryCount = cashCallMain.Where(csh => csh.FiscalCountriesID == cashCall.FiscalCountriesID 
                                                 && csh.PlannedPayment == cashCall.PlannedPayment).ToList();

            if (countryCount.Count == 0)
            {
                return false;
            }

            return true;
        }

        private void ClearFields()
        {
            TxtCountries.Text = "";
            TxtPlannedPayment.Text = "";
            TxtPlannedPaymentDate.Text = "";
            TxtRemarks.Text = "";
        }

        private void BtnCommit_Click(object sender, RoutedEventArgs e)
        {
            CashCallMain cashCallMain;
            Fiscal.CashCall cashCall;
            Fiscal.MainCommit mainCommit;
            Fiscal.MissCommitMain missCommitMain = new Fiscal.MissCommitMain();
            missCommitMain.InitList();

            cashCall = LstMain.SelectedItem as Fiscal.CashCall;

            if (LstMain.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select an Item", "No Selected Item!", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            if (!string.IsNullOrEmpty(cashCall.CommitNumber))
            {
                _ = MessageBox.Show("The Payment has already been committed!", "Committed Record", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            if (cashCall.PaymentAmount <= 0)
            {
                _ = MessageBox.Show("Since the Payment Amount is zero, you cannot take commit number!", "Empty Payment Amount!", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            int lstComId = 0;
            string lstCommit = string.Empty;

            foreach (var cmt in missCommitMain.Select(cmt => cmt.ID))
            {
                lstCommit = cmt;
            }

            if (!string.IsNullOrEmpty(lstCommit))
            {
                lstComId = int.Parse(lstCommit);
            }

            cashCall.CommitDate = DateTime.Now;
            cashCall.CommitNumber = "88-00-00-" + (lstComId + 1).ToString().PadLeft(4, '0');

            mainCommit = new Fiscal.MainCommit()
            {
                CommitNu = cashCall.CommitNumber,
                CommitDate = cashCall.CommitDate,
                TableName = "FiscalCashCall",
            };

            if (!missCommitMain.SaveData(mainCommit))
            {
                _ = MessageBox.Show("Data couldn't save!");
                return;
            }

            cashCallMain = new CashCallMain();

            if(!cashCallMain.SaveComitNumber(cashCall))
            {
                _ = MessageBox.Show("Data couldn't save!");
                return;
            }

            _ = MessageBox.Show("Data is saved.", "Saving", MessageBoxButton.OK, MessageBoxImage.Information);
            cashCallMain.InitList();
            LstMain.ItemsSource = cashCallMain;
        }

        private void SumLstMain()
        {
            CashCallMain cashCallMain = LstMain.ItemsSource as CashCallMain;
            TxtLstMainPlannedPaymentTotal.Text = cashCallMain.Select(cshCall => cshCall.PlannedPayment).Sum().ToString(prgrmConsts.curFormat);
            TxtLstMainPaymentAmountTotal.Text = cashCallMain.Select(cshCall => cshCall.PaymentAmount).Sum().ToString(prgrmConsts.curFormat);
        }

    }

}
