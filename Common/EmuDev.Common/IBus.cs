using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmuDev.Common
{
    public interface IBus<TBusWidth>
    {
        ushort DataLength
        {
            get;
        }

        public byte ReadByte(TBusWidth addr);

        public void WriteByte(TBusWidth addr, byte value);
    }
}
