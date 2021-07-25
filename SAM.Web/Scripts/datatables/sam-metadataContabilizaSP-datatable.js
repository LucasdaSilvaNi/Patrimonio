MetadataContabilizaSPDataTable = (function () {
    let tableName = '#tableMetadata';
    let actionEditar = '.action-editar';
    let actionExcluir = '.action-excluir';
    let actionConsulta = '#spanPesquisa';
    let consultaText = '#search-tables';
    var _form;
    MetadataContabilizaSPDataTable.prototype.Load = function (form) {
        _form = form;
        criarTabelaMetadataContabilizaSP(form);
        consultarMetadataContabilizaSP();
    }
    let consultarMetadataContabilizaSP = function () {
        $(actionConsulta).click(function () {
            $(tableName).DataTable().clear();
            $(tableName).DataTable().destroy();
            criarTabelaMetadataContabilizaSP(_form);
        });
    };
    //eventos
    let editarClickHandler = function (metadado) {
        window.location = './MetaDataTypeServiceContabilizaSP/Edit/' + metadado.Codigo;
    };
    let excluirClickHandler = function (metadado) {
        window.location = './MetaDataTypeServiceContabilizaSP/Delete/' + metadado.Codigo;
    };
    //criação
    criarTabelaMetadataContabilizaSP = function (form) {
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
                "url": "MetaDataTypeServiceContabilizaSP/IndexJSONResult",
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
                { "data": "Codigo" },
                { "data": "Nome" },
                {
                    "defaultContent": `<div class='form-group' style='margin-left: -1%;'>
                                       <a class='action-editar' title='Editar'><i class='glyphicon glyphicon-pencil editarclass'></i><span class='sr-only'>Editar</span></a>
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