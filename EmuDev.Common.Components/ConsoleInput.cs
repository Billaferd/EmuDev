using System;
using System.Collections.Generic;
using System.Threading;

namespace EmuDev.Common.Components
{
    public class ConsoleInput : IInput
    {
        private readonly Dictionary<ConsoleKey, byte> _keyMapping = new Dictionary<ConsoleKey, byte>
        {
            { ConsoleKey.D1, 0x1 }, { ConsoleKey.D2, 0x2 }, { ConsoleKey.D3, 0x3 }, { ConsoleKey.D4, 0xC },
            { ConsoleKey.Q, 0x4 }, { ConsoleKey.W, 0x5 }, { ConsoleKey.E, 0x6 }, { ConsoleKey.R, 0xD },
            { ConsoleKey.A, 0x7 }, { ConsoleKey.S, 0x8 }, { ConsoleKey.D, 0x9 }, { ConsoleKey.F, 0xE },
            { ConsoleKey.Z, 0xA }, { ConsoleKey.X, 0x0 }, { ConsoleKey.C, 0xB }, { ConsoleKey.V, 0xF }
        };

        private byte[] _keyStates = new byte[16];
        private DateTime[] _lastPressTimes = new DateTime[16];
        private const double KEY_HOLD_DURATION_MS = 100;

        public byte this[int index]
        {
            get => _keyStates[index];
            set => _keyStates[index] = value;
        }

        public int Size => _keyStates.Length;

        public bool IsKeyPressed(byte key)
        {
            if (key >= 16) return false;
            return _keyStates[key] != 0 && (DateTime.Now - _lastPressTimes[key]).TotalMilliseconds < KEY_HOLD_DURATION_MS;
        }

        public byte? WaitForKeyPress()
        {
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var keyInfo = Console.ReadKey(true);
                    if (_keyMapping.TryGetValue(keyInfo.Key, out byte chip8Key))
                    {
                        _keyStates[chip8Key] = 1;
                        _lastPressTimes[chip8Key] = DateTime.Now;
                        return chip8Key;
                    }
                }
                Thread.Sleep(10);
            }
        }

        public void Tick()
        {
            while (Console.KeyAvailable)
            {
                var keyInfo = Console.ReadKey(true);
                if (_keyMapping.TryGetValue(keyInfo.Key, out byte chip8Key))
                {
                    _keyStates[chip8Key] = 1;
                    _lastPressTimes[chip8Key] = DateTime.Now;
                }
            }

            // Optional: Aging out old key states
            for (int i = 0; i < 16; i++)
            {
                if (_keyStates[i] != 0 && (DateTime.Now - _lastPressTimes[i]).TotalMilliseconds >= KEY_HOLD_DURATION_MS)
                {
                    _keyStates[i] = 0;
                }
            }
        }

        public void CopyTo(IBusComponent<byte> comp, int index)
        {
            for (int i = 0; i < Size && (index + i) < comp.Size; i++)
            {
                comp[index + i] = _keyStates[i];
            }
        }
    }
}
