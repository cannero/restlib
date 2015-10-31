using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RestLib.Utils
{
    public class RestLogger
    {
        /// <summary>
        /// first string of the ExceptionEvent is "class::method"
        /// </summary>
        public static event Action<string, Exception> ExceptionEvent;
        public static event Action<string> WarningEvent;
        public static event Action<string> InfoEvent;
        
        public static void Log(string caller, Exception ex)
        {
            Action<string, Exception> ev = ExceptionEvent;
            if (ev != null)
            {
                ev(caller, ex);
            }
        }

        public static void LogWarning(string warning, params object[] args)
        {
            CallAction(WarningEvent, warning, args);
        }

        public static void LogInfo(string info, params object[] args)
        {
            CallAction(InfoEvent, info, args);
        }

        private static void CallAction(Action<string> action, string message, params object[] args)
        {
            Action<string> ev = action;
            if (ev != null)
            {
                string messageStr = message;
                try
                {
                    messageStr = string.Format(message, args);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
                ev(messageStr);
            }
        }
    }
}
