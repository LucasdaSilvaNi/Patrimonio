BolsaEstadualDataTable = (function () {
    let tableName = '#tableBolsaEstadual';
    let actionListarRequisicoes = '.btnLista';
    let actionRetirada = '.btnExcluir2';
    let actionConsulta = '#spanPesquisa';
    let consultaText = '#search-tables';
    let retiradaBolsa = "#retiradaBolsa";
    var _form;

    BolsaEstadualDataTable.prototype.Load = function (form) {
        _form = form;
        criarTabelaBolsaEstadual(form);
        consultarBolsaEstadual();
    }
    let consultarBolsaEstadual = function () {
        $(actionConsulta).click(function () {
            $(tableName).DataTable().clear();
            $(tableName).DataTable().destroy();
            criarTabelaBolsaEstadual(_form);
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
            "searchString": "BolsaEstadual"
        };

        window.location = "./MovimentoBolsa?" + $.param(object);

    };

    //criação
    criarTabelaBolsaEstadual = function (form) {
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
                "url": "IndexJSONResultBolsaEstadual",
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
            
                { "data": "Orgao",
                  "className": "text-center"},

                { "data": "Gestor" },
                { "data": "UO" },
                { "data": "UGE" },
                {
                    "defaultContent": `<div class="form-group" style="margin-left: -1%;"><a class="btnLista" title="Listar Requisições">
                                        <i class ="glyphicon glyphicon-list"></i><span class="sr-only">Listar Requisições</span></a>&nbsp;
                                        <a class ="btnExcluir2" title="Retirada da Bolsa"><i class ="glyphicon glyphicon-remove"></i>
                                        <span class ="sr-only">Retirada da Bolsa</span></a></div>`,
                    "className": "text-center"
                }
            ],
            "drawCallback": function () {
                $('.dataTables_paginate').attr('class', 'pagination-container pagination-datatable');
                $("#search-tables").attr('onKeyPress', 'return sam.search.clickEnterDataTable(this,event)');
            },
            "rowCallback": function (row, data, Object, index) {
                $(actionListarRequisicoes, row).attr('onclick', "sam.exchange.AbrirModalBolsa('" + data.Id + "','BolsaEstadual')");

                $(actionListarRequisicoes, row).bind('mouseover', () => {
                    $(actionListarRequisicoes, row).css('cursor', 'pointer');
                });

                $(actionRetirada, row).bind('click', () => {
                    retiradaClickHandler(data);
                });

                $(actionRetirada, row).bind('mouseover', () => {
                    $(actionRetirada, row).css('cursor', 'pointer');
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