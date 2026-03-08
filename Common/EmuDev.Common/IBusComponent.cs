using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmuDev.Common
{
    public interface IBusComponent<TSize> : ITickable
    {
        public TSize this[int index] { get; set; }

        public int Size { get; }

        public void CopyTo(IBusComponent<TSize> comp, int index);
    }
}
