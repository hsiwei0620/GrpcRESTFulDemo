using System;
using System.Diagnostics;

namespace MqGrpcsClient
{
    public class GlobalClass
    {
        
        public static String GetField(String FieldName, String FieldValue){
            String Result = "";
            if (FieldValue != ""){
                Result = "<" + FieldName + ">" + FieldValue + @"</" + FieldName + ">";
            }
            return Result;
        }

        public static String objtoStr(Object values){
            try
            {
                return Convert.ToString(values).Trim();
            }
            catch (System.Exception )
            {
                return "";
            }
        }
        
        public static String objtoStr(Object values, String dvalue){
            try
            {
                return Convert.ToString(values);
            }
            catch (System.Exception )
            {
                return dvalue;
            }
        }
        public static Int32 objtoInt32(Object values){
            try
            {
                return Convert.ToInt32(values);
            }
            catch (System.Exception )
            {
                return 0;
            }
        }
        public static Int32 objtoInt32(Object values, Int32 dvalue){
            try
            {
                return Convert.ToInt32(values);
            }
            catch (System.Exception )
            {
                return dvalue;
            }
        }
        public static String GetMethodName(){
            StackTrace st   = new StackTrace(true);      //true means get line numbers.
            return st.GetFrame(1).GetMethod().Name;
        }

        public static String GetErrMsg(Exception ex, String sMsg, String MethodName){
            StackTrace e = new StackTrace(ex, true);
            return MethodName + " Error <br>" + "<b>Happen Time</b> : " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + 
                                             "<br><b>LineNumber</b> : " + e.GetFrame(0).GetFileLineNumber().ToString() + 
                                             "<br><b>Column</b> : " + e.GetFrame(0).GetFileColumnNumber().ToString() + 
                                             "<br><b>MSG</b> : <br>" + ex.StackTrace.ToString().Replace("'", "<br>") + 
                                             "<br><b>自定義的錯誤訊息 : </b><br>" + sMsg;
        }
    }
}