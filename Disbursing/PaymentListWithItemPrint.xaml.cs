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

namespace G8FinApp.Disbursing
{
    /// <summary>
    /// Interaction logic for PaymentListWithItemPrint.xaml
    /// </summary>
    public partial class PaymentListWithItemPrint : Window
    {
        private IEnumerable<Invoice> _invoiceMain;

        public PaymentListWithItemPrint(IEnumerable<Invoice> invoiceMain)
        {
            InitializeComponent();
            _invoiceMain = invoiceMain;
            FirstRow();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog pd = new PrintDialog();

            if (pd.ShowDialog() == true)
            {
                pd.PrintVisual(FixPageMain, "Fixed Page Report");
            }
        }

        private void FirstRow()
        {
            int rwNum = 4;

            foreach(Invoice inv in _invoiceMain)
            {
                #region ID
                TextBlock textBlock = new TextBlock()
                {
                    Text = inv.ID, //Field Name
                    FontSize = 9,
                    FontFamily = new FontFamily("Arial"),
                    TextAlignment = TextAlignment.Center,
                };

                textBlock.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);

                Border rowBorder = new Border()
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0.25),
                };

                rowBorder.Child = textBlock;
                rowBorder.SetValue(Grid.ColumnProperty, 0);
                rowBorder.SetValue(Grid.RowProperty, rwNum); //Row number
                rowBorder.SetValue(VerticalAlignmentProperty, VerticalAlignment.Stretch);

                MainGrid.Children.Add(rowBorder);
                #endregion

                #region CompanyName
                textBlock = new TextBlock()
                {
                    Text = inv.CompanyName, //Field Name
                    FontSize = 9,
                    FontFamily = new FontFamily("Arial"),
                    TextWrapping = TextWrapping.Wrap,
                };

                textBlock.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
                textBlock.SetValue(MarginProperty, new Thickness(5,0,0,0));

                rowBorder = new Border()
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0.25),
                };

                rowBorder.Child = textBlock;
                rowBorder.SetValue(Grid.ColumnProperty, 1);
                rowBorder.SetValue(Grid.ColumnSpanProperty, 2);
                rowBorder.SetValue(Grid.RowProperty, rwNum); //Row number
                rowBorder.SetValue(VerticalAlignmentProperty, VerticalAlignment.Stretch);

                MainGrid.Children.Add(rowBorder);
                #endregion

                #region CompanyAddress
                textBlock = new TextBlock()
                {
                    Text = inv.CompanyAddress, //Field Name
                    FontSize = 9,
                    FontFamily = new FontFamily("Arial"),
                    TextWrapping = TextWrapping.Wrap,
                };

                textBlock.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
                textBlock.SetValue(MarginProperty, new Thickness(5, 0, 0, 0));

                rowBorder = new Border()
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0.25),
                };

                rowBorder.Child = textBlock;
                rowBorder.SetValue(Grid.ColumnProperty, 3);
                rowBorder.SetValue(Grid.ColumnSpanProperty, 3);
                rowBorder.SetValue(Grid.RowProperty, rwNum); //Row number
                rowBorder.SetValue(VerticalAlignmentProperty, VerticalAlignment.Stretch);

                MainGrid.Children.Add(rowBorder);
                #endregion

                #region BankName
                textBlock = new TextBlock()
                {
                    Text = inv.BankName, //Field Name
                    FontSize = 9,
                    FontFamily = new FontFamily("Arial"),
                    TextWrapping = TextWrapping.Wrap,
                };

                textBlock.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
                textBlock.SetValue(MarginProperty, new Thickness(5, 0, 0, 0));

                rowBorder = new Border()
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0.25),
                };

                rowBorder.Child = textBlock;
                rowBorder.SetValue(Grid.ColumnProperty, 6);
                rowBorder.SetValue(Grid.ColumnSpanProperty, 2);
                rowBorder.SetValue(Grid.RowProperty, rwNum); //Row number
                rowBorder.SetValue(VerticalAlignmentProperty, VerticalAlignment.Stretch);

                MainGrid.Children.Add(rowBorder);
                #endregion

                #region IBANNu
                textBlock = new TextBlock()
                {
                    Text = inv.IBANNu, //Field Name
                    FontSize = 9,
                    FontFamily = new FontFamily("Arial"),
                    TextWrapping = TextWrapping.Wrap,
                };

                textBlock.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
                textBlock.SetValue(MarginProperty, new Thickness(5, 0, 0, 0));

                rowBorder = new Border()
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0.25),
                };

                rowBorder.Child = textBlock;
                rowBorder.SetValue(Grid.ColumnProperty, 8);
                rowBorder.SetValue(Grid.ColumnSpanProperty, 3);
                rowBorder.SetValue(Grid.RowProperty, rwNum); //Row number
                rowBorder.SetValue(VerticalAlignmentProperty, VerticalAlignment.Stretch);

                MainGrid.Children.Add(rowBorder);
                #endregion

                #region InvNu
                textBlock = new TextBlock()
                {
                    Text = inv.InvNu, //Field Name
                    FontSize = 9,
                    FontFamily = new FontFamily("Arial"),
                    TextWrapping = TextWrapping.Wrap,
                };

                textBlock.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
                textBlock.SetValue(MarginProperty, new Thickness(5, 0, 0, 0));

                rowBorder = new Border()
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0.25),
                };

                rowBorder.Child = textBlock;
                rowBorder.SetValue(Grid.ColumnProperty, 11);
                rowBorder.SetValue(Grid.ColumnSpanProperty, 2);
                rowBorder.SetValue(Grid.RowProperty, rwNum); //Row number
                rowBorder.SetValue(VerticalAlignmentProperty, VerticalAlignment.Stretch);

                MainGrid.Children.Add(rowBorder);
                #endregion

                #region TotalAmount
                textBlock = new TextBlock()
                {
                    Text = inv.PayAmount.ToString(), //Field Name
                    FontSize = 9,
                    FontFamily = new FontFamily("Arial"),
                    TextWrapping = TextWrapping.Wrap,
                };

                textBlock.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
                textBlock.SetValue(MarginProperty, new Thickness(5, 0, 0, 0));

                rowBorder = new Border()
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0.25),
                };

                rowBorder.Child = textBlock;
                rowBorder.SetValue(Grid.ColumnProperty, 13);
                rowBorder.SetValue(Grid.ColumnSpanProperty, 3);
                rowBorder.SetValue(Grid.RowProperty, rwNum); //Row number
                rowBorder.SetValue(VerticalAlignmentProperty, VerticalAlignment.Stretch);

                _ = MainGrid.Children.Add(rowBorder);

                rwNum++;

                #endregion
            }
        }
    }
}
