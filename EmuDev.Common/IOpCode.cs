namespace EmuDev.Common
{
    // Used for Opcode Extraction and manipulation
    public interface IOpCode<T>
    {
        public String Explain();
        public T OpCode { get; }
        public bool Mask(T mask);
        public void Execute();
    }
}