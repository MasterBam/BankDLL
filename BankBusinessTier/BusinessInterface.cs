
/*------------------------------------------------*
 * Module: BusinessInterface                      *
 * Description: The interface of the business tier*
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
using System.Threading.Tasks;

namespace BankBusinessTier
{
    [ServiceContract]
    public interface BusinessInterface
    {
        [OperationContract]
        int GetNumEntries();

        [OperationContract]
        [FaultContract(typeof(InvalidIndexError))]
        int GetParsedIndex(string arg, string source);

        [OperationContract]
        [FaultContract(typeof(IndexOutOfRange))]
        void GetValuesForEntry(int index, string source, out uint acctNo, out uint pin, out string fName, out string lName, out int bal, out byte[] icon);

        [OperationContract]
        [FaultContract(typeof(SearchNotFound))]
        [FaultContract(typeof(InvalidSearch))]
        int GetSearchResult(string search, string source);
    }
}
