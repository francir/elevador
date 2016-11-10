using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Teste.Elevador
{
    public class Elevador : IElevador
    {
        //Temporizador de verificação do elevador com tempo de 5 segundos para fechamento da porta
        protected System.Timers.Timer m_mainTimer;
        protected int interval = 5000;
        protected IVisor Visor;
        protected int TempoEntreAndares = 1000;

        protected Enums.StatusElevador StatusElevador { get; set; }
        protected Enums.StatusPorta StatusPorta { get; set; }

        protected int AndarAtual { get; set; }
        protected bool[] Andares { get; set; }
        protected int QtdAndares { get; set; }

        protected int QtdPessoas { get; set; }
        protected int QtdMaxPessoas { get; set; }

        protected Rota Rota { get; set; }

        public Elevador(int qtdAndares, int qtdMaxPessoas)
        {
            //1.O elevador começa parado no térreo(Andar Zero), com as portas abertas;
            AndarAtual = 0;
            StatusElevador = Enums.StatusElevador.Parado;
            StatusPorta = Enums.StatusPorta.Aberta;
            Rota = null;

            QtdAndares = qtdAndares;
            QtdMaxPessoas = qtdMaxPessoas;

            Andares = new bool[QtdAndares + 1];

            //Inicializa o timer
            m_mainTimer = new System.Timers.Timer();
            m_mainTimer.Interval = interval;
            m_mainTimer.Elapsed += m_mainTimer_Elapsed;
            m_mainTimer.AutoReset = false;
            m_mainTimer.Start();
        }

        /// <summary>
        /// Metodo disparado a cada 5 segundos para verificação se existe alguma rota a seguir, conseguindo assim simular um elevador
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void m_mainTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Verifica se existe uma rota a seguir
            CriaRota();
            //4. A porta do elevador só se fecha quando há uma rota a seguir; 
            if (Rota != null)
                FecharPorta();

            //Reinicia o tempo de espera com a porta aberta para receber novos comandos e calcular nova rota, caso exista
            m_mainTimer.Start();
            MostrarVisor();
        }

        /// <summary>
        /// Método para movimentar o elevador até uma rota (andar) destino
        /// </summary>
        /// <param name="rota">Rota que contem a direção e o andar a seguir</param>
        protected void MoverPara(Rota rota)
        {
            //Move entres os andares até o destino atualizando o status
            while (AndarAtual != rota.Andar)
            {
                //2. Ao mover o elevador, deve-se atualizar o seu status (subindo ou descendo); 
                if (rota.Direcao == Enums.DirecaoRota.Cima)
                    AndarAtual++;
                else
                    AndarAtual--;

                //Timer para conseguir visualizar o movimento
                Thread.Sleep(TempoEntreAndares);
                MostrarVisor();
            }
            Andares[AndarAtual] = false;
            //Ao chegar no destino, abra a porta e atualize o status para parado
            AbrirPorta();
        }

        /// <summary>
        /// Metodo para criação da rota a ser seguida
        /// Este item foi modificado - 5. A rota deve conter apenas os andares que ainda não foram visitados; 
        /// Para poder selecionar um andar, em uma pausa durante o percurso, 
        /// defini que a rota só retorna o próximo andar a seguir, e não o caminho completo
        /// </summary>
        protected void CriaRota()
        {
            //8. Uma rota não pode ser feita com um número superior à capacidade máxima de pessoas; 
            if (QtdPessoas > QtdMaxPessoas)
            {
                Rota = null;
                return;
            }
            //10. Assim que selecionado os andares o elevador cria a rota;
            var acima = GetProximoAcima();
            var abaixo = GetProximoAbaixo();
            if (!acima.HasValue && !abaixo.HasValue)
                Rota = null;

            //Se já existir uma rota, verifica a direção para criar a nova rota
            if (Rota != null)
            {
                //3.O elevador deve levar em consideração se está descendo ou subindo para definir a rota, ou seja, 
                //  se estiver subindo deve subir até o ultimo andar definido antes de começar a descer;
                if (Rota.Direcao == Enums.DirecaoRota.Cima && acima.HasValue)
                    Rota.Andar = acima.Value;
                else if (Rota.Direcao == Enums.DirecaoRota.Cima && abaixo.HasValue)
                {
                    Rota.Andar = abaixo.Value;
                    Rota.Direcao = Enums.DirecaoRota.Baixo;
                }
                else if (Rota.Direcao == Enums.DirecaoRota.Baixo && abaixo.HasValue)
                    Rota.Andar = abaixo.Value;
                else if (Rota.Direcao == Enums.DirecaoRota.Baixo && acima.HasValue)
                {
                    Rota.Andar = acima.Value;
                    Rota.Direcao = Enums.DirecaoRota.Cima;
                }
                else
                    Rota = null;
            }
            else
            {
                //Se não existe rota definida, cria rota para o andar mais próximo
                if (acima.HasValue && !abaixo.HasValue)
                    Rota = new Rota { Andar = acima.Value, Direcao = Enums.DirecaoRota.Cima };
                else if (!acima.HasValue && abaixo.HasValue)
                    Rota = new Rota { Andar = abaixo.Value, Direcao = Enums.DirecaoRota.Baixo };
                else if (acima.HasValue && abaixo.HasValue)
                {
                    //Se tiver andar solicitado acima e abaixo, verificar o mais perto
                    if (Math.Abs(AndarAtual - acima.Value) > Math.Abs(AndarAtual - abaixo.Value))
                        Rota = new Rota { Andar = abaixo.Value, Direcao = Enums.DirecaoRota.Baixo };
                    else
                        Rota = new Rota { Andar = acima.Value, Direcao = Enums.DirecaoRota.Cima };
                }
            }

        }

        /// <summary>
        /// Método para verificar se existe um andar acima do atual solicitado
        /// </summary>
        /// <returns></returns>
        protected int? GetProximoAcima()
        {
            for (int next = AndarAtual + 1; next <= QtdAndares; next++)
                if (Andares[next])
                    return next;
            return null;
        }

        /// <summary>
        /// Método para verificar se existe um andar abaixo do atual solicitado
        /// </summary>
        /// <returns></returns>
        protected int? GetProximoAbaixo()
        {
            for (int next = AndarAtual - 1; next >= 0; next--)
                if (Andares[next])
                    return next;
            return null;
        }

        protected void AbrirPorta()
        {
            //6. O status parado só ocorre quando a porta é aberta, a mudança para subindo ou descendo ocorre quando a porta se fecha; 
            StatusElevador = Enums.StatusElevador.Parado;
            StatusPorta = Enums.StatusPorta.Aberta;
            MostrarVisor();
        }

        protected void FecharPorta()
        {
            if (Rota == null)
                throw new InvalidOperationException("Não existe rota para se locomover.");

            //6. O status parado só ocorre quando a porta é aberta, a mudança para subindo ou descendo ocorre quando a porta se fecha; 
            StatusPorta = Enums.StatusPorta.Fechada;
            if (Rota.Direcao == Enums.DirecaoRota.Cima)
                StatusElevador = Enums.StatusElevador.Subindo;
            else
                StatusElevador = Enums.StatusElevador.Descendo;

            MostrarVisor();
            //9.O Elevador só se movimenta quando a porta se fecha;
            MoverPara(Rota);
        }

        /// <summary>
        /// Método utilizado para atualizar o status do visor, caso exista um
        /// </summary>
        protected void MostrarVisor()
        {
            if (Visor != null)
                Visor.Mostrar(StatusElevador, StatusPorta, AndarAtual, Rota, Andares, QtdPessoas);
        }

        #region Métodos implementados da interface

        public void SelecionaAndar(int andar)
        {
            if (andar > QtdAndares || andar < 0)
                //13. As exceções de sistema devem ser usadas corretamente (Operações inválidas, argumentos nulos, etc.). Observação: Sempre que possível, utilize as exceções de sistema já definidas, ex: NullReferenceException, ArgumentException, InvalidCastException, etc. ; 
                throw new ArgumentException("Andar inválido.");

            //12.Os passageiros só podem selecionar o andar de destino da rota com a porta aberta;
            if (StatusPorta == Enums.StatusPorta.Fechada)
                //13. As exceções de sistema devem ser usadas corretamente (Operações inválidas, argumentos nulos, etc.). Observação: Sempre que possível, utilize as exceções de sistema já definidas, ex: NullReferenceException, ArgumentException, InvalidCastException, etc. ; 
                throw new InvalidOperationException("Não é possível selecionar o andar com a porta fechada.");

            //11.Se um dos andares selecionados for o andar atual o elevador simplesmente ignora o andar atual e não o incluir na rota;
            if (andar != AndarAtual)
                Andares[andar] = true;
        }

        public void Embarcar(int qtdPessoas)
        {
            //7. Os passageiros só podem embarcar ou desembarcar com o elevador parado e de portas abertas;
            if (StatusPorta == Enums.StatusPorta.Aberta && StatusElevador == Enums.StatusElevador.Parado)
                QtdPessoas += qtdPessoas;
            else
                throw new InvalidOperationException("Não é possível embarcar com a porta fechada");

            MostrarVisor();
        }

        public void Desembarcar(int qtdPessoas)
        {
            //7. Os passageiros só podem embarcar ou desembarcar com o elevador parado e de portas abertas;
            if (StatusPorta == Enums.StatusPorta.Aberta && StatusElevador == Enums.StatusElevador.Parado)
            {
                QtdPessoas -= qtdPessoas;
                if (qtdPessoas < 0) qtdPessoas = 0;
            }
            else
                throw new InvalidOperationException("Não é possível desembarcar com a porta fechada");

            MostrarVisor();
        }

        public void setVisor(IVisor visor)
        {
            Visor = visor;
        }

        #endregion
    }
}
