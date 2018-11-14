using MqGrpcProject;
using Grpc.Core;
using System;

namespace MqGrpcsServer
{
    public class APIMTLSTc:GlobalClass
    {
        public static APIMTLST_Reply GetDatas(APIMTLST_Request request, ServerCallContext context){
            APIMTLST_Reply Result = null;
            string ErrMsg = "";
            string Body = "";
            string ServerIp = "";

            try
            {
                Body = GetBodyData(request);      
                ServerIp = MSMQ.GetMSMQServer(request.Serverip);
                if (ServerIp == "ERROR"){
                    Result = new APIMTLST_Reply(){Errmsg = "Server IP Error!!"};
                }else{
                    APIMTLST MSMQResult = MSMQ.SendRecive<APIMTLST>(
                        ServerIp,
                        Body,
                        "APIMTLST",
                        "I",
                        ref ErrMsg);
                    Result = GetResultData(MSMQResult);
                }
            }
            catch (System.Exception excp)
            {
                Console.WriteLine(GetMethodName() +"ErrMsg:" + excp.Message.ToString());
            }     
            return Result;
        }

        private static String GetBodyData(APIMTLST_Request request)
        {
            String Body = "";            
            Body = "<transaction>" +
                   "<trx_id>APIMTLST</trx_id>" + 
                   "<type_id>I</type_id>" ;
            Body += GetField("mtrl_cate", request.Mtrlcate);
            Body += GetField("mtrl_product_id", request.Mtrlproductid);
            Body += GetField("mtrl_lot_id", request.Mtrllotid);
            Body += GetField("mtrl_stat", request.Mtrlstat);
            Body += GetField("mtrl_position_id", request.Mtrlpositionid);

            Body += GetField("con_eqpt_id", request.Coneqptid);
            Body += GetField("sales_order", request.Salesorder);
            Body += GetField("line_id", request.Lineid);
            Body += GetField("create_date", request.Createdate);
            Body += GetField("lot_id", request.Lotid);

            Body += GetField("nx_ope_no", request.Nxopeno);
            Body += GetField("query_sub_mtrl", request.Querysubmtrl);         
            Body += @"</transaction>"; 
            return Body;
        }

        private static APIMTLST_Reply GetResultData(APIMTLST MSMQResult){
            APIMTLST_Reply Result = null;
            Int32 mtrl_cnt = 0;
            Int32 mas_cnt = 0;
            try
            {
                if (MSMQResult != null){
                    Result = new APIMTLST_Reply();
                    Result.Trxid = objtoStr(MSMQResult.transaction.trx_id, "");
                    Result.Typeid = objtoStr(MSMQResult.transaction.type_id, "");
                    Result.Rtncode = objtoStr(MSMQResult.transaction.rtn_code, "");
                    Result.Rtnmesg = objtoStr(MSMQResult.transaction.rtn_mesg, "");

                    mtrl_cnt = objtoInt32(MSMQResult.transaction.mtrl_cnt, 0);
                    mas_cnt = objtoInt32(MSMQResult.transaction.mas_cnt, 0);

                    Result.Mtrlcnt = mtrl_cnt.ToString();
                    Result.Mascnt = mas_cnt.ToString();
                    
                    for (int idx = 0; idx < mtrl_cnt; idx ++){
                        APIMTLSTo_a obj = new APIMTLSTo_a();
                        APIMTLST.APIMTLST_t.Oary oary = MSMQResult.transaction.oary[idx];
                        obj.Mtrlproductid = objtoStr(oary.mtrl_product_id, "");
                        obj.Mtrllotid = objtoStr(oary.mtrl_lot_id, "");
                        obj.Seqno= objtoStr(oary.seq_no, "");
                        obj.Containerid = objtoStr(oary.container_id, "");
                        obj.Mtrlcate = objtoStr(oary.mtrl_cate, "");

                        obj.Mtrlstat = objtoStr(oary.mtrl_stat, "");
                        obj.Mtrlqty = objtoStr(oary.mtrl_qty, "");
                        obj.Chgdate = objtoStr(oary.chg_date, "");
                        obj.Chgtime = objtoStr(oary.chg_time, "");
                        obj.Chguser = objtoStr(oary.chg_user, "");

                        obj.Tareqptid = objtoStr(oary.tar_eqpt_id, "");
                        obj.Eqptid = objtoStr(oary.eqpt_id, "");
                        obj.Eqptmtg = objtoStr(oary.eqpt_mtg, "");
                        obj.Comment = objtoStr(oary.comment, "");
                        obj.Mskpositionid = objtoStr(oary.msk_position_id, "");

                        obj.Datarepdatetime = objtoStr(oary.data_rep_date_time, "");
                        obj.Oosflg = objtoStr(oary.oos_flg, "");
                        obj.Createdate = objtoStr(oary.create_date, "");
                        obj.Expiredate = objtoStr(oary.expire_date, "");
                        obj.Validperiod = objtoStr(oary.valid_period, "");

                        obj.Minuseqty = objtoStr(oary.min_use_qty, "");
                        obj.Salesorder = objtoStr(oary.sales_order, "");
                        obj.Lineid = objtoStr(oary.line_id, "");
                        obj.Enoughflg = objtoStr(oary.enough_flg, "");
                        obj.Lotid = objtoStr(oary.lot_id, "");

                        obj.Sheets = objtoStr(oary.sheets, "");
                        Result.Oary.Add(obj);
                    }
                    for (int idx = 0; idx <  mas_cnt; idx ++){
                        APIMTLSTo_a2 obj = new APIMTLSTo_a2();
                        APIMTLST.APIMTLST_t.Oary2 oary2 = MSMQResult.transaction.oary2[idx];
                        obj.Mtrlprodid = objtoStr(oary2.mtrl_prod_id, "");
                        obj.Submtrlprodid = objtoStr(oary2.sub_mtrl_prod_id, "");
                        obj.Proportion = objtoStr(oary2.proportion, "");
                        obj.Userid = objtoStr(oary2.user_id, "");
                        obj.Chgdate = objtoStr(oary2.chg_date, "");
                        obj.Chgtime = objtoStr(oary2.chg_time, "");

                        Result.Oary2.Add(obj);
                    }        
                }    
            }
            catch (System.Exception excp)
            {
                Console.WriteLine(GetErrMsg(excp, "", GetMethodName()));
            }
            return Result;
        }
    }
}