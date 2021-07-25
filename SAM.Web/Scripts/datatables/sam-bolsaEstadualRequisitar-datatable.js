BolsaEstadualRequisitarDataTable = (function () {
    let tableName = '#tableBolsaEstadualRequisitar';
    let actionRequisitarBolsa = '.btnRequisitar';
    let actionExcluir = '.btnExcluir2';
    let actionConsulta = '#spanPesquisa';
    let consultaText = '#search-tables';
    var _form;

    BolsaEstadualRequisitarDataTable.prototype.Load = function (form) {
        _form = form;
        criarTabelaBolsaEstadualRequisitar(form);
        consultarBolsaEstadualRequisitar();
    }
    let consultarBolsaEstadualRequisitar = function () {
        $(actionConsulta).click(function () {
            $(tableName).DataTable().clear();
            $(tableName).DataTable().destroy();
            criarTabelaBolsaEstadualRequisitar(_form);
        });
    };
    //criação
    criarTabelaBolsaEstadualRequisitar = function (form) {
        let csrfToken = $('input[name="__RequestVerificationToken"]', $(form)).val();
        let table = $(tableName).DataTable({
            "bPaginate": true,
            "bLengthChange": true,
            "bFilter": false,
            "bSort": true,
            "bInfo": true,
            "bAutoWidth": false,
            "columnsDefs": [
                { "targets": [9], "searchable": false, "orderable": false, "visible": true }
            ],
            "initComplete": function () {

            },
            "fnUpdate": function () {

            },
            "processing": true,
            "serverSide": true,
            "ajax":
            {
                "url": "IndexJSONResultBolsaEstadualRequisitar",
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
                { "data": "DescricaoUGE" },
                {
                    "defaultContent": `<a class="btnRequisitar" title="Requisitar"><i class="glyphicon glyphicon-check"></i><span class="sr-only">Requisitar</span></a>
                                       <a class ="btnExcluir2" title="Excluir Requisição"><i class ="glyphicon glyphicon-remove"></i>
                                       <span class ="sr-only">Excluir Requisição</span></a>`,
                    "className": "text-center"
                }
            ],
            "drawCallback": function () {
                $('.dataTables_paginate').attr('class', 'pagination-container pagination-datatable');
                $("#search-tables").attr('onKeyPress', 'return sam.search.clickEnterDataTable(this,event)');
            },
            "rowCallback": function (row, data, Object, index) {

                if (!data.Selecionado) {
                    $(actionRequisitarBolsa, row).attr('onclick', "sam.exchange.EventoRequisitar('" + data.Id + "', '" +
                        $('#institution').val() + "', '" + $('#budgetUnit').val() + "', '" + $('#managerUnit').val() + "')");

                    $(actionRequisitarBolsa, row).bind('mouseover', () => {
                        $(actionRequisitarBolsa, row).css('cursor', 'pointer');
                    });

                    $(actionExcluir, row).attr('title', 'Sem permissão para excluir');
                    $(actionExcluir, row).find('i').addClass('icon-desactived');
                } else {
                    $(actionExcluir, row).attr('onclick', "sam.exchange.EventoExcluirRequisicao('" + data.Id + "', '" +
                        $('#institution').val() + "', '" + $('#budgetUnit').val() + "', '" + $('#managerUnit').val() + "')");

                    $(actionExcluir, row).bind('mouseover', () => {
                        $(actionExcluir, row).css('cursor', 'pointer');
                    });

                    $(actionRequisitarBolsa, row).attr('title', 'Sem permissão para requisitar');
                    $(actionRequisitarBolsa, row).find('i').addClass('icon-desactived');
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