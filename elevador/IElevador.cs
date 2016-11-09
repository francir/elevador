using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teste.Elevador
{
    public interface IElevador
    {
        //void Mover();
        void SelecionaAndar(int andar);
        void CriaRota();
        void Embarcar(int qtdPessoas);
        void Desembarcar(int qtdPessoas);
        void FecharPorta();
        void AbrirPorta();
        void setVisor(IVisor visor);
    }
}
