using SAM.Web.Models;
using SAM.Web.Common.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace SAM.Web.Legados
{
	public class ImportarBemPatrimonial
	{
		private Legado _legado =  null;
		private StringBuilder builder = null;
		private String _Uge;
		private int quantidade = 0;
		private SAMContext contexto = null;

		private bool UAvalida = false;
		private bool UGEValida = false;
		private bool UOValida = false;

		private StringBuilder sbMensagem = null;

		public ImportarBemPatrimonial(SAMContext contexto, Legado legado, DataRowCollection rows, string baseDeDestino)
		{
			this._legado = legado;
			this.contexto = contexto;

			int contador = 1;
			quantidade = 0;

			sbMensagem = new StringBuilder();

            foreach (DataRow row in rows)
            {
                if (bool.Parse(row["Ua Validada"].ToString()))
                {
                    try
                    {
                        ValidaColunasObrigatorias(row);

                        if (sbMensagem.Length > 0)
                        {
                            row["BemPatrimonial Importado"] = "Não";
                            row["BemPatrimonial Mensagem"] = sbMensagem.ToString();
                            continue;
                        }

                        AssetMovements assetMoviments = getAssetMovements(row, baseDeDestino);

                        if (assetMoviments.ManagerUnitId <= 0)
                        {
                            row["BemPatrimonial Importado"] = "Não";
                            row["BemPatrimonial Mensagem"] = "O código da UGE não se encontra na base de dados";
                            continue;
                        }

                        if (assetMoviments.BudgetUnitId <= 0)
                        {
                            row["BemPatrimonial Importado"] = "Não";
                            row["BemPatrimonial Mensagem"] = "Verifique hierarquia de UA com UGE e UO";
                            continue;
                        }

                        Asset asset = getAsset(row, baseDeDestino, assetMoviments);

                        asset.AssetMovements.Add(assetMoviments);

                        if (UAvalida || UGEValida)
                        {
                            int assetId = contexto.Database.SqlQuery<int>(createSQLBemPatrimonial(asset, baseDeDestino)).FirstOrDefault();
                            if (assetId > 0)
                            {
                                assetMoviments.AssetId = assetId;
                                var linhasFetadas = contexto.Database.ExecuteSqlCommand(createSQLBemPatrimonialMovimento(assetMoviments, baseDeDestino));//_legado.createComando(createSQLBemPatrimonialMovimento(assetMoviments, baseDeDestino))
                                                                                                                                                         //  .ExecuteComandoNoQuery(CommandType.Text);
                                quantidade += 1;
                                row["BemPatrimonial Importado"] = "Sim";
                                row["BemPatrimonial Mensagem"] = "Bem patrimonial importado";
                            }
                            else
                            {
                                row["BemPatrimonial Importado"] = "Não";
                                row["BemPatrimonial Mensagem"] = "Bem patrimonial já cadastrado";
                            }
                        }
                        else
                        {
                            row["BemPatrimonial Importado"] = "Não";
                            row["BemPatrimonial Mensagem"] = "Verifique os Códigos de UGE e UA";
                        }
                    }
                    catch (Exception ex)
                    {
                        row["BemPatrimonial Importado"] = "Não";
                        row["BemPatrimonial Mensagem"] = ex.Message;

                        if (ex.Equals("The underlying provider failed on Open."))
                        {
                            contexto.Database.Connection.Open();
                        }

                    }

                    contador += 1;
                }
                else
                {
                    row["BemPatrimonial Importado"] = "Não";
                    row["BemPatrimonial Mensagem"] = "BP não importado pois a UA está inválida";
                }
            }
		}
		public int getTotalImportados()
		{
			return quantidade;
		}
		private string limparCache()
		{
			StringBuilder builder = new StringBuilder();


			//Forçar a escrita das páginas em disco "limpando-as"
			builder.Append("CHECKPOINT ");
			builder.Append(Environment.NewLine);
			//Eliminar as páginas de buffer limpas
			builder.Append("DBCC DROPCLEANBUFFERS ");
			builder.Append(Environment.NewLine);
			//Eliminar todas as entradas do CACHE de "Procedures"
			builder.Append("DBCC FREEPROCCACHE ");
			builder.Append(Environment.NewLine);

			//Limpar as entradas de Cache não utilizadas
			builder.Append("DBCC FREESYSTEMCACHE ('ALL')");
			builder.Append(Environment.NewLine);

			return builder.ToString();
		}
		private DateTime conveterDataLegadoParaData(string dataLegado)
		{
			if (string.IsNullOrWhiteSpace(dataLegado))
				return new DateTime(1900, 1, 1);

			int ano;
			int mes;
			int dia = 1;

			if (dataLegado.Length == 8)
			{
				dia = getNumerosNoTextoDoLegado(dataLegado.Substring(dataLegado.Length - 2, 2));
				mes = getNumerosNoTextoDoLegado(dataLegado.Substring(dataLegado.Length - 4, 2));
				ano = getNumerosNoTextoDoLegado(dataLegado.Substring(0, 4));

			}
			else if (dataLegado.Length == 6)
			{
				mes = getNumerosNoTextoDoLegado(dataLegado.Substring(dataLegado.Length - 2, 2));
				ano = getNumerosNoTextoDoLegado(dataLegado.Substring(0, 4));

			}
			else if (dataLegado.Length == 5)
			{
				mes = getNumerosNoTextoDoLegado(dataLegado.Substring(dataLegado.Length - 1, 1));
				ano = getNumerosNoTextoDoLegado(dataLegado.Substring(0, 4));
			}
			else
			{
				mes = 1;
				ano = 1900;
			}
			try
			{
				DateTime date = new DateTime(ano, mes, dia);
				return date;
			}
			catch (Exception ex)
			{
				return DateTime.Now;
			}
		}
		private int getNumerosNoTextoDoLegado(string texto)
		{
			StringBuilder builder = new StringBuilder();
			foreach (char c in texto)
			{

				if (char.IsNumber(c) && builder.Length < 9)
					builder.Append(c);

			}
			if (builder.Length < 1)
				builder.Append("0");

			return int.Parse(builder.ToString().Trim().TrimEnd().TrimStart().Trim());
		}

		private void AcrescentaSeparador()
		{
			if (sbMensagem.Length > 0)
				sbMensagem.Append(" e ");
		}

		private void ValidaColunasObrigatorias(DataRow row)
		{
			int saidaNumerica = 0;
			sbMensagem.Clear();

			if (row["Sigla"].ToString().Length > 10)
			{
				AcrescentaSeparador();
				sbMensagem.Append("O tamanho da sigla ultrapassa o limite de 10 caracteres.");
			}

			if (!int.TryParse(row["Chapa"].ToString(), out saidaNumerica) || row["Chapa"].ToString().Contains(".") == true || row["Chapa"].ToString().Contains(",") == true)
			{
				AcrescentaSeparador();
				sbMensagem.Append("A chapa deste BP se encontra com um caracter não numérico.");
			}

			if (String.IsNullOrEmpty(row["Código do Orgão"].ToString().Replace(" ", "").TrimEnd().TrimStart().Trim()))
			{
				AcrescentaSeparador();
				sbMensagem.Append("O código do órgão é obrigatório");
			}

			if (String.IsNullOrEmpty(row["Código da UGE"].ToString().Replace(" ", "").TrimEnd().TrimStart().Trim()))
			{
				AcrescentaSeparador();
				sbMensagem.Append("O código da UGE é obrigatório");
			}
		}


		private Asset getAsset(DataRow row, string baseDeDestino, AssetMovements assetMoviments)
		{
			Asset asset = new Models.Asset();

			asset.NumberIdentification = row["Chapa"].ToString().Replace(" ", "").TrimEnd().TrimStart();

			DateTime dataAquisicao;
			DateTime.TryParse(row["Data de Aquisição"].ToString(), out dataAquisicao);

			if (dataAquisicao == null || dataAquisicao == DateTime.MinValue)
				dataAquisicao = DateTime.MaxValue;
			else
				dataAquisicao = (DateTime)_legado.conveterDataLegadoParaData(dataAquisicao.Day.ToString().PadLeft(2, '0') + dataAquisicao.Month.ToString().PadLeft(2, '0') + dataAquisicao.Year.ToString().PadLeft(2, '0'));

			asset.AcquisitionDate = dataAquisicao;

			decimal _valorAquisicao = 0;
			decimal.TryParse(row["Valor de Aquisição"].ToString(), out _valorAquisicao);
			asset.ValueAcquisition = _valorAquisicao;

			asset.MovimentDate = assetMoviments.MovimentDate;

			asset.SerialNumber = (row["Número de Serie"] == DBNull.Value ? "" : _legado.setTamanhoDeCaracteres(row["Número de Serie"].ToString().Replace("'", ""), 50));

			DateTime dataFabricacao;
			DateTime.TryParse(row["Data de Fabricação"].ToString(), out dataFabricacao);

			if (dataFabricacao == null || dataFabricacao == DateTime.MinValue)
				asset.ManufactureDate = null;
			else
				asset.ManufactureDate = (DateTime)_legado.conveterDataLegadoParaData(dataFabricacao.Day.ToString().PadLeft(2, '0') + dataFabricacao.Month.ToString().PadLeft(2, '0') + dataFabricacao.Year.ToString().PadLeft(2, '0'));


			DateTime dataGarantia;
			DateTime.TryParse(row["Data de Garantia"].ToString(), out dataGarantia);

			if (dataGarantia == null || dataGarantia == DateTime.MinValue)
				asset.DateGuarantee = null;
			else
				asset.DateGuarantee = (DateTime)_legado.conveterDataLegadoParaData(dataGarantia.Day.ToString().PadLeft(2, '0') + dataGarantia.Month.ToString().PadLeft(2, '0') + dataGarantia.Year.ToString().PadLeft(2, '0'));

			asset.ChassiNumber = (row["Número do Chassi"] == DBNull.Value ? "" : _legado.setTamanhoDeCaracteres(row["Número do Chassi"].ToString().Replace("'", ""), 20));
			asset.Brand = (row["Marca"] == DBNull.Value ? "" : _legado.setTamanhoDeCaracteres(row["Marca"].ToString().Replace("'", ""), 20));
			asset.Model = (row["Modelo"] == DBNull.Value ? "" : _legado.setTamanhoDeCaracteres(row["Modelo"].ToString().Replace("'", ""), 20));
			asset.NumberPlate = (row["PLACA"] == DBNull.Value ? "" : _legado.setTamanhoDeCaracteres(row["PLACA"].ToString().Replace("'", ""), 8));
			//IDataReader reader;
			//reader = _legado.createComando("SELECT TOP 1 [man].[BudgetUnitId], [man].[Id] FROM [" + baseDeDestino + "].[dbo].[ManagerUnit] [man] INNER JOIN[" + baseDeDestino + "].[dbo].[AdministrativeUnit] [adm] ON [adm].[ManagerUnitId] = [man].[Id] WHERE [man].[Status] = 1 AND [adm].[Code] = " + int.Parse(row["Código da UA"].ToString()) + " ORDER BY [man].[Id] DESC").createDataReader(CommandType.Text);

			int? BudgetUnitId = assetMoviments.BudgetUnitId;
			int? ManagerUnitId = assetMoviments.ManagerUnitId;
			asset.ManagerUnitId = assetMoviments.ManagerUnitId;


			if (row["Sigla Antiga"] == DBNull.Value || row["Sigla Antiga"].ToString().Replace(" ", "") == "")
				asset.OldInitial = null;
			else
			{
				if (ManagerUnitId.HasValue)
				{
					if (BudgetUnitId.HasValue)
					{
						int? OldInitialId = contexto.Database.SqlQuery<int?>("SELECT TOP 1 [ini].[Id] FROM [" + baseDeDestino + "].[dbo].[Initial] [ini] WHERE [ini].[Name] ='" + row["Sigla Antiga"].ToString() + "' AND [InstitutionId] =" + _legado.getInstitutionId().ToString() + " AND [BudgetUnitId] =" + BudgetUnitId.Value.ToString() + " AND ManagerUnitId =" + ManagerUnitId.Value.ToString()).FirstOrDefault();
						if (OldInitialId.HasValue)
							asset.OldInitial = OldInitialId;
						else
						{
							if (BudgetUnitId.HasValue)
							{
								OldInitialId = contexto.Database.SqlQuery<int?>("SELECT TOP 1 [ini].[Id] FROM [" + baseDeDestino + "].[dbo].[Initial] [ini] WHERE [ini].[Name] ='" + row["Sigla Antiga"].ToString() + "' AND [InstitutionId] =" + _legado.getInstitutionId().ToString() + " AND [BudgetUnitId] =" + BudgetUnitId.Value.ToString()).FirstOrDefault();
								if (OldInitialId.HasValue)
									asset.OldInitial = OldInitialId;
							}
						}
					}
				}
				else
				{
					if (BudgetUnitId.HasValue)
					{
						int? OldInitialId = contexto.Database.SqlQuery<int?>("SELECT TOP 1 [ini].[Id] FROM [" + baseDeDestino + "].[dbo].[Initial] [ini] WHERE [ini].[Name] ='" + row["Sigla Antiga"].ToString() + "' AND [InstitutionId] =" + _legado.getInstitutionId().ToString() + " AND [BudgetUnitId] =" + BudgetUnitId.Value.ToString()).FirstOrDefault();
						if (OldInitialId.HasValue)
							asset.OldInitial = OldInitialId;
					}
				}

			}
			if (ManagerUnitId.HasValue)
			{
				if (BudgetUnitId.HasValue)
				{
					if (string.IsNullOrEmpty(row["Sigla"].ToString()))
					{
						contexto.Database.ExecuteSqlCommand(createSQLSiglaGenerica(assetMoviments, baseDeDestino));
						asset.InitialId = contexto.Database.SqlQuery<int>("SELECT TOP 1 Id FROM [" + baseDeDestino + "].[dbo].[Initial] WHERE InstitutionId = " + assetMoviments.InstitutionId + " AND ManagerUnitId = " + assetMoviments.ManagerUnitId + " AND Description = 'SIGLA_GENERICA'").FirstOrDefault();
						asset.InitialName = contexto.Database.SqlQuery<string>("SELECT TOP 1 Name FROM [" + baseDeDestino + "].[dbo].[Initial] WHERE InstitutionId = " + assetMoviments.InstitutionId + " AND ManagerUnitId = " + assetMoviments.ManagerUnitId + " AND Description = 'SIGLA_GENERICA'").FirstOrDefault();
					}
					else
					{
						int? InitialId = contexto.Database.SqlQuery<int?>("SELECT TOP 1 [ini].[Id] FROM [" + baseDeDestino + "].[dbo].[Initial] [ini] WHERE [ini].[Name] ='" + row["Sigla"].ToString() + "' AND [InstitutionId] =" + _legado.getInstitutionId().ToString() + " AND [BudgetUnitId] =" + BudgetUnitId.Value.ToString() + " AND ManagerUnitId =" + ManagerUnitId.Value.ToString()).FirstOrDefault();
						if (InitialId.HasValue)
						{
							asset.InitialName = row["Sigla"].ToString();
							asset.InitialId = InitialId.Value;
						}
						else
						{
							InitialId = contexto.Database.SqlQuery<int?>("SELECT TOP 1 [ini].[Id] FROM [" + baseDeDestino + "].[dbo].[Initial] [ini] WHERE [ini].[Name] ='" + row["Sigla"].ToString() + "' AND [InstitutionId] =" + _legado.getInstitutionId().ToString() + " AND [BudgetUnitId] =" + BudgetUnitId.Value.ToString()).FirstOrDefault();
							if (InitialId.HasValue)
							{
								asset.InitialName = row["Sigla"].ToString();
								asset.InitialId = InitialId.Value;
							}
							else
							{
								InitialId = contexto.Database.SqlQuery<int?>("SELECT TOP 1 [ini].[Id] FROM [" + baseDeDestino + "].[dbo].[Initial] [ini] WHERE [ini].[Name] ='" + row["Sigla"].ToString() + "' AND [InstitutionId] =" + _legado.getInstitutionId().ToString()).FirstOrDefault();
								if (InitialId.HasValue)
								{
									asset.InitialName = row["Sigla"].ToString();
									asset.InitialId = InitialId.Value;
								}
								else
								{
									contexto.Database.ExecuteSqlCommand(createSQLSiglaGenerica(assetMoviments, baseDeDestino));
									asset.InitialId = contexto.Database.SqlQuery<int>("SELECT TOP 1 Id FROM [" + baseDeDestino + "].[dbo].[Initial] WHERE InstitutionId = " + assetMoviments.InstitutionId + " AND ManagerUnitId = " + assetMoviments.ManagerUnitId + " AND Description = 'SIGLA_GENERICA'").FirstOrDefault();
									asset.InitialName = contexto.Database.SqlQuery<string>("SELECT TOP 1 Name FROM [" + baseDeDestino + "].[dbo].[Initial] WHERE InstitutionId = " + assetMoviments.InstitutionId + " AND ManagerUnitId = " + assetMoviments.ManagerUnitId + " AND Description = 'SIGLA_GENERICA'").FirstOrDefault();
								}
							}
						}
					}
				}
			}
			else
			{
				if (string.IsNullOrEmpty(row["Sigla"].ToString()))
				{
					createSQLSiglaGenerica(assetMoviments, baseDeDestino);
					asset.InitialId = contexto.Database.SqlQuery<int>("SELECT TOP 1 [ini].[Id] FROM [" + baseDeDestino + "].[dbo].[Initial] [ini] WHERE [ini].[Description] = 'SIGLA_GENERICA'").FirstOrDefault();
					asset.InitialName = contexto.Database.SqlQuery<string>("SELECT TOP 1 [ini].[Name] FROM [" + baseDeDestino + "].[dbo].[Initial] [ini] WHERE [ini].[Description] = 'SIGLA_GENERICA'").FirstOrDefault();
				}
				else
				{
					if (BudgetUnitId.HasValue)
					{
						int? InitialId = contexto.Database.SqlQuery<int?>("SELECT TOP 1 [ini].[Id] FROM [" + baseDeDestino + "].[dbo].[Initial] [ini] WHERE [ini].[Name] ='" + row["Sigla"].ToString() + "' AND [InstitutionId] =" + _legado.getInstitutionId().ToString() + " AND [BudgetUnitId] =" + BudgetUnitId.Value.ToString()).FirstOrDefault();
						if (InitialId.HasValue)
						{
							asset.InitialName = row["Sigla"].ToString();
							asset.InitialId = InitialId.Value;
						}
					}
				}
			}


			bool notaFiscalValida = false;
			Int32 numberOut = 0;

			if (row["Nota Fiscal"] == null || row["Nota Fiscal"].ToString().Trim() == string.Empty)
			{
				notaFiscalValida = false;
			}
			else
			{
				notaFiscalValida = Int32.TryParse(row["Nota Fiscal"].ToString().Replace('.', ' '), out numberOut);
			}

			if (notaFiscalValida == true)
			{
				asset.NumberDoc = row["Nota Fiscal"].ToString().Replace('.', ' ');
			}
			else
			{
				asset.NumberDoc = getNumeroDeDocumento(row["Código da UGE"].ToString()); //código da UGE vai vim com 4 digitos
			}

			//asset.NumberDoc = getNumeroDeDocumento(row["Código da UGE"].ToString());

			if (row["Chapa Antiga"] == DBNull.Value || row["Chapa Antiga"].ToString() == "")
				asset.OldNumberIdentification = null;
			else
				asset.OldNumberIdentification = row["Chapa Antiga"].ToString();
			asset.Empenho = (row["Número do Empenho"] == DBNull.Value ? "" : _legado.setTamanhoDeCaracteres(row["Número do Empenho"].ToString().Replace("'", ""), 15));
			asset.AdditionalDescription = (row["Descrição Adicional"] == DBNull.Value ? "" : _legado.setTamanhoDeCaracteres(row["Descrição Adicional"].ToString().Replace("'", ""), 20));


			string _shortDescriptionItem = string.Empty;

			if (string.IsNullOrEmpty(row["Descrição do Item de Material"].ToString()))
			{
				_shortDescriptionItem = "Descrição do Item Vazio";
			}
			else
			{
				_shortDescriptionItem = _legado.setTamanhoDeCaracteres(row["Descrição do Item de Material"].ToString().Replace("'", ""), 255);
			}

			int ShortDescriptionItemId = contexto.Database.SqlQuery<int>("SELECT TOP 1 [Id] FROM [" + baseDeDestino + "].[dbo].[ShortDescriptionItem] WHERE [Description] = '" + _shortDescriptionItem + "' ORDER BY [Id] DESC").FirstOrDefault();
			asset.ShortDescriptionItemId = ShortDescriptionItemId;

			int codItemMaterial = 0;
			int.TryParse(row["Código Item de Material"].ToString().Replace(" ", "").TrimEnd().TrimStart(), out codItemMaterial);

			if (codItemMaterial == 0)
				asset.MaterialItemCode = 99999;
			else
				asset.MaterialItemCode = int.Parse(row["Código Item de Material"].ToString().Replace("'", "").Replace(" ", "").Replace("-", "").Trim().TrimEnd().TrimStart());

			asset.MaterialItemDescription = _legado.setTamanhoDeCaracteres(row["Descrição do Item de Material"].ToString().Replace("'", ""), 120);
			asset.Status = true;

			asset.MovementTypeId = (int)EnumMovimentType.IncorporacaoDeInventarioInicial;
            asset.IndicadorOrigemInventarioInicial = (int)EnumOrigemInventarioInicial.CargaDosBPs;

            asset.SupplierId = contexto.Database.SqlQuery<int?>("SELECT TOP 1 [Id] FROM [" + baseDeDestino + "].[dbo].[Supplier]  WHERE [CPFCNPJ] ='" + _legado.removerCaracteresCpfCnpj(row["CNPJ do Fornecedor"].ToString(), false) + "' AND [Name] ='" + _legado.setTamanhoDeCaracteres(row["Nome do Fornecedor"].ToString().Replace("'", ""), 60) + "'").FirstOrDefault();
			asset.OutSourcedId = contexto.Database.SqlQuery<int?>("SELECT TOP 1 [Id] FROM [" + baseDeDestino + "].[dbo].[OutSourced]  WHERE [CPFCNPJ] ='" + _legado.removerCaracteresCpfCnpj(row["CPF/CNPJ do Terceiro"].ToString(), false) + "' AND [Name] ='" + _legado.setTamanhoDeCaracteres(row["Nome do Terceiro"].ToString().Replace("'", ""), 60) + "'").FirstOrDefault();

			asset.flagAcervo = row["Acervo"].ToString().Trim().ToLower() == "sim" ? true : false;
            if (asset.flagAcervo == true) {
                asset.MaterialItemCode = 5628156;
                asset.MaterialItemDescription = "Acervos";
                asset.MaterialGroupCode = 99;
            }

            asset.flagTerceiro = row["Terceiro"].ToString().ToLower() == "sim" ? true : false;
            if (asset.flagTerceiro == true)
            {
                asset.flagAcervo = false;
                asset.MaterialItemCode = 5628121;
                asset.MaterialItemDescription = "Bens de Terceiros";
                asset.MaterialGroupCode = 99;
            }

            return asset;
		}



		private AssetMovements getAssetMovements(DataRow row, string baseDeDestino)
		{
			UAvalida = false;
			UGEValida = false;
			UOValida = false;

			AssetMovements assetMoviments = new AssetMovements();

            getContaContabil(assetMoviments, row, baseDeDestino);

			assetMoviments.InstitutionId = _legado.getInstitutionId();

			int? ManagerUnitId = null;

			if (!string.IsNullOrEmpty(row["Código da UGE"].ToString().Replace(" ", "").TrimEnd().TrimStart().Trim()))
			{
				ManagerUnitId = contexto.Database.SqlQuery<int?>("SELECT TOP 1 [man].[Id] FROM [" + baseDeDestino + "].[dbo].[ManagerUnit] [man] INNER JOIN [" + baseDeDestino + "].[dbo].[BudgetUnit] [bud] ON [bud].Id = [man].BudgetUnitId INNER JOIN [" + baseDeDestino + "].[dbo].[Institution] [ins] ON [ins].Id = [bud].InstitutionId INNER JOIN [" + baseDeDestino + "].[dbo].[AdministrativeUnit] [adm] ON [adm].[ManagerUnitId] = [man].[Id] WHERE [man].[Status] = 1 AND CONVERT(int, [adm].[Code]) = '" + int.Parse(row["Código da UA"].ToString().Trim()) + "' AND CONVERT(int, [ins].[Id]) = " + assetMoviments.InstitutionId + " ORDER BY [man].[Id] DESC").FirstOrDefault();
			}

			if (ManagerUnitId.HasValue && ManagerUnitId.Value != 0)
			{
				assetMoviments.ManagerUnitId = ManagerUnitId.Value;
				UGEValida = true;
			}
			
			int? AdministrativeUnitId = null;

			if (UGEValida == true)
			{
				if (!string.IsNullOrEmpty(row["Código da UA"].ToString().Replace(" ", "").TrimEnd().TrimStart().Trim()))
				{
					AdministrativeUnitId = contexto.Database.SqlQuery<int?>("SELECT TOP 1 [adm].[Id] FROM [" + baseDeDestino + "].[dbo].[ManagerUnit] [man] INNER JOIN [" + baseDeDestino + "].[dbo].[AdministrativeUnit] [adm] ON [adm].[ManagerUnitId] = [man].[Id] WHERE [man].[Status] = 1 AND CONVERT(int, [adm].[Code]) = " + int.Parse(row["Código da UA"].ToString().Trim()) + " ORDER BY [man].[Id] DESC").FirstOrDefault();

					if (AdministrativeUnitId.HasValue && AdministrativeUnitId != 0)
					{
						assetMoviments.AdministrativeUnitId = AdministrativeUnitId.Value;
						UAvalida = true;
					}
					else
					{
						contexto.Database.ExecuteSqlCommand(createSQLUAGenerica(assetMoviments, baseDeDestino));
						assetMoviments.AdministrativeUnitId = contexto.Database.SqlQuery<int>("SELECT TOP 1 Id FROM [" + baseDeDestino + "].[dbo].[AdministrativeUnit] WHERE ManagerUnitId = " + assetMoviments.ManagerUnitId + " AND Code = '99999999'").FirstOrDefault();
					}
				}

				int? BudgetUnitId = null;

				if (UGEValida == true)
				{
					BudgetUnitId = contexto.Database.SqlQuery<int>("SELECT TOP 1 [man].[BudgetUnitId] FROM [" + baseDeDestino + "].[dbo].[ManagerUnit] [man] WHERE CONVERT(int, [man].[Code]) = '" + int.Parse(row["Código da UGE"].ToString().Trim()) + "' ORDER BY [man].[Id] DESC").FirstOrDefault();

					if (BudgetUnitId.HasValue && BudgetUnitId.Value != 0)
					{
						assetMoviments.BudgetUnitId = BudgetUnitId.Value;
						UOValida = true;
					}
				}

				if (UOValida == true)
				{
					int MovementTypeId = contexto.Database.SqlQuery<int>("SELECT TOP 1 [Id] FROM [" + baseDeDestino + "].[dbo].[MovementType]  WHERE [Code] = 5").FirstOrDefault();
					assetMoviments.MovementTypeId = MovementTypeId;

					//string mesAnoRefUGE = string.Empty;
					//mesAnoRefUGE = contexto.Database.SqlQuery<string>("SELECT TOP 1 [ManagmentUnit_YearMonthReference] FROM [" + baseDeDestino + "].[dbo].[ManagerUnit] WHERE [Id] =" + ManagerUnitId).FirstOrDefault();

					//DateTime dataMovimento = new DateTime(int.Parse(mesAnoRefUGE.Substring(0, 4)), int.Parse(mesAnoRefUGE.Substring(4, 2)), 1);
					//assetMoviments.MovimentDate = dataMovimento;

					string mesAnoIncUGE = string.Empty;
					mesAnoIncUGE = contexto.Database.SqlQuery<string>("SELECT TOP 1 [ManagmentUnit_YearMonthStart] FROM [" + baseDeDestino + "].[dbo].[ManagerUnit] WHERE [Id] =" + ManagerUnitId).FirstOrDefault();

					DateTime dataMovimento = new DateTime(int.Parse(mesAnoIncUGE.Substring(0, 4)), int.Parse(mesAnoIncUGE.Substring(4, 2)), 1);
					assetMoviments.MovimentDate = dataMovimento;


					assetMoviments.Observation = (row["Observações"] == DBNull.Value ? "" : _legado.setTamanhoDeCaracteres(row["Observações"].ToString().Replace("'", ""), 500));

					if (UAvalida == true)
					{
						int? ResponsibleId = contexto.Database.SqlQuery<int?>("SELECT TOP 1 [resp].[Id] FROM [" + baseDeDestino + "].[dbo].[Responsible] [resp] INNER JOIN [" + baseDeDestino + "].[dbo].[AdministrativeUnit][adm] ON [adm].[Id] = [resp].[AdministrativeUnitId] WHERE [resp].[CPF] ='" + _legado.removerCaracteresCpfCnpj(row["CPF do Responsável"].ToString().Replace(".", "").Replace("-", ""), true) + "' AND [resp].[Name] = '" + _legado.setTamanhoDeCaracteres(row["Nome do Responsável"].ToString().Replace("'", ""), 100) + "' AND CONVERT(int, [adm].[Code]) =  " + int.Parse(row["Código da UA"].ToString().Trim()) + " ORDER BY  [resp].[Id] DESC").FirstOrDefault();

						if (ResponsibleId == null)
						{
							Responsible gravaResp = new Responsible();
							gravaResp.Name = _legado.setTamanhoDeCaracteres(row["Nome do Responsável"].ToString().Replace("'", ""), 100);
							gravaResp.Position = _legado.setTamanhoDeCaracteres(row["Cargo do Responsável"].ToString(), 100);
							gravaResp.Status = true;
							gravaResp.AdministrativeUnitId = AdministrativeUnitId.Value;
							gravaResp.CPF = _legado.removerCaracteresCpfCnpj(row["CPF do Responsável"].ToString().Replace(".", "").Replace("-", ""), true);
							gravaResp.Email = null;

							contexto.Responsibles.Add(gravaResp);
							contexto.SaveChanges();


							ResponsibleId = gravaResp.Id; 
						}

						assetMoviments.ResponsibleId = ResponsibleId;

						if (row["Código da Divisão"].ToString() != "" && row["Código da UA"].ToString() != "")
						{
							int? SectionId = contexto.Database.SqlQuery<int?>("SELECT TOP 1 [sec].[Id] FROM [" + baseDeDestino + "].[dbo].[Section] [sec] INNER JOIN [" + baseDeDestino + "].[dbo].[AdministrativeUnit] [adm] ON [adm].[Id] = [sec].[AdministrativeUnitId] WHERE CONVERT(int, [sec].[Code]) =" + int.Parse(row["Código da Divisão"].ToString().Trim()) + " AND [adm].[Code] =  " + _legado.getNumerosNoTextoDoLegado(row["Código da UA"].ToString()) + " ORDER BY [Id] DESC").FirstOrDefault();
							assetMoviments.SectionId = SectionId;
						}
						else if (row["Nome da Divisão"].ToString() != "" && row["Código da UA"].ToString() != "")
						{
							int? SectionId = contexto.Database.SqlQuery<int?>("SELECT TOP 1 [sec].[Id] FROM [" + baseDeDestino + "].[dbo].[Section] [sec] INNER JOIN [" + baseDeDestino + "].[dbo].[AdministrativeUnit] [adm] ON [adm].[Id] = [sec].[AdministrativeUnitId] WHERE [sec].[Description] ='" + row["Nome da Divisão"].ToString() + "' AND [adm].[Code] =  " + _legado.getNumerosNoTextoDoLegado(row["Código da UA"].ToString()) + " ORDER BY [Id] DESC").FirstOrDefault();
							assetMoviments.SectionId = SectionId;
						}
					}

					int? StateConservationId = contexto.Database.SqlQuery<int?>("SELECT TOP 1 [Id] FROM [" + baseDeDestino + "].[dbo].[StateConservation] WHERE [Description] ='" + row["Estado de Conservação"].ToString().Replace("'", "") + "' ORDER BY [Id] DESC").FirstOrDefault();

					if (StateConservationId == null)
					{
						StateConservationId = contexto.Database.SqlQuery<int?>("SELECT TOP 1 [Id] FROM [" + baseDeDestino + "].[dbo].[StateConservation] WHERE [Description] ='ESTADO_GENERICO' ORDER BY [Id] DESC").FirstOrDefault();
					}

					if (StateConservationId.HasValue)
					{
						assetMoviments.StateConservationId = StateConservationId.Value;
					}
					else
					{
						row["Estado de Conservação Importado"] = "Não";
						row["Estado de Conservação Mensagem"] = "Status informado não existente no sistema";
					}
					assetMoviments.Status = true;

					SAM.Web.Common.ComumLayout comumLayout = new SAM.Web.Common.ComumLayout();
					User u = comumLayout.CurrentUser();

					assetMoviments.Login = u.CPF;
					assetMoviments.DataLogin = DateTime.Now;
				}
			}

			return assetMoviments;
		}
		private String getNumeroDeDocumento(string Uge)
		{
			this._Uge = Uge;

			return DateTime.Now.Year.ToString() + Uge.PadLeft(6,'0') + "0001";
		}

        private void getContaContabil(AssetMovements assetMoviments, DataRow row, string baseDeDestino) {
            string RelacionadoBP = "0";
            if (row["Acervo"].ToString().Trim().ToLower() == "sim")
                RelacionadoBP = "1";

            if (row["Terceiro"].ToString().Trim().ToLower() == "sim")
                RelacionadoBP = "2";


            int? AuxiliaryAccountId = contexto.Database.SqlQuery<int?>("SELECT TOP 1 [Id] FROM [" + baseDeDestino + "].[dbo].[AuxiliaryAccount] WHERE [BookAccount] = " + _legado.getNumerosNoTextoDoLegado(row["Contabil Auxiliar"].ToString()) + " AND RelacionadoBP = " + RelacionadoBP + " AND [Status] = 1 ORDER BY [Id] DESC").FirstOrDefault();
            if (AuxiliaryAccountId.HasValue)
                assetMoviments.AuxiliaryAccountId = AuxiliaryAccountId;
            else
            {
                AuxiliaryAccountId = contexto.Database.SqlQuery<int?>("SELECT TOP 1 [Id] FROM [" + baseDeDestino + "].[dbo].[AuxiliaryAccount] WHERE [BookAccount] = " + _legado.getNumerosNoTextoDoLegado(row["Contabil Auxiliar"].ToString()) + " AND RelacionadoBP = " + RelacionadoBP + " ORDER BY [Id] DESC").FirstOrDefault();
                if (AuxiliaryAccountId.HasValue)
                    assetMoviments.AuxiliaryAccountId = AuxiliaryAccountId;
                else
                {
                    if (RelacionadoBP != "0")
                    {
                        AuxiliaryAccountId = contexto.Database.SqlQuery<int?>("SELECT TOP 1 [Id] FROM [" + baseDeDestino + "].[dbo].[AuxiliaryAccount] WHERE RelacionadoBP = " + RelacionadoBP + " ORDER BY [Id] DESC").FirstOrDefault();
                        assetMoviments.AuxiliaryAccountId = AuxiliaryAccountId;
                    }
                    else {
                        row["ContaAuxliar Importado"] = "Não";
                        row["ContaAuxiliar Mensagem"] = "conta informada não encontrada";
                    }
                    
                }
            }
        }
		public string createSQLBemPatrimonial(Asset asset, string baseDeDestino)
		{
			AssetMovements movimento = asset.AssetMovements.FirstOrDefault();

			builder = new StringBuilder();
			int bemPatrimonialId = 0;

			builder.Append("SELECT TOP 1 [aset].[Id] FROM [" + baseDeDestino + "].[dbo].[Asset] [aset] ");
			builder.Append(" INNER JOIN [" + baseDeDestino + "].[dbo].[AssetMovements] [mov] ON [mov].[AssetId] = [aset].[Id] ");
			builder.Append(" INNER JOIN [" + baseDeDestino + "].[dbo].[BudgetUnit] [bug] ON [bug].[Id] = [mov].[BudgetUnitId] ");
			builder.Append(" INNER JOIN [" + baseDeDestino + "].[dbo].[ManagerUnit] [man] ON [man].[Id] = [mov].[ManagerUnitId] ");
			builder.Append("    WHERE [aset].[InitialId] = " + asset.InitialId.ToString() + " AND [aset].[NumberIdentification] = '" + asset.NumberIdentification + "'");
            builder.Append("      AND [aset].[DiferenciacaoChapa] = '' ");
            builder.Append("      AND [man].[Id] =" + movimento.ManagerUnitId.ToString());
			builder.Append("      AND [bug].[Id] =" + movimento.BudgetUnitId.ToString());
			builder.Append("      AND [bug].[InstitutionId] =" + movimento.InstitutionId.ToString());

			bemPatrimonialId = contexto.Database.SqlQuery<int>(builder.ToString()).FirstOrDefault();
			builder = null;


			if (bemPatrimonialId < 1)
			{
				builder = new StringBuilder();

				builder.Append(" SET DATEFORMAT DMY ");
				builder.Append(" INSERT INTO [" + baseDeDestino + "].[dbo].[Asset] ");
				builder.Append(" ( [InitialId] ");
				builder.Append(" ,[InitialName] ");
				builder.Append(" ,[NumberIdentification] ");
				builder.Append(" ,[OutSourcedId] ");
				builder.Append(" ,[MovementTypeId] ");
				builder.Append(" ,[SupplierId] ");
				builder.Append(" ,[AcquisitionDate] ");
				builder.Append(" ,[ValueAcquisition] ");
				builder.Append(" ,[ValueUpdate] ");
				builder.Append(" ,[SerialNumber] ");
				builder.Append(" ,[ManufactureDate] ");
				builder.Append(" ,[DateGuarantee] ");
				builder.Append(" ,[ChassiNumber] ");
				builder.Append(" ,[Brand] ");
				builder.Append(" ,[Model] ");
				builder.Append(" ,[NumberPlate] ");
				builder.Append(" ,[AdditionalDescription] ");
				builder.Append(" ,[OldInitial] ");
				builder.Append(" ,[OldNumberIdentification] ");
				builder.Append(" ,[Status] ");
				builder.Append(" ,[LifeCycle] ");
				builder.Append(" ,[RateDepreciationMonthly] ");
				builder.Append(" ,[ResidualValue] ");
				builder.Append(" ,[AceleratedDepreciation] ");
				builder.Append(" ,[MaterialItemCode] ");
				builder.Append(" ,[MaterialItemDescription] ");
				builder.Append(" ,[MaterialGroupCode] ");
				builder.Append(" ,[Empenho] ");
				builder.Append(" ,[ShortDescriptionItemId] ");
				builder.Append(" ,[flagVerificado] ");
				builder.Append(" ,[flagDepreciaAcumulada] ");
				builder.Append(" ,[NumberDoc] ");
				builder.Append(" ,[MovimentDate] ");
				builder.Append(" ,[ManagerUnitId] ");
				builder.Append(" ,[flagAcervo] ");
				builder.Append(" ,[flagTerceiro] ");
                builder.Append(" ,[flagVindoDoEstoque] ");
                builder.Append(" ,[IndicadorOrigemInventarioInicial]) ");
                builder.Append(" VALUES");
				builder.Append(" (" + asset.InitialId.ToString());
				builder.Append(" ,'" + asset.InitialName + "'");
				builder.Append(" ,'" + (asset.NumberIdentification != null && asset.NumberIdentification.Length > 250 ? asset.NumberIdentification.Substring(0, 250) : asset.NumberIdentification) + "'");
				builder.Append(" ," + (asset.OutSourcedId.HasValue ? asset.OutSourcedId.ToString() : "Null"));
				builder.Append(" ," + asset.MovementTypeId.ToString());
				builder.Append(" ," + (asset.SupplierId.HasValue ? asset.SupplierId.ToString() : "Null"));
				builder.Append(" ,'" + asset.AcquisitionDate.ToShortDateString() + "'");
				builder.Append(" ," + asset.ValueAcquisition.ToString().Replace(",", "."));
				builder.Append(" ," + (asset.ValueUpdate.HasValue ? asset.ValueUpdate.ToString().Replace(",", ".") : "Null"));
				builder.Append(" ,'" + (asset.SerialNumber != null && asset.SerialNumber.Length > 50 ? asset.SerialNumber.Substring(0, 50) : asset.SerialNumber) + "'");
				builder.Append(" ," + (asset.ManufactureDate.HasValue ? "'" + asset.ManufactureDate.Value.ToShortDateString() + "'" : "Null"));
				builder.Append(" ," + (asset.DateGuarantee.HasValue ? "'" + asset.DateGuarantee.Value.ToShortDateString() + "'" : "Null"));
				builder.Append(" ,'" + asset.ChassiNumber + "'");
				builder.Append(" ,'" + (asset.Brand != null && asset.Brand.Length > 20 ? asset.Brand.Substring(0, 20) : asset.Brand) + "'");
				builder.Append(" ,'" + (asset.Model != null && asset.Model.Length > 20 ? asset.Model.Substring(0, 20) : asset.Model) + "'");
				builder.Append(" ,'" + (asset.NumberPlate != null && asset.NumberPlate.Length > 8 ? asset.NumberPlate.Substring(0, 8) : asset.NumberPlate) + "'");
				builder.Append(" ,'" + (asset.AdditionalDescription != null && asset.AdditionalDescription.Length > 20 ? asset.AdditionalDescription.Substring(0, 20) : asset.AdditionalDescription) + "'");
				builder.Append(" ," + (asset.OldInitial.HasValue ? asset.OldInitial.ToString() : "Null"));
				builder.Append(" ,'" + (asset.OldNumberIdentification != null && asset.OldNumberIdentification.Length > 250 ? asset.OldNumberIdentification.Substring(0, 250) : asset.OldNumberIdentification) + "'");
				builder.Append(" , 1");
				builder.Append(" ," + asset.LifeCycle.ToString());
				builder.Append(" ," + asset.RateDepreciationMonthly.ToString().Replace(",", "."));
				builder.Append(" ," + asset.ResidualValue.ToString().Replace(",", "."));
				builder.Append(" ,1");
				builder.Append(" ," + asset.MaterialItemCode.ToString());
				builder.Append(" ,'" + (asset.MaterialItemDescription != null && asset.MaterialItemDescription.Length > 120 ? asset.MaterialItemDescription.Substring(0, 120).Replace("'", "") : asset.MaterialItemDescription.Replace("'", "")) + "'");
				builder.Append(" ," + asset.MaterialGroupCode);
				builder.Append(" ,'" + (asset.Empenho != null && asset.Empenho.Length > 15 ? asset.Empenho.Substring(0, 15).Replace("'", "") : asset.Empenho.Replace("'", "")) + "'");
				builder.Append(" ," + asset.ShortDescriptionItemId.ToString());
				builder.Append(" ,NULL");
				builder.Append(" ,NULL");
				builder.Append(" ,'" + (asset.NumberDoc != null && asset.NumberDoc.Length > 15 ? asset.NumberDoc.Substring(0, 15).Replace("'", "") : asset.NumberDoc.Replace("'", "")) + "'");
				builder.Append(" ,'" + asset.MovimentDate + "'");
				builder.Append(" ," + asset.ManagerUnitId);
				builder.Append(" ," + (asset.flagAcervo == null || asset.flagAcervo == false ? 0 : 1));
				builder.Append(" ," + (asset.flagTerceiro == null || asset.flagTerceiro == false ? 0 : 1));
                builder.Append(" ,0");
                builder.Append(" ," + asset.IndicadorOrigemInventarioInicial + ")");
                builder.Append(" SELECT CAST(@@IDENTITY AS INT) ");

				return builder.ToString();
			}
			else
				return "SELECT - 1";
		}

		private string createSQLBemPatrimonialMovimento(AssetMovements assetMoviments, string baseDeDestino)
		{
			builder = new StringBuilder();

			builder.Append(" SET DATEFORMAT DMY ");
			builder.Append(" INSERT INTO [" + baseDeDestino + "].[dbo].[AssetMovements] ");
			builder.Append(" ([AssetId] ");
			builder.Append(" ,[Status] ");
			builder.Append(" ,[MovimentDate] ");
			builder.Append(" ,[MovementTypeId] ");
			builder.Append(" ,[StateConservationId] ");
			builder.Append(" ,[InstitutionId] ");
			builder.Append(" ,[BudgetUnitId] ");
			builder.Append(" ,[ManagerUnitId] ");
			builder.Append(" ,[AdministrativeUnitId] ");
			builder.Append(" ,[SectionId] ");
			builder.Append(" ,[AuxiliaryAccountId] ");
			builder.Append(" ,[ResponsibleId] ");
			builder.Append(" ,[SourceDestiny_ManagerUnitId] ");
			builder.Append(" ,[AssetTransferenciaId] ");
			builder.Append(" ,[ExchangeId] ");
			builder.Append(" ,[ExchangeDate] ");
			builder.Append(" ,[NumberPurchaseProcess]");
			builder.Append(" ,[Observation]");
			builder.Append(" ,[ExchangeUserId] ");
			builder.Append(" ,[Login] ");
			builder.Append(" ,[DataLogin]) ");
			builder.Append(" VALUES ");
			builder.Append(" (" + assetMoviments.AssetId);
			builder.Append(" ,1");
			builder.Append(" ,'" + assetMoviments.MovimentDate.ToShortDateString() + "'");
			builder.Append(" ," + assetMoviments.MovementTypeId);
			builder.Append(" ," + assetMoviments.StateConservationId);
			builder.Append(" ," + assetMoviments.InstitutionId);
			builder.Append(" ," + assetMoviments.BudgetUnitId);
			builder.Append(" ," + assetMoviments.ManagerUnitId);
			builder.Append(" ," + (assetMoviments.AdministrativeUnitId.HasValue ? assetMoviments.AdministrativeUnitId.ToString() : "Null"));
			builder.Append(" ," + (assetMoviments.SectionId.HasValue ? assetMoviments.SectionId.ToString() : "Null"));
			builder.Append(" ," + (assetMoviments.AuxiliaryAccountId.HasValue && assetMoviments.AuxiliaryAccountId.Value > 0 ? assetMoviments.AuxiliaryAccountId.ToString() : "Null"));
			builder.Append(" ," + (assetMoviments.ResponsibleId.HasValue ? assetMoviments.ResponsibleId.ToString() : "Null"));
			builder.Append(" ," + (assetMoviments.SourceDestiny_ManagerUnitId.HasValue ? assetMoviments.SourceDestiny_ManagerUnitId.ToString() : "Null"));
			builder.Append(" ," + (assetMoviments.AssetTransferenciaId.HasValue ? assetMoviments.AssetTransferenciaId.ToString() : "Null"));
			builder.Append(" ," + (assetMoviments.ExchangeId.HasValue ? assetMoviments.ExchangeId.ToString() : "Null"));
			builder.Append(" ," + (assetMoviments.ExchangeDate.HasValue ? assetMoviments.ExchangeDate.ToString() : "Null"));
			builder.Append(" ,'" + (assetMoviments.NumberPurchaseProcess != null && assetMoviments.NumberPurchaseProcess.Length > 25 ? assetMoviments.NumberPurchaseProcess.Substring(0, 25) : assetMoviments.NumberPurchaseProcess) + "'");
			builder.Append(" ," + (string.IsNullOrEmpty(assetMoviments.Observation) ? "Null" : "'" + assetMoviments.Observation + "'"));
			builder.Append(" ," + (assetMoviments.ExchangeUserId.HasValue ? assetMoviments.ExchangeUserId.ToString() : "Null"));
			builder.Append(" ,'" + assetMoviments.Login + "'");
			builder.Append(" ,'" + assetMoviments.DataLogin + "'" + ")");

			return builder.ToString();
		}

		private string createSQLSiglaGenerica(AssetMovements assetMovement, string baseDeDestino)
		{
			builder = new StringBuilder();

			try
			{
				builder.Append("IF NOT EXISTS(SELECT TOP 1 Id FROM [" + baseDeDestino + "].[dbo].[Initial] WHERE InstitutionId = " + assetMovement.InstitutionId + " AND ManagerUnitId = " + assetMovement.ManagerUnitId + " AND Description = 'SIGLA_GENERICA')");
				builder.Append(Environment.NewLine);
				builder.Append("BEGIN");
				builder.Append(Environment.NewLine);
				builder.Append(" INSERT INTO [" + baseDeDestino + "].[dbo].[Initial] ");
				builder.Append("([Name] ");
				builder.Append(",[Description] ");
				builder.Append(",[BarCode] ");
				builder.Append(",[InstitutionId] ");
				builder.Append(",[BudgetUnitId] ");
				builder.Append(",[ManagerUnitId] ");
				builder.Append(",[Status])");


				builder.Append(" VALUES (");
				builder.Append("'SIGLA_GENE'");
				builder.Append("," + "'SIGLA_GENERICA'");
				builder.Append("," + "NULL");
				builder.Append("," + assetMovement.InstitutionId);
				builder.Append("," + "NULL");
				builder.Append("," + assetMovement.ManagerUnitId);
				builder.Append("," + "1)");
				builder.Append(Environment.NewLine);
				builder.Append("END");
				return builder.ToString();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		private string createSQLUOGenerica(AssetMovements assetMovement, string baseDeDestino)
		{
			builder = new StringBuilder();

			try
			{
				builder.Append("IF NOT EXISTS(SELECT TOP 1 Id FROM [" + baseDeDestino + "].[dbo].[BudgetUnit] WHERE InstitutionId = " + assetMovement.InstitutionId + " AND Code = '99999')");
				builder.Append(Environment.NewLine);
				builder.Append("BEGIN");
				builder.Append(Environment.NewLine);
				builder.Append(" INSERT INTO [" + baseDeDestino + "].[dbo].[BudgetUnit] ");
				builder.Append("([InstitutionId] ");
				builder.Append(",[Code] ");
				builder.Append(",[Description] ");
				builder.Append(",[Direct] ");
				builder.Append(",[Status])");

				builder.Append(" VALUES (");
				builder.Append(assetMovement.InstitutionId);
				builder.Append("," + "'99999' ");
				builder.Append("," + "'UO_GENERICA' ");
				builder.Append("," + "1 ");
				builder.Append("," + "1)");
				builder.Append(Environment.NewLine);
				builder.Append("END");
				return builder.ToString();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		private string createSQLUGEGenerica(AssetMovements assetMovement, string baseDeDestino)
		{
			builder = new StringBuilder();

			try
			{
				builder.Append("IF NOT EXISTS(SELECT TOP 1 Id FROM [" + baseDeDestino + "].[dbo].[ManagerUnit] WHERE BudgetUnitId = " + assetMovement.BudgetUnitId + " AND Code = '999999')");
				builder.Append(Environment.NewLine);
				builder.Append("BEGIN");
				builder.Append(Environment.NewLine);
				builder.Append(" INSERT INTO [" + baseDeDestino + "].[dbo].[ManagerUnit] ");
				builder.Append("([BudgetUnitId] ");
				builder.Append(",[Code] ");
				builder.Append(",[Description] ");
				builder.Append(",[Status] ");
				builder.Append(",[ManagmentUnit_YearMonthStart] ");
				builder.Append(",[ManagmentUnit_YearMonthReference]");
				builder.Append(",[RowId])");

				builder.Append(" VALUES (");
				builder.Append(assetMovement.BudgetUnitId);
				builder.Append("," + "'999999' ");
				builder.Append("," + "'UGE_GENERICA'");
				builder.Append("," + "0 ");
				builder.Append("," + "'" + Convert.ToString(DateTime.Now.Year).PadLeft(4, '0') + Convert.ToString(DateTime.Now.Month).PadLeft(2, '0') + "'");
				builder.Append("," + "'" + Convert.ToString(DateTime.Now.Year).PadLeft(4, '0') + Convert.ToString(DateTime.Now.Month).PadLeft(2, '0') + "'");
				builder.Append("," + "NULL)");
				builder.Append(Environment.NewLine);
				builder.Append("END");
				return builder.ToString();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		private string createSQLUAGenerica(AssetMovements assetMovement, string baseDeDestino)
		{
			builder = new StringBuilder();

			try
			{
				builder.Append("IF NOT EXISTS(SELECT TOP 1 Id FROM [" + baseDeDestino + "].[dbo].[AdministrativeUnit] WHERE ManagerUnitId = " + assetMovement.ManagerUnitId + " AND Code = '99999999')");
				builder.Append(Environment.NewLine);
				builder.Append("BEGIN");
				builder.Append(Environment.NewLine);
				builder.Append(" INSERT INTO [" + baseDeDestino + "].[dbo].[AdministrativeUnit] ");
				builder.Append("([ManagerUnitId] ");
				builder.Append(",[Code] ");
				builder.Append(",[Description] ");
				builder.Append(",[RelationshipAdministrativeUnitId] ");
				builder.Append(",[Status] ");
				builder.Append(",[RowId])");

				builder.Append(" VALUES (");
				builder.Append(assetMovement.ManagerUnitId);
				builder.Append("," + "'99999999' ");
				builder.Append("," + "'UA_GENERICA'");
				builder.Append("," + "NULL ");
				builder.Append("," + "0 ");
				builder.Append("," + "NULL)");
				builder.Append(Environment.NewLine);
				builder.Append("END");
				return builder.ToString();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}
}