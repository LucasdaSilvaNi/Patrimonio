AuditoriaIntegracaoDataTable = (function () {
    let tableName = '#tableAuditoriaIntegracao';
    let actionEditar = '.action-editar';
    let actionDetalhar = '.action-detalhar';
    let actionExcluir = '.action-excluir';
    let actionConsulta = '#spanPesquisa';
    let consultaText = '#search-tables';
    let permissao = '#temPermissao';
    var _form;

    AuditoriaIntegracaoDataTable.prototype.Load = function (form) {
        _form = form;
        criaTabelaAuditoriaIntegracao(form);
        consultarAuditoriaIntegracao();
    };
    let consultarAuditoriaIntegracao = function () {
        $(actionConsulta).click(function () {
            $(tableName).DataTable().clear();
            $(tableName).DataTable().destroy();
            criaTabelaAuditoriaIntegracao(_form);
        });
    };
    //eventos
    let editarClickHandler = function (user) {
        window.location = './AuditoriaIntegracao/Edit/' + user.Id;
    };

    let detalharClickHandler = function (user) {
        window.location = './AuditoriaIntegracao/Details/' + user.Id;
    };

    //let excluirClickHandler = function (user) {
    //    window.location = './AuditoriaIntegracao/Delete/' + user.Id;
    //};
    //criação
    function criaTabelaAuditoriaIntegracao (form) {
        let csrfToken = $('input[name="__RequestVerificationToken"]', $(form)).val();
        let table = $(tableName).DataTable({
            "bPaginate": true,
            "bLengthChange": true,
            "bFilter": false,
            "bSort": true,
            "bInfo": true,
            "bAutoWidth": false,
            "columnsDefs": [
                { "targets": [4], "searchable": false, "orderable": false, "visible": true }
            ],
            "processing": true,
            "serverSide": true,
            "ajax": {
                "url": "AuditoriaIntegracao/IndexJSONResult",
                "type": "POST",
                "headers": {
                    "XSRF-TOKEN": csrfToken
                },
                "dataType": "JSON",
                "data": {
                    "currentFilter": $(consultaText).val(),
                    "currentHier": sam.commun.Organiza()
                }
            },
            "columns": [
                { "data": "Id" },
                { "data": "DataHoraEnvioSIAFEM" },
                ////{ "data": "MsgEstimuloWS" },
                ////{ "data": "MsgRetornoWS" },
                { "data": "NomeSistema" },
                { "data": "UsuarioSAM" },
                { "data": "UsuarioSistemaExterno" },
                ////{ "data": "ManagerUnitId" },
                { "data": "TokenAuditoriaIntegracao" },
                { "data": "DataHoraRetornoSIAFEM" },
                { "data": "DocumentoId" },
                { "data": "TipoMovimento" },
                { "data": "DataMovimento" },
                { "data": "UgeOrigem" },
                { "data": "Gestao" },
                { "data": "Tipo_Entrada_Saida_Reclassificacao_Depreciacao" },
                { "data": "CpfCnpjUgeFavorecida" },
                { "data": "GestaoFavorecida" },
                { "data": "Item" },
                { "data": "TipoEstoque" },
                { "data": "Estoque" },
                { "data": "EstoqueDestino" },
                { "data": "EstoqueOrigem" },
                { "data": "TipoMovimentacao" },
                { "data": "ValorTotal" },
                { "data": "ControleEspecifico" },
                { "data": "ControleEspecificoEntrada" },
                { "data": "ControleEspecificoSaida" },
                { "data": "FonteRecurso" },
                { "data": "NLEstorno" },
                { "data": "Empenho" },
                { "data": "Observacao" },
                { "data": "NotaFiscal" },
                { "data": "ItemMaterial" },
                { "data": "NotaLancamento" },
                { "data": "MsgErro" },
                {
                    "defaultContent": `<div class='form-group text-center' style='margin-left: -1%;'><a class='action-editar' title='Editar'><i class='glyphicon glyphicon-pencil editarclass'></i><span class='sr-only'>Editar</span></a>
                                       <a class='action-detalhar' title='Detalhes'><i class='glyphicon glyphicon-search detalheclass'></i><span class='sr-only'>Detalhes</span></a>
                                       //<a class ='action-excluir' title='Excluir'><i class ='glyphicon glyphicon-remove deleteclass'></i><span class='sr-only'>Excluir</span></a></div>`,
                    "class": "action text-center"
                }
            ],
            "drawCallback": function () {
                $('.dataTables_paginate').attr('class', 'pagination-container pagination-datatable');
                $("#search-tables").attr('onKeyPress', 'return sam.search.clickEnterDataTable(this,event)');
                $('.sorting_asc').removeClass("sorting_asc");
            },
            "rowCallback": function (row, data, Object, index) {

                $(actionDetalhar, row).bind('click', () => {
                    detalharClickHandler(data);
                });

                $(actionDetalhar, row).bind('mouseover', () => {
                    $(actionDetalhar, row).css('cursor', 'pointer');
                });

                if (data["RegistroOK"] == true) //TEM NL
                {
                    $('td', row).css('background-color', '#ccffb3'); //verde 85%
                }

                if (data["RegistroOK"] == false && data["PossuiInconsistencias"] == false) //SEM NL E COM MENSAGEM DE ERRO
                {
                    $('td', row).css('background-color', '#ff6600'); //laranja 50%
                }

                if (data["PossuiInconsistencias"] == true) //SEM NL E SEM MENSAGEM DE ERRO
                {
                    $('td', row).css('background-color', '#ff5050'); //vermelho 66%
                }


                if ($(permissao).val() === '1') {
                    $(actionEditar, row).bind('click', () => {
                        editarClickHandler(data);
                    });

                    $(actionEditar, row).bind('mouseover', () => {
                        $(actionEditar, row).css('cursor', 'pointer');
                    });

                    //$(actionExcluir, row).bind('click', () => {
                    //    excluirClickHandler(data);
                    //});

                    //$(actionExcluir, row).bind('mouseover', () => {
                    //    $(actionExcluir, row).css('cursor', 'pointer');
                    //});
                } else {
                    $(actionEditar, row).attr('title', 'Sem permissão para editar');
                    $(actionEditar, row).find('i').addClass('icon-desactived');
                    //$(actionExcluir, row).attr('title', 'Sem permissão para excluir');
                    //$(actionExcluir, row).find('i').addClass('icon-desactived');
                }
                
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

});