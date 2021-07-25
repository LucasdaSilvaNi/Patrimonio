SectionsDataTable = (function () {
    let tableName = '#tableSections';
    let actionEditar = '.action-editar';
    let actionDetalhar = '.action-detalhar';
    let actionConsulta = '#spanPesquisa';
    let consultaText = '#search-tables';
    let cbStatus = "#cbStatus";
    var _form;
    SectionsDataTable.prototype.Load = function (form) {
        _form = form;
        criaTabelaSections(form);
        consultarSections();
    };
    let consultarSections = function () {
        $(actionConsulta).click(function () {
            $(tableName).DataTable().clear();
            $(tableName).DataTable().destroy();
            criaTabelaSections(_form);
        });
        $(cbStatus).change(function () {
            $(tableName).DataTable().clear();
            $(tableName).DataTable().destroy();
            criaTabelaSections(_form);
        });
    };
    //eventos
    let editarClickHandler = function (user) {
        window.location = './Sections/Edit/' + user.Id;
    };

    let detalharClickHandler = function (user) {
        window.location = './Sections/Details/' + user.Id;
    };
    //criação
    let criaTabelaSections = function (form) {
        let csrfToken = $('input[name="__RequestVerificationToken"]', $(form)).val();
        let table = $(tableName).DataTable({
            "bPaginate": true,
            "bLengthChange": true,
            "bFilter": false,
            "bSort": true,
            "bInfo": true,
            "bAutoWidth": false,
            "columnsDefs": [
                { "targets": [8], "searchable": false, "orderable": false, "visible": true }
            ],
            "processing": true,
            "serverSide": true,
            "ajax": {
                "url": "Sections/IndexJSONResult",
                "type": "POST",
                "headers": {
                    "XSRF-TOKEN": csrfToken
                },
                "dataType": "JSON",
                "data": {
                    "currentFilter": $(consultaText).val(),
                    "cbStatus": $(cbStatus).val(),
                    "currentHier": sam.commun.Organiza()
                }
            },
            "columns": [
                {
                    "data": "CodigoOrgao",
                    "className": "text-center"
                },
                {
                    "data": "CodigoUO",
                    "className": "text-center"
                },
                {
                    "data": "CodigoUGE",
                    "className": "text-center"
                },
                {
                    "data": "CodigoUA",
                    "className": "text-center"
                },
                { "data": "DescricaoUA" },
                {
                    "data": "Code",
                    "className": "text-center"
                },
                { "data": "Description" },
                //{ "data": "Responsavel" },
                {
                    "data": "Bp",
                    "className": "text-center"
                },
                {
                    "defaultContent": `<a class="btnEditar action-editar" title="Editar"><i class="glyphicon glyphicon-pencil editarclass"></i><span class ="sr-only">Editar</span></a>&nbsp;
                                       <a class ="btnDetalhe action-detalhar" title="Detalhes"><i class ="glyphicon glyphicon-search detalheclass"></i><span class="sr-only">Detalhes</span></a>&nbsp;</div>`,
                    "className": "text-center"
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