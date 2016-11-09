using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Elevador
{
    public class Elevador : IElevador
    {
        //Temporizador de verificação do elevador com tempo de 5 segundos para fechamento da porta
        private System.Timers.Timer m_mainTimer;
        private int interval = 5000;
        protected IVisor Visor;
        private int TempoEntreAndares = 1000;

        protected Enums.StatusElevador StatusElevador { get; set; }
        protected Enums.StatusPorta StatusPorta { get; set; }

        protected int AndarAtual { get; set; }
        protected bool[] Andares { get; set; }
        protected int QtdAndares { get; set; }

        protected int QtdPessoas { get; set; }
        protected int QtdMaxPessoas { get; set; }

        protected Rota Rota { get; set; }

        public Elevador(int qtdAndares, int qtdMaxPessoas, IVisor visor)
        {
            Visor = visor;
            AndarAtual = 0;
            StatusElevador = Enums.StatusElevador.Parado;
            StatusPorta = Enums.StatusPorta.Aberta;
            Rota = null;

            QtdAndares = qtdAndares;
            QtdMaxPessoas = qtdMaxPessoas;

            Andares = new bool[QtdAndares + 1];
        }

        private void InicializaTimerPorta()
        {
            m_mainTimer = new System.Timers.Timer();
            m_mainTimer.Interval = interval;
            m_mainTimer.Elapsed += m_mainTimer_Elapsed;
            m_mainTimer.AutoReset = false;
            m_mainTimer.Start();
        }

        private void m_mainTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Verifica se existe uma rota a seguir
            CriaRota();
            //Caso exista, fecha a porta para iniciar o movimento
            if (Rota != null)
                FecharPorta();

            //Reinicia o tempo de espera com a porta aberta para receber novos comandos e calcular nova rota, caso exista
            m_mainTimer.Start();
        }

        protected void MoverPara(Rota rota)
        {
            //Move entres os andares até o destino atualizando o status
            while (AndarAtual != rota.Andar)
            {
                if (rota.Direcao == Enums.DirecaoRota.Cima)
                {
                    AndarAtual++;
                    StatusElevador = Enums.StatusElevador.Subindo;
                }
                else
                {
                    AndarAtual--;
                    StatusElevador = Enums.StatusElevador.Descendo;
                }

                Thread.Sleep(TempoEntreAndares);
                Andares[AndarAtual] = false;
                Visor.Mostrar(StatusElevador, StatusPorta, AndarAtual, Rota, Andares);
            }

            //Ao chegar no destino, abra a porta e atualize o status para parado
            AbrirPorta();
        }

        public void CriaRota()
        {
            //Se existir mais pessoas que o suportado, não cria a rota
            if (QtdPessoas > QtdMaxPessoas)
                Rota = null;

            //Resgata os andares acima e abaixo solicitados
            var acima = GetProximoAcima();
            var abaixo = GetProximoAbaixo();

            if (!acima.HasValue && !abaixo.HasValue)
                Rota = null;

            //Se já existir uma rota, verifica a direção para criar a nova rota
            if (Rota != null)
            {
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
                else
                {
                    //Se tiver andar solicitado acima e abaixo, verificar o mais perto
                    if (Math.Abs(AndarAtual - acima.Value) > Math.Abs(AndarAtual - abaixo.Value))
                        Rota = new Rota { Andar = abaixo.Value, Direcao = Enums.DirecaoRota.Baixo };
                    else
                        Rota = new Rota { Andar = acima.Value, Direcao = Enums.DirecaoRota.Cima };
                }
            }

        }

        public int? GetProximoAcima()
        {
            for (int next = AndarAtual + 1; next <= QtdAndares; next++)
                if (Andares[next])
                    return next;
            return null;
        }

        public int? GetProximoAbaixo()
        {
            for (int next = AndarAtual - 1; next >= 0; next--)
                if (Andares[next])
                    return next;
            return null;
        }

        public void SelecionaAndar(int andar)
        {
            if (andar > QtdAndares || andar < 0)
                throw new ArgumentException("Andar inválido.");
            if (StatusPorta == Enums.StatusPorta.Fechada)
                throw new InvalidOperationException("Não é possível selecionar o andar com a porta fechada.");

            if (andar != AndarAtual)
            {
                Andares[andar] = true;
                InicializaTimerPorta();
            }
        }

        public void Embarcar(int qtdPessoas)
        {
            if (StatusPorta == Enums.StatusPorta.Aberta && StatusElevador == Enums.StatusElevador.Parado)
                QtdPessoas += qtdPessoas;
        }

        public void Desembarcar(int qtdPessoas)
        {
            if (StatusPorta == Enums.StatusPorta.Aberta && StatusElevador == Enums.StatusElevador.Parado)
                QtdPessoas -= qtdPessoas;
        }

        public void AbrirPorta()
        {
            StatusElevador = Enums.StatusElevador.Parado;
            StatusPorta = Enums.StatusPorta.Aberta;
            Visor.Mostrar(StatusElevador, StatusPorta, AndarAtual, Rota, Andares);
        }

        public void FecharPorta()
        {
            if (Rota == null)
                throw new InvalidOperationException("Não existe rota para se locomover.");

            //Se fechou a porta, atualiza o status e mova até o destino
            if (Rota.Direcao == Enums.DirecaoRota.Cima)
                StatusElevador = Enums.StatusElevador.Subindo;
            else
                StatusElevador = Enums.StatusElevador.Descendo;

            Visor.Mostrar(StatusElevador, StatusPorta, AndarAtual, Rota, Andares);
            MoverPara(Rota);
        }

    }
}
