using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmuDev.Common
{
    public interface IBusComponent<TSize>
    {
        public TSize this[int index] { get; set; }

        public void Tick();

        public int Size { get; }

        public void CopyTo(IBusComponent<TSize> comp, int index);
    }
}
