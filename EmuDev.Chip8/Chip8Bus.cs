using EmuDev.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmuDev.Chip8
{
    public class Chip8Bus : IBus<ushort>
    {
        private byte[] Ram { get; }
        public ushort DataLength { get; internal set; }

        public Chip8Bus(byte[] rom)
        {
            Ram = new byte[4096];
            rom.CopyTo(Ram, 0x200);
            DataLength = (ushort)rom.Length;
        }

        public byte ReadByte(ushort addr)
        {
            return Ram[addr & 0x0FFF];
        }

        public void WriteByte(ushort addr, byte value)
        {
            Ram[addr & 0x0FFF] = value;
        }
    }
}
