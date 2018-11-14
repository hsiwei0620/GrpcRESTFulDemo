using MqGrpcProject;
using Grpc.Core;
using System;

namespace MqGrpcsServer
{
    public class APMEQRSVc:GlobalClass
    {
        public static APMEQRSV_Reply GetDatas(APMEQRSV_Request request, ServerCallContext context){
            APMEQRSV_Reply Result = null;
            string ErrMsg = "";
            string Body = "";
            string ServerIp = "";

            try
            {
                Body = GetBodyData(request);      
                ServerIp = MSMQ.GetMSMQServer(request.Serverip);
                if (ServerIp == "ERROR"){
                    Result = new APMEQRSV_Reply(){Errmsg = "Server IP Error!!"};
                }else{
                    APMEQRSV MSMQResult = MSMQ.SendRecive<APMEQRSV>(
                        ServerIp,
                        Body,
                        "APMEQRSV",
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

        private static String GetBodyData(APMEQRSV_Request request)
        {
            String Body = "";
            Int32 iarys = 0;        //Lotarycnt

            Body = "<transaction>" +
                   "<trx_id>APMEQRSV</trx_id>" + 
                   "<type_id>I</type_id>" ;
            Body += GetField("action_type", request.Actiontype);
            Body += GetField("fab_id", request.Fabid);
            Body += GetField("bay_id", request.Bayid);
            Body += GetField("resv_eqpt_id", request.Resveqptid);
            Body += GetField("clm_user", request.Clmuser);
            Body += GetField("lot_id", request.Lotid);
            Body += GetField("cr_sht_cnt_flg", request.Crshtcntflg);
            Body += GetField("only_resv_flg", request.Onlyresvflg);
            Body += GetField("only_outuside_flg", request.Onlyoutusideflg);
            
            iarys = objtoInt32(request.Lotarycnt);
            if (iarys > 0){
                for(int idx=0; idx < iarys; idx++){
                    Body += GetField("lot_ary_cnt", request.Lotarycnt);
                    Body += "<iary>";
                    Body += GetField("lot_id", request.Iary[idx].Lotid);
                    Body += GetField("nx_ope_no", request.Iary[idx].Nxopeno);
                    Body += GetField("nx_ope_id", request.Iary[idx].Nxopeid);
                    Body += GetField("nx_ope_ver", request.Iary[idx].Nxopever);
                    Body += GetField("splt_id", request.Iary[idx].Spltid);
                    Body += GetField("resv_eqpt_id", request.Iary[idx].Resveqptid);
                    Body += GetField("lot_stat", request.Iary[idx].Lotstat);
                    Body += GetField("resv_date", request.Iary[idx].Resvdate);
                    Body += GetField("resv_shift_seq", request.Iary[idx].Resvshiftseq);
                    Body += GetField("resv_comment", request.Iary[idx].Resvcomment);
                    Body += GetField("plan_opt_weight", request.Iary[idx].Planoptweight);
                    Body += GetField("move_out_weight", request.Iary[idx].Moveoutweight);
                    Body += GetField("sht_cnt", request.Iary[idx].Shtcnt);
                    Body += GetField("cr_sht_cnt", request.Iary[idx].Crshtcnt);
                    Body += GetField("run_flag", request.Iary[idx].Runflag);
                    
                    Body += GetField("product_id", request.Iary[idx].Productid);
                    Body += GetField("ec_code", request.Iary[idx].Eccode);
                    Body += GetField("route_id", request.Iary[idx].Routeid);
                    Body += GetField("route_ver", request.Iary[idx].Routever);
                    Body += GetField("cr_ope_no", request.Iary[idx].Cropeno);
                    Body += GetField("cr_recipe_idope_no", request.Iary[idx].Recipeid);
                    Body += GetField("in_sht_cnt", request.Iary[idx].Inshtcnt);
                    Body += GetField("std_ld_time", request.Iary[idx].Stdldtime);
                    Body += GetField("man_ope_time", request.Iary[idx].Manopetime);
                    Body += GetField("piece", request.Iary[idx].Piece);
                    
                    Body += GetField("ope_no_flag", request.Iary[idx].Openoflag);
                    Body += GetField("key", request.Iary[idx].Key);
                    Body += GetField("type", request.Iary[idx].Type);
                    Body += @"</iary>";
                }
            }
            Body += @"</transaction>"; 
            return Body;
        }

        private static APMEQRSV_Reply GetResultData(APMEQRSV MSMQResult){
            APMEQRSV_Reply Result = null;
            Int32 lot_ary_cnt = 0;
            Int32 no_rsv_cnt = 0;
            try
            {
                if (MSMQResult != null){
                    Result = new APMEQRSV_Reply();
                    Result.Trxid =      objtoStr(MSMQResult.transaction.trx_id, "");
                    Result.Typeid =     objtoStr(MSMQResult.transaction.type_id, "");
                    Result.Rtncode =    objtoStr(MSMQResult.transaction.rtn_code, "");
                    Result.Rtnmesg =    objtoStr(MSMQResult.transaction.rtn_mesg, "");

                    lot_ary_cnt =   objtoInt32(MSMQResult.transaction.lot_ary_cnt, 0);
                    no_rsv_cnt =    objtoInt32(MSMQResult.transaction.no_rsv_cnt, 0);

                    Result.Lotarycnt = lot_ary_cnt.ToString();
                    Result.Norsvcnt = no_rsv_cnt.ToString();
                    
                    for (int idx = 0; idx < lot_ary_cnt; idx ++){
                        APMEQRSVo_a1 obj = new APMEQRSVo_a1();
                        obj.Lotid =         objtoStr(MSMQResult.transaction.oary[idx].lot_id, "");
                        obj.Nxopeno =       objtoStr(MSMQResult.transaction.oary[idx].nx_ope_no, "");
                        obj.Nxopeid=        objtoStr(MSMQResult.transaction.oary[idx].nx_ope_id, "");
                        obj.Nxopever =      objtoStr(MSMQResult.transaction.oary[idx].nx_ope_ver, "");
                        obj.Spltid =        objtoStr(MSMQResult.transaction.oary[idx].splt_id, "");
                        obj.Resveqptid =    objtoStr(MSMQResult.transaction.oary[idx].resv_eqpt_id, "");
                        obj.Lotstat =       objtoStr(MSMQResult.transaction.oary[idx].lot_stat, "");
                        obj.Resvdate =      objtoStr(MSMQResult.transaction.oary[idx].resv_date, "");
                        obj.Resvshiftseq =  objtoStr(MSMQResult.transaction.oary[idx].resv_shift_seq, "");
                        obj.Resvcomment =   objtoStr(MSMQResult.transaction.oary[idx].resv_comment, "");
                        obj.Planoptweight = objtoStr(MSMQResult.transaction.oary[idx].plan_opt_weight, "");
                        obj.Moveoutweight = objtoStr(MSMQResult.transaction.oary[idx].move_out_weight, "");
                        obj.Shtcnt =        objtoStr(MSMQResult.transaction.oary[idx].sht_cnt, "");
                        obj.Crshtcnt =      objtoStr(MSMQResult.transaction.oary[idx].cr_sht_cnt, "");
                        obj.Runflag =       objtoStr(MSMQResult.transaction.oary[idx].run_flag, "");
                        obj.Productid =     objtoStr(MSMQResult.transaction.oary[idx].product_id, "");
                        obj.Eccode =        objtoStr(MSMQResult.transaction.oary[idx].ec_code, "");
                        obj.Routeid =       objtoStr(MSMQResult.transaction.oary[idx].route_id, "");
                        obj.Routever =      objtoStr(MSMQResult.transaction.oary[idx].route_ver, "");
                        obj.Cropeno =       objtoStr(MSMQResult.transaction.oary[idx].cr_ope_no, "");
                        obj.Recipeid =      objtoStr(MSMQResult.transaction.oary[idx].recipe_id, "");
                        obj.Inshtcnt =      objtoStr(MSMQResult.transaction.oary[idx].in_sht_cnt, "");
                        obj.Stdldtime =     objtoStr(MSMQResult.transaction.oary[idx].std_ld_time, "");
                        obj.Manopetime =    objtoStr(MSMQResult.transaction.oary[idx].man_ope_time, "");
                        obj.Piece =         objtoStr(MSMQResult.transaction.oary[idx].piece, "");
                        obj.Openoflag =     objtoStr(MSMQResult.transaction.oary[idx].ope_no_flag, "");
                        obj.Key =           objtoStr(MSMQResult.transaction.oary[idx].key, "");
                        obj.Type =          objtoStr(MSMQResult.transaction.oary[idx].type, "");
                        Result.Oary.Add(obj);
                    }
                    for (int idx = 0; idx <  no_rsv_cnt; idx ++){
                        APMEQRSVo_a2 obj = new APMEQRSVo_a2();
                        obj.Lotid =         objtoStr(MSMQResult.transaction.oary1[idx].lot_id, "");
                        obj.Productid =     objtoStr(MSMQResult.transaction.oary1[idx].product_id, "");
                        obj.Eccode =        objtoStr(MSMQResult.transaction.oary1[idx].ec_code, "");
                        obj.Planoptweight = objtoStr(MSMQResult.transaction.oary1[idx].plan_opt_weight, "");
                        obj.Shtcnt =        objtoStr(MSMQResult.transaction.oary1[idx].sht_cnt, "");
                        obj.Routeid =       objtoStr(MSMQResult.transaction.oary1[idx].route_id, "");
                        obj.Routever =      objtoStr(MSMQResult.transaction.oary1[idx].route_ver, "");
                        obj.Cropeno =       objtoStr(MSMQResult.transaction.oary1[idx].cr_ope_no, "");
                        obj.Nxopeno =       objtoStr(MSMQResult.transaction.oary1[idx].nx_ope_no, "");
                        obj.Nxopeid =       objtoStr(MSMQResult.transaction.oary1[idx].nx_ope_id, "");
                        obj.Nxopever =      objtoStr(MSMQResult.transaction.oary1[idx].nx_ope_ver, "");
                        obj.Openoflag =     objtoStr(MSMQResult.transaction.oary1[idx].ope_no_flag, "");
                        obj.Stdldtime =     objtoStr(MSMQResult.transaction.oary1[idx].std_ld_time, "");
                        obj.Manopetime =    objtoStr(MSMQResult.transaction.oary1[idx].man_ope_time, "");
                        obj.Type =          objtoStr(MSMQResult.transaction.oary1[idx].type, "");
                        obj.Fiteqpts =      objtoStr(MSMQResult.transaction.oary1[idx].fit_eqpts, "");
                        Result.Oary1.Add(obj);
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