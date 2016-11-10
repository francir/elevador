using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teste.Elevador
{
    /// <summary>
    /// Interface para permitir injeção de dependencia de um objeto visor na classe elevador, 
    /// assim, permitir mostrar o status do elevador
    /// </summary>
    public interface IVisor
    {
        void Mostrar(Enums.StatusElevador statusElevador, Enums.StatusPorta statusPorta, int andarAtual, Rota rota, bool[] Andares, int QtdPessoas);
    }
}
