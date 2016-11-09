using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teste.Elevador;

namespace elevadorTeste
{
    public class ElevadorFactory
    {
        //14. A classe ElevadorFactory deve permitir ser criada sem parâmetros no construtor;
        public static IElevador Create()
        {
            return new Elevador(10, 8);
        }
    }
}
