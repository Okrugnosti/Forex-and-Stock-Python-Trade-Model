using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace R10
{
    public class QuikApi
    {
        private static object m_Lock = new Object();
        //public enum enumQuikResults :int
        //{
        //    TRANS2QUIK_SUCCESS = 0,
        //    TRANS2QUIK_FAILED = 1,
        //    TRANS2QUIK_QUIK_TERMINAL_NOT_FOUND = 2,
        //    TRANS2QUIK_DLL_VERSION_NOT_SUPPORTED = 3,
        //    TRANS2QUIK_ALREADY_CONNECTED_TO_QUIK = 4,
        //    TRANS2QUIK_WRONG_SYNTAX = 5,
        //    TRANS2QUIK_QUIK_NOT_CONNECTED = 6,
        //    TRANS2QUIK_DLL_NOT_CONNECTED = 7,
        //    TRANS2QUIK_QUIK_CONNECTED = 8,
        //    TRANS2QUIK_QUIK_DISCONNECTED = 9,
        //    TRANS2QUIK_DLL_CONNECTED = 10,
        //    TRANS2QUIK_DLL_DISCONNECTED = 11,
        //    TRANS2QUIK_MEMORY_ALLOCATION_ERROR = 12,
        //    TRANS2QUIK_WRONG_CONNECTION_HANDLE = 13,
        //    TRANS2QUIK_WRONG_INPUT_PARAMS = 14
        //}
        #region Константы возвращаемых значений
        public const int TRANS2QUIK_SUCCESS = 0;
        public const int TRANS2QUIK_FAILED = 1;
        public const int TRANS2QUIK_QUIK_TERMINAL_NOT_FOUND = 2;
        public const int TRANS2QUIK_DLL_VERSION_NOT_SUPPORTED = 3;
        public const int TRANS2QUIK_ALREADY_CONNECTED_TO_QUIK = 4;
        public const int TRANS2QUIK_WRONG_SYNTAX = 5;
        public const int TRANS2QUIK_QUIK_NOT_CONNECTED = 6;
        public const int TRANS2QUIK_DLL_NOT_CONNECTED = 7;
        public const int TRANS2QUIK_QUIK_CONNECTED = 8;
        public const int TRANS2QUIK_QUIK_DISCONNECTED = 9;
        public const int TRANS2QUIK_DLL_CONNECTED = 10;
        public const int TRANS2QUIK_DLL_DISCONNECTED = 11;
        public const int TRANS2QUIK_MEMORY_ALLOCATION_ERROR = 12;
        public const int TRANS2QUIK_WRONG_CONNECTION_HANDLE = 13;
        public const int TRANS2QUIK_WRONG_INPUT_PARAMS = 14;

        // replecode
        public const int вы_не_можете_снять_данную_заявку = 5;
        public const int не_найдена_заявка_для_удаления = 4;
        public const int успешно = 3;
        public const int нехватка_средств_по_лимитам = 4;
        public const int error = 4;


        //public const int 
        //public static string replyCode_toString(int rez)
        //{
        //    switch (rez)
        //    { 
        //        case 3:
        //            return "[FORTS] Заявка успешно зарегистрирована";
        //        default:
        //            return "UNKNOWN_VALUE";   
        //    }
        //}

        //public enum enumQuikResults : int
        //{
        //    TRANS2QUIK_SUCCESS = 0,
        //    TRANS2QUIK_FAILED = 1,
        //    TRANS2QUIK_QUIK_TERMINAL_NOT_FOUND = 2,
        //    TRANS2QUIK_DLL_VERSION_NOT_SUPPORTED = 3,
        //    TRANS2QUIK_ALREADY_CONNECTED_TO_QUIK = 4,
        //    TRANS2QUIK_WRONG_SYNTAX = 5,
        //    TRANS2QUIK_QUIK_NOT_CONNECTED = 6,
        //    TRANS2QUIK_DLL_NOT_CONNECTED = 7,
        //    TRANS2QUIK_QUIK_CONNECTED = 8,
        //    TRANS2QUIK_QUIK_DISCONNECTED = 9,
        //    TRANS2QUIK_DLL_CONNECTED = 10,
        //    TRANS2QUIK_DLL_DISCONNECTED = 11,
        //    TRANS2QUIK_MEMORY_ALLOCATION_ERROR = 12,
        //    TRANS2QUIK_WRONG_CONNECTION_HANDLE = 13,
        //    TRANS2QUIK_WRONG_INPUT_PARAMS = 14
        //};
        //public static string ResultToString(int res)
        //{
        //    string result = "RESULT_UNKNOWN";

        //    if ((res >= 0) && (res <= 14))
        //        result = ((enumQuikResults)res).ToString();

        //    return result;
        //}

        #endregion

        public static string ResultToString(int Result)
        {
            switch (Result)
            {
                case TRANS2QUIK_SUCCESS:                                //0
                    return "TRANS2QUIK_SUCCESS";

                case TRANS2QUIK_FAILED:                                 //1
                    return "TRANS2QUIK_FAILED";

                case TRANS2QUIK_QUIK_TERMINAL_NOT_FOUND:                //2
                    return "TRANS2QUIK_QUIK_TERMINAL_NOT_FOUND";

                case TRANS2QUIK_DLL_VERSION_NOT_SUPPORTED:              //3
                    return "TRANS2QUIK_DLL_VERSION_NOT_SUPPORTED";

                case TRANS2QUIK_ALREADY_CONNECTED_TO_QUIK:              //4
                    return "TRANS2QUIK_ALREADY_CONNECTED_TO_QUIK";

                case TRANS2QUIK_WRONG_SYNTAX:                           //5
                    return "TRANS2QUIK_WRONG_SYNTAX";

                case TRANS2QUIK_QUIK_NOT_CONNECTED:                     //6
                    return "TRANS2QUIK_QUIK_NOT_CONNECTED";

                case TRANS2QUIK_DLL_NOT_CONNECTED:                      //7
                    return "TRANS2QUIK_DLL_NOT_CONNECTED";

                case TRANS2QUIK_QUIK_CONNECTED:                         //8
                    return "TRANS2QUIK_QUIK_CONNECTED";

                case TRANS2QUIK_QUIK_DISCONNECTED:                      //9
                    return "TRANS2QUIK_QUIK_DISCONNECTED";

                case TRANS2QUIK_DLL_CONNECTED:                          //10
                    return "TRANS2QUIK_DLL_CONNECTED";

                case TRANS2QUIK_DLL_DISCONNECTED:                       //11
                    return "TRANS2QUIK_DLL_DISCONNECTED";

                case TRANS2QUIK_MEMORY_ALLOCATION_ERROR:                //12
                    return "TRANS2QUIK_MEMORY_ALLOCATION_ERROR";

                case TRANS2QUIK_WRONG_CONNECTION_HANDLE:                //13
                    return "TRANS2QUIK_WRONG_CONNECTION_HANDLE";

                case TRANS2QUIK_WRONG_INPUT_PARAMS:                     //14
                    return "TRANS2QUIK_WRONG_INPUT_PARAMS";

                default:
                    return "UNKNOWN_VALUE";

            }
        }


        public static string ByteToString(byte[] strByte)
        {

            int count = 0;
            for (int i = 0; i < strByte.Length; ++i)
            {
                if (0 == strByte[i])
                {
                    count = i;
                    break;
                }
            }
            return System.Text.Encoding.Default.GetString(strByte, 0, count);

        }

        // соединение dll с квиком
        public const string path_dll = "TRANS2QUIK.DLL";
        //public static string path_to_quik = @"H:\InvestProjekt\VisualStudio8.0\QUIK\QUIK - KIT-FinanceRU";
        [DllImport(path_dll, EntryPoint = "_TRANS2QUIK_CONNECT@16",
        CallingConvention = CallingConvention.StdCall)]
        static extern int dllConnect(string connectionParamsString, ref int extendedErrorCode, byte[] errorMessage, uint errorMessageSize);

        public static int ConnectDLL(ref int extended_error_code, ref string error_message, string path_to_quik)
        {
            byte[] EMsg = new byte[50];

            int result = -1;

            extended_error_code = 0;
            error_message = string.Empty;
            lock (m_Lock)
            {
                result = dllConnect(path_to_quik, ref extended_error_code, EMsg, 50);
            }
            error_message = ByteToString(EMsg);

            return result & 255;
        }


        // проверка на коннект dll к квику
        [DllImport(path_dll, EntryPoint = "_TRANS2QUIK_IS_DLL_CONNECTED@12", CallingConvention = CallingConvention.StdCall)]
        static extern int dllIsDLLConnected(ref int extendedErrorCode, byte[] errorMessage, uint errorMessageSize);

        public static bool IsDLLConnected(ref int extendedErrorCode, ref string errorMessage)
        {
            bool result = false;
            byte[] EMsg = new byte[50];
            extendedErrorCode = 0;
            int res = -1;
            errorMessage = string.Empty;
            lock (m_Lock)
            {
                res = dllIsDLLConnected(ref extendedErrorCode, EMsg, 50);
            }
            errorMessage = ByteToString(EMsg);

            //if ((res & 255) == (int)enumQuikResults.TRANS2QUIK_DLL_CONNECTED)
            if ((res & 255) == TRANS2QUIK_DLL_CONNECTED)
            {
                result = true;
            }

            return result;
        }




        //  разрыв связи dll с терминалом квик
        [DllImport(path_dll, EntryPoint = "_TRANS2QUIK_DISCONNECT@12",
        CallingConvention = CallingConvention.StdCall)]
        static extern int dllDisconnect(ref int extendedErrorCode, byte[] errorMessage, uint errorMessageSize);

        public static int DisconnectDLL(ref int extendedErrorCode, ref string errorMessage)
        {
            int result = -1;
            byte[] EMsg = new byte[50];
            extendedErrorCode = 0;
            errorMessage = string.Empty;
            lock (m_Lock)
            {
                result = dllDisconnect(ref extendedErrorCode, EMsg, 50);
            }
            errorMessage = ByteToString(EMsg);

            return result & 255;
        }



        // проверка наличия соединения терминала с сервером
        [DllImport(path_dll, EntryPoint = "_TRANS2QUIK_IS_QUIK_CONNECTED@12",
        CallingConvention = CallingConvention.StdCall)]
        static extern int dllIsQuikConnected(ref int extendedErrorCode, byte[] errorMessage, uint errorMessageSize);

        public static bool IsQuikConnected(ref int extendedErrorCode, ref string errorMessage)
        {
            bool result = false;

            extendedErrorCode = 0;
            byte[] EMsg = new byte[50];
            errorMessage = string.Empty;
            int res;
            lock (m_Lock)
            {
                res = dllIsQuikConnected(ref extendedErrorCode, EMsg, 50);
            }
            errorMessage = ByteToString(EMsg);
            //if ((res & 255) == (int)enumQuikResults.TRANS2QUIK_QUIK_CONNECTED)
            if ((res & 255) == TRANS2QUIK_QUIK_CONNECTED)
            {
                result = true;
            }

            return result;
        }



        // посыл синхронной транзакции
        [DllImport(path_dll, EntryPoint = "_TRANS2QUIK_SEND_SYNC_TRANSACTION@36",
        CallingConvention = CallingConvention.StdCall)]
        static extern int dllSendSyncTransaction(string transactionString, ref int replyCode,
        ref uint transactionID, ref double orderNumber, byte[] resultMessage, uint resultMessageSize,
        ref int extendedErrorCode, byte[] errorMessage, uint errorMessageSize);

        public static int SendSyncTransaction(string strTransaction, ref long orderNumber, ref int replyCode, ref string resultMessage, ref int extendedErrorCode, ref string errorMessage)
        {
            int result = -1;
            double order = 0;
            byte[] RM = new byte[160];
            //string RM="0";
            byte[] EM = new byte[160];
            extendedErrorCode = 0;
            replyCode = 0;
            uint transactionID = 0;

            lock (m_Lock)
            {
                result = dllSendSyncTransaction(strTransaction, ref replyCode, ref transactionID, ref order, RM, 160, ref extendedErrorCode, EM, 160);
            }

            orderNumber = (long)order;
            resultMessage = ByteToString(RM);
            //resultMessage = RM;
            errorMessage = ByteToString(EM);

            return result & 255;
        }

               

    }



    public class Modul_API
    {

            QuikApi a = new QuikApi(); // объявляем класс по API передачи в QUIK
            int extendedErrorCode = 0;
            string errorMessage = "0";
            long orderNumber = 0;
            int replyCode = 0;
            string resultMessage = " ";

            int result; // переменная получает результат проведенной операции. коды ошибок в классе


            public long Send_Transaktions(string strTransaction, string QUIK_PATH)
            {
                result = QuikApi.ConnectDLL(ref extendedErrorCode, ref errorMessage, QUIK_PATH); // соединяем DLL с QUIK


                //strTransaction = "ACCOUNT=15003bf; CLIENT_CODE=xR01; TYPE = L; TRANS_ID=34; CLASSCODE=SPBFUT; SECCODE=O2Z2; ACTION=NEW_ORDER; OPERATION=S; PRICE=10100; QUANTITY=1;"; // в нее пишем торговую команду для QUIK
                result = QuikApi.SendSyncTransaction(strTransaction, ref orderNumber, ref replyCode, // отправка транзакции в QUIK
                            ref resultMessage, ref extendedErrorCode, ref errorMessage);

                //Console.WriteLine("Заявка1");

                //Console.WriteLine(strTransaction);
                //Console.WriteLine(result);
                //Console.WriteLine(orderNumber);
                //Console.WriteLine(replyCode);
                //Console.WriteLine(resultMessage);
                //Console.WriteLine(extendedErrorCode);
                //Console.WriteLine(errorMessage);
                              

                return orderNumber;
            }

        }
    
}