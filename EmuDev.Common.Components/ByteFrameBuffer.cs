namespace EmuDev.Common.Components
{
    public class ByteFrameBuffer : IBusComponent<byte>
    {
        private const int WIDTH = 64;
        private const int HEIGHT = 32;

        private ushort[] frameData = new ushort[WIDTH * HEIGHT];
        private ushort[] frameData2 = new ushort[WIDTH * HEIGHT];

        public byte this[int index] { 
            get => (byte)frameData[index];
            set
            {
                if (value > 0) { frameData2[index] = Convert.ToUInt16('▓'); } else { frameData2[index] = 0; }
            }
        }

        public int Size => frameData.Length;

        public void Tick()
        {
            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    if (frameData[x + (y * 64)] != frameData2[x + (y * 64)])
                    {
                        Console.SetCursorPosition(x, y);

                        ushort frame = frameData2[x + (y * 64)];

                        if (frame > 0)
                        {
                            Console.Write(Convert.ToChar(frame));
                        }
                        else
                        {
                            Console.Write(Convert.ToChar(' '));
                        }

                        frameData[x + (y * 64)] = frameData2[x + (y * 64)];
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