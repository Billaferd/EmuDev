using Spectre.Console;
using EmuDev.Chip8;
using System.IO;
using System.Linq;

namespace EmuDev
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AnsiConsole.Write(
                new FigletText("EmuDev")
                    .Centered()
                    .Color(Color.DeepSkyBlue1));

            while (true)
            {
                var emulator = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select an [green]Emulator[/]:")
                        .PageSize(10)
                        .AddChoices(new[] { "Chip-8", "Exit" }));

                if (emulator == "Exit") break;

                var action = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title($"[blue]{emulator}[/] - Select an [green]Action[/]:")
                        .AddChoices(new[] { "Run ROM", "Disassemble", "Back" }));

                if (action == "Back") continue;

                string romFolder = Path.Combine(Directory.GetCurrentDirectory(), "Roms");
                if (!Directory.Exists(romFolder))
                {
                    AnsiConsole.MarkupLine("[red]Error:[/] 'Roms' directory not found.");
                    continue;
                }

                var roms = Directory.GetFiles(romFolder, "*.ch8");
                if (roms.Length == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]No ROMs found in the 'Roms' directory.[/]");
                    continue;
                }

                var romChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select a [green]ROM[/]:")
                        .AddChoices(roms.Select(Path.GetFileName).Cast<string>())
                        .AddChoices("Back"));

                if (romChoice == "Back") continue;

                string selectedRomPath = Path.Combine(romFolder, romChoice);

                try
                {
                    var computer = new Chip8Computer(selectedRomPath);

                    if (action == "Run ROM")
                    {
                        AnsiConsole.Clear();
                        AnsiConsole.MarkupLine($"[green]Running[/] {romChoice}...");
                        computer.Run();
                    }
                    else if (action == "Disassemble")
                    {
                        var disassembly = computer.Disassemble();
                        AnsiConsole.Write(new Rule($"[yellow]Disassembly:[/] {romChoice}"));
                        AnsiConsole.WriteLine(disassembly);
                        AnsiConsole.Write(new Rule());
                        AnsiConsole.MarkupLine("\nPress [blue]Enter[/] to return to menu...");
                        Console.ReadLine();
                    }
                }
                catch (Exception ex)
                {
                    AnsiConsole.WriteException(ex);
                }
            }
        }
    }
}
