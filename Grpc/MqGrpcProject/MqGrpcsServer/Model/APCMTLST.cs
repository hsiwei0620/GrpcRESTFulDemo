using Newtonsoft.Json;
using System.Collections.Generic;

namespace MqGrpcsServer
{
    public class APCMTLST
    {
        public APCMTLST_t transaction { get; set; }

        public class APCMTLST_t{
            public string trx_id {get; set;} 				//       Transaction ID
            public string type_id {get; set;} 				//       input / output
            public string rtn_code {get; set;} 				//       return code
            public string rtn_mesg {get; set;} 				//       return message
        }
    }
}