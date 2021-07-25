AssetPendingDataTable = (function () {
    let tableName = '#tableAssetPending';
    let actionDetalhar = '.detalheclass';
    let actionEditar = '.editarclass';
    let currentFilter = '.current-filter';
    let cbFiltros = '#cbFiltros';
    let actionConsulta = '#spanPesquisa';
    let consultaText = '#search-tables';
    let tester;
    let _form;
    AssetPendingDataTable.prototype.Load = function (form) {
        _form = form;
        criarTableAssetPending(form);
        consultarAssetPending();
    };
    //evento
    let editarClickHandler = function (assetpending) {
        window.location = './AssetPending/Edit/' + assetpending.Id;
    };
    let detalharClickHandler = function (assetpending) {
        window.location = './AssetPending/Details/' + assetpending.Id;
    };
    let formatarMoedaBrasil = function (valor) {
        return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(valor);
    };
    let formatarDecimalBrasil = function (valor) {
        return new Intl.NumberFormat('pt-BR', { maximumSignificantDigits: 20 }).format(valor);
    };
    let consultarAssetPending = function () {
        $(actionConsulta).click(function () {
            $(tableName).DataTable().clear();
            $(tableName).DataTable().destroy();
            criarTableAssetPending(_form);
        });
    };
    //criação
    criarTableAssetPending = function (form) {
        let csrfToken = $('input[name="__RequestVerificationToken"]', $(form)).val();
        let table = $(tableName).DataTable({
            "bPaginate": true,
            "bLengthChange": true,
            "bFilter": false,
            "bSort": true,
            "bInfo": true,
            "bAutoWidth": false,
            "columnDefs": [
                { "targets": [8], "searchable": false, "orderable": false, "visible": true }
            ],
            "language":
            {
                "processing": '<div class="modal- loading" id="modal - loading"><div><div><img src="~/Content/images/preloader03.gif" class="modal-img" /><span>Carregando...</span></div></div></div>',
                "decimal": ",",
                "thousands": "."
            },
            "processing": true,
            "serverSide": true,
            "ajax":
            {
                "url": "AssetPending/IndexJSONResult",
                "type": "POST",
                "headers": {
                    "XSRF-TOKEN": csrfToken
                },
                "dataType": "JSON",
                "data": {
                    "currentFilter": $(consultaText).val(),
                    "cbFiltro": $(cbFiltros).val(),
                    "currentHier": sam.commun.Organiza()
                }

            },
            "columns": [
                {
                    "data": "Sigla",
                    "render": function (data, type, row) {
                                        tester = data;
                                        if (data == null) {
                                            return '';
                                        } else {
                                            return data;
                                        }
                                    }
                },
                {
                    "data": "Chapa",
                    "className": "text-center"
                },
                {
                    "data": "CodigoMaterial",
                    "className": "text-center",
                },
                { "data": "DescricaoMaterial" },
                {
                    "data": "UGE",
                    "render": function (data, type, row) {
                                      if (data == null) {
                                        return '';
                                    } else {
                                        return data;
                                    }
                                }
                },
                {
                    "data": "UA",
                    "render": function (data, type, row) {
                                      if (data == null) {
                                        return '';
                                    } else {
                                        return data;
                                    }
                                }
                },
                {
                    "data": "ValorAquisicao",
                    "className": "text-center",
                    "render": function(data,type,row){
                        return formatarMoedaBrasil(data);
                    }
                },
                {
                    "data": "TaxaDepreciacaoMensal",
                    "className": "text-center",
                    "render": function(data,type,row){
                        return formatarDecimalBrasil(data);
                    }
                },
                {
                    "defaultContent": `<div class="form-group" style="margin-left: -1%;">
                    <a class="btnEditar" title="Editar"><i class="glyphicon glyphicon-pencil editarclass"></i><span class ="sr-only">Editar</span></a>
                    <a class ="btnDetalhe" title="Detalhes">
                    <i class="glyphicon glyphicon-search detalheclass"></i><span class="sr-only">Detalhes</span></a></div>` }
            ],
            "drawCallback": function () {
                var api = this.api();
                $('.dataTables_paginate').attr('class', 'pagination-container pagination-datatable');
                $("#search-tables").attr('onKeyPress', 'return sam.search.clickEnterDataTable(this,event)');
                if (api.rows().count() === 0) {
                    $('.btnExcel').attr("disabled", "disabled");
                } else {
                    if ($('#desabilitaExcel').val() == '1') {
                        $('.btnExcel').removeAttr('disabled');
                    }
                }
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
                return row;
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
        $.fn.DataTable.ext.pager.numbers_length = 10;
    };


});