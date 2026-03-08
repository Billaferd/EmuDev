using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmuDev.Common
{
    public interface ICpu<T> : ITickable
    {
        public void Execute(T opcode);
        public string Disassemble();
    }
}
