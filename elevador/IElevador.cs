using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teste.Elevador
{
    /// <summary>
    /// Interface que define as operações que poderão ser utilizadas pelo usuário
    /// </summary>
    public interface IElevador
    {
        /// <summary>
        /// Método usado para selecionar um andar
        /// </summary>
        /// <param name="andar"></param>
        void SelecionaAndar(int andar);

        /// <summary>
        /// Método para embarcar pessoas no elevador
        /// </summary>
        /// <param name="qtdPessoas"></param>
        void Embarcar(int qtdPessoas);

        /// <summary>
        /// Método para desembarcar pessoas no elevador
        /// </summary>
        /// <param name="qtdPessoas"></param>
        void Desembarcar(int qtdPessoas);

        /// <summary>
        /// Método usado para injeção de dependencia pra visualização do status do elevador ficar independente
        /// </summary>
        /// <param name="visor"></param>
        void setVisor(IVisor visor);
    }
}
