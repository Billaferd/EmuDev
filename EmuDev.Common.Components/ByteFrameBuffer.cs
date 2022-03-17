namespace EmuDev.Common.Components
{
    public class ByteFrameBuffer : IBusComponent<byte>
    {
        private const int WIDTH = 64;
        private const int HEIGHT = 32;

        private ushort[] frameData = new ushort[WIDTH * HEIGHT];

        public byte this[int index] { 
            get => (byte)frameData[index];
            set => frameData[index] = Convert.ToUInt16('▓');
        }

        public int Size => frameData.Length;

        public void Tick()
        {
            Console.Clear();

            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    ushort frame = frameData[x + (y * 64)];

                    if (frame > 0)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(Convert.ToChar(frame));
                    }   
                }
            }
        }

        public void CopyTo(IBusComponent<byte> comp, int index)
        {
            int x = 0;
            for(int i = index; i <= comp.Size; i++)
            {
                comp[i] = this[x];
                x++;
            }
        }
    }
}