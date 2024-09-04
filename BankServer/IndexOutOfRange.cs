
/*------------------------------------------------*
 * Module: IndexOutOfRange                        *
 * Description: Error to handle an out of index   *
 *              search                            *
 * Author: Jauhar                                 *
 * ID: 21494299                                   *
 *------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BankServer
{
    [DataContract]
    public class IndexOutOfRange
    {
        [DataMember]
        public string Fault { get; set; }
    }
}
