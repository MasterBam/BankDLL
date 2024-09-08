
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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
        private static uint LogNumber = 0;
        private static readonly object _logLock = new object();
        private readonly string LogFilePath;

        public BusinessServer()
        {
            //Setting up connection
            ChannelFactory<DatabaseInterface> channelFactory;

            var tcp = new NetTcpBinding();

            string URL = "net.tcp://localhost:8100/BankDataService";

            channelFactory = new ChannelFactory<DatabaseInterface>(tcp, URL);

            dbServer = channelFactory.CreateChannel();

            // Set log file path to the BusinessTier project folder
            string projectFolder = Directory.GetParent(Assembly.GetExecutingAssembly().Location).Parent.Parent.Parent.FullName;
            LogFilePath = Path.Combine(projectFolder, "BusinessTierLog.txt");

            Log("", "");
        }

        // The Log method to be used internally
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void Log(string logString, string source)
        {
            lock (_logLock)
            {
                //Reset logfile
                if(logString == "")
                {
                    using (StreamWriter writer = new StreamWriter(LogFilePath, false))
                    {
                        writer.WriteLine("======== New Log Session Started =======" +
                                        $"\nFrom: {this.GetType().Name}" +
                                        $"\nBegins at: {DateTime.Now}");
                    }
                }
                else
                {
                    LogNumber++;
                    string logEntry = $"\nLog #{LogNumber} - {DateTime.Now}: {logString} {source}";
                    
                    //Write the log entry to a file
                    using (StreamWriter writer = new StreamWriter(LogFilePath, true))
                    {
                        writer.WriteLine(logEntry);
                    }

                    //Write to the console as well
                    Console.WriteLine(logEntry);
                }
            }
        }

        public int GetNumEntries()
        {
            return dbServer.GetNumEntries();
        }

        public int GetParsedIndex(string arg, string source)
        {
            int correctIndex = 0;
            string error= string.Empty;

            //Check if the first character is '0' when the string length is greater than 1
            if (arg.Length > 1 && arg[0] == '0')
            {
                error = "InvalidIndexError:: Attempted to enter an index with a leading zero '0'";
                Log(error, source + "\n             Detected at line 86");
                throw new FaultException<InvalidIndexError>(
                    new InvalidIndexError { Fault = $"Index should not start with a leading zero: '{arg}'." },
                    new FaultReason("Invalid index"));
            }

            //Check if unkown chars are typed into index box
            if (int.TryParse(arg, out var index))
            {
                correctIndex = index;
            }
            else
            {
                error = "InvalidIndexError:: An unkown character detected when attempted to parse index";
                Log(error, source + "\n             Detected at line 96");
                Console.WriteLine(error);
                throw new FaultException<InvalidIndexError>(
                    new InvalidIndexError { Fault = "An invalid index input detected" },
                    new FaultReason("Invalid index"));
            }

            return correctIndex;
        }

        public void GetValuesForEntry(int index, string source, out uint acctNo, out uint pin, out string fName, out string lName, out int bal, out byte[] icon)
        {
            if (index < 0 || index >= dbServer.GetNumEntries())
            {
                string error = "IndexOutOfRange:: Client attempt to access entry out of range";
                Log(error, source + "\n             Detected at line 115");
                Console.WriteLine(error);
                throw new FaultException<IndexOutOfRange>(
                    new IndexOutOfRange { Fault = "Index not within range" },
                    new FaultReason("Index out of range"));
            }

            dbServer.GetValuesForEntry(index, out acctNo, out pin, out fName, out lName, out bal, out icon);
        }

        public int GetSearchResult(string search, string source)
        {
            if (string.IsNullOrEmpty(search))
            {
                throw new ArgumentNullException(nameof(search));
            }

            //Validate that the search term contains only alphabetic characters
            if (!System.Text.RegularExpressions.Regex.IsMatch(search, @"^[a-zA-Z]+$"))
            {
                string error = $"InvalidSearch:: Attempted search with invalid characters: '{search}'";
                Log(error, source + "\n             Detected at line 148");
                Console.WriteLine(error);
                throw new FaultException<InvalidSearch>(
                    new InvalidSearch { Fault = "Search term contains invalid characters. Only alphabetic characters are allowed." },
                    new FaultReason("Invalid search term"));
            }

            int? resultI = null;

            lock (_logLock)
            {
                string name = search;
                resultI = dbServer.GetSearch(name);

                if (resultI is null)
                {
                    string error = $"SearchNotFound:: Client attempt to search a non-existent record of '{name}', result index: {resultI}";
                    Log(error, source + "\n             Detected at line 167");
                    Console.WriteLine(error);
                    throw new FaultException<SearchNotFound>(
                        new SearchNotFound { Fault = $"Record of '{name}' is non-existent" },
                        new FaultReason("Non-existent record"));
                }

                return (int)resultI;
            }
        }
    }
}