sam.assetpending = {
    modalMaterial: '.ShowModalMaterial',
    CarregaCheckBox: function () {
        if ($('#checkDepreciacao').is(':checked')) {
            $('#omite_div_excluir').css('display', 'block');
            $('#omite_div_tela').css('display', 'none');
        }
        else {
            $('#omite_div_excluir').css('display', 'none');
            $('#MovementTypeId').val('');
            $('#omite_div_tela').css('display', 'block');
        }

        $('#checkDepreciacao').on('ifChanged', function (event) {
            if (this.checked) {
                $('#omite_div_excluir').css('display', 'block');
                $('#omite_div_tela').css('display', 'none');
            }
            else {
                $('#omite_div_excluir').css('display', 'none');
                $('#MovementTypeId').val('');
                $('#omite_div_tela').css('display', 'block');
            }
        });

        // Depreciacao
        $('#MovementTypeId').val('');

        //Quando recarrega a pagina 
        if ($('#checkFlagAcervo').is(':checked')) {
            $('#divAcervo').css('display', 'block');
            $('#checkFlagTerceiro').iCheck('uncheck');
            $('#checkFlagDecretoSefaz').iCheck('uncheck');
            $('#divDepreciacao').css('display', 'none');
            $('#div_GrupoMaterial').css('display', 'none');
            $('#msgAnimalAServico').css('display', 'none');
        }
        else if ($('#checkFlagTerceiro').is(':checked')) {
            $('#divAcervo').css('display', 'block');
            $('#checkFlagAcervo').iCheck('uncheck');
            $('#checkFlagDecretoSefaz').iCheck('uncheck');
            $('#divDepreciacao').css('display', 'none');
            $('#div_GrupoMaterial').css('display', 'none');
            $('#msgAnimalAServico').css('display', 'none');
        }
        else if ($('#checkFlagDecretoSefaz').is(':checked')) {
            $('#divAcervo').css('display', 'block');
            $('#checkFlagAcervo').iCheck('uncheck');
            $('#checkFlagTerceiro').iCheck('uncheck');
            $('#divDepreciacao').css('display', 'block');
            $('#div_GrupoMaterial').css('display', 'block');
        }
        else {
            $('#divAcervo').css('display', 'none');
            $('#divDepreciacao').css('display', 'block');
            $('#div_GrupoMaterial').css('display', 'block');
        }

        //Decreto SEFAZ - Evento de click no check box
        $('#checkFlagDecretoSefaz').on('ifChanged', function (event) {
            if (this.checked) {
                $('#divAcervo').css('display', 'block');
                $('#div_pesquisaItemMaterial').css('display', 'block');
                $('span[data-valmsg-for="ShortDescription"]').text('');
                $('#ShortDescription').val($('#ShortDescriptionAuxiliar').val());
                sam.commun.limpar('#div_Acervo');
                $('#checkFlagTerceiro').iCheck('uncheck');
                $('#checkFlagAcervo').iCheck('uncheck');
                $('#divDepreciacao').css('display', 'block');
                $('#div_GrupoMaterial').css('display', 'block');

                if (sessionStorage.getItem('valoresAnteriores') != null) {
                    let objeto = JSON.parse(sessionStorage.getItem('valoresAnteriores'));
                    $('#MaterialItemCode').val(objeto.MaterialItemCode);
                    $('#MaterialItemDescription').val(objeto.MaterialItemDescription);
                    $('#ShortDescription').val(objeto.ShortDescription);
                    $('#MaterialGroupCode').val(objeto.MaterialGroupCode);
                    $('#MaterialGroupDescription').val(objeto.MaterialGroupDescription);
                    sessionStorage.setItem('valoresAnteriores', null)
                } else {
                    $('#MaterialItemCode').val('');
                    $('#MaterialItemDescription').val('');
                    $('#ShortDescription').val('');
                    $('#MaterialGroupCode').val('');
                    $('#MaterialGroupDescription').val('');
                }

                if ($('#MaterialGroupCode').val() != null
                    && $('#MaterialGroupCode').val() != undefined
                    && $('#MaterialGroupCode').val() != '') {
                    sam.materialItem.consultaContaContabilPorGrupoMaterial($('#MaterialGroupCode').val());

                    if($('#MaterialGroupCode').val() == 88){
                        $("#msgAnimalAServico").css("display", "block");
                    }
                } else {
                    $("#AuxiliaryAccountId").empty().append('<option class="filter-option pull-left" value> -- Selecione -- </option>');
                }
            }
            else if (!$('#checkFlagTerceiro')[0].checked && !$('#checkFlagAcervo')[0].checked) {
                $('#divAcervo').css('display', 'none');
                //document.getElementById('ShortDescription').readOnly = true;
                $('#ShortDescription').val($('#ShortDescriptionAuxiliar').val());
                $('span[data-valmsg-for="ShortDescription"]').text('');
                sam.commun.limpar('#div_Acervo');
                $('#divDepreciacao').css('display', 'block');
                $('#div_GrupoMaterial').css('display', 'block');
                if ($('#MaterialGroupCode').val() == 88) {
                    $("#msgAnimalAServico").css("display", "block");
                }
            }
        });

        //Acervo - Evento de click no check box
        $('#checkFlagAcervo').on('ifChanged', function (event) {
            if (this.checked) {
                $('#divAcervo').css('display', 'block');
                $('#div_pesquisaItemMaterial').css('display', 'none');
                sam.commun.limpar('#div_Acervo');
                $('#checkFlagDecretoSefaz').iCheck('uncheck');
                $('#checkFlagTerceiro').iCheck('uncheck');
                $('#divDepreciacao').css('display', 'none');
                $("#msgAnimalAServico").css("display", "none");
                $('#div_GrupoMaterial').css('display', 'none');
                sam.assetpending.PreencheItemMaterialParaAcervo();
                sam.materialItem.consultaContaContabilPorTipo(1);
                //sam.asset.CamposDepreciacao(0);
                //sam.asset.CamposItemMaterial(0);
            }
            else if (!$('#checkFlagTerceiro')[0].checked && !$('#checkFlagDecretoSefaz')[0].checked) {
                $('#divAcervo').css('display', 'none');
                $('#div_pesquisaItemMaterial').css('display', 'block');
                $('#ShortDescription').val($('#ShortDescriptionAuxiliar').val());
                $('span[data-valmsg-for="ShortDescription"]').text('');
                sam.commun.limpar('#div_Acervo');
                $('#divDepreciacao').css('display', 'block ');
                $('#div_GrupoMaterial').css('display', 'block');
                sam.assetpending.RetornaValores();

                if ($('#MaterialGroupCode').val() != null
                    && $('#MaterialGroupCode').val() != undefined
                    && $('#MaterialGroupCode').val() != '') {
                    sam.materialItem.consultaContaContabilPorGrupoMaterial($('#MaterialGroupCode').val());
                    
                    if ($('#MaterialGroupCode').val() == 88) {
                        $("#msgAnimalAServico").css("display", "block");
                    }
                } else {
                    $("#AuxiliaryAccountId").empty().append('<option class="filter-option pull-left" value> -- Selecione -- </option>');
                }
            }
        });

        //Terceiro - Evento de click no check box
        $('#checkFlagTerceiro').on('ifChanged', function (event) {
            if (this.checked) {
                $('#divAcervo').css('display', 'block');
                $('#div_pesquisaItemMaterial').css('display', 'none');
                sam.commun.limpar('#div_Acervo');
                $('#checkFlagDecretoSefaz').iCheck('uncheck');
                $('#checkFlagAcervo').iCheck('uncheck');
                $('#divDepreciacao').css('display', 'none');
                $("#msgAnimalAServico").css("display", "none");
                $('#div_GrupoMaterial').css('display', 'none');
                sam.assetpending.PreencheItemMaterialParaTerceiro();
                sam.materialItem.consultaContaContabilPorTipo(2);
            }
            else if (!$('#checkFlagAcervo')[0].checked && !$('#checkFlagDecretoSefaz')[0].checked) {
                $('#divAcervo').css('display', 'none');
                $('#div_pesquisaItemMaterial').css('display', 'block');
                $('#ShortDescription').val($('#ShortDescriptionAuxiliar').val());
                $('span[data-valmsg-for="ShortDescription"]').text('');
                sam.commun.limpar('#div_Acervo');
                $('#divDepreciacao').css('display', 'block ');
                $('#div_GrupoMaterial').css('display', 'block');
                sam.assetpending.RetornaValores();

                if ($('#MaterialGroupCode').val() != null
                    && $('#MaterialGroupCode').val() != undefined
                    && $('#MaterialGroupCode').val() != '') {
                    sam.materialItem.consultaContaContabilPorGrupoMaterial($('#MaterialGroupCode').val());
                    if ($('#MaterialGroupCode').val() == 88) {
                        $("#msgAnimalAServico").css("display", "block");
                    }
                } else {
                    $("#AuxiliaryAccountId").empty().append('<option class="filter-option pull-left" value> -- Selecione -- </option>');
                }
            }
        });

        //-------------------------------------------------------------
    },
    CarregaCheckBoxIntegracao: function () {
        //Quando recarrega a pagina 
        if ($('#checkFlagAcervo').is(':checked')) {
            $('#divAcervo').css('display', 'block');
            $('#div_pesquisaItemMaterial').css('display', 'none');
            $('#div_GrupoMaterial').css('display', 'none');
            $('#div_ItemMaterialVindoDoEstoque').css('display', 'none');
            $('#div_ItemMaterialAcervo').css('display', 'block');
            $("#msgAnimalAServico").css("display", "none");

            if ($('#ShortDescription').val() != "" && $('#ShortDescription').val() != null)
                $('span[data-valmsg-for="ShortDescription"]').text('');
            $('#divDepreciacao').css('display', 'none');
        } else {
            $('#divAcervo').css('display', 'none');
            $('#divDepreciacao').css('display', 'block');
            $('#div_pesquisaItemMaterial').css('display', 'block');
            $('#div_GrupoMaterial').css('display', 'block');
            $('#div_ItemMaterialVindoDoEstoque').css('display', 'block');
            $('#div_ItemMaterialAcervo').css('display', 'none');
            if ($('#MaterialGroupCode').val() == 88) {
                $("#msgAnimalAServico").css("display", "block");
            }
        }

        //Acervo - Evento de click no check box
        $('#checkFlagAcervo').on('ifChanged', function (event) {
            if (this.checked) {
                $('#divAcervo').css('display', 'block');
                sam.commun.limpar('#div_Acervo');
                $('#divDepreciacao').css('display', 'none');
                $("#msgAnimalAServico").css("display", "none");
                $('#div_pesquisaItemMaterial').css('display', 'none');
                $('#div_GrupoMaterial').css('display', 'none');
                $('#div_ItemMaterialVindoDoEstoque').css('display', 'none');
                $('#div_ItemMaterialAcervo').css('display', 'block');
                $('#ShortDescription').val('Acervos');
                sam.materialItem.consultaContaContabilPorTipo(1);
            }
            else {
                $('#divAcervo').css('display', 'none');
                $('#ShortDescription').val($('#ShortDescriptionAuxiliar').val());
                $('span[data-valmsg-for="ShortDescription"]').text('');
                sam.commun.limpar('#div_Acervo');
                $('#div_pesquisaItemMaterial').css('display', 'block');
                $('#div_GrupoMaterial').css('display', 'block');
                $('#divDepreciacao').css('display', 'block ');
                $('#div_ItemMaterialVindoDoEstoque').css('display', 'block');
                $('#div_ItemMaterialAcervo').css('display', 'none');

                if ($('#MaterialGroupCode').val() != null
                    && $('#MaterialGroupCode').val() != undefined
                    && $('#MaterialGroupCode').val() != '') {
                    sam.materialItem.consultaContaContabilPorGrupoMaterial($('#MaterialGroupCode').val());

                    if ($('#MaterialGroupCode').val() == 88) {
                        $("#msgAnimalAServico").css("display", "block");
                    }
                } else {
                    $("#AuxiliaryAccountId").empty().append('<option class="filter-option pull-left" value> -- Selecione -- </option>');
                }
            }
        });

        //-------------------------------------------------------------
    },

    Load: function () {
        $('#ShortDescriptionAuxiliar').val($('#ShortDescription').val());
        sam.assetpending.submitForm();
        if ($("#assetIdsIntegracao").val() == "")
            sam.assetpending.CarregaCheckBox();
        else
            sam.assetpending.CarregaCheckBoxIntegracao();

        if ($("#msgAnimalAServicoInicial").length > 0) {
            $("#msgAnimalAServicoInicial").remove();
            sam.materialItem.mostraModalAnimalAServico($("#MaterialGroupCode").val());
        }

        $(document).ready(function () {
            $(window).load(function () {

                sam.assetpending.consultaClick();
                sam.assetpending.CriarDivMaterialGroupNaoCadastrado();

                $("#btnConsultar").click(function () {
                    if ($("#materialItemPesquisa").val() != "") {
                        $.blockUI({ message: $('#modal-loading') });

                        // Consultar tabela item_siafisico do banco antes de consultar o servico dos itens materiais
                        $.get(sam.path.webroot + "MaterialItems/GetItemMaterialBD", { materialItemCode: $("#materialItemPesquisa").val() }, function (data) {
                            try {
                                if (data[0].Mensagem == undefined) {
                                    var retorno = JSON.parse(data);

                                    sam.assetpending.VerificarDescricaoResumidaMaterial($("#MaterialItemDescription").val());

                                    $("#MaterialGroupNaoCadastrado").val('');
                                    $("#MaterialItemDescription").val(retorno.Description);
                                    $("#MaterialItemCode").val(retorno.Code);
                                    $("#MaterialGroupCode").val(retorno.MaterialGroupCode);
                                    $("#MaterialGroupDescription").val(retorno.MaterialGroupDescription);
                                    $('#materialItemPesquisa').val('');
                                    sam.materialItem.consultaCamposDepreciacaoDoGrupo(retorno.MaterialGroupCode);
                                    sam.materialItem.mostraModalAnimalAServico(retorno.MaterialGroupCode);
                                    if (retorno.MaterialGroupCode != 88) {
                                        sam.materialItem.alteraTextoAnimalServico("");
                                    }

                                    let terceiroNaoChecado = true;
                                    if ($('#checkFlagTerceiro').length > 0) {
                                        terceiroNaoChecado = ($('#checkFlagTerceiro').iCheck('update')[0].checked == false);
                                    }

                                    if ($('#checkFlagAcervo').iCheck('update')[0].checked == false && terceiroNaoChecado) {
                                        sam.materialItem.consultaContaContabilPorGrupoMaterial(retorno.MaterialGroupCode);
                                    }

                                    $.unblockUI({ message: $('#modal-loading') });
                                }
                                else {
                                    // Consultar o servico dos itens materiais
                                    $.get(sam.path.webroot + "MaterialItems/GetItemMaterial", { materialItemCode: $("#materialItemPesquisa").val() }, function (data) {
                                        try {
                                            if (data[0].Mensagem == undefined) {
                                                var retorno = JSON.parse(data);

                                                sam.assetpending.VerificarDescricaoResumidaMaterial($("#MaterialItemDescription").val());

                                                $("#MaterialGroupNaoCadastrado").val('');
                                                $("#MaterialItemDescription").val(retorno.Description);
                                                $("#MaterialItemCode").val(retorno.Code);
                                                $("#MaterialGroupCode").val(retorno.MaterialGroupCode);
                                                $("#MaterialGroupDescription").val(retorno.MaterialGroupDescription);
                                                $('#materialItemPesquisa').val('');
                                                sam.materialItem.consultaCamposDepreciacaoDoGrupo(retorno.MaterialGroupCode);
                                                sam.materialItem.mostraModalAnimalAServico(retorno.MaterialGroupCode);
                                                if (retorno.MaterialGroupCode != 88) {
                                                    sam.materialItem.alteraTextoAnimalServico("");
                                                }

                                                let terceiroNaoChecado = true;
                                                if ($('#checkFlagTerceiro').length > 0) {
                                                    terceiroNaoChecado = ($('#checkFlagTerceiro').iCheck('update')[0].checked == false);
                                                }

                                                if ($('#checkFlagAcervo').iCheck('update')[0].checked == false && terceiroNaoChecado) {
                                                    sam.materialItem.consultaContaContabilPorGrupoMaterial(retorno.MaterialGroupCode);
                                                }

                                                $.unblockUI({ message: $('#modal-loading') });
                                            }
                                            else {
                                                var retorno2 = (data[0].Mensagem.length < 1);
                                                //$("#MaterialItemDescription").val('');
                                                //$("#MaterialItemCode").val('');
                                                //$("#MaterialGroupCode").val('');
                                                //$("#MaterialGroupDescription").val('');
                                                //$('#ShortDescription').val('');
                                                //$('#LifeCycle').val('');
                                                //$('#RateDepreciationMonthly').val('');
                                                //$('#ResidualValue').val('');
                                                sam.materialItem.mostraModalAnimalAServico(false);
                                                sam.materialItem.alteraTextoAnimalServico("");
                                                $('#modalMaterialGroupNaoCadastrado').modal('show');
                                                $('#mensagemmodal').html(data[0].Mensagem);
                                                $("#MaterialGroupNaoCadastrado").val('true');
                                                $.unblockUI({ message: $('#modal-loading') });
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
                    }
                    else {
                        sam.materialItem.alteraTextoAnimalServico("");
                        $('#modalMaterialGroupNaoCadastrado').modal('show');
                        $('#mensagemmodal').html("Favor informar o código do Item Material para realizar a pesquisa!");
                    }
                });
            });
        });
    },
    VerificarDescricaoResumidaMaterial: function (_material) {
        $.get(sam.path.webroot + "ShortDescriptionItem/VerificarDescricaoResumida", { Material: _material }, function (data) {
            var retorno = JSON.parse(data);
            $('#ShortDescriptionItemId').val(retorno.Id);
            $('#ShortDescription').val(retorno.Description);
            $('#ShortDescriptionAuxiliar').val(retorno.Description);
        });
    },
    CriarDivMaterialGroupNaoCadastrado: function () {
        var htmlBuilder = [];

        htmlBuilder.push('<div class="modal fade modalView" id="modalMaterialGroupNaoCadastrado" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel">');
        htmlBuilder.push('<div class="modal-dialog" role="document">');
        htmlBuilder.push('    <div class="modal-content">');
        htmlBuilder.push('        <div class="modal-header">');
        htmlBuilder.push('            <button type="button" id="closemodalMaterialGroupNaoCadastrado" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>');
        htmlBuilder.push('            <h4 class="modal-title" id="exampleModalLabel">Usuario</h4>');
        htmlBuilder.push('        </div>');
        htmlBuilder.push('        <div class="modal-body">');
        htmlBuilder.push('                    <div class="form-group">');
        htmlBuilder.push('                <label for="recipient-name" id="mensagemmodal" class="control-label"></label>');
        htmlBuilder.push('            </div>');
        htmlBuilder.push('        </div>');
        htmlBuilder.push('        <div class="modal-footer">');
        htmlBuilder.push('            <button type="button" id="fecharModalGroupNaoCadastrado" class="btn btn-primary buttonClose" data-dismiss="modal">Fechar</button>');
        htmlBuilder.push('        </div>');
        htmlBuilder.push('    </div>');
        htmlBuilder.push('</div>');
        htmlBuilder.push('</div>');
        htmlBuilder.join("")

        $(document.body).append(htmlBuilder.join(""));
    },
    consultaClick: function () {
        $(sam.assetpending.modalMaterial).click(function () {
            sam.materialItem.unloadModalView();
            sam.materialItem.removeModaView();
            sam.materialItem.itemMaterialShow();
        });
    },
    submitForm: function () {
        $("#formBemPatrimonialPendente").submit(function () {
            $.blockUI({ message: $('#modal-loading') });
            if ($('#checkDepreciacao').is(':checked')) {
                if ($('#MovementTypeId').val() == "") {
                    $('span[data-valmsg-for="MovementTypeId"]').text('Selecione um Tipo de Baixa!');
                    $.unblockUI({ message: $('#modal-loading') });
                    return false;
                }
            }
        });
    },
    PreencheItemMaterialParaAcervo: function () {
        sam.assetpending.GuardaValores();
        $('#MaterialItemCode').val('5628156');
        $('#MaterialItemDescription').val('Acervos');
        $('#ShortDescription').val('Acervos');
    },
    PreencheItemMaterialParaTerceiro: function () {
        sam.assetpending.GuardaValores();
        $('#MaterialItemCode').val('5628121');
        $('#MaterialItemDescription').val('Bens de Terceiros');
        $('#ShortDescription').val('Bens de Terceiros');
    },
    GuardaValores: function () {
        if (sessionStorage.getItem('valoresAnteriores') == null ||
            sessionStorage.getItem('valoresAnteriores') == 'null'
        ) {
            $("#materialItemPesquisa").val($('#MaterialItemCode').val());
            sessionStorage.setItem('valoresAnteriores', JSON.stringify({
                    MaterialItemCode: $('#MaterialItemCode').val(),
                    MaterialItemDescription: $('#MaterialItemDescription').val(),
                    ShortDescription: $('#ShortDescription').val(),
                    MaterialGroupCode: $('#MaterialGroupCode').val(),
                    MaterialGroupDescription: $('#MaterialGroupDescription').val()
                })
            );
        }
    },
    RetornaValores: function () {
        if (sessionStorage.getItem('valoresAnteriores')) {
            let objeto = JSON.parse(sessionStorage.getItem('valoresAnteriores'));
            $('#MaterialItemCode').val(objeto.MaterialItemCode);
            $('#MaterialItemDescription').val(objeto.MaterialItemDescription);
            $('#ShortDescription').val(objeto.ShortDescription);
            $('#MaterialGroupCode').val(objeto.MaterialGroupCode);
            $('#MaterialGroupDescription').val(objeto.MaterialGroupDescription);
            sessionStorage.removeItem('valoresAnteriores');
        } else {
            $('#MaterialItemCode').val('');
            $('#MaterialItemDescription').val('');
            $('#ShortDescription').val('');
            $('#MaterialGroupCode').val('');
            $('#MaterialGroupDescription').val('');
        }
    }
}