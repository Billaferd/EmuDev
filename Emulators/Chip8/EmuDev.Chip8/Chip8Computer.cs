using EmuDev.Common;
using EmuDev.Common.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EmuDev.Chip8
{
    public class Chip8Computer
    {
        private byte[] _data { get; set; }

        private IBus<ushort> Bus { get; set; }

        private ICpu<ushort> Cpu { get; set; }

        private IInput Input { get; set; }

        private ISound Sound { get; set; }

        public Chip8Computer(String romPath)
            : this(new FileStream(romPath, FileMode.Open, FileAccess.Read))
        { }

        public Chip8Computer(FileStream romStream)
        {
            _data = new byte[romStream.Length];
            romStream.Read(_data, 0, _data.Length);
            Bus = new Chip8Bus(_data);
            Input = new ConsoleInput();
            Sound = new ConsoleSound();
            Cpu = new Chip8Cpu(Bus, Input, Sound);
            Console.SetWindowSize(64, 32);
        }

        public string Disassemble()
        {
            return Cpu.Disassemble();
        }

        public void Run()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            double cpuAccumulator = 0;
            double timerAccumulator = 0;

            const double cpuTargetInterval = 1000.0 / 500.0; // 500Hz (2ms)
            const double timerTargetInterval = 1000.0 / 60.0; // 60Hz (16.67ms)

            long lastTime = stopwatch.ElapsedTicks;
            double tickToMs = 1000.0 / System.Diagnostics.Stopwatch.Frequency;

            while (true)
            {
                long currentTime = stopwatch.ElapsedTicks;
                double deltaTime = (currentTime - lastTime) * tickToMs;
                lastTime = currentTime;

                cpuAccumulator += deltaTime;
                timerAccumulator += deltaTime;

                // Execute CPU instructions at 500Hz
                while (cpuAccumulator >= cpuTargetInterval)
                {
                    Cpu.Tick();
                    cpuAccumulator -= cpuTargetInterval;
                }

                // Update timers at 60Hz
                if (timerAccumulator >= timerTargetInterval)
                {
                    ((Chip8Cpu)Cpu).TickTimers();
                    timerAccumulator -= timerTargetInterval;
                }

                // Yield to prevent 100% CPU usage while remaining responsive
                if (cpuAccumulator < cpuTargetInterval && timerAccumulator < timerTargetInterval)
                {
                    Thread.Yield();
                }
            }
        }
    }
}
