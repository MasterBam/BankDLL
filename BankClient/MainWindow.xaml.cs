
/*------------------------------------------------*
 * Module: BankClient                             *
 * Description: The implementation of the async   *
 *              utilizing client (Task 3)         *
 * Author: Jauhar                                 *
 * ID: 21494299                                   *
 *------------------------------------------------*/
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
using System.Runtime.CompilerServices;

namespace BankClient
{
    public partial class MainWindow : Window
    {
        private BusinessInterface db;
        private string search;
        private string eSource;

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

            //Set source
            eSource = $"AsyncClient.{this.GetType().FullName}";

            // Also, tell me how many entries are in the DB.
            NoItems.Content = "Total Items: " + db.GetNumEntries();
            Load(0);
            IndexBox.Text = "0";
        }

        private void IndexButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string source = eSource + ".IndexButton_Click(object sender, RoutedEventArfs e), line 74";
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

            // Define the timeout duration
            TimeSpan timeout = TimeSpan.FromSeconds(60); // Adjust as needed

            var searchTask = Task.Run(() => SearchDB());
            var timeoutTask = Task.Delay(timeout);

            var completedTask = await Task.WhenAny(searchTask, timeoutTask);

            if (completedTask == searchTask)
            {
                // The search task completed within the timeout
                int account = await searchTask;
                Load(account);
            }
            else
            {
                // The timeout task completed first
                MessageBox.Show("The search operation timed out (time elapsed: 1.00 mins).");
            }
        }

        private void Load(int index)
        {
            try
            {
                string source = eSource + ".Load(int index), line 112";
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
                string source = eSource + ".SearchDB(string val), line 154";
                //Disable buttons and set text boxes to read-only
                SetWaitingState(true);
                return db.GetSearchResult(search, source);
            }
            catch (FaultException<SearchNotFound> exception)
            {
                MessageBox.Show(exception.Detail.Fault);
            }
            catch (FaultException<InvalidSearch> exception)
            {
                MessageBox.Show(exception.Detail.Fault);
            }
            catch (ArgumentNullException e)
            {
                MessageBox.Show($"The search value '{search}' passed through as null: {e.Message}");
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
