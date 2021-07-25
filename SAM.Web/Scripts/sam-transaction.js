sam.transaction = {
    controleTransacaoIndexPadrao: function (_user, _url) {
        var urlTransacao = sam.path.webroot + "Base/ControleTransacoes";
        $.get(urlTransacao, { url: _url }, function (data) {
            if (data.indexOf(1) == -1) {
                $('.btnNovo').removeAttr('href');
                $('.btnNovo').attr({ title: "Sem permissão para incluir" });
                $('.btnNovo').css("background-color", "#ccc")
                $('.btnNovo').css("border-color", "#ccc")
                $('.btnNovo').css("cursor", "default")
                $('.novoclass').removeClass('glyphicon glyphicon-plus-sign');
                $('.novoclass').addClass('glyphicon glyphicon-plus-sign icon-desactived');
            }
            if (data.indexOf(2) == -1) {
                $('.btnEditar').removeAttr('href')
                $('.btnEditar').attr({ title: "Sem permissão para editar" });
                $('.editarclass').removeClass('glyphicon glyphicon-pencil');
                $('.editarclass').addClass('glyphicon glyphicon-pencil icon-desactived');
            }
            if (data.indexOf(3) == -1) {
                $('.btnDetalhe').removeAttr('href')
                $('.btnDetalhe').attr({ title: "Sem permissão para visualizar" });
                $('.detalheclass').removeClass('glyphicon glyphicon-search');
                $('.detalheclass').addClass('glyphicon glyphicon-search icon-desactived');
            }
            if (data.indexOf(7) == -1) {
                if ($('.btnExcel') != null && $('.btnExcel') != undefined) {
                    $('.btnExcel').removeAttr('href');
                    $('.btnExcel').attr({ title: "Sem permissão para Gerar Excel" });
                    $('.btnExcel').css("background-color", "#ccc")
                    $('.btnExcel').css("border-color", "#ccc")
                    $('.btnExcel').css("cursor", "default")
                    $('.Excelclass').removeClass('glyphicon glyphicon-plus-sign');
                    $('.Excelclass').addClass('glyphicon glyphicon-plus-sign icon-desactived');
                }
            }
        });
    },
    controleTransacaoDetailsPadrao: function (_user, _url) {
        var _url = _url.toUpperCase();
        if (_url.indexOf("DETAILS") != -1) {
            var busca = "DETAILS";
            var strbusca = eval('/' + busca + '/g');
            var _url = _url.replace(strbusca, 'EDIT');
        }

        $.get(sam.path.webroot + "/Patrimonio/Base/ControleTransacoes", { user: _user, url: _url }, function (data) {
            if (data.indexOf(2) == -1) {
                $('.btnEditar').removeAttr('href')
                $('.btnEditar').attr({ title: "Sem permissão para editar" });
                $('.btnEditar').css("background-color", "#ccc")
                $('.btnEditar').css("border-color", "#ccc")
                $('.btnEditar').css("cursor", "default")
                $('.editarclass').removeClass('glyphicon glyphicon-pencil');
                $('.editarclass').addClass('glyphicon glyphicon-pencil icon-desactived');
            }
        });
    },
    controleTransacaoIndexMovimento: function (_user, _url) {
        var urlTransacao = sam.path.webroot + "Base/ControleTransacoes";
        $.get(urlTransacao, { user: _user, url: _url }, function (data) {
            if (data.indexOf(1) == -1) {
                $('.btnIncorporar').removeAttr('href');
                $('.btnIncorporar').attr({ title: "Sem permissão para Incorporar" });
                $('.btnIncorporar').css("background-color", "#ccc")
                $('.btnIncorporar').css("border-color", "#ccc")
                $('.btnIncorporar').css("cursor", "default")
                $('.Incorporarclass').removeClass('glyphicon glyphicon-plus-sign');
                $('.Incorporarclass').addClass('glyphicon glyphicon-plus-sign icon-desactived');

                $('.btnMovimento').removeAttr('href');
                $('.btnMovimento').attr({ title: "Sem permissão para Incorporar" });
                $('.btnMovimento').css("background-color", "#ccc")
                $('.btnMovimento').css("border-color", "#ccc")
                $('.btnMovimento').css("cursor", "default")
                $('.Movimentoclass').removeClass('glyphicon glyphicon-plus-sign');
                $('.Movimentoclass').addClass('glyphicon glyphicon-plus-sign icon-desactived');

            }
            if (data.indexOf(2) == -1) {
                $('.btnEditar').removeAttr('href')
                $('.btnEditar').attr({ title: "Sem permissão para Editar" });
                $('.editarclass').removeClass('glyphicon glyphicon-pencil');
                $('.editarclass').addClass('glyphicon glyphicon-pencil icon-desactived');
            }
            if (data.indexOf(5) == -1) {
                $('.btnHistorico').removeAttr('href')
                $('.btnHistorico').removeAttr('onclick')
                $('.btnHistorico').attr({ title: "Sem permissão para Historico" });
                $('.Historicoclass').removeClass('glyphicon glyphicon-remove');
                $('.Historicoclass').addClass('glyphicon glyphicon-remove icon-desactived');
            }
            if (data.indexOf(7) == -1) {
                $('.btnExcel').removeAttr('href');
                $('.btnExcel').attr({ title: "Sem permissão para Gerar Excel" });
                $('.btnExcel').css("background-color", "#ccc")
                $('.btnExcel').css("border-color", "#ccc")
                $('.btnExcel').css("cursor", "default")
                $('.Excelclass').removeClass('glyphicon glyphicon-plus-sign');
                $('.Excelclass').addClass('glyphicon glyphicon-plus-sign icon-desactived');
            }
        });
    },

    controleTransacaoIndexNotaLancamentoPendenteSIAFEM: function (_user, _url) {
        var urlTransacao = sam.path.webroot + "Base/ControleTransacoes";
        $.get(urlTransacao, { user: _user, url: _url }, function (data) {
            if (data.indexOf(6) == -1) {
                $('.btnInclusaoManual').removeAttr('href');
                $('.btnInclusaoManual').attr({ title: "Sem permissão para Incorporar" });
                $('.btnInclusaoManual').css("background-color", "#ccc")
                $('.btnInclusaoManual').css("border-color", "#ccc")
                $('.btnInclusaoManual').css("cursor", "default")
                $('.InclusaoManualclass').removeClass('glyphicon glyphicon-pencil');
                $('.InclusaoManualclass').addClass('glyphicon glyphicon-pencil icon-desactived');

                $('.btnReenvioAutomatico').removeAttr('href');
                $('.btnReenvioAutomatico').attr({ title: "Sem permissão para Incorporar" });
                $('.btnReenvioAutomatico').css("background-color", "#ccc")
                $('.btnReenvioAutomatico').css("border-color", "#ccc")
                $('.btnReenvioAutomatico').css("cursor", "default")
                $('.ReenvioAutomaticolass').removeClass('glyphicon glyphicon-transfer');
                $('.ReenvioAutomaticoclass').addClass('glyphicon glyphicon-transfer icon-desactived');

            }

            //TODO: FUTURO BREVE (DETALHE PENDENCIA NL SIAFEM)
            //if (data.indexOf(2) == -1) {
            //    $('.DetalhePendenciaNotaLancamentoSIAFEM').removeAttr('href')
            //    $('.DetalhePendenciaNotaLancamentoSIAFEM').removeAttr('onclick')
            //    $('.DetalhePendenciaNotaLancamentoSIAFEM').attr({ title: "Sem permissão para Historico" });
            //    $('.Historicoclass').removeClass('glyphicon glyphicon-remove');
            //    $('.Historicoclass').addClass('glyphicon glyphicon-remove icon-desactived');
            //}
        });
    },
}