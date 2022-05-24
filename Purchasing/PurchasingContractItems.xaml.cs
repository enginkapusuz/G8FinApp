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
    /// Interaction logic for PurchasingContractItems.xaml
    /// </summary>
    public partial class PurchasingContractItems : Window
    {
        Contract contract = null;
        ContractItemsMain contractItemsMain = new ContractItemsMain();
        Item item;
        public PurchasingContractItems(Contract _contract)
        {
            InitializeComponent();

            contract = _contract;
            TxtBiddingName.Text = contract.BiddingName;
            TxtPcAmount.Text = contract.PcAmount.ToString("#,#.00");
            
            contractItemsMain.OpenInitData(contract.BiddingId);
            
            decimal totalAmount = contractItemsMain.Select(itm => itm.TotalAmount).Sum();
            TxtBlckTotalAmount.Text = totalAmount.ToString("#,#.00");

            if (!(contractItemsMain.Count == 0))
            {
                LstMain.ItemsSource = contractItemsMain;
                return;
            }

            contractItemsMain.Clear();
            contractItemsMain.OpenSecondInitData(contract.BiddingId);

            totalAmount = contractItemsMain.Select(itm => itm.TotalAmount).Sum();
            TxtBlckTotalAmount.Text = totalAmount.ToString("#,#.00");

            LstMain.ItemsSource = contractItemsMain;
        }

        private void LstMain_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            
            if (LstMain.SelectedIndex == -1)
            {
                return;
            }

            item = LstMain.SelectedItem as Item;
            TxtItemNu.Text = item.ItemNu;
            TxtDescription.Text = item.Description;
            TxtQuantity.Text = item.Quantity.ToString("#,#.00");
            TxtUnit.Text = item.Unit;
            TxtUnitPrice.Text = item.UnitPrice.ToString("#,#.00");
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
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

            Item newItem = new Item()
            {
                BiddingId = contract.BiddingId,
                ItemNu = TxtItemNu.Text.Trim(),
                Description = TxtDescription.Text.Trim(),
                Quantity = fltQuantity,
                Unit = TxtUnit.Text.Trim(),
                UnitPrice = dcmlUnitPrice,
                TotalAmount = (decimal)fltQuantity * dcmlUnitPrice,
            };
            if (!(item is null) && contractItemsMain.Select(itm => itm.TotalAmount).Sum() - item.TotalAmount + newItem.TotalAmount > contract.PcAmount)
            {
                _ = MessageBox.Show("Total amount is greater than P&C Amount!");
                return;
            }
            else if (contractItemsMain.Select(itm => itm.TotalAmount).Sum() + newItem.TotalAmount > contract.PcAmount)
            {
                _ = MessageBox.Show("Total amount is greater than P&C Amount!");
                return;
            }
            if (!(item is null))
            {
                int indx = contractItemsMain.IndexOf(item);

                if (indx >= 0)
                {
                    contractItemsMain.RemoveAt(indx);
                }
                else
                {
                    indx = 0;
                }
                
                contractItemsMain.Insert(indx, newItem);
                LstMain.ItemsSource = OrderItemsMain(contractItemsMain);
                TxtBlckTotalAmount.Text = contractItemsMain.Select(itm => itm.TotalAmount).Sum().ToString("#,#.0000");
                item = null;
                return;
            }

            contractItemsMain.Add(newItem);
            LstMain.ItemsSource = OrderItemsMain(contractItemsMain);
            TxtBlckTotalAmount.Text = contractItemsMain.Select(itm => itm.TotalAmount).Sum().ToString("#,#.0000");
        }
        private ContractItemsMain OrderItemsMain(ContractItemsMain itemsMain)
        {
            int indx = 1;
            ContractItemsMain newItemMain = new ContractItemsMain();
            foreach (Item itm in itemsMain)
            {
                itm.ItemNu = indx++.ToString();
                newItemMain.Add(itm);
            }

            return newItemMain;
        }

        private void LstMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (LstMain.SelectedIndex == -1)
                {
                    return;
                }

                int indxOfItm = LstMain.SelectedIndex;
                contractItemsMain.RemoveAt(indxOfItm);
                contractItemsMain = OrderItemsMain(contractItemsMain);
                LstMain.ItemsSource = contractItemsMain;
                TxtBlckTotalAmount.Text = contractItemsMain.Select(itm => itm.TotalAmount).Sum().ToString("#,#.00");
                e.Handled = true;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!contractItemsMain.DeleteData(contract.BiddingId))
            {
                return;
            }

            if (contractItemsMain.Count == 0)
            {
                _ = MessageBox.Show("Data is saved");
                return;
            }

            if (!contractItemsMain.SaveData())
            {
                _ = MessageBox.Show("Data couldn't save!");
                return;
            }

            _ = MessageBox.Show("Data is saved");
        }
    }
}
