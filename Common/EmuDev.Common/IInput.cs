namespace EmuDev.Common
{
    public interface IInput : IBusComponent<byte>
    {
        bool IsKeyPressed(byte key);
        byte? GetAnyKeyPressed();
        byte? WaitForKeyPress(); // Keeping for compatibility or specific needs
    }
}
