using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmuDev.Common
{
    public interface IBusComponent : IList<byte>, ICollection<byte>, IEnumerable<byte>
    {
    }
}
