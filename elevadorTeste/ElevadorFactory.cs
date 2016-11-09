using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace elevadorTeste
{
    public class ElevadorFactory
    {
        public static Elevador.IElevador Create()
        {
            return new Elevador.Elevador(10, 8, new VisorTeste());
        }
    }
}
