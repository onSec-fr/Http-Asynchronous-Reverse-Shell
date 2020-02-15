using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace HARS
{
    class IO
    {
        public static string stdout = "";
        public static string stderr = "";
        public static bool firstline = true;
        public static void readProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            // Write what was sent in the event
            if (!firstline)
                stdout += e.Data + Environment.NewLine;
            else
                firstline = false;
        }
        public static void readProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            // Write what was sent in the event
            stderr += e.Data + Environment.NewLine;
        }
        public static string DeleteLines(string input, int linesToSkip)
        {
            int startIndex = 0;
            for (int i = 0; i < linesToSkip; ++i)
                startIndex = input.IndexOf('\n', startIndex) + 1;
            return input.Substring(startIndex);
        }
    }
}
