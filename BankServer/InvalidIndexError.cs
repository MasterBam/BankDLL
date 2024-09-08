
/*------------------------------------------------*
 * Module: InvalidIndexError                      *
 * Description: Error to handle invalid index     *
 *              (the literal arguements)          *
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
    public class InvalidIndexError
    {
        [DataMember]
        public string Fault { get; set; }
    }
}
