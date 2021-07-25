BolsaOrgaoDataTable = (function () {
    let tableName = '#tableBolsaOrgao';
    let actionListarRequisicoes = '.btnLista';
    let actionRetirada = '.btnExcluir2';
    let actionConsulta = '#spanPesquisa';
    let consultaText = '#search-tables';
    let retiradaBolsa = "#retiradaBolsa";
    var _form;
    BolsaOrgaoDataTable.prototype.Load = function (form) {
        _form = form;
        criarTabelaBolsaOrgao(form);
        consultarBolsaOrgao();
    }
    let consultarBolsaOrgao = function () {
        $(actionConsulta).click(function () {
            $(tableName).DataTable().clear();
            $(tableName).DataTable().destroy();
            criarTabelaBolsaOrgao(_form);
        });
    };
    //eventos

    let retiradaClickHandler = function (data) {
        var object = {
            "AssetId": data.Id,
            "tipoMovimento": $(retiradaBolsa).val(),
            "institutionIdDestino": data.InstitutionId,
            "budgetUnitIdDestino": data.BudgetUnitId,
            "managerUnitIdDestino": data.ManagerUnitId,
            "searchString": "BolsaOrgao"
        };

        window.location = "./MovimentoBolsa?" + $.param(object);

    };

    //criação
    criarTabelaBolsaOrgao = function (form) {
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
            "initComplete": function () {

            },
            "fnUpdate": function () {

            },
            "processing": true,
            "serverSide": true,
            "ajax":
            {
                "url": "IndexJSONResultBolsaOrgao",
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
                { "data": "Sigla" },
                { "data": "Chapa" },
                { "data": "Item" },
                { "data": "DescricaoDoItem" },
                {
                    "data": "Orgao",
                    "className": "text-center"
                },
                { "data": "Gestor" },
                { "data": "UO" },
                { "data": "UGE" },
                {
                    "className": "text-center",
                    "render": function (data, type, row) {
                        if ($('#habilitado').val() === '1') {
                            return `<div class="form-group" style="margin-left: -1%;"><a class="btnLista" title="Listar Requisições">
                                        <i class ="glyphicon glyphicon-list"></i><span class="sr-only">Listar Requisições</span></a>&nbsp;
                                        <a class ="btnExcluir2" title="Retirada da Bolsa"><i class ="glyphicon glyphicon-remove"></i>
                                        <span class ="sr-only">Retirada da Bolsa</span></a></div>`;
                        } else {
                            return `<div class="form-group" style="margin-left: -1%;"><a class="btnLista" title="Sem permissão para listar requisições">
                                        <i class ="glyphicon glyphicon-list icon-desactived"></i><span class="sr-only">Listar Requisições</span></a>&nbsp;
                                        <a class ="btnExcluir2" title="sem permissão para retirar da Bolsa"><i class ="glyphicon glyphicon-remove icon-desactived"></i>
                                        <span class ="sr-only">Retirada da Bolsa</span></a></div>`;
                        }
                    }
                }
            ],
            "drawCallback": function () {
                $('.dataTables_paginate').attr('class', 'pagination-container pagination-datatable');
                $("#search-tables").attr('onKeyPress', 'return sam.search.clickEnterDataTable(this,event)');
            },
            "rowCallback": function (row, data, Object, index) {

                if ($('#habilitado').val() === '1') {
                    $(actionListarRequisicoes, row).attr('onclick', "sam.exchange.AbrirModalBolsa('" + data.Id + "','BolsaOrgao')");

                    $(actionListarRequisicoes, row).bind('mouseover', () => {
                        $(actionListarRequisicoes, row).css('cursor', 'pointer');
                    });

                    $(actionRetirada, row).bind('click', () => {
                        retiradaClickHandler(data);
                    });

                    $(actionRetirada, row).bind('mouseover', () => {
                        $(actionRetirada, row).css('cursor', 'pointer');
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