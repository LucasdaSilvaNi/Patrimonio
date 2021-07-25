incorporacaoEmLote = {
    divTabelaNumeroDocumentos: "#divTabelaNumeroDocumentos",
    load: function () {
        this.EventoTrocaTipoDeIncorporacao();
    },
    EscondeElementos: function(){
        $("#tabelaNumeroDocumentos").hide();
    },
    EventoTrocaTipoDeIncorporacao: function () {
        $("#MovementTypeId").change(function (e) {
            let valor = e.target.value;
            if (valor != "0" && valor != "") {
                $(divTabelaNumeroDocumentos).show();
                this.BuscaNumerosDeDocumentos();
            } else {
                $(divTabelaNumeroDocumentos).hide();
            }
        });
    },
    BuscaNumerosDeDocumentos: function () {
        let csrfToken = $('input[name="__RequestVerificationToken"]', $("#formtabela")).val();
        $("#tabelaNumeroDocumentos").DataTable().clear();
        $("#tabelaNumeroDocumentos").DataTable().destroy();
        $("#tabelaNumeroDocumentos").DataTable({
                "bPaginate": true,
                "bLengthChange": false,
                "bFilter": false,
                "bSort": false,
                "bInfo": false,
                "bAutoWidth": false,
                "processing": true,
                "serverSide": true,
                "bMaxLength": 15,
                "ajax": {
                    "url": "IncorporacaoEmLote/BuscaNumerosDeDocumentosPorIncorporacao",
                    "type": "POST",
                    "headers": {
                        "XSRF-TOKEN": csrfToken
                    },
                    "dataType": "JSON",
                    "data": { pesquisa: $("#search-tables").val(), tipoMovimentacao: $("#MovementTypeId").val(), "currentHier": sam.commun.Organiza() }
                },
                "columns": [  
                    { "data": "NumeroDeDocumento" },
                    { "data": "OrigemUGE" },
                    {
                        "data": "NumeroDeDocumento",
                        "render": function (data, type, row) {
                            return "<button class='btn btn-primary botaoNumero' id='"+ data +"'>Incorporar</button>";
                        },
                    }
                ],
                "drawCallback": function () {
                    $('.dataTables_paginate').attr('class', 'pagination-container pagination-datatable');
                },
                "rowCallback": function (row, data, Object, index) {
                    $(".botaoNumero", row).bind('click', () => {
                        this.CarregaBPsDoNumeroDoDocumento(data);
                    });
                },
                "oLanguage": {
                    "sProcessing": $('#modal-loading').show(),
                    "sZeroRecords": "Não foram encontrados resultados",
                    "oPaginate": {
                        "sFirst": "Primeiro",
                        "sPrevious": "Anterior",
                        "sNext": "Próximo",
                        "sLast": "Último"
                    }
                }
            });
    },
    CarregaBPsDoNumeroDoDocumento: function (data) {
        $(divTabelaNumeroDocumentos).hide();
    }
}