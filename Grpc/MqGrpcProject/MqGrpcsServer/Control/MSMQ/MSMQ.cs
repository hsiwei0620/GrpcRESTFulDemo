using System;
using System.Collections;
using System.Xml;
using Experimental.System.Messaging;
using Newtonsoft.Json;

namespace MqGrpcsServer
{
    public class MSMQ
    {
        const String ShareReply = @"\private$\SHARE.REPLY";     //MES MSMQ回覆
        const String MsmqPath = @"\private$\KD1.";
        const String QueueName = "";

        private String _ErrMsg = "";
        public String ErrMsg
        {
            get { return this._ErrMsg; }
            set { this._ErrMsg = value; }
        }

        
        public static String GetMSMQServer(string Ip)
        {
            String Result = "";
            switch (Ip)
            {
                case "P":
                    Result = "FormatName:Direct=TCP:192.1.1.185";
                    break;
                case "T":
                    Result = "FormatName:Direct=TCP:192.1.1.180";
                    break;
                case "D":
                    Result = "FormatName:Direct=TCP:192.1.1.181";
                    break;
                default:
                    if (Ip.Split(".").Length == 4){
                        Result = "FormatName:Direct=TCP:" + Ip;
                    }else{
                        Result = "ERROR";
                    }
                    break;
            }
            return Result;
        }
        
        
        public static T SendRecive<T>(String ServerIp, String Body, String QueueName, String Kind, ref String ErrMsg)
        {
            XmlDocument ResultXml = null;
            Queue OpiShareReply = new Queue(ServerIp + ShareReply);
            Message Msg = new Message();
            Msg.Formatter = new StringMessageFormatter();
            Msg.Body = Body;
            Queue objMSMQ = new Queue(ServerIp + MsmqPath + QueueName + Kind);
            ResultXml = objMSMQ.sendReceive(Msg, OpiShareReply);
            ErrMsg += objMSMQ.ErrMsg + Environment.NewLine;
            objMSMQ.Dispose();
            OpiShareReply.Dispose();
            if (ResultXml != null && ResultXml.LastChild != null)
            {
                XmlNode responseNode = ResultXml.SelectSingleNode("/transaction");
                // 透過Json.NET將XmlNode轉為Json格式字串
                string jsonText = JsonConvert.SerializeXmlNode(responseNode);   
                // 透過Json.NET反序列化為物件
                T responseObj = JsonConvert.DeserializeObject<T>(jsonText);
                return (T)responseObj;
            }
            else
            {
                return default(T);
            }
        }        
    }    
}
