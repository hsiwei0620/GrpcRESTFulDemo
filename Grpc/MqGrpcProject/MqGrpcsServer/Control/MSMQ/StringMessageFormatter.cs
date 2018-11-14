using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Experimental.System.Messaging;

namespace MqGrpcsServer
{
    public class StringMessageFormatter : IMessageFormatter, ICloneable
    {

        public bool CanRead(Message message)
        {
            return message.BodyStream != null;
        }

        public object Read(Message message)
        {
            if (message.BodyStream == null)
            {
                return null;
            }

            var bytes = new byte[message.BodyStream.Length];
            message.BodyStream.Read(bytes, 0, bytes.Length);
            message.Body = System.Text.Encoding.Default.GetString(bytes); ;
            return message.Body;
        }

        public void Write(Message message, object obj)
        {
            var str = obj as string;
            if (str != null)
            {
                //MSMQ Triggers是將Body以Unicode的方式傳給Console
                //如果不用Unicode就要在Console處理轉碼
                //還有因為UTF-8是1-4 Byte，但Unicode 是 2 Byte
                //被MSMQ Triggers硬轉成Unicode時，奇數Byte會掉一個Byte
                var bytes = Encoding.Default.GetBytes(str);
                message.BodyStream = new System.IO.MemoryStream(bytes);
            }
        }

        public object Clone()
        {
            return new StringMessageFormatter();
        }
    }
}
