using Memory.Enums;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Memory
{
    public unsafe class Memory
    {
        internal IntPtr processHandle { get; set; }
        public uint baseAddress { get; }
        public Memory(Process process)
        {
            if(process.MainModule != null)
            {
                baseAddress = (uint)process.MainModule.BaseAddress.ToInt32();
                processHandle = Imports.OpenProcess(ProcessAccessFlags.Flags.PROCESS_ALL_ACCESS, false, process.Id);
            }
            else
            {
                Console.WriteLine("processHandle process Error");
            }
        }

        ~Memory()
        {
            Imports.CloseHandle(this.processHandle);
        }

        public short ReadInt16(uint address)
        {
            byte[] buffer = new byte[sizeof(Int16)];
            IntPtr bytesRead;

            bool success = Imports.ReadProcessMemory(processHandle, address, buffer, buffer.Length, out bytesRead);
            if(success)
            {
                return BitConverter.ToInt16(buffer, 0);
            }
            return 0;
        }

        public uint RebaseAddress(uint address)
        {
            return (baseAddress + address);
        }

        public Int32 ReadInt32(uint address)
        {
            byte[] buffer = new byte[sizeof(Int32)];
            IntPtr bytesRead;

            bool success = Imports.ReadProcessMemory(processHandle, address, buffer, buffer.Length, out bytesRead);
            if (success)
            {
                return BitConverter.ToInt32(buffer, 0);
            }
            return 0;
        }

        public Int64 ReadInt64(uint address)
        {
            byte[] buffer = new byte[sizeof(Int64)];
            IntPtr bytesRead;
            bool success = Imports.ReadProcessMemory(processHandle, address, buffer, buffer.Length, out bytesRead);
            if (success)
            {
                return BitConverter.ToInt64(buffer, 0);
            }
            return 0;
        }

        public byte Readbyte(uint address)
        {
            byte[] buffer = new byte[sizeof(byte)];
            IntPtr bytesRead;
            bool success = Imports.ReadProcessMemory(processHandle, address, buffer, buffer.Length, out bytesRead);
            if (success)
            {
                return buffer[0];
            }
            return 0;
        }


        [HandleProcessCorruptedStateExceptions]
        public byte[] ReadBytes(uint address, int size)
        {
            byte[] buffer = new byte[size];
            IntPtr bytesRead;
            bool success = Imports.ReadProcessMemory(processHandle, address, buffer, buffer.Length, out bytesRead);
            if (success)
            {
                return buffer;
            }
            return null;
        }

        public UInt32 ReadUInt32(uint address)
        {
            byte[] array = ReadBytes(address, 4);
            if (array != null)
            {
                return BitConverter.ToUInt32(array, 0);
            }
            return 0;
        }
      
        public virtual string __ReadString(IntPtr address, Encoding encoding = null, int maxLength = 512,
                                        bool relative = false)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            byte[] buffer = ReadBytes((uint)address, maxLength);
            string ret = encoding.GetString(buffer);
            if (ret.IndexOf('\0') != -1)
            {
                ret = ret.Remove(ret.IndexOf('\0'));
            }
            return ret;
        }


        public string ReadString(uint address, int size = 512)
        {
            if (address == 0)
                return null;

            try
            {
                byte[] buffer = ReadBytes(address, size);

                if (buffer == null || buffer.Length == 0)
                    return null;

                string result = Encoding.ASCII.GetString(buffer);

                if (result.IndexOf('\0') != -1)
                    result = result.Remove(result.IndexOf('\0'));

                return result;
            }
            catch (Exception ex)
            {
                // Log the error and return null or throw the exception depending on the use case.
                Console.WriteLine($"Error reading string from address {address}: {ex.Message}");
                return null;
            }
        }

        [HandleProcessCorruptedStateExceptions]
        static public byte[] _ReadBytes(uint address, int count)
        {
            if (address == 0)
                return null;

            try
            {
                byte[] result = new byte[count];

                for (int i = 0; i < count; i++)
                {
                    result[i] = Marshal.ReadByte((IntPtr)(address + i));
                }

                return result;
            }
            catch (Exception ex)
            {
                // Log the error and return null or throw the exception depending on the use case.
                Console.WriteLine($"Error reading bytes from address {address}: {ex.Message}");
                return null;
            }
        }

        public  UInt64 ReadUInt64(uint address)
        {
            byte[] buffer = new byte[sizeof(UInt64)];
            IntPtr bytesRead;
            bool success = Imports.ReadProcessMemory(processHandle, address, buffer, buffer.Length, out bytesRead);
            if (success)
            {
                return BitConverter.ToUInt64(buffer, 0);
            }
            return 0;
        }

        public T ReadObject<T>(uint address) where T : struct
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] buffer = new byte[size];
            IntPtr bytesRead;
            bool success = Imports.ReadProcessMemory(processHandle, address, buffer, buffer.Length, out bytesRead);
            if (success)
            {
                GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                T obj = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
                handle.Free();
                return obj;
            }
            return default(T);
        }


     

        public T RegisterDelegate<T>(IntPtr address, bool isRelative = false) where T : class
        {
            return Marshal.GetDelegateForFunctionPointer(address, typeof(T)) as T;
        }


        public byte[] __ReadBytes(IntPtr address, int numBytes)
        {
            byte[] buffer = new byte[numBytes];
            IntPtr bytesRead;
            bool success = Imports.ReadProcessMemory(processHandle, (uint)address, buffer, numBytes, out bytesRead);
            if (!success)
            {
                throw new Exception("ReadProcessMemory failed: " + Marshal.GetLastWin32Error());
            }
            return buffer;
        }


        public IntPtr ReadIntPtr(IntPtr address)
        {
            var buffer = new byte[IntPtr.Size];
            IntPtr bytesRead;


            var addr = RebaseAddress((uint)address);
            if (!Imports.ReadProcessMemory(this.processHandle, addr, buffer, (int)(uint)IntPtr.Size, out bytesRead))
            {
                throw new Exception("Failed to read memory at address " + address.ToString());
            }
            return (IntPtr)BitConverter.ToInt32(buffer, 0);
        }

        
        public IntPtr __ReadIntPtr(IntPtr address)
        {
            if (address == IntPtr.Zero)
                return IntPtr.Zero;

            try
            {
                return *(IntPtr*)address;
            }
            catch (AccessViolationException)
            {
                Console.WriteLine("Access Violation on " + address.ToString("X") + " with type IntPtr");
                return default;
            }
        }



        public IntPtr GetObjectVtableFunction(IntPtr objectBase, uint functionIndex)
        {
            // [[objBase]+4*vfuncIdx]
            return ReadIntPtr(new IntPtr(ReadUInt32((uint)objectBase) + functionIndex * 4));
        }

    }
}
