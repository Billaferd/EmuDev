using EmuDev.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmuDev.Chip8
{
    public class Chip8Computer
    {
        private byte[] _data { get; set; }

        private IBus<ushort> Bus { get; set; }

        private ICpu Cpu { get; set; }

        public Chip8Computer(String romPath)
            : this(new FileStream(romPath, FileMode.Open, FileAccess.Read))
        { }

        public Chip8Computer(FileStream romStream)
        {
            _data = new byte[romStream.Length];
            romStream.Read(_data, 0, _data.Length);
            Bus = new Chip8Bus(_data);
            Cpu = new Chip8Cpu(Bus);
        }

        public string Disassemble()
        {
            return Cpu.Disassemble();
        }

        public void Run()
        {
            int i = 0;

            // char e = 'N';

            while(true)
            {
                Cpu.Clock();
                i++;
            }
        }
    }
}
