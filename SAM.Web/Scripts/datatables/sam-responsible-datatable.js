ResponsibleDataTable = (function () {
    let tableName = '#tableResponsibles';
    let actionEditar = '.action-editar';
    let actionDetalhar = '.action-detalhar';
    let actionExcluir = '.action-excluir';
    let actionConsulta = '#spanPesquisa';
    let consultaText = '#search-tables';
    var _form;

    ResponsibleDataTable.prototype.Load = function (form) {
        _form = form;
        criarTabelaResponsible(form);
        consultarResponsibles();
    };
    let consultarResponsibles = function () {
        $(actionConsulta).click(function () {
            $(tableName).DataTable().clear();
            $(tableName).DataTable().destroy();
            criarTabelaResponsible(_form);
        });
    };
    //eventos
    let verificaNulidade = function (data) {
        if (data == null)
            return "";
        else
            return data;
    };

    let editarClickHandler = function (user) {
        window.location = './Responsibles/Edit/' + user.Id;
    };

    let detalharClickHandler = function (user) {
        window.location = './Responsibles/Details/' + user.Id;
    };

    let excluirClickHandler = function (user) {
        window.location = './Responsibles/Delete/' + user.Id;
    };
    //criação
    let criarTabelaResponsible = function (form) {
        let csrfToken = $('input[name="__RequestVerificationToken"]', $(form)).val();
        let table = $(tableName).DataTable({
            "bPaginate": true,
            "bLengthChange": true,
            "bFilter": false,
            "bSort": true,
            "bInfo": true,
            "bAutoWidth": false,
            "columnsDefs": [
                { "targets": [7], "searchable": false, "orderable": false, "visible": true }
            ],
            "processing":true,
            "serverSide":true,
            "ajax": {
                "url": "Responsibles/IndexJSONResult",
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
                {
                    "data": "InstitutionCode",
                    "className": "text-center",
                    "render": function (data, type, row) { return verificaNulidade(data); }
                },
                {
                    "data": "BudgetUnitCode",
                    "className": "text-center",
                    "render": function (data, type, row) { return verificaNulidade(data); }
                },
                {
                    "data": "ManagerUnitCode",
                    "className": "text-center",
                    "render": function (data, type, row) { return verificaNulidade(data); }
                },
                {
                    "data": "AdministrativeUnitCode",
                    "className": "text-center",
                    "render": function (data, type, row) { return verificaNulidade(data); }
                },
                {
                    "data": "AdministrativeUnitDescription",
                    "render": function (data, type, row) { return verificaNulidade(data); }
                },
                { "data": "Name" },
                { "data": "Position" },
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
                $(actionEditar, row).bind('click', () => {
                    editarClickHandler(data);
                });

                $(actionDetalhar, row).bind('click', () => {
                    detalharClickHandler(data);
                });

                $(actionEditar, row).bind('mouseover', () => {
                    $(actionEditar, row).css('cursor', 'pointer');
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