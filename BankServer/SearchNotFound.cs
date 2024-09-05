
/*------------------------------------------------*
 * Module: IndexOutOfRange                        *
 * Description: Error when there is no mathcing   *
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
    public class SearchNotFound
    {
        [DataMember]
        public string Fault { get; set; }
    }
}
