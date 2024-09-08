
/*------------------------------------------------*
 * Module: DatabaseGenerator                      *
 * Description: Generates data                    *
 * Author: Jauhar                                 *
 * ID: 21494299                                   *
 *------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace BankDLL
{
    public class DatabaseGenerator
    {
        private readonly Random _random = new Random();

        private readonly string[] _fNames =
        {
            "Ace", "Arnold", "Ackerman", "Bob", "Brian", "Brown", "Blaque", "Claire", "Chloe", "Clancy", "Darius", "Dran", "Dillan", "Dilla", "Damien", "Thomas", "Reid", "Zlam"
        };

        private readonly string[] _lNames =
        {
            "Xavier", "Black", "Dick", "Trevor", "Vlad", "Mirian", "Joseph", "Arby", "Mc'Kenen", "Prowl", "Chairmy", "Fraw", "Plog", "Brad", "Heimer", "Chad", "Ghan", "Khan", "Talani"
        };

        private readonly List<Bitmap> _icons;

        public DatabaseGenerator()
        {
            _icons = new List<Bitmap>();
            // Generate a few really basic icons
            for (var i = 0; i < 10; i++)
            {
                var image = new Bitmap(64, 64);
                for (var x = 0; x < 64; x++)
                {
                    for (var y = 0; y < 64; y++)
                    {
                        image.SetPixel(x, y, Color.FromArgb(_random.Next(256), _random.Next(256), _random.Next(256)));
                    }
                }
                _icons.Add(image);
            }
        }

        public int GetNumAcct() => _random.Next(100000, 999999);
        private string GetFirstName() => _fNames[_random.Next(_fNames.Length)];
        private string GetLastName() => _lNames[_random.Next(_lNames.Length)];

        private uint GetPIN() => (uint)_random.Next(9999);
        private uint GetAcctNo() => (uint)_random.Next(100000000, 999999999);
        private int GetBalance() => _random.Next(0, 999999);

        private Bitmap GetIcon() => _icons[_random.Next(_icons.Count)];

        public void GetNextAccount(out uint pin, out uint acctNo, out string firstName, out string lastName, out int balance, out Bitmap icon) 
        {
            pin = GetPIN();
            acctNo = GetAcctNo();
            firstName = GetFirstName();
            lastName = GetLastName();
            balance = GetBalance();
            icon = GetIcon();
        }
    }
}
