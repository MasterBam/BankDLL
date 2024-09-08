
/*------------------------------------------------*
 * Module: BusinessServer                         *
 * Description: The implementation of the business*
 *              server interface                  *
 * Author: Jauhar                                 *
 * ID: 21494299                                   *
 *------------------------------------------------*/

using BankServer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BankBusinessTier
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    internal class BusinessServer : BusinessInterface
    {
        private DatabaseInterface dbServer;


        public BusinessServer()
        {
            //Setting up connection
            ChannelFactory<DatabaseInterface> channelFactory;

            var tcp = new NetTcpBinding();

            string URL = "net.tcp://localhost:8100/BankDataService";

            channelFactory = new ChannelFactory<DatabaseInterface>(tcp, URL);

            dbServer = channelFactory.CreateChannel();
        }
        public int GetNumEntries()
        {
            return dbServer.GetNumEntries();
        }

        public void GetValuesForEntry(int index, out uint acctNo, out uint pin, out string fName, out string lName, out int bal, out byte[] icon)
        {
            if (index < 0 || index >= dbServer.GetNumEntries())
            {
                Console.WriteLine("Client attempt to access entry out of range");
                throw new FaultException<IndexOutOfRange>(
                    new IndexOutOfRange { Fault = "Index not within range" },
                    new FaultReason("Index out of range"));
            }

            dbServer.GetValuesForEntry(index, out acctNo, out pin, out fName, out lName, out bal, out icon);
        }

        public int GetSearchResult(string search)
        {
            int? resultI = dbServer.GetSearch(search);

            if (resultI is null)
            {
                Console.WriteLine("Client attempt to search a non-existent record");
                throw new FaultException<SearchNotFound>(
                    new SearchNotFound { Fault = $"Record of '{search}' is non-existent" },
                    new FaultReason("Non-existent record"));
            }

            return (int)resultI;
        }
    }
}