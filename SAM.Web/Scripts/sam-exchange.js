sam.exchange = {
    CriarDivGrid: function (_nome) {
        var htmlBuilder = [];

        htmlBuilder.push('<div class="modal fade modalView" id="modalViewGrid" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel">');
        htmlBuilder.push('<div class="modal-dialog" role="document">');
        htmlBuilder.push('    <div class="modal-content">');
        htmlBuilder.push('        <div class="modal-header">');
        htmlBuilder.push('            <h4 class="modal-title" id="exampleModalLabel"> ' + _nome + ' </h4>');
        htmlBuilder.push('        </div>');
        htmlBuilder.push('        <div class="modal-body">');
        htmlBuilder.push('                    <div class="form-group">');
        htmlBuilder.push('                <label for="recipient-name" id="mensagemmodal" class="control-label"></label>');
        htmlBuilder.push('            </div>');
        htmlBuilder.push('        </div>');
        htmlBuilder.push('        <div class="modal-footer">');
        htmlBuilder.push('            <button type="button" id="fecharModal" class="btn btn-primary buttonClose" data-dismiss="modal" >Fechar</button>');
        htmlBuilder.push('        </div>');
        htmlBuilder.push('    </div>');
        htmlBuilder.push('</div>');
        htmlBuilder.push('</div>');
        htmlBuilder.join("")

        $(document.body).append(htmlBuilder.join(""));
    },
    limparModalDivGrid: function () {
        $('.modalViewGrid').empty();
        $('.divViewGrid').empty();
        $('.modalViewGrid').remove();
    },

    EventoExcluirRequisicao: function (asseId, institution, budgetUnit, managerUnit) {
        sam.exchange.limparModalDivGrid();
        sam.exchange.CriarDivGrid("Bolsa");
        $.blockUI({ message: $('#modal-loading') });
        $.get(sam.path.webroot + "Exchange/ExcluirItemRequisicao", { _asseId: asseId, _institution: institution, _budgetUnit: budgetUnit, _managerUnit: managerUnit }, function (data) {

            try {
                if (data.Mensagem == undefined) {
                    var retorno = JSON.parse(data);

                    if (retorno > 0) {
                        $('#modalViewGrid').modal('show');
                        $('#mensagemmodal').html("Requisição excluida com sucesso!");
                        $('#modalViewGrid').on('hidden.bs.modal', function () {
                            location.reload().done($.unblockUI({ message: $('#modal-loading') }));
                        });
                    }
                    else {
                        $('#modalViewGrid').modal('show');
                        $('#mensagemmodal').html("Não foi possivel excluir a requisição do item da Bolsa, favor contatar o administrador responsável!");
                        $('#modalViewGrid').on('hidden.bs.modal', function () {
                            location.reload().done($.unblockUI({ message: $('#modal-loading') }));
                        });
                    }
                }
                else {
                    $('#modalViewGrid').modal('show');
                    $('#mensagemmodal').html(data.Mensagem);
                    $('#modalViewGrid').on('hidden.bs.modal', function () {
                        location.reload().done($.unblockUI({ message: $('#modal-loading') }));
                    });
                }
            }
            catch (e) {
                // Possivelmente o perfil de usuário esta sem permisão de acesso.
                $.unblockUI({ message: $('#modal-loading') });
                $('#modalViewGrid').modal('show');
                $('#mensagemmodal').html("1:Não foi possivel excluir a requisição do item da Bolsa, favor contatar o administrador responsável!");
            }
        }).fail(function () {
            $.unblockUI({ message: $('#modal-loading') });
            $('#modalViewGrid').modal('show');
            $('#mensagemmodal').html("1:Não foi possivel excluir a requisição do item da Bolsa, favor contatar o administrador responsável!");
        });
    },

    EventoRequisitar: function (asseId, institution, budgetUnit, managerUnit) {
        sam.exchange.limparModalDivGrid();
        sam.exchange.CriarDivGrid("Bolsa");
        $.blockUI({ message: $('#modal-loading') });
        $.get(sam.path.webroot + "Exchange/RequisitarItemNaBolsa", { _asseId: asseId, _institution: institution, _budgetUnit: budgetUnit, _managerUnit: managerUnit }, function (data) {

            try {
                if (data.Mensagem == undefined) {
                    var retorno = JSON.parse(data);

                    if (retorno > 0) {
                        $('#modalViewGrid').modal('show');
                        $('#mensagemmodal').html("Item Requisitado com sucesso!");
                        $('#modalViewGrid').on('hidden.bs.modal', function () {
                            location.reload().done($.unblockUI({ message: $('#modal-loading') }));
                        });
                    }
                    else {
                        $('#modalViewGrid').modal('show');
                        $('#mensagemmodal').html("Não foi possivel requisitar o item da Bolsa, favor contatar o administrador responsável!");
                        $('#modalViewGrid').on('hidden.bs.modal', function () {
                            location.reload().done($.unblockUI({ message: $('#modal-loading') }));//sam.exchange.FecharModalBolsa();
                        });
                    }
                }
                else {
                    $('#modalViewGrid').modal('show');
                    $('#mensagemmodal').html(data.Mensagem);
                    $('#modalViewGrid').on('hidden.bs.modal', function () {
                        location.reload().done($.unblockUI({ message: $('#modal-loading') }));
                    });
                }
            }
            catch (e) {
                // Possivelmente o perfil de usuário esta sem permisão de acesso.
                $.unblockUI({ message: $('#modal-loading') });
                $('#modalViewGrid').modal('show');
                $('#mensagemmodal').html("1: Não foi possivel requisitar o item da Bolsa, favor contatar o administrador responsável!");
            }
        }).fail(function () {
            $.unblockUI({ message: $('#modal-loading') });
            $('#modalViewGrid').modal('show');
            $('#mensagemmodal').html("1: Não foi possivel requisitar o item da Bolsa, favor contatar o administrador responsável!");
        });
    },

    AbrirModalBolsa: function (_assetId, _tela) {
        sam.exchange.CarregaGridBolsa(_assetId, _tela);
    },
    CarregaGridBolsa: function (_assetId, _tela) {
        $.blockUI({ message: $('#modal-loading') });

        $('#ListarItem').empty();
        $.get(sam.path.webroot + "Exchange/ListarItemNaBolsa", { assetId: _assetId, tela: _tela }, function () {
        }).done(function (data) {

            $('#ListarItem').html(data);
            $('#modal').modal('show');
            $('#modal').on('hidden.bs.modal', function () {
                sam.exchange.FecharModalBolsa();
            });

            $.unblockUI({ message: $('#modal-loading') });

        }).fail(function () {
            $.unblockUI({ message: $('#modal-loading') });
            alert('Erro na rotina CarregaGridBolsa.');
        });
    },
    FecharModalBolsa: function () {
        $('#modal').modal('hide');
    }
}