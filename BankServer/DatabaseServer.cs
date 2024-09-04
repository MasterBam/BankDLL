/*
 * 
 * 
 * 
 */



using System;
using System.Collections.Generic;
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
        private readonly BankDatabase _database;
        public DatabaseServer() 
        {
            _database = new BankDatabase();
        }
        public int GetNumEntries()
        {
            return _database.GetNumRecords();
        }

        public void GetValuesForEntry(int index, out uint acctNo, out uint pin, out string fName, out string lName, out int bal)
        {
            if (index < 0 || index >= _database.GetNumRecords())
            {
                Console.WriteLine("Client attempt to access entry out of range...");
                throw new ArgumentOutOfRangeException("Index not in range...");
            }

            acctNo = _database.GetAcctNoByIndex(index);
            pin = _database.GetPINByIndex(index);
            fName = _database.GetFirstNameByIndex(index);
            lName = _database.GetLastNameByIndex(index);
            bal = _database.GetBalanceByIndex(index);
        }
    }
}
