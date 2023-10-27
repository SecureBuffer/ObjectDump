using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDump.ObjectManager.Objects
{
    public  class LocalPlayer : WowPlayer
    {
        public LocalPlayer(uint baseAddress) : base(baseAddress)
        {
        }
    }
}
