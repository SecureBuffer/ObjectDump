using ObjectDump.ObjectManager.Enums;
using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ObjectDump.ObjectManager
{
    public class WowObject
    {
        public uint BaseAddress { get; set; }

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate IntPtr GetObjectNameDelegate(IntPtr thisPointer);

        private readonly GetObjectNameDelegate _getObjectName;

        public WowObject(uint baseAddress)
        {
            this.BaseAddress = baseAddress;
            _getObjectName = RegisterVirtualFunction<GetObjectNameDelegate>(54);
        }

        public bool IsVaild()
        {
            return this.BaseAddress != 0;
        }

        public virtual String Name
        {
            get
            {

                IntPtr pointer = _getObjectName((IntPtr)BaseAddress);
                Console.WriteLine(pointer);
                if (pointer == IntPtr.Zero)
                    return "UNKNOWN";
                return Program.r.__ReadString(pointer);
            }
        }

        public int Type
        {
            get
            {
                return Program.r.ReadInt32(this.BaseAddress + 0x14);
            }
        }

        public virtual ulong GUID
        {
            get
            {
                return this.GetDescriptor<ulong>((uint)WoWObjectFields.OBJECT_FIELD_GUID);
            }
        }

        public uint Entry
        {
            get
            {
                return this.GetDescriptor<uint>((uint)WoWObjectFields.OBJECT_FIELD_ENTRY);
            }
        }

        public T GetDescriptor<T>(uint field) where T : struct
        {
            try
            {
                field *= 4U;
                return Program.r.ReadObject<T>(Descriptors() + field);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetDescriptor Error {ex.StackTrace}");
            }
            return default;
        }

        internal uint Descriptors()
        {
            try
            {
                if (this.IsVaild())
                {
                    return Program.r.ReadUInt32(this.BaseAddress + 0x08);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Descriptors Error {ex.StackTrace}");
            }
            return 0;
        }

      
        protected T RegisterVirtualFunction<T>(uint offset) where T : class
        {
            IntPtr pointer = Program.r.GetObjectVtableFunction((IntPtr)this.BaseAddress, offset);
          
            
            if (pointer == IntPtr.Zero)
                return null;
            return Program.r.RegisterDelegate<T>(pointer);
        }
    }
}
