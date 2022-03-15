using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmuDev.Common
{
    public interface ICpu
    {
        public void Clock();

        public string Disassemble();
    }
}
