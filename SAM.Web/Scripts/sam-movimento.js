var movimento = {

    mensagemTransferenciaOuDoacao: '',
    recarregaPagina: false,
    msgContabiliza: '',

    Load: function () {
        //movimento.EventoTogglePanelFilter();
        movimento.EventoChangeTipoMovimento();
        movimento.EventoChangeUGE();
        movimento.eventoSubmitForm();
        movimento.DestacaUltimoHistorico();
        movimento.InicializaDatePicker();
        movimento.CarregaDadosPatrimoniosDisponiveisGrid(1);
        movimento.CarregaDadosPatrimoniosParaMovimentoGrid(1);
        movimento.EventoMostraMensagemObrigatorioUGE();
        movimento.EventoChangeReportDoacaoTransferenciaLimpar();
        //movimento.EventoTravaGridsMovimentacaoInventario();
    },

    LoadMovimentoItemInventario: function () {
        movimento.EventoChangeTipoMovimento();
        movimento.EventoChangeUGE();
        movimento.DestacaUltimoHistorico();
        movimento.InicializaDatePicker();
        movimento.EventoMostraMensagemObrigatorioUGE();
        movimento.EventoChangeReportDoacaoTransferenciaLimpar();
        movimento.eventoSubmitFormItemInventario();
    },

    EventoTravaGridsMovimentacaoInventario: function () {
        //var urlChamadora = $(location).attr('href');
        var origemPaginaInventario = $('#OrigemInventario').val();

        if (origemPaginaInventario != null && origemPaginaInventario == true) {
            $('#MovementTypeId').attr('disabled', 'disabled');
            $('#btnFiltrar').attr('disabled', 'disabled');
            $('#Observation').attr('disabled', 'disabled');
            $('.addBem').attr('disabled', 'disabled');
            $('.addTodos').attr('disabled', 'disabled');
            $('.removerBem').attr('disabled', 'disabled');
            $('.removerTodos').attr('disabled', 'disabled');

        }
    },

    EventoTogglePanelFilter: function () {
        $('#btnMostrarFiltro').click(function () {
            $(".panel-filter").toggle("blind", 500);

            //Limpa os campos após ocultar ou mostrar os filtros
            $('#AdministrativeUnitId:first').val($("#AdministrativeUnitId:first option:first").val());
            $('#SectionId:first').val($("#SectionId:first option:first").val());
        });
    },

    EnumMovementType: {
        IncorpEmpenho: 1,
        IncorpTransferencia: 2,
        IncorpDoacao: 3,
        IncorpDoacaoTerceiros: 4,
        IncorpInventarioInicial: 5,
        IncorpCompraDireta: 6,
        IncorpNascimentoAnimais: 7,
        IncorpMaterialTransTerceiro: 8,
        IncorpCompraRegimeAdiant: 9,
        MovimentacaoInterna: 10,
        Transferencia: 11,
        Doacao: 12,
        VoltaConserto: 13,
        SaidaConserto: 14,
        Extravio: 15,
        Obsoleto: 16,
        Danificado: 17,
        Sucata: 18,
        MovInservivelNaUGE: 19,
        DisponibilizadoParaBolsaSecretaria: 20,
        DisponibilizadoParaBolsaEstadual: 21,
        RetiradaDaBolsa: 22,
        BaixaMaterialDeConsumo: 23,
        BaixaDoacaoTransferencia: 24,
        IncorporacaoPorEmpenhoRestosAPagar: 25,
        IncorpAnimaisPesquisaSememPeixe: 26,
        IncorpComodatoDeTerceirosRecebidos: 27,
        IncorpComodatoConcedidoBensMoveis: 28,
        IncorpConfiscoBensMoveis: 29,
        IncorpDoacaoConsolidacao: 30,
        IncorpDoacaoIntraNoEstado: 31,
        IncorpDoacaoMunicipio: 32,
        IncorpDoacaoOutrosEstados: 33,
        IncorpDoacaoUniao: 34,
        IncorpVegetal: 35,
        IncorpMudancaDeCategoriaRevalorizacao: 36,
        IncorpNascimentoDeAnimais: 37,
        IncorpRecebimentoDeInservivelUGEDoacao: 38,
        IncorpRecebimentoDeInservivelUGETranferencia: 39,
        IncorpTransferenciaMesmoOrgaoPatrimoniado: 40,
        IncorpTransferenciaOutroOrgaoPatrimoniado: 41,
        MovSaidaInservivelDaUGEDoacao: 42,
        MovSaidaInservivelDaUGETransferencia: 43,
        MovComodatoConcedidoBensMoveis: 44,
        MovComodatoTerceirosRecebidos: 45,
        MovDoacaoConsolidacao: 46,
        MovDoacaoIntraNoEstado: 47,
        MovDoacaoMunicipio: 48,
        MovDoacaoOutrosEstados: 49,
        MovDoacaoUniao: 50,
        MovExtravioFurtoRouboBensMoveis: 51,
        MovMorteAnimalPatrimoniado: 52,
        MovMorteVegetalPatrimoniado: 53,
        MovMudancaCategoriaDesvalorizacao: 54,
        MovSementesPlantasInsumosArvores: 55,
        MovTransferenciaOutroOrgaoPatrimoniado: 56,
        MovTransferenciaMesmoOrgaoPatrimoniado: 57,
        MovPerdaInvoluntariaBensMoveis: 58,
        MovPerdaInvoluntariaInservivelBensMoveis: 59,
        MovVendaLeilaoSemoventes: 95
    },

    EventoChangeTipoMovimento: function () {
        $('#MovementTypeId').change(function () {

            var _movimentTypeId = $('#MovementTypeId option:selected').val();

            if (_movimentTypeId != '') {

                //Limpa os itens selecionados
                $('#ListAssetsParaMovimento').val(JSON.stringify([]));

                //Recarrega os dados dos grids
                var ugeAlterada = true;

                movimento.CarregaDadosPatrimoniosDisponiveisGrid(1, ugeAlterada);
                movimento.CarregaDadosPatrimoniosParaMovimentoGrid(1);

                //Bloqueia a tela
                $.blockUI({ message: $('#modal-loading') });

                $.get(sam.path.webroot + "Movimento/CarregaPartialViewTipoMovimento",
                { movimentTypeId: _movimentTypeId, hierarquia: sam.commun.Organiza() },
                function () {
                }).done(function (data) {

                    //Carrega a partialView do tipo de movimento
                    $('#partialViewTipoMovimento').html(data);
                    $('#partialViewTipoMovimento').css('display', 'block');

                    //Desbloqueia a tela
                    $.unblockUI({ message: $('#modal-loading') });

                    //Caso seja uma movimentação interna
                    if (_movimentTypeId == movimento.EnumMovementType.MovimentacaoInterna ||
                        _movimentTypeId == movimento.EnumMovementType.VoltaConserto ||
                        _movimentTypeId == movimento.EnumMovementType.SaidaConserto ||
                        _movimentTypeId == movimento.EnumMovementType.Extravio ||
                        _movimentTypeId == movimento.EnumMovementType.Obsoleto ||
                        _movimentTypeId == movimento.EnumMovementType.Danificado ||
                        _movimentTypeId == movimento.EnumMovementType.Sucata ||
                        _movimentTypeId == movimento.EnumMovementType.MovInservivelNaUGE ||
                        _movimentTypeId == movimento.EnumMovementType.DisponibilizadoParaBolsaSecretaria ||
                        _movimentTypeId == movimento.EnumMovementType.DisponibilizadoParaBolsaEstadual ||
                        _movimentTypeId == movimento.EnumMovementType.RetiradaDaBolsa) {

                        var instituationDestino = $('#InstituationIdDestino').val();
                        var budgetUnitIdDestino = $('#BudgetUnitIdDestino').val();
                        var managerUnitIdDestino = $('#ManagerUnitIdDestino').val();

                        //Seta os valores de órgão, UO e UGE nos combos de filtros, para recuperar apenas os bens relacionados a mesma hierarquia de movimentação (movimentação interna)
                        $('#InstituationId').val(instituationDestino);
                        $('#BudgetUnitId').val(budgetUnitIdDestino);
                        $('#ManagerUnitId').val(managerUnitIdDestino);

                        //Disabilita os combos de filtro
                        $('#InstituationId').attr('disabled', 'disabled');
                        $('#BudgetUnitId').attr('disabled', 'disabled');
                        if ($("#MovimentoDeUmItemDeInventario").length == 0) {
                            $('#ManagerUnitId').attr('disabled', 'disabled');
                        }

                        //Caso seja alterada a UGE de movimentação
                        $('.comboManagerUnit2').bind('change', function () {

                            //Replica a UGE de movimentação, para a UGE a ser filtrada e recarrega os grids
                            var managerUnitId = $(this).val();
                            $('#ManagerUnitId').val(managerUnitId).change(); //O método change força o navegador a chamar o evento de change para o combo de Id #ManagerUnitId

                            movimento.EventoMostraMensagemObrigatorioUGE();

                            //Limpa os itens selecionados
                            $('#ListAssetsParaMovimento').val(JSON.stringify([]));

                            var ugeAlterada = true;

                            movimento.CarregaDadosPatrimoniosDisponiveisGrid(1, ugeAlterada);
                            movimento.CarregaDadosPatrimoniosParaMovimentoGrid(1);
                        });
                    }
                    else {
                        //Desativa o evento de change
                        $('.comboManagerUnit2').unbind("change");

                        //Habilita novamente os combos dos filtros
                        $('#InstituationId').removeAttr('disabled');
                        $('#BudgetUnitId').removeAttr('disabled');
                        $('#ManagerUnitId').removeAttr('disabled');
                    }

                }).fail(function () {
                    $.unblockUI({ message: $('#modal-loading') });
                    alert('Erro na rotina EventoChangeTipoIncorporacao.');
                });
            }
            else {
                $('#partialViewTipoMovimento').html('');
            }
        });

        if ($('#MovementTypeId').val() != "") {
            $('#partialViewTipoMovimento').css('display', 'block');
        }
    },

    EventoChangeUGE: function () {
        $('.comboManagerUnitFiltro').change(function () {
            //Recarrega UA
            //$('.comboAdministrativeUnit').empty();
            var options = '';
            var ugeId = $('.comboManagerUnitFiltro').val();
            $.get(sam.path.webroot + "/Hierarquia/GetUas", { ugeId: ugeId }, function (data) {
                var options = '';
                $('.selectpicker.comboAdministrativeUnit').empty().append('<option value="">Selecione a UA</option>');
                $.each(data, function (key, value) {
                    options += '<option value="' + value.Id + '">' + value.Description + '</option>';
                    $('.selectpicker.comboAdministrativeUnit').append(options);
                    options = '';
                });
                $('.selectpicker.comboAdministrativeUnit').selectpicker('refresh');

                //Limpa os itens selecionados
                $('#ListAssetsParaMovimento').val(JSON.stringify([]));

                //bloqueio de UGE para Movimentação no Mesmo Órgão
                if ($('#MovementTypeId option:selected').val() == movimento.EnumMovementType.MovTransferenciaMesmoOrgaoPatrimoniado ||
                    $('#MovementTypeId option:selected').val() == movimento.EnumMovementType.MovSaidaInservivelDaUGETransferencia) {
                    if ($('.comboManagerUnit2 option:selected').val() == $('.comboManagerUnitFiltro option:selected').val()) {
                        if ($(".comboManagerUnit2 option[value='']").val() == undefined) {
                            $('.comboManagerUnit2').val('0');
                        } else {
                            $('.comboManagerUnit2').val('');
                        }
                    }

                    $(".comboManagerUnit2 option[disabled='disabled']").removeAttr('disabled');
                    $(".comboManagerUnit2 option[value='" + $('.comboManagerUnitFiltro option:selected').val() + "']").attr('disabled', 'disabled');

                }

                var ugeAlterada = true;

                movimento.CarregaDadosPatrimoniosDisponiveisGrid(1, ugeAlterada);
                movimento.CarregaDadosPatrimoniosParaMovimentoGrid(1);

                movimento.InicializaDatePicker();

                movimento.EventoMostraMensagemObrigatorioUGE();

            });
            $('.selectpicker.comboSection').empty().append('<option value="">Selecione a Divisão</option>');
            $('.selectpicker.comboSection').selectpicker('refresh');
        });
    },

    CarregaDadosPatrimoniosDisponiveisGrid: function (_page, limpaGrid, pesquisa) {
        var url = sam.path.webroot + "/Movimento/CarregaPatrimonios";

        var _institutionId = $('#InstituationId').val();
        var _budgetUnitId = $('#BudgetUnitId').val();
        var _managerUnitId = limpaGrid == true ? 0 : $('#ManagerUnitId').val();
        var _administrativeUnitId = $('#AdministrativeUnitId').val();
        var _sectionId = $('#SectionId').val();
        var _searchString = $('#SearchString').val();
        var _listAssets = $('#ListAssetsParaMovimento').val();
        var _contaContabilId = $('#ContaContabilId').val();

        $.get(url, {
            institutionId: _institutionId,
            contaContabilId: _contaContabilId,
            budgetUnitId: _budgetUnitId,
            managerUnitId: _managerUnitId,
            administrativeUnitId: _administrativeUnitId,
            sectionId: _sectionId,
            searchString: _searchString,
            listAssets: _listAssets,
            page: _page,
            movimentTypeId: $('#MovementTypeId option:selected').val(),
            pesquisa: pesquisa
        }, function () {
        }).done(function (data) {
            $('#partialPatrimoniosDisponiveis').html(data);
            movimento.EventoPaginacaoGridAssets();

        }).fail(function () {
            alert('Erro ao carregar a rotina CarregaDadosPatrimoniosDisponiveisGrid.');
        });
    },

    CarregaDadosPatrimoniosParaMovimentoGrid: function (_page) {

        var url = sam.path.webroot + "/Movimento/CarregaPatrimoniosParaMovimento";

        var _institutionId = $('#InstituationId').val();
        var _budgetUnitId = $('#BudgetUnitId').val();
        var _managerUnitId = $('#ManagerUnitId').val();
        var _administrativeUnitId = $('#AdministrativeUnitId').val();
        var _sectionId = $('#SectionId').val();

        var _listAssets = $('#ListAssetsParaMovimento').val();

        $.get(url, {
            institutionId: _institutionId,
            budgetUnitId: _budgetUnitId,
            managerUnitId: _managerUnitId,
            administrativeUnitId: _administrativeUnitId,
            sectionId: _sectionId,
            //searchString: _searchString,
            listAssets: _listAssets,
            page: _page

        }, function () {
        }).done(function (data) {
            $('#partialPatrimoniosParaMovimento').html(data);
            movimento.EventoPaginacaoGridMoviments();

        }).fail(function () {
            alert('Erro ao carregar a rotina CarregaDadosPatrimoniosParaMovimentoGrid.');
        });
    },

    validaTransferencia: function () {

        var valido = true;
        var movimentTypeId = $('#MovementTypeId option:selected').val();

        //Valida o tipo de movimentação
        if (movimentTypeId != '') {

            switch (eval(movimentTypeId)) {

                case movimento.EnumMovementType.MovimentacaoInterna:

                    $('#spanMovimentType').text('');
                    $('#spanInstitution').text('');
                    $('#spanBudgetUnits').text('');
                    $('#spanManagerUnits').text('');
                    $('#spanAdministrativeUnits').text('');
                    $('#spanSection').text('');
                    $('#spanResponsible').text('');

                    if ($('#InstituationIdDestino option:selected').val() == '' || $('#InstituationIdDestino option:selected').val() == undefined || $('#InstituationIdDestino option:selected').val() == '0') {
                        $('#spanInstitution').text('Por favor, informe o Órgão.');
                        valido = false;
                    }

                    if ($('#BudgetUnitIdDestino option:selected').val() == '' || $('#BudgetUnitIdDestino option:selected').val() == undefined || $('#BudgetUnitIdDestino option:selected').val() == '0') {
                        $('#spanBudgetUnits').text('Por favor, informe a UO.');
                        valido = false;
                    }

                    if ($('#ManagerUnitIdDestino option:selected').val() == '' || $('#ManagerUnitIdDestino option:selected').val() == undefined || $('#ManagerUnitIdDestino option:selected').val() == '0') {
                        $('#spanManagerUnits').text('Por favor, informe a UGE.');
                        valido = false;
                    }

                    if ($('#AdministrativeUnitIdDestino option:selected').val() == '' || $('#AdministrativeUnitIdDestino option:selected').val() == undefined || $('#AdministrativeUnitIdDestino option:selected').val() == '0') {
                        $('#spanAdministrativeUnits').text('Por favor, informe a UA.');
                        valido = false;
                    }

                    // Tarefa tira a obrigatoriedade da divisao - 14-09-2018
                    //if ($('#SectionIdDestino option:selected').val() == '' || $('#SectionIdDestino option:selected').val() == undefined || $('#SectionIdDestino option:selected').val() == '0') {
                    //    $('#spanSection').text('Por favor, informe a Divisão.');
                    //    valido = false;
                    //}

                    if ($('#ResponsibleId option:selected').val() == '' || $('#ResponsibleId option:selected').val() == undefined || $('#ResponsibleId option:selected').val() == '0') {
                        $('#spanResponsible').text('Por favor, informe o Responsável.');
                        valido = false;
                    }

                    break;

                case movimento.EnumMovementType.VoltaConserto:
                    $('#spanRepairValue').text('');

                    if ($('#InstituationIdDestino option:selected').val() == '' || $('#InstituationIdDestino option:selected').val() == undefined || $('#InstituationIdDestino option:selected').val() == '0') {
                        $('#spanInstitution').text('Por favor, informe o Órgão.');
                        valido = false;
                    }

                    if ($('#BudgetUnitIdDestino option:selected').val() == '' || $('#BudgetUnitIdDestino option:selected').val() == undefined || $('#BudgetUnitIdDestino option:selected').val() == '0') {
                        $('#spanBudgetUnits').text('Por favor, informe a UO.');
                        valido = false;
                    }

                    if ($('#ManagerUnitIdDestino option:selected').val() == '' || $('#ManagerUnitIdDestino option:selected').val() == undefined || $('#ManagerUnitIdDestino option:selected').val() == '0') {
                        $('#spanManagerUnits').text('Por favor, informe a UGE.');
                        valido = false;
                    }

                    if ($('#RepairValue').val() == '') {
                        $('#spanRepairValue').text('Por favor, informe o Valor do Corserto.');
                        valido = false;
                    }

                    break;

                case movimento.EnumMovementType.SaidaConserto:
                    $('#spanRepairValue').text('');

                    if ($('#InstituationIdDestino option:selected').val() == '' || $('#InstituationIdDestino option:selected').val() == undefined || $('#InstituationIdDestino option:selected').val() == '0') {
                        $('#spanInstitution').text('Por favor, informe o Órgão.');
                        valido = false;
                    }

                    if ($('#BudgetUnitIdDestino option:selected').val() == '' || $('#BudgetUnitIdDestino option:selected').val() == undefined || $('#BudgetUnitIdDestino option:selected').val() == '0') {
                        $('#spanBudgetUnits').text('Por favor, informe a UO.');
                        valido = false;
                    }

                    if ($('#ManagerUnitIdDestino option:selected').val() == '' || $('#ManagerUnitIdDestino option:selected').val() == undefined || $('#ManagerUnitIdDestino option:selected').val() == '0') {
                        $('#spanManagerUnits').text('Por favor, informe a UGE.');
                        valido = false;
                    }

                    if ($('#RepairValue').val() == '') {
                        $('#spanRepairValue').text('Por favor, informe o Valor do Corserto.');
                        valido = false;
                    }

                    break;

                case movimento.EnumMovementType.MovSaidaInservivelDaUGEDoacao:

                    if ($('#InstituationIdDestino').length > 0) {
                        valido = true;
                    } else {
                        valido = false;
                    }

                    break;

                case movimento.EnumMovementType.MovSaidaInservivelDaUGETransferencia:

                    if ($('#InstituationIdDestino').length > 0) {
                        valido = true;
                    } else {
                        valido = false;
                    }

                    break;

                case movimento.EnumMovementType.MovComodatoConcedidoBensMoveis:

                    $('#spanMovimentType').text('');
                    $('#spanInstitution').text('');
                    $('#spanBudgetUnits').text('');
                    $('#spanManagerUnits').text('');

                    if ($('#InstituationIdDestino option:selected').val() == '' || $('#InstituationIdDestino option:selected').val() == undefined || $('#InstituationIdDestino option:selected').val() == '0') {
                        $('#spanInstitution').text('Por favor, informe o Órgão.');
                        valido = false;
                    }

                    if ($('#BudgetUnitIdDestino option:selected').val() == '' || $('#BudgetUnitIdDestino option:selected').val() == undefined || $('#BudgetUnitIdDestino option:selected').val() == '0') {
                        $('#spanBudgetUnits').text('Por favor, informe a UO.');
                        valido = false;
                    }

                    if ($('#ManagerUnitIdDestino option:selected').val() == '' || $('#ManagerUnitIdDestino option:selected').val() == undefined || $('#ManagerUnitIdDestino option:selected').val() == '0') {
                        $('#spanManagerUnits').text('Por favor, informe a UGE.');
                        valido = false;
                    }

                    break;

                case movimento.EnumMovementType.MovComodatoTerceirosRecebidos:

                    if ($('#InstituationIdDestino option:selected').val() == '' || $('#InstituationIdDestino option:selected').val() == undefined || $('#InstituationIdDestino option:selected').val() == '0') {
                        $('#spanInstitution').text('Por favor, informe o Órgão.');
                        valido = false;
                    }

                    if ($('#BudgetUnitIdDestino option:selected').val() == '' || $('#BudgetUnitIdDestino option:selected').val() == undefined || $('#BudgetUnitIdDestino option:selected').val() == '0') {
                        $('#spanBudgetUnits').text('Por favor, informe a UO.');
                        valido = false;
                    }

                    if ($('#ManagerUnitIdDestino option:selected').val() == '' || $('#ManagerUnitIdDestino option:selected').val() == undefined || $('#ManagerUnitIdDestino option:selected').val() == '0') {
                        $('#spanManagerUnits').text('Por favor, informe a UGE.');
                        valido = false;
                    }

                    break;

                case movimento.EnumMovementType.MovDoacaoConsolidacao:

                    $('#spanMovimentType').text('');
                    $('#spanInstitution').text('');
                    $('#spanBudgetUnits').text('');
                    $('#spanManagerUnits').text('');
                    $('#spanTypeDocumentOut').text('');
                    $('#spanNumberProcess').text('');
                    $('#spanCPFCNPJ').text('');

                    if ($('#CPFCNPJ').val() == '' || $('#CPFCNPJ').val() == undefined || $('#CPFCNPJ').val() == '0') {
                        $('#spanCPFCNPJ').text('Por favor, informe o CPF/CNPJ.');
                        valido = false;
                    }

                    //if ($('#InstituationIdDestino option:selected').val() == '' || $('#InstituationIdDestino option:selected').val() == undefined || $('#InstituationIdDestino option:selected').val() == '0') {
                    //    $('#spanInstitution').text('Por favor, informe o Órgão.');
                    //    valido = false;
                    //}

                    //if ($('#BudgetUnitIdDestino option:selected').val() == '' || $('#BudgetUnitIdDestino option:selected').val() == undefined || $('#BudgetUnitIdDestino option:selected').val() == '0') {
                    //    $('#spanBudgetUnits').text('Por favor, informe a UO.');
                    //    valido = false;
                    //}

                    //if ($('#ManagerUnitIdDestino option:selected').val() == '' || $('#ManagerUnitIdDestino option:selected').val() == undefined || $('#ManagerUnitIdDestino option:selected').val() == '0') {
                    //    $('#spanManagerUnits').text('Por favor, informe a UGE.');
                    //    valido = false;
                    //}

                    break;

                case movimento.EnumMovementType.MovDoacaoIntraNoEstado:

                    $('#spanMovimentType').text('');
                    $('#spanInstitution').text('');
                    $('#spanBudgetUnits').text('');
                    $('#spanManagerUnits').text('');
                    $('#spanTypeDocumentOut').text('');
                    $('#spanNumberProcess').text('');

                    if ($('#InstituationIdDestino option:selected').val() == '' || $('#InstituationIdDestino option:selected').val() == undefined || $('#InstituationIdDestino option:selected').val() == '0') {
                        $('#spanInstitution').text('Por favor, informe o Órgão.');
                        valido = false;
                    }

                    if ($('#BudgetUnitIdDestino option:selected').val() == '' || $('#BudgetUnitIdDestino option:selected').val() == undefined || $('#BudgetUnitIdDestino option:selected').val() == '0') {
                        $('#spanBudgetUnits').text('Por favor, informe a UO.');
                        valido = false;
                    }

                    if ($('#ManagerUnitIdDestino option:selected').val() == '' || $('#ManagerUnitIdDestino option:selected').val() == undefined || $('#ManagerUnitIdDestino option:selected').val() == '0') {
                        $('#spanManagerUnits').text('Por favor, informe a UGE.');
                        valido = false;
                    }

                    break;

                case movimento.EnumMovementType.MovDoacaoMunicipio:

                    $('#spanMovimentType').text('');
                    $('#spanInstitution').text('');
                    $('#spanBudgetUnits').text('');
                    $('#spanManagerUnits').text('');
                    $('#spanTypeDocumentOut').text('');
                    $('#spanNumberProcess').text('');
                    $('#spanCPFCNPJ').text('');

                    if ($('#CPFCNPJ').val() == '' || $('#CPFCNPJ').val() == undefined || $('#CPFCNPJ').val() == '0') {
                        $('#spanCPFCNPJ').text('Por favor, informe o CPF/CNPJ.');
                        valido = false;
                    }

                    //if ($('#InstituationIdDestino option:selected').val() == '' || $('#InstituationIdDestino option:selected').val() == undefined || $('#InstituationIdDestino option:selected').val() == '0') {
                    //    $('#spanInstitution').text('Por favor, informe o Órgão.');
                    //    valido = false;
                    //}

                    //if ($('#BudgetUnitIdDestino option:selected').val() == '' || $('#BudgetUnitIdDestino option:selected').val() == undefined || $('#BudgetUnitIdDestino option:selected').val() == '0') {
                    //    $('#spanBudgetUnits').text('Por favor, informe a UO.');
                    //    valido = false;
                    //}

                    //if ($('#ManagerUnitIdDestino option:selected').val() == '' || $('#ManagerUnitIdDestino option:selected').val() == undefined || $('#ManagerUnitIdDestino option:selected').val() == '0') {
                    //    $('#spanManagerUnits').text('Por favor, informe a UGE.');
                    //    valido = false;
                    //}

                    break;

                case movimento.EnumMovementType.MovDoacaoOutrosEstados:

                    $('#spanMovimentType').text('');
                    $('#spanInstitution').text('');
                    $('#spanBudgetUnits').text('');
                    $('#spanManagerUnits').text('');
                    $('#spanTypeDocumentOut').text('');
                    $('#spanNumberProcess').text('');
                    $('#spanCPFCNPJ').text('');

                    if ($('#CPFCNPJ').val() == '' || $('#CPFCNPJ').val() == undefined || $('#CPFCNPJ').val() == '0') {
                        $('#spanCPFCNPJ').text('Por favor, informe o CPF/CNPJ.');
                        valido = false;
                    }

                    //if ($('#InstituationIdDestino option:selected').val() == '' || $('#InstituationIdDestino option:selected').val() == undefined || $('#InstituationIdDestino option:selected').val() == '0') {
                    //    $('#spanInstitution').text('Por favor, informe o Órgão.');
                    //    valido = false;
                    //}

                    //if ($('#BudgetUnitIdDestino option:selected').val() == '' || $('#BudgetUnitIdDestino option:selected').val() == undefined || $('#BudgetUnitIdDestino option:selected').val() == '0') {
                    //    $('#spanBudgetUnits').text('Por favor, informe a UO.');
                    //    valido = false;
                    //}

                    //if ($('#ManagerUnitIdDestino option:selected').val() == '' || $('#ManagerUnitIdDestino option:selected').val() == undefined || $('#ManagerUnitIdDestino option:selected').val() == '0') {
                    //    $('#spanManagerUnits').text('Por favor, informe a UGE.');
                    //    valido = false;
                    //}

                    break;

                case movimento.EnumMovementType.MovDoacaoUniao:

                    $('#spanMovimentType').text('');
                    $('#spanInstitution').text('');
                    $('#spanBudgetUnits').text('');
                    $('#spanManagerUnits').text('');
                    $('#spanTypeDocumentOut').text('');
                    $('#spanNumberProcess').text('');
                    $('#spanCPFCNPJ').text('');

                    if ($('#CPFCNPJ').val() == '' || $('#CPFCNPJ').val() == undefined || $('#CPFCNPJ').val() == '0') {
                        $('#spanCPFCNPJ').text('Por favor, informe o CPF/CNPJ.');
                        valido = false;
                    }

                    //if ($('#InstituationIdDestino option:selected').val() == '' || $('#InstituationIdDestino option:selected').val() == undefined || $('#InstituationIdDestino option:selected').val() == '0') {
                    //    $('#spanInstitution').text('Por favor, informe o Órgão.');
                    //    valido = false;
                    //}

                    //if ($('#BudgetUnitIdDestino option:selected').val() == '' || $('#BudgetUnitIdDestino option:selected').val() == undefined || $('#BudgetUnitIdDestino option:selected').val() == '0') {
                    //    $('#spanBudgetUnits').text('Por favor, informe a UO.');
                    //    valido = false;
                    //}

                    //if ($('#ManagerUnitIdDestino option:selected').val() == '' || $('#ManagerUnitIdDestino option:selected').val() == undefined || $('#ManagerUnitIdDestino option:selected').val() == '0') {
                    //    $('#spanManagerUnits').text('Por favor, informe a UGE.');
                    //    valido = false;
                    //}

                    break;

                case movimento.EnumMovementType.MovExtravioFurtoRouboBensMoveis:

                    if ($('#InstituationIdDestino option:selected').val() == '' || $('#InstituationIdDestino option:selected').val() == undefined || $('#InstituationIdDestino option:selected').val() == '0') {
                        $('#spanInstitution').text('Por favor, informe o Órgão.');
                        valido = false;
                    }

                    if ($('#BudgetUnitIdDestino option:selected').val() == '' || $('#BudgetUnitIdDestino option:selected').val() == undefined || $('#BudgetUnitIdDestino option:selected').val() == '0') {
                        $('#spanBudgetUnits').text('Por favor, informe a UO.');
                        valido = false;
                    }

                    if ($('#ManagerUnitIdDestino option:selected').val() == '' || $('#ManagerUnitIdDestino option:selected').val() == undefined || $('#ManagerUnitIdDestino option:selected').val() == '0') {
                        $('#spanManagerUnits').text('Por favor, informe a UGE.');
                        valido = false;
                    }

                    break;

                case movimento.EnumMovementType.MovMorteAnimalPatrimoniado:

                    if ($('#InstituationIdDestino option:selected').val() == '' || $('#InstituationIdDestino option:selected').val() == undefined || $('#InstituationIdDestino option:selected').val() == '0') {
                        $('#spanInstitution').text('Por favor, informe o Órgão.');
                        valido = false;
                    }

                    if ($('#BudgetUnitIdDestino option:selected').val() == '' || $('#BudgetUnitIdDestino option:selected').val() == undefined || $('#BudgetUnitIdDestino option:selected').val() == '0') {
                        $('#spanBudgetUnits').text('Por favor, informe a UO.');
                        valido = false;
                    }

                    if ($('#ManagerUnitIdDestino option:selected').val() == '' || $('#ManagerUnitIdDestino option:selected').val() == undefined || $('#ManagerUnitIdDestino option:selected').val() == '0') {
                        $('#spanManagerUnits').text('Por favor, informe a UGE.');
                        valido = false;
                    }

                    break;

                case movimento.EnumMovementType.MovMudancaCategoriaDesvalorizacao:

                    if ($('#InstituationIdDestino option:selected').val() == '' || $('#InstituationIdDestino option:selected').val() == undefined || $('#InstituationIdDestino option:selected').val() == '0') {
                        $('#spanInstitution').text('Por favor, informe o Órgão.');
                        valido = false;
                    }

                    if ($('#BudgetUnitIdDestino option:selected').val() == '' || $('#BudgetUnitIdDestino option:selected').val() == undefined || $('#BudgetUnitIdDestino option:selected').val() == '0') {
                        $('#spanBudgetUnits').text('Por favor, informe a UO.');
                        valido = false;
                    }

                    if ($('#ManagerUnitIdDestino option:selected').val() == '' || $('#ManagerUnitIdDestino option:selected').val() == undefined || $('#ManagerUnitIdDestino option:selected').val() == '0') {
                        $('#spanManagerUnits').text('Por favor, informe a UGE.');
                        valido = false;
                    }

                    break;

                case movimento.EnumMovementType.MovSementesPlantasInsumosArvores:

                    if ($('#InstituationIdDestino option:selected').val() == '' || $('#InstituationIdDestino option:selected').val() == undefined || $('#InstituationIdDestino option:selected').val() == '0') {
                        $('#spanInstitution').text('Por favor, informe o Órgão.');
                        valido = false;
                    }

                    if ($('#BudgetUnitIdDestino option:selected').val() == '' || $('#BudgetUnitIdDestino option:selected').val() == undefined || $('#BudgetUnitIdDestino option:selected').val() == '0') {
                        $('#spanBudgetUnits').text('Por favor, informe a UO.');
                        valido = false;
                    }

                    if ($('#ManagerUnitIdDestino option:selected').val() == '' || $('#ManagerUnitIdDestino option:selected').val() == undefined || $('#ManagerUnitIdDestino option:selected').val() == '0') {
                        $('#spanManagerUnits').text('Por favor, informe a UGE.');
                        valido = false;
                    }

                    break;

                case movimento.EnumMovementType.MovTransferenciaOutroOrgaoPatrimoniado:

                    $('#spanMovimentType').text('');
                    $('#spanInstitution').text('');
                    $('#spanBudgetUnits').text('');
                    $('#spanManagerUnits').text('');

                    if ($('#InstituationIdDestino option:selected').val() == '' || $('#InstituationIdDestino option:selected').val() == undefined || $('#InstituationIdDestino option:selected').val() == '0') {
                        $('#spanInstitution').text('Por favor, informe o Órgão.');
                        valido = false;
                    }

                    if ($('#BudgetUnitIdDestino option:selected').val() == '' || $('#BudgetUnitIdDestino option:selected').val() == undefined || $('#BudgetUnitIdDestino option:selected').val() == '0') {
                        $('#spanBudgetUnits').text('Por favor, informe a UO.');
                        valido = false;
                    }

                    if ($('#ManagerUnitIdDestino option:selected').val() == '' || $('#ManagerUnitIdDestino option:selected').val() == undefined || $('#ManagerUnitIdDestino option:selected').val() == '0') {
                        $('#spanManagerUnits').text('Por favor, informe a UGE.');
                        valido = false;
                    }

                    break;

                case movimento.EnumMovementType.MovTransferenciaMesmoOrgaoPatrimoniado:

                    $('#spanMovimentType').text('');
                    $('#spanInstitution').text('');
                    $('#spanBudgetUnits').text('');
                    $('#spanManagerUnits').text('');

                    if ($('#InstituationIdDestino option:selected').val() == '' || $('#InstituationIdDestino option:selected').val() == undefined || $('#InstituationIdDestino option:selected').val() == '0') {
                        $('#spanInstitution').text('Por favor, informe o Órgão.');
                        valido = false;
                    }

                    if ($('#BudgetUnitIdDestino option:selected').val() == '' || $('#BudgetUnitIdDestino option:selected').val() == undefined || $('#BudgetUnitIdDestino option:selected').val() == '0') {
                        $('#spanBudgetUnits').text('Por favor, informe a UO.');
                        valido = false;
                    }

                    if ($('#ManagerUnitIdDestino option:selected').val() == '' || $('#ManagerUnitIdDestino option:selected').val() == undefined || $('#ManagerUnitIdDestino option:selected').val() == '0') {
                        $('#spanManagerUnits').text('Por favor, informe a UGE.');
                        valido = false;
                    }

                    break;

                case movimento.EnumMovementType.MovPerdaInvoluntariaBensMoveis:

                    if ($('#InstituationIdDestino option:selected').val() == '' || $('#InstituationIdDestino option:selected').val() == undefined || $('#InstituationIdDestino option:selected').val() == '0') {
                        $('#spanInstitution').text('Por favor, informe o Órgão.');
                        valido = false;
                    }

                    if ($('#BudgetUnitIdDestino option:selected').val() == '' || $('#BudgetUnitIdDestino option:selected').val() == undefined || $('#BudgetUnitIdDestino option:selected').val() == '0') {
                        $('#spanBudgetUnits').text('Por favor, informe a UO.');
                        valido = false;
                    }

                    if ($('#ManagerUnitIdDestino option:selected').val() == '' || $('#ManagerUnitIdDestino option:selected').val() == undefined || $('#ManagerUnitIdDestino option:selected').val() == '0') {
                        $('#spanManagerUnits').text('Por favor, informe a UGE.');
                        valido = false;
                    }

                    break;

                case movimento.EnumMovementType.MovPerdaInvoluntariaInservivelBensMoveis:

                    if ($('#InstituationIdDestino option:selected').val() == '' || $('#InstituationIdDestino option:selected').val() == undefined || $('#InstituationIdDestino option:selected').val() == '0') {
                        $('#spanInstitution').text('Por favor, informe o Órgão.');
                        valido = false;
                    }

                    if ($('#BudgetUnitIdDestino option:selected').val() == '' || $('#BudgetUnitIdDestino option:selected').val() == undefined || $('#BudgetUnitIdDestino option:selected').val() == '0') {
                        $('#spanBudgetUnits').text('Por favor, informe a UO.');
                        valido = false;
                    }

                    if ($('#ManagerUnitIdDestino option:selected').val() == '' || $('#ManagerUnitIdDestino option:selected').val() == undefined || $('#ManagerUnitIdDestino option:selected').val() == '0') {
                        $('#spanManagerUnits').text('Por favor, informe a UGE.');
                        valido = false;
                    }

                    break;

                case movimento.EnumMovementType.MovVendaLeilaoSemoventes:

                    if ($('#InstituationIdDestino option:selected').val() == '' || $('#InstituationIdDestino option:selected').val() == undefined || $('#InstituationIdDestino option:selected').val() == '0') {
                        $('#spanInstitution').text('Por favor, informe o Órgão.');
                        valido = false;
                    }

                    if ($('#BudgetUnitIdDestino option:selected').val() == '' || $('#BudgetUnitIdDestino option:selected').val() == undefined || $('#BudgetUnitIdDestino option:selected').val() == '0') {
                        $('#spanBudgetUnits').text('Por favor, informe a UO.');
                        valido = false;
                    }

                    if ($('#ManagerUnitIdDestino option:selected').val() == '' || $('#ManagerUnitIdDestino option:selected').val() == undefined || $('#ManagerUnitIdDestino option:selected').val() == '0') {
                        $('#spanManagerUnits').text('Por favor, informe a UGE.');
                        valido = false;
                    }

                    break;
            }
        }

        return valido;
    },

    AbrirModalHistorico: function (_assetId) {
        movimento.CarregaGridHistorico(_assetId);
    },

    FecharModalHistorico: function () {
        //Caso não exista mais linhas no grid de histórico recarregar a página, pois o Grid terá algum registro desativo
        if (movimento.recarregaPagina == true) {
            location.href = sam.path.webroot + "/Movimento";
            location.reload();
        }

        $('#modal').modal('hide');
    },

    CarregaGridHistorico: function (_assetId) {
        $.blockUI({ message: $('#modal-loading') });

        $('#historicoDoItem').empty();

        var _somenteAtivos = eval($('#cbStatus').val()) == 1 ? true : false;

        $.get(sam.path.webroot + "Movimento/CarregaPartialViewHistorico", { assetId: _assetId, somenteAtivos: _somenteAtivos }, function () {
        }).done(function (data) {

            $('#historicoDoItem').html(data);
            movimento.DestacaUltimoHistorico();
            $('#modal').modal({ keyboard: false, backdrop: 'static', show: true });

            if (movimento.msgContabiliza != "" && movimento.msgContabiliza != null) {
                alert(movimento.msgContabiliza);
            }
            movimento.msgContabiliza = '';

            $.unblockUI({ message: $('#modal-loading') });

        }).fail(function () {
            $.unblockUI({ message: $('#modal-loading') });
            alert('Erro na rotina CarregaGridHistorico.');
        });
    },

    DestacaUltimoHistorico: function () {
        $('#result-tables-patrimonio').find('tr:last').css('background-color', 'antiquewhite');
    },

    EventoSelecionaTodosRegistros: function () {

        $('input.checkTodosPatrimonio').on('ifChanged', function (event) {

            var checkRegisters = $('#result-tables-patrimonio').find('.checkPatrimonio');

            if ($(this).is(":checked")) {

                for (var i = 0; i < checkRegisters.length; i++) {
                    $(checkRegisters[i]).iCheck('check');
                }
            }
            else {
                for (var i = 0; i < checkRegisters.length; i++) {
                    $(checkRegisters[i]).iCheck('uncheck');
                }
            }
        });
    },

    EventoTrocaStatusRetorno: function () {
        $('#cbStatus').change(function () {
            $('#spanPesquisa').click()
        });
    },

    VinculaAssetsSelecionados: function () {

        var assetsSelecionadosParaMovimento = $('input[name="ListAssetsParaMovimento"]').val();
        $('input[name="ListAssets"]').val(assetsSelecionadosParaMovimento);
    },

    InicializaDatePicker: function () {

        var _managerUnitId = $('#ManagerUnitId').val();

        $.get(sam.path.webroot + "Movimento/RecuperaMesAnoReferenciaPorUGE", { managerUnitId: _managerUnitId }, function () {
        }).done(function (data) {

            var mes = data.mes;
            var ano = data.ano;
            var dataInicial = '01' + '-' + mes + '-' + ano;

            var dataAtual = new Date();

            var ultimoDiaDoMes = new Date(ano, mes, 0).getDate();
            var dataFinalDoMes = new Date(parseInt(ano), (parseInt(mes) - 1), ultimoDiaDoMes);
            var dataFinal = '';

            if (dataFinalDoMes > dataAtual) {
                dataFinal = dataAtual.getDate + '-' + (dataAtual.getMonth + 1) + '-' + dataAtual.getFullYear;
            }
            else {
                dataFinal = dataFinalDoMes.getDate() + '-' + (dataFinalDoMes.getMonth() + 1) + '-' + dataFinalDoMes.getFullYear();
            }

            $('.datepicker').datepicker('remove');
            $('.datepicker').datepicker({ language: 'pt-BR', todayHighlight: true, autoclose: true, orientation: "bottom", startDate: dataInicial, endDate: dataFinal });

        }).fail(function () {
            $.unblockUI({ message: $('#modal-loading') });
            alert('Erro na rotina InicializaDatePicker.');
        });
    },
    EstornoDaMovimentacao: function (_assetId, _assetMovementId, _movimentTypeId, _groupMovimentId, _chapa) {
        $(".estorno").click(function (e) {
            e.target.disabled = true;
            if (movimento.EstornoIncoporacoesConfirma(_groupMovimentId, _chapa)) {
                $.get(sam.path.webroot + "/Estorno/ValidaEstornoMovimentoEVerificaMovimentoIntegrado",
                    {
                        assetId: _assetId,
                        assetMovementId: _assetMovementId,
                        movimentTypeId: _movimentTypeId,
                        groupMovimentId: _groupMovimentId
                    }, function () { })
                 .done(function (data) {
                     if (data.trava == undefined) {
                         if (data.integrado == true) {
                             $("#modalLoginSiafem").modal({ keyboard: false, backdrop: 'static', show: true });
                             $('#SaveLoginSiafem').click(function (e) {
                                 e.target.disabled = true;
                                 if ($('#CPFSIAFEMModal').val().length != 11) {
                                     alert('Digite os 11 números do CPF');
                                     e.target.disabled = false;
                                     return false;
                                 }

                                 $('#LoginSiafem').val($('#CPFSIAFEMModal').val());
                                 $('#SenhaSiafem').val($('#SenhaSIAFEMModal').val());

                                 if ($('#LoginSiafem').val() != "" && $('#SenhaSiafem').val() != "") {
                                     if (data.contemNL == true) {
                                         $('#modalLoginSiafem').modal('hide');
                                         movimento.BuscaExtratosNLEstorno(_assetId, _assetMovementId, _movimentTypeId, _groupMovimentId, $('#LoginSiafem').val(), $('#SenhaSiafem').val());
                                         return false;
                                     } else if (data.contemPendencia == true) {
                                         movimento.CorrigePendenciaSIAFEMEstorno(_assetId, _assetMovementId, _movimentTypeId, _groupMovimentId, $('#LoginSiafem').val(), $('#SenhaSiafem').val());
                                     } else {
                                         movimento.ChamadaEstornoDaMovimentacao(_assetId, _assetMovementId, _movimentTypeId, _groupMovimentId);
                                     }
                                 } else {
                                     movimento.ChamadaEstornoDaMovimentacao(_assetId, _assetMovementId, _movimentTypeId, _groupMovimentId);
                                 }
                             });
                         } else {
                             movimento.ChamadaEstornoDaMovimentacao(_assetId, _assetMovementId, _movimentTypeId, _groupMovimentId);
                         };
                     } else {
                         alert(data.trava);
                     }

                 });
            }
        });
        $("#rapido").remove();
        
    },
    ChamadaEstornoDaMovimentacao: function (_assetId, _assetMovementId, _movimentTypeId, _groupMovimentId, _integrado) {
        var url = sam.path.webroot + "/Estorno/EstornoDaMovimentacao";
        $.blockUI({ message: $('#modal-loading') });
        $('#modal').modal('hide');
        $.get(url, {
            assetId: _assetId,
            assetMovementId: _assetMovementId,
            movimentTypeId: _movimentTypeId,
            groupMovimentId: _groupMovimentId
        }, function () {
        }).done(function (data) {

            if (data.trava == undefined) {
                if (data.retornoContabiliza != undefined) {
                    if (data.retornoContabiliza != "" && data.retornoContabiliza != null) {
                        movimento.msgContabiliza = data.retornoContabiliza;
                    } else {
                        movimento.msgContabiliza = '';
                    }
                } else {
                    movimento.msgContabiliza = '';
                }
                
                if (_integrado == true) {
                    movimento.recarregaPagina = true;
                    movimento.FecharModalHistorico();
                } else {
                    movimento.recarregaPagina = data.recarregaPagina;

                    var itensAtivos = eval($('#cbStatus').val()) == 1 ? true : false;

                    if (itensAtivos) {
                        if (_groupMovimentId == 1) {
                            movimento.FecharModalHistorico();
                        } else {
                            movimento.CarregaGridHistorico(_assetId);
                        }
                    }
                    else
                        movimento.FecharModalHistorico();
                }
            }
            else {
                alert(data.trava);
            }

            $.unblockUI({ message: $('#modal-loading') });

        }).fail(function () {
            $.unblockUI({ message: $('#modal-loading') });
            alert('Erro ao executar a rotina EstornoDaMovimentacao.');
        });
    },
    EstornoIncoporacoesConfirma: function (_groupMovimentId, _chapa) {
        if (_groupMovimentId == 1) {
            return confirm("Deseja REALMENTE estornar a incorporação deste Bem Patrimonial (" + $("#SiglaDoBPHistorico").val() + "-" + _chapa + ")?");
        } else {
            return true;
        }
    },

    MovimentoIntegradoAoSIAFEM: function (_movimentTypeId, _groupMovimentId) {
        if (_groupMovimentId == 1) {
            return _movimentTypeId == movimento.EnumMovementType.IncorpAnimaisPesquisaSememPeixe ||
                   _movimentTypeId == movimento.EnumMovementType.IncorpComodatoConcedidoBensMoveis ||
                   _movimentTypeId == movimento.EnumMovementType.IncorpComodatoDeTerceirosRecebidos ||
                   _movimentTypeId == movimento.EnumMovementType.IncorpConfiscoBensMoveis ||
                   _movimentTypeId == movimento.EnumMovementType.IncorpDoacaoConsolidacao ||
                   _movimentTypeId == movimento.EnumMovementType.IncorpDoacaoIntraNoEstado ||
                   _movimentTypeId == movimento.EnumMovementType.IncorpDoacaoMunicipio ||
                   _movimentTypeId == movimento.EnumMovementType.IncorpDoacaoOutrosEstados ||
                   _movimentTypeId == movimento.EnumMovementType.IncorpDoacaoUniao ||
                   _movimentTypeId == movimento.EnumMovementType.IncorpVegetal ||
                   _movimentTypeId == movimento.EnumMovementType.IncorpMudancaDeCategoriaRevalorizacao ||
                   _movimentTypeId == movimento.EnumMovementType.IncorpNascimentoDeAnimais ||
                   _movimentTypeId == movimento.EnumMovementType.IncorpRecebimentoDeInservivelUGEDoacao ||
                   _movimentTypeId == movimento.EnumMovementType.IncorpRecebimentoDeInservivelUGETranferencia ||
                   _movimentTypeId == movimento.EnumMovementType.IncorpTransferenciaMesmoOrgaoPatrimoniado ||
                   _movimentTypeId == movimento.EnumMovementType.IncorpTransferenciaOutroOrgaoPatrimoniado;
        } else {
            return _movimentTypeId == movimento.EnumMovementType.MovSaidaInservivelDaUGEDoacao ||
                   _movimentTypeId == movimento.EnumMovementType.MovSaidaInservivelDaUGETransferencia ||
                   _movimentTypeId == movimento.EnumMovementType.MovComodatoConcedidoBensMoveis ||
                   _movimentTypeId == movimento.EnumMovementType.MovComodatoTerceirosRecebidos ||
                   _movimentTypeId == movimento.EnumMovementType.MovDoacaoConsolidacao ||
                   _movimentTypeId == movimento.EnumMovementType.MovDoacaoIntraNoEstado ||
                   _movimentTypeId == movimento.EnumMovementType.MovDoacaoMunicipio ||
                   _movimentTypeId == movimento.EnumMovementType.MovDoacaoOutrosEstados ||
                   _movimentTypeId == movimento.EnumMovementType.MovDoacaoUniao ||
                   _movimentTypeId == movimento.EnumMovementType.MovExtravioFurtoRouboBensMoveis ||
                   _movimentTypeId == movimento.EnumMovementType.MovMorteAnimalPatrimoniado ||
                   _movimentTypeId == movimento.EnumMovementType.MovMudancaCategoriaDesvalorizacao ||
                   _movimentTypeId == movimento.EnumMovementType.MovSementesPlantasInsumosArvores ||
                   _movimentTypeId == movimento.EnumMovementType.MovTransferenciaOutroOrgaoPatrimoniado ||
                   _movimentTypeId == movimento.EnumMovementType.MovTransferenciaMesmoOrgaoPatrimoniado ||
                   _movimentTypeId == movimento.EnumMovementType.MovPerdaInvoluntariaBensMoveis ||
                   _movimentTypeId == movimento.EnumMovementType.MovPerdaInvoluntariaInservivelBensMoveis ||
                   _movimentTypeId == movimento.EnumMovementType.MovInservivelNaUGE ||
                   _movimentTypeId == movimento.EnumMovementType.MovVendaLeilaoSemoventes;
        }
    },
    MensagemValidaEstornoCompleto: function (ehMesAnoReferenciaAtualDaUGEEmissora, dataIncorporacaoMesAno, dataMesRefInicialMesAno) {
        ehMesAnoReferenciaAtualDaUGEEmissora = JSON.parse(ehMesAnoReferenciaAtualDaUGEEmissora.toLowerCase());

        if (ehMesAnoReferenciaAtualDaUGEEmissora == false) {
            var mensagem = "Para estornar este BP e todas as suas Movimentações, o mês de Referencia da UGE, precisa estar no mesmo mês da Data de Movimentação ( " + dataIncorporacaoMesAno + " ) ou no 'Mês de Referencia Inicial' da UGE ( " + dataMesRefInicialMesAno + " ) \n";
        }
        alert(mensagem);
    },

    EventoPaginacaoGridAssets: function () {
        $('#partialPatrimoniosDisponiveis .pagination li a').click(function () {

            var url = sam.path.webroot + "/Movimento/CarregaPatrimonios";
            var href = $(this).attr('href');
            var page = parseInt(sam.utils.getURLParameter(href, 'page'));

            var _institutionId = $('#InstituationId').val();
            var _contaContabilId = $('#ContaContabilId').val();
            var _budgetUnitId = $('#BudgetUnitId').val();
            var _managerUnitId = $('#ManagerUnitId').val();
            var _administrativeUnitId = $('#AdministrativeUnitId').val();
            var _sectionId = $('#SectionId').val();
            var _searchString = $('#SearchString').val();
            var _listAssets = $('#ListAssetsParaMovimento').val();

            $.get(url, {
                institutionId: _institutionId,
                contaContabilId:_contaContabilId,
                budgetUnitId: _budgetUnitId,
                managerUnitId: _managerUnitId,
                administrativeUnitId: _administrativeUnitId,
                sectionId: _sectionId,
                searchString: _searchString,
                listAssets: _listAssets,
                page: page,
                movimentTypeId: $('#MovementTypeId option:selected').val(),
                pesquisa: true

            }, function () {
            }).done(function (data) {
                $('#partialPatrimoniosDisponiveis').html(data);
                movimento.EventoPaginacaoGridAssets();

            }).fail(function () {
                alert('Erro ao carregar a rotina CarregaDadosPatrimoniosDisponiveisGrid.');
            });

            return false;
        });
    },

    EventoPaginacaoGridMoviments: function () {
        $('#partialPatrimoniosParaMovimento .pagination li a').click(function () {

            var url = sam.path.webroot + "/Movimento/CarregaPatrimoniosParaMovimento";
            var href = $(this).attr('href');
            var page = parseInt(sam.utils.getURLParameter(href, 'page'));

            var _institutionId = $('#InstituationId').val();
            var _budgetUnitId = $('#BudgetUnitId').val();
            var _managerUnitId = $('#ManagerUnitId').val();
            var _administrativeUnitId = $('#AdministrativeUnitId').val();
            var _sectionId = $('#SectionId').val();

            var _listAssets = $('#ListAssetsParaMovimento').val();

            $.get(url, {
                institutionId: _institutionId,
                budgetUnitId: _budgetUnitId,
                managerUnitId: _managerUnitId,
                administrativeUnitId: _administrativeUnitId,
                sectionId: _sectionId,
                //searchString: _searchString,
                listAssets: _listAssets,
                page: page

            }, function () {
            }).done(function (data) {
                $('#partialPatrimoniosParaMovimento').html(data);
                movimento.EventoPaginacaoGridMoviments();

            }).fail(function () {
                alert('Erro ao carregar a rotina CarregaDadosPatrimoniosParaMovimentoGrid.');
            });

            return false;
        });
    },

    EventoAddBem: function () {

        var arrayAssets = null;
        var assetId = null;
        var listAssets = null;

        $('.addBem').click(function () {
            if ($(this).hasClass("addBem")) {
                var movementTypeId = $('#MovementTypeId :selected').val();

                if (movementTypeId == movimento.EnumMovementType.VoltaConserto) {
                    if ($(this).closest('tr').hasClass('emConserto') == false) {
                        sam.commun.CriarAlertDinamico('Sinto muito, este item não se encontra em conserto.');
                        return false;
                    }
                }
                else {
                    if ($(this).closest('tr').hasClass('emConserto') == true) {
                        sam.commun.CriarAlertDinamico('Sinto muito, este item se encontra em conserto.');
                        return false;
                    }
                }

                //Recupera array de bens
                arrayAssets = $('#ListAssetsParaMovimento').val();
                listAssets = JSON.parse(arrayAssets);

                //Adiciona ao array o novo bem clicado
                assetId = parseInt($(this).closest('td')[0].id);
                listAssets.push(assetId);
                $('#ListAssetsParaMovimento').val(JSON.stringify(listAssets));

                //Remove a classe que possibilita a inclusão do bem na lista
                $(this).closest('a').removeClass('addBem');

                //Adiciona a classe que da o estilo de item desabilitado
                $(this).closest('a').addClass('disabilitado');

                //Carrega o Grid com os Bens para Movimento
                movimento.CarregaDadosPatrimoniosParaMovimentoGrid(1);
            }
        });
    },

    EventoAddTodos: function () {

        $('.addTodos').click(function () {
            var url = sam.path.webroot + "/Movimento/IncluirTodosBps";
            var href = $(this).attr('href');
            var page = parseInt(sam.utils.getURLParameter(href, 'page'));

            var _movimentTypeId = $('#MovementTypeId :selected').val();

            var _institutionId = $('#InstituationId').val();
            var _budgetUnitId = $('#BudgetUnitId').val();
            var _managerUnitId = $('#ManagerUnitId').val();
            var _administrativeUnitId = $('#AdministrativeUnitId').val();
            var _sectionId = $('#SectionId').val();
            var _searchString = $('#SearchString').val();
            var _listAssets = $('#ListAssetsParaMovimento').val();

            $.get(url, {
                institutionId: _institutionId,
                budgetUnitId: _budgetUnitId,
                managerUnitId: _managerUnitId,
                administrativeUnitId: _administrativeUnitId,
                sectionId: _sectionId,
                searchString: _searchString,
                listAssets: _listAssets,
                movimentTypeId: _movimentTypeId,
            }, function () {
            }).done(function (data) {

                //if (data.Bps > 100)
                //{
                //    alert('Só é permitido Movimentar 100 Bps por vez.');
                //}

                $('#ListAssetsParaMovimento').val(JSON.stringify(data.lstDadosBD));

                //Recarrega os Grids para a atualização

                $(".adicionar").each(function () {
                    if ($('#ListAssetsParaMovimento').val().indexOf($(this).attr('id')) >= 0) {
                        $("#" + $(this).attr('id') + ".adicionar > .addBem").addClass("disabilitado");
                        $("#" + $(this).attr('id') + ".adicionar > .disabilitado").removeClass("addBem");
                    }
                });

                $(".addTodos").addClass("disabilitado");
                $(".disabilitado").removeClass("addTodos");
                movimento.CarregaDadosPatrimoniosParaMovimentoGrid(1);
                //movimento.CarregaDadosPatrimoniosDisponiveisGrid(1);
            }).fail(function () {
                alert('Erro ao carregar a rotina CarregaDadosPatrimoniosParaMovimentoGrid.');
            });
        });
    },

    EventoRemoveBem: function () {

        var arrayAssets = null;
        var assetId = null;
        var listAssets = null;

        $('.removerBem').click(function () {

            //Recupera array de bens
            arrayAssets = $('#ListAssetsParaMovimento').val();
            listAssets = JSON.parse(arrayAssets);

            //Remove do array o bem clicado
            assetId = parseInt($(this).closest('td')[0].id);
            listAssets = sam.commun.RemoveItemArray(listAssets, assetId);
            $('#ListAssetsParaMovimento').val(JSON.stringify(listAssets));

            //Recarrega os Grids para a atualização
            //movimento.CarregaDadosPatrimoniosDisponiveisGrid(1, false, true);
            $(".adicionar").each(function () {
                if ($('#ListAssetsParaMovimento').val().indexOf($(this).attr('id')) < 0) {
                    $("#" + $(this).attr('id') + ".adicionar > .disabilitado").addClass("addBem");
                    $("#" + $(this).attr('id') + ".adicionar > .addBem").removeClass("disabilitado");
                }
            });

            $(".btn-Multi-Add").removeClass("disabilitado");
            $(".btn-Multi-Add").addClass("addTodos");
            movimento.CarregaDadosPatrimoniosParaMovimentoGrid(1);

        });
    },

    EventoRemoveTodos: function () {
        $('.removerTodos').click(function () {

            $('#ListAssetsParaMovimento').val("[]");

            if ($(".adicionar").length > 0) {
                $(".btn-Multi-Add").addClass("addTodos");
                $(".btn-Multi-Add").removeClass("disabilitado");
            }
            $(".removerTodos").remove();
            $(".adicionar > a").addClass("addBem");
            $(".adicionar > a").removeClass("disabilitado");
            $(".removerBem").parents("tr").remove();
            $(".paginacaoListaParaMovimento > .pagination-container").hide();
            $(".paginacaoListaParaMovimento").html("Página 0 de 0");
            //Recarrega os Grids para a atualização
            //movimento.CarregaDadosPatrimoniosDisponiveisGrid(1, false, true);
            //movimento.CarregaDadosPatrimoniosParaMovimentoGrid(1);
        });
    },

    eventoSubmitForm: function () {

        //Atribui um id ao botão de submit
        $('button[type=submit]').attr('id', 'submit');

        //Remove atributo de submit do botão
        $('button[type=submit]').removeAttr('type');
        $('#submit').attr('type', 'button');

        $('#submit').click(function () {

            $('#spanMovimentType').val('');
            $('#spanMovimentoDate').val('');

            var arrayAssets = $('#ListAssetsParaMovimento').val();
            var listAssets = JSON.parse(arrayAssets);

            if (listAssets.length == 0) {
                $('#MsgQtdBensSelecionados').text('Por favor, selecione pelo menos um patrimônio para a movimentação.');
            }

            if (movimento.validaTransferencia() == true && listAssets.length > 0) {

                var _movimentTypeId = $('#MovementTypeId option:selected').val();

                if (
                        _movimentTypeId == movimento.EnumMovementType.Transferencia ||
                        _movimentTypeId == movimento.EnumMovementType.Doacao ||
                        _movimentTypeId == movimento.EnumMovementType.MovDoacaoIntraNoEstado ||
                        _movimentTypeId == movimento.EnumMovementType.MovTransferenciaMesmoOrgaoPatrimoniado ||
                        _movimentTypeId == movimento.EnumMovementType.MovTransferenciaOutroOrgaoPatrimoniado
                   ) {
                    var a = confirm(movimento.mensagemTransferenciaOuDoacao);
                    if (a == false) {
                        $('#submit').removeAttr('type');
                        $('#submit').attr('type', 'submit');
                        return false;
                    }
                }

                var x;
                var r = confirm("Deseja realmente movimentar " + listAssets.length + " Bp(s)?");
                if (r == false) {
                    $('#submit').removeAttr('type');
                    $('#submit').attr('type', 'submit');
                    return false;
                }

                $.blockUI({ message: $('#modal-loading') });

                //Habilita novamente os combos dos filtros
                $('#InstituationId').removeAttr('disabled');
                $('#BudgetUnitId').removeAttr('disabled');
                $('#ManagerUnitId').removeAttr('disabled');

                movimento.VinculaAssetsSelecionados();

                //Adiciona o atributo de submit do botão
                $('#submit').removeAttr('type');
                $('#submit').attr('type', 'submit');
                //Submete o formulário
                $('#submit').submit();
                //}
            }
        });
    },
    eventoSubmitFormItemInventario: function () {
        //Atribui um id ao botão de submit
        $('button[type=submit]').attr('id', 'submit');

        //Remove atributo de submit do botão
        $('button[type=submit]').removeAttr('type');
        $('#submit').attr('type', 'button');

        $('#submit').click(function () {

            $('#spanMovimentType').val('');
            $('#spanMovimentoDate').val('');

            if (movimento.validaTransferencia()) {
                //Adiciona o atributo de submit do botão
                $('#submit').removeAttr('type');
                $('#submit').attr('type', 'submit');

                $('#submit').submit();
            }

        });
    },
    eventoSubmitFormBolsa: function (msg) {
        $('#btnSalvarSubmit').click(function () {
            var resultado = confirm(msg);
            if (!resultado) {
                return false
            }
            $('#btnSalvarSubmit').submit();
        });
    },
    CarregaMensagemDeVAlidacaoOrgaoImplantado: function () {

        var url = "";
        var codigoManagerUnit = 0;

        if (sam.path.webroot[sam.path.webroot.length - 1] == "/") {
            url = sam.path.webroot + "Movimento/ValidaOrgaoImplantado";
        } else {
            url = sam.path.webroot + "/Movimento/ValidaOrgaoImplantado";
        }

        if ($('#ManagerUnitIdDestino').val() != null
            && $('#ManagerUnitIdDestino').val() != undefined
            && $('#ManagerUnitIdDestino').val() != '') {
            codigoManagerUnit = $('#ManagerUnitIdDestino').val();
        }

        $.get(url, {
            institutionIdDestino: $('#InstituationIdDestino').val(),
            managerUnitIdDestino: codigoManagerUnit
        }, function () {
        }).done(function (data) {

            movimento.mensagemTransferenciaOuDoacao = data.Texto;

        }).fail(function () {
            sam.commun.CriarAlertDinamico('Erro ao carregar a rotina de ValidaOrgaoImplantado.');
        });
    },

    InicializaNotificationMenu: function (exibeMenuLateralDireito) {

        if (exibeMenuLateralDireito) {
            $.get(sam.path.webroot + "Movimento/NumerosNotificacao", { parametros: sam.commun.Organiza() }, function (data) {
                var options = null;

                var notifications = new $.ttwNotificationMenu({
                    notificationList: {
                        anchor: 'item',
                        offset: '0 15'
                    }
                });

                notifications.initMenu({
                    bolsaEstado: '#bolsaEstado',
                    bolsaSecretaria: '#bolsaSecretaria',
                    pendentesDeIncorporacao: '#pendentesDeIncorporacao'
                });

                //Bolsa Estado --------------------------------------------------------------------
                options = {
                    category: 'bolsaEstado',
                    message: 'Existem itens na bolsa estadual'
                };

                for (var i = 1; i <= parseInt(data.qtdItensBolsaEstadual) ; i++) {

                    notifications.createNotification(options);
                }
                //---------------------------------------------------------------------------------

                //Bolsa Secretaria ----------------------------------------------------------------
                options = {
                    category: 'bolsaSecretaria',
                    message: 'Existem itens na bolsa secretaria'
                };

                for (var i = 1; i <= parseInt(data.qtdItensBolsaSecretaria) ; i++) {

                    notifications.createNotification(options);
                }
                //---------------------------------------------------------------------------------

                //Itens pendetes de incorporação --------------------------------------------------
                options = {
                    category: 'pendentesDeIncorporacao',
                    message: 'Existem itens pendentes de incorporação'
                };

                for (var i = 1; i <= parseInt(data.qtdItensPendentesIncorporacao) ; i++) {

                    notifications.createNotification(options);
                }
                //---------------------------------------------------------------------------------
            });
        }
    },

    LimpaListaDeBensSelecionados: function (lista) {

        if (lista == null || lista.length == 0) {
            //Inicializa lista de itens movimentados
            $('#ListAssetsParaMovimento').val(JSON.stringify([]));
        }
    },

    EventoMostraMensagemObrigatorioUGE: function () {
        if ($('#ManagerUnitId').val() == '0') {
            $('#spanManagerUnitFiltro').text('Por favor, informe uma UGE para filtrar patrimônios.');
        }
        else {
            $('#spanManagerUnitFiltro').text('');
        }
    },
    EventoChangeReportDoacaoTransferenciaLimpar: function () {
        $("#cbDocumento").change(function () {
            var cb1val = $(this).val();
            $("#cbDocumento option").each(function () {
                if ($(this).val() != "0") {
                    $("#SearchString").val("");
                    $("#btnPdf").prop("disabled", false)
                    if (($("#SearchString").val() == "") && ($("#cbDocumento").val() == "0"))
                        $("#btnPdf").prop("disabled", true)
                }

            });

        });
    },

    EventoSelecionaTodosRegistros: function () {

        $('input.checkTodosPatrimonio').change(function () {

            var checkRegisters = $('#result-tables-patrimonio-pendentes').find('.checkPatrimonio');
            var assetsChecks = [];

            if ($(this).is(":checked")) {

                for (var i = 0; i < checkRegisters.length; i++) {
                    $(checkRegisters[i]).prop('checked', true);

                    assetsChecks.push($(checkRegisters[i]).data('id'));
                }
                $('input[name="ListAssetsPendingSelected"]').val(JSON.stringify(assetsChecks));
            }
            else {
                for (var i = 0; i < checkRegisters.length; i++) {
                    $(checkRegisters[i]).prop('checked', false);
                }
                $('input[name="ListAssetsPendingSelected"]').val(JSON.stringify(assetsChecks));
            }
        });
    },

    EventoAlteraOrgaoDestino: function () {

        //Carrega a primeira vez
        if (
                $('#MovementTypeId').val() == '11' ||
                $('#MovementTypeId').val() == '12' ||
                $('#MovementTypeId').val() == '47' ||
                $('#MovementTypeId').val() == '56' ||
                $('#MovementTypeId').val() == '57'
           ) {
            movimento.CarregaMensagemDeVAlidacaoOrgaoImplantado();
        }
        else {
            movimento.mensagemTransferenciaOuDoacao = '';
        }


        $('#InstituationIdDestino').on('change', function () {

            if (
                    $('#MovementTypeId').val() == '11' ||
                    $('#MovementTypeId').val() == '12' ||
                    $('#MovementTypeId').val() == '47' ||
                    $('#MovementTypeId').val() == '56' ||
                    $('#MovementTypeId').val() == '57'
                ) {
                movimento.CarregaMensagemDeVAlidacaoOrgaoImplantado();
            }
            else {
                movimento.mensagemTransferenciaOuDoacao = '';
            }
        });
    },

    IncorporacaoAutomatizada: function () {
        $('#AceiteManual').val(false);
        $('#NumberDoc').val('');
        $('#NumberDocModel').val('');
        $('#ValueAcquisitionModel').attr('disabled', 'disabled');
        $('#AcquisitionDate').attr('disabled', 'disabled');
    },

    IncorporacaoManual: function () {
        $('#AceiteManual').val(true);
        $('#ValueAcquisitionModel').removeAttr('disabled');
        $('#AcquisitionDate').removeAttr('disabled');
        $('#NumberDocModel').val($('#NumberDoc').val());
        $('#AssetsIdTransferencia').val("");
    },

    //Tratamento para Doação Intra no estado, pois existem duas maneiras de preencher o número do documento
    ClonaInfNumberDocModelParaNumberDoc: function () {
        $('#NumberDoc').val($('#NumberDocModel').val());
    },


    SelecioneVisualizacaoAceite: function (manual) {

        if (manual == null || manual == undefined || manual == false) {
            manual = false;
        }

        if (manual) {
            $('#notafiscalManual').click();
        }
    },

    BloqueioInicialTransferenciaMesmoOrgao() {
        if ($('.comboManagerUnit2 option:selected').val() == $('.comboManagerUnitFiltro option:selected').val()) {
            if ($(".comboManagerUnit2 option[value='']").val() == undefined) {
                $('.comboManagerUnit2').val('0');
            } else {
                $('.comboManagerUnit2').val('');
            }
        }

        $(".comboManagerUnit2 option[disabled='disabled']").removeAttr('disabled');

        if ($('.comboManagerUnitFiltro option:selected').val() != '0') {
            $(".comboManagerUnit2 option[value='" + $('.comboManagerUnitFiltro option:selected').val() + "']").attr('disabled', 'disabled');
        } else {
            if ($(".comboManagerUnit2 option[value='']").val() == undefined) {
                $('.comboManagerUnit2').val('0');
            } else {
                $('.comboManagerUnit2').val('');
            }
        }

    },

    LoginSiafemMovimento: function (_movimentTypeId) {
        // SERVICO FEITO NAS PRESSAS

        if ($('#flagIntegracaoSiafem').val() == "1") {

            if (_movimentTypeId == movimento.EnumMovementType.MovSaidaInservivelDaUGEDoacao
                || _movimentTypeId == movimento.EnumMovementType.MovSaidaInservivelDaUGETransferencia
                || _movimentTypeId == movimento.EnumMovementType.MovComodatoConcedidoBensMoveis
                || _movimentTypeId == movimento.EnumMovementType.MovComodatoTerceirosRecebidos
                || _movimentTypeId == movimento.EnumMovementType.MovDoacaoConsolidacao
                || _movimentTypeId == movimento.EnumMovementType.MovDoacaoIntraNoEstado
                || _movimentTypeId == movimento.EnumMovementType.MovDoacaoMunicipio
                || _movimentTypeId == movimento.EnumMovementType.MovDoacaoOutrosEstados
                || _movimentTypeId == movimento.EnumMovementType.MovDoacaoUniao
                || _movimentTypeId == movimento.EnumMovementType.MovExtravioFurtoRouboBensMoveis
                || _movimentTypeId == movimento.EnumMovementType.MovMorteAnimalPatrimoniado
                || _movimentTypeId == movimento.EnumMovementType.MovMudancaCategoriaDesvalorizacao
                || _movimentTypeId == movimento.EnumMovementType.MovSementesPlantasInsumosArvores
                || _movimentTypeId == movimento.EnumMovementType.MovTransferenciaOutroOrgaoPatrimoniado
                || _movimentTypeId == movimento.EnumMovementType.MovTransferenciaMesmoOrgaoPatrimoniado
                || _movimentTypeId == movimento.EnumMovementType.MovPerdaInvoluntariaBensMoveis
                || _movimentTypeId == movimento.EnumMovementType.MovPerdaInvoluntariaInservivelBensMoveis
                || _movimentTypeId == movimento.EnumMovementType.MovInservivelNaUGE
                || _movimentTypeId == movimento.EnumMovementType.MovVendaLeilaoSemoventes) {

                $('#SaveLoginSiafem').click(function (e) {
                    e.target.disabled = true;

                    if ($('#CPFSIAFEMModal').val().length != 11) {
                        alert('Digite os 11 números do CPF');
                        e.target.disabled = false;
                        return false;
                    }

                    if ($('#LoginSiafem').val() == "" || $('#SenhaSiafem').val() == "") {
                        $('#LoginSiafem').val($('#CPFSIAFEMModal').val());
                        $('#SenhaSiafem').val($('#SenhaSIAFEMModal').val());
                    }

                    if ($('#LoginSiafem').val() == "" || $('#SenhaSiafem').val() == "") {
                        e.target.disabled = false;
                        return false;
                    }
                });

                if ($('#CPFSIAFEMModal').val() == "" || $('#SenhaSIAFEMModal').val() == "") {
                    $('[data-toggle="tooltip"]').tooltip();
                    $('#modalLoginSiafem').modal({ keyboard: false, backdrop: 'static', show: true });
                }
            }
        }

        // FIM SERVICO
    },
    LoginSiafemBolsa: function () {
        if ($('#LoginSiafem').val() == "" || $('#SenhaSiafem').val() == "") {
            movimento.LoginSiafemMovimento($('#MovementTypeId').val());
        }
    },
    Sucesso: function (msg) {
        if (msg != null && msg != undefined && msg != '') {
            alert(msg);
            //location.href = "/Patrimonio/Movimento";
        }
    },
    SucessoBolsa: function (msg, redirect) {
        if (msg != null && msg != undefined && msg != '') {
            alert(msg);
            location.href = redirect;
        }
    },
    UGEIntegradaSemLoginSiafem: function () {
        $('#SaveLoginSiafem').click(function () {
            if ($('#CPFSIAFEMModal').val().length != 11) {
                alert('Digite os 11 números do CPF');
                return false;
            }

            if ($('#LoginSiafem').val() == "" || $('#SenhaSiafem').val() == "") {
                $('#LoginSiafem').val($('#CPFSIAFEMModal').val());
                $('#SenhaSiafem').val($('#SenhaSIAFEMModal').val());

                if ($('#LoginSiafem').val() == "" || $('#SenhaSiafem').val() == "") {
                    return false;
                }
            }
        });

        if ($('#CPFSIAFEMModal').val() == "" || $('#SenhaSIAFEMModal').val() == "") {
            $('[data-toggle="tooltip"]').tooltip();
            $('#modalLoginSiafem').modal({ keyboard: false, backdrop: 'static', show: true });
        }
    },
    MostraExtratoSIAFEM: function (ids) {
        $("#modalExtratoSIAFEM").modal({ keyboard: false, backdrop: 'static', show: true });
        movimento.AbortarSIAFEM($("#Ids").val());
        movimento.ProsseguirSIAFEM($("#Ids").val());
    },
    ProsseguirSIAFEM: function (numero) {
        $("#prosseguir").click(function (e) {
            e.target.disabled = true;
            $.post(sam.path.webroot + "Movimento/Prosseguir",
            { auditorias: numero, LoginSiafem: $("#LoginSiafem").val(), SenhaSiafem: $("#SenhaSiafem").val() },
            function () { }).done(function (res) {
                $("#retornoSIAFEM").html(res);
                $("#modalRetornoSIAFEM").modal({ keyboard: false, backdrop: 'static', show: true });
                movimento.GerarPendenciaSIAFEM(numero, $("#LoginSiafem").val(), $("#SenhaSiafem").val());
                movimento.AbortarAposProsseguirSIAFEM(numero);
                $("#ok").click(function () {
                    window.location.href = "/Patrimonio/Movimento";
                });
            }).fail(function () { window.location.href = "/Patrimonio/Movimento"; });
        });
    },
    GerarPendenciaSIAFEM: function (numero, login, senha) {
        $("#gerar_pendencias").click(function (e) {
            e.target.disabled = true;
            $.post(sam.path.webroot + "Movimento/GerarPendenciaSIAFEM",
            {
                auditorias: numero, LoginSiafem: login, SenhaSiafem: senha
            },
            function () { }).done(function (res) {
                window.location.href = "/Patrimonio/Movimento";
            }).fail(function () {
                window.location.href = "/Patrimonio/Movimento";
            });

        });
    },
    AbortarSIAFEM: function (numero) {
        $("#abortar").click(function (e) {
            e.target.disabled = true;
            $.post(sam.path.webroot + "Movimento/Abortar", { auditorias: numero }, function () { }).done(function (res) {
                window.location.href = "/Patrimonio/Movimento";
            }).fail(function () {
                alert('Houve um erro ao abortar o processo.Estorne a movimentação pela tela Visão Geral');
                window.location.href = "/Patrimonio/Movimento";
            });

        });
    },
    AbortarAposProsseguirSIAFEM: function (numero) {
        $.blockUI({ message: $('#modal-loading') });
        $("#abortarApos").click(function (e) {
            e.target.disabled = true;
            $.post(sam.path.webroot + "Movimento/Abortar", { auditorias: numero }, function () { })
             .done(function (res) {
                 window.location.href = "/Patrimonio/Movimento";
             }).fail(function () {
                 alert('Houve um erro ao abortar o processo.Estorne a movimentação pela tela Visão Geral');
                 window.location.href = "/Patrimonio/Movimento";
             });
        });
    },
    
    CorrigePendenciaSIAFEMEstorno: function (_assetId, _assetMovementId, _movimentTypeId, _groupMovimentId, loginSiafem, senhaSiafem) {
        $.post(sam.path.webroot + "Estorno/CorrigePendenciaSiafem", { historico: _assetMovementId }, function () { })
         .done(function (res) {
             if (res.retorno != undefined && res.retorno != null && res.retorno != '')
             {
                 alert(res.retorno);
             }
             movimento.ChamadaEstornoDaMovimentacao(_assetId, _assetMovementId, _movimentTypeId, _groupMovimentId, true);
         }).fail(function () {
             alert('Ocorreu um erro na hora de corrigir as pendências do histórico. A página será recarregada');
             location.reload();
         });;
    },
    BuscaExtratosNLEstorno: function (_assetId, _assetMovementId, _movimentTypeId, _groupMovimentId, login, senha) {
        $.post(sam.path.webroot + "Estorno/GeraExtratos", { historico: _assetMovementId }, function () { })
         .done(function (res) {
             $(".extratos").empty();
             $.each(res.textos, function (key, value) {
                 $(".extratos").append('<div class="text-center" style="border: 1px solid #000">' + value + '</div><br />');
             });
             $("#modalExtratoEstornoSIAFEM").modal({ keyboard: false, backdrop: 'static', show: true });
             movimento.ProsseguirSIAFEMEstorno(_assetId, _assetMovementId, _movimentTypeId, _groupMovimentId, login, senha, res.auditorias);
             movimento.AbortaSIAFEMEstorno(res.auditorias);
         }).fail(function () {
             alert('Ocorreu um erro. A página será recarregada');
             location.reload();
         });;
    },
    ProsseguirSIAFEMEstorno: function (_assetId, _assetMovementId, _movimentTypeId, _groupMovimentId, login, senha, auditoria) {
        $("#prosseguir").click(function (e) {
            e.target.disabled = true;
            $("#modalExtratoEstornoSIAFEM").modal('hide');
            $.post(sam.path.webroot + "Estorno/Prosseguir",
                {
                    auditoria: auditoria,
                    LoginSiafem: login,
                    SenhaSiafem: senha,
                    assetId: _assetId,
                    assetMovementId: _assetMovementId,
                    movimentTypeId: _movimentTypeId,
                    groupMovimentId: _groupMovimentId
                }, function () { })
             .done(function (res) {
                 $("#retornoSIAFEM").html(res);
                 $("#modalRetornoSIAFEM").modal({ keyboard: false, backdrop: 'static', show: true });

                 movimento.ContinuaEstornoMovimento(_assetId, _assetMovementId, _movimentTypeId, _groupMovimentId);
                 movimento.AbortaAposProsseguirSIAFEMEstornoMovimento(auditoria);
                 movimento.GerarPendenciaSIAFEMEstorno(auditoria);
             }).fail(function () {
                 alert('Ocorreu um erro na hora de prosseguir. A página será recarregada');
                 location.reload();
             });
            return false;
        });
    },
    ContinuaEstornoMovimento: function (_assetId, _assetMovementId, _movimentTypeId, _groupMovimentId) {
        $("#continua").click(function (e) {
            e.target.disabled = true;
            movimento.ChamadaEstornoDaMovimentacao(_assetId, _assetMovementId, _movimentTypeId, _groupMovimentId, true);
            return false;
        });
    },
    AbortaAposProsseguirSIAFEMEstornoMovimento: function (auditoria) {
        $("#abortarApos").click(function (e) {
            e.target.disabled = true;
            $.post(sam.path.webroot + "Estorno/AbortarAposProsseguirMovimento", { auditoria: auditoria }, function () { })
             .done(function (res) {
                 location.reload();
             }).fail(function () {
                 alert('Ocorreu um erro na hora de abortar. A página será recarregada');
                 location.reload();
             });
        });
    },
    AbortaSIAFEMEstorno: function (auditoria) {
        $("#abortar").click(function (e) {
            e.target.disabled = true;
            $.post(sam.path.webroot + "Estorno/Abortar", { auditoria: auditoria }, function () { })
             .done(function (res) {
                 location.reload();
             }).fail(function () {
                 alert('Ocorreu um erro na hora de abortar o estorno. A página será recarregada');
                 location.reload();
             });
        });
    },
    GerarPendenciaSIAFEMEstorno: function (auditorias) {
        $("#gerar_pendencias").click(function (e) {
            e.target.disabled = true;
            $.blockUI({ message: $('#modal-loading') });
            $.post(sam.path.webroot + "Estorno/GerarPendenciaSIAFEMEstorno",
                {
                    auditorias: auditorias
                }, function () { }).done(function (res) {
                    $.unblockUI({ message: $('#modal-loading') });
                    location.reload();
                }).fail(function () {
                    alert('Ocorreu um erro na hora de gerar pendências. A página será recarregada');
                    location.reload();
                });

        });
    }
}