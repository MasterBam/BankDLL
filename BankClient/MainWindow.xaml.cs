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

            search = SearchBox.Text;
            Task<AccountData> task = new Task<AccountData>(SearchDB);
            task.Start();
            statusLabel.Content = "Searching starts.....";
            AccountData account = await task;
            UpdateGui(account);
            statusLabel.Content = "Searching ends.....";
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
                // Convert to image source
                UserIcon.Source = Imaging.CreateBitmapSourceFromHBitmap(icon.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                icon.Dispose();
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

        private AccountData SearchDB()
        {
            try
            {
                AccountData acct = new AccountData();
                db.GetSearchResult(this.search, out acct.acctNo, out acct.pin, out acct.firstName, out acct.lastName, out acct.balance, out acct.icon);
                return acct;
            }
            catch(FaultException<SearchNotFound> exception)
            {
                MessageBox.Show(exception.Detail.Fault);
            }
            return null;
        }

        private void UpdateGui(AccountData acct)
        {
            fNameBox.Text = acct.firstName;
            lNameBox.Text = acct.lastName;
            balanceBox.Text = acct.balance.ToString("C");
            acctNoBox.Text = acct.acctNo.ToString();
            pinBox.Text = acct.pin.ToString("D4");
        }
    }
}
