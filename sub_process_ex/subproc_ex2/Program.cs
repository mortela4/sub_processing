using System;
using System.Diagnostics;


namespace subproc_ex2
{
    class Program
    {
        static void Run(string exeFilePath)
        {
            char[] pbuf = new char[80];

            Process process = new Process();
            process.StartInfo.FileName = exeFilePath;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            
            process.Start();

            while ( process.HasExited == false )
            {
                // async - won't block GUI ...
                int numBytes = process.StandardOutput.Read(pbuf, 0, 1);   // 1-by-1 char-buffered!
                pbuf[numBytes] = '\0';
                string inp = new string(pbuf);
                Console.WriteLine("Got input from subproc: " + inp);
            }
            
            process.WaitForExit();           // *SKAL* være safe - men OBS: chars KAN ankomme senere!!
            process.Close();
        }

        static void Main(string[] args)
        {
            //Run("dir.exe");
            Run("test_app/bin/Debug/test_app.exe");
        }
    }
}
