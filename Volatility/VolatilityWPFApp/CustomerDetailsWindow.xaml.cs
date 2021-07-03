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
using VolatilityContracts;
using VolatilityWPFApp.ViewModels;

namespace VolatilityWPFApp
{
    /// <summary>
    /// Interaction logic for CustomerDetailsWindow.xaml
    /// </summary>
    public partial class CustomerDetailsWindow : Window
    {
        public List<TitleView> Titles { get; set; } = new List<TitleView>();
        public CustomerDetailsView CustomerDetailsView { get; set; } = new CustomerDetailsView();

        internal Action<CustomerDetails> SetRecordDel = null;
        public CustomerDetailsWindow()
        {
            InitializeComponent();
            this.Title = "Customer Details";
            this.DataContext = CustomerDetailsView;

        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            SetRecordDel(CustomerDetailsView.CustomerDetails);
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtContactNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int v;
            e.Handled = !int.TryParse(e.Text, out v);
            
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            SetRecordDel = null;
        }

        
    }
}
