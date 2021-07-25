using PatrimonioBusiness.fechamento.interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrimonioBusiness.fechamento.abstracts
{
    public abstract class MaterialItemDepreciacaoAbstract
    {
        protected IsolationLevel isolationLevel { get; set; }
        public MaterialItemDepreciacaoAbstract(IsolationLevel isolationLevel)
        {
            this.isolationLevel = isolationLevel;
        }

        public int TotalRegistros { get; protected set; }

        /// <summary>
        /// Verifica os itens de materiais da ManagerUnit(Uge) que não foram depreciados, data inválida, campo nulo.
        /// </summary>
        /// <param name="managerUnitId">Id da ManagerUnit(Uge)</param>
        /// <param name="mesAnoReferencia">Mês/Ano de referência do mês ativo da ManagerUnit(Uge)</param>
        /// <returns>Retorna as todos itens de materiais que não foram depreciados por motivo de erro de dados no banco.</returns>
        public abstract IEnumerable<IMaterialItemDepreciacao> GetNaoDepreciados(int managerUnitId, DateTime mesAnoReferencia);

    }
}
