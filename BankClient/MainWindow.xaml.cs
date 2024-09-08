using BankBusinessTier;
using BankServer;
using BankDLL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.IO;

namespace BankClient
{
    public delegate int Search(string value);
    public partial class MainWindow : Window
    {
        private BusinessInterface db;
        private string search;

        public MainWindow()
        {
            InitializeComponent();

            // This is a factory that generates remote connections to our remote class. This 
            // is what hides the RPC stuff!
            var tcp = new NetTcpBinding();

            //Set the URL and create the connection!
            var URL = "net.tcp://localhost:8200/BankBusinessService";
            var chanFactory = new ChannelFactory<BusinessInterface>(tcp, URL);
            db = chanFactory.CreateChannel();

            // Increase the OperationTimeout for the proxy (client-side)
            var contextChannel = (IContextChannel)db;
            contextChannel.OperationTimeout = TimeSpan.FromMinutes(5); // Increase the timeout to 5 minutes

            // Also, tell me how many entries are in the DB.
            NoItems.Content = "Total Items: " + db.GetNumEntries();
            Load(0);
            IndexBox.Text = "0";
        }

        private void IndexButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(IndexBox.Text, out var index))
            {
                Load(index);
            }
            else
            {
                MessageBox.Show($"\"{IndexBox.Text}\" is not a valid integer...");
            }
        }

        private async void SearchButthon_Click(object sender, RoutedEventArgs e)
        {
            // Disable buttons and set text boxes to read-only
            SearchButton.IsEnabled = false;
            IndexButton.IsEnabled = false;
            fNameBox.IsReadOnly = true;
            lNameBox.IsReadOnly = true;
            balanceBox.IsReadOnly = true;
            acctNoBox.IsReadOnly = true;
            pinBox.IsReadOnly = true;

            // Set progress bar to Marquee (indeterminate mode)
            searchProgressBar.Visibility = Visibility.Visible;
            searchProgressBar.IsIndeterminate = true;

            statusLabel.Content = "Searching starts.....";

            search = SearchBox.Text;
            Task<int> task = new Task<int>(SearchDB);
            task.Start();
            int account = await task;

            await Task.Delay(5000);
            Load(account);
            statusLabel.Content = "Searching ends.....";

            // Re-enable buttons and set text boxes to editable
            SearchButton.IsEnabled = true;
            IndexButton.IsEnabled = true;
            fNameBox.IsReadOnly = false;
            lNameBox.IsReadOnly = false;
            balanceBox.IsReadOnly = false;
            acctNoBox.IsReadOnly = false;
            pinBox.IsReadOnly = false;

            // Set progress bar to Continuous (completed mode)
            searchProgressBar.IsIndeterminate = false;
            searchProgressBar.Visibility = Visibility.Collapsed;
        }

        private void Load(int index)
        {
            try
            {
                db.GetValuesForEntry(index, out var accNo, out var pin, out var fName, out var lName, out var bal , out var icon);
                fNameBox.Text = fName;
                lNameBox.Text = lName;
                balanceBox.Text = bal.ToString("C");
                acctNoBox.Text = accNo.ToString();
                pinBox.Text = pin.ToString("D4");

                // Convert byte array to Bitmap
                using (var ms = new MemoryStream(icon))
                {
                    using (var finalIcon = new Bitmap(ms))
                    {
                        // Convert to image source
                        UserIcon.Source = Imaging.CreateBitmapSourceFromHBitmap(
                            finalIcon.GetHbitmap(),
                            IntPtr.Zero,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions()
                        );
                    }
                }

                IndexBox.Text = index.ToString();
            }
            catch (FaultException<IndexOutOfRange> exception)
            {
                MessageBox.Show(exception.Detail.Fault);
            }
            catch (FaultException faultEx)
            {
                // If any other fault exceptions are caught
                MessageBox.Show("A general fault occurred: " + faultEx.Message);
            }
        }

        private int SearchDB()
        {
            try
            {
                return db.GetSearchResult(search);
            }
            catch (FaultException<SearchNotFound> exception)
            {
                MessageBox.Show(exception.Detail.Fault);
            }
            return 0;
        }
    }
}
