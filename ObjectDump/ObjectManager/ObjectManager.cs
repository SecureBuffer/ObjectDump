using ObjectDump.ObjectManager.Enums;
using ObjectDump.ObjectManager.Objects;
using System;
using System.Collections.Generic;
using System.Timers;

namespace ObjectDump.ObjectManager
{
    public class ObjectManager 
    {
        internal static uint ObjectConnection { get; set; }
        internal static Dictionary<ulong, WowObject> ObjectTable;
        internal static readonly System.Timers.Timer aTimer = new System.Timers.Timer(500);
        static ObjectManager()
        {   try
            {
                var ClientConnection = Program.r.ReadUInt32(Program.r.ReadUInt32(((uint)Client.Base + (uint)Client.FirstConnection)));
                var ClientConnectionOffset = Program.r.ReadUInt32((uint)(0x04D7795 + Client.LastConnection));
                ObjectConnection = Program.r.ReadUInt32(ClientConnection + ClientConnectionOffset);


                ObjectTable = new Dictionary<ulong, WowObject>();


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

     

      

        internal static void Pulse()
        {
            WowObject wowObject = new WowObject(Program.r.ReadUInt32(ObjectConnection + (uint)ObjectTracker.First));
            while (wowObject.BaseAddress != 0 && wowObject.BaseAddress % 2 == 0)
            {
                WowObject @object = null;
                switch (wowObject.Type)
                {
                    case (int)WowObjectType.WowItem:
                        @object = new WowItem(wowObject.BaseAddress);
                        //Console.Write($"Type {WowObjectType.WowItem} GUID {wowObject.GUID} Entry {wowObject.Entry} baseAddress {wowObject.BaseAddress}\n");
                        break;
                    case (int)WowObjectType.WowContainer:
                        //Console.Write($"Type {WowObjectType.WowContainer} GUID {wowObject.GUID} Entry {wowObject.Entry} baseAddress {wowObject.BaseAddress}\n");
                        break;
                    case (int)WowObjectType.WowUnit:
                        @object = new WowUnit(wowObject.BaseAddress);
                        //Console.Write($"Type {WowObjectType.WowUnit} GUID {wowObject.GUID} Entry {wowObject.Entry} baseAddress {wowObject.BaseAddress}\n");
                        break;
                    case (int)WowObjectType.WowPlayer:
                        @object = new WowPlayer(wowObject.BaseAddress);
                        //Console.Write($"Type {WowObjectType.WowPlayer} GUID {wowObject.GUID} baseAddress {wowObject.BaseAddress}\n");
                        break;
                    case (int)WowObjectType.WowGameobject:
                        @object = new WowGameObject(wowObject.BaseAddress);
                        //Console.Write($"Type {WowObjectType.WowGameobject} GUID {wowObject.GUID} Entry {wowObject.Entry} baseAddress {wowObject.BaseAddress}\n");
                        break;
                    case (int)WowObjectType.WowDynamicObject:
                        @object = new WowDynamicObject(wowObject.BaseAddress);
                        //Console.Write($"Type {WowObjectType.WowDynamicObject} GUID {wowObject.GUID} Entry {wowObject.Entry} baseAddress {wowObject.BaseAddress}\n");
                        break;
                    case (int)WowObjectType.WowCorpse:
                        @object = new WowCorpse(wowObject.BaseAddress);
                        //Console.Write($"Type {WowObjectType.WowCorpse} GUID {wowObject.GUID} Entry {wowObject.Entry} baseAddress {wowObject.BaseAddress}\n");
                        break;
                }
                if(@object != null)
                {
                    if (ObjectTable.ContainsKey(@object.GUID))
                    {
                        ObjectTable[@object.GUID] = @object;
                    }
                    else ObjectTable.Add(@object.GUID, @object);
                }

                wowObject.BaseAddress = Program.r.ReadUInt32(wowObject.BaseAddress + (uint)ObjectTracker.Next);
            }
        }

        static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Console.WriteLine("---------------------- Update ----------------------");
            Pulse();
            foreach (var obj in ObjectTable)
            {
                Console.WriteLine($"Object Guid : {obj.Value.GUID}");
            }
            Console.WriteLine("---------------------- End ----------------------");
        }

        public static void PulseThread()
        {
            aTimer.Elapsed += OnTimedEvent;
            aTimer.Enabled = true;
        }
    }
}
