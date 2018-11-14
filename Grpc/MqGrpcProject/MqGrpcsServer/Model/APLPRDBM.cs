using Newtonsoft.Json;
using System.Collections.Generic;

namespace MqGrpcsServer
{
    public class APLPRDBM
    {
        public APLPRDBM_t transaction { get; set; }

        public class APLPRDBM_t{
            public string trx_id {get; set;}				//       Transaction ID
            public string type_id {get; set;}				//       input / output
            public string rtn_code {get; set;}				//       return code
            public string rtn_mesg {get; set;}				//       return message
            public string product_id {get; set;}			//       Product ID
            public string ec_code {get; set;}				//       Engineering Change
            public string route_id {get; set;}				//       Route ID
            public string route_ver {get; set;}				//       Route Version
            public string bom_cnt {get; set;}				//       Material Product Count

            [Newtonsoft.Json.JsonConverter(typeof(SingleOrArrayConverter<Oary>))]
            public List<Oary>  oary1  { get; set; }
            
            public class Oary{
                public string ope_id {get; set;}            //
                public string ope_ver {get; set;}		    //       Current Operation Version
                public string bom_id {get; set;}			//       Equipment ID
                public string bom_dsc {get; set;}			//       BOM Description
                public string stage_id {get; set;}
                public string addt_info_1 {get; set;}		//       Additional Information1
                public string addt_info_2 {get; set;}		//       Additional Information2
                public string addt_info_3 {get; set;}		//       Additional Information3
                public string mtrl_cnt {get; set;}			//       Material  Count
                
                [Newtonsoft.Json.JsonConverter(typeof(SingleOrArrayConverter<Oary2>))]
                public List<Oary2>  oary2  { get; set; }
                
                public class Oary2{
                    public string mtrl_product_id {get; set;} 		    //       mtrl_prod_id
                    public string mtrl_product_dsc {get; set;} 			//       Substitude Material Product ID
                    public string mtrl_cate {get; set;} 				//       %
                    public string plan_qty {get; set;} 				    //       user_id
                    public string mtrl_unit {get; set;} 		        //       chg_date
                    
                    public string spcfy_mtrl_lot_id {get; set;} 	    //       chg_time
                    public string ext_1 {get; set;}                    
                    public string ext_2 {get; set;}                    
                    public string ext_3 {get; set;}
                    public string ext_4 {get; set;}
                    public string ext_5 {get; set;}                    

                    public string parent_id {get; set;}
                }
            }

            public string spec_cnt {get; set;}
            
            [Newtonsoft.Json.JsonConverter(typeof(SingleOrArrayConverter<Oary3>))]
            public List<Oary3>  oary3  { get; set; }
            public class Oary3{
                public string s_name {get; set;} 		    //       mtrl_prod_id
                public string s_value {get; set;} 			//       Substitude Material Product ID
            }
        }
    }
}