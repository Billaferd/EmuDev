// See https://aka.ms/new-console-template for more information
using EmuDev.Chip8;

// BinaryReader reader = new BinaryReader(new FileStream(@"C:\Users\ritch\OneDrive\Documents\Chip8\sctest.ch8", FileMode.Open));
// ushort PC = 1;

Chip8Computer comp = new Chip8Computer(@"C:\Users\ritch\OneDrive\Documents\Chip8\test_opcode.ch8");
//Console.WriteLine(comp.Disassemble());

comp.Run();
Console.Read();