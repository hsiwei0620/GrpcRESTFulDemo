using System;
using Grpc.Core;
using MqGrpcProject;

using Newtonsoft.Json;
namespace MqGrpcsClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //Channel channel = new Channel("192.1.1.61:8080", ChannelCredentials.Insecure);
            //var client = new MqGrpcs.MqGrpcsClient(channel);
            //APMEQRSV_I_Request obj = new APMEQRSV_I_Request();
            //obj.Actiontype = "Q";
            //obj.Resveqptid = "U-11D";
            //obj.Onlyresvflg = "Y";
            //obj.Lotarycnt = "0";
            //obj.Serverip = "192.1.1.182";
//

            Channel channel = new Channel("192.1.1.61:8080", ChannelCredentials.Insecure);
            var client = new MqGrpcs.MqGrpcsClient(channel);
            APLPRDBM_Request obj = new APLPRDBM_Request();
            obj.Productid = "U79700N";
            obj.Eccode = "00";
            obj.Routeid = "U79700N";
            obj.Routever = "003";
            obj.Opeid = "U-79700-9-N003";
            obj.Opever = "00";
            obj.Resveqptid = "U-11D";
            obj.Lotid = "U101810260002";
            obj.Nxopeno = "0900100";
            obj.Serverip = "192.1.1.102";
            var reply = client.APLPRDBM_Send(obj);
            Console.WriteLine(JsonConvert.SerializeObject(reply));

            try
            {
            //    var reply = client.APMEQRSV_I(obj);
            //    Console.WriteLine(JsonConvert.SerializeObject(reply));
                //Console.WriteLine("trxid:" + reply.Trxid);
                //Console.WriteLine("typeid:" + reply.Typeid);
                //Console.WriteLine("rtncode:" + reply.Rtncode);
                //Console.WriteLine("rtnmesg:" + reply.Rtnmesg);
                //Lotarycnt = objtoInt32(reply.Lotarycnt, 0);
                //Norsvcnt = objtoInt32(reply.Norsvcnt, 0);
//
                //Console.WriteLine("lotarycnt:" + reply.Lotarycnt);
                //Console.WriteLine("norsvcnt:" + reply.Norsvcnt);
                //if (Lotarycnt >0){
                //    Console.WriteLine(JsonConvert.SerializeObject(reply.oary));
                //}


            }
            catch (System.Exception excp)
            {
                Console.WriteLine(excp.Message.ToString());
            }
            //var reply = client.APMEQRSV_I(new APMEQRSV_I_Request{
            //        Actiontype = "Q",
            //        Resveqptid = "U-11D",
            //        Onlyresvflg = "Y",
            //        Lotarycnt = "0"
            //});
            //Console.WriteLine("Greeting: " + reply.Result);
        }
    }
}
