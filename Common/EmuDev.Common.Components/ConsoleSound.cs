using System;

namespace EmuDev.Common.Components
{
    public class ConsoleSound : ISound
    {
        public void Beep()
        {
            // Note: Console.Beep is technically blocking on some platforms/drivers, 
            // but for a simple bleeper it's usually acceptable if kept short.
            // Frequency 440Hz, duration 10ms
            Console.Beep(440, 10);
        }
    }
}
