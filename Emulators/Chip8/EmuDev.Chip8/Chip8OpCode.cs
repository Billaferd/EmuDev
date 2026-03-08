using EmuDev.Common;
using System;

namespace EmuDev.Chip8
{
    public readonly struct Chip8OpCode : IOpCode<ushort>
    {
        #region Private Properties
        private readonly ushort _opcode;

        private ushort X => (ushort)((_opcode & 0x0F00) >> 8);
        private ushort Y => (ushort)((_opcode & 0x00F0) >> 4);
        private byte N => (byte)(_opcode & 0x000F);
        private byte KK => (byte)(_opcode & 0x00FF);
        private ushort NNN => (ushort)(_opcode & 0x0FFF);
        #endregion

        public ushort OpCode => _opcode;

        public Chip8OpCode(ushort opcode)
        {
            _opcode = opcode;
        }

        public string Explain()
        {
            switch (_opcode & 0xF000)
            {
                case 0x0000:
                    switch (_opcode & 0x00FF)
                    {
                        case 0x00E0: return $"{_opcode:X4} CLS -> Clear screen";
                        case 0x00EE: return $"{_opcode:X4} RET -> Return from a subroutine";
                        default: return $"{_opcode:X4} UNK -> Unknown OpCode";
                    }
                case 0x1000: return $"{_opcode:X4} JP -> Jump to location ${NNN:X4}";
                case 0x2000: return $"{_opcode:X4} CALL -> Call subroutine at location ${NNN:X4}";
                case 0x3000: return $"{_opcode:X4} SE -> Skip next instruction if V{X:X} = {KK:X2}";
                case 0x4000: return $"{_opcode:X4} SNE -> Skip next instruction if V{X:X} != {KK:X2}";
                case 0x5000: return $"{_opcode:X4} SE -> Skip next instruction if V{X:X} == V{Y:X}";
                case 0x6000: return $"{_opcode:X4} LD -> Set V{X:X} = {KK:X2}";
                case 0x7000: return $"{_opcode:X4} ADD -> Set V{X:X} = V{X:X} + {KK:X2}";
                case 0x8000:
                    switch (_opcode & 0x000F)
                    {
                        case 0x0: return $"{_opcode:X4} LD -> Set V{X:X} = V{Y:X}";
                        case 0x1: return $"{_opcode:X4} OR -> Set V{X:X} = V{X:X} OR V{Y:X}";
                        case 0x2: return $"{_opcode:X4} AND -> Set V{X:X} = V{X:X} AND V{Y:X}";
                        case 0x3: return $"{_opcode:X4} XOR -> Set V{X:X} = V{X:X} XOR V{Y:X}";
                        case 0x4: return $"{_opcode:X4} ADD -> Set V{X:X} = V{X:X} + V{Y:X}, Set VF = carry";
                        case 0x5: return $"{_opcode:X4} SUB -> Set V{X:X} = V{X:X} - V{Y:X}, Set VF = NOT borrow";
                        case 0x6: return $"{_opcode:X4} SHR -> Set V{X:X} = V{X:X} SHR 1";
                        case 0x7: return $"{_opcode:X4} SUBN -> Set V{X:X} = V{Y:X} - V{X:X}";
                        case 0xE: return $"{_opcode:X4} SHL -> Set V{X:X} = V{X:X} SHL 1";
                        default: return $"{_opcode:X4} UNK -> Unknown OpCode";
                    }
                case 0x9000: return $"{_opcode:X4} SNE -> Skip next instruction if V{X:X} != V{Y:X}";
                case 0xA000: return $"{_opcode:X4} LD -> The value of register I is set to ${NNN:X4}";
                case 0xB000: return $"{_opcode:X4} JP -> Jump to V0 + ${NNN:X4}";
                case 0xC000: return $"{_opcode:X4} RND -> Set V{X:X} = <Random Byte> AND {KK:X2}";
                case 0xD000: return $"{_opcode:X4} DRW -> Display a {N} byte sprite at (V{X:X}, V{Y:X})";
                case 0xE000:
                    switch (KK)
                    {
                        case 0x9E: return $"{_opcode:X4} SKP -> Skip next instruction if key with the value of V{X:X} is pressed";
                        case 0xA1: return $"{_opcode:X4} SKNP -> Skip next instruction if key with the value of V{X:X} is NOT pressed";
                        default: return $"{_opcode:X4} UNK -> Unknown OpCode";
                    }
                case 0xF000:
                    switch (KK)
                    {
                        case 0x07: return $"{_opcode:X4} LD -> Set V{X:X} = delay timer value";
                        case 0x0A: return $"{_opcode:X4} LD -> Wait for a key press, store the value of the key in V{X:X}";
                        case 0x15: return $"{_opcode:X4} LD -> Set delay timer = V{X:X}";
                        case 0x18: return $"{_opcode:X4} LD -> Set sound timer = V{X:X}";
                        case 0x1E: return $"{_opcode:X4} ADD -> Set I = I + V{X:X}";
                        case 0x29: return $"{_opcode:X4} LD -> Set I = location of sprite for digit V{X:X}";
                        case 0x33: return $"{_opcode:X4} LD -> Store BCD representation of V{X:X} in memory locations I, I+1, and I+2";
                        case 0x55: return $"{_opcode:X4} LD -> Store registers V0 through V{X:X} in memory starting at location I";
                        case 0x65: return $"{_opcode:X4} LD -> Read registers V0 through V{X:X} from memory starting at location I";
                        default: return $"{_opcode:X4} UNK -> Unknown OpCode";
                    }
                default:
                    return $"{_opcode:X4} UNK -> Unknown OpCode";
            }
        }

        public bool Mask(ushort mask)
        {
            return (OpCode & mask) > 0;
        }
    }
}
