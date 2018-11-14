using System;
using Experimental.System.Messaging;
using System.Xml;
using System.Text;

namespace MqGrpcsServer
{
   public  class Queue
    {
        public MessageQueue mq;
        public string sReceiveData;
        public string sCorrelationId, sMsgId;
        public string _ErrMsg = "";

        public string ErrMsg{
            get { return this._ErrMsg;}
            set { this._ErrMsg = value;}
        }

        public Queue(String QueuePath)
        {
            try
            {
                //將MessageQueue物件指向 指定的MSMQ
                mq = new MessageQueue(QueuePath);
                mq.Formatter = new StringMessageFormatter();
                mq.MessageReadPropertyFilter.CorrelationId = true;
                mq.Refresh();
                sReceiveData = "";
            }
            catch (Exception ex)
            {
                this.ErrMsg = "queue ERROR " + ex.Message.ToString() + Environment.NewLine;
            }
        }

        public void SendByLabel(Message msgBody, string sLabel)
        {
            try
            {
                mq.Send(msgBody, sLabel);
            }
            catch (Exception ex)
            {
                this.ErrMsg = "SendByLabel ERROR " + ex.Message.ToString() + Environment.NewLine;
            }
        }

        public void SendByText(string msgBody)
        {
            try
            {
                mq.Send(msgBody);
            }
            catch (Exception ex)
            {
                this.ErrMsg = "SendByText ERROR " + ex.Message.ToString() + Environment.NewLine;
            }
        }

        public bool Peek()
        {
            try
            {
                TimeSpan timeout = new TimeSpan(0, 0, 1); //設定timeout
                if (mq.Peek(timeout) != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (MessageQueueException ex)
            {
                Console.WriteLine(ex.ErrorCode);
                if (ex.ErrorCode == -2147467259) //已無訊息佇列
                {
                    return false;
                }
                else
                {
                    this.ErrMsg = "Peek ERROR " + ex.Message.ToString() + Environment.NewLine;
                    throw ex;
                }
            }
        }

        public void Receive()
        {
            Receive("");
        }

        public void Receive(string strCorrelationID)
        {
            try
            {
                Message msg;
                TimeSpan timeout = new TimeSpan(0, 0, 60); //設定timeout

                if (strCorrelationID.Length > 0)
                    msg = mq.ReceiveByCorrelationId(strCorrelationID, timeout);
                else
                    msg = mq.Receive(timeout);

                DateTime dtNow = DateTime.Now;
                
                sCorrelationId = msg.CorrelationId;
                sMsgId = msg.Id;

                //Big5編碼
                var encoding = Encoding.GetEncoding("Big5");
	            byte[] buffer = new byte[msg.BodyStream.Length];  
	            msg.BodyStream.Position = 0;  
	            msg.BodyStream.Read(buffer, 0, (int)msg.BodyStream.Length);
                sReceiveData = encoding.GetString(buffer); 
                
                //UTF8編碼(.net core系統預設)
                //sReceiveData = msg.Body.ToString();               
            }
            catch (Exception ex)
            {
               this.ErrMsg = "Receive ERROR " + ex.Message.ToString() + Environment.NewLine;
            }
        }
        public XmlDocument sendReceive(Message msg, Queue queReply)
        {
            try
            {
                SendByLabel(msg, "EAP");
                queReply.Receive(msg.Id);

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(queReply.sReceiveData);

                return xmlDoc;
            }
            catch
            {
                return null;
            }
        }
        public void Dispose()
        {
            mq.Dispose();
        }
    }
}
