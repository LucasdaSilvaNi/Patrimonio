var closing = {
    Load: function () {
        closing.EventoChangeUGE();
        closing.ValidaFechamentoOuReabertura();
        //closing.ValidacoesIniciaisDaIntegracao();
    },
    ValidacaoFechamento: function () {
        $('.modalViewGridFechamento').modal('hide');

        var _managerUnitId = $('#ManagerUnitId').val();
        $.get(sam.path.webroot + "/Closings/VerificaIntegracaoSIAFEM", {
            managerUnitId: _managerUnitId
        }, function () { })
        .done(function (result) {
            if (result == true) {
                closing.LoginSiafemFechamento();
            } else {
                closing.EfetuaFechamento();
            }
        }).fail(function () {
            $.unblockUI({ message: $('#modal-loading') });
        });
    },
    EfetuaFechamento: function () {

        $('.modalViewGridFechamento').modal('hide');
        
        if ($('#modal-loading').length > 0) {
            $.blockUI({ message: $('#modal-loading') });
        } else {
            $.blockUI({ message: $('#modal-loading-Fechamento') });
        }
        var _url = sam.path.webroot + "/Closings/EfetuaFechamento";
        var _managerUnitId = $('#ManagerUnitId').val();


        $.post(_url, { managerUnitId: _managerUnitId }, function () {
        }).done(function (result) {
            if ($('#modal-loading').length > 0) {
                $.unblockUI({ message: $('#modal-loading') });
            } else {
                $.unblockUI({ message: $('#modal-loading-Fechamento') });
            }
            console.log(result.data);

            if (result.data === "" || result.data === undefined || result.data === null) {
                if (result.integrado == true) {
                    closing.ConfereDadosSIAFEM(result.numeroUGE, result.mesref, $("#LoginSiafem").val(), $("#SenhaSiafem").val());
                } else {
                    sam.commun.CriarAlertDinamico(result.mensagem, function () {
                        location.reload();
                    });
                }
            } else {
                closing.CriarModalFechamentoDepreciacao(result.data, function () {
                    location.reload();
                });
            }
            //location.reload();

            //if (result.Mensagem != undefined) {
            //    if (result.data == undefined || result.data == null)
            //        return sam.commun.CriarAlertDinamico(result.Mensagem);
            //    else
            //        return closing.CriarModalFechamentoDepreciacao(result.data);
            //}

            //if (result.data == undefined || result.data == null)
            //    if (result.data.Erro === false || result.data.Erro === 'false')
            //        return sam.commun.CriarAlertDinamico(result.Mensagem);
            //    else {
            //        sam.commun.CriarAlertDinamico(result.mensagem, function () {
            //            location.reload();
            //        });
            //    }
            //else {
            //    sam.commun.CriarAlertDinamico(result.mensagem, function () {
            //        location.reload();
            //    });
            //}

        }).fail(function () {
            alert('Erro ao carregar EfetuaFechamento.');
        });
    },
    EfetuaReabertura: function () {

        $('.modalViewGridReabertura').modal('hide');
        if ($('#modal-loading').length > 0) {
            $.blockUI({ message: $('#modal-loading') });
        }
        var _url = sam.path.webroot + "/Closings/EfetuaReabertura";
        var _managerUnitId = $('#ManagerUnitId').val();

        $.post(_url, { managerUnitId: _managerUnitId }, function () {
        }).done(function (data) {
            if ($('#modal-loading').length > 0) {
                $.unblockUI({ message: $('#modal-loading') });
            }
            if (data.integrada == true) {
                closing.ConfereDadosSIAFEMReabertura(data.numeroUGE, data.mesAnterior);
            } else {
                if (data.Mensagem != undefined)
                    return sam.commun.CriarAlertDinamico(data.Mensagem);

                sam.commun.CriarAlertDinamico(data.mensagem, function () {
                    location.reload();
                });
            }

        }).fail(function () {
            alert('Erro ao carregar EfetuaReabertura.');
        });
    },
    ValidacoesIniciaisDaIntegracao: function () {
        if ($(".btn-integracao").length > 0) {
            $(".btn-integracao").remove();
            closing.VerificarAEnviarParaSIAFEM();
        }
    },
    RecuperaMesAnoFechamentoAtual: function (callback) {

        var _url = sam.path.webroot + "/Closings/RecuperaMesAnoFechamentoAtual";
        var _budgetUnitId = $('#BudgetUnitId').val();
        var _managerUnitId = $('#ManagerUnitId').val();

        $.get(_url, {
            budgetUnitId: _budgetUnitId,
            managerUnitId: _managerUnitId
        }, function () {
        }).done(function (data) {

            callback(data.mesAnoReferenciaAtual);

        }).fail(function () {
            alert('Erro ao carregar RecuperaMesAnoFechamentoAtual.');
        });
    },
    RecuperaMesAnoParaReabertura: function (callback) {

        var _url = sam.path.webroot + "/Closings/RecuperaMesAnoParaReabertura";
        var _managerUnitId = $('#ManagerUnitId').val();
        var _budgetUnitId = $('#BudgetUnitId').val();

        $.get(_url, {
            budgetUnitId: _budgetUnitId,
            managerUnitId: _managerUnitId
        }, function () {
        }).done(function (data) {

            callback(data.mesAnoParaReabertura);

        }).fail(function () {
            alert('Erro ao carregar RecuperaMesAnoParaReabertura.');
        });
    },
    MostrarModalConfirmaFechamento: function () {

        if (closing.ValidaDados()) {
            closing.RecuperaMesAnoFechamentoAtual(function (mesAnoReferenciaAtual) {
                $('#mensagemFechamento').text("Pressione OK para confirmar fechamento do mês " + mesAnoReferenciaAtual + ".");
                $('.modalViewGridFechamento').modal('show');
            });
        }
    },

    MostrarModalConfirmaReabertura: function () {

        if (closing.ValidaDados()) {
            closing.RecuperaMesAnoParaReabertura(function (mesAnoParaReabertura) {
                $('#mensagemReabertura').html("Pressione OK para estornar o fechamento do mês " + mesAnoParaReabertura + "<br><br><span style='color:red'>Obs.: Todas as valorizações e desvalorizações realizadas para qualquer Patrimônio desta UGE no mês atual serão perdidas.</span>");
                $('.modalViewGridReabertura').modal('show');
            });
        }
    },

    ValidaDados: function () {

        var valido = true;

        if ($('#InstitutionId option:selected').val() == '' || $('#InstitutionId option:selected').val() == undefined || $('#InstitutionId option:selected').val() == '0') {
            $('#spanInstitution').text('Por favor, informe o Órgão.');
            valido = false;
        }

        if ($('#BudgetUnitId option:selected').val() == '' || $('#BudgetUnitId option:selected').val() == undefined || $('#BudgetUnitId option:selected').val() == '0') {
            $('#spanBudgetUnits').text('Por favor, informe a UO.');
            valido = false;
        }

        if ($('#ManagerUnitId option:selected').val() == '' || $('#ManagerUnitId option:selected').val() == undefined || $('#ManagerUnitId option:selected').val() == '0') {
            $('#spanManagerUnits').text('Por favor, informe a UGE.');
            valido = false;
        }

        return valido;
    },

    EventoChangeUGE: function () {

        $('.comboManagerUnit').change(function () {
            closing.ValidaFechamentoOuReabertura();
        });
    },

    ValidaFechamentoOuReabertura: function () {

        if ($('#modal-loading').length > 0) {
            $.blockUI({ message: $('#modal-loading') });
        }
        var _budgetUnitId = $('#BudgetUnitId').val();
        var _managerUnitId = $('#ManagerUnitId').val();

        if (_budgetUnitId != undefined && _budgetUnitId != '0' &&
            _managerUnitId != undefined && _managerUnitId != '0') {

            $.get(sam.path.webroot + "Closings/ValidaFechamentoOuReabertura", {
                budgetUnitId: _budgetUnitId,
                managerUnitId: _managerUnitId
            }, function () {
            }).done(function (data) {

                if (data.validaFechamento) {
                    $('#btnFechamento').removeAttr('disabled');
                    closing.ConfiguraEventoBotaoFechamento();
                }
                else {
                    $('#btnFechamento').attr('disabled', 'disabled');
                }

                if (data.validaReabertura) {
                    $('#btnReabertura').removeAttr('disabled');
                    closing.ConfiguraEventoBotaoReabertura();
                }
                else {
                    $('#btnReabertura').attr('disabled', 'disabled');
                }

                $('#MesAnoRefUGE').val(data.mesAnoReferenciaAtual);
                if ($('#modal-loading').length > 0) {
                    $.unblockUI({ message: $('#modal-loading') });
                }

                if (data.integradoSIAFEM != undefined && data.integradoSIAFEM == 1)
                {
                    closing.VerificarAEnviarParaSIAFEM();
                }

            }).fail(function () {
                $.unblockUI({ message: $('#modal-loading') });
                alert('Erro na rotina InicializaDatePicker.');
            });
        }
    },
    ConfiguraEventoBotaoFechamento: function() {
        $("#btnFechamento").click(function (e) {
            e.target.disabled = true;
            closing.ValidacaoFechamento();
        });
    },
    ConfiguraEventoBotaoReabertura: function () {
        $("#btnReabertura").click(function (e) {
            e.target.disabled = true;
            closing.MostrarModalConfirmaReabertura();
        });
    },
    CriarModalFechamentoDepreciacao: function (data, callback) {
        $('#modalDepreciacao').empty();
        let builder = [];

        builder.push('<div class="modal-dialog" role="document">');
        builder.push(' <div class="modal-content">');
        builder.push('    <div class="modal-header">');
        builder.push('        <button type="button" class="close fechar-depreciacao" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>');
        builder.push('        <h3 class="modal-title text-center text-warning"></h3>');
        builder.push('        <h4 class="modal-title text-center text-danger">A Prodesp mediante novas regras da Sefaz providenciou o recálculo da depreciação como segue abaixo. Segue para ciência: Por favor clique no botão detalhes para maiores informações. </h4>');
        builder.push('    </div>');
        builder.push('    <div class="modal-body">');
        builder.push('        <table class="table" id="tableFechamentoDepreciacao">');
        builder.push('            <thead>');
        builder.push('                <tr>');
        builder.push('                    <th class="text-info" data-sortable="false">Chapa</th>');
        builder.push('                    <th class="text-info" data-sortable="true">Código do Material</th>');
        builder.push('                    <th class="text-info" data-sortable="true">Data de Aquisição</th>');
        builder.push('                    <th class="text-info" data-sortable="false">Ações</th>');
        builder.push('                </tr>');
        builder.push('            </thead>');
        builder.push('            <tbody>');
        $.each(data, function (key, value) {
            builder.push('                <tr>');
            builder.push('                    <td class="text-info" data-sortable="false">' + value.NumberIdentification + '</td>');
            builder.push('                    <td class="text-info" data-sortable="true">' + value.MaterialItemCode + '</td>');
            builder.push('                    <td class="text-info" data-sortable="true">' + new Date(parseInt(value.AcquisitionDate.replace("/Date(", "").replace(")/", ""), 10)).toLocaleDateString('pt-BR') + '</td>');
            builder.push('                    <td class="text-info" data-sortable="false"><a href="Closings/DetalhesDepreciacao?assetId=' + value.AssetId.toString() + '" class="btn btn-info">Detalhes<a></td>');
            builder.push('                </tr>');
        });
        builder.push('        </table>');
        builder.push('    </div>');
        builder.push('    <div class="modal-footer">');
        builder.push('        <button type="button" class="btn btn-default fechar-depreciacao" data-dismiss="modal">Fechar</button>');
        builder.push('    </div>');
        builder.push('</div>');
        builder.push('</div>');
        if (data[0].MesReferencia != "900")
            $('#MesAnoRefUGE').val(data[0].MesReferencia);


        $('#modalDepreciacao').append(builder.join(""));
        $('#modalDepreciacao').modal('show');

        //value = new Date(parseInt(value.replace("/Date(", "").replace(")/", ""), 10));
    },
    ConfereDadosSIAFEM: function (numeroUge, mes, loginSiafem, senhaSiafem) {
        if ($('#modal-loading').length > 0) {
            $.blockUI({ message: $('#modal-loading') });
        }
        $.post(sam.path.webroot + "/Closings/DadosSIAFEMParaValidar", { uge: numeroUge, mesref: mes }, function (data) {
        }).done(function (data) {
            $('.bodyModalDadosSiafem').html(data);
            $('#modalVerificacaoDadosSIAFEM').modal({ keyboard: false, backdrop: 'static', show: true });
            closing.Prosseguir(numeroUge, mes, loginSiafem, senhaSiafem);
            closing.Abortar(numeroUge, mes);
            if ($('#modal-loading').length > 0) {
                $.unblockUI({ message: $('#modal-loading') });
            }
        }).fail(function () {
            $.post(sam.path.webroot + "/Closings/Abortar", { uge: numeroUge, mesref: mes }, function (data) { })
                .done(function () {
                    alert('Houve um erro, e a operação de Fechamento foi cancelado.Por gentileza, tente novamente mais tarde');
                    location.reload();
                }).fail(function () {
                    alert('Houve um erro. Por favor, contate o suporte e aguarde a respota');
                    location.reload();
                });
        });;

    },
    Abortar: function (numeroUge, mes) {
        if ($('#modal-loading').length > 0) {
            $.blockUI({ message: $('#modal-loading') });
        }
        $('#abortar').click(function (e) {
            e.target.disabled = true;
            $.post(sam.path.webroot + "/Closings/Abortar", { uge: numeroUge, mesref: mes }, function (data) { })
                .done(function () {
                    location.reload();
                }).fail(function () {
                    alert('Houve um erro ao abortar o processo. Página será recarregada');
                    location.reload();
                });
        });
    },
    Prosseguir: function (numeroUge, mes, loginSiafem, senhaSiafem) {
        if ($('#modal-loading').length > 0) {
            $.blockUI({ message: $('#modal-loading') });
        }
        $('#prosseguir').click(function (e) {
            e.target.disabled = true;
            $.post(sam.path.webroot + "/Closings/Prosseguir", { uge: numeroUge, mesref: mes, loginSiafem: loginSiafem, senhaSiafem: senhaSiafem }, function (data) { })
            .done(function (data) {
                $('.bodyModalDadosSiafem').html(data);
                $('.tituloModalSiafem').text("Resultado Final do Fechamento");
                $('#modalVerificacaoDadosSIAFEM').modal({ keyboard: false, backdrop: 'static', show: true });
                $('.ok').click(function () { location.reload(); });
            }).fail(function () {
                alert('Houve algum erro na conexão com o Contabiliza. O fechamento no SAM foi realizado. Por favor, contate o suporte e aguarde a respota');
                location.reload();
            });
        });
    },
    LoginSiafemFechamento: function () {
        $('#SaveLoginSiafem').click(function () {

            if ($('#CPFSIAFEMModal').val().length != 11) {
                alert('Digite os 11 números do CPF');
                return false;
            }

            if ($('#LoginSiafem').val() == "" || $('#SenhaSiafem').val() == "") {
                $('#LoginSiafem').val($('#CPFSIAFEMModal').val());
                $('#SenhaSiafem').val($('#SenhaSIAFEMModal').val());
            }

            if ($('#LoginSiafem').val() == "" || $('#SenhaSiafem').val() == "") {
                return false;
            } else {
                closing.EfetuaFechamento();
            }
        });

        $("#LoginSiafem").val("");
        $("#SenhaSiafem").val("");
        $('#modalLoginSiafem').modal({ keyboard: false, backdrop: 'static', show: true });
    },
    VerificarAEnviarParaSIAFEM: function () {
        var _managerUnitId = $('#ManagerUnitId').val();
        
        if ($('#modal-loading').length > 0) {
            $.blockUI({ message: $('#modal-loading') });
        }
        if (_managerUnitId > 0) {
            $.get(sam.path.webroot + "/Closings/UGEEmFechamento", {
                managerUnitId: _managerUnitId
            }, function () { })
        .done(function (result) {
            if ($('#modal-loading').length > 0) {
                $.unblockUI({ message: $('#modal-loading') });
            }
            if (result.emProcesso == true) {
                closing.ConfereDadosSIAFEMAEnviar(_managerUnitId, result.mesRef)
            }
        }).fail(function () {
            $.unblockUI({ message: $('#modal-loading') });
        });
        }
    },
    ConfereDadosSIAFEMAEnviar: function (numeroUge, mes) {
        if ($('#modal-loading').length > 0) {
            $.blockUI({ message: $('#modal-loading') });
        }
        $.post(sam.path.webroot + "/Closings/DadosSIAFEMParaValidar", { uge: numeroUge, mesref: mes }, function (data) {
        }).done(function (data) {
            $('.bodyModalDadosSiafem').html(data);
            $('#modalVerificacaoDadosSIAFEM').modal({ keyboard: false, backdrop: 'static', show: true });
            closing.ProsseguirAindaAEnviar(numeroUge, mes);
            closing.Abortar(numeroUge, mes);
            if ($('#modal-loading').length > 0) {
                $.unblockUI({ message: $('#modal-loading') });
            }
        }).fail(function () {
            $.post(sam.path.webroot + "/Closings/Abortar", { uge: numeroUge, mesref: mes }, function (data) { })
                .done(function () {
                    alert('Houve um erro, e a operação de Fechamento foi cancelado.Por gentileza, tente novamente mais tarde');
                    location.reload();
                }).fail(function () {
                    alert('Houve um erro. Por favor, contate o suporte e aguarde a respota');
                    location.reload();
                });
        });;
    },
    ProsseguirAindaAEnviar: function (numeroUge, mes) {
        $('#prosseguir').click(function (e) {
            e.target.disabled = true;
            $('#SaveLoginSiafem').click(function () {

                if ($('#CPFSIAFEMModal').val().length != 11) {
                    alert('Digite os 11 números do CPF');
                    return false;
                }

                if ($('#LoginSiafem').val() == "" || $('#SenhaSiafem').val() == "") {
                    $('#LoginSiafem').val($('#CPFSIAFEMModal').val());
                    $('#SenhaSiafem').val($('#SenhaSIAFEMModal').val());
                }

                if ($('#LoginSiafem').val() == "" || $('#SenhaSiafem').val() == "") {
                    return false;
                } else {
                    closing.ProsseguirAEnviar(numeroUge, mes, $('#LoginSiafem').val(), $('#SenhaSiafem').val());
                    return true;
                }
            });

            $("#LoginSiafem").val("");
            $("#SenhaSiafem").val("");
            $('#modalLoginSiafem').modal({ keyboard: false, backdrop: 'static', show: true });
        });
    },
    ProsseguirAEnviar: function (numeroUge, mes, loginSiafem, senhaSiafem) {
        $.post(sam.path.webroot + "/Closings/Prosseguir", { uge: numeroUge, mesref: mes, loginSiafem: loginSiafem, senhaSiafem: senhaSiafem }, function (data) { })
            .done(function (data) {
                $('.bodyModalDadosSiafem').html(data);
                $('.tituloModalSiafem').text("Resultado Final do Fechamento");
                $('#modalVerificacaoDadosSIAFEM').modal({ keyboard: false, backdrop: 'static', show: true });
                $('.ok').click(function () { location.reload(); });
            }).fail(function () {
                alert('Houve algum erro na conexão com o Contabiliza. O fechamento no SAM foi realizado. Por favor, contate o suporte e aguarde a respota');
                location.reload();
            });
    },
    ConfereDadosSIAFEMReabertura: function (numeroUge, mes) {
        if ($('#modal-loading').length > 0) {
            $.blockUI({ message: $('#modal-loading') });
        }
        $.post(sam.path.webroot + "/Closings/DadosSIAFEMParaValidarReabertura", { uge: numeroUge, mesref: mes }, function (data) {
        }).done(function (data) {
            $('.bodyModalDadosSiafem').html(data);
            $('#modalVerificacaoDadosSIAFEM').modal({ keyboard: false, backdrop: 'static', show: true });

            closing.ProsseguirReabertura(numeroUge, mes);
            $('#abortar').click(function () { location.reload(); });

            if ($('#modal-loading').length > 0) {
                $.unblockUI({ message: $('#modal-loading') });
            }
        }).fail(function () {
            alert('Ocorreu um erro inesperado. O mês não foi reaberto. A página será recarregada');
            location.reload();
        });

    },
    ProsseguirReabertura: function (numeroUge, mes) {
        $('#prosseguir').click(function (e) {
            e.target.disabled = true;
            $('#SaveLoginSiafem').click(function () {

                if ($('#CPFSIAFEMModal').val().length != 11) {
                    alert('Digite os 11 números do CPF');
                    return false;
                }

                if ($('#LoginSiafem').val() == "" || $('#SenhaSiafem').val() == "") {
                    $('#LoginSiafem').val($('#CPFSIAFEMModal').val());
                    $('#SenhaSiafem').val($('#SenhaSIAFEMModal').val());
                }

                if ($('#LoginSiafem').val() == "" || $('#SenhaSiafem').val() == "") {
                    return false;
                } else {
                    closing.ProsseguirEstorno(numeroUge, mes, $('#LoginSiafem').val(), $('#SenhaSiafem').val());
                }
            });

            $("#LoginSiafem").val("");
            $("#SenhaSiafem").val("");
            $('#modalLoginSiafem').modal({ keyboard: false, backdrop: 'static', show: true });
        });
    },
    ProsseguirEstorno: function (numeroUge, mes, loginSiafem, senhaSiafem) {
        if ($('#modal-loading').length > 0) {
            $.blockUI({ message: $('#modal-loading') });
        }
            $.post(sam.path.webroot + "/Closings/ProsseguirEstorno", { uge: numeroUge, mesref: mes, loginSiafem: loginSiafem, senhaSiafem: senhaSiafem }, function (data) { })
            .done(function (data) {
                $('.bodyModalDadosSiafem').html(data);
                $('.tituloModalSiafem').text("Resultado Final do Fechamento");
                $('#modalVerificacaoDadosSIAFEM').modal({ keyboard: false, backdrop: 'static', show: true });
                $('.ok').click(function () { location.reload(); });
            }).fail(function () {
                alert('Houve algum erro na conexão com o Contabiliza. O fechamento no SAM foi realizado. Por favor, contate o suporte e aguarde a respota');
                location.reload();
            });
    },
};