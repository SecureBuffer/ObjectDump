using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;

namespace ObjectDump
{
    internal class Program
    {
        public static Memory.Memory r = null;

      
        static void Main(string[] args)
        {
            Process[] Processes = Process.GetProcessesByName("Wow");
            if (Processes[0] != null)
            {
                r = new Memory.Memory(Processes[0]);
                ObjectManager.ObjectManager p = new ObjectManager.ObjectManager();
                p.Run();
                //ObjectManager.ObjectManager.PulseThread();

               // Console.WriteLine(r.ReadString(0x00C79B9E, 30));
            }
            else
            {
                Console.WriteLine("Processes[0] error");
            }
            Console.Read();
        }
    }
}
