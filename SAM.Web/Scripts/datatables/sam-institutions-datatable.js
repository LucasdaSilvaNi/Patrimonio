InstitutionsDataTable = (function () {
    let tableName = '#tableInstitutions';
    let actionEditar = '.action-editar';
    let actionDetalhar = '.action-detalhar';
    let actionExcluir = '.action-excluir';
    let actionConsulta = '#spanPesquisa';
    let consultaText = '#search-tables';
    let cbFiltro = "#cbFiltro";
    var _form;
    InstitutionsDataTable.prototype.Load = function (form) {
        _form = form;
        criaTabelaInstitutions(form);
        consultaInstitutions();
    };
    let verificaFlag = function (flag) {
        if (flag === true) {
            return "Sim";
        } else {
            return "Não";
        }
    };
    let consultaInstitutions = function () {
        $(actionConsulta).click(function () {
            $(tableName).DataTable().clear();
            $(tableName).DataTable().destroy();
            criaTabelaInstitutions(_form);
        });
        $(cbFiltro).change(function () {
            $(tableName).DataTable().clear();
            $(tableName).DataTable().destroy();
            criaTabelaInstitutions(_form);
        });
    };
    //eventos
    let editarClickHandler = function (institution) {
        window.location = './Institutions/Edit/' + institution.Id;
    };

    let detalharClickHandler = function (institution) {
        window.location = './Institutions/Details/' + institution.Id;
    };

    let excluirClickHandler = function (institution) {
        window.location = './Institutions/Delete/' + institution.Id;
    };
    //criação
    let criaTabelaInstitutions = function (form) {
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
            "processing": true,
            "serverSide": true,
            "ajax": {
                "url": "Institutions/IndexJSONResult",
                "type": "POST",
                "headers": {
                    "XSRF-TOKEN": csrfToken
                },
                "dataType": "JSON",
                "data": {
                    "currentFilter": $(consultaText).val(),
                    "cbFiltro": $(cbFiltro).val()
                }
            },
            "columns": [
                { "data": "Codigo" },
                { "data": "Description" },
                { "data": "DescricaoResumida", "class": "text-center" },
                { "data": "CodigoGestao", "class": "text-center" },
                {
                    "data": "OrgaoImplantado",
                    "render": function (data, type, row) {
                        return verificaFlag(data);
                    },
                    "class": "text-center"
                },
                {
                    "data": "IntegradoComSiafem",
                    "render": function (data, type, row) {
                        return verificaFlag(data);
                    },
                    "class": "text-center"
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

                if ($("#PerfilPermissao").val() === '1') {
                    $(actionEditar, row).bind('click', () => {
                        editarClickHandler(data);
                    });

                    $(actionDetalhar, row).bind('click', () => {
                        detalharClickHandler(data);
                    });

                    $(actionExcluir, row).bind('click', () => {
                        excluirClickHandler(data);
                    });

                    $(actionEditar, row).bind('mouseover', () => {
                        $(actionEditar, row).css('cursor', 'pointer');
                    });

                    $(actionDetalhar, row).bind('mouseover', () => {
                        $(actionDetalhar, row).css('cursor', 'pointer');
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