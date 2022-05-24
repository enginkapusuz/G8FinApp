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
    /// Interaction logic for FiscalPendingsList.xaml
    /// </summary>
    public partial class FiscalPendingsList : Window
    {
        public FiscalPendingsList()
        {
            InitializeComponent();
            LoadLstMain();
        }

        private void LoadLstMain()
        {
            PendingMain pendingMain = new PendingMain();
            pendingMain.InitList();
            LstMain.ItemsSource = pendingMain;
        }
    }
}
