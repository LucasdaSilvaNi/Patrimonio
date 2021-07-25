sam.materialItem = {
    paginacao: '.pagination li a',
    consulta: '.gridConsulta',
    divGrid: '.divGrid',
    buttonConsulta: '.btnConsulta',
    divView: '.divView',
    modalView: '.modalView',
    modalMaterial: '.ShowModalMaterial',
    materialItemId: '.materialItemId',
    materialItemDescricao: '.materialItemDescricao',
    hrefMaterialConsulta: '.tr-material-consulta',
    materialConsultaId: '.material-consulta-id',
    materialConsultaDescricao: '.material-consulta-descricao',
    paramItemMaterialId: 'itemMaterialId',
    LifeCycle: '.lifeCycle',
    paramLifeCycle: 'LifeCycle',
    RateDepreciationMonthly: '.rateDepreciationMonthly',
    paramRateDepreciationMonthly: 'RateDepreciationMonthly',
    ResidualValue: '.residualValue',
    paramResidualValue: 'ResidualValue',
    textoPesquisa: '#MaterialItemDescription',
    MaterialItemCode: '#MaterialItemCode',
    MaterialGroupCode: '#MaterialGroupCode',
    Empenho: '#Empenho',
    init: function () {
        $('[data-toggle="tooltip"]').tooltip();
    },
    Load: function () {
        sam.materialItem.consultaClick();
        sam.materialItem.CriarDivMaterialGroupNaoCadastrado();
        //sam.materialItem.CriarDivLoginSiafem();
        sam.materialItem.BuscarItemMaterial();
    },
    BuscarItemMaterial: function () {
        $("#btnConsultar").click(function () {

            if ($("#materialItemPesquisa").val() != "") {
                //$.blockUI({ message: $('#modal-loading') });

                // Consultar tabela item_siafisico do BD antes de consultar o servico - 5: Inventario Inicial
				// OBS: Rotina alterada, pois no ambiente de HML o Servico nao esta funcionando
                if ($("#MovementTypeId").val() == movimento.EnumMovementType.IncorpInventarioInicial ||
                    $("#MovementTypeId").val() == movimento.EnumMovementType.IncorpComodatoDeTerceirosRecebidos)
                {
                    sam.materialItem.ConsultaItemMaterialBDeServico();
                    //$.unblockUI({ message: $('#modal-loading') });
                }
                else
                {
                    sam.materialItem.ConsultaItemMaterialServico();
                    //$.unblockUI({ message: $('#modal-loading') });
                }
            }
            else {
                $('#modalMaterialGroupNaoCadastrado').modal('show');
                $('#mensagemmodal').html("Favor informar o código do Item Material para realizar a pesquisa!");
            }
        });
    },

    ConsultaItemMaterialServico : function()
    {
        $.blockUI({ message: $('#modal-loading') });
        $.get(sam.path.webroot + "MaterialItems/GetItemMaterial", { materialItemCode: $("#materialItemPesquisa").val() }, function (data) {
            try {
                $.unblockUI({ message: $('#modal-loading') });
                if (data[0].Mensagem == undefined) {
                    var retorno = JSON.parse(data);

                    $("#MaterialGroupNaoCadastrado").val('');
                    $(sam.materialItem.textoPesquisa).val(retorno.Description);
                    $(sam.materialItem.MaterialItemCode).val(retorno.Code);
                    $(sam.materialItem.MaterialGroupCode).val(retorno.MaterialGroupCode);
                    $('#MaterialGroupDescription').val(retorno.MaterialGroupDescription);
                    sam.materialItem.consultaCamposDepreciacaoDoGrupo(retorno.MaterialGroupCode);
                    sam.materialItem.mostraModalAnimalAServico(retorno.MaterialGroupCode);
                    if (retorno.MaterialGroupCode != 88) {
                        sam.materialItem.alteraTextoAnimalServico("");
                    }

                    $('#materialItemPesquisa').val('');
                   
                    sam.materialItem.VerificarDescricaoResumida(retorno.Material);
                    if (   $("#MovementTypeId").val() == movimento.EnumMovementType.IncorpMudancaDeCategoriaRevalorizacao ) {
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
                    } else if ($("#MovementTypeId").val() == movimento.EnumMovementType.IncorpComodatoDeTerceirosRecebidos) {
                        sam.materialItem.consultaContaContabilPorTipo(2);
                    }
                    else
                    {
                        sam.materialItem.consultaContaContabilPorGrupoMaterial(retorno.MaterialGroupCode);
                    }
                }
                else {
                    var retorno2 = (data[0].Mensagem.length < 1);
                    $(sam.materialItem.textoPesquisa).val('');
                    $(sam.materialItem.MaterialItemCode).val('');
                    $(sam.materialItem.MaterialGroupCode).val('');
                    $('#ShortDescriptionItemId').val('');
                    $('#ShortDescription').val('');
                    $('#MaterialGroupDescription').val('');
                    $('#LifeCycle').val('');
                    $('#RateDepreciationMonthly').val('');
                    $('#ResidualValue').val('');
                    $('#modalMaterialGroupNaoCadastrado').modal('show');
                    $('#mensagemmodal').html(data[0].Mensagem);
                    $("#MaterialGroupNaoCadastrado").val('true');
                    sam.materialItem.mostraModalAnimalAServico(false);
                    sam.materialItem.alteraTextoAnimalServico("");
                    if ($("#MovementTypeId").val() != movimento.EnumMovementType.IncorpComodatoDeTerceirosRecebidos) {
                        $("#AuxiliaryAccountId").empty().append('<option class="filter-option pull-left" value> -- Selecione -- </option>');
                    }

                    //$.unblockUI({ message: $('#modal-loading') });
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

    ConsultaItemMaterialBDeServico : function()
    {
        $.blockUI({ message: $('#modal-loading') });
        $.get(sam.path.webroot + "MaterialItems/GetItemMaterialBD", { materialItemCode: $("#materialItemPesquisa").val() }, function (data) {
            try {
                $.unblockUI({ message: $('#modal-loading') });
                if (data[0].Mensagem == undefined) {
                    var retorno = JSON.parse(data);

                    $("#MaterialGroupNaoCadastrado").val('');
                    $(sam.materialItem.textoPesquisa).val(retorno.Description);
                    $(sam.materialItem.MaterialItemCode).val(retorno.Code);
                    $(sam.materialItem.MaterialGroupCode).val(retorno.MaterialGroupCode);
                    $('#MaterialGroupDescription').val(retorno.MaterialGroupDescription);
                    $('#materialItemPesquisa').val('');
                    sam.materialItem.consultaCamposDepreciacaoDoGrupo(retorno.MaterialGroupCode);
                    sam.materialItem.mostraModalAnimalAServico(retorno.MaterialGroupCode);
                    if (retorno.MaterialGroupCode != 88) {
                        sam.materialItem.alteraTextoAnimalServico("");
                    }

                    if ($("#MovementTypeId").val() == movimento.EnumMovementType.IncorpInventarioInicial) {
                        if ($('#checkFlagAcervo').iCheck('update')[0].checked == false && $('#checkFlagTerceiro').iCheck('update')[0].checked == false) {
                            sam.materialItem.consultaContaContabilPorGrupoMaterial(retorno.MaterialGroupCode);
                        }
                    }

                    sam.materialItem.VerificarDescricaoResumida(retorno.Material);
                }
                else {
                    // Consultar o servico dos itens materiais
                    sam.materialItem.ConsultaItemMaterialServico();
                }
            }
            catch (e) {
                // Possivelmente o perfil de usuário esta sem permisão de acesso.
                sam.materialItem.alteraTextoAnimalServico("");
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
    VerificarDescricaoResumida: function (_material) {
        $.get(sam.path.webroot + "ShortDescriptionItem/VerificarDescricaoResumida", { Material: _material }, function (data) {
            var retorno = JSON.parse(data);
            $('#ShortDescription').val(retorno.Description);
            if ($('#ShortDescriptionAuxiliar').length > 0) {
                $('#ShortDescriptionAuxiliar').val(retorno.Description);
            }
        });
    },
    VerificarFornecedor: function (_CgcCpf, _DescricaoFornecedor) {
        $.get(sam.path.webroot + "Suppliers/VerificarFornecedor", { CgcCpf: _CgcCpf, DescricaoFornecedor: _DescricaoFornecedor }, function (data) {
            var retorno = JSON.parse(data);
            $('#SupplierId').val(retorno.Id);
            $('#SupplierName').val(retorno.CPFCNPJ + " - " + retorno.Name);
        });
    },
    unloadModalView: function () {
        $(sam.materialItem.modalView).empty();
        $(sam.materialItem.modalView).modal('hide');
        $(sam.materialItem.divView).empty();

    },
    removeModaView: function () {
        $(sam.materialItem.modalView).remove();
    },
    loadModalView: function () {
        $('[data-toggle="tooltip"]').tooltip();
        sam.materialItem.consultaMaterial();
        sam.materialItem.paginacaoGridMaterial();
        sam.materialItem.getItemMaterial();

    },
    itemMaterialShow: function () {

        sam.materialItem.CriarDivItemMaterial();
        $(sam.materialItem.modalView).modal('show');
        $(sam.materialItem.divView).load(sam.path.webroot + "/MaterialItems/GridItemMaterial");

    },
    CriarDivItemMaterial: function () {
        var htmlBuilder = [];

        htmlBuilder.push('<div class="modal fade modalView" id="modalItemMaterial" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel">');
        htmlBuilder.push('<div class="modal-dialog" role="document">');
        htmlBuilder.push('    <div class="modal-content">');
        htmlBuilder.push('        <div class="modal-header">');
        htmlBuilder.push('            <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>');
        htmlBuilder.push('            <h4 class="modal-title" id="exampleModalLabel">Consultar Item de Material</h4>');
        htmlBuilder.push('        </div>');
        htmlBuilder.push('        <div class="modal-body">');
        htmlBuilder.push('          <div class="form-group divView" id="partialView">');
        htmlBuilder.push('          </div>');
        htmlBuilder.push('        </div>');
        htmlBuilder.push('        <div class="modal-footer">');
        htmlBuilder.push('            <button type="button"  data-toggle="tooltip" data-placement="top" data-original-title="Click para fechar a consulta de material!" class="btn btn-primary buttonClose" data-dismiss="modal">Fechar</button>');
        htmlBuilder.push('        </div>');
        htmlBuilder.push('    </div>');
        htmlBuilder.push('</div>');
        htmlBuilder.push('</div>');
        htmlBuilder.join("");

        $(document.body).append(htmlBuilder.join(""));
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

    CriarDivLoginSiafem: function () {
        var htmlBuilder = [];

        htmlBuilder.push('<div class="modal fade modalView in" id="modalLoginSiafem" tabindex="-1" aria-labelledby="exampleModalLabel" aria-hidden="false">');
        htmlBuilder.push('	<div class="modal-dialog" style="width: 20%;">');
        htmlBuilder.push('		<div class="modal-content">');
        htmlBuilder.push('			<div class="modal-header">');
        htmlBuilder.push('				<h4 class="modal-title" id="exampleModalLabelLoginSiafem">Usuario SIAFEM</h4>');
        htmlBuilder.push('			</div>');
        htmlBuilder.push('			<div class="modal-body">');
        htmlBuilder.push('				<div class="form-group">');
        htmlBuilder.push('					<label class="col-md-2 control-label" for="CPF">Login</label>');
        htmlBuilder.push('					<div class="col-md-6">');
        htmlBuilder.push('						<input autocomplete="off" class="form-control" data-val="true" data-val-required="O campo Login é obrigatório." id="CPFSIAFEMModal" maxlength="11" name="CPF" type="text" value="">');
        htmlBuilder.push('							<span class="field-validation-valid text-danger" data-valmsg-for="CPF" data-valmsg-replace="true"/>');
        htmlBuilder.push('						</div>');
        htmlBuilder.push('					</div>');
        htmlBuilder.push('					<br>');
        htmlBuilder.push('						<div class="form-group">');
        htmlBuilder.push('							<label class="col-md-2 control-label" for="Senha">Senha</label>');
        htmlBuilder.push('							<div class="col-md-6">');
        htmlBuilder.push('								<input class="form-control" data-val="true" data-val-required="O campo Senha é obrigatório." id="SenhaSIAFEMModal" name="SenhaSIAFEMModal" onfocus="$(this).removeAttr(' + "'readonly'" + ')" readonly="readonly" type="password">');
        htmlBuilder.push('									<span class="field-validation-valid text-danger" data-valmsg-for="Senha" data-valmsg-replace="true"/>');
        htmlBuilder.push('								</div>');
        htmlBuilder.push('							</div>');
        htmlBuilder.push('						</div>');
        htmlBuilder.push('						<div class="modal-footer" style="border-top:0px !important">');
        htmlBuilder.push('							<button type="button" id="SaveLoginSiafem" class="btn submit-comentario btn-success btn-salvar " data-dismiss="modal">OK</button>');
        htmlBuilder.push('						</div>');
        htmlBuilder.push('					</div>');
        htmlBuilder.push('				</div>');
        htmlBuilder.push('			</div>');
        htmlBuilder.join("")

        $(document.body).append(htmlBuilder.join(""));
    },

    //setItemMaterial: function (id, texto, lifeCycle, rateDepreciationMonthly, residualValue) {
    setItemMaterial: function (texto, lifeCycle, rateDepreciationMonthly, residualValue) {
        $(sam.materialItem.materialItemDescricao).val(texto);
        $(sam.materialItem.LifeCycle).val(parseInt(lifeCycle));
        $(sam.materialItem.RateDepreciationMonthly).val(rateDepreciationMonthly);
        $(sam.materialItem.ResidualValue).val(residualValue);
        sam.materialItem.unloadModalView();
        $(sam.materialItem.materialItemDescricao).attr('title', texto);
    },
    getItemMaterial: function () {
        $(sam.materialItem.hrefMaterialConsulta).click(function () {

            $(this).each(function () {
                $this = $(this)

                $this.find('td').each(function () {
                    var href = $this.find(sam.materialItem.materialConsultaId).attr('href');
                    //var id = parseInt(sam.utils.getURLParameter(href, sam.materialItem.paramItemMaterialId));
                    var descricao = $this.find(sam.materialItem.materialConsultaDescricao).text().ltrim().rtrim();
                    var lifeCycle = sam.utils.getURLParameter(href, sam.materialItem.paramLifeCycle);
                    var rateDepreciationMonthly = sam.utils.getURLParameter(href, sam.materialItem.paramRateDepreciationMonthly).replace(".", ",");
                    var residualValue = sam.utils.getURLParameter(href, sam.materialItem.paramResidualValue).replace(".", ",");



                    sam.materialItem.setItemMaterial(id, descricao, lifeCycle, rateDepreciationMonthly, residualValue);
                });
            });


            return false;
        });
    },
    paginacaoGridMaterial: function () {
        $(sam.materialItem.paginacao).click(function () {
            var sortOrder;
            var searchString = $(sam.materialItem.consulta).val();
            var currentFilter;

            var href = $(this).attr('href');
            var page = parseInt(sam.utils.getURLParameter(href, 'page'));


            $(sam.materialItem.divView).load(sam.path.webroot + "/MaterialItems/GridItemMaterial", { sortOrder: sortOrder, searchString: searchString, currentFilter: currentFilter, page: page }, function () {
                $(sam.materialItem.consulta).val(searchString);
                $(sam.materialItem.divGrid).show();

            });

            return false;
        });
    },

    consultaMaterial: function () {
        $(sam.materialItem.divGrid).hide();
        $(sam.materialItem.buttonConsulta).click(function () {

            var sortOrder;
            var searchString = $(sam.materialItem.consulta).val();
            var currentFilter;
            var page;

            $(sam.materialItem.divView).load(sam.path.webroot + "/MaterialItems/GridItemMaterial", { sortOrder: sortOrder, searchString: searchString, currentFilter: currentFilter, page: page }, function () {
                $(sam.materialItem.consulta).val(searchString);
                $(sam.materialItem.divGrid).show();

            });

        });
    },

    consultaClick: function () {
        $(sam.materialItem.modalMaterial).click(function () {
            sam.materialItem.unloadModalView();
            sam.materialItem.removeModaView();
            sam.materialItem.itemMaterialShow();
        });
    },

    consultaMaterialItemPorCodigo: function (code) {
        $(sam.materialItem.divView).load(sam.path.webroot + "/MaterialItems/GridItemMaterial", { sortOrder: sortOrder, searchString: code, currentFilter: currentFilter, page: page }, function () {
        });
    },

    consultaContaContabilPorGrupoMaterial: function (codigo) {
        $("#AuxiliaryAccountId").empty().append('<option class="filter-option pull-left" value> -- Selecione -- </option>');
        $.get(sam.path.webroot + "AuxiliaryAccounts/CarregaComboContaContabilPorGrupo", { code: codigo }, function (data) {
            if (data.length == 1) {
                $("#AuxiliaryAccountId").append('<option class="filter-option pull-left" value="' + data[0].Value + '">' + data[0].Text + '</option>');
                $("#AuxiliaryAccountId option:eq(1)").prop("selected", true);
            } else {
                $.each(data, function (key, value) {
                    $("#AuxiliaryAccountId").append('<option class="filter-option pull-left" value="' + value.Value + '">' + value.Text + '</option>');
                });
            }

        });
    },
    consultaContaContabilPorTipo: function (codigo) {
        $("#AuxiliaryAccountId").empty().append('<option class="filter-option pull-left" value> -- Selecione -- </option>');
        $.get(sam.path.webroot + "AuxiliaryAccounts/CarregaComboContaContabilPorTipo", { tipo: codigo }, function (data) {
            if (data.length == 1) {
                $("#AuxiliaryAccountId").append('<option class="filter-option pull-left" value="' + data[0].Value + '">' + data[0].Text + '</option>');
                $("#AuxiliaryAccountId option:eq(1)").prop("selected", true);
            } else {
                $.each(data, function (key, value) {
                    $("#AuxiliaryAccountId").append('<option class="filter-option pull-left" value="' + value.Value + '">' + value.Text + '</option>');
                });
            }

        });
    },
    consultaCamposDepreciacaoDoGrupo: function (codigo) {
        $.get(sam.path.webroot + "MaterialGroups/ValoresDepreciacaoGrupoMaterial", { MaterialGroupCode: codigo }, function (data) {
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
    },
    mostraModalAnimalAServico: function (codigo) {
        if (codigo == 88) {
            $("#modalAnimalAServico").modal(
                {
                    keyboard: false,
                    backdrop: 'static',
                    show: true
                });
            $("#msgAnimalAServico").css("display", "block");
        } else {
            $("#msgAnimalAServico").css("display", "none");
        }
    },
    alteraTextoAnimalServico: function (texto) {
        $("#msgAnimalAServico").html(texto);
    }

}
