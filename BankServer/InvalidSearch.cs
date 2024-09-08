/*------------------------------------------------*
 * Module: InvalidSearch                          *
 * Description: Error when there is an attempt to *
 *              search with non-alphabet chars    *
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
    public class InvalidSearch
    {
        [DataMember]
        public string Fault { get; set; }
    }
}