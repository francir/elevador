using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevador
{
    public class ElevadorFactory
    {
        public static IElevador Create()
        {
            return new Elevador(10, 8);
        }
    }
}
