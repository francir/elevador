using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teste.Elevador
{
    public interface IVisor
    {
        void Mostrar(Enums.StatusElevador statusElevador, Enums.StatusPorta statusPorta, int andarAtual, Rota rota, bool[] Andares, int QtdPessoas);
    }
}
