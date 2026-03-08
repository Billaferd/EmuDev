using EmuDev.Common;
using EmuDev.Common.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmuDev.Chip8
{
    public class Chip8Cpu : ICpu<ushort>
    {
        #region Private State
        private readonly IBus<ushort> _bus;
        private ushort _i = 0;
        private ushort _pc = 0x200;
        private byte[] _v = new byte[16];
        private byte _delayTimer;
        private byte _soundTimer;
        private Stack<ushort> _stack = new Stack<ushort>();
        private bool _draw = false;
        #endregion

        #region Public Interface
        public ushort I => _i;
        public ushort PC => _pc;
        public IReadOnlyList<byte> V => _v;
        public byte DelayTimer => _delayTimer;
        public byte SoundTimer => _soundTimer;
        public bool DrawFlag => _draw;

        public IBusComponent<Byte> FrameBuffer { get; } = new ByteFrameBuffer();
        #endregion

        public Chip8Cpu(IBus<ushort> bus)
        {
            _bus = bus;
        }

        public void Tick()
        {
            if (_draw)
            {
                FrameBuffer.Tick();
                _draw = false;
            }

            // Fetch
            ushort opcode = (ushort)(_bus.ReadByte(_pc) << 8 | _bus.ReadByte((ushort)(_pc + 1)));
            _pc += 2;

            // Execute
            Execute(opcode);
        }

        public void TickTimers()
        {
            if (_delayTimer > 0) _delayTimer--;
            if (_soundTimer > 0) _soundTimer--;
        }

        public void Execute(ushort opcode)
        {
            ushort x = (ushort)((opcode & 0x0F00) >> 8);
            ushort y = (ushort)((opcode & 0x00F0) >> 4);
            byte n = (byte)(opcode & 0x000F);
            byte kk = (byte)(opcode & 0x00FF);
            ushort nnn = (ushort)(opcode & 0x0FFF);

            switch (opcode & 0xF000)
            {
                case 0x0000:
                    switch (opcode & 0x00FF)
                    {
                        case 0x00E0: // CLS
                            for (int i = 0; i < FrameBuffer.Size; i++) { FrameBuffer[i] = 0; }
                            _draw = true;
                            break;
                        case 0x00EE: // RET
                            _pc = _stack.Pop();
                            break;
                    }
                    break;

                case 0x1000: // JP addr
                    _pc = nnn;
                    break;

                case 0x2000: // CALL addr
                    _stack.Push(_pc);
                    _pc = nnn;
                    break;

                case 0x3000: // SE Vx, byte
                    if (_v[x] == kk) _pc += 2;
                    break;

                case 0x4000: // SNE Vx, byte
                    if (_v[x] != kk) _pc += 2;
                    break;

                case 0x5000: // SE Vx, Vy
                    if (_v[x] == _v[y]) _pc += 2;
                    break;

                case 0x6000: // LD Vx, byte
                    _v[x] = kk;
                    break;

                case 0x7000: // ADD Vx, byte
                    _v[x] += kk;
                    break;

                case 0x8000:
                    switch (opcode & 0x000F)
                    {
                        case 0x0: // LD Vx, Vy
                            _v[x] = _v[y];
                            break;
                        case 0x1: // OR Vx, Vy
                            _v[x] |= _v[y];
                            break;
                        case 0x2: // AND Vx, Vy
                            _v[x] &= _v[y];
                            break;
                        case 0x3: // XOR Vx, Vy
                            _v[x] ^= _v[y];
                            break;
                        case 0x4: // ADD Vx, Vy
                            {
                                ushort sum = (ushort)(_v[x] + _v[y]);
                                _v[0xF] = (byte)(sum > 255 ? 1 : 0);
                                _v[x] = (byte)(sum & 0xFF);
                            }
                            break;
                        case 0x5: // SUB Vx, Vy
                            _v[0xF] = (byte)(_v[x] >= _v[y] ? 1 : 0);
                            _v[x] = (byte)(_v[x] - _v[y]);
                            break;
                        case 0x6: // SHR Vx {, Vy}
                            _v[0xF] = (byte)(_v[x] & 0x1);
                            _v[x] >>= 1;
                            break;
                        case 0x7: // SUBN Vx, Vy
                            _v[0xF] = (byte)(_v[y] >= _v[x] ? 1 : 0);
                            _v[x] = (byte)(_v[y] - _v[x]);
                            break;
                        case 0xE: // SHL Vx {, Vy}
                            _v[0xF] = (byte)((_v[x] & 0x80) >> 7);
                            _v[x] <<= 1;
                            break;
                    }
                    break;

                case 0x9000: // SNE Vx, Vy
                    if (_v[x] != _v[y]) _pc += 2;
                    break;

                case 0xA000: // LD I, addr
                    _i = nnn;
                    break;

                case 0xB000: // JP V0, addr
                    _pc = (ushort)(_v[0] + nnn);
                    break;

                case 0xC000: // RND Vx, byte
                    _v[x] = (byte)(Random.Shared.Next() & kk);
                    break;

                case 0xD000: // DRW Vx, Vy, nibble
                    {
                        int sx = _v[x] % 64;
                        int sy = _v[y] % 32;
                        _v[0xF] = 0;

                        for (int row = 0; row < n; row++)
                        {
                            byte spriteByte = _bus.ReadByte((ushort)(_i + row));
                            for (int col = 0; col < 8; col++)
                            {
                                if ((spriteByte & (0x80 >> col)) != 0)
                                {
                                    int px = (sx + col) % 64;
                                    int py = (sy + row) % 32;
                                    int index = (py * 64) + px;

                                    if (FrameBuffer[index] != 0)
                                    {
                                        _v[0xF] = 1;
                                    }
                                    FrameBuffer[index] ^= 0xFF;
                                }
                            }
                        }
                        _draw = true;
                    }
                    break;

                case 0xE000:
                    switch (kk)
                    {
                        case 0x9E: // SKP Vx
                            // TODO: Keyboard implementation
                            break;
                        case 0xA1: // SKNP Vx
                            // TODO: Keyboard implementation
                            break;
                    }
                    break;

                case 0xF000:
                    switch (kk)
                    {
                        case 0x07: // LD Vx, DT
                            _v[x] = _delayTimer;
                            break;
                        case 0x0A: // LD Vx, K
                            // TODO: Keyboard implementation
                            break;
                        case 0x15: // LD DT, Vx
                            _delayTimer = _v[x];
                            break;
                        case 0x18: // LD ST, Vx
                            _soundTimer = _v[x];
                            break;
                        case 0x1E: // ADD I, Vx
                            {
                                int val = _i + _v[x];
                                _v[0xF] = (byte)(val > 0x0FFF ? 1 : 0);
                                _i = (ushort)val;
                            }
                            break;
                        case 0x29: // LD F, Vx
                            _i = (ushort)(0x50 + (5 * _v[x]));
                            break;
                        case 0x33: // LD B, Vx
                            {
                                byte value = _v[x];
                                _bus.WriteByte((ushort)(_i + 0), (byte)(value / 100));
                                _bus.WriteByte((ushort)(_i + 1), (byte)((value / 10) % 10));
                                _bus.WriteByte((ushort)(_i + 2), (byte)(value % 10));
                            }
                            break;
                        case 0x55: // LD [I], Vx
                            for (int i = 0; i <= x; i++) { _bus.WriteByte((ushort)(_i + i), _v[i]); }
                            break;
                        case 0x65: // LD Vx, [I]
                            for (int i = 0; i <= x; i++) { _v[i] = _bus.ReadByte((ushort)(_i + i)); }
                            break;
                    }
                    break;
            }
        }

        public string Disassemble()
        {
            StringBuilder opBuilder = new StringBuilder();
            ushort tempI = 0x200;

            while (tempI < _bus.DataLength + 0x200)
            {
                ushort opcode = (ushort)(_bus.ReadByte(tempI) << 8 | _bus.ReadByte((ushort)(tempI + 1)));
                tempI += 2;
                Chip8OpCode op = new Chip8OpCode(opcode);
                opBuilder.AppendLine(op.Explain());
            }

            return opBuilder.ToString();
        }
    }
}
