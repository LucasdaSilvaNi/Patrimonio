using PatrimonioBusiness.integracao.abstracts;
using PatrimonioBusiness.integracao.contexto;
using PatrimonioBusiness.integracao.entidades;
using PatrimonioBusiness.integracao.interfaces;
using PatrimonioBusiness.integracao.repositorio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrimonioBusiness.integracao.business
{
    internal class MovimentoIntegracaoBusiness: MovimentoIntegracaoAbstract
    {
        private readonly MovimentoIntegracaoRepositorio movimentoIntegracaoRepositorio = null;
        private MovimentoIntegracaoBusiness(IntegracaoContexto integracaoContexto, IsolationLevel isolationLevel):base(isolationLevel) {

            movimentoIntegracaoRepositorio = MovimentoIntegracaoRepositorio.GetInstancia(integracaoContexto, base.isolationLevel);
        }
        internal static MovimentoIntegracaoBusiness GetInstancia(IntegracaoContexto integracaoContexto, IsolationLevel isolationLevel)
        {
            return new MovimentoIntegracaoBusiness(integracaoContexto, isolationLevel);
        }

        public override async Task<MovimentoIntegracaoAbstract> IncluirAsync(IMovimentoIntegracao movimentoIntegracao)
        {
            var transacao = this.movimentoIntegracaoRepositorio.createTransacao();
            try
            {
                var _movimentoIntegracao = MovimentoIntegracao.GetMovimentoIntegracao(movimentoIntegracao);
                this.movimentoIntegracaoRepositorio.Adicionar(_movimentoIntegracao);
                var resultado =  await this.movimentoIntegracaoRepositorio.SalvarAnsyc();
                transacao.Commit();
                base.movimentoIntegracao = _movimentoIntegracao.GetMovimentoIntegracao();
                return this;
            }catch(Exception ex)
            {
                transacao.Rollback();
                throw ex;
            }
            finally
            {
                transacao.Dispose();
                transacao = null;
            }
        }

        public override async Task<MovimentoIntegracaoAbstract> AtualizarAsync(IMovimentoIntegracao movimentoIntegracao)
        {
            var transacao = this.movimentoIntegracaoRepositorio.createTransacao();
            try
            {
                var _movimentoIntegracao = MovimentoIntegracao.GetMovimentoIntegracao(movimentoIntegracao);
                this.movimentoIntegracaoRepositorio.Atualizar(_movimentoIntegracao);
                var resultado = await this.movimentoIntegracaoRepositorio.SalvarAnsyc();
                transacao.Commit();
                base.movimentoIntegracao = _movimentoIntegracao.GetMovimentoIntegracao();
                return this;
            }
            catch (Exception ex)
            {
                transacao.Rollback();
                throw ex;
            }
            finally
            {
                transacao.Dispose();
                transacao = null;
            }
        }

        public override async Task<MovimentoIntegracaoAbstract> ExcluirAsync(IMovimentoIntegracao movimentoIntegracao)
        {
            var transacao = this.movimentoIntegracaoRepositorio.createTransacao();
            try
            {
                var _movimentoIntegracao = MovimentoIntegracao.GetMovimentoIntegracao(movimentoIntegracao);
                this.movimentoIntegracaoRepositorio.Excluir(x => x.Id == movimentoIntegracao.Id);
                var resultado = await this.movimentoIntegracaoRepositorio.SalvarAnsyc();
                transacao.Commit();
                base.movimentoIntegracao = _movimentoIntegracao.GetMovimentoIntegracao();
                return this;
            }
            catch (Exception ex)
            {
                transacao.Rollback();
                throw ex;
            }
            finally
            {
                transacao.Dispose();
                transacao = null;
            }
        }

        public override async Task<IMovimentoIntegracao> Get(int Id)
        {
            IQueryable<MovimentoIntegracao> query = this.movimentoIntegracaoRepositorio.Set(x => x.Id == Id).Get();
            var resultado = await query.FirstOrDefaultAsync();
            return resultado.GetMovimentoIntegracao();
        }

        public override async Task<IList<IMovimentoIntegracao>> GetsAsync(int codigoUge, string mesReferencia)
        {

            StringBuilder builder = new StringBuilder();
            builder.Append(" SELECT DISTINCT [IT].[TB_MOVIMENTO_INTEGRACAO_ID] AS 'Id' ");
            builder.Append(",[IT].[TB_TIPO_MOVIMENTO_ID] AS 'TipoMovimentoId'");
            builder.Append(",[IT].[TB_MOVIMENTO_NUMERO_DOCUMENTO] AS 'NumeroDocumento'");
            builder.Append(",[IT].[TB_MOVIMENTO_ANO_MES_REFERENCIA] AS 'AnoMesReferencia'");
            builder.Append(",[IT].[TB_MOVIMENTO_EMPENHO] AS 'Empenho'");
            builder.Append(",[IT].[TB_MOVIMENTO_OBSERVACOES] AS 'Obeservacoes'");
            builder.Append(",[IT].[TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO] AS 'AlmoxarifadoOrigemDestinoId'");
            builder.Append(",[IT].[TB_MOVIMENTO_DATA_MOVIMENTO] AS 'DataMovimento'");
            builder.Append(",[IT].[TB_MOVIMENTO_DATA_OPERACAO] AS 'DataOperacao'");
            builder.Append(",[IT].[TB_MOVIMENTO_ITEM_QTDE_MOV] AS 'QuantidadeMovimentada'");
            builder.Append(",[IT].[TB_MOVIMENTO_ITEM_VALOR_UNIT_EMP] AS 'ValorUnitario'");
            builder.Append(",[IT].[TB_MOVIMENTO_ATIVO] AS 'Ativo'");
            builder.Append(",[IT].[TB_MOVIMENTO_ITEM_ATIVO] AS 'ItemAtivo'");
            builder.Append(",[IT].[TB_INTEGRACAO_REALIZADO] AS 'IntegracaoRealizado'");
            builder.Append(",[IT].[TB_DATA_INCLUSAO] AS 'DataInclusao'");
            builder.Append(",[IT].[TB_DATA_ALTERACAO] AS 'DataAlteracao'");
            builder.Append(",[uge].[TB_UGE_CODIGO] AS 'CodigoUge'");
            builder.Append(",[IT].[TB_UA_CODIGO] AS 'CodigoUa'");
            builder.Append(",(SELECT TOP 1 [mat].[TB_ITEM_MATERIAL_CODIGO] FROM [dbo].[TB_ITEM_MATERIAL] [mat] ");
            builder.Append("      INNER JOIN [dbo].[TB_MOVIMENTO_ITEM] [movt] ON [movt].[TB_MOVIMENTO_ID] = [mov].[TB_MOVIMENTO_ID] WHERE [mat].[TB_ITEM_MATERIAL_ID] = [movt].[TB_ITEM_MATERIAL_ID]  AND [movt].[TB_MOVIMENTO_ITEM_ID] = [IT].[TB_MOVIMENTO_ITEM_ID]) AS 'ItemMaterialCode'");
            builder.Append(",(SELECT TOP 1 [mat].[TB_ITEM_MATERIAL_DESCRICAO] FROM [dbo].[TB_ITEM_MATERIAL] [mat] ");
            builder.Append("      INNER JOIN [dbo].[TB_MOVIMENTO_ITEM] [movt] ON [movt].[TB_MOVIMENTO_ID] = [mov].[TB_MOVIMENTO_ID] WHERE [mat].[TB_ITEM_MATERIAL_ID] = [movt].[TB_ITEM_MATERIAL_ID] AND [movt].[TB_MOVIMENTO_ITEM_ID] = [IT].[TB_MOVIMENTO_ITEM_ID]) AS 'ItemMaterialDescricao'");
            builder.Append("  FROM [dbo].[TB_MOVIMENTO_INTEGRACAO] [IT] ");
            builder.Append("  INNER JOIN [dbo].[TB_MOVIMENTO] [mov] ON [mov].[TB_MOVIMENTO_ID] = [IT].[TB_MOVIMENTO_ID] ");
            builder.Append("  INNER JOIN [dbo].[TB_UGE] [uge] ON [uge].[TB_UGE_ID] = [IT].[TB_UGE_ID] ");
            builder.Append("  LEFT OUTER JOIN [dbo].[TB_UA] [ua] ON [ua].[TB_UGE_ID] = [uge].[TB_UGE_ID] ");
            builder.Append("   AND [ua].[TB_UA_ID] = [mov].[TB_UA_ID] ");
            builder.Append("  INNER JOIN [dbo].[TB_UO] [uo] ON [uo].[TB_UO_ID] = [uge].[TB_UO_ID] ");
            builder.Append("  INNER JOIN [dbo].[TB_ORGAO] [orgao] ON [orgao].[TB_ORGAO_ID] =  [uo].[TB_ORGAO_ID] ");
            builder.Append(" WHERE [IT].[TB_INTEGRACAO_REALIZADO] = 0 AND [uge].[TB_UGE_CODIGO] =" + codigoUge.ToString());
            builder.Append("   AND [IT].[TB_MOVIMENTO_ATIVO] = 1 AND [IT].[TB_MOVIMENTO_ITEM_ATIVO] = 1");
           // builder.Append("   AND [IT].[TB_MOVIMENTO_ANO_MES_REFERENCIA] =" + mesReferencia);






            //this.movimentoIntegracaoRepositorio.Contexto().MovimentoIntegracoes.SqlQuery(builder.ToString());


            var resultado = await this.movimentoIntegracaoRepositorio.Contexto().MovimentoIntegracoes.SqlQuery(builder.ToString()).ToListAsync();
        
            return resultado.ConvertAll(new Converter<MovimentoIntegracao, IMovimentoIntegracao>(x => x.GetMovimentoIntegracao()));
        }
        
        public override async Task<bool> ConfirmarIntegracao(int movimentoIntegracaoId)
        {
            var transacao = this.movimentoIntegracaoRepositorio.createTransacao();
            try
            {
                StringBuilder builder = new StringBuilder();

                builder.Append("UPDATE [dbo].[TB_MOVIMENTO_INTEGRACAO] SET [TB_INTEGRACAO_REALIZADO] = 1");
                builder.Append(" WHERE [TB_MOVIMENTO_INTEGRACAO_ID] = " + movimentoIntegracaoId.ToString());

               
                var resultado = await this.movimentoIntegracaoRepositorio.Contexto().Database.ExecuteSqlCommandAsync(builder.ToString());
                transacao.Commit();
                if (resultado < 1)
                    return false;
                else
                    return true;
            }catch(Exception ex)
            {
                transacao.Rollback();
                throw ex;
            }
        }

        public override async Task<IList<IMovimentoIntegracao>> Gets(int paginaIndex, string filtro, int registroPorPagina)
        {
            var transacao = this.movimentoIntegracaoRepositorio.createTransacao();
            try
            {
                IQueryable<MovimentoIntegracao> query = null;

                int _totalRegistro = 0;

                if (paginaIndex < 0)
                    paginaIndex = 0;

                if (!string.IsNullOrEmpty(filtro) &&
                   !string.IsNullOrWhiteSpace(filtro))
                {
                    query = (from q in this.movimentoIntegracaoRepositorio.Contexto().MovimentoIntegracoes
                             where q.AlmoxarifadoId.ToString().Equals(filtro)
                                || q.DataMovimento.ToString().Equals(filtro)
                                || q.ItemMaterialId.Equals(filtro)
                                || q.Obeservacoes.Equals(filtro)
                                || q.NumeroDocumento.Equals(filtro)
                             select q);

                    _totalRegistro =  await this.movimentoIntegracaoRepositorio.Set(query).Get().CountAsync();

                    query = query.OrderBy(x => x.DataMovimento).Skip(paginaIndex).Take(registroPorPagina);

                }
                else
                {
                    query = (from q in this.movimentoIntegracaoRepositorio.Contexto().MovimentoIntegracoes
                           
                             select q);

                    _totalRegistro = await this.movimentoIntegracaoRepositorio.Set(query).Get().CountAsync();

                    query = query.OrderBy(x => x.DataMovimento).Skip(paginaIndex).Take(registroPorPagina);

                }

                var resultado = await this.movimentoIntegracaoRepositorio.Set(query).Get().ToListAsync();

                transacao.Commit();
                base.totalRegistroPorPagina = registroPorPagina;
                base.registroIndex = paginaIndex;
                base.filtro = filtro;
                base.totalRegistrosRetorno = _totalRegistro;
                return resultado.ConvertAll(new Converter<MovimentoIntegracao, IMovimentoIntegracao>(x => x.GetMovimentoIntegracao()));
            }
            catch(Exception ex)
            {
                transacao.Rollback();
                throw ex;
            }
            finally
            {
                transacao.Dispose();
                transacao = null;
            }
        }

        public override async Task<IList<IMovimentoIntegracao>> Gets(int UgeId)
        {
            IQueryable<MovimentoIntegracao> query = this.movimentoIntegracaoRepositorio.Set(x => x.UgeId  == UgeId).Get();
            var resultado = await query.ToListAsync();
            return resultado.ConvertAll(new Converter<MovimentoIntegracao, IMovimentoIntegracao>(x => x.GetMovimentoIntegracao()));
        }

        public override async Task<IList<IMovimentoIntegracao>> Gets(int codigoUA, int codigoItemMaterial)
        {
            IQueryable<MovimentoIntegracao> query = (from q in this.movimentoIntegracaoRepositorio.Contexto().MovimentoIntegracoes
                                                     where q.CodigoUa == codigoUA
                                                       && q.ItemMaterialCode == codigoItemMaterial
                                                     select q
                                                     );
            var resultado = await this.movimentoIntegracaoRepositorio.Set(query).Get().ToListAsync();
            return resultado.ConvertAll(new Converter<MovimentoIntegracao, IMovimentoIntegracao>(x => x.GetMovimentoIntegracao()));
        }

        public async override Task<MovimentoIntegracaoAbstract> DepreciarIncorporacao(int codigoDaUge, DateTime dataFinalDaDepreciacao, int assetStartId)
        {
            var transacao = this.movimentoIntegracaoRepositorio.createTransacao();
            try
            {

                this.retornoDepreciacao = await this.movimentoIntegracaoRepositorio.Contexto().Database.SqlQuery<string>("EXEC [dbo].[SAM_CALCULAR_DEPRECIACAO_INCORPORAR_START_EXECUTAR] @CodigoUge,@DataFinal,@AssetStartId", new Object[] {
                            new SqlParameter("@CodigoUge", codigoDaUge),
                            new SqlParameter("@DataFinal", dataFinalDaDepreciacao),
                            new SqlParameter("@AssetStartId", assetStartId),
                        }).FirstOrDefaultAsync();

                transacao.Commit();

                return this;
            }
            catch (Exception ex)
            {
                transacao.Rollback();
                throw ex;
            }
            finally
            {
                transacao.Dispose();
                transacao = null;
            }
        }
        public async override Task<MovimentoIntegracaoAbstract> DepreciarIntegracao(int codigoDaUge, DateTime dataFinalDaDepreciacao, int assetStartId)
        {
            var transacao = this.movimentoIntegracaoRepositorio.createTransacao();
            try
            {

                this.retornoDepreciacao = await this.movimentoIntegracaoRepositorio.Contexto().Database.SqlQuery<string>("EXEC [dbo].[SAM_CALCULAR_DEPRECIACAO_INTEGRACAO_START_EXECUTAR] @CodigoUge,@DataFinal,@AssetStartId", new Object[] {
                            new SqlParameter("@CodigoUge", codigoDaUge),
                            new SqlParameter("@DataFinal", dataFinalDaDepreciacao),
                            new SqlParameter("@AssetStartId", assetStartId),
                        }).FirstOrDefaultAsync();

                transacao.Commit();

                return this;
            }
            catch (Exception ex)
            {
                transacao.Rollback();
                throw ex;
            }
            finally
            {
                transacao.Dispose();
                transacao = null;
            }
        }

        public override IList<IMovimentoIntegracao> Gets()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(" SELECT DISTINCT [IT].[TB_MOVIMENTO_INTEGRACAO_ID] AS 'Id' ");
            builder.Append(",[IT].[TB_TIPO_MOVIMENTO_ID] AS 'TipoMovimentoId'");
            builder.Append(",[IT].[TB_MOVIMENTO_NUMERO_DOCUMENTO] AS 'NumeroDocumento'");
            builder.Append(",[IT].[TB_MOVIMENTO_ANO_MES_REFERENCIA] AS 'AnoMesReferencia'");
            builder.Append(",[IT].[TB_MOVIMENTO_EMPENHO] AS 'Empenho'");
            builder.Append(",[IT].[TB_MOVIMENTO_OBSERVACOES] AS 'Obeservacoes'");
            builder.Append(",[IT].[TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO] AS 'AlmoxarifadoOrigemDestinoId'");
            builder.Append(",[IT].[TB_MOVIMENTO_DATA_MOVIMENTO] AS 'DataMovimento'");
            builder.Append(",[IT].[TB_MOVIMENTO_DATA_OPERACAO] AS 'DataOperacao'");
            builder.Append(",[IT].[TB_MOVIMENTO_ITEM_QTDE_MOV] AS 'QuantidadeMovimentada'");
            builder.Append(",[IT].[TB_MOVIMENTO_ITEM_VALOR_UNIT_EMP] AS 'ValorUnitario'");
            builder.Append(",[IT].[TB_MOVIMENTO_ATIVO] AS 'Ativo'");
            builder.Append(",[IT].[TB_MOVIMENTO_ITEM_ATIVO] AS 'ItemAtivo'");
            builder.Append(",[IT].[TB_INTEGRACAO_REALIZADO] AS 'IntegracaoRealizado'");
            builder.Append(",[IT].[TB_DATA_INCLUSAO] AS 'DataInclusao'");
            builder.Append(",[IT].[TB_DATA_ALTERACAO] AS 'DataAlteracao'");
            builder.Append(",[uge].[TB_UGE_CODIGO] AS 'CodigoUge'");
            builder.Append(",[IT].[TB_UA_CODIGO] AS 'CodigoUa'");
            builder.Append(",(SELECT TOP 1 [mat].[TB_ITEM_MATERIAL_CODIGO] FROM [dbo].[TB_ITEM_MATERIAL] [mat] ");
            builder.Append("      INNER JOIN [dbo].[TB_MOVIMENTO_ITEM] [movt] ON [movt].[TB_MOVIMENTO_ID] = [mov].[TB_MOVIMENTO_ID] WHERE [mat].[TB_ITEM_MATERIAL_ID] = [movt].[TB_ITEM_MATERIAL_ID]  AND [movt].[TB_MOVIMENTO_ITEM_ID] = [IT].[TB_MOVIMENTO_ITEM_ID]) AS 'ItemMaterialCode'");
            builder.Append(",(SELECT TOP 1 [mat].[TB_ITEM_MATERIAL_DESCRICAO] FROM [dbo].[TB_ITEM_MATERIAL] [mat] ");
            builder.Append("      INNER JOIN [dbo].[TB_MOVIMENTO_ITEM] [movt] ON [movt].[TB_MOVIMENTO_ID] = [mov].[TB_MOVIMENTO_ID] WHERE [mat].[TB_ITEM_MATERIAL_ID] = [movt].[TB_ITEM_MATERIAL_ID] AND [movt].[TB_MOVIMENTO_ITEM_ID] = [IT].[TB_MOVIMENTO_ITEM_ID]) AS 'ItemMaterialDescricao'");
            builder.Append("  FROM [dbo].[TB_MOVIMENTO_INTEGRACAO] [IT] ");
            builder.Append("  INNER JOIN [dbo].[TB_MOVIMENTO] [mov] ON [mov].[TB_MOVIMENTO_ID] = [IT].[TB_MOVIMENTO_ID] ");
            builder.Append("  INNER JOIN [dbo].[TB_UGE] [uge] ON [uge].[TB_UGE_ID] = [IT].[TB_UGE_ID] ");
            builder.Append("  LEFT OUTER JOIN [dbo].[TB_UA] [ua] ON [ua].[TB_UGE_ID] = [uge].[TB_UGE_ID] ");
            builder.Append("   AND [ua].[TB_UA_ID] = [mov].[TB_UA_ID] ");
            builder.Append("  INNER JOIN [dbo].[TB_UO] [uo] ON [uo].[TB_UO_ID] = [uge].[TB_UO_ID] ");
            builder.Append("  INNER JOIN [dbo].[TB_ORGAO] [orgao] ON [orgao].[TB_ORGAO_ID] =  [uo].[TB_ORGAO_ID] ");
            builder.Append(" WHERE [IT].[TB_INTEGRACAO_REALIZADO] = 0");





            //this.movimentoIntegracaoRepositorio.Contexto().MovimentoIntegracoes.SqlQuery(builder.ToString());


            var resultado = this.movimentoIntegracaoRepositorio.Contexto().MovimentoIntegracoes.SqlQuery(builder.ToString()).ToList();

            return resultado.ConvertAll(new Converter<MovimentoIntegracao, IMovimentoIntegracao>(x => x.GetMovimentoIntegracao()));
        }
    }
}
