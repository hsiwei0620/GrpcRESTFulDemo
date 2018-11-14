using MqGrpcProject;
using Grpc.Core;
using System;

namespace MqGrpcsServer
{
    public class APLRSVPRc:GlobalClass
    {
        public static APLRSVPR_Reply GetDatas(APLRSVPR_Request request, ServerCallContext context){
            APLRSVPR_Reply Result = null;
            string ErrMsg = "";
            string Body = "";
            string ServerIp = "";

            try
            {
                Body = GetBodyData(request);      
                Console.WriteLine("MQ : " + Body);
                ServerIp = MSMQ.GetMSMQServer(request.Serverip);
                if (ServerIp == "ERROR"){
                    Result = new APLRSVPR_Reply(){Errmsg = "Server IP Error!!"};
                }else{
                    APLRSVPR MSMQResult = MSMQ.SendRecive<APLRSVPR>(
                        ServerIp,
                        Body,
                        "APLRSVPR",
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

        private static String GetBodyData(APLRSVPR_Request request)
        {
            String Body = "";
            Body = "<transaction>" +
                   "<trx_id>APLRSVPR</trx_id>" + 
                   "<type_id>I</type_id>" ;
            Body += GetField("bay_id", request.Bayid);
            Body += GetField("resv_eqpt_id", request.Resveqptid);
            Body += GetField("resv_date", request.Resvdate);
            Body += GetField("resv_shift_seq", request.Resvshiftseq);
            Body += GetField("prep_start_date", request.Prepstartdate);

            Body += GetField("prep_end_date", request.Prependdate);
            Body += GetField("prep_type", request.Preptype);
            Body += GetField("lot_id", request.Lotid);
            Body += GetField("only_outside_flg", request.Onlyoutsideflg);
            Body += GetField("need_order_flg", request.Needorderflg);
            Body += @"</transaction>"; 
            return Body;
        }

        private static APLRSVPR_Reply GetResultData(APLRSVPR MSMQResult){
            APLRSVPR_Reply Result = null;
            Int32 lot_ary_cnt = 0;
            try
            {
                if (MSMQResult != null){
                    Result = new APLRSVPR_Reply();
                    Result.Trxid =      objtoStr(MSMQResult.transaction.trx_id, "");
                    Result.Typeid =     objtoStr(MSMQResult.transaction.type_id, "");
                    Result.Rtncode =    objtoStr(MSMQResult.transaction.rtn_code, "");
                    Result.Rtnmesg =    objtoStr(MSMQResult.transaction.rtn_mesg, "");
                    
                    lot_ary_cnt =   objtoInt32(MSMQResult.transaction.lot_ary_cnt, 0); 
                    Result.Lotarycnt = lot_ary_cnt.ToString();
                    
                    for (int idx = 0; idx < lot_ary_cnt; idx ++){
                        APLRSVPRo_a obj = new APLRSVPRo_a();
                        APLRSVPR.APLRSVPR_t.Oary oary = MSMQResult.transaction.oary[idx];

                        obj.Lotid = objtoStr(oary.lot_id, "");
                        obj.Lotstat = objtoStr(oary.lot_stat, "");
                        obj.Nxopeno = objtoStr(oary.nx_ope_no, "");
                        obj.Mtrlproductid = objtoStr(oary.mtrl_product_id, "");
                        obj.Preptype = objtoStr(oary.prep_type, "");

                        obj.Prepstat = objtoStr(oary.prep_stat, "");
                        obj.Prepdate = objtoStr(oary.prep_date, "");
                        obj.Prepseqno = objtoStr(oary.prep_seq_no, "");
                        obj.Resvdate = objtoStr(oary.resv_date, "");
                        obj.Resvshiftseq = objtoStr(oary.resv_shift_seq, "");

                        obj.Routeid = objtoStr(oary.route_id, "");
                        obj.Routever = objtoStr(oary.route_ver, "");
                        obj.Claimdate = objtoStr(oary.claim_date, "");
                        obj.Claimtime = objtoStr(oary.claim_time, "");
                        obj.Claimuser = objtoStr(oary.claim_user, "");

                        obj.Nxopeid = objtoStr(oary.nx_ope_id, "");
                        obj.Nxopever = objtoStr(oary.nx_ope_ver, "");
                        obj.Spltid = objtoStr(oary.splt_id, "");
                        obj.Resveqptid = objtoStr(oary.resv_eqpt_id, "");
                        obj.Shtcnt = objtoInt32(oary.sht_cnt, 0).ToString();

                        obj.Readyshtcnt = objtoInt32(oary.ready_sht_cnt, 0).ToString();
                        obj.Productid = objtoStr(oary.product_id, "");                        
                        obj.Eccode = objtoStr(oary.ec_code, "");
                        obj.Mainrouteid = objtoStr(oary.main_route_id, "");
                        obj.Mainroutever = objtoStr(oary.main_route_ver, "");

                        obj.Cropeno = objtoStr(oary.cr_ope_no, "");
                        obj.Lineid = objtoStr(oary.line_id, "");

                        Result.Oary.Add(obj);
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