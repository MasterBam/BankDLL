using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace BankServer
{
    [ServiceContract]
    public interface DatabaseInterface
    {
        [OperationContract]
        int GetNumEntries();
        [OperationContract]

        void GetValuesForEntry(int index, out uint acctNo, out uint pin, out string fName, out string lName, out int bal);
    }
}
