EventServicesContabilizaSPDataTable = (function () {
    let tableName = '#tableEventServicesContabilizaSP';
    let actionEditar = '.action-editar';
    let actionDetalhar = '.action-detalhar';
    let actionExcluir = '.action-excluir';
    let actionConsulta = '#spanPesquisa';
    let consultaText = '#search-tables';
    let permissao = '#temPermissao';
    var _form;

    EventServicesContabilizaSPDataTable.prototype.Load = function (form) {
        _form = form;
        criaTabelaEventServicesContabilizaSP(form);
        consultaEventServicesContabilizaSP();
    };
    let consultaEventServicesContabilizaSP = function () {
        $(actionConsulta).click(function () {
            $(tableName).DataTable().clear();
            $(tableName).DataTable().destroy();
            criaTabelaEventServicesContabilizaSP(_form);
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

        return dia + '/' + mes + '/' + data.getFullYear();// + ' ' + horas + ":" + minutos + ":" + segundos;
    };
    let verificaFlag = function (flag) {
        if (flag === true) {
            return "Sim";
        } else {
            return "Não";
        }
    };
    //eventos
    let editarClickHandler = function (user) {
        window.location = './EventServiceContabilizaSP/Edit/' + user.Id;
    };

    let detalharClickHandler = function (user) {
        window.location = './EventServiceContabilizaSP/Details/' + user.Id;
    };

    let excluirClickHandler = function (user) {
        window.location = './EventServiceContabilizaSP/Delete/' + user.Id;
    };
    //criação
    function criaTabelaEventServicesContabilizaSP(form) {
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
                "url": "EventServiceContabilizaSP/IndexJSONResult",
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
                { "data": "DescricaoTipoMovimento_SamPatrimonio" },
                { "data": "AccountEntryType" },
                { "data": "InputOutputReclassificationDepreciationType" },
                { "data": "DescricaoTipoMovimentacao_ContabilizaSP" },
                { "data": "MetaDataType_StockSourceDescription" },
                { "data": "MetaDataType_StockDestinationDescription" },
                { "data": "MetaDataType_DateFieldDescription" },
                { "data": "MetaDataType_AccountingValueFieldDescription" },
                {
                    "data": "MovimentacaoPatrimonialDepreciaOuReclassifica",
                    "render": function (data, type, row) { return verificaFlag(data); },
                    "class": "text-center"
                },
                { "data": "ExecutionOrder" },
                 {
                     "data": "DataAtivacaoTipoMovimentacao",
                     "render": function (data, type, row) {
                         return formatarData(new Date(parseInt(data.substr(6))));
                     },
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
                $('.sorting_asc').removeClass("sorting_asc");
            },
            "rowCallback": function (row, data, Object, index) {

                $(actionDetalhar, row).bind('click', () => {
                    detalharClickHandler(data);
                });

                $(actionDetalhar, row).bind('mouseover', () => {
                    $(actionDetalhar, row).css('cursor', 'pointer');
                });

                if ($(permissao).val() === '1') {
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