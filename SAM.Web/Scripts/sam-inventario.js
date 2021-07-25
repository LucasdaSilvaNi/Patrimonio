sam.inventario = {
    divView: '.divView',
    modalView: '.modalView',

    init: function () {
        $('[data-toggle="tooltip"]').tooltip();
    },
    Load: function () {
        $(document).ready(function () {
            $(window).load(function () {
                sam.inventario.CriarDivMensagemModal();
            });
        });
    },
    atualizar: function(_InventarioId, _page) {
        $.get(sam.path.webroot + "Inventarios/VerificarInventario", { InventarioId: _InventarioId, page: _page }, function (data) {
            try {
                if (data[0].Id == 0) {
                    $('#modal').modal('show');
                    $('#save-btn-modal').hide();
                    $('#mensagemmodal').html(data[0].Mensagem);
                }
                else if (data[0].Id == 1)
                {
                    $('#modal').modal('show');
                    $('#save-btn-modal').show();
                    $('#save-btn-modal').click(function () { sam.inventario.finalizarInventario(_InventarioId) });
                    $('#mensagemmodal').html(data[0].Mensagem);
                }
                else if (data[0].Id == 2)
                {
                    $('#modal').modal('show');
                    $('#save-btn-modal').hide();
                    $('#mensagemmodal').html(data[0].Mensagem);
                }
            }
            catch (e) {
                // Possivelmente o perfil de usuário esta sem permisão de acesso.
                $('#modal').modal('show');
                $('#save-btn-modal').hide();
                $('#mensagemmodal').html("1: Não foi possivel realizar a pesquisa do Inventario, favor contatar o administrador responsável!");
            }
        });
    },
    finalizarInventario: function (_InventarioId) {
        $.get(sam.path.webroot + "Inventarios/FinalizarInventario", { InventarioId: _InventarioId}, function (data) {
            try {
                $('#modal').modal('show');
                $('#save-btn-modal').hide();
                $('#mensagemmodal').html(data);
                $('#modal').on('hidden.bs.modal', function () {
                    location.reload()
                });
            }
            catch (e) {
                // Possivelmente o perfil de usuário esta sem permisão de acesso.
                $('#modal').modal('show');
                $('#save-btn-modal').hide();
                $('#mensagemmodal').html("1: Não foi possivel realizar a pesquisa do Inventario, favor contatar o administrador responsável!");
            }
        });
    },
    atualizarItens: function (_InventarioId) {
        $.get(sam.path.webroot + "ItemInventarios/AtualizarItens", { InventarioId: _InventarioId }, function (data) {
            try {
                location.reload();
            }
            catch (e) {
                // Possivelmente o perfil de usuário esta sem permisão de acesso.
                $('#modal').modal('show');
                $('#save-btn-modal').hide();
                $('#mensagemmodal').html("1: Não foi possivel atualizar, favor contatar o administrador responsável!");
            }
        });
    },
    criaRelatorioInventarioManual: function () {
        $('.intervaloModal').modal('hide');
        //$.blockUI({ message: $('#modal-loading') });

        var _url = sam.path.webroot + "Inventarios/ReportInventarioManual";
        var _orgaoID = $('#InstitutionId').val();
        var _uoID = $('#BudgetUnitId').val();
        var _ugeID = $('#ManagerUnitId').val();
        var _uaID = $('#AdministrativeUnitId').val();
        var _responsavelID = $('#idResponsable').val();
        var _divisaoID = (($('#SectionId').val() != "") ? $('#SectionId').val() : 0);


        $.post(_url, { orgaoID: _orgaoID, uoID: _uoID, ugeID: _ugeID, uaID: _uaID, responsavelID: _responsavelID, divisaoID: _divisaoID }, function () {
        }).done(function (data) {
            //$.unblockUI({ message: $('#modal-loading') });

            if (data.Mensagem != undefined)
                //return sam.commun.CriarAlertDinamico(data.Mensagem);
            {
                sam.commun.CriarAlertDinamico(data.Mensagem);
                data.preventDefault();
            }

            sam.commun.CriarAlertDinamico(data.mensagem, function () {
                location.reload();
            });

        }).fail(function () {
            alert('Erro ao carregar função de geração de \'Relatório de Inventário Manual.\'');
        });
    },
    gravaRelatorioInventarioManual: function () {
        $('.intervaloModal').modal('hide');
        //$.blockUI({ message: $('#modal-loading') });

        var _url = sam.path.webroot + "/Inventarios/SetReportInventarioManual";
        var orgaoId = parseInt($('#InstitutionId').val());
        var uoId = parseInt($('#BudgetUnitId').val());
        var ugeId = parseInt($('#ManagerUnitId').val());
        var uaId = parseInt($('#AdministrativeUnitId').val());
        var responsavelId = parseInt($('#idResponsable').val());
        var divisaoId = parseInt((($('#SectionId').val() != "") ? $('#SectionId').val() : 0));

        //int orgaoId, int uoId, int ugeId, int uaId, int responsavelId, int divisaoId
        $.get(_url, { orgaoId: orgaoId, uoId: uoId, ugeId: ugeId, uaId: uaId, responsavelId: responsavelId, divisaoId: divisaoId }, function (data) {
            //$.unblockUI({ message: $('#modal-loading') });
        }).done(function (data) {
            if (data.Mensagem != undefined)
            {
                sam.commun.CriarAlertDinamico(data.Mensagem);
            }
            else
            {
                sam.commun.CriarAlertDinamico('Erro ao processar relatório!');
            }
        }).fail(function () {
            alert('Erro ao carregar função de geração de \'Relatório de Inventário Manual.\'');
        });
    },

    geraPdfRelatorioInventarioManual: function () {
        $('.intervaloModal').modal('hide');
        //$.blockUI({ message: $('#modal-loading') });

        var _url = sam.path.webroot + "Inventarios/GeraRelatorioInventarioManual";
        var _orgaoID = $('#InstitutionId').val();
        var _uoID = $('#BudgetUnitId').val();
        var _ugeID = $('#ManagerUnitId').val();
        var _uaID = $('#AdministrativeUnitId').val();
        var _responsavelID = $('#idResponsable').val();
        var _divisaoID = (($('#SectionId').val() != "") ? $('#SectionId').val() : 0);


        $.post(_url, { orgaoID: _orgaoID, uoID: _uoID, ugeID: _ugeID, uaID: _uaID, responsavelID: _responsavelID, divisaoID: _divisaoID }, function () {
        }).done(function (data) {
            //$.unblockUI({ message: $('#modal-loading') });

            if (data.Mensagem != undefined) {
                sam.commun.CriarAlertDinamico(data.Mensagem);
            }
            else {
                sam.commun.CriarAlertDinamico('Erro ao processar relatório!');
            }
        }).fail(function () {
            alert('Erro ao carregar função de geração de \'Relatório de Inventário Manual.\'');
        });
    },

    ImportaArquivo_ColetorDados: function () {

        $('.modalViewInformeProcessamento').modal('hide');
        $.blockUI({ message: $('#modal-loading') });

        var _url = sam.path.webroot + "/OperacoesComColetor/ImportaArquivo_ColetorDados";

        $.post(_url, function () {
        }).done(function (data) {

            $.unblockUI({ message: $('#modalViewInformeProcessamento') });

            if (data.Mensagem != undefined)
                sam.commun.CriarAlertDinamico(data.Mensagem);
            //$('.modalViewGridReabertura').modal('show');

            sam.commun.CriarAlertDinamico(data.mensagem, function () {
                location.reload();
            });

        }).fail(function () {
            alert('Erro ao processar arquivo de carga do coletor!');
        });
    },

    existeItensInventarioPendentesDePreenchimento: function (inventarioID) {
        $('.intervaloModal').modal('hide');

        var _url = sam.path.webroot + "Inventarios/ExisteItensInventarioPendentesDePreenchimento";

        $.post(_url, { inventarioID: inventarioID }, function () {
        }).done(function (data) {

            if (data.Mensagem != undefined) {
                sam.commun.CriarAlertDinamico(data.Mensagem);
            }
            else {
                sam.commun.CriarAlertDinamico('Erro ao processar função \'ExisteItensInventarioPendentesDePreenchimento\'');
            }
        }).fail(function () {
            alert('Erro ao carregar função \'ExisteItensInventarioPendentesDePreenchimento\'');
        });
    },

    pesquisarBemPatrimonial: function () {
        if ($("#NumberCode").val() != "") {
            $.blockUI({ message: $('#modal-loading') });
            $.get(sam.path.webroot + "ItemInventarios/PesquisarBemPatrimonial", { Chapa: $("#NumberCode").val(), InventarioId: $("#InventarioId").val() }, function (data) {
                try {
                    var retorno = JSON.parse(data);

                    if (retorno.Estado == 1) {
                        $('#Code').val(retorno.Code);
                        $('#Item').val(retorno.Item);
                        $('#EstadoDescricao').val(retorno.EstadoDescricao);
                        $('#Estado').val(retorno.Estado);
                        $.unblockUI({ message: $('#modal-loading') });
                    }
                    else {
                        $('#Code').val(retorno.Code);
                        $('#Item').val(retorno.Item);
                        $('#EstadoDescricao').val(retorno.EstadoDescricao);
                        $('#Estado').val(retorno.Estado);
                        $('#modal').modal('show');
                        $('#save-btn-modal').hide();
                        $('#mensagemmodal').html(retorno.Mensagem);
                        $.unblockUI({ message: $('#modal-loading') });
                    }
                }
                catch (e) {
                    // Possivelmente o perfil de usuário esta sem permisão de acesso.
                    $.unblockUI({ message: $('#modal-loading') });
                    $('#modal').modal('show');
                    $('#Code').val('');
                    $('#Item').val('');
                    $('#EstadoDescricao').val('');
                    $('#Estado').val('');
                    $('#save-btn-modal').hide();
                    $('#mensagemmodal').html("1: Não foi possivel realizar a pesquisa do Bem Patrimonial, favor contatar o administrador responsável!");
                }
            }).fail(function () {
                $.unblockUI({ message: $('#modal-loading') });
                $('#modal').modal('show');
                $('#Code').val('');
                $('#Item').val('');
                $('#EstadoDescricao').val('');
                $('#Estado').val('');
                $('#save-btn-modal').hide();
                $('#mensagemmodal').html("2: Não foi possivel realizar a pesquisa do Bem Patrimonial, favor contatar o administrador responsável!");
            });
        }
        else {
            $('#modal').modal('show');
            $('#save-btn-modal').hide();
            $('#Code').val('');
            $('#Item').val('');
            $('#EstadoDescricao').val('');
            $('#Estado').val('');
            $('#mensagemmodal').html("Favor informar a Chapa para realizar a pesquisa!");
        }
    },
    CriarDivMensagemModal: function () {
        var htmlBuilder = [];

        htmlBuilder.push('<div class="modal fade modalView" id="modal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel">');
        htmlBuilder.push('<div class="modal-dialog" role="document">');
        htmlBuilder.push('    <div class="modal-content">');
        htmlBuilder.push('        <div class="modal-header">');
        htmlBuilder.push('            <button type="button" id="closemodal" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>');
        htmlBuilder.push('            <h4 class="modal-title" id="exampleModalLabel">Inventário</h4>');
        htmlBuilder.push('        </div>');
        htmlBuilder.push('        <div class="modal-body">');
        htmlBuilder.push('                    <div class="form-group">');
        htmlBuilder.push('                <label for="recipient-name" id="mensagemmodal" class="control-label"></label>');
        htmlBuilder.push('            </div>');
        htmlBuilder.push('        </div>');
        htmlBuilder.push('        <div class="modal-footer">');
        htmlBuilder.push('            <button type="button" id="save-btn-modal" class="btn submit-comentario btn-success btn-salvar">Finalizar</button>');
        htmlBuilder.push('            <button type="button" id="fecharModal" class="btn btn-primary buttonClose" data-dismiss="modal">Fechar</button>');
        htmlBuilder.push('        </div>');
        htmlBuilder.push('    </div>');
        htmlBuilder.push('</div>');
        htmlBuilder.push('</div>');
        htmlBuilder.join("")

        $(document.body).append(htmlBuilder.join(""));
    },
    MovimentacaoAutomaticaInventario: function (numero) {
        $.blockUI({ message: $('#modal-loading') });
        $.post("/Patrimonio/ItemInventarios/MovimentaDoItemDoInventario", { valor: numero }, function (data) {
           if (data.Mensagem != undefined) {
               alert(data.Mensagem);
               $.unblockUI({ message: $('#modal-loading') });
           } else {
               alert(data.resultado);
               $.unblockUI({ message: $('#modal-loading') });
               location.reload();
           }
        });
    },
    DevolucaoInventario: function (numero) {
        $.blockUI({ message: $('#modal-loading') });
        $.post("/Patrimonio/ItemInventarios/DevolucaoFisicaDoItemDoInventario", { valor: numero }, function (data) {
            if (data.Mensagem != undefined) {
                alert(data.Mensagem);
                $.unblockUI({ message: $('#modal-loading') });
            } else {
                alert(data.resultado);
                $.unblockUI({ message: $('#modal-loading') });
                location.reload();
            }
        });
    },
    CorrigeHierarquia: function (numero) {
        $.blockUI({ message: $('#modal-loading') });
        $.post("/Patrimonio/ItemInventarios/CorrigeHierarquia", { valor: numero }, function (data) {
            if (data.Mensagem != undefined) {
                alert(data.Mensagem);
                $.unblockUI({ message: $('#modal-loading') });
            } else {
                alert(data.resultado);
                $.unblockUI({ message: $('#modal-loading') });
                location.reload();
            }
        });
    }
}