using System;
using System.Collections.Generic;
using System.Text;

namespace HARS
{
    class Config
    {
        // Behavior
        public static bool DisplayErrorMsg = true;
        public static string ErrorMsgTitle = "This application could not be started.";
        public static string ErrorMsgDesc = "Unhandled exception has occured in your application. \r\r Object {0} is not valid.";
        public static int MinDelay = 2;
        public static int MaxDelay = 5;
        public static string Url = "search?q=search+something&qs=n&form=QBRE&cvid=";
        // Listener
        public static string Server = "https://127.0.0.1";
        public static string Port = "443";
        public static bool AllowInsecureCertificate = true;
    }
}
