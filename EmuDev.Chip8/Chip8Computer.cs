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

        private ICpu<ushort> Cpu { get; set; }

        public Chip8Computer(String romPath)
            : this(new FileStream(romPath, FileMode.Open, FileAccess.Read))
        { }

        public Chip8Computer(FileStream romStream)
        {
            _data = new byte[romStream.Length];
            romStream.Read(_data, 0, _data.Length);
            Bus = new Chip8Bus(_data);
            Cpu = new Chip8Cpu(Bus);
            Console.SetWindowSize(64, 32);
        }

        public string Disassemble()
        {
            return Cpu.Disassemble();
        }

        public void Run()
        {
            var timerStopwatch = System.Diagnostics.Stopwatch.StartNew();
            var timerInterval = 1000.0 / 60.0; // 60Hz

            while(true)
            {
                // Instructions usually run at ~500-700Hz. 
                // A simple approach is to run some instructions, then check timers.
                for (int i = 0; i < 10; i++)
                {
                    Cpu.Tick();
                }

                if (timerStopwatch.Elapsed.TotalMilliseconds >= timerInterval)
                {
                    ((Chip8Cpu)Cpu).TickTimers();
                    timerStopwatch.Restart();
                }

                // Small sleep to avoid pegged CPU
                Thread.Sleep(1);
            }
        }
    }
}
