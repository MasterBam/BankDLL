
/*------------------------------------------------*
 * Module: Program                                *
 * Description: The program of the server         *
 * Author: Jauhar                                 *
 * ID: 21494299                                   *
 *------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace BankServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //Start the server
            Console.WriteLine("Welcome:: Data Service initiation...");

            //create tcp binding
            var tcp = new NetTcpBinding();
            
            //Create the host + binding server to implementation of server
            var host = new ServiceHost(typeof(DatabaseServer));

            host.AddServiceEndpoint(typeof(DatabaseInterface), tcp, "net.tcp://0.0.0.0:8100/BankDataService");

            //open server
            host.Open();
            //Hold the server open until someone does something
            Console.WriteLine("System Online");
            Console.ReadLine();

            //Close the host
            host.Close();
        }
    }
}
