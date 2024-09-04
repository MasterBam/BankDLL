using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDLL
{
    public class BankDatabase
    {
        private readonly List<AccountData> _accounts;

        public BankDatabase()
        {
            _accounts = new List<AccountData>();

            var generator = new DatabaseGenerator();

            for (var i = 0; i < generator.GetNumAcct(); i++)
            {
                var temp = new AccountData();

                generator.GetNextAccount(out temp.pin, out temp.acctNo, out temp.firstName, out temp.lastName, out temp.balance);

                _accounts.Add(temp);
            }
        }

        public uint GetAcctNoByIndex(int index)
        {
            return _accounts[index].acctNo;
        }

        public uint GetPINByIndex(int index)
        {
            return _accounts[index].pin;
        }

        public string GetFirstNameByIndex(int index) 
        {
            return _accounts[index].firstName;
        }

        public string GetLastNameByIndex(int index)
        {
            return _accounts[index].lastName;
        }

        public int GetBalanceByIndex(int index)
        {
            return _accounts[index].balance;
        }

        public int GetNumRecords() 
        {
            return _accounts.Count;
        }
    }
}
