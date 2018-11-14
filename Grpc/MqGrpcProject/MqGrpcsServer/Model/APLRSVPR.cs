using Newtonsoft.Json;
using System.Collections.Generic;

namespace MqGrpcsServer
{
    public class APLRSVPR
    {
        public APLRSVPR_t transaction { get; set; }

        public class APLRSVPR_t{
            public string trx_id {get; set;}
            public string type_id {get; set;}
            public string rtn_code {get; set;}
            public string rtn_mesg {get; set;}
            public string lot_ary_cnt {get; set;}
            [Newtonsoft.Json.JsonConverter(typeof(SingleOrArrayConverter<Oary>))]
            public List<Oary>  oary  { get; set; }

            public class Oary{
                public string lot_id {get; set;} 
                public string lot_stat {get; set;} 
                public string nx_ope_no {get; set;} 
                public string mtrl_product_id {get; set;} 
                public string prep_type {get; set;} 		//備料類別:RCUT/DRUG
                public string prep_stat {get; set;} 		//備料狀態:INIT/PARE/COMP
                public string prep_date {get; set;} 		//備料排產日期
                public string prep_seq_no {get; set;} 		//備料排產順序
                public string resv_date {get; set;} 		//工單排產日期
                public string resv_shift_seq {get; set;} 	//工單排產班別
                public string route_id {get; set;} 
                public string route_ver {get; set;}
                public string claim_date {get; set;} 
                public string claim_time {get; set;} 
                public string claim_user {get; set;} 
                public string nx_ope_id {get; set;} 
                public string nx_ope_ver {get; set;} 
                public string splt_id {get; set;} 
                public string resv_eqpt_id {get; set;} 
                public string sht_cnt {get; set;} 
                public string ready_sht_cnt {get; set;} 
                public string product_id {get; set;} 
                public string ec_code {get; set;} 
                public string main_route_id {get; set;}
                public string main_route_ver {get; set;}
                public string cr_ope_no {get; set;}
                public string line_id {get; set;}
            }
        }        
    }
}