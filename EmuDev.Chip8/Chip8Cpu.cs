using EmuDev.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmuDev.Chip8
{
    public class Chip8Cpu : ICpu
    {
        internal IBus<ushort> _bus;
        internal ushort I = 0;
        internal ushort PC = 0x200;
        internal byte[] V = new byte[16];
        internal ushort[] Delay = new ushort[2];
        internal Stack<ushort> Stack = new Stack<ushort>();

        public Chip8Cpu(IBus<ushort> bus)
        {
            _bus = bus;
        }

        public void Clock()
        {
            ushort opcode = (ushort)(_bus.ReadByte(PC) << (2 * 4) | _bus.ReadByte((ushort)(PC + 1)));
            Chip8OpCode op = new Chip8OpCode(opcode, this);
            Console.WriteLine(op.Explain());
            op.Execute();
            PC += 2;
        }

        public string Disassemble()
        {
            StringBuilder opBuilder = new StringBuilder();

            while (I < _bus.DataLength + 0x200)
            {
                ushort opcode = (ushort)(_bus.ReadByte(I) << (2 * 4) | _bus.ReadByte((ushort)(I + 1)));
                I += 2;
                Chip8OpCode op = new Chip8OpCode(opcode, this);
                opBuilder.AppendLine(op.Explain());
            }

            return opBuilder.ToString();
        }
    }
}
