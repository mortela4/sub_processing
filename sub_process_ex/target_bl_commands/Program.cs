using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace target_bl_commands
{
    class Program
    {
        static void Main(string[] args)
        {
            var UUT = new rom_bl_commands();

            string res = ( UUT.BLhostErase() ) ? "PASS" : "FAIL";

            Console.WriteLine("Command result: " + res);
        }
    }
}
