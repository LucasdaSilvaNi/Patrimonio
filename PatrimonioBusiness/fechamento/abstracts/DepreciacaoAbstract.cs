using PatrimonioBusiness.fechamento.interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrimonioBusiness.fechamento.abstracts
{
    public abstract class DepreciacaoAbstract
    {
        protected IsolationLevel isolationLevel { get; set; }
        public DepreciacaoAbstract(IsolationLevel isolationLevel)
        {
            this.isolationLevel = isolationLevel;
        }

        public int TotalRegistros { get; protected set; }

        /// <summary>
        /// Retorna a uma depreciação do item de material do mês corrente
        /// </summary>
        /// <param name="assetStartId">Id de Inicio</param>
        /// <param name="managerUnitId">Id da Uge</param>
        /// <param name="materialItemCode">Código do item de material</param>
        /// <param name="currentDate">Data do mês de depreciação(mês/ano de referência)</param>
        /// <returns>Retorna as informações da depreciação</returns>
        public abstract Task<List<IDepreciacao>> Get(Int64 assetStartId, int managerUnitId, int materialItemCode, DateTime currentDate);
        /// <summary>
        /// Depreciações do item de material
        /// </summary>
        /// <param name="assetStartId">Id de Inicio</param>
        /// <param name="managerUnitId">Id da Uge</param>
        /// <param name="materialItemCode">Código do item de material</param>
        /// <returns>Retorna as todas as depreciações do item de material</returns>
        public abstract Task<List<IDepreciacao>> Gets(Int64 assetStartId, int managerUnitId, int materialItemCode);
        /// <summary>
        /// Simula e valida a depreciação
        /// </summary>
        /// <param name="assetStartId">Id de Inicio</param>
        /// <param name="managerUnitId">Id da Uge</param>
        /// <param name="materialItemCode">Código do item de material</param>
        /// <returns>Retorna a simulção e erro na simulação se ouver</returns>
        public abstract Task<ISimulacaoResultado> SimularDepreciacao(Int64 assetStartId, int managerUnitId, int materialItemCode, DateTime dataFinal);
        /// <summary>
        /// Realiza a depreciação
        /// </summary>
        /// <param name="assetStartId">Id de Inicio</param>
        /// <param name="managerUnitId">Id da Uge</param>
        /// <param name="materialItemCode">Código do item de material</param>
        /// <returns>Retorna verdadeiro se a depreciação foi executada com sucesso, falso se acontecer algum erro</returns>
        public abstract Task<Boolean> Depreciar(Int64 assetStartId, int managerUnitId, int materialItemCode, DateTime dataFinal);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="managerUnitId"></param>
        /// <param name="mesReferencia"></param>
        /// <returns></returns>
        public abstract int CreateRelatorioContabil(int managerUnitId, string mesReferencia);
    }
}
