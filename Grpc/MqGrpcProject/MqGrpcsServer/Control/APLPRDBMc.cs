using MqGrpcProject;
using Grpc.Core;
using System;

namespace MqGrpcsServer
{
    public class APLPRDBMc:GlobalClass
    {
        public static APLPRDBM_Reply GetDatas(APLPRDBM_Request request, ServerCallContext context){
            APLPRDBM_Reply Result = null;
            string ErrMsg = "";
            string Body = "";
            string ServerIp = "";

            try
            {
                Body = GetBodyData(request);      
                ServerIp = MSMQ.GetMSMQServer(request.Serverip);
                if (ServerIp == "ERROR"){
                    Result = new APLPRDBM_Reply(){Errmsg = "Server IP Error!!"};
                }else{
                    APLPRDBM MSMQResult = MSMQ.SendRecive<APLPRDBM>(
                        ServerIp,
                        Body,
                        "APLPRDBM",
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

        private static String GetBodyData(APLPRDBM_Request request)
        {
            String Body = "";            
            Body = "<transaction>" +
                   "<trx_id>APLPRDBM</trx_id>" + 
                   "<type_id>I</type_id>" ;
            Body += GetField("product_id", request.Productid);
            Body += GetField("ec_code", request.Eccode);
            Body += GetField("route_cate", request.Routecate);
            Body += GetField("route_id", request.Routeid);
            Body += GetField("route_ver", request.Routever);

            Body += GetField("ope_id", request.Opeid);
            Body += GetField("ope_ver", request.Opever);
            Body += GetField("resv_eqpt_id", request.Resveqptid);
            Body += GetField("lot_id", request.Lotid);
            Body += GetField("nx_ope_no", request.Nxopeno);

            Body += GetField("use_recycle_flag", request.Userecycleflag);  
            Body += GetField("moveout_lenght", request.Moveoutlenght);        
            Body += @"</transaction>"; 
            Console.WriteLine(Body);
            return Body;
        }

        private static APLPRDBM_Reply GetResultData(APLPRDBM MSMQResult){
            APLPRDBM_Reply Result = null;
            Int32 bom_cnt = 0;
            Int32 mtrl_cnt = 0;
            Int32 spec_cnt = 0;
            try
            {
                if (MSMQResult != null){
                    Result = new APLPRDBM_Reply();
                    Result.Trxid = objtoStr(MSMQResult.transaction.trx_id, "");
                    Result.Typeid = objtoStr(MSMQResult.transaction.type_id, "");
                    Result.Rtncode = objtoStr(MSMQResult.transaction.rtn_code, "");
                    Result.Rtnmesg = objtoStr(MSMQResult.transaction.rtn_mesg, "");
                    Result.Productid = objtoStr(MSMQResult.transaction.product_id, "");

                    Result.Eccode = objtoStr(MSMQResult.transaction.ec_code, "");
                    Result.Routeid = objtoStr(MSMQResult.transaction.route_id, "");
                    Result.Routever = objtoStr(MSMQResult.transaction.route_ver, "");

                    bom_cnt = objtoInt32(MSMQResult.transaction.bom_cnt, 0);
                    Result.Bomcnt = bom_cnt.ToString();
                    
                    for (int idx = 0; idx < bom_cnt; idx ++){
                        APLPRDBMo_a1 obj = new APLPRDBMo_a1();
                        APLPRDBM.APLPRDBM_t.Oary oary = MSMQResult.transaction.oary1[idx];
                        obj.Opeid = objtoStr(oary.ope_id, "");
                        obj.Opever = objtoStr(oary.ope_ver, "");
                        obj.Bomid = objtoStr(oary.bom_id, "");
                        obj.Bomdsc= objtoStr(oary.bom_dsc, "");
                        obj.Stageid = objtoStr(oary.stage_id, "");

                        obj.Addtinfo1 = objtoStr(oary.addt_info_1, "");
                        obj.Addtinfo2 = objtoStr(oary.addt_info_2, "");
                        obj.Addtinfo3 = objtoStr(oary.addt_info_3, "");

                        mtrl_cnt = objtoInt32(oary.mtrl_cnt, 0);                                
                        obj.Mtrlcnt =  mtrl_cnt.ToString();

                        for (int idx2 = 0; idx2 < mtrl_cnt; idx2++)
                        {
                            APLPRDBM.APLPRDBM_t.Oary.Oary2 oary2 = oary.oary2[idx2];
                            APLPRDBMo_a2 obj2 = new APLPRDBMo_a2();
                            obj2.Mtrlproductid =  objtoStr(oary2.mtrl_product_id, "");
                            obj2.Mtrlproductdsc =  objtoStr(oary2.mtrl_product_dsc, "");
                            obj2.Mtrlcate =  objtoStr(oary2.mtrl_cate, "");
                            obj2.Planqty =  objtoStr(oary2.plan_qty, "");
                            obj2.Mtrlunit =  objtoStr(oary2.mtrl_unit, "");
                            
                            obj2.Spcfymtrllotid =  objtoStr(oary2.spcfy_mtrl_lot_id, "");
                            obj2.Ext1 =  objtoStr(oary2.ext_1, "");
                            obj2.Ext2 =  objtoStr(oary2.ext_2, "");
                            obj2.Ext3 =  objtoStr(oary2.ext_3, "");
                            obj2.Ext4 =  objtoStr(oary2.ext_4, "");

                            obj2.Ext5 =  objtoStr(oary2.ext_5, "");
                            obj2.Parentid =  objtoStr(oary2.parent_id, "");
                            obj.Oary2.Add(obj2);
                        }                        
                        Result.Oary1.Add(obj);
                    } 
                    spec_cnt = objtoInt32(MSMQResult.transaction.spec_cnt, 0);
                    for (int idx = 0; idx < spec_cnt; idx ++){
                        APLPRDBM.APLPRDBM_t.Oary3 oary3 = MSMQResult.transaction.oary3[idx];
                        APLPRDBMo_a obj3 = new APLPRDBMo_a();
                        obj3.Sname = objtoStr(oary3.s_name, "");
                        obj3.Svalue = objtoStr(oary3.s_value, "");
                        Result.Oary3.Add(obj3);
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