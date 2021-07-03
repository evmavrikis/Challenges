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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VolatilityContracts;
using System.Collections.ObjectModel;
using VolatilityWPFApp.Extensions;

namespace VolatilityWPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IVolatilityService _service;
        private IVolatilityCallback _callBack;
        public ObservableCollection<Customer> Customers { get; set; } = new ObservableCollection<Customer>();

        public MainWindow()
        {
            InitializeComponent();
            CompleteInitialise();
        }

        private void CompleteInitialise()
        {
            this.MinWidth = 500;
            this.MinHeight = 500;
            this.Title = "Volatility";
            this.DataContext = this;

            bool useMocks = false;
            if (App.Args.Length > 0)
            {
                useMocks = (App.Args[0] == "1");
            }

            if (useMocks)
            {
                _service = new VolatilityWPFApp.Mocks.VolatilityServiceMock();
                _callBack = new VolatilityWPFApp.Mocks.VolatiltiyCallbackMock();
            }
            else
            {

            }
        }

        private void menuItemNew_Click(object sender, RoutedEventArgs e)
        {
            AddNewRecord();
        }

        private void menuItemEdit_Click(object sender, RoutedEventArgs e)
        {
            var item = this.dataGrid.SelectedItem;
            if (item != null)
            {
                if (item is Customer)
                {
                    EditExistingRecord((Customer)item);
                }
            }
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            // Some operations with this row
            var cust = (Customer)row.Item;
            EditExistingRecord(cust);
        }
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void Refresh()
        {
            try
            {
                var filters = new VolatilityContracts.RequestFilters()
                {
                    FirstName = this.txtFirstNameFilter.Text,
                    LastName = this.txtLastNameFilter.Text
                };
                var customers = _service.GetCustomers(filters);
                Customers.Clear();
                foreach (var c in customers)
                {
                    Customers.Add(c);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Volatility Unexpected Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

       

        private void AddNewRecord()
        {
            var wnd = new CustomerDetailsWindow();
            wnd.ShowDialog();
        }

        private void EditExistingRecord(Customer customer)
        {
            var wnd = new CustomerDetailsWindow();
            var record = _service.GetCustomerDetails(customer.Id);
            wnd.CustomerDetailsView.CustomerDetails.CopyFrom(record);
            wnd.ShowDialog();
            

        }

        
    }
}
