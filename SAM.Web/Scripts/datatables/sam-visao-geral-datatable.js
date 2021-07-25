VisaoGeralDataTable = (function () {

    let tableName = '#result-tables';
    let actionHistorico = '.btnHistorico';
    let actionEditar = '.action-editar';
    let actionExcluir = '.action-excluir';
    let actionId = '.asset-id';
    let currentFilter = '.current-filter';
    let cbStatus = '#cbStatus';
    let cbFiltros = '#cbFiltros';
    let actionConsulta = '.action-consulta';
    let consultaText = '#search-tables';
    let numeroRegistros = '#result-tables_length';
    let total = 0;
    let _form;
    VisaoGeralDataTable.prototype.Load = function (form) {
        _form = form;
        criarTableVisaoGeral(form);
    };
    let formatarMoedaBrasil = function (valor) {
        return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(valor);
    };
    let formatarDecimalBrasil = function (valor) {
        if (valor === undefined || valor === null)
            valor = 0.0;

        return new Intl.NumberFormat('pt-BR', { maximumSignificantDigits: 20 }).format(valor.toFixed(2));
    };
    let editarClickHandler = function (asset) {
        window.location = './Assets/Edit/' + asset.Id;
    };
    let historicoClickHandler = function (asset) {
        movimento.AbrirModalHistorico(asset.Id);
    };
    let filtrarStatus = function () {
        $('#cbStatus').change(function () {
            $(tableName).DataTable().clear();
            $(tableName).DataTable().destroy();
            criarTableVisaoGeral(_form);
        });
    };
    let consultarVisaoGeral = function () {
        $(actionConsulta).click(function () {
            $(tableName).DataTable().clear();
            $(tableName).DataTable().destroy();
            criarTableVisaoGeral(_form);
        });
    };
    let verificaNulidade = function (data) {
        if (data === null)
            return "";
        else
            return data;
    };
    let formataDescricaoDivisao = function (data) {
        if (data !== null) {
            let descricaoFormatada = "";
            let tamanho = 0;
            let subpalavra = "";
            data.split(' ').forEach(function (element, index, array) {
                if (element.length < 20) {
                    descricaoFormatada += element + " ";
                } else {
                    tamanho = element.split("/");
                    descricaoFormatada += tamanho[0] + "/<br/>";
                    if (tamanho[1] !== null) { descricaoFormatada += tamanho[1] + " "; } else { descricaoFormatada += " "; }
                }
            });

            return descricaoFormatada;
        } else {
            return data;
        }
    };
    let formataDescricao = function (data) {
        let descricaoFormatadaPelasVirgulas = "";
        let descricaoFormatadaPelosPontos = "";
        let descricaoFormatadaFinal = "";
        let tamanho = 0;
        let tamanhoAux = 0;
        let subpalavra = "";
        data.split(' ').forEach(function (element, index, array) {
            tamanho = element.split(",");

            for (var i = 0; i < tamanho.length; i++) {
                if (i === tamanho.length - 1) {
                    descricaoFormatadaPelasVirgulas += tamanho[i] + " ";
                } else {
                    descricaoFormatadaPelasVirgulas += tamanho[i] + ", " + "<br/>";
                }
            }
        });

        descricaoFormatadaPelosPontos.split(' ').forEach(function (element, index, array) {
            tamanho = descricaoFormatadaPelasVirgulas.split(".");

            for (var i = 0; i < tamanho.length; i++) {
                if (i === tamanho.length - 1) {
                    descricaoFormatadaPelosPontos += tamanho[i];
                } else {
                    descricaoFormatadaPelosPontos += tamanho[i] + "." + "<br/>";
                }
            }
        });

        return descricaoFormatadaPelosPontos;
    };
    let criarTableVisaoGeral = function (form) {
        let csrfToken = $('input[name="__RequestVerificationToken"]', $(form)).val();
        let table = $(tableName).DataTable({
            "bPaginate": true,
            "bLengthChange": true,
            "bFilter": false,
            "bSort": true,
            "bInfo": true,
            "bAutoWidth": false,
            "columnDefs": [
                { "className": "actionButton border-table-td coluna-grid-padding", "targets": [19] },
                { "className": "border-table-td text-center coluna-grid-padding", "targets": [1] },
                { "className": "border-table-td text-center ativo coluna-grid-padding", "targets": [3] },
                { "className": "td-invisivel asset-id coluna-grid-padding", "targets": [0, 15], "searchable": false },
                { "className": "text-datatable-campo-size text-center custom-middle-align coluna-grid-padding", "targets": [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15] },
                { "visible": false, "targets": [15] },
                { "targets": [1], "searchable": false, "orderable": false, "visible": true }

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
                "url": "Movimento/IndexJSONResult",
                "type": "POST",
                "headers": {
                    "XSRF-TOKEN": csrfToken
                },
                "dataType": "JSON",
                "data": {
                    "currentFilter": $(consultaText).val(),
                    "cbStatus": $(cbStatus).val(),
                    "cbFiltros": $(cbFiltros).val(),
                    "currentHier": sam.commun.Organiza()
                }

            },
            "columns": [
                {
                    "defaultContent": '<div class="form-group" style="margin-left: -1%;"><a class="btnHistorico action-historico" href="#" title="Histórico"><i class="glyphicon glyphicon-list-alt Historicoclass" style="font-size: 22px;"></i><span class="sr-only">Histórico</span></a>&nbsp;</div>',
                    "className": "text-center padding-numeroDocumento",
                    "searchable": false, "orderable": false, "visible": true,
                    "render": function (data, type, row) {
                        if ($("#perfilOperador").val() != '1' ) {
                            return `<div class="form-group" style="margin-left: -1%;"><a class="btnHistorico" title="Sem permissão para consultar Historico"><i class="glyphicon-list-alt Historicoclass glyphicon glyphicon-remove icon-desactived" style="font-size: 22px;"></i><span class="sr-only">Histórico</span></a>&nbsp;</div>`;
                        } else {
                            return `<div class="form-group" style="margin-left: -1%;"><a class="btnHistorico action-historico" href="#" title="Histórico"><i class="glyphicon glyphicon-list-alt Historicoclass" style="font-size: 22px;"></i><span class="sr-only">Histórico</span></a>&nbsp;</div>`;
                        }
                    }
                },
                {
                    "data": "SiglaChapa",
                    "render": function (data, type, row) {
                        return row.Sigla + '-' + row.Chapa;
                    },
                    "className": "tamanho-visao-geral-pdrao padding-visao-geral-padrao text-center"
                },
                {
                    "data": "GrupoItem",
                    "render": function (data, type, row) {
                        return row.GrupoItem + '-' + row.Item;
                    },
                    "className": "tamanho-visao-geral-pdrao padding-visao-geral-padrao"
                },
                {
                    "data": "DescricaoDoItem",
                    "render": function (data, type, row) {
                        return formataDescricao(data);
                    },
                    "className": "tamanho-visao-geral-pdrao padding-numeroDocumento"
                },
                {
                    "data": "Orgao",
                    "className": "tamanho-visao-geral-pdrao padding-visao-geral-padrao"
                },
                {
                    "data": "UO",
                    "className": "tamanho-visao-geral-pdrao padding-visao-geral-padrao"
                },
                {
                    "data": "UGE",
                    "className": "tamanho-visao-geral-pdrao padding-visao-geral-padrao"
                },
                {
                    "data": "UA",
                    "className": "tamanho-visao-geral-pdrao padding-visao-geral-padrao"
                },
                {
                    "data": "DescricaoDaDivisao",
                    "render": function (data, type, row) {
                        return formataDescricaoDivisao(data);
                    },
                    "className": "tamanho-visao-geral-pdrao padding-numeroDocumento"
                },
                {
                    "data": "Responsavel",
                    "className": "tamanho-visao-geral-pdrao padding-visao-geral-padrao"
                },
                {
                    "data": "ContaContabil",
                    "className": "tamanho-visao-geral-pdrao padding-visao-geral-padrao text-center"
                },
                {
                    "data": "ValorDeAquisicao",
                    "render": function (data, type, row) {
                        return formatarMoedaBrasil(data);
                    },
                    "className": "tamanho-visao-geral-pdrao padding-visao-geral-padrao"
                },
                {
                    "data": "DepreciacaoAcumulada",
                    "render": function (data, type, row) {
                        return formatarMoedaBrasil(data);
                    },
                    "className": "tamanho-visao-geral-pdrao padding-visao-geral-padrao"
                },
                {
                    "data": "ValorAtual",
                    "render": function (data, type, row) {
                        if (row.LifeCycle === null || row.LifeCycle === '0' || row.LifeCycle === '' || row.LifeCycle === undefined) {
                            return formatarMoedaBrasil(row.ValorDeAquisicao);
                        }
                        return formatarMoedaBrasil(data);
                    },
                    "className": "tamanho-visao-geral-pdrao padding-visao-geral-padrao"
                },
                {
                    "data": "DepreciacaoMensal",
                    "render": function (data, type, row) {
                        return formatarDecimalBrasil(data);
                    },
                    "className": "tamanho-visao-geral-pdrao padding-visao-geral-padrao"
                },
                {
                    "data": "VidaUtil",
                    "className": "tamanho-visao-geral-pdrao padding-visao-geral-padrao",
                    "render": function (data, type, row) {
                        if (data !== null)
                            return data.toString() + '/' + row.LifeCycle.toString();
                        else
                            return '0/' + (row.LifeCycle !== null ? row.LifeCycle.toString() : '0');
                    }
                },
                {
                    "data": "DataAquisicao",
                    "className": "tamanho-visao-geral-pdrao padding-numeroDocumento"
                },
                {
                    "data": "DataIncorporacao",
                    "className": "tamanho-visao-geral-pdrao padding-numeroDocumento"
                },
                {
                    "data": "Empenho",
                    "render": function (data, type, row) {
                        return verificaNulidade(data);
                    },
                    "className": "tamanho-visao-geral-pdrao padding-numeroDocumento"

                },
                {
                    "data": "NumeroDocumento",
                    "render": function (data, type, row) {
                        return verificaNulidade(data);
                    },
                    "className": "tamanho-visao-geral-pdrao padding-numeroDocumento"
                },
                {
                    "data": "DataUltimoHistorico",
                    "render": function (data, type, row) {
                        return row.UltimoHistorico + ' em ' + row.DataUltimoHistorico;
                    },
                    "className": "tamanho-visao-geral-pdrao padding-numeroDocumento"
                },
                {
                    "data": "TipoBp",
                    "render": function (data, type, row) {
                        return verificaNulidade(data);
                    },
                    "className": "tamanho-visao-geral-pdrao padding-numeroDocumento"
                },
                {
                    "className": "tamanho-visao-geral-pdrao padding-visao-geral-padrao text-center",
                    "render": function (data, type, row) {
                        if ($("#perfilOperador").val() != '1') {
                            return `<div class="form-group" style="margin-left: -1%;"><a class="btnEditar" title="Sem permissão para Editar"><i class="editarclass glyphicon glyphicon-pencil icon-desactived"></i><span class="sr-only">Editar</span></a></div>`;
                        } else {
                            return `<div class="form-group default-content-data-table" style="margin-left: -1%;"><a class="action-editar" title="Editar"><i class="glyphicon glyphicon-pencil editarclass"></i><span class="sr-only" aria-hidden="true">Editar</span></a></div>`;
                        }
                    }
                }


            ],
            "aoColumnDefs": [
                {
                    //"mRrender": function (data, type, row) {
                    //    return data;
                    //}, "visible": false, "aTargets": [15] 

                }],
            "initComplete": function () {
            },
            "fnUpdate": function () {

            },
            "finfoCallback": function (oSettings, iStart, iEnd, iMax, iTotal, sPre) {
                $('#infoRefistros').remove();
                $('.dataTables_info').after('<span id="infoRefistros">' + sPre.replace('.', '').replace(',', '') + '</span>');
                total = iTotal;
            },
            "drawCallback": function () {
                var api = this.api();
                $('.dataTables_paginate').attr('class', 'pagination-container pagination-datatable align-baseline');
                $(consultaText).attr('onKeyPress', 'return sam.search.clickEnterDataTable(this,event)');
                $('.historico').removeClass('sorting_asc');
                if (api.rows().count() === 0) {
                    $('.btnMovimento').attr("disabled", "disabled");
                    $('.btnExcel').attr("disabled", "disabled");
                }
            },
            "rowCallback": function (row, data, Object, index) {
                $(row).find('td').each(function () {
                    if (data.Status === true ) {
                        if ((data.EntreOsTiposDeMovimento === true)) {
                            $(this).css("background-color", "#f6b5b5");
                        }

                        if (data.AReclassificar === true)
                        {
                            $(this).css("background-color", "#fcfba2");
                        }
                    }
                });

                $(actionEditar, row).bind('mouseover', () => {
                    $(actionEditar, row).css('cursor', 'pointer');
                });

                if ($("#perfilOperador").val() === '1') {
                    $(actionEditar, row).bind('click', () => {
                        editarClickHandler(data);
                    });
                    $(actionHistorico, row).bind('click', () => {
                        historicoClickHandler(data);
                    });
                }

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
        $.fn.DataTable.ext.pager.numbers_length = 15;
    };
});