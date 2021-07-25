using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EstoquePatrimonio.Security.Infrastructure
{
    public interface ICrudBase<T>
    {
        /// <summary>
        /// 
        /// </summary>
        int PularRegistros
        {
            get;
            set;
        }
        
        T Entity { get; set; }

        /// <summary>
        /// Recupera uma lista completa da tabela 
        /// </summary>
        /// <returns></returns>
        IList<T> Listar();
        /// <summary>
        /// Recupera todos os Códigos
        /// </summary>
        /// <returns></returns>
        IList<T> ListAllCode();

        T LerRegistro();

        /// <summary>
        /// Recupera uma lista Customizada de uma tabela para ser utilizada  em Relatórios 
        /// </summary>
        /// <returns></returns>
        IList<T> Imprimir();

        void Excluir();

        void Salvar();

        bool PodeExcluir();

        bool ExisteCodigoInformado();

        int TotalRegistros();
    }
}
