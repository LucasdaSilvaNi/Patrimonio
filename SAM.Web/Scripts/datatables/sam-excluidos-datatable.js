ExcluidosDataTable = (function () {
    let tableName = '#tableExcluidos';
    let consultaText = '#search-tables';
    let actionConsulta = '#spanPesquisa';
    let cbFiltro = "#cbFiltro";
    let perfilPermissao = '#perfilPermissao';
    let perfilOperadorUGE = '#perfilOperadorUGE';
    let UgeAtual = '#UgeAtual';
    var _form

    ExcluidosDataTable.prototype.Load = function (form) {
        _form = form;
        criaTabelaExcluidos(form);
        consultaExcluidos();
    };

    let verificaFlag = function (flag) {
        if (flag === true) {
            return "Sim";
        } else {
            return "Não";
        }
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

    let consultaExcluidos = function () {
        $(actionConsulta).click(function () {
            $(tableName).DataTable().clear();
            $(tableName).DataTable().destroy();
            criaTabelaExcluidos(_form);
        });
        $(cbFiltro).change(function () {
            $(tableName).DataTable().clear();
            $(tableName).DataTable().destroy();
            criaTabelaExcluidos(_form);
        });
    };

    //criação
    let criaTabelaExcluidos = function (form) {
        let csrfToken = $('input[name="__RequestVerificationToken"]', $(form)).val();
        let table = $(tableName).DataTable({
            "bPaginate": true,
            "bLengthChange": true,
            "bFilter": false,
            "bSort": true,
            "bInfo": true,
            "bAutoWidth": false,
            "columnsDefs": [
                { "targets": [19], "searchable": false, "orderable": false, "visible": true }
            ],
            "processing": true,
            "serverSide": true,
            "ajax": {
                "url": "Excluidos/IndexJSONResult",
                "type": "POST",
                "headers": {
                    "XSRF-TOKEN": csrfToken
                },
                "dataType": "JSON",
                "data": {
                    "currentFilter": $(consultaText).val(),
                    "cbFiltro": $(cbFiltro).val(),
                    "currentHier": sam.commun.Organiza()
                }
            },
            "columns": [
                { "data": "Sigla" },
                { "data": "Chapa", "class": "text-center" },
                { "data": "ItemMaterial", "class": "text-center" },
                { "data": "GrupoMaterial", "class": "text-center" },
                { "data": "UGECode", "class": "text-center" },
                { "data": "UACode", "class": "text-center" },
                { "data": "NomeResponsavel", "class": "text-center" },
                { "data": "Processo", "class": "text-center" },
                { "data": "ValorDeAquisicao", "class": "text-center" },
                {
                    "data": "DataDeAquisicaoCompleta", "class": "text-center",
                    "render": function (data, type, row) {
                        return formatarData(new Date(parseInt(data.substr(6))));
                    }
                },
                {
                    "data": "DataIncorporacao", "class": "text-center",
                    "render": function (data, type, row) {
                        return formatarData(new Date(parseInt(data.substr(6))));
                    }
                },
                { "data": "Tipo", "class": "text-center" },

                {
                    "data": "DataAcao", "class": "text-center",
                    "render": function (data, type, row) {
                        return formatarData(new Date(parseInt(data.substr(6))));
                    }
                },
                { "data": "Cpf", "class": "text-center" },
                { "data": "NotaLancamentoEstorno", "class": "text-center" },
                { "data": "NotaLancamentoDepreciacaoEstorno", "class": "text-center" },
                { "data": "NotaLancamentoReclassificacaoEstorno", "class": "text-center" },
            ],
            "drawCallback": function () {
                $('.dataTables_paginate').attr('class', 'pagination-container pagination-datatable');
                $("#search-tables").attr('onKeyPress', 'return sam.search.clickEnterDataTable(this,event)');
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

