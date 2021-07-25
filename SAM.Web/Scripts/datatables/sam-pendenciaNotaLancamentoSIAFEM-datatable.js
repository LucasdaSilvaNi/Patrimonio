NotaLancamentoPendenteSIAFEMDataTable = (function () {
    let tableName = '#tableNotasLancamentosPendentesSIAFEM';
    let actionConsulta = '#spanPesquisa';
    let consultaText = '#search-tables';
    let actionInclusaoManualNL = '#btnInclusaoManual';
    let actionReenvioAutomatico = '#btnReenvioAutomatico';
    let actionDetalheNLPendente = '#btnDetalheNotaLancamentoPendente';
    let campoNL = '#complementonl';
    let loginUsuarioSIAFEM = '#LoginSiafem';
    let senhaUsuarioSIAFEM = '#SenhaSiafem';
    let user = '';
    let path = '';
    var _form;
    NotaLancamentoPendenteSIAFEMDataTable.prototype.Load = function (form, usuario, caminho) {
        _form = form;
        user = usuario;
        path = caminho;
        criaTabelaNotasLancamentos(form);
        consultarNotasLancamentos();
    };
    let consultarNotasLancamentos = function () {
        $(actionConsulta).click(function () {
            $(tableName).DataTable().clear();
            $(tableName).DataTable().destroy();
            criaTabelaNotasLancamentos(_form);
        });
    };

    let formatarData = function (data) {
        var dia = data.getDate();
        if (dia < 10) dia = '0' + dia;

        var mes = data.getMonth() + 1;
        if (mes < 10) mes = '0' + mes;

        var horas = data.getHours();
        if (horas < 10) horas = '0' + horas;

        var minutos = data.getMinutes();
        if (minutos < 10) minutos = '0' + minutos;

        var segundos = data.getSeconds();
        if (segundos < 10) segundos = '0' + segundos;

        return dia + '/' + mes + '/' + data.getFullYear() + ' ' + horas + ":" + minutos + ":" + segundos;
    };
    //eventos
    let detalheNLPendente = function (_notaLancamentoPendenteSIAFEMCodigo) {
        $.blockUI({ message: $('#modal-loading') });

        $('#detalheNotaLancamentoPendente').empty();
        $.get(sam.path.webroot + "PendenciaNotaLancamentoSIAFEM/PreencheDetalhesNotaLancamentoPendente", { notaLancamentoPendenteSIAFEMCodigo: _notaLancamentoPendenteSIAFEMCodigo }, function () {
        }).done(function (data) {

            $('#detalheNotaLancamentoPendente').html(data);
            $('#modal').modal({ keyboard: false, backdrop: 'static', show: true });

            $.unblockUI({ message: $('#modal-loading') });

        }).fail(function () {
            $.unblockUI({ message: $('#modal-loading') });
            alert('Erro na rotina PreencheDetalhesNotaLancamentoPendente.');
        });
    };
    let inclusaoManualNL = function (codigo) {
        var complementoNL = $(".complementonl_" + codigo).val();
        if (complementoNL === null || complementoNL === undefined || complementoNL === '') {
            alert('Campo precisa ser preenchido!');
        } else {
            $.blockUI({ message: $('#modal-loading') });
            $.get(sam.path.webroot + "PendenciaNotaLancamentoSIAFEM/InclusaoManualNL", { Codigo: codigo, ComplNL: complementoNL }, function (data) {
                if (data.resultado) {
                    if (data.mensagem !== null || data.mensagem !== '') {
                        alert(data.mensagem);
                    } else {
                        alert('Erro ao processar resolução manual de Pendência Contabiliza-SP!');
                    }
                }
                atualizaPagina();
            });
        }
    };
    let reenvioAutomatico = function (codigo) {
        if ($("#LoginSiafem").val() == ""
            || $("#SenhaSiafem").val() == "") {
            LoginSiafemNotaLancamentoPendenteSIAFEM(codigo);
        } else {
            processaReenvioAutomatico(codigo);
        }
    };
    let processaReenvioAutomatico = function (codigo) {
        $.blockUI({ message: $('#modal-loading') });
        $.get(sam.path.webroot + "PendenciaNotaLancamentoSIAFEM/ReenvioAutomatico", { Codigo: codigo, LoginUsuarioSIAFEM: $(loginUsuarioSIAFEM).val(), SenhaUsuarioSIAFEM: $(senhaUsuarioSIAFEM).val() }, function (data) {
            if (data.mensagem !== null) {
                alert(data.mensagem);
            } else {
                alert('Erro ao processar reenvio automático de comando para sistema Contabiliza-SP!');
            }
            $("#codigo").val("");
            atualizaPagina();
        });
    };
    let LoginSiafemNotaLancamentoPendenteSIAFEM = function (codigo) {
            if ($('#modalLoginSiafem').length == 0) {
                CriarDivLoginSiafem();

                $('#SaveLoginSiafem').click(function () {

                    if ($('#CPFSIAFEMModal').val() === "") {
                        alert('O campo Login é obrigatório!');
                        return false;
                    } else {
                        if ($('#SenhaSIAFEMModal').val() === "") {
                            alert('O campo Senha é obrigatório!');
                            return false;
                        } else {
                            if ($('#CPFSIAFEMModal').val().length != 11) {
                                alert('Digite os 11 números do CPF');
                                return false;
                            } else {
                                $(loginUsuarioSIAFEM).val($('#CPFSIAFEMModal').val());
                                $(senhaUsuarioSIAFEM).val($('#SenhaSIAFEMModal').val());
                                processaReenvioAutomatico($("#codigo").val());
                            }
                        }
                    }
                });
            }

            $('[data-toggle="tooltip"]').tooltip();
            $('#modalLoginSiafem').modal({ keyboard: false, backdrop: 'static', show: true });

            $("#codigo").val(codigo);
    };

    //criação
    let criaTabelaNotasLancamentos = function (form) {
        let csrfToken = $('input[name="__RequestVerificationToken"]', $(form)).val();
        let table = $(tableName).DataTable({
            "bPaginate": true,
            "bLengthChange": true,
            "bFilter": false,
            "bSort": true,
            "bInfo": true,
            "bAutoWidth": false,
            "columnsDefs": [
                { "targets": [6], "searchable": false, "orderable": false, "visible": true }
            ],
            "processing": true,
            "serverSide": true,
            "ajax": {
                "url": "PendenciaNotaLancamentoSIAFEM/IndexJSONResult",
                "type": "POST",
                "headers": {
                    "XSRF-TOKEN": csrfToken
                },
                "dataType": "JSON",
                "data": {
                    "currentFilter": $(consultaText).val()
                }
            },
            "columns": [
                { "defaultContent": '<div class="form-group" style="margin-left: -1%;"><a id="btnDetalheNotaLancamentoPendente" class="btnDetalhePendencia action-detalhePendencia" href="#" title="Detalhe Pendência"><i class="glyphicon glyphicon-list-alt Historicoclass" style="font-size: 22px;"></i><span class="sr-only">Detalhe</span></a>&nbsp;</div>' },
                { "data": "Orgao" },
                { "data": "UO" },
                { "data": "UGE" },
                { "data": "NumeroDocumentoSAM" },
                { "data": "TipoAgrupamentoMovimentacaoPatrimonial" },
                { "data": "TipoMovimentacaoPatrimonial" },
                { "data": "ContaContabil" },
                { "data": "DataMovimentacao" },
                { "data": "ValorMovimentacao" },
                { "data": "DescricaoTipoNotaSIAFEM" },
                { "data": "ErroSIAFEM" },
                { "data": "DataEnvioSIAFEM" },
                {
                    "data": "PrefixoNL",
                    "render": function (data, type, row) {
                        return data + "<input type='text' id='complementonl' style='margin-left: 2%;' maxlength='5' name='search-tables' class='form-control text-box input-sm single-line' />";
                    },
                },
                {
                    "defaultContent": "<div class='form-group text-center' style='margin-left: -1%;'><a id='btnInclusaoManual' data-placement='top' style='background-color: #597EB7 !important; border-color: #597EB7 !important;' class='btn btn-info btnInclusaoManual'><i class='glyphicon glyphicon-pencil InclusaoManualclass' aria-hidden='true'></i> Inclusão Manual</a>" +
                                      "<a id='btnReenvioAutomatico' data-placement='top' style='background-color: #597EB7 !important; border-color: #597EB7 !important;margin-left: 10px;' class='btn btn-info btnReenvioAutomatico'><i class='glyphicon glyphicon-transfer ReenvioAutomaticolass' aria-hidden='true'></i> Reenvio SIAFEM</a></div>"
                }
            ],
            "drawCallback": function () {
                $('.dataTables_paginate').attr('class', 'pagination-container pagination-datatable');
                $("#search-tables").attr('onKeyPress', 'return sam.search.clickEnterDataTable(this,event)');
                if ($('.btnInclusaoManual').length > 0){
                    sam.transaction.controleTransacaoIndexNotaLancamentoPendenteSIAFEM(user, path);
                }
            },
            "rowCallback": function (row, data, Object, index) {

                $(campoNL, row).addClass('complementonl_' + data.Id);
                $(campoNL, row).on('input', function (event) {
                    //this.value = this.value.replace(/[^a-z]_/g, '');
                    this.value = this.value.replace(/[^A-Z0-9]$/g, ''); /* somente alfanumericos em caixa alta */
                });

                if ($("#perfilOperador").val() == 1 || $("#perfilOperador").val() == '1') {
                    $(actionInclusaoManualNL, row).bind('click', () => {
                        inclusaoManualNL(data.Id);
                    });

                    $(actionReenvioAutomatico, row).bind('click', () => {
                        reenvioAutomatico(data.Id);
                    });
                }
                $(actionDetalheNLPendente, row).bind('click', () => {
                    detalheNLPendente(data.Id);
                });
            },
            "oLanguage": {
                "sProcessing": $('#modal-loading').show(),
                "sThousands": ".",
                "sLengthMenu": "Mostrar _MENU_ registros",
                "sZeroRecords": "Não foram encontrados resultados",
                "sInfo": "Página _PAGE_ de _PAGES_",
                "sInfoEmpty": "",
                "sInfoFiltered": "",
                "sInfoPostFix": "",
                "sSearch": "Buscar:",
                "sUrl": "",
                "oPaginate": {
                    "sFirst": "Primeiro",
                    "sPrevious": "Anterior",
                    "sNext": "Próximo",
                    "sLast": "Último"
                }
            }
        });
    };

    let CriarDivLoginSiafem = function () {
        var htmlBuilder = [];

        htmlBuilder.push('<div class="modal fade modalView in" id="modalLoginSiafem" tabindex="-1" aria-labelledby="exampleModalLabel" aria-hidden="false">');
        htmlBuilder.push('	<div class="modal-dialog" style="width: 20%;">');
        htmlBuilder.push('		<div class="modal-content">');
        htmlBuilder.push('			<div class="modal-header">');
        htmlBuilder.push('				<h4 class="modal-title" id="exampleModalLabelLoginSiafem">Usuario SIAFEM</h4>');
        htmlBuilder.push('			</div>');
        htmlBuilder.push('			<div class="modal-body">');
        htmlBuilder.push('				<div class="form-group">');
        htmlBuilder.push('					<label class="col-md-2 control-label" for="CPF">Login</label>');
        htmlBuilder.push('					<div class="col-md-6">');
        htmlBuilder.push('						<input autocomplete="off" class="form-control" data-val="true" data-val-required="O campo Login é obrigatório." id="CPFSIAFEMModal" maxlength="11" name="CPF" type="text" value="">');
        htmlBuilder.push('							<span class="field-validation-valid text-danger" data-valmsg-for="CPF" data-valmsg-replace="true"/>');
        htmlBuilder.push('						</div>');
        htmlBuilder.push('					</div>');
        htmlBuilder.push('					<br>');
        htmlBuilder.push('						<div class="form-group">');
        htmlBuilder.push('							<label class="col-md-2 control-label" for="Senha">Senha</label>');
        htmlBuilder.push('							<div class="col-md-6">');
        htmlBuilder.push('								<input class="form-control" data-val="true" data-val-required="O campo Senha é obrigatório." id="SenhaSIAFEMModal" name="SenhaSIAFEMModal" onfocus="$(this).removeAttr(' + "'readonly'" + ')" readonly="readonly" type="password">');
        htmlBuilder.push('									<span class="field-validation-valid text-danger" data-valmsg-for="Senha" data-valmsg-replace="true"/>');
        htmlBuilder.push('								</div>');
        htmlBuilder.push('							</div>');
        htmlBuilder.push('						</div>');
        htmlBuilder.push('						<div class="modal-footer" style="border-top:0px !important">');
        htmlBuilder.push('							<button type="button" id="SaveLoginSiafem" class="btn submit-comentario btn-success btn-salvar " data-dismiss="modal">OK</button>');
        htmlBuilder.push('						</div>');
        htmlBuilder.push('					</div>');
        htmlBuilder.push('				</div>');
        htmlBuilder.push('			</div>');
        htmlBuilder.join("")

        $(document.body).append(htmlBuilder.join(""));
    };
    let atualizaPagina = function () {
        window.location = sam.path.webroot + 'PendenciaNotaLancamentoSIAFEM/ReloadIndex?LoginSIAFEM=' + $(loginUsuarioSIAFEM).val() + '&SenhaSIAFEM=' + $(senhaUsuarioSIAFEM).val();
    }
});