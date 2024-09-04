
/*------------------------------------------------*
 * Module: DatabaseServer                         *
 * Description: The implementation of the server  *
 *              interface                         *
 * Author: Jauhar                                 *
 * ID: 21494299                                   *
 *------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using BankDLL;

namespace BankServer
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext= false)]

    internal class DatabaseServer : DatabaseInterface
    {
        private readonly BankDatabase _database = BankDatabase.Instance;

        public int GetNumEntries()
        {
            return _database.GetNumRecords();
        }

        public void GetValuesForEntry(int index, out uint acctNo, out uint pin, out string fName, out string lName, out int bal, out Bitmap icon)
        {
            if (index < 0 || index >= _database.GetNumRecords())
            {
                Console.WriteLine("Client attempt to access entry out of range");
                throw new FaultException<IndexOutOfRange>(new IndexOutOfRange { Fault = "Index not within range"});
            }

            acctNo = _database.GetAcctNoByIndex(index);
            pin = _database.GetPINByIndex(index);
            fName = _database.GetFirstNameByIndex(index);
            lName = _database.GetLastNameByIndex(index);
            bal = _database.GetBalanceByIndex(index);
            icon = _database.GetIconByIndex(index);
        }
    }
}
