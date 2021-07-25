sam.itemInventario = {
    selectElementInstitution: '.comboinstitution',
    selectElementBudgetUnit: '.comboBudgetUnit',
    selectElementManagerUnit: '.comboManagerUnit',
    selectElementAdministrativeUnit: '.comboAdministrativeUnit',
    selectElementSection: '.comboSection',
    selectElementResponsible: '.comboResponsible',
    selectElementIncorporacao: '.comboIncorporacao',
    codigoMaterial : "#codigoMaterial",
    Load: function () {
        sam.itemInventario.submitForm();
        sam.itemInventario.setHiddenValues();
        sam.itemInventario.CarregaCheckBox();
        sam.itemInventario.EventoTrocaTipoMovimento();
        sam.itemInventario.BuscaInicialPorCodigoMaterial();
        $('#StateConservationId').val('11836');
    },
    submitForm: function () {
        $("#formBemPatrimonial").submit(function () {
            $('.sam-moeda').val($('.sam-moeda').val().replace('.', ''));

            if (parseFloat($('#ValueAcquisitionModel').val().replace(".", "")) < parseFloat("10,00")) {
                $('span[data-valmsg-for="ValueAcquisitionModel"]').text('Valor não pode ser menor que 10,00!');
                $.unblockUI({ message: $('#modal-loading') });
                return false;
            }
            else
                $("#ValueAcquisitionModel").text('');

            if ($("#ShortDescription").val() == "")
            {
                $('span[data-valmsg-for="ShortDescription"]').text('O Campo Descrição Resumida do Item é obrigatório');
                $.unblockUI({ message: $('#modal-loading') });
                return false;
            }

            // Exibir XML antes de Salvar o BP e enviar o XML para o SIAFEM.
            //if (sam.asset.voltaXml == 0) {

            //    var _head = 'Alerta';
            //    var _foot = '<button style="background-color: #42983c !important; border-color: #42983c !important;" class="btn btn-info" onclick="sam.asset.voltaXml = 1;$(\'#formBemPatrimonial\').submit()"><i class="glyphicon glyphicon-ok" aria-hidden="true"></i> Salvar e Enviar</button> <button class="btn btn-default" onclick="Alert.ok()"><i class="glyphicon glyphicon-share-alt"></i> Voltar</button>';

            //    $.get(sam.path.webroot + "Assets/GerarXMLSiafemDocNLPatrimonial", { _ManagerUnitId: $("#ManagerUnitId").val(), _MovimentDate: $("#MovimentDate").val(), _InstitutionId: $("#InstitutionId").val(), _NumberDoc: $("#NumberDoc").val(), _CPFCNPJ: $("#CPFCNPJ").val(), _MovementTypeId: $("#MovementTypeId").val(), _MaterialItemCode: $("#MaterialItemCode").val(), _Valor: $("#ValueAcquisitionModel").val(), LoginSiafem: $("#LoginSiafem").val(), SenhaSiafem: $("#SenhaSiafem").val() }, function (data) {
            //        if (data[0].Mensagem == undefined) {
            //            sam.asset.xml = data;
            //            Alert.render(sam.asset.xml, _head, _foot);
            //        }
            //        else {
            //            alert(data[0].Mensagem);
            //        }
            //    });

            //    $.unblockUI({ message: $('#modal-loading') });
            //    return false;
            //}
        });
    },
    CarregaCheckBox: function () {
        if ($('#checkComplemento').is(':checked'))
            $('#omite_div_veiculo').css('display', 'block');
        else {
            $('#omite_div_veiculo').css('display', 'none');
            sam.commun.limpar('#omite_div_veiculo');
        }

        $('#checkComplemento').on('ifChanged', function (event) {
            if (this.checked) {
                $('#omite_div_veiculo').css('display', 'block');
                sam.commun.limpar('#omite_div_veiculo');
            }
            else {
                $('#omite_div_veiculo').css('display', 'none');
                sam.commun.limpar('#omite_div_veiculo');
            }

            if ($('#hiddenEmpenho').val() != null)
                $('#Empenho').val($('#hiddenEmpenho').val());
        });

        // Depreciacao
        if ($('#checkDepreciacao').is(':checked')) {
            document.getElementById('LifeCycle').readOnly = false;
            document.getElementById('ResidualValue').readOnly = false;
        }
        else {
            document.getElementById('LifeCycle').readOnly = true;
            document.getElementById('ResidualValue').readOnly = true;
        }

        $('#checkDepreciacao').on('ifChanged', function (event) {
            if (this.checked) {
                $('#textoDepreciacao').html("O Item será depreciado manualmente de acordo com os dados digitados na tela, <b>a alteração desses campos será impossibilitada após a criação do Bem Patrimonial</b>");
                document.getElementById('LifeCycle').readOnly = false;
                document.getElementById('ResidualValue').readOnly = false;
            }
            else {
                if ($("#MaterialGroupCode").val() != '') {
                    $.get(sam.path.webroot + "Assets/RecarregaDadosGrupoMaterial", { MaterialGroupCode: $("#MaterialGroupCode").val() }, function (data) {
                        if (data != null) {
                            $('#LifeCycle').val(data.LifeCycle);
                            $('#LifeCycle').val($('#LifeCycle').val().replace('.', ','));
                            $('#ResidualValue').val(data.ResidualValue);
                            $('#ResidualValue').val($('#ResidualValue').val().replace('.', ','));
                            $('#RateDepreciationMonthly').val(data.RateDepreciationMonthly);
                            $('#RateDepreciationMonthly').val($('#RateDepreciationMonthly').val().replace('.', ','));
                        }
                        else {
                            $('#LifeCycle').val('');
                            $('#ResidualValue').val('');
                            $('#RateDepreciationMonthly').val('');
                        }
                    });
                }
                else {
                    $('#LifeCycle').val('');
                    $('#ResidualValue').val('');
                    $('#RateDepreciationMonthly').val('');
                }
                $('#textoDepreciacao').html("O Item será depreciado automaticamente de acordo com o Grupo do Material, <b>a alteração desses campos será impossibilitada após a criação do Bem Patrimonial</b>");
                document.getElementById('LifeCycle').readOnly = true;
                document.getElementById('ResidualValue').readOnly = true;
            }
        });

        //Quando recarrega a pagina 
        if ($('#checkFlagAcervo').is(':checked')) {
            $('#div_Acervo').css('display', 'block');
            $('#div_InformaItem').css('display', 'block');
            $('#checkFlagTerceiro').iCheck('uncheck');
            $('#checkFlagDecretoSefaz').iCheck('uncheck');
            $("#div_terceiro").css('display', 'none');
            $('#div_Depreciacao').css('display', 'none');
            $('#MaterialItemCode').val('5628156');
            $('#MaterialItemDescription').val('Acervos');
            $('#ShortDescription').val('Acervos');
            $("#msgAnimalAServico").css("display", "none");
        }
        else if ($('#checkFlagTerceiro').is(':checked')) {
            $('#div_Acervo').css('display', 'block');
            $('#div_InformaItem').css('display', 'block');
            $('#checkFlagAcervo').iCheck('uncheck');
            $('#checkFlagDecretoSefaz').iCheck('uncheck');
            $('#div_Depreciacao').css('display', 'none');
            $("#div_terceiro").css('display', 'block');
            $('#MaterialItemCode').val('5628121');
            $('#MaterialItemDescription').val('Bens de Terceiros');
            $('#ShortDescription').val('Bens de Terceiros');
            $("#msgAnimalAServico").css("display", "none");
        } else if ($('#checkFlagDecretoSefaz').is(':checked')) {
            $('#div_Acervo').css('display', 'block');
            $('#div_InformaItem').css('display', 'none');
            $('#div_Acervo').css('display', 'block');
            $('#div_InformaItem').css('display', 'none');

            //parte baixo
            $('#div_Material').css('display', 'block');
            $('#div_Depreciacao').css('display', 'block');
            //document.getElementById('ShortDescription').readOnly = true;

            $('#checkFlagAcervo').iCheck('uncheck');
            $('#checkFlagTerceiro').iCheck('uncheck');
            $("#div_terceiro").css('display', 'none');

            if ($('#MaterialGroupCode').val() == 88) {
                $("#msgAnimalAServico").css("display", "block");
            }
        }
        else {
            $('#div_Acervo').css('display', 'none');
            $('#div_Material').css('display', 'block');
            $('#div_Depreciacao').css('display', 'block');
            $("#div_terceiro").css('display', 'none');
            if ($('#MaterialGroupCode').val() == 88) {
                $("#msgAnimalAServico").css("display", "block");
            }
        }

        //Acervo - Evento de click no check box
        $('#checkFlagAcervo').on('ifChanged', function (event) {
            if (this.checked) {
                $(sam.itemInventario.selectElementIncorporacao).val(movimento.EnumMovementType.IncorpInventarioInicial);
                $("#MovementTypeId").val(movimento.EnumMovementType.IncorpInventarioInicial);
                $('#div_Acervo').css('display', 'block');
                $('#div_InformaItem').css('display', 'block');
                $('#div_Material').css('display', 'none');
                $("#div_terceiro").css('display', 'none');
                sam.commun.limpar('#div_Acervo');
                $('#checkFlagTerceiro').iCheck('uncheck');
                $('#checkFlagDecretoSefaz').iCheck('uncheck');
                $('#div_Depreciacao').css('display', 'none');
                $("#msgAnimalAServico").css("display", "none");
                sam.itemInventario.PreencheItemMaterialParaAcervo();
                sam.materialItem.consultaContaContabilPorTipo(1);
            }
            else if (!$('#checkFlagTerceiro')[0].checked && !$('#checkFlagDecretoSefaz')[0].checked) {
                $('#div_Acervo').css('display', 'none');
                $('#div_Material').css('display', 'block');
                $("#div_terceiro").css('display', 'none');
                sam.commun.limpar('#div_Acervo');
                $('#div_Depreciacao').css('display', 'block ');
                sam.itemInventario.VoltaValores();
                sam.materialItem.consultaContaContabilPorGrupoMaterial($("#MaterialGroupCode").val());
                if ($('#MaterialGroupCode').val() == 88) {
                    $("#msgAnimalAServico").css("display", "block");
                }
            }
        });

        //Terceiro - Evento de click no check box
        $('#checkFlagTerceiro').on('ifChanged', function (event) {
            if (this.checked) {
                $(sam.itemInventario.selectElementIncorporacao).val(movimento.EnumMovementType.IncorpComodatoDeTerceirosRecebidos);
                $("#MovementTypeId").val(movimento.EnumMovementType.IncorpComodatoDeTerceirosRecebidos);
                $('#div_Acervo').css('display', 'block');
                $('#div_Material').css('display', 'none');
                $('#div_InformaItem').css('display', 'block');
                $("#div_terceiro").css('display', 'block');
                sam.commun.limpar('#div_Acervo');
                $('#checkFlagAcervo').iCheck('uncheck');
                $('#checkFlagDecretoSefaz').iCheck('uncheck');
                $('#div_Depreciacao').css('display', 'none');
                $("#msgAnimalAServico").css("display", "none");
                sam.itemInventario.PreencheItemMaterialParaTerceiro();
                sam.materialItem.consultaContaContabilPorTipo(2);
            }
            else if (!$('#checkFlagAcervo')[0].checked && !$('#checkFlagDecretoSefaz')[0].checked) {
                $(sam.itemInventario.selectElementIncorporacao).val(movimento.EnumMovementType.IncorpInventarioInicial);
                $("#MovementTypeId").val(movimento.EnumMovementType.IncorpInventarioInicial);
                $('#div_Acervo').css('display', 'none');
                $("#div_terceiro").css('display', 'none');
                $('#div_Material').css('display', 'block');
                sam.commun.limpar('#div_Acervo');
                $('#div_Depreciacao').css('display', 'block ');
                sam.itemInventario.VoltaValores();
                sam.materialItem.consultaContaContabilPorGrupoMaterial($("#MaterialGroupCode").val());
                if ($('#MaterialGroupCode').val() == 88) {
                    $("#msgAnimalAServico").css("display", "block");
                }
            }
        });

        //Decreto - Evento de click no check box
        $('#checkFlagDecretoSefaz').on('ifChanged', function (event) {
            if (this.checked) {
                $(sam.itemInventario.selectElementIncorporacao).val(movimento.EnumMovementType.IncorpInventarioInicial);
                $("#MovementTypeId").val(movimento.EnumMovementType.IncorpInventarioInicial);

                $('#div_Acervo').css('display', 'block');
                $("#div_terceiro").css('display', 'none');
                $('#div_InformaItem').css('display', 'none');

                //parte baixo
                $("#div_Material").css('display', 'block');
                $('#div_Depreciacao').css('display', 'block');

                //checkboxes
                $('#checkFlagAcervo').iCheck('uncheck');
                $('#checkFlagTerceiro').iCheck('uncheck');

                sam.itemInventario.VoltaValores();
                sam.materialItem.consultaContaContabilPorGrupoMaterial($("#MaterialGroupCode").val());
                if ($('#MaterialGroupCode').val() == 88) {
                    $("#msgAnimalAServico").css("display", "block");
                }
            }
            else if (!$('#checkFlagTerceiro')[0].checked && !$('#checkFlagAcervo')[0].checked) {
                $('#div_Acervo').css('display', 'none');
                $("#div_terceiro").css('display', 'none');
                $('#div_Material').css('display', 'block');
                //document.getElementById('ShortDescription').readOnly = true;
                $('#div_Depreciacao').css('display', 'block ');
                sam.commun.limpar('#div_Acervo');
                sam.itemInventario.VoltaValores();
                sam.materialItem.consultaContaContabilPorGrupoMaterial($("#MaterialGroupCode").val());
                if ($('#MaterialGroupCode').val() == 88) {
                    $("#msgAnimalAServico").css("display", "block");
                }
            }
        });
    },
    EventoTrocaTipoMovimento: function () {
        //Tipo incorporacao

        if ($('#MovementTypeId').val() != "") {
            $('#omite_div_incorporacao').css('display', 'block');

            $('.bloquear').prop('readonly', false);
            $('.esconder').css('display', 'block');
        }
    },
    setHiddenValues: function () {
        $("#InstitutionId").val($(sam.itemInventario.selectElementInstitution).val());
        $("#ManagerId").val($(sam.itemInventario.selectElementManager).val());
        $("#BudgetUnitId").val($(sam.itemInventario.selectElementBudgetUnit).val());
        $("#ManagerUnitId").val($(sam.itemInventario.selectElementManagerUnit).val());
        $("#AdministrativeUnitId").val($(sam.itemInventario.selectElementAdministrativeUnit).val());
        $("#ResponsibleId").val($(sam.itemInventario.selectElementResponsible).val());
        $("#MovementTypeId").val($(sam.itemInventario.selectElementIncorporacao).val());
    },
    BuscaInicialPorCodigoMaterial() {
        if ($("#InventarioId") != undefined
                && ($("#codigoMaterial").val() != ''
                && $("#codigoMaterial").val() != null
                && $("#codigoMaterial").val() != undefined)
                && $('#checkFlagAcervo').is(':checked') == false
                && $('#checkFlagTerceiro').is(':checked') == false
            )
        {
            sam.itemInventario.ConsultaItemMaterialBDeServico($("#MaterialItemCode").val());
        };
    },
    ConsultaItemMaterialServico : function(codigoMaterial)
    {
        $.get(sam.path.webroot + "MaterialItems/GetItemMaterial", { materialItemCode: codigoMaterial }, function (data) {
            try {
                if (data[0].Mensagem == undefined) {
                    var retorno = JSON.parse(data);

                    $("#MaterialGroupNaoCadastrado").val('');
                    $(sam.materialItem.textoPesquisa).val(retorno.Description);
                    $(sam.materialItem.MaterialItemCode).val(retorno.Code);
                    $(sam.materialItem.MaterialGroupCode).val(retorno.MaterialGroupCode);
                    $('#MaterialGroupDescription').val(retorno.MaterialGroupDescription);
                    $('#materialItemPesquisa').val('');
                    $('#LifeCycle').val(retorno.LifeCycle);
                    $('#LifeCycle').val($('#LifeCycle').val().replace('.', ','));
                    $('#RateDepreciationMonthly').val(retorno.RateDepreciationMonthly);
                    $('#RateDepreciationMonthly').val($('#RateDepreciationMonthly').val().replace('.', ','));
                    $('#ResidualValue').val(retorno.ResidualValue);
                    $('#ResidualValue').val($('#ResidualValue').val().replace('.', ','));
                   
                    sam.materialItem.VerificarDescricaoResumida(retorno.Material);
                    if ($("#msgAnimalAServicoInicial").length > 0) {
                        $("#msgAnimalAServicoInicial").remove();
                        sam.materialItem.mostraModalAnimalAServico(retorno.MaterialGroupCode);
                    }

                    if (   $("#MovementTypeId").val() == movimento.EnumMovementType.IncorpMudancaDeCategoriaRevalorizacao
                        || $("#MovementTypeId").val() == movimento.EnumMovementType.IncorpComodatoConcedidoBensMoveis) {
                        if ($('#checkFlagAcervo').iCheck('update')[0].checked == false && $('#checkFlagTerceiro').iCheck('update')[0].checked == false) {
                            sam.materialItem.consultaContaContabilPorGrupoMaterial(retorno.MaterialGroupCode);
                        } else {
                            if ($('#checkFlagAcervo').iCheck('update')[0].checked == true) {
                                sam.materialItem.consultaContaContabilPorTipo(1);
                            } else { 
                                if ($('#checkFlagTerceiro').iCheck('update')[0].checked == true) {
                                    sam.materialItem.consultaContaContabilPorTipo(2);
                                }
                            }
                        }
                    } else {
                        sam.materialItem.consultaContaContabilPorGrupoMaterial(retorno.MaterialGroupCode);
                    }
                }
                else {
                    var retorno2 = (data[0].Mensagem.length < 1);
                    $(sam.materialItem.textoPesquisa).val('');
                    $(sam.materialItem.MaterialItemCode).val('');
                    $(sam.materialItem.MaterialGroupCode).val('');
                    sam.materialItem.mostraModalAnimalAServico(false);
                    sam.materialItem.alteraTextoAnimalServico("");
                    $('#ShortDescriptionItemId').val('');
                    $('#ShortDescription').val('');
                    $('#MaterialGroupDescription').val('');
                    $('#LifeCycle').val('');
                    $('#RateDepreciationMonthly').val('');
                    $('#ResidualValue').val('');
                    $('#modalMaterialGroupNaoCadastrado').modal('show');
                    $('#mensagemmodal').html(data[0].Mensagem);
                    $("#MaterialGroupNaoCadastrado").val('true');
                    $("#btnSalvarSubmit").remove();

                    $("#AuxiliaryAccountId").empty().append('<option class="filter-option pull-left" value> -- Selecione -- </option>');

                    return retorno2;
                }
            }
            catch (e) {
                sam.materialItem.alteraTextoAnimalServico("");
                // Possivelmente o perfil de usuário esta sem permisão de acesso.
                $.unblockUI({ message: $('#modal-loading') });
                $('#modalMaterialGroupNaoCadastrado').modal('show');
                $('#mensagemmodal').html("1: Não foi possivel realizar a pesquisa do Item material, favor contatar o administrador responsável!");
            }
        }).fail(function () {
            sam.materialItem.alteraTextoAnimalServico("");
            $.unblockUI({ message: $('#modal-loading') });
            $('#modalMaterialGroupNaoCadastrado').modal('show');
            $('#mensagemmodal').html("2: Não foi possivel realizar a pesquisa do Item material, favor contatar o administrador responsável!");
        });
    },
    ConsultaItemMaterialBDeServico: function (codigoMaterial) {
        $.get(sam.path.webroot + "MaterialItems/GetItemMaterialBD", { materialItemCode: codigoMaterial }, function (data) {
            try {
                if (data[0].Mensagem == undefined) {
                    var retorno = JSON.parse(data);

                    $("#MaterialGroupNaoCadastrado").val('');
                    $(sam.materialItem.textoPesquisa).val(retorno.Description);
                    $(sam.materialItem.MaterialItemCode).val(retorno.Code);
                    $(sam.materialItem.MaterialGroupCode).val(retorno.MaterialGroupCode);
                    $('#MaterialGroupDescription').val(retorno.MaterialGroupDescription);
                    $('#materialItemPesquisa').val('');
                    sam.materialItem.consultaCamposDepreciacaoDoGrupo(retorno.MaterialGroupCode);

                    if ($("#msgAnimalAServicoInicial").length > 0) {
                        $("#msgAnimalAServicoInicial").remove();
                        sam.materialItem.mostraModalAnimalAServico(retorno.MaterialGroupCode);
                    }

                    if ($('#checkFlagAcervo').iCheck('update')[0].checked == false && $('#checkFlagTerceiro').iCheck('update')[0].checked == false) {
                        sam.materialItem.consultaContaContabilPorGrupoMaterial(retorno.MaterialGroupCode);
                    }

                    //Validação exclusiva para incorporação de item do inventário
                    if ($("#ShortDescription").val() == "") {
                        sam.materialItem.VerificarDescricaoResumida(retorno.Material);
                    }
                }
                else {
                    // Consultar o servico dos itens materiais
                    sam.itemInventario.ConsultaItemMaterialServico(codigoMaterial);
                }
            }
            catch (e) {
                sam.materialItem.alteraTextoAnimalServico("");
                // Possivelmente o perfil de usuário esta sem permisão de acesso.
                $.unblockUI({ message: $('#modal-loading') });
                $('#modalMaterialGroupNaoCadastrado').modal('show');
                $('#mensagemmodal').html("1: Não foi possivel realizar a pesquisa do Item material, favor contatar o administrador responsável!");
            }
        }).fail(function () {
            sam.materialItem.alteraTextoAnimalServico("");
            $.unblockUI({ message: $('#modal-loading') });
            $('#modalMaterialGroupNaoCadastrado').modal('show');
            $('#mensagemmodal').html("2: Não foi possivel realizar a pesquisa do Item material, favor contatar o administrador responsável!");
        });
    },
    PreencheItemMaterialParaAcervo: function () {
        $('#MaterialItemCode').val('5628156');
        $('#MaterialItemDescription').val('Acervos');
        $('#ShortDescription').val('Acervos');
    },
    PreencheItemMaterialParaTerceiro: function () {
        $('#MaterialItemCode').val('5628121');
        $('#MaterialItemDescription').val('Bens de Terceiros');
        $('#ShortDescription').val('Bens de Terceiros');
    },
    VoltaValores: function () {
        $('#MaterialItemCode').val($("#MaterialItemCodeAuxiliar").val());
        $('#MaterialItemDescription').val($("#MaterialItemDescriptionAuxiliar").val());
        $('#ShortDescription').val($("#ShortDescriptionAuxiliar").val());
    }

}