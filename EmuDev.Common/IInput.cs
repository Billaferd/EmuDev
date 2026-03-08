namespace EmuDev.Common
{
    public interface IInput : IBusComponent<byte>
    {
        bool IsKeyPressed(byte key);
        byte? WaitForKeyPress();
    }
}
