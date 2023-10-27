using System;

namespace Memory.Enums
{
    public class ProcessAccessFlags
    {
        [Flags]
        public enum Flags : uint
        {
            Terminate = 0x0001,
            CreateThread = 0x0002,
            VirtualMemoryOperation = 0x0008,
            VirtualMemoryRead = 0x0010,
            VirtualMemoryWrite = 0x0020,
            DuplicateHandle = 0x0040,
            CreateProcess = 0x0080,
            SetQuota = 0x0100,
            SetInformation = 0x0200,
            QueryInformation = 0x0400,
            QueryLimitedInformation = 0x1000,
            Synchronize = 0x00100000,
            PROCESS_ALL_ACCESS = 0x1FFFFF
        }
    }
}
