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
            try
            {
                string source = this.GetType().Name + ".IndexButton_Click(object sender, RoutedEventArfs e), line 61";
                Load(db.GetParsedIndex(IndexBox.Text, source));
            }
            catch (FaultException<InvalidIndexError> exception)
            {
                MessageBox.Show(exception.Detail.Fault);
            }
        }

        private async void SearchButthon_Click(object sender, RoutedEventArgs e)
        {
            search = SearchBox.Text;
            Task<int> task = new Task<int>(SearchDB);
            task.Start();
            int account = await task;


            Load(account);
        }

        private void Load(int index)
        {
            try
            {
                string source = this.GetType().Name + ".Load(int index), line 85";
                db.GetValuesForEntry(index, source, out var accNo, out var pin, out var fName, out var lName, out var bal, out var icon);
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
                string source = this.GetType().Name + ".SearchDB(string val), line 125";
                //Disable buttons and set text boxes to read-only
                SetWaitingState(true);
                return db.GetSearchResult(search, source);
            }
            catch (FaultException<SearchNotFound> exception)
            {
                MessageBox.Show(exception.Detail.Fault);
            }
            finally
            {
                //Enable features again
                SetWaitingState(false);
            }
            return 0;
        }

        private void SetWaitingState(bool waiting)
        {
            // Use Dispatcher to ensure UI updates are on the main thread
            Dispatcher.Invoke(() =>
            {
                // Set text boxes ReadOnly
                fNameBox.IsReadOnly = waiting;
                lNameBox.IsReadOnly = waiting;
                balanceBox.IsReadOnly = waiting;
                acctNoBox.IsReadOnly = waiting;
                pinBox.IsReadOnly = waiting;
                IndexBox.IsReadOnly = waiting;
                SearchBox.IsReadOnly = waiting;

                // Disable/enable buttons
                IndexButton.IsEnabled = !waiting;
                SearchButton.IsEnabled = !waiting;

                // Show/hide progress bar
                searchProgressBar.Visibility = waiting ? Visibility.Visible : Visibility.Collapsed;
                if (waiting)
                {
                    statusLabel.Content = "Searching starts.....";
                    searchProgressBar.IsIndeterminate = true;
                }
                else
                {
                    statusLabel.Content = "Searching ends";
                    searchProgressBar.IsIndeterminate = false;
                }
            });
        }
    }
}
