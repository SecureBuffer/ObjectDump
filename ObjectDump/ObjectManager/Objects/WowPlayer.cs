using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDump.ObjectManager.Objects
{
    public class WowPlayer : WowUnit
    {
        public WowPlayer(uint baseAddress) : base(baseAddress)
        {
        }

        public int Level
        {
            get
            {
                return base.GetDescriptor<int>(0x40);
            }
        }
    }
}
