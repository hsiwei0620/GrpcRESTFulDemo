using MqGrpcProject;
using Grpc.Core;
using System;

namespace MqGrpcsServer
{
    public class APISHTEQc:GlobalClass
    {
        public static APISHTEQ_Reply GetDatas(APISHTEQ_Request request, ServerCallContext context){
            APISHTEQ_Reply Result = null;
            string ErrMsg = "";
            string Body = "";
            string ServerIp = "";

            try
            {
                Body = GetBodyData(request);      
                Console.WriteLine("MQ : " + Body);
                ServerIp = MSMQ.GetMSMQServer(request.Serverip);
                if (ServerIp == "ERROR"){
                    Result = new APISHTEQ_Reply(){Errmsg = "Server IP Error!!"};
                }else{
                    APISHTEQ MSMQResult = MSMQ.SendRecive<APISHTEQ>(
                        ServerIp,
                        Body,
                        "APISHTEQ",
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

        private static String GetBodyData(APISHTEQ_Request request)
        {
            String Body = "";
            Body = "<transaction>" +
                   "<trx_id>APISHTEQ</trx_id>" + 
                   "<type_id>I</type_id>" ;
            Body += GetField("eqpt_id", request.Eqptid);
            Body += GetField("sgr_id", request.Sgrid);
            Body += GetField("nx_ope_no", request.Nxopeno);
            Body += GetField("lot_id", request.Lotid);
            Body += @"</transaction>"; 
            return Body;
        }

        private static APISHTEQ_Reply GetResultData(APISHTEQ MSMQResult){
            APISHTEQ_Reply Result = null;
            Int32 sgr_cnt = 0;
            Int32 sht_ary_cnt = 0;
            try
            {
                if (MSMQResult != null){
                    Result = new APISHTEQ_Reply();
                    Result.Trxid =      objtoStr(MSMQResult.transaction.trx_id, "");
                    Result.Typeid =     objtoStr(MSMQResult.transaction.type_id, "");
                    Result.Rtncode =    objtoStr(MSMQResult.transaction.rtn_code, "");
                    Result.Rtnmesg =    objtoStr(MSMQResult.transaction.rtn_mesg, "");
                    Result.Eqptid =     objtoStr(MSMQResult.transaction.eqpt_id, "");
                    
                    Result.Eqptdsc =    objtoStr(MSMQResult.transaction.eqpt_dsc, "");
                    Result.Eqptcate =   objtoStr(MSMQResult.transaction.eqpt_cate, "");
                    Result.Recipeid =   objtoStr(MSMQResult.transaction.recipe_id, "");
                    Result.Eqptstat =   objtoStr(MSMQResult.transaction.eqpt_stat, "");
                    Result.Eqptstatdsc =objtoStr(MSMQResult.transaction.eqpt_stat_dsc, "");
                    Result.Eqptmode =   objtoStr(MSMQResult.transaction.eqpt_mode, "");

                    sgr_cnt =   objtoInt32(MSMQResult.transaction.sgr_cnt, 0); 
                    Result.Sgrcnt = sgr_cnt.ToString();
                    
                    for (int idx = 0; idx < sgr_cnt; idx ++){
                        APISHTEQo_a1 obj = new APISHTEQo_a1();
                        obj.Sgrid = objtoStr(MSMQResult.transaction.oary1[idx].sgr_id, "");
                        
                        sht_ary_cnt = objtoInt32(MSMQResult.transaction.oary1[idx].sht_ary_cnt, 0);

                        obj.Shtarycnt = sht_ary_cnt.ToString();
                        
                        for (int idx2 = 0; idx2 < sht_ary_cnt; idx2 ++){
                            APISHTEQo_a2 obj2 = new APISHTEQo_a2();
                            APISHTEQ.APISHTEQ_t.Oary2 oary2 = MSMQResult.transaction.oary1[idx].oary2[idx2];
                            obj2.Shtid = objtoStr(oary2.sht_id, "");
                            obj2.Shtstat = objtoStr(oary2.sht_stat, "");
                            obj2.Lotid = objtoStr(oary2.lot_id, "");
                            obj2.Spltid = objtoStr(oary2.splt_id, "");
                            obj2.Nxroutecate = objtoStr(oary2.nx_route_cate, "");
                            obj2.Nxrouteid = objtoStr(oary2.nx_route_id, "");
                            obj2.Nxroutever = objtoStr(oary2.nx_route_ver, "");
                            obj2.Cropeno = objtoStr(oary2.cr_ope_no, "");
                            obj2.Nxopeno = objtoStr(oary2.nx_ope_no, "");
                            obj2.Nxopeid = objtoStr(oary2.nx_ope_id, "");
                            obj2.Nxopever = objtoStr(oary2.nx_ope_ver, "");
                            obj2.Nxopedsc = objtoStr(oary2.nx_ope_dsc, "");
                            obj2.Nxpeplvl = objtoStr(oary2.nx_pep_lvl, "");
                            obj2.Nxrecipeid = objtoStr(oary2.nx_recipe_id, "");
                            obj2.Nxprocid = objtoStr(oary2.nx_proc_id, "");
                            obj2.Productid = objtoStr(oary2.product_id, "");
                            obj2.Eccode = objtoStr(oary2.ec_code, "");
                            obj2.Unit = objtoStr(oary2.unit, "");
                            obj2.Sgrid = objtoStr(oary2.sgr_id, "");

                            Int32 Shtcnt2 = objtoInt32(oary2.sht_cnt, 0);
                            obj2.Shtcnt = Shtcnt2.ToString();
                            obj2.Moveinflg = objtoStr(oary2.move_in_flg, "");
                            obj2.Slotno = objtoStr(oary2.slot_no, "");
                            obj2.Showflag = objtoStr(oary2.show_flag, "");
                            obj2.Fakeid = objtoStr(oary2.fake_id, "");
                            obj2.Erplotno = objtoStr(oary2.erp_lot_no, "");
                            obj.Oary2.Add(obj2);
                        }
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