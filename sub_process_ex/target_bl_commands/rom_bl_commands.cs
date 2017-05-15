using System;
using System.Diagnostics;
using System.IO;

namespace target_bl_commands
{

    class rom_bl_commands
    {
        public const string BLHOST_CMD_PREFIX = @"\blhost_files\blhost.exe";
        public const string TARGET_UID_RESPONSE = @"Unique Device ID = ";       // 'Get UID' cmd pattern

        public static string targetUID = string.Empty;

        public rom_bl_commands()
        {
            // pass
        }

        // Generic command-runner : PRIVATE
        private static Tuple<bool,string> RunBLhostCmd(string cmd)
        {
            string patternLine = string.Empty;
            string fullPathCmd = Directory.GetCurrentDirectory() + BLHOST_CMD_PREFIX;
            Tuple<bool, string> retVal;

            Process process = new Process();
            process.StartInfo.FileName = fullPathCmd;
            process.StartInfo.Arguments = " -u -- " + cmd;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                if (e.Data != null)
                {
                    // Debug:
                    Console.WriteLine("Got line: " + e.Data);
                    // Process Line:
                    if ( e.Data.StartsWith(TARGET_UID_RESPONSE) )
                    {
                        var subStrings = e.Data.Split( new char[] { '=' } );
                        targetUID = subStrings[1];
                    }
                }
            });
            process.Start();
            process.BeginOutputReadLine();   // Line-buffered
            process.WaitForExit();           // *SKAL* være safe - men OBS: chars KAN ankomme senere!!
            if (process.ExitCode == 0)
                retVal = new Tuple<bool, string>(true, patternLine);
            else
                retVal = new Tuple<bool, string>(false, string.Empty);

            // Etter 'Close()' vil process-pbjekt være =null.
            process.Close();

            return retVal;
        }

        // Target-specific commands : PUBLIC
        public bool BLhostErase()
        {
            bool status = false;
            string result;

            string cmd = "flash-erase-all-unsecure";

            var ret = RunBLhostCmd(cmd);
            status = ret.Item1;
            result = ret.Item2;

            return status;
        }

        public bool BLhostProgram(string binFile)
        {
            bool status = false;

            return status;
        }

        public bool BLhostGetUID()
        {
            bool status = false;

            return status;
        }

        public bool BLhostWriteSensorID(string sensorID)
        {
            bool status = false;

            return status;
        }

    }

}
