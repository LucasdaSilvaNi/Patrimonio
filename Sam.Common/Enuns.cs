using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;


namespace Sam.Common
{
    public struct Enuns
    {

        public enum NivelAcessoEnum
        {
            Orgao = 1,
            UO = 2,
            UGE = 3,
            UA = 4,
            UA_GESTOR = 8,
            DIVISAO = 5,
            DIVISAO_UA = 9,
            GESTOR = 6,
            ALMOXARIFADO = 7

        }

        //public enum Perfil
        //{
        //    ADMINISTRADOR_GESTOR = 4,
        //    ADMINISTRADOR_ORGAO = 7,
        //    ADMINISTRADOR_GERAL = 5
        //    //ADMINISTRADOR_GESTOR = 3,
        //    //ADMINISTRADOR_ORGAO = 5,
        //    //ADMINISTRADOR_GERAL = 4

        //}


        public enum AcessoTransacao
        {
            Negado = 0,
            Consulta = 1,
            Edita = 2
        }

    }
}
