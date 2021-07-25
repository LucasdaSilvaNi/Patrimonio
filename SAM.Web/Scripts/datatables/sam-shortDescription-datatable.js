ShortDescriptionDataTable = (function () {
    let tableName = '#tableShortDescription';
    let actionEditar = '.action-editar';
    let actionDetalhar = '.action-detalhar';
    let actionExcluir = '.action-excluir';
    let actionConsulta = '#spanPesquisa';
    let consultaText = '#search-tables';
    var _form;
    ShortDescriptionDataTable.prototype.Load = function (form) {
        _form = form;
        criarTabelaShortDescription(form);
        consultarShortDescription();
    }
    let consultarShortDescription = function () {
        $(actionConsulta).click(function () {
            $(tableName).DataTable().clear();
            $(tableName).DataTable().destroy();
            criarTabelaShortDescription(_form);
        });
    };
    //eventos
    let editarClickHandler = function (user) {
        window.location = './ShortDescriptionItem/Edit/' + user.Id;
    };

    let detalharClickHandler = function (user) {
        window.location = './ShortDescriptionItem/Details/' + user.Id;
    };

    let excluirClickHandler = function (user) {
        window.location = './ShortDescriptionItem/Delete/' + user.Id;
    };
    //criação
    criarTabelaShortDescription = function (form) {
        let csrfToken = $('input[name="__RequestVerificationToken"]', $(form)).val();
        let table = $(tableName).DataTable({
            "bPaginate": true,
            "bLengthChange": true,
            "bFilter": false,
            "bSort": true,
            "bInfo": true,
            "bAutoWidth": false,
            "columnsDefs": [
                { "targets": [1], "searchable": false, "orderable": false, "visible": true }
            ],
            "processing": true,
            "serverSide": true,
            "ajax":
            {
                "url": "ShortDescriptionItem/IndexJSONResult",
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
                {"data":"Description"},
                {
                    "defaultContent": `<div class='form-group' style='margin-left: -1%;'><a class='action-editar' title='Editar'><i class='glyphicon glyphicon-pencil editarclass'></i><span class='sr-only'>Editar</span></a>
                                       <a class='action-detalhar' title='Detalhes'><i class='glyphicon glyphicon-search detalheclass'></i><span class='sr-only'>Detalhes</span></a>
                                       <a class ='action-excluir' title='Excluir'><i class ='glyphicon glyphicon-remove deleteclass'></i><span class='sr-only'>Excluir</span></a></div>`,
                     "className": "text-center"
                }
            ],
            "drawCallback": function () {
                $('.dataTables_paginate').attr('class', 'pagination-container pagination-datatable');
                $("#search-tables").attr('onKeyPress', 'return sam.search.clickEnterDataTable(this,event)');
            },
            "rowCallback": function (row, data, Object, index) {
                $(actionEditar, row).bind('click', () => {
                    editarClickHandler(data);
                });

                $(actionEditar, row).bind('mouseover', () => {
                    $(actionEditar, row).css('cursor', 'pointer');
                });

                $(actionDetalhar, row).bind('click', () => {
                    detalharClickHandler(data);
                });

                $(actionDetalhar, row).bind('mouseover', () => {
                    $(actionDetalhar, row).css('cursor', 'pointer');
                });

                $(actionExcluir, row).bind('click', () => {
                    excluirClickHandler(data);
                });
                $(actionExcluir, row).bind('mouseover', () => {
                    $(actionExcluir, row).css('cursor', 'pointer');
                });
            },
            "oLanguage": {
                "sProcessing": $('#modal-loading').show(),
                "sThousands": ".",
                "sLengthMenu": "Mostrar _MENU_ registros",
                "sZeroRecords": "Não foram encontrados resultados",
                "sInfo": "Página _PAGE_ de _PAGES_",
                "sInfoEmpty": "Mostrando de 0 até 0 de 0 registros",
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