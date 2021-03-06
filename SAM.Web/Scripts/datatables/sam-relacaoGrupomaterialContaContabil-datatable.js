RelacaoGrupomaterialContaContabil = (function () {
    let tableName = '#tableRelacaoGrupomaterialContaContabil';
    let actionEditar = '.action-editar';
    let actionDetalhar = '.action-detalhar';
    let actionExcluir = '.action-excluir';
    let actionConsulta = '#spanPesquisa';
    let consultaText = '#search-tables';
    let temPermissao = '#temPermissao';
    var _form;

    RelacaoGrupomaterialContaContabil.prototype.Load = function (form) {
        _form = form;
        criarTabelaRelacao(form);
        consultarRelacao();
    };
    let consultarRelacao = function () {
        $(actionConsulta).click(function () {
            $(tableName).DataTable().clear();
            $(tableName).DataTable().destroy();
            criarTabelaRelacao(_form);
        });
    };
    //eventos
    let editarClickHandler = function (user) {
        window.location = './RelationshipAuxiliaryAccountItemGroup/Edit/' + user.Id;
    };

    let detalharClickHandler = function (user) {
        window.location = './RelationshipAuxiliaryAccountItemGroup/Details/' + user.Id;
    };

    let excluirClickHandler = function (user) {
        window.location = './RelationshipAuxiliaryAccountItemGroup/Delete/' + user.Id;
    };
    //criação
    let criarTabelaRelacao = function (form) {
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
                "url": "RelationshipAuxiliaryAccountItemGroup/IndexJSONResult",
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
                { "data": "CodigoGrupoMaterial" },
                { "data": "DescricaoGrupoMaterial" },
                {
                    "data": "ContaContabil",
                    "className" : "text-center"
                },
                { "data": "DescricaoContaContabil" },
                {
                    "defaultContent": `<div class='form-group text-center' style='margin-left: -1%;'><a class='action-editar' title='Editar'><i class='glyphicon glyphicon-pencil editarclass'></i><span class='sr-only'>Editar</span></a>
                                       <a class='action-detalhar' title='Detalhes'><i class='glyphicon glyphicon-search detalheclass'></i><span class='sr-only'>Detalhes</span></a>
                                       <a class ='action-excluir' title='Excluir'><i class ='glyphicon glyphicon-remove deleteclass'></i><span class='sr-only'>Excluir</span></a></div>`,
                    "class": "action text-center"
                }
            ],
            "drawCallback": function () {
                $('.dataTables_paginate').attr('class', 'pagination-container pagination-datatable');
                $("#search-tables").attr('onKeyPress', 'return sam.search.clickEnterDataTable(this,event)');
                $('.sorting_asc').removeClass("sorting_asc");
            },
            "rowCallback": function (row, data, Object, index) {

                if ($(temPermissao).val() === '1') {
                    $(actionEditar, row).bind('click', () => {
                        editarClickHandler(data);
                    });

                    $(actionEditar, row).bind('mouseover', () => {
                        $(actionEditar, row).css('cursor', 'pointer');
                    });

                    $(actionExcluir, row).bind('click', () => {
                        excluirClickHandler(data);
                    });
                    $(actionExcluir, row).bind('mouseover', () => {
                        $(actionExcluir, row).css('cursor', 'pointer');
                    });
                } else {
                    $(actionEditar, row).attr('title', 'Sem permissão para editar');
                    $(actionEditar, row).find('i').addClass('icon-desactived');
                    $(actionExcluir, row).attr('title', 'Sem permissão para excluir');
                    $(actionExcluir, row).find('i').addClass('icon-desactived');
                }

                $(actionDetalhar, row).bind('click', () => {
                    detalharClickHandler(data);
                });

                $(actionDetalhar, row).bind('mouseover', () => {
                    $(actionDetalhar, row).css('cursor', 'pointer');
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
});