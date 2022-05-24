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
using System.Printing;
using Excel = Microsoft.Office.Interop.Excel;


namespace G8FinApp.Disbursing
{
    /// <summary>
    /// Interaction logic for PaymentListWithItems.xaml
    /// </summary>
    public partial class PaymentListWithItems : Window
    {
        private PaymentList _paymentList;
        IEnumerable<Invoice> notPaidInvoices;
        InvoiceAccountMain invoiceAccountMain;

        //This varilable will be assigned in PaymentCashBook.xaml
        public bool isCashBookSaved { private get; set; }
        public DateTime cashBookDate { private get; set; }
        public string  bookCashLastId { private get; set; }

        //This variable will be assigned in PaymentListAddAccount.xaml.cs
        public bool isAccountAdded = false;

        public PaymentListWithItems(PaymentList paymentList)
        {
            InitializeComponent();
            _paymentList = paymentList;
            InitListMain();
            invoiceAccountMain = new InvoiceAccountMain();
        }

        public void InitListMain()
        {
            InvoiceMain invoiceMain = new InvoiceMain();
            invoiceMain.InitList();

            List<string> invIdList = (from InvoiceAccount inv in LstAddedAccount.ItemsSource as InvoiceAccountMain
                                     select inv.InvoiceId).ToList();

            notPaidInvoices = from inv in invoiceMain
                              where inv.PaymentListId == _paymentList.ID && !invIdList.Contains(inv.ID)
                              select inv;

            LstMain.ItemsSource = notPaidInvoices;
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            PaymentItemsPrint paymentItemsPrint = new PaymentItemsPrint(notPaidInvoices);
            paymentItemsPrint.ShowDialog();
        }

        private void OpenUpdateForm(object sender, RoutedEventArgs e)
        {
            Invoice invoice;
            PaymentListWithItemUpdate payListWithItemUpdate;

            if (LstMain.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select a record from Main List");
                return;
            }

            invoice = LstMain.SelectedItem as Invoice;
            payListWithItemUpdate = new PaymentListWithItemUpdate(invoice, this);
            
            payListWithItemUpdate.ShowDialog();
        }

        private void DoubleClick_ListView(object sender, MouseButtonEventArgs e)
        {
            OpenAccountForm(sender, e);
        }

        private void Click_ListView(object sender, MouseButtonEventArgs e)
        {
        }

        private void OpenAccountForm(object sender, RoutedEventArgs e)
        {

            InvoiceAccount invoiceAccount;
            PaymentListAddAccount paymentListAddAccount;
            Invoice invoice;

            #region InitInvoice

            if (LstMain.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select an invoice from Main List!");
                return;
            }

            invoice = LstMain.SelectedItem as Invoice;

            invoiceAccount = new InvoiceAccount()
            {
                InvoiceId = invoice.ID,
                PaymentListId = invoice.PaymentListId,
                CompanyName = invoice.CompanyName,
                CompanyAddress = invoice.CompanyAddress,
                InvNu = invoice.InvNu,
                InvDate = invoice.InvDate,
                PayAmount = invoice.PayAmount,
                PayCurr = invoice.PayCurr,

            };

            #endregion

            #region OpenAddAccountForm

            paymentListAddAccount = new PaymentListAddAccount(invoice, this, ref invoiceAccount);
            paymentListAddAccount.ShowDialog();
            
            #endregion

            #region UpdateListMainForm

            //isAccounted is effected in PaymentListAddAccount!
            //Bad code!!!!
            //Change it!
            if (!isAccountAdded)
            {
                return;
            }

            #region RemoveInvoiceFromMainList

            notPaidInvoices = from inv in notPaidInvoices
                              where inv != invoice
                              select inv;

            LstMain.ItemsSource = notPaidInvoices;

            #endregion

            isAccountAdded = false;

            invoiceAccountMain.AddItem(invoiceAccount);
            LstAddedAccount.ItemsSource = invoiceAccountMain;

            #endregion
        }

        private void BtnCash_Click(object sender, RoutedEventArgs e)
        {
            PaymentListMain paymentListMain;
            BookPayment bookPayment;
            BookPaymentMain bookPaymentMain;
            PaymentCashBook paymentCashBook;
            InvoiceAccount invoiceAccount;

            if (LstMain.Items.Count > 0)
            {
                _ = MessageBox.Show("All elemenst in the Main List must be passed to the Payment List!");
                return;
            }

            #region CashBookOpenAndTakeSavingResult

            paymentCashBook = new PaymentCashBook(this, invoiceAccountMain);
            _ = paymentCashBook.ShowDialog();

            if (!isCashBookSaved)
            {
                _ = MessageBox.Show("Saving is not successfull!");
                return;
            }
            #endregion

            #region CreatePaymentListforFiscalSaving

            paymentListMain = new PaymentListMain(isUpdate: true);
            string paymentListId = invoiceAccountMain.First().PaymentListId;

            #endregion

            #region PaymentListSaving

            if (string.IsNullOrEmpty(paymentListId))
            {
                _ = MessageBox.Show("Error:PaymentListWithItems:BtnApprove_Click:" + "paymentListId couldn't take!");
                return;
            }

            if (!paymentListMain.UpdateData("DisPaymentList", paymentListId, "PAID"))
            {
                return;
            }

            #region FiscalPaymentListSavePreparation

            //Update list date for Fiscal
            _paymentList.ListDate = DateTime.Parse(DateTime.Now.ToString("d"));

            if (!paymentListMain.FiscalSaveData(_paymentList))
            {
                return;
            }
            _ = MessageBox.Show("The Payment List has been sent to Fiscal!");

            #endregion

            #endregion

            #region BookPaymentSavingProcess

            invoiceAccount = LstAddedAccount.Items.GetItemAt(0) as InvoiceAccount;

            if (invoiceAccount is null)
            {
                _ = MessageBox.Show("Error:PaymentListWithItems.xaml.cs: Invoice Data couldn't get!");
                return;
            }

            bookPayment = new BookPayment()
            {
                Description = invoiceAccount.CompanyName + ":" + invoiceAccount.InvNu,
                PaymentBookDate = cashBookDate,
                BookCashID = bookCashLastId,
                DisPaymentListId = paymentListId,
            };

            bookPaymentMain = new BookPaymentMain();

            if(!bookPaymentMain.SaveData(bookPayment))
            {
                return;
            }
            #endregion

            Close();
        }

        private void DeleteFromPaymentList(object sender, RoutedEventArgs e)
        {
            if (LstMain.SelectedIndex == -1)
            {
                _ = MessageBox.Show("Please select an item!");
                return;
            }

            if (MessageBox.Show("Do you want to delete this item?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            Invoice invoice = LstMain.SelectedItem as Invoice;
            InvoiceMain invoiceMain = new InvoiceMain();

            if (!invoiceMain.DeleteData(invoice) || !invoiceMain.DeleteFromDisbursingApprove(invoice))
            {
                _ = MessageBox.Show("Data couldn't delete!");
                return;
            }

            _ = MessageBox.Show("The item is removed from Payment List");

            invoice = new Invoice();
            PaymentListMain paymentListMain = new PaymentListMain();
            PaymentList paymentList = paymentListMain.TakeNotPaidPaymentList(invoice);
            invoice.PaymentListId = paymentList is default(PaymentList) ? string.Empty : paymentList.ID;

            if (string.IsNullOrEmpty(invoice.PaymentListId))
            {
                _ = MessageBox.Show("PaymentListid couldn't be taken!");
            }

            invoiceMain = new InvoiceMain();
            invoiceMain.InitList();

            LstMain.ItemsSource = invoiceMain.Where(inv => inv.PaymentListId == invoice.PaymentListId);
        }

        private void PrinttoExcelOne(object sender, RoutedEventArgs e)
        {
            const string fileName = "03-Payment-01";
            List<Invoice> invoices = TakeInvoices();
            if (invoices is null)
            {
                _ = MessageBox.Show("Invoice list couldn't take!");
                return;
            }
            WriteReportFile(fileName, invoices);
        }

        private void PrinttoExcelTwo(object sender, RoutedEventArgs e)
        {
            const string fileName = "03-Payment-02";
            List<Invoice> invoices = TakeInvoices();

            if (invoices is null)
            {
                _ = MessageBox.Show("Invoice list couldn't take!");
                return;
            }
            WriteReportFile(fileName, invoices);
        }

        private List<Invoice> TakeInvoices()
        {
            List<Invoice> invoices = null;
            foreach (Invoice inv in LstMain.Items)
            {
                if (invoices is null)
                {
                    invoices = new List<Invoice>();
                }
                invoices.Add(inv);
            }

            return invoices;
        }

        private void WriteReportFile(string fileName, List<Invoice> invoices)
        {
            Reports.ReportFile reportFile;
            Reports.ReportExcel reportExcel;

            reportFile = new Reports.ReportFile().TakeFileData(fileName);
            if (reportFile is null)
            {
                _ = MessageBox.Show("File couldn't be found!");
                return;
            }

            reportExcel = new Reports.ReportExcel().OpenTemplateFile(reportFile);
            if (reportExcel is null)
            {
                _ = MessageBox.Show("Excel couldn't open the template!");
                return;
            }


            var currencyGroupedInvoices = invoices.GroupBy(inv => inv.PayCurr);
            int row = 0;
            int startRowNumber = 4;
            int endRowNumber = 200;
            

            foreach(var invgrp in currencyGroupedInvoices)
            {
                int itemNu = 1;
                decimal sumofCurrency = invgrp.Sum(inv => inv.PayAmount);

                foreach(var inv in invgrp)
                {
                    reportExcel.ReportSheet.Range["A4"].Offset[row, 0].Value = itemNu++;
                    reportExcel.ReportSheet.Range["B4"].Offset[row, 0].Value = inv.CompanyName;
                    reportExcel.ReportSheet.Range["C4"].Offset[row, 0].Value = inv.CompanyAddress;
                    reportExcel.ReportSheet.Range["D4"].Offset[row, 0].Value = inv.BankName;
                    reportExcel.ReportSheet.Range["E4"].Offset[row, 0].Value = inv.IBANNu;
                    reportExcel.ReportSheet.Range["F4"].Offset[row, 0].Value = inv.InvNu;
                    reportExcel.ReportSheet.Range["G4"].Offset[row, 0].NumberFormat = "#,#0.00 \"" + inv.PayCurr + "\"";
                    reportExcel.ReportSheet.Range["G4"].Offset[row, 0].Value = inv.PayAmount;
                    row++;
                }

                reportExcel.ReportSheet.Range["G4"].Offset[row, 0].NumberFormat = "#,#0.00 \"" + invgrp.Key + "\"";
                reportExcel.ReportSheet.Range["G4"].Offset[row, 0].Value = sumofCurrency;
                reportExcel.ReportSheet.Range["A4"].Offset[row, 0].Value = "TOPLAM" + invgrp.Key + "(TOTAL " + invgrp.Key + ")";

                reportExcel.ReportSheet.Application.DisplayAlerts = false;
                string mergeRow = "A" + (row + startRowNumber).ToString() + ":" + "F" + (row + startRowNumber).ToString();
                reportExcel.ReportSheet.Range[mergeRow].Merge();
                reportExcel.ReportSheet.Range[mergeRow].Font.Bold = true;
                reportExcel.ReportSheet.Range[mergeRow].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                row++;
            }

            string deleteRow = "A" + (row + startRowNumber).ToString() + ":" + "G" + endRowNumber;
            reportExcel.ReportSheet.Range[deleteRow].Delete(Excel.XlDeleteShiftDirection.xlShiftUp);
            reportExcel.ReportSheet.Range["A4"].Select();
            reportExcel.ReportSheet.Application.DisplayAlerts = true;
        }
    }
}
