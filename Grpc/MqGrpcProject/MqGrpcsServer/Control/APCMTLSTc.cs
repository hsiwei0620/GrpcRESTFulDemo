using MqGrpcProject;
using Grpc.Core;
using System;

namespace MqGrpcsServer
{
    public class APCMTLSTc:GlobalClass
    {
        public static APCMTLST_Reply GetDatas(APCMTLST_Request request, ServerCallContext context){
            APCMTLST_Reply Result = null;
            string ErrMsg = "";
            string Body = "";
            string ServerIp = "";

            try
            {
                Body = GetBodyData(request);      
                ServerIp = MSMQ.GetMSMQServer(request.Serverip);
                if (ServerIp == "ERROR"){
                    Result = new APCMTLST_Reply(){Errmsg = "Server IP Error!!"};
                }else{
                    APCMTLST MSMQResult = MSMQ.SendRecive<APCMTLST>(
                        ServerIp,
                        Body,
                        "APCMTLST",
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

        private static String GetBodyData(APCMTLST_Request request)
        {
            String Body = "";  
            Int32 iarys = 0;
            Int32 iarys2 = 0;       
            Body = "<transaction>" +
                   "<trx_id>APCMTLST</trx_id>" + 
                   "<type_id>I</type_id>" ;
            Body += GetField("clm_mtst_typ", request.Clmmtsttyp);
            Body += GetField("clm_date", request.Clmdate);
            Body += GetField("clm_time", request.Clmtime);
            Body += GetField("user_id", request.Userid);
            Body += GetField("bom_id", request.Bomid);

            iarys = objtoInt32(request.Mtrlcnt);
            Console.WriteLine(iarys.ToString());
            Console.WriteLine(request.ToString());
            if (iarys > 0){
                Body += GetField("mtrl_cnt", request.Mtrlcnt);
                for(int idx=0; idx < iarys; idx++){
                    Console.WriteLine("idx:" + idx.ToString());
                    Console.WriteLine("iarys.length:" + request.Iary[idx].ToString());
                    Body += "<iary>";
                    Body += GetField("mtrl_product_id", request.Iary[idx].Mtrlproductid);
                    Body += GetField("mtrl_lot_id", request.Iary[idx].Mtrllotid);
                    Body += GetField("container_id", request.Iary[idx].Containerid);
                    Body += GetField("mtrl_stat", request.Iary[idx].Mtrlstat);
                    Body += GetField("mtrl_qty", request.Iary[idx].Mtrlqty);

                    Body += GetField("per_sht_qty", request.Iary[idx].Pershtqty);
                    Body += GetField("sht_cnt", request.Iary[idx].Shtcnt);
                    Body += GetField("comment", request.Iary[idx].Comment);
                    Body += GetField("scp_rsn_cate", request.Iary[idx].Scprsncate);
                    Body += GetField("scp_rsn_code", request.Iary[idx].Scprsncode);

                    Body += GetField("dept_code", request.Iary[idx].Deptcode);
                    Body += GetField("scp_qty", request.Iary[idx].Scpqty);
                    Body += GetField("tar_eqpt_id", request.Iary[idx].Tareqptid);
                    Body += GetField("create_date", request.Iary[idx].Createdate);
                    Body += GetField("expire_date", request.Iary[idx].Expiredate);
                    
                    Body += GetField("valid_period", request.Iary[idx].Validperiod);
                    Body += GetField("min_use_qty", request.Iary[idx].Minuseqty);
                    Body += GetField("flag", request.Iary[idx].Flag);
                    Body += GetField("sales_order", request.Iary[idx].Salesorder);
                    Body += GetField("line_id", request.Iary[idx].Lineid);
                    
                    iarys2 = objtoInt32(request.Iary[idx].Iary2Cnt);
                    Console.WriteLine("  iarys2:" + iarys2.ToString());
                    if (iarys2 > 0){
                        Body += GetField("iary2_cnt", request.Iary[idx].Iary2Cnt);
                        for(int idx2=0; idx2 < iarys2; idx2++){
                            Console.WriteLine("  idx2:" + idx2.ToString());
                            Body += "<iary2>";
                            Body += GetField("use_mtrl_product_id", request.Iary[idx].Iary2[idx2].Usemtrlproductid);
                            Body += GetField("use_mtrl_lot_id", request.Iary[idx].Iary2[idx2].Usemtrllotid);
                            Body += GetField("use_qty", request.Iary[idx].Iary2[idx2].Useqty);
                            Body += GetField("use_seq_no", request.Iary[idx].Iary2[idx2].Useseqno);
                            Body += GetField("lot_id", request.Iary[idx].Iary2[idx2].Lotid);

                            Body += @"</iary2>";
                        }
                    }
                    Body += @"</iary>";
                }
            }       
            Body += @"</transaction>"; 
            Console.WriteLine(Body);
            return Body;
        }

        private static APCMTLST_Reply GetResultData(APCMTLST MSMQResult){
            APCMTLST_Reply Result = null;
            try
            {
                if (MSMQResult != null){
                    Result = new APCMTLST_Reply();
                    Result.Trxid = objtoStr(MSMQResult.transaction.trx_id, "");
                    Result.Typeid = objtoStr(MSMQResult.transaction.type_id, "");
                    Result.Rtncode = objtoStr(MSMQResult.transaction.rtn_code, "");
                    Result.Rtnmesg = objtoStr(MSMQResult.transaction.rtn_mesg, "");                    
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