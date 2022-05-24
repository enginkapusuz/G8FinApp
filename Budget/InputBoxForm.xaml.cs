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

namespace G8FinApp.Budget
{
    /// <summary>
    /// Interaction logic for InputBoxForm.xaml
    /// </summary>
    public partial class InputBoxForm : Window
    {
        public Budget newBudget { get; private set; }
        public InputBoxForm(CISICodeMain cisiCodeMain, Budget _budget)
        {
            
            InitializeComponent();
            Budget budget = LoadTxtCisiCodes(cisiCodeMain, _budget);
            if (budget is null)
            {
                _ = MessageBox.Show("Cisi Codes couldn't taken!");
                Close();
            }

            newBudget = budget;
        }

        private Budget LoadTxtCisiCodes(CISICodeMain cisiCodeMain, Budget _budget)
        {
            Budget budget = null;
            
            var cisiCodeMainFiltered = from cisi in cisiCodeMain
                                       where cisi.CISICODE != _budget.CISICODE
                                       select cisi;

            foreach (var cisi in cisiCodeMainFiltered)
            {
                TxtCisiCodes.Items.Add(cisi.CISICODE);
                if (budget is null)
                {
                    budget = _budget;
                }
            }
            return budget;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (TxtCisiCodes.SelectedIndex == -1)
            {
                newBudget = null;
                Close();
                return;
            }

            newBudget.CISICODE = TxtCisiCodes.Text;
            Close();
        }
    }
}
