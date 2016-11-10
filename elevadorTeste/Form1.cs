using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Teste.Elevador;

namespace elevadorTeste
{
    /// <summary>
    /// Formulario para simulação da classe elevador.
    /// Esta classe implementa a interface visor para poder mostrar os dados na tela em tempo real
    /// </summary>
    public partial class frmControle : Form, IVisor
    {
        IElevador elevador;

        public frmControle()
        {
            InitializeComponent();
            elevador = ElevadorFactory.Create();
            elevador.setVisor(this);
        }

        private void btnAndar_Click(object sender, EventArgs e)
        {
            try
            {
                var andar = int.Parse(((Button)sender).Name.Replace("btn", ""));
                elevador.SelecionaAndar(andar);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnEmbarcar_Click(object sender, EventArgs e)
        {
            try
            {
                elevador.Embarcar(int.Parse(txtQtdPessoas.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDesembarcar_Click(object sender, EventArgs e)
        {
            try
            {
                elevador.Desembarcar(int.Parse(txtQtdPessoas.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region Funções do Visor

        public void Mostrar(Enums.StatusElevador statusElevador, Enums.StatusPorta statusPorta, int andarAtual, Rota rota, bool[] Andares, int QtdPessoas)
        {
            if (this.lblStatusElevador.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(DefinirTextoStatusElevador);
                this.Invoke(d, new object[] { statusElevador.ToString() });
            }
            else
                this.lblStatusElevador.Text = statusElevador.ToString();

            if (this.lblStatusPorta.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(DefinirTextoStatusPorta);
                this.Invoke(d, new object[] { statusPorta.ToString() });
            }
            else
                this.lblStatusPorta.Text = statusPorta.ToString();

            if (this.lblAndarAtual.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(DefinirTextoAndarAtual);
                this.Invoke(d, new object[] { andarAtual.ToString() });
            }
            else
                this.lblAndarAtual.Text = andarAtual.ToString();

            var texto = "";
            if (rota != null)
                texto = rota.Direcao.ToString();

            if (this.lblDirecao.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(DefinirTextoDirecao);
                this.Invoke(d, new object[] { texto });
            }
            else
                this.lblDirecao.Text = texto;

            if (rota != null)
                texto = rota.Andar.ToString();

            if (this.lblProximo.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(DefinirTextoProximo);
                this.Invoke(d, new object[] { texto });
            }
            else
                this.lblProximo.Text = texto;

            if (this.lblQtdPessoas.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(DefinirTextoQtdePessoas);
                this.Invoke(d, new object[] { QtdPessoas.ToString() });
            }
            else
                this.lblQtdPessoas.Text = QtdPessoas.ToString();
        }

        delegate void SetTextCallback(string texto);
        private void DefinirTextoStatusElevador(string texto)
        {
            this.lblStatusElevador.Text = texto;
        }
        private void DefinirTextoStatusPorta(string texto)
        {
            this.lblStatusPorta.Text = texto;
        }
        private void DefinirTextoAndarAtual(string texto)
        {
            this.lblAndarAtual.Text = texto;
        }
        private void DefinirTextoDirecao(string texto)
        {
            this.lblDirecao.Text = texto;
        }
        private void DefinirTextoQtdePessoas(string texto)
        {
            this.lblQtdPessoas.Text = texto;
        }
        private void DefinirTextoProximo(string texto)
        {
            this.lblProximo.Text = texto;
        }
        #endregion
    }
}
