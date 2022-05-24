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

namespace G8FinApp.Purchasing
{
    /// <summary>
    /// Interaction logic for PurchasingItems.xaml
    /// </summary>
    public partial class PurchasingItems : Window
    {
        Bidding bidding;
        public ItemsMain itemsMain = new ItemsMain();
        public PurchasingItems(Bidding _bidding)
        {
            InitializeComponent();
            bidding = _bidding;
            TxtBiddingName.Text = bidding.BiddingName;
            TxtBiddingPrice.Text = bidding.BiddingPrice.ToString("#,#.00");
            itemsMain.OpenInitData(bidding.ID);
            LstMain.ItemsSource = itemsMain;
            TxtBlckTotalAmount.Text = itemsMain.Select(itm => itm.TotalAmount).Sum().ToString("#,#.0000");
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            Item item;
            string brand = string.Empty;

            if (string.IsNullOrEmpty(TxtItemNu.Text))
            {
                _ = MessageBox.Show("ItemNu is empty!");
                return;
            }

            if (string.IsNullOrEmpty(TxtDescription.Text))
            {
                _ = MessageBox.Show("Description is empty!");
                return;
            }

            if (!string.IsNullOrEmpty(TxtBrand.Text))
            {
                brand = TxtBrand.Text.Trim();
            }

            if (string.IsNullOrEmpty(TxtQuantity.Text))
            {
                _ = MessageBox.Show("Quantity is empty!");
                return;
            }

            if (string.IsNullOrEmpty(TxtUnit.Text))
            {
                _ = MessageBox.Show("Unit is empty!");
                return;
            }

            if (string.IsNullOrEmpty(TxtUnitPrice.Text))
            {
                _ = MessageBox.Show("UnitPrice is empty!");
                return;
            }

            if (!(int.TryParse(TxtItemNu.Text, out int intItemNu) && intItemNu > 0))
            {
                _ = MessageBox.Show("ItemNu is not proper!");
                return;
            }

            if (!(float.TryParse(TxtQuantity.Text, out float fltQuantity) && fltQuantity > 0))
            {
                _ = MessageBox.Show("Quantity is not proper format!");
                return;
            }

            if (!(decimal.TryParse(TxtUnitPrice.Text, out decimal dcmlUnitPrice) && dcmlUnitPrice > 0))
            {
                _ = MessageBox.Show("UnitPrice is not proper format!");
                return;
            }

            item = new Item()
            {
                ItemNu = TxtItemNu.Text,
                BiddingId = bidding.ID,
                Description = TxtDescription.Text.Trim().ToUpper(),
                Brand = brand,
                Quantity = fltQuantity,
                Unit = TxtUnit.Text.Trim().Trim(),
                UnitPrice = dcmlUnitPrice,
                TotalAmount = (decimal)fltQuantity * dcmlUnitPrice,
            };

            if (bidding.BiddingPrice < itemsMain.Select(itm => itm.TotalAmount).Sum() + item.TotalAmount)
            {
                _ = MessageBox.Show("Total amount of Items is greater than Bidding Amount!");
                return;
            }

            if (int.Parse(item.ItemNu) == itemsMain.Count + 1)
            {
                itemsMain.Add(item);
            }
            else
            {
                itemsMain.Insert(int.Parse(item.ItemNu) - 1, item);
                itemsMain = OrderItemsMain(itemsMain);
            }

            LstMain.ItemsSource = itemsMain;
            TxtItemNu.Text = (LstMain.Items.Count + 1).ToString();
            TxtBlckTotalAmount.Text = itemsMain.Select(itm => itm.TotalAmount).Sum().ToString("#,#.00");
        }

        private void BtnSaveList_Click(object sender, RoutedEventArgs e)
        {
            if (!itemsMain.DeleteData(bidding.ID))
            {
                return;
            }

            if (itemsMain.Count == 0)
            {
                _ = MessageBox.Show("Data is saved");
                return;
            }

            if (!itemsMain.SaveData())
            {
                _ = MessageBox.Show("Data couldn't save!");
                return;
            }

            _ = MessageBox.Show("Data is saved");

            Close();
        }

        private void LstMain_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Delete)
            {
                if (LstMain.SelectedIndex == -1)
                {
                    return;
                }

                int indxOfItm = LstMain.SelectedIndex;
                itemsMain.RemoveAt(indxOfItm);
                itemsMain = OrderItemsMain(itemsMain);
                LstMain.ItemsSource = itemsMain;
                TxtBlckTotalAmount.Text = itemsMain.Select(itm => itm.TotalAmount).Sum().ToString("#,#.00");
                e.Handled = true;
            }
        }

        private ItemsMain OrderItemsMain(ItemsMain itemsMain)
        {
            int indx = 1;
            ItemsMain newItemMain = new ItemsMain();
            foreach (Item itm in itemsMain)
            {
                itm.ItemNu = indx++.ToString();
                newItemMain.Add(itm);
            }

            return newItemMain;
        }

        private void TxtUnitPrice_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TxtUnitPrice.Text) && decimal.TryParse(TxtUnitPrice.Text, out decimal dcmlUnitPrice))
            {
                TxtUnitPrice.Text = dcmlUnitPrice.ToString("#,#.00");
            }
        }
    }
}
