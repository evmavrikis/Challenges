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
using System.ServiceModel;
using System.Configuration;

namespace VolatilityWPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IVolatilityCallback
    {
        private const int MaxBufferSize = 2000000;
        private const int MaxReceivedMessageSize = 2000000;

        private IVolatilityService _service;
        private InstanceContext _context;

        private NotificationInfo _notificationInfo = new NotificationInfo();
        private readonly object _monitor = new object();

      
        public ObservableCollection<Customer> Customers { get; set; } = new ObservableCollection<Customer>();

        private CustomerDetails _modifiedRecord = null;
        public MainWindow()
        {
            InitializeComponent();
            CompleteInitialise();
        }

        public void SendNotification(Notification notification)
        {
            UpdateNotifications(notification);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Refresh();
            }
            catch (Exception ex)
            {
          
                DisplayError(ex);
            }
        }
        private void CompleteInitialise()
        {
            this.MinWidth = 600;
            this.MinHeight = 500;
            this.Title = "Volatility";
            this.DataContext = this;

            bool useMockService = false;
            if (App.Args.Length > 0)
            {
                useMockService = (App.Args[0] == "1");
            }

            
            if (useMockService)
            {
                _service = new VolatilityWPFApp.Mocks.VolatilityServiceMock(this); 
            }
            else
            {
                // Initialise WCF client.
                InitConnection();
               
            }
        }


        private bool CheckException(Exception ex)
        {
            if (ex.HResult == -2146233087)
            {
                return InitConnection();
            }

            return false;
        }
        private bool InitConnection()
        {
            var dt = DateTime.Now;
            bool success = false;
            int secs;
            var v = ConfigurationManager.AppSettings["ConnectThresholdSeconds"];
            int.TryParse(v, out secs);
            secs = Math.Max(secs, 1);
            while(!success && (DateTime.Now - dt).TotalSeconds < secs)
            {
                success = TryInitConnection();
            }

            return success;
        }
        private bool TryInitConnection()
        {
            try
            {
                var address = new EndpointAddress(ConfigurationManager.AppSettings["ServiceEndPoint"]);
                var binding = new NetNamedPipeBinding()
                {
                    MaxBufferSize = MainWindow.MaxBufferSize,
                    MaxReceivedMessageSize = MainWindow.MaxReceivedMessageSize
                };

                _context = new InstanceContext(this);

                var factory = new DuplexChannelFactory<IVolatilityService>(_context, binding, address);
                _service = factory.CreateChannel();
                return _service.Ping();
            }
            catch
            {
                return false;
            }
        }
        private void menuItemExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void menuItemNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddNewRecord();
            }
            catch (Exception ex)
            {
                DisplayError(ex);
                
            }
        }

        private void menuItemEdit_Click(object sender, RoutedEventArgs e)
        {
            ExecuteActionForSelectedCustomer(EditExistingRecord);
        }

        private void menuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            ExecuteActionForSelectedCustomer(DeleteExistingRecord);
        }
        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DataGridRow row = sender as DataGridRow;
                var cust = (Customer)row.Item;
                EditExistingRecord(cust);
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Refresh();
            }
            catch(Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void RefreshCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                Refresh();
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void NewCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                AddNewRecord();
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void OpenCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ExecuteActionForSelectedCustomer(EditExistingRecord);
        }

        private void DeleteCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ExecuteActionForSelectedCustomer(DeleteExistingRecord);
        }

        private void menuItemRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Refresh();
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                ExecuteActionForSelectedCustomer(DeleteExistingRecord);
                //_notificationInfo.RecordsDisplayed--;
                //SetNotifictionContent();
                
            }
        }

        private void ExecuteActionForSelectedCustomer(Action<Customer> act)
        {
            try
            {
                var item = this.dataGrid.SelectedItem;
                if (item != null)
                {
                    if (item is Customer)
                    {
                        var c = ((Customer)item);
                        act(c);
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }


        }

        
        private void Refresh()
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

            lock(_monitor)
            {
                _notificationInfo.Reset();
                _notificationInfo.RecordsDisplayed = this.Customers.Count;
            }
            SetNotifictionContent();
            
        }

       

        private void AddNewRecord()
        {
            var wnd = new CustomerDetailsWindow()
            {
                Title = "New Customer",
                SetRecordDel = SetModifiedRecord
            };
            var record = new CustomerDetails();

            wnd.CustomerDetailsView.CustomerDetails.CopyFrom(record);
            _modifiedRecord = null;

            wnd.ShowDialog();

            if (_modifiedRecord != null)
            {
                _service.AddNewCustomer(_modifiedRecord);
                Refresh();
            }
        }

        private void EditExistingRecord(Customer customer)
        {
            
            var record = _service.GetCustomerDetails(customer.Id);

            if (record == null)
            {
                MessageBox.Show("Customer details could not be retrieved. Please refresh.", "Volatility warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var wnd = new CustomerDetailsWindow()
            {
                Title = "Edit Customer",
                SetRecordDel = SetModifiedRecord
            };
            wnd.CustomerDetailsView.CustomerDetails.CopyFrom(record);
            _modifiedRecord = null;

            wnd.ShowDialog();
            
            if (_modifiedRecord != null)
            {
                _service.UpdateCustomer(_modifiedRecord);
            }

        }

        private void DeleteExistingRecord(Customer customer)
        {
            if ( !_service.DeleteCustomer(customer.Id))
            {
                MessageBox.Show("Customer could not be deleted.", "Volatility warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            Refresh();
        }


        private void SetModifiedRecord(CustomerDetails record)
        {
            _modifiedRecord = record;
        }
        
        private void UpdateNotifications(Notification notification)
        {
            lock (_monitor)
            {
                switch (notification)
                {
                    case Notification.UnexpectedError:
                        _notificationInfo.Errors++;
                        break;
                    case Notification.RecordAdded:
                        _notificationInfo.Additions++;
                        break;
                    case Notification.RecordDeleted:
                        _notificationInfo.Deletions++;
                        break;                     
                    case Notification.RecordUpdated:
                        _notificationInfo.Updates++;
                        break;
                }
            }

            SetNotifictionContent();
        }

        private void SetNotifictionContent()
        {
            var act = new Action(() => this.lblNotifications.Content = _notificationInfo.GetDisplayString());
            Dispatcher.Invoke(act);
        }

        private void DisplayError(Exception ex)
        {
            if (CheckException(ex))
            {
                MessageBox.Show("There was a connection loss which is now restored. Please try again.", "Volatility Connection Issue", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                MessageBox.Show(ex.Message, "Volatility Unexpected Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                _context.Close();
            }
            catch
            {

            }
        }

        
    }
}
