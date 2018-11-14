using Newtonsoft.Json;
using System.Collections.Generic;

namespace MqGrpcsServer
{
    public class APIMTLST
    {
        public APIMTLST_t transaction { get; set; }

        public class APIMTLST_t{
            public string trx_id {get; set;} 				//       Transaction ID
            public string type_id {get; set;} 				//       input / output
            public string rtn_code {get; set;} 				//       return code
            public string rtn_mesg {get; set;} 				//       return message
            public string mtrl_cnt {get; set;} 				//       Material Product Count

            [Newtonsoft.Json.JsonConverter(typeof(SingleOrArrayConverter<Oary>))]
            public List<Oary>  oary  { get; set; }

            public string mas_cnt {get; set;}
            [Newtonsoft.Json.JsonConverter(typeof(SingleOrArrayConverter<Oary2>))]
            public List<Oary2>  oary2  { get; set; }

            public class Oary{
                public string mtrl_product_id {get; set;} 		    //       Material Product ID
                public string mtrl_lot_id {get; set;} 				//       Material Lot ID
                public string seq_no {get; set;} 				    //       Sequence NO
                public string container_id {get; set;} 				//       M1.00 CONTAINER ID
                public string mtrl_cate {get; set;} 				//       Material Category
                public string mtrl_stat {get; set;} 				//       Material Status
                public string mtrl_qty {get; set;} 				    //       Material Lot Count
                public string chg_date {get; set;} 				    //       Change Date
                public string chg_time {get; set;} 				    //       Change Time
                public string chg_user {get; set;} 				    //       Change User ID
                public string tar_eqpt_id {get; set;} 				//       Target Equipment ID( Sometimes Unit ID )
                public string eqpt_id {get; set;} 				    //       Main Equipment ID
                public string eqpt_mtg {get; set;} 				    //       batch id for equipment( auto assign by MES )
                public string comment {get; set;} 				    //       Comment
                public string msk_position_id {get; set;}           //       Position ID
                public string data_rep_date_time {get; set;} 	    //       Data update date time
                public string oos_flg {get; set;} 				    //       OOS Falg
                public string create_date {get; set;} 				//       Create date
                public string expire_date {get; set;} 
                public string valid_period {get; set;} 				//       day
                public string min_use_qty {get; set;} 	            //Hsiwei Add min_use_qty
                public string sales_order {get; set;} 
                public string line_id {get; set;} 
                public string enough_flg {get; set;} 
                public string lot_id {get; set;} 
                public string sheets {get; set;} 
            }
            public class Oary2{
                public string mtrl_prod_id {get; set;} 				//       mtrl_prod_id
                public string sub_mtrl_prod_id {get; set;} 			//       Substitude Material Product ID
                public string proportion {get; set;} 				//       %
                public string user_id {get; set;} 				    //       user_id
                public string chg_date {get; set;} 				    //       chg_date
                public string chg_time {get; set;} 				    //       chg_time
            }
        }
    }
}