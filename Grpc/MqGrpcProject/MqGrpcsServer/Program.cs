using System;
using System.Threading.Tasks;
using System.Text;
using Grpc.Core;
using MqGrpcProject;

// 以下必備安裝
// BIG5 編碼元件
// dotnet add package System.Text.Encoding.CodePages --version 4.5.0
// MSMQ 元件
// dotnet add package Experimental.System.Messaging --version 1.0.0
// Newtonsoft.Json 元件
// dotnet add package Newtonsoft.Json --version 11.0.2

namespace MqGrpcsServer
{    
    class Program
    {

        static void Main(string[] args)
        {
            int Port = 8080;
            string IpAddress = "localhost";

            if (args.Length>0){
                for (Int32 idx = 0; idx < args.Length;idx++){
                    string[] temp = args[idx].Split('=');
                    if(temp.Length == 2){
                        switch(temp[0].Trim().ToLower())
                        {
                            case "ip":
                                IpAddress = temp[1];
                                break;
                            case "port":
                                Port = GlobalClass.objtoInt32(temp[1]);
                                break;
                        }
                    }
                }
            }

            //一定要加這一行才能支援BIG5編碼
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        
            Server server = new Server
            {
                Services = { MqGrpcs.BindService(new MqGrpcsImpl())},
                Ports = { new ServerPort(IpAddress, Port, ServerCredentials.Insecure)}
            };
            server.Start();
            Console.WriteLine("MqGrpcs server listening on Ip " + IpAddress);
            Console.WriteLine("MqGrpcs server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();
            server.ShutdownAsync().Wait();
        }
    }

    #region Server List
    class MqGrpcsImpl : MqGrpcs.MqGrpcsBase
    {        
        //dotnet run -f netcoreapp2.1
        public override Task<APMEQRSV_Reply> APMEQRSV_Send(APMEQRSV_Request request, ServerCallContext context)
        {
            try
            {
                Console.WriteLine("client called");
                return Task.FromResult(APMEQRSVc.GetDatas(request, context));
            }
            catch (System.Exception excp)
            {
                Console.WriteLine(excp.Message.ToString());
                return Task.FromResult(new APMEQRSV_Reply());
            }            
        }
        
        #region APCMTLST
        public override Task<APCMTLST_Reply> APCMTLST_Send(APCMTLST_Request request, ServerCallContext context)
        {
            try
            {
                Console.WriteLine("client called");
                return Task.FromResult(APCMTLSTc.GetDatas(request, context));
            }
            catch (System.Exception excp)
            {
                Console.WriteLine(excp.Message.ToString());
                return Task.FromResult(new APCMTLST_Reply());
            }            
        }
        #endregion



        #region APISHTEQ
        public override Task<APISHTEQ_Reply> APISHTEQ_Send(APISHTEQ_Request request, ServerCallContext context)
        {
            try
            {
                Console.WriteLine("client called");
                return Task.FromResult(APISHTEQc.GetDatas(request, context));
            }
            catch (System.Exception excp)
            {
                Console.WriteLine(excp.Message.ToString());
                return Task.FromResult(new APISHTEQ_Reply());
            }            
        }
        #endregion        
        #region APLPRDBM
        public override Task<APLPRDBM_Reply> APLPRDBM_Send(APLPRDBM_Request request, ServerCallContext context)
        {
            try
            {
                Console.WriteLine("client called");
                //dotnet run -f netcoreapp2.1
                return Task.FromResult(APLPRDBMc.GetDatas(request, context));
            }
            catch (System.Exception excp)
            {
                Console.WriteLine(excp.Message.ToString());
                return Task.FromResult(new APLPRDBM_Reply());
            }            
        }
        #endregion
        #region APLRSVPR
        public override Task<APLRSVPR_Reply> APLRSVPR_Send(APLRSVPR_Request request, ServerCallContext context)
        {
            try
            {
                Console.WriteLine("client called");
                //dotnet run -f netcoreapp2.1
                return Task.FromResult(APLRSVPRc.GetDatas(request, context));
            }
            catch (System.Exception excp)
            {
                Console.WriteLine(excp.Message.ToString());
                return Task.FromResult(new APLRSVPR_Reply());
            }            
        }
        #endregion
        #region APIMTLST
        public override Task<APIMTLST_Reply> APIMTLST_Send(APIMTLST_Request request, ServerCallContext context)
        {
            try
            {
                Console.WriteLine("client called");
                //dotnet run -f netcoreapp2.1
                return Task.FromResult(APIMTLSTc.GetDatas(request, context));
            }
            catch (System.Exception excp)
            {
                Console.WriteLine(excp.Message.ToString());
                return Task.FromResult(new APIMTLST_Reply());
            }            
        }
        #endregion        
    }
    #endregion    
}
