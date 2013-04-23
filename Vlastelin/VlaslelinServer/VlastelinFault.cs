using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace VlastelinServer
{
    [DataContract(Namespace = "vlastelinService")]
    public class VlastelinFault
    {
        [DataMember]
        public bool isCritical { get; set; }

        [DataMember]
        public string message { get; set; }


        public VlastelinFault(bool critical, string msg)
        {
            isCritical = critical;
            message = msg;
        }

        public VlastelinFault() :
            this(false, "")
        {
        }
    }
}
