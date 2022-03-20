using EmuDev.Common;
using EmuDev.Common.Components;
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
        internal byte DelayTimer;
        internal byte SoundDelay;
        internal Stack<ushort> Stack = new Stack<ushort>();

        // internal int counter = 0;
        internal bool draw = false;

        public IBusComponent<Byte> FrameBuffer { get; } = new ByteFrameBuffer();

        public Chip8Cpu(IBus<ushort> bus)
        {
            _bus = bus;
        }

        public void Tick()
        {
            if (draw)
            {
                FrameBuffer.Tick();
                draw = false;
            }

            if (DelayTimer == 0)
            {
                ushort opcode = (ushort)(_bus.ReadByte(PC) << (2 * 4) | _bus.ReadByte((ushort)(PC + 1)));
                Chip8OpCode op = new Chip8OpCode(opcode, this);
                // Console.WriteLine(op.Explain());
                op.Execute();
            }
            else
            {
                DelayTimer--;
            }
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
