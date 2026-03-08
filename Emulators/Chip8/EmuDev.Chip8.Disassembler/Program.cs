// See https://aka.ms/new-console-template for more information
using EmuDev.Chip8;
using System.IO; // Added for File.Exists

// BinaryReader reader = new BinaryReader(new FileStream(@"C:\Users\ritch\OneDrive\Documents\Chip8\sctest.ch8", FileMode.Open));
// ushort PC = 1;
// sctest.ch8
// test_opcode.ch8
// c8_test.ch8
if (args.Length == 0)
{
    Console.WriteLine("Usage: EmuDev.Chip8.Disassembler <romPath>");
    return;
}

string romPath = args[0];
if (!File.Exists(romPath))
{
    Console.WriteLine($"Error: ROM file not found at {romPath}");
    return;
}

try
{
    Chip8Computer comp = new Chip8Computer(romPath);
    // Console.WriteLine(comp.Disassemble());
    comp.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Error starting emulator: {ex.Message}");
}
Console.Read();

// Console