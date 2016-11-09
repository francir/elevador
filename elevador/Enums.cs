using System;

namespace Elevador
{
    public class Enums
    {
        public enum StatusElevador
        {
            Parado,
            Subindo,
            Descendo
        };

        public enum StatusPorta
        {
            Fechada,
            Aberta
        };

        public enum DirecaoRota
        {
            Cima,
            Baixo
        };
    }
}