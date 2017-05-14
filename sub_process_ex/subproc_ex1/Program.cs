using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace subproc_ex1
{

    class Program
    {
        static void Run(string exeFilePath)
        {
            Process process = new Process();
            process.StartInfo.FileName = exeFilePath;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
           {
               if (e.Data != null)
               {
                   Console.WriteLine("Got char: " + e.Data);
                    // ProgressBar.Value += 10; // Add 10% per '.' f.ex.
                }
           });
            process.Start();
            process.BeginOutputReadLine();   // Line-buffered
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
