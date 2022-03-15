using EmuDev.Common;

namespace EmuDev.Chip8
{
    public record Chip8OpCode : IOpCode<ushort>
    {
        #region Private Properties
        private readonly ushort _opcode;
        private static Chip8Cpu _cpu;
        #region ExplainMap
        private static readonly Dictionary<ushort, Func<ushort, string>> _explainMap = new Dictionary<ushort, Func<ushort, string>>()
        {
            {0x0000, (ushort opcode) =>
                {
                    if ((opcode & 0x00F0) == 0x00E0)
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        return _explainMap[(ushort)(opcode & 0xF0FF)](opcode);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    return $"{opcode.ToString("X4")} UNK -> Unknown OpCode";
                }
            },
            { 0x00E0, (ushort opcode) => $"{opcode.ToString("X4")} CLS -> Clear screen" },
            { 0x00EE, (ushort opcode) => $"{opcode.ToString("X4")} RET -> Return from a subroutine" },
            { 0x1000, (ushort opcode) => $"{opcode.ToString("X4")} JP -> Jump to location ${opcode & 0x0FFF}" },
            { 0x2000, (ushort opcode) => $"{opcode.ToString("X4")} CALL -> Call subroutine at location ${(opcode & 0x0FFF).ToString("X4")}" },
            { 0x3000, (ushort opcode) => $"{opcode.ToString("X4")} SE -> Skip next instruction if V{(opcode & 0x0F00) >> (2 * 4)} = {(opcode & 0x00FF).ToString("X2")}" },
            { 0x4000, (ushort opcode) => $"{opcode.ToString("X4")} SNE -> Skip next instruction if V{(opcode & 0x0F00) >> (2 * 4)} != {(opcode & 0x00FF).ToString("X2")}"},
            { 0x5000, (ushort opcode) => $"{opcode.ToString("X4")} SE -> Skip next instruction if V{(opcode & 0x0F00) >> (2 * 4)} != V{(opcode & 0x00F0) >> (1 * 4)}"},
            { 0x6000, (ushort opcode) => $"{opcode.ToString("X4")} LD -> Set V{(opcode & 0x0F00) >> (2 * 4)} = {(opcode & 0x00FF).ToString("X2")}" },
            { 0x7000, (ushort opcode) => $"{opcode.ToString("X4")} ADD -> Set V{(opcode & 0x0F00) >> (2 * 4)} = V{(opcode & 0x0F00) >> (2 * 4)} + {(opcode & 0x00FF).ToString("X2")}"},
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            { 0x8000, (ushort opcode) => {
                if ((opcode & 0xF00F) == 0x8000)
                    return $"{opcode.ToString("X4")} LD -> Set V{(opcode & 0x0F00) >> (2 * 4)} = V{(opcode & 0x00F0) >> (1 * 4)}";
                try
                {
                    return _explainMap[(ushort)(opcode & 0xF00F)](opcode);
                }
                catch (Exception ex)
                {
                    return $"{opcode.ToString("X4")} UNK -> Unknown OpCode";
                }

                } },
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            { 0x8001, (ushort opcode) => $"{opcode.ToString("X4")} OR -> Set V{(opcode & 0x0F00) >> (2 * 4)} = V{(opcode & 0x0F00) >> (2 * 4)} OR V{(opcode & 0x00F0) >> (1 * 4)}" },
            { 0x8002, (ushort opcode) => $"{opcode.ToString("X4")} AND -> Set V{(opcode & 0x0F00) >> (2 * 4)} = V{(opcode & 0x0F00) >> (2 * 4)} AND V{(opcode & 0x00F0) >> (1 * 4)}" },
            { 0x8003, (ushort opcode) => $"{opcode.ToString("X4")} XOR -> Set V{(opcode & 0x0F00) >> (2 * 4)} = V{(opcode & 0x0F00) >> (2 * 4)} XOR V{(opcode & 0x00F0) >> (1 * 4)}" },
            { 0x8004, (ushort opcode) => $"{opcode.ToString("X4")} ADD -> Set V{(opcode & 0x0F00) >> (2 * 4)} = V{(opcode & 0x0F00) >> (2 * 4)} + V{(opcode & 0x00F0) >> (1 * 4)}, Set VF = carry" },
            { 0x8005, (ushort opcode) => $"{opcode.ToString("X4")} SUB -> Set V{(opcode & 0x0F00) >> (2 * 4)} = V{(opcode & 0x0F00) >> (2 * 4)} - V{(opcode & 0x00F0) >> (1 * 4)}, Set VF = NOT borrow" },
            { 0x8006, (ushort opcode) => $"{opcode.ToString("X4")} SHR -> Set V{(opcode & 0x0F00) >> (2 * 4)} = V{(opcode & 0x0F00) >> (2 * 4)} SHR 1" },
            { 0x8007, (ushort opcode) => $"{opcode.ToString("X4")} SUBN -> Set V{(opcode & 0x0F00) >> (2 * 4)} = V{(opcode & 0x00F0) >> (1 * 4)} - V{(opcode & 0x0F00) >> (2 * 4)}" },
            { 0x800E, (ushort opcode) => $"{opcode.ToString("X4")} SHL -> Set V{(opcode & 0x0F00) >> (2 * 4)} = V{(opcode & 0x0F00) >> (2 * 4)} SHL 1" },
            { 0x9000, (ushort opcode) => $"{opcode.ToString("X4")} SNE -> Skip next instuction if V{(opcode & 0x0F00) >> (2 * 4)} != V{(opcode & 0x00F0) >> (1 * 4)}" },
            { 0xA000, (ushort opcode) => $"{opcode.ToString("X4")} LD -> The value of register I is set to ${(opcode & 0x0FFF).ToString("X4")}" },
            { 0xB000, (ushort opcode) => $"{opcode.ToString("X4")} JP -> Jump to V0 + ${(opcode & 0x0FFF).ToString("X4")}" },
            { 0xC000, (ushort opcode) => $"{opcode.ToString("X4")} RND -> Set V{(opcode & 0x0F00) >> (2 * 4)} = <Random Byte> AND {(opcode & 0x00FF).ToString("X2")}" },
            { 0xD000, (ushort opcode) => $"{opcode.ToString("X4")} DRW -> Display a {opcode & 0x000F} byte sprite at ({(opcode & 0x0F00) >> (2*4)},{(opcode & 0x00F0) >> (1*4)})" },
            { 0xE000, (ushort opcode) =>
                {
                    if ((opcode & 0x00FF) > 0)
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        return _explainMap[(ushort)(opcode & 0xF0FF)](opcode);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    return $"{opcode.ToString("X4")} UNK -> Unknown OpCode";
                }
            },
            { 0xE09E, (ushort opcode) => $"{opcode.ToString("X4")} SKP -> Skip next instruction if key with the value of V{(opcode & 0x0F00) >> (2 * 4)} is pressed"},
            { 0xE0A1, (ushort opcode) => $"{opcode.ToString("X4")} SKNP -> Skip next instruction if key with the value of V{(opcode & 0x0F00) >> (2 * 4)} is NOT pressed"},

        };
        #endregion
        #region ExecuteMap
        private static readonly Dictionary<ushort, Action<ushort>> _executeMap = new Dictionary<ushort, Action<ushort>>()
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            {0x0000, (ushort opcode) =>
                {
                    if ((opcode & 0x00F0) == 0x00E0)

                        _executeMap[(ushort)(opcode & 0xF0FF)](opcode);
                }
            },
            { 0x00E0, (ushort opcode) => { } },
            { 0x00EE, (ushort opcode) => { _cpu.PC = _cpu.Stack.Pop(); } },
            { 0x1000, (ushort opcode) => { _cpu.PC = (ushort)((opcode & 0x0FFF)); } },
            { 0x2000, (ushort opcode) => { _cpu.Stack.Push(_cpu.PC); _cpu.PC = (ushort)(opcode & 0x0FFF); } },
            { 0x3000, (ushort opcode) => { if (_cpu.V[(opcode & 0x0F00) >> (2 * 4)] == (byte)(opcode & 0x00FF)) _cpu.PC += 2; } },
            { 0x4000, (ushort opcode) => { if(_cpu.V[(opcode & 0x0F00) >> (2 * 4)] != (byte)(opcode & 0x00FF)) _cpu.PC += 2; }  },
            { 0x5000, (ushort opcode) => { if(_cpu.V[(opcode & 0x0F00) >> (2 * 4)] != _cpu.V[(opcode & 0x00F0) >> (1 * 4)]) _cpu.PC += 2; } },
            { 0x6000, (ushort opcode) => { _cpu.V[(opcode & 0x0F00) >> (2 * 4)] = (byte)(opcode & 0x00FF); } },
            { 0x7000, (ushort opcode) => { _cpu.V[(opcode & 0x0F00) >> (2 * 4)] += (byte)(opcode & 0x00FF); } },
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            { 0x8000, (ushort opcode) => {
                if ((opcode & 0xF00F) == 0x8000)
                {
                    _cpu.V[(opcode & 0x0F00) >> (2 * 4)] = _cpu.V[(opcode & 0x00F0) >> (1 * 4)];
                }
                else
                {
                    try
                    {
                        _executeMap[(ushort)(opcode & 0xF00F)](opcode);
                    }
                    catch (Exception ex)
                    {
                        { }
                    }
                }
                } },
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            { 0x8001, (ushort opcode) => { _cpu.V[(opcode & 0x0F00) >> (2 * 4)] = (byte)(_cpu.V[(opcode & 0x0F00) >> (2 * 4)] | _cpu.V[(opcode & 0x00F0) >> (1 * 4)]); } },
            { 0x8002, (ushort opcode) => { _cpu.V[(opcode & 0x0F00) >> (2 * 4)] = (byte)(_cpu.V[(opcode & 0x0F00) >> (2 * 4)] & _cpu.V[(opcode & 0x00F0) >> (1 * 4)]); } },
            { 0x8003, (ushort opcode) => { _cpu.V[(opcode & 0x0F00) >> (2 * 4)] = (byte)(_cpu.V[(opcode & 0x0F00) >> (2 * 4)] ^ _cpu.V[(opcode & 0x00F0) >> (1 * 4)]); } },
            { 0x8004, (ushort opcode) => { _cpu.V[(opcode & 0x0F00) >> (2 * 4)] = (byte)(_cpu.V[(opcode & 0x0F00) >> (2 * 4)] + _cpu.V[(opcode & 0x00F0) >> (1 * 4)]); } },
            { 0x8005, (ushort opcode) => { _cpu.V[(opcode & 0x0F00) >> (2 * 4)] = (byte)(_cpu.V[(opcode & 0x0F00) >> (2 * 4)] - _cpu.V[(opcode & 0x00F0) >> (1 * 4)]); } },
            { 0x8006, (ushort opcode) => { _cpu.V[(opcode & 0x0F00) >> (2 * 4)] = (byte)(_cpu.V[(opcode & 0x0F00) >> (2 * 4)] >> 1); } },
            { 0x8007, (ushort opcode) => { _cpu.V[(opcode & 0x0F00) >> (2 * 4)] = (byte)(_cpu.V[(opcode & 0x00F0) >> (1 * 4)] - _cpu.V[(opcode & 0x0F00) >> (2 * 4)]); } },
            { 0x800E, (ushort opcode) => { _cpu.V[(opcode & 0x0F00) >> (2 * 4)] = (byte)(_cpu.V[(opcode & 0x0F00) >> (2 * 4)] << 1); } },
            { 0x9000, (ushort opcode) => { if(_cpu.V[(opcode & 0x0F00) >> (2 * 4)] != _cpu.V[(opcode & 0x00F0) >> (1 * 4)]) _cpu.PC +=2; } },
            { 0xA000, (ushort opcode) => { _cpu.I = (ushort)(opcode & 0x0FFF); } },
            { 0xB000, (ushort opcode) => { _cpu.PC = (ushort)(_cpu.V[0] + (opcode & 0x0FFF)); } },
            { 0xC000, (ushort opcode) => { _cpu.V[(opcode & 0x0F00) >> (2 * 4)] = (byte)(System.Random.Shared.Next() & (opcode & 0x00FF)); } },
            { 0xD000, (ushort opcode) => { } },
            #pragma warning restore CS8602 // Dereference of a possibly null reference.
            { 0xE000, (ushort opcode) =>
                {
                    if ((opcode & 0x00FF) > 0)
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        _executeMap[(ushort)(opcode & 0xF0FF)](opcode);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                }
            },
            { 0xE09E, (ushort opcode) => { } },
            { 0xE0A1, (ushort opcode) => { } },
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        };
        #endregion
        #endregion

        public ushort OpCode
        {
            get
            {
                return _opcode;
            }
        }

        public Chip8OpCode(ushort opcode, Chip8Cpu cpu)
        {
            _opcode = opcode;
            _cpu = cpu;
        }

        public void Execute()
        {
            try
            {
                _executeMap[(ushort)(OpCode & 0xF000)](_opcode);
            }
            catch (Exception ex)
            {
                // $"{_opcode.ToString("X4")} UNK -> Unknown OpCode";
            }
        }

        public string Explain()
        {
            try
            {
                return _explainMap[(ushort)(OpCode & 0xF000)](_opcode);
            }
            catch (Exception ex)
            {
                return $"{_opcode.ToString("X4")} UNK -> Unknown OpCode";
            }
        }

        public bool Mask(ushort mask)
        {
            return (OpCode & mask) > 0;
        }


    }
}