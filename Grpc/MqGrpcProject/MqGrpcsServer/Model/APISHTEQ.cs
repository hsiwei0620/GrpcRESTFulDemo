using Newtonsoft.Json;
using System.Collections.Generic;

namespace MqGrpcsServer
{
    public class APISHTEQ
    {
        public APISHTEQ_t transaction { get; set; }

        public class APISHTEQ_t
        {
            public string trx_id { get; set; }
            public string type_id { get; set; }
            public string rtn_code { get; set; }
            public string rtn_mesg { get; set; }
            public string eqpt_id { get; set; }

            public string eqpt_dsc { get; set; }
            public string eqpt_cate { get; set; }
            public string recipe_id { get; set; }

            public string eqpt_stat { get; set; }
            public string eqpt_stat_dsc { get; set; }
            public string eqpt_mode { get; set; }
            public string sgr_cnt { get; set; }
            [Newtonsoft.Json.JsonConverter(typeof(SingleOrArrayConverter<Oary1>))]
            public List<Oary1>  oary1  { get; set; }
            
            
            public class Oary1
            {
                public string sgr_id { get; set; }
                public string sht_ary_cnt { get; set; }
                [Newtonsoft.Json.JsonConverter(typeof(SingleOrArrayConverter<Oary2>))]
                public List<Oary2> oary2 { get; set; }                
            }
            
            public class Oary2
            {
                public string sht_id { get; set; }
                public string sht_stat { get; set; }
                public string lot_id { get; set; }
                public string splt_id { get; set; }
                public string nx_route_cate { get; set; }
                public string nx_route_id { get; set; }
                public string nx_route_ver { get; set; }
                public string cr_ope_no { get; set; }
                public string nx_ope_no { get; set; }
                public string nx_ope_id { get; set; }
                public string nx_ope_ver { get; set; }
                public string nx_ope_dsc { get; set; }
                public string nx_pep_lvl { get; set; }
                public string nx_recipe_id { get; set; }
                public string nx_proc_id { get; set; }
                public string product_id { get; set; }
                public string ec_code { get; set; }
                public string unit { get; set; }
                public string sgr_id { get; set; }
                public string sht_cnt { get; set; }
                public string move_in_flg { get; set; }
                public string slot_no { get; set; }
                public string show_flag { get; set; }
                public string fake_id { get; set; }
                public string erp_lot_no { get; set; }
            }
        }   
    }     
}