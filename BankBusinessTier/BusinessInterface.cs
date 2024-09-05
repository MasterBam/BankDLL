
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
        [FaultContract(typeof(IndexOutOfRange))]
        void GetValuesForEntry(int index, out uint acctNo, out uint pin, out string fName, out string lName, out int bal, out Bitmap icon);

        [OperationContract]
        [FaultContract(typeof(SearchNotFound))]
        int GetSearchResult(string search);
    }
}
