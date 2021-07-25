sam.tipoMovimentacaoIntegracao = {
    edit: function () {
        //sam.tipoMovimentacaoIntegracao.init();
        sam.tipoMovimentacaoIntegracao.carregaAcoes();
    },
    create: function () {
        sam.tipoMovimentacaoIntegracao.carregaAcoes();
    },
    carregaAcoes: function () {

        $("#CodigoTipoMovimento_ContabilizaSP").change(function () {
            if (this.value == 9) {
                $("#divTipoMovimentacaoContabiliza").show();
            } else {
                $("#divTipoMovimentacaoContabiliza").hide();
            }
        });

        $("#CodigoEstoque_Origem").change(function () {
            if (this.value == 9) {
                $("#divEstoqueOrigem").show();
            } else {
                $("#divEstoqueOrigem").hide();
            }
        });

        $("#CodigoEstoque_Destino").change(function () {
            if (this.value == 9) {
                $("#divEstoqueDestino").show();
            } else {
                $("#divEstoqueDestino").hide();
            }
        });

        $("#CodigoCE").change(function () {
            if (this.value == 9) {
                $("#divCE").show();
            } else {
                $("#divCE").hide();
            }
        });

        $("#CodigoCE_Entrada").change(function () {
            if (this.value == 9) {
                $("#divCEEntrada").show();
            } else {
                $("#divCEEntrada").hide();
            }
        });

        $("#CodigoCE_Saida").change(function () {
            if (this.value == 9) {
                $("#divCESaida").show();
            } else {
                $("#divCESaida").hide();
            }
        });
    }
}