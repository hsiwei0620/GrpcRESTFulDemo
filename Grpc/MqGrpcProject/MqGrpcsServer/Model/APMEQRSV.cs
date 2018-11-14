using Newtonsoft.Json;
using System.Collections.Generic;

namespace MqGrpcsServer
{
    public class APMEQRSV
    {
        public APMEQRSV_t transaction { get; set; }

        public class APMEQRSV_t
        {
            public string trx_id { get; set; }
            public string type_id { get; set; }
            public string rtn_code { get; set; }
            public string rtn_mesg { get; set; }
            public string lot_ary_cnt { get; set; }
            [Newtonsoft.Json.JsonConverter(typeof(SingleOrArrayConverter<Oary>))]
            public List<Oary>  oary  { get; set; }
            public string no_rsv_cnt { get; set; }
            [Newtonsoft.Json.JsonConverter(typeof(SingleOrArrayConverter<Oary1>))]
            public List<Oary1> oary1 { get; set; }

            public class Oary
            {
                public string lot_id { get; set; }
                public string nx_ope_no { get; set; }
                public string nx_ope_id { get; set; }
                public string nx_ope_ver { get; set; }
                public string splt_id { get; set; }
                public string resv_eqpt_id { get; set; }
                public string lot_stat { get; set; }
                public string resv_date { get; set; }
                public string resv_shift_seq { get; set; }
                public string resv_comment { get; set; }
                public string plan_opt_weight { get; set; }
                public string move_out_weight { get; set; }
                public string sht_cnt { get; set; }
                public string cr_sht_cnt { get; set; }
                public string run_flag { get; set; }
                public string product_id { get; set; }
                public string ec_code { get; set; }
                public string route_id { get; set; }
                public string route_ver { get; set; }
                public string cr_ope_no { get; set; }
                public string recipe_id { get; set; }
                public string in_sht_cnt { get; set; }
                public string std_ld_time { get; set; }
                public string man_ope_time { get; set; }
                public string piece { get; set; }
                public string ope_no_flag { get; set; }
                public string key { get; set; }
                public string type { get; set; }
            }
            public class Oary1
            {
                public string lot_id { get; set; }
                public string product_id { get; set; }
                public string ec_code { get; set; }
                public string plan_opt_weight { get; set; }
                public string sht_cnt { get; set; }
                public string route_id { get; set; }
                public string route_ver { get; set; }
                public string cr_ope_no { get; set; }
                public string nx_ope_no { get; set; }
                public string nx_ope_id { get; set; }
                public string nx_ope_ver { get; set; }
                public string ope_no_flag { get; set; }
                public string std_ld_time { get; set; }
                public string man_ope_time { get; set; }
                public string type { get; set; }
                public string fit_eqpts { get; set; }
            }
        }   
    }     
}