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
using System.Windows.Markup;
using System.Printing;
using System.Windows.Xps;

namespace G8FinApp.Disbursing
{
    /// <summary>
    /// Interaction logic for PaymentItemsPrint.xaml
    /// </summary>
    public partial class PaymentItemsPrint : Window
    {
        private IEnumerable<Invoice> _notPaidInvoices;

        public PaymentItemsPrint(IEnumerable<Invoice> nonPaidInvoices)
        {
            InitializeComponent();
            _notPaidInvoices = nonPaidInvoices;
            MakeCurrecyGroup();
        }

        private void MakeCurrecyGroup()
        {
            Canvas lastCanvas = null;

            int lineSpace = 0;
            int cnvTop = 0;

            var currencyGroup = from inv in _notPaidInvoices
                                group inv by inv.PayCurr into g
                                //select new { Currency = g.Key, Currencies = g };
                                select (Currency: g.Key, Currencies: g);

            foreach (var g in currencyGroup)
            {
                IEnumerable<Invoice> invList = g.Currencies.Select(inv => inv);
                lastCanvas = WriteRows(invList, ref lineSpace, ref cnvTop);
            }

            if (lastCanvas is null)
            {
                return;
            }

            WriteSignatures(lastCanvas, cnvTop + 300, lineSpace);
        }

        private Border CreateRefAndDateBorder(string cellValue, int cellWidth, int lineSpace, bool isDate = false)
        {
            TextBlock txtBlock;
            Border cellBorder;

            txtBlock = new TextBlock()
            {
                Text = cellValue,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,

                FontFamily = new FontFamily("Arial"),
                FontWeight = FontWeights.Bold,
                FontSize = 11,
            };

            txtBlock.TextAlignment = isDate ? TextAlignment.Right : TextAlignment.Left;

            cellBorder = new Border()
            {
                Child = txtBlock,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Center,
                Width = cellWidth,
                Height = lineSpace,
            };
            return cellBorder;
        }

        private Border CreateCellBorder(string cellValue, int cellWidth, int lineSpace, bool isHeader = false)
        {
            TextBlock txtBlock;
            Border cellBorder;

            txtBlock = new TextBlock()
            {
                Text = cellValue,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,

                FontFamily = new FontFamily("Arial"),
                FontSize = 10,
            };

            if (isHeader)
            {
                txtBlock.FontWeight = FontWeights.Bold;
                txtBlock.FontSize = 11;
            }

            cellBorder = new Border()
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(0.25),
                Child = txtBlock,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Center,
                Width = cellWidth,
                Height = lineSpace,
            };

            return cellBorder;
        }

        private Border CreateSignatureBorder(string cellValue, int cellWidth, int lineSpace)
        {
            TextBlock txtBlock;
            Border cellBorder;

            txtBlock = new TextBlock()
            {
                Text = cellValue,
                TextAlignment = TextAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,

                FontFamily = new FontFamily("Arial"),
                FontSize = 12,
                FontWeight = FontWeights.Bold,
            };

            cellBorder = new Border()
            {
                Child = txtBlock,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Center,
                Width = cellWidth,
                Height = lineSpace,
            };

            return cellBorder;
        }

        private void WriteSignatures(Canvas canvas, int canvTop, int lineSpace)
        {
            #region Signatures

            List<string> names = new List<string>()
            {
                "Selçuk GÜNDAĞ",
                "Onur YILDIRIM",
            };

            List<string> ranks = new List<string>()
            {
                "1st LT TUR A",
                "COL TUR A",
            };

            List<string> titles = new List<string>()
            {
                "DISBURSING OFF.",
                "FINCON",
            };

            List<List<string>> signatureList = new List<List<string>>()
            {
                names,
                ranks,
                titles
            };

            Border disname;

            List<int> signWidths = new List<int>()
            {
                550,
                200,
            };

            StackPanel rowStackPanel = new StackPanel();
            Border rowBorder = new Border();

            foreach (var lst in signatureList)
            {
                int cntr = 0;

                rowStackPanel = new StackPanel();
                rowStackPanel.Orientation = Orientation.Horizontal;

                foreach (var lstVal in lst)
                {
                    disname = CreateSignatureBorder(lstVal, signWidths[cntr], lineSpace);
                    rowStackPanel.Children.Add(disname);
                    cntr++;
                }

                rowBorder = new Border();
                rowBorder.Child = rowStackPanel;

                Canvas.SetLeft(rowBorder, 70);
                Canvas.SetTop(rowBorder, canvTop);
                canvas.Children.Add(rowBorder);

                canvTop += 20;
            }

            #endregion

        }

        private Canvas WriteRows(IEnumerable<Invoice> invList, ref int _cnvTop, ref int _lineSpace)
        {
            PageContent firstPage;
            FixedPage fixedPage;
            Canvas canvas = new Canvas();

            Border cellBorder;

            #region PageMeasurements

            int orderCounter = 1;

            int pageRowCount = 13;
            int lineSpace = 70;

            int canvTop = 70;
            int rowCounter = 0;

            int leftSpaceId = 70;
            int widthId = 50;

            int leftSpaceCompanyName = leftSpaceId + widthId;
            int widthCompanyName = 100;

            int leftSpaceCompanyAddress = leftSpaceCompanyName + widthCompanyName;
            int widthCompanyAddress = 100;

            int leftSpaceBankName = leftSpaceCompanyAddress + widthCompanyAddress;
            int widthBankName = 100;

            int leftSpaceIBANNo = leftSpaceBankName + widthBankName;
            int widthIBANNO = 150;


            int leftInvNumber = leftSpaceIBANNo + widthIBANNO;
            int widthInvNumber = 100;

            int leftPayAmount = leftInvNumber + widthInvNumber;
            int widthPayAmount = 100;


            int leftSpaceTotal = leftSpaceId;
            int widthCurrencyName = widthId + widthCompanyName + widthCompanyAddress + widthBankName + widthIBANNO + widthInvNumber;
            int widthCurrencyTotal = widthPayAmount;

            int leftListHeader = leftSpaceId;
            int widhtListHeader = widthCurrencyName + widthCurrencyTotal;

            #endregion


            #region ReferenceNumberAndDate

            Border cellRefNumber = CreateRefAndDateBorder(cellValue: "5330/TUCBUD/" + DateTime.Now.ToString("yy") + "/", cellWidth: widthCurrencyName, lineSpace: lineSpace, isDate: false);
            Border cellDate = CreateRefAndDateBorder(cellValue: DateTime.Now.ToString("dd MMMM yyyy"), cellWidth: widthCurrencyTotal, lineSpace: lineSpace, isDate: true);

            StackPanel rowStackPanel = new StackPanel();
            rowStackPanel.Orientation = Orientation.Horizontal;
            rowStackPanel.Children.Add(cellRefNumber);
            rowStackPanel.Children.Add(cellDate);

            Border rowBorder = new Border();
            rowBorder.Child = rowStackPanel;

            Canvas.SetLeft(rowBorder, leftSpaceTotal);
            Canvas.SetTop(rowBorder, canvTop);
            canvas.Children.Add(rowBorder);

            #endregion

            rowCounter += 1;
            canvTop += lineSpace;

            #region ListHeader

            TextBlock txtBlockListHeader = new TextBlock()
            {
                Text = "PAYMENT LIST / ÖDEME LİSTESİ",
                FontFamily = new FontFamily("Arial"),
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Width = widhtListHeader,
                TextAlignment = TextAlignment.Center,

            };

            Canvas.SetLeft(txtBlockListHeader, leftListHeader);
            Canvas.SetTop(txtBlockListHeader, canvTop);
            canvas.Children.Add(txtBlockListHeader);

            #endregion

            canvTop += (int)(lineSpace / 2);

            #region TurkishFieldNames

            Border cellBorderId = CreateCellBorder(cellValue: "Sıra Nu", cellWidth: widthId, lineSpace: lineSpace, isHeader: true);
            Border cellBorderCompanyName = CreateCellBorder(cellValue: "Şirket Adı", cellWidth: widthCompanyName, lineSpace: lineSpace, isHeader: true);
            Border cellBorderCompanyAddress = CreateCellBorder(cellValue: "Şirket Adresi", cellWidth: widthCompanyAddress, lineSpace: lineSpace, isHeader: true);
            Border cellBorderBankName = CreateCellBorder(cellValue: "Banka Adı", cellWidth: widthBankName, lineSpace: lineSpace, isHeader: true);
            Border cellBorderIBANNo = CreateCellBorder(cellValue: "Hesap Nu/IBAN Nu", cellWidth: widthIBANNO, lineSpace: lineSpace, isHeader: true);
            Border cellBorderInvNu = CreateCellBorder(cellValue: "Fatura Nu", cellWidth: widthInvNumber, lineSpace: lineSpace, isHeader: true);
            Border cellBorderPayAmount = CreateCellBorder(cellValue: "Toplam Tutar", cellWidth: widthPayAmount, lineSpace: lineSpace, isHeader: true);


            rowBorder = new Border();
            rowStackPanel = new StackPanel();

            rowStackPanel.Orientation = Orientation.Horizontal;
            rowStackPanel.Children.Add(cellBorderId);
            rowStackPanel.Children.Add(cellBorderCompanyName);

            rowStackPanel.Children.Add(cellBorderCompanyAddress);
            rowStackPanel.Children.Add(cellBorderBankName);
            rowStackPanel.Children.Add(cellBorderIBANNo);

            rowStackPanel.Children.Add(cellBorderInvNu);
            rowStackPanel.Children.Add(cellBorderPayAmount);

            rowBorder.Child = rowStackPanel;
            canvas.Children.Add(rowBorder);

            Canvas.SetLeft(rowBorder, leftSpaceId);
            Canvas.SetTop(rowBorder, canvTop);

            #endregion

            canvTop += lineSpace;
            rowCounter += 1;

            #region EnglishFieldNames

            cellBorderId = CreateCellBorder(cellValue: "Item Nu", cellWidth: widthId, lineSpace: lineSpace, isHeader:true);
            cellBorderCompanyName = CreateCellBorder(cellValue: "Company Name", cellWidth: widthCompanyName, lineSpace: lineSpace, isHeader: true);
            cellBorderCompanyAddress = CreateCellBorder(cellValue: "Company Address", cellWidth: widthCompanyAddress, lineSpace: lineSpace, isHeader: true);
            cellBorderBankName = CreateCellBorder(cellValue: "Bank Name", cellWidth: widthBankName, lineSpace: lineSpace, isHeader: true);
            cellBorderIBANNo = CreateCellBorder(cellValue: "IBAN Nu", cellWidth: widthIBANNO, lineSpace: lineSpace, isHeader: true);
            cellBorderInvNu = CreateCellBorder(cellValue: "Invoice Nu", cellWidth: widthInvNumber, lineSpace: lineSpace, isHeader: true);
            cellBorderPayAmount = CreateCellBorder(cellValue: "Total Amount", cellWidth: widthPayAmount, lineSpace: lineSpace, isHeader: true);


            rowBorder = new Border();
            rowStackPanel = new StackPanel();

            rowStackPanel.Orientation = Orientation.Horizontal;
            rowStackPanel.Children.Add(cellBorderId);
            rowStackPanel.Children.Add(cellBorderCompanyName);

            rowStackPanel.Children.Add(cellBorderCompanyAddress);
            rowStackPanel.Children.Add(cellBorderBankName);
            rowStackPanel.Children.Add(cellBorderIBANNo);

            rowStackPanel.Children.Add(cellBorderInvNu);
            rowStackPanel.Children.Add(cellBorderPayAmount);

            rowBorder.Child = rowStackPanel;
            canvas.Children.Add(rowBorder);

            Canvas.SetLeft(rowBorder, leftSpaceId);
            Canvas.SetTop(rowBorder, canvTop);

            #endregion

            rowCounter += 1;

            firstPage = new PageContent();
            fixedPage = new FixedPage();

            fixedPage.Children.Add(canvas);
            ((IAddChild)firstPage).AddChild(fixedPage);
            FixDoc.Pages.Add(firstPage);

            canvTop += lineSpace;

            foreach (Invoice inv in invList)
            {
                #region MaximumRowCount

                if (rowCounter == pageRowCount)
                {
                    canvTop = 70;

                    firstPage = new PageContent();
                    fixedPage = new FixedPage();

                    canvas = new Canvas()
                    {
                        Width = FixDoc.DocumentPaginator.PageSize.Width,
                        Height = FixDoc.DocumentPaginator.PageSize.Height,
                    };

                    fixedPage.Children.Add(canvas);
                    ((IAddChild)firstPage).AddChild(fixedPage);
                    FixDoc.Pages.Add(firstPage);
                }

                rowCounter = rowCounter == pageRowCount ? 0 : rowCounter;
                rowCounter += 1;

                #endregion

                #region Fields

                #region ID

                cellBorder = CreateCellBorder(cellValue: orderCounter.ToString(), cellWidth: widthId, lineSpace: lineSpace);
                Canvas.SetLeft(cellBorder, leftSpaceId);
                canvas.Children.Add(cellBorder);
                Canvas.SetTop(cellBorder, canvTop);
                orderCounter++;

                #endregion


                #region CompanyName

                cellBorder = CreateCellBorder(cellValue: inv.CompanyName, cellWidth: widthCompanyName, lineSpace: lineSpace);
                Canvas.SetLeft(cellBorder, leftSpaceCompanyName);
                canvas.Children.Add(cellBorder);
                Canvas.SetTop(cellBorder, canvTop);

                #endregion


                #region CompanyAddress

                cellBorder = CreateCellBorder(cellValue: inv.CompanyAddress, cellWidth: widthCompanyAddress, lineSpace: lineSpace);
                Canvas.SetLeft(cellBorder, leftSpaceCompanyAddress);
                canvas.Children.Add(cellBorder);
                Canvas.SetTop(cellBorder, canvTop);

                #endregion

                #region BankName

                cellBorder = CreateCellBorder(cellValue: inv.BankName, cellWidth: widthBankName, lineSpace: lineSpace);
                Canvas.SetLeft(cellBorder, leftSpaceBankName);
                canvas.Children.Add(cellBorder);
                Canvas.SetTop(cellBorder, canvTop);

                #endregion

                #region IBANNo

                cellBorder = CreateCellBorder(cellValue: inv.IBANNu, cellWidth: widthIBANNO, lineSpace: lineSpace);
                Canvas.SetLeft(cellBorder, leftSpaceIBANNo);
                canvas.Children.Add(cellBorder);
                Canvas.SetTop(cellBorder, canvTop);

                #endregion

                #region InvoiceNu

                cellBorder = CreateCellBorder(cellValue: inv.InvNu, cellWidth: widthInvNumber, lineSpace: lineSpace);
                Canvas.SetLeft(cellBorder, leftInvNumber);
                canvas.Children.Add(cellBorder);
                Canvas.SetTop(cellBorder, canvTop);

                #endregion

                #region PayAmount

                cellBorder = CreateCellBorder(cellValue: inv.PayAmount.ToString("#,#.00") + " " + inv.PayCurr, cellWidth: widthPayAmount, lineSpace: lineSpace);
                Canvas.SetLeft(cellBorder, leftPayAmount);
                canvas.Children.Add(cellBorder);
                Canvas.SetTop(cellBorder, canvTop);

                #endregion

                #endregion

                canvTop += lineSpace;
            }


            #region CurrencyTotal

            string currencyName = string.Empty;
            var currencyNameList = invList.Select(inv => inv.PayCurr).ToList();
            decimal currencyTotal = invList.Sum(inv => inv.PayAmount);

            currencyName = currencyNameList[0] ?? currencyName;

            //foreach (var cur in currencyNameList)
            //{
            //    currencyName = cur;
            //    break;
            //}

            if (string.IsNullOrEmpty(currencyName))
            {
                return canvas;
            }

            Border cellCurrencyName = CreateCellBorder(cellValue: "Total:" + currencyName, cellWidth: widthCurrencyName, lineSpace: lineSpace);
            Border cellTotalBorder = CreateCellBorder(cellValue: currencyTotal.ToString("#,#.00"), cellWidth: widthCurrencyTotal, lineSpace: lineSpace);

            rowStackPanel = new StackPanel();
            rowStackPanel.Orientation = Orientation.Horizontal;
            rowStackPanel.Children.Add(cellCurrencyName);
            rowStackPanel.Children.Add(cellTotalBorder);

            rowBorder = new Border();
            rowBorder.Child = rowStackPanel;

            Canvas.SetLeft(rowBorder, leftSpaceTotal);
            Canvas.SetTop(rowBorder, canvTop);
            canvas.Children.Add(rowBorder);

            #endregion

            canvTop += lineSpace;
            canvTop += lineSpace;
            canvTop += lineSpace;

            _cnvTop = canvTop;
            _lineSpace = lineSpace;

            return canvas;

            //PrintQueue queue = pd.PrintQueue;
            //XpsDocumentWriter docWriter = PrintQueue.CreateXpsDocumentWriter(queue);
            //docWriter.Write(FixDoc.DocumentPaginator);

        }
    }
}
