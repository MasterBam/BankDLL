
/*------------------------------------------------*
 * Module: BankDatabase                           *
 * Description: The database of the bank          *
 * Author: Jauhar                                 *
 * ID: 21494299                                   *
 *------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDLL
{
    public class BankDatabase
    {
        private readonly List<AccountData> _accounts;
        private readonly Dictionary<string, List<int>> _lastNameIndex;
        private readonly static object _lock = new object();

        public static BankDatabase Instance { get; } = new BankDatabase();

        private BankDatabase()
        {
            _accounts = new List<AccountData>();
            _lastNameIndex = new Dictionary<string, List<int>>();


            var generator = new DatabaseGenerator();

            for (var i = 0; i < generator.GetNumAcct(); i++)
            {
                var temp = new AccountData();

                generator.GetNextAccount(out temp.pin, out temp.acctNo, out temp.firstName, out temp.lastName, out temp.balance, out temp.icon);

                lock (_lock)
                {
                    _accounts.Add(temp);

                    if (!_lastNameIndex.ContainsKey(temp.lastName))
                    {
                        _lastNameIndex[temp.lastName] = new List<int>();
                    }
                    _lastNameIndex[temp.lastName].Add(i);
                }
            }
        }

        public uint GetAcctNoByIndex(int index)
        {
            lock (_lock)
            {
                return _accounts[index].acctNo;
            }
        }

        public uint GetPINByIndex(int index)
        {
            lock (_lock)
            {
                return _accounts[index].pin;
            }
        }

        public string GetFirstNameByIndex(int index)
        {
            lock (_lock)
            {
                return _accounts[index].firstName;
            }
        }

        public string GetLastNameByIndex(int index)
        {
            lock (_lock)
            {
                return _accounts[index].lastName;
            }
        }

        public int GetBalanceByIndex(int index)
        {
            lock (_lock)
            {
                return _accounts[index].balance;
            }
        }

        public byte[] GetIconByIndex(int index)
        {
            lock (_lock)
            {
                ImageConverter img = new ImageConverter();
                return (byte[])img.ConvertTo(_accounts[index].icon, typeof(byte[]));
            }
        }

        public int GetNumRecords()
        {
            lock (_lock)
            {
                return _accounts.Count;
            }
        }

        public int? GetFirstIndexByLastName(string lastName)
        {
            lock (_lock)
            {
                if (_lastNameIndex.TryGetValue(lastName, out var indices) && indices.Count > 0)
                {
                    return indices[0]; // Return the first occurrence
                }
                return null; // Or throw an exception if preferred
            }
        }
    }
}
