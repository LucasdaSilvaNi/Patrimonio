ManagerUnitsDataTable = (function () {
    let tableName = '#tableManagerUnits';
    let actionEditar = '.action-editar';
    let actionDetalhar = '.action-detalhar';
    let actionExcluir = '.action-excluir';
    let actionConsulta = '#spanPesquisa';
    let consultaText = '#search-tables';
    let perfilPermissao = '#perfilPermissao';
    let perfilOperadorUGE = '#perfilOperadorUGE';
    let UgeAtual = '#UgeAtual';
    var _form;
    ManagerUnitsDataTable.prototype.Load = function (form) {
        _form = form;
        criaTabelaManagerUnits(form);
        consultarManagerUnits();
    };
    let consultarManagerUnits = function () {
        $(actionConsulta).click(function () {
            $(tableName).DataTable().clear();
            $(tableName).DataTable().destroy();
            criaTabelaManagerUnits(_form);
        });
    };
    //eventos
    let editarClickHandler = function (user) {
        window.location = './ManagerUnits/Edit/' + user.Id;
    };

    let detalharClickHandler = function (user) {
        window.location = './ManagerUnits/Details/' + user.Id;
    };

    let excluirClickHandler = function (user) {
        window.location = './ManagerUnits/Delete/' + user.Id;
    };
    //criação
    let criaTabelaManagerUnits = function (form) {
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
                "url": "ManagerUnits/IndexJSONResult",
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
                { "data": "BudgetUnitCode" },
                { "data": "BudgetUnitDescription" },
                {
                    "data": "Code",
                    "className" : "text-center"
                },
                { "data": "Description" },
                {
                    "data": "ManagmentUnit_YearMonthStart",
                    "className": "text-center"

                },
                {
                    "data": "ManagmentUnit_YearMonthReference",
                    "className": "text-center"
                },
                {
                    "data": "FlagSiafem",
                    "className": "text-center"
                },
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
            },
            "rowCallback": function (row, data, Object, index) {
                
                if ($("#perfilPermissao").val() === '1') {
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
                    $(actionEditar, row).addClass('icon-desactived');
                    $(actionExcluir, row).attr('title', 'Sem permissão para excluir');
                    $(actionExcluir, row).addClass('icon-desactived');
                }

                if ($(perfilOperadorUGE).val() === '1') {
                    if (data.Code != $(UgeAtual).val()) {
                        $(actionDetalhar, row).attr('title', 'Sem permissão para ver detalhes');
                        $(actionDetalhar, row).addClass('icon-desactived');
                    } else {
                        $(actionDetalhar, row).bind('click', () => {
                            detalharClickHandler(data);
                        });

                        $(actionDetalhar, row).bind('mouseover', () => {
                            $(actionDetalhar, row).css('cursor', 'pointer');
                        });
                    }
                } else {
                    $(actionDetalhar, row).bind('click', () => {
                        detalharClickHandler(data);
                    });

                    $(actionDetalhar, row).bind('mouseover', () => {
                        $(actionDetalhar, row).css('cursor', 'pointer');
                    });
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