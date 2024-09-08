using BankBusinessTier;
using BankServer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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

namespace BankClientDelegate
{
    public delegate int Search(string searchVal);
    public partial class MainWindow : Window
    {
        private BusinessInterface db;
        private Search search;

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

        private void Load(int index)
        {
            try
            {
                db.GetValuesForEntry(index, out var accNo, out var pin, out var fName, out var lName, out var bal, out var icon);

                fNameBox.Dispatcher.Invoke(new Action(() => fNameBox.Text = fName));
                lNameBox.Dispatcher.Invoke(new Action(() => lNameBox.Text = lName));
                balanceBox.Dispatcher.Invoke(new Action(() => balanceBox.Text = bal.ToString("C")));
                acctNoBox.Dispatcher.Invoke(new Action(() => acctNoBox.Text = accNo.ToString()));
                pinBox.Dispatcher.Invoke(new Action(() => pinBox.Text = pin.ToString("D4")));

                // Convert byte array to Bitmap
                using (var ms = new MemoryStream(icon))
                {
                    using (var finalIcon = new Bitmap(ms))
                    {
                        // Convert to image source
                        UserIcon.Dispatcher.Invoke(new Action(() => UserIcon.Source = Imaging.CreateBitmapSourceFromHBitmap(
                            finalIcon.GetHbitmap(),
                            IntPtr.Zero,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions()
                        )));
                    }
                }

                IndexBox.Dispatcher.Invoke(new Action(() => IndexBox.Text = index.ToString()));
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

        private void SearchButthon_Click(object sender, RoutedEventArgs e)
        {
            //Set to waiting state
            SetWaitingState(true);

            search = SearchDB;
            AsyncCallback callback;
            callback = this.OnCompletion;
            IAsyncResult result = search.BeginInvoke(SearchBox.Text, callback, null);
        }

        private int SearchDB(string val)
        {
            try
            {
                return db.GetSearchResult(val);
            }
            catch (FaultException<SearchNotFound> exception)
            {
                MessageBox.Show(exception.Detail.Fault);
            }
            return 0;
        }

        private void OnCompletion(IAsyncResult asyncResult)
        {
            int iStudent = 0;
            Search search = null;
            AsyncResult asyncobj = (AsyncResult)asyncResult;
            if (asyncobj.EndInvokeCalled == false)
            {
                search = (Search)asyncobj.AsyncDelegate;
                iStudent = search.EndInvoke(asyncobj);
                Load(iStudent);
            }

            //Close job
            asyncobj.AsyncWaitHandle.Close();

            //Transition out of waiting
            SetWaitingState(false);
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
