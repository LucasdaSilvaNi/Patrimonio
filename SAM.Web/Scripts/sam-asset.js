sam.asset = {
    calendario: '.calendario',
    formSubmit: '.formSubmit',
    selectElementInitialValue: '<option value="">-- Selecione --</option>',
    selectElementInstitution: '.comboinstitution',
    selectElementBudgetUnit: '.comboBudgetUnit',
    selectElementManagerUnit: '.comboManagerUnit',
    selectElementAdministrativeUnit: '.comboAdministrativeUnit',
    selectElementSection: '.comboSection',
    selectElementMaterial: '.ddl-asset-material',
    selectElementMaterialClass: '.ddl-asset-materialClass',
    selectElementMaterialItem: '.ddl-asset-materialItem',
    selectElementMaterialGroup: '.ddl-asset-materialGroup',
    InstitutionIdBemPatrimonial: 0,
    ManagerIdBemPatrimonial: 0,
    BudgetUnitIdBempatrimonial: 0,
    ManagerUnitIdBemPatrimonial: 0,
    AdministrativeUnitIdBemPatrimonial: 0,
    SectionIdBemPatrimonial: 0,
    NumberIdentification: '.NumberIdentification',
    buttonConsulta: '.btnConsulta',
    divView: '.divView',
    modalView: '.modalView',
    paginacao: '.pagination li a',
    consulta: '.gridConsulta',
    divGrid: '.divGrid',
    xml: '',
    voltaXml: 0,
    init: function () {
        $('[data-toggle="tooltip"]').tooltip();
    },
    load: function (institutionId, callback) {
        sam.asset.submitForm();
        sam.asset.setHiddenValues();
        sam.asset.verficarChapaFinalMenorQueInicial();
        sam.asset.EventoTrocaTipoMovimento();
        sam.asset.CarregaCheckBox();
        sam.asset.SetaFlag();
        sam.asset.EventoTravaGridsMovimentacaoInventario();
        sam.asset.verificaBloqueioCampoEmpenho();
    },
    SetaFlag: function () {
        if ($('#MovementTypeId').val() === "2" || $('#MovementTypeId').val() === "3" || $('#MovementTypeId').val() == "47" || $('#MovementTypeId').val() == "40" || $('#MovementTypeId').val() == "41") {
            if ($('#checkFlagDecretoSefaz').val() === "true" || $('#checkFlagAcervo').val() == "true" || $('#checkFlagTerceiro').val() == "true") {
                if ($('#checkFlagDecretoSefaz').val() == "true") {
                    document.getElementById('TipoFlag').innerHTML = "BEM PATRIMONIAL - TIPO DECRETO";
                    $('#checkFlagAcervo').val(false);
                    $('#checkFlagTerceiro').val(false);
                }
                if ($('#checkFlagAcervo').val() == "true") {
                    document.getElementById('TipoFlag').innerHTML = "BEM PATRIMONIAL - TIPO ACERVO";
                    $('#checkFlagDecretoSefaz').val(false);
                    $('#checkFlagTerceiro').val(false);
                    $('#div_Depreciacao').css('display', 'none');
                    sam.asset.CamposDepreciacao(0);
                }
                if ($('#checkFlagTerceiro').val() == "true") {
                    document.getElementById('TipoFlag').innerHTML = "BEM PATRIMONIAL - TIPO TERCEIRO";
                    $('#checkFlagDecretoSefaz').val(false);
                    $('#checkFlagAcervo').val(false);
                    $('#div_Depreciacao').css('display', 'none');
                    sam.asset.CamposDepreciacao(0);
                }
            }
        }
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
           
            if($('#hiddenEmpenho').val() != null)
                $('#Empenho').val($('#hiddenEmpenho').val());
        });

        // Depreciacao
        if ($('#checkDepreciacao').is(':checked')) {
            document.getElementById('LifeCycle').readOnly = false;
            //document.getElementById('RateDepreciationMonthly').readOnly = false;
            document.getElementById('ResidualValue').readOnly = false;
            $('#textoDepreciacao').html($('#textoDepreciacao').html().replace("automaticamente", "manualmente"));
        }
        else {
            document.getElementById('LifeCycle').readOnly = true;
            //document.getElementById('RateDepreciationMonthly').readOnly = true;
            document.getElementById('ResidualValue').readOnly = true;
        }

        $('#checkDepreciacao').on('ifChanged', function (event) {
            if (this.checked) {
                $('#textoDepreciacao').html($('#textoDepreciacao').html().replace("automaticamente","manualmente"));
                document.getElementById('LifeCycle').readOnly = false;
                //document.getElementById('RateDepreciationMonthly').readOnly = false;
                document.getElementById('ResidualValue').readOnly = false;
            }
            else {
                if ($("#MaterialGroupCode").val() != '') {
                    sam.materialItem.consultaCamposDepreciacaoDoGrupo($("#MaterialGroupCode").val());
                }
                else {
                    $('#LifeCycle').val('');
                    $('#ResidualValue').val('');
                    $('#RateDepreciationMonthly').val('');
                }
                $('#textoDepreciacao').html($('#textoDepreciacao').html().replace("manualmente", "automaticamente"));
                document.getElementById('LifeCycle').readOnly = true;
                //document.getElementById('RateDepreciationMonthly').readOnly = true;
                document.getElementById('ResidualValue').readOnly = true;
            }
        });

        if ($('#MovementTypeId').val() == '40' ||
            $('#MovementTypeId').val() == '41' ||
            $('#MovementTypeId').val() == '31' ||
            $('#MovementTypeId').val() == '38' ||
            $('#MovementTypeId').val() == '39')
        {
            sam.asset.BloqueiaCampoLoteChapa();
        } else {
            sam.asset.CarregaCampoLoteChapa();
        }

        if ($('#MovementTypeId').val() == '36') {
            sam.asset.CarregaCamposMudancaRevalorizacao();
        }
      

        if ($('#MovementTypeId').val() == '5') {
            sam.asset.CarregaCamposInventarioInicial();
        }

        if ($('#MovementTypeId').val() == '27') {
            $('#div_Acervo').css('display', 'block');
            $('#div_Depreciacao').css('display', 'none');
            $('#MaterialItemCode').val('5628121');
            $('#MaterialItemDescription').val('Bens de Terceiros');
            sam.asset.CamposDepreciacao(0);
            sam.materialItem.consultaContaContabilPorTipo(2);
        }
    },

    CamposDepreciacao: function (valor) {
        $('#LifeCycle').val(valor);
        $('#ResidualValue').val(valor);
        $('#RateDepreciationMonthly').val(valor);
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

    LimpaCamposItemMaterial: function (valor) {
        $('#MaterialItemCode').val(valor);
        $('#MaterialItemDescription').val(valor);
        $('#MaterialGroupCode').val(valor);
        $('#MaterialGroupDescription').val(valor);
    },

    EventoTrocaTipoMovimento: function () {
        //Tipo incorporacao
        $('#MovementTypeId').change(function (e) {
            
            //Desbloqueia o campo empenho de dados complementares
            $('#Empenho').removeClass("Empenho");
            $('#Empenho').removeAttr("readonly");
            $('#AcquisitionDate').removeAttr('disabled');
            $('#hiddenEmpenho').val('');

            $("#msgAnimalAServico").html("");
            $("#msgAnimalAServico").css("display", "none");
            sam.asset.xml = "";

            if ($('#MovementTypeId').val() == '40' ||
                $('#MovementTypeId').val() == '41' ||
                $('#MovementTypeId').val() == '31' ||
                $('#MovementTypeId').val() == '38' ||
                $('#MovementTypeId').val() == '39'
               ) {
                sam.asset.BloqueiaCampoLoteChapa();
                
                if (
                    ($('#MovementTypeId').val() == '40') ||
                    ($('#MovementTypeId').val() != '40' && $('#AceiteManual').val() != 'true'))
                {
                    $('#ValueAcquisitionModel').attr('disabled', 'disabled');
                    $('#AcquisitionDate').attr('disabled', 'disabled');
                } else {
                    $('#ValueAcquisitionModel').removeAttr('disabled');
                    $('#checkLoteChapa').removeAttr("disabled");
                }

            } else {
                $('#ValueAcquisitionModel').removeAttr('disabled');
                $('#checkLoteChapa').removeAttr("disabled");
            }

            //Verifica se apresenta a div dos campos de depreciação ou não
            if ($('#MovementTypeId').val() == movimento.EnumMovementType.IncorpComodatoDeTerceirosRecebidos) {
                $('#div_Depreciacao').css('display', 'none');
            } else {
                $('#div_Depreciacao').css('display', 'block');
            }

            var tipo = eval($(e.currentTarget).val());
            if (tipo == 0 || tipo == undefined) {
                $('#omite_div_incorporacao').css('display', 'none');
                sam.commun.limpar('#omite_div_incorporacao');
            }
            else {
                $.blockUI({ message: $('#modal-loading') });
                $.get(sam.path.webroot + "Assets/CarregaPartialViewIncorporation", { tipoMovimento: $("#MovementTypeId").val() }, function (data) {

                    $('#omite_div_incorporacao').html(data);
                    $('#omite_div_incorporacao').css('display', 'block');

                    $('#AuxiliaryAccountId').empty().append('<option class="filter-option pull-left" value> -- Selecione -- </option>');

                    $.unblockUI({ message: $('#modal-loading') });

                }).fail(function () {
                    $.unblockUI({ message: $('#modal-loading') });
                    alert('Erro na rotina EventoTrocaTipoMovimento.');
                });
            }
            sam.asset.limpaCamposCarregados();
            sam.asset.HabilitarCamposTipoMovimento();
            
            // permanecer os dados complementares abertos e checados
            if ($('#omite_div_veiculo').css('display') == "block") {
                $('#checkComplemento').parent().addClass("checked");
                $('#checkComplemento').prop("checked", true);
            }

            sam.asset.setHiddenValuesOnLoad();
        });

        if ($('#MovementTypeId').val() != "") {
            $('#omite_div_incorporacao').css('display', 'block');

            sam.asset.HabilitarCamposTipoMovimento();
        }
    },
    HabilitarCamposTipoMovimento: function () {
         $('.bloquear').prop('readonly', false);
         $('.esconder').css('display', 'block');        
    },
    limpaCamposCarregados: function () {

        // Dados do Movimento Patrimonio
        $('#StateConservationId').val('');
        $('#AuxiliaryAccountId').val('');
        $("#OrigemManagerUnit").val('');

        // Dados do Patrimonio
        $("#NumberDoc").val('');
        $("#NumeroDocumentoAtual").val('');
        $("#MaterialItemCode").val('');
        $("#MaterialItemDescription").val('');
        $("#MaterialGroupCode").val('');
        $("#MaterialGroupDescription").val('');
        $("#ShortDescription").val('');
        $('#LifeCycle').val('');
        $('#ResidualValue').val('');
        $('#RateDepreciationMonthly').val('');
        //$("#checkDepreciacao").val('');
        $('#OldNumberIdentification').val('');
        $('#DiferenciacaoChapaAntiga').val('');
        $('#OldInitial').val('');
        $('#ValueAcquisitionModel').val('');
        //$('#ValueUpdateModel').val('');
        $('#AcquisitionDate').val('');
        $('#EmpenhoResultado').val('');
        $('#Empenho').val('');
        $('#EndNumberIdentification').val('');

        // Dados Complementares
        $('#SerialNumber').val('');
        $('#ManufactureDate').val('');
        $('#DateGuarantee').val('');
        $('#ChassiNumber').val('');
        $('#Brand').val('');
        $('#Model').val('');
        $('#NumberPlate').val('');
        $('#AdditionalDescription').val('');

        document.getElementById('ValueAcquisitionModel').readOnly = false;
        document.getElementById('AcquisitionDate').readOnly = false;
        $('#checkComplemento')[0].checked = false;
        $('#checkDepreciacao')[0].checked = false;
        document.getElementById('LifeCycle').readOnly = true;
        document.getElementById('ResidualValue').readOnly = true;
        //document.getElementById('RateDepreciationMonthly').readOnly = true;
        $('#checkLoteChapa')[0].checked = false;
        $('.icheckbox_square-blue').removeClass('checked');
    },

    setHiddenValues: function () {
        $("#InstitutionId").val($(sam.asset.selectElementInstitution).val());
        $("#ManagerId").val($(sam.asset.selectElementManager).val());
        $("#BudgetUnitId").val($(sam.asset.selectElementBudgetUnit).val());
        $("#ManagerUnitId").val($(sam.asset.selectElementManagerUnit).val());
        $("#AdministrativeUnitId").val($(sam.asset.selectElementAdministrativeUnit).val());
        $("#SectionId").val($(sam.asset.selectElementSection).val());
    },

    verficarChapaFinalMenorQueInicial: function () {
        $('#EndNumberIdentification').blur(function () {
            var chapaFinal = $('#EndNumberIdentification').val();
            var chapaInicial = $('#NumberIdentification').val();

            if (parseInt(chapaFinal) < parseInt(chapaInicial)) {
                $('span[data-valmsg-for="EndNumberIdentification"]').text('Número da chapa final menor que chapa inicial');
                $.unblockUI({ message: $('#modal-loading') });
            }
            else
                $('span[data-valmsg-for="EndNumberIdentification"]').text('');
        });

    },

    CustomAlert: function () {
        this.render = function (dialog, _head, _foot) {
            var winW = window.innerWidth;
            var winH = window.innerHeight;
            var dialogoverlay = document.getElementById('dialogoverlay');
            var dialogbox = document.getElementById('dialogbox');
            dialogoverlay.style.display = "block";
            dialogoverlay.style.height = winH + "px";
            dialogbox.style.left = (winW / 2) - (550 * .5) + "px";
            dialogbox.style.top = "100px";
            dialogbox.style.display = "block";
            document.getElementById('dialogboxhead').innerHTML = _head;
            document.getElementById('dialogboxbody').innerHTML = dialog;
            document.getElementById('dialogboxfoot').innerHTML = _foot;
        }

        this.ok = function () {
            document.getElementById('dialogbox').style.display = "none";
            document.getElementById('dialogoverlay').style.display = "none";
        }
    },

    submitForm: function () {
        $("#formBemPatrimonial").submit(function () {
            var chapaFinal = $('#EndNumberIdentification').val();
            $('.sam-moeda').val($('.sam-moeda').val().replace('.', ''));

            if (chapaFinal != NaN) {
                var chapaInicial = $('#NumberIdentification').val();
                if (parseInt(chapaFinal) < parseInt(chapaInicial)) {
                    $('span[data-valmsg-for="EndNumberIdentification"]').text('Número da chapa final menor que chapa inicial');
                    $.unblockUI({ message: $('#modal-loading') });
                    return false;
                }
            }

            if ($("#ShortDescription").val() == "") {
                $('span[data-valmsg-for="ShortDescription"]').text('O Campo Descrição Resumida do Item é obrigatório');
                $.unblockUI({ message: $('#modal-loading') });
                return false;
            }

            if ($('#MovementTypeId').val() == movimento.EnumMovementType.IncorpTransferenciaOutroOrgaoPatrimoniado ||
                $('#MovementTypeId').val() == movimento.EnumMovementType.IncorpDoacaoIntraNoEstado ||
                $('#MovementTypeId').val() == movimento.EnumMovementType.IncorpRecebimentoDeInservivelUGEDoacao ||
                $('#MovementTypeId').val() == movimento.EnumMovementType.IncorpRecebimentoDeInservivelUGETranferencia)
            {
                if ($('#AceiteManual').val() == 'true') {

                    if ($('#ManagerUnitIdDestino').val() <= 0    ||
                        $('#BudgetUnitIdDestino').val() <= 0     ||
                        $('#InstituationIdDestino').val() <= 0   ||
                        $('#ManagerUnitIdDestino').val() == null ||
                        $('#BudgetUnitIdDestino').val() == null  ||
                        $('#InstituationIdDestino').val() == null
                        ) {

                        if ($('#ManagerUnitIdDestino').val() <= 0 || $('#ManagerUnitIdDestino').val() == null) {
                            $('#spanManagerUnits').text('O campo UGE é obrigatório');
                        } else {
                            $('#spanManagerUnits').text('');
                        }

                        if ($('#BudgetUnitIdDestino').val() <= 0 || $('#BudgetUnitIdDestino').val() == null) {
                            $('#spanBudgetUnits').text('O campo UO é obrigatório');
                        } else {
                            $('#spanBudgetUnits').text('');
                        }

                        if ($('#InstituationIdDestino').val() <= 0 || $('#InstituationIdDestino').val() == null) {
                            $('#spanInstitution').text('O campo Orgão é obrigatório');
                        } else {
                            $('#spanInstitution').text('');
                        }

                        $.unblockUI({ message: $('#modal-loading') });
                        return false;
                    }
                }
            }

            if ($('#checkFlagDecretoSefaz').val() == "true") {
                $("#ValueAcquisitionModel").text('');
            }
            else {

                if ($('#MovementTypeId').val() == movimento.EnumMovementType.IncorpTransferenciaMesmoOrgaoPatrimoniado ||
                    $('#MovementTypeId').val() == movimento.EnumMovementType.IncorpTransferenciaOutroOrgaoPatrimoniado ||
                    $('#MovementTypeId').val() == movimento.EnumMovementType.IncorpDoacaoIntraNoEstado ||
                    $('#MovementTypeId').val() == movimento.EnumMovementType.IncorpRecebimentoDeInservivelUGEDoacao ||
                    $('#MovementTypeId').val() == movimento.EnumMovementType.IncorpRecebimentoDeInservivelUGETranferencia
                    ) {
                    if (parseFloat($('#ValueAcquisitionModel').val().replace(".", "")) < parseFloat("1,00")) {
                        $('span[data-valmsg-for="ValueAcquisitionModel"]').text('Valor não pode ser menor que 1,00!');
                        $.unblockUI({ message: $('#modal-loading') });
                        return false;
                    }
                    else
                        $("#ValueAcquisitionModel").text('');
                } else {
                    if (parseFloat($('#ValueAcquisitionModel').val().replace(".", "")) < parseFloat("10,00")) {
                        $('span[data-valmsg-for="ValueAcquisitionModel"]').text('Valor não pode ser menor que 10,00!');
                        $.unblockUI({ message: $('#modal-loading') });
                        return false;
                    }
                    else
                        $("#ValueAcquisitionModel").text('');

                }
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
    //SetEventoValor: function () {
    //    $('#ValueAcquisitionModel').blur(function () {
    //        if ($('#MovementTypeId').val() == '40' || $('#MovementTypeId').val() == '41' || $('#MovementTypeId').val() == '31') {
    //            if (parseFloat($('#ValueAcquisitionModel').val().replace(".", "")) < parseFloat("1,00")) {
    //                $('span[data-valmsg-for="ValueAcquisitionModel"]').text('Valor não pode ser menor que 1,00, favor verificar!');
    //                return false;
    //            }
    //            else
    //                $('span[data-valmsg-for="ValueAcquisitionModel"]').text('');
    //        } else {
    //            if (parseFloat($('#ValueAcquisitionModel').val().replace(".", "")) < parseFloat("10,00")) {
    //                $('span[data-valmsg-for="ValueAcquisitionModel"]').text('Valor não pode ser menor que 10,00, favor verificar!');
    //                return false;
    //            }
    //            else
    //                $('span[data-valmsg-for="ValueAcquisitionModel"]').text('');
    //        }
    //    });
    //},

    GetInicioDaProximaNumberIdentification: function (NumberIdentification) {
        $.post(sam.path.webroot + "/Assets/GetInicioDaProximaNumberIdentification", { NumberIdentification: NumberIdentification }, function (data) {
    
            if (data != "False") {
                $('span[data-valmsg-for="NumberIdentification"]').text('Número de inicio da chapa já está cadastrado');
                return false;
            }
            else {
                $('span[data-valmsg-for="NumberIdentification"]').text('');
            }
        });
    },
    SetEventoProximaNumberIdentification: function () {
        $('#NumberIdentification').blur(function () {
            if ($('#NumberIdentification').val() != "")
                sam.asset.GetInicioDaProximaNumberIdentification($('#NumberIdentification').val());
            else
                $('span[data-valmsg-for="NumberIdentification"]').text('');
        });
    },
    GetFinalDoProximaNumberIdentification: function (assetId) {
        $.get(sam.path.webroot + "/Assets/GetFinalDoProximaNumberIdentification", { assetId: assetId }, function (data) {
            $(NumberIdentification).val(data);
        });
    },
    FormataCalendario: function () {
        $(window).load(function () {
            var d = new Date($(sam.asset.calendario).val());
            var concatenacao = (d.getDay().toString().length = 1 ? "0" + d.getDay() : d.getDay()) + "/" + d.getDate() + "/" + d.getFullYear();

            $(sam.asset.calendario).val(concatenacao);
        });


    },
    loadModalView: function () {
        $('[data-toggle="tooltip"]').tooltip();
        sam.asset.consultaAssetsOut();
        sam.asset.paginacaoGridAssets();
        //sam.asset.getItemMaterial();

    },
    paginacaoGridAssets: function () {
        $(sam.asset.paginacao).click(function () {
            var sortOrder;
            var searchString = $(sam.asset.consulta).val();
            var currentFilter;

            var href = $(this).attr('href');
            var page = parseInt(sam.utils.getURLParameter(href, 'page'));


            $(sam.materialItem.divView).load(sam.path.webroot + "/AssetsOut/GridAssets", { sortOrder: sortOrder, searchString: searchString, currentFilter: currentFilter, page: page }, function () {
                $(sam.asset.consulta).val(searchString);
                $(sam.asset.divGrid).show();

            });

            return false;
        });
    },
    consultaAssetsOut: function () {
        $(sam.asset.divGrid).hide();
        $(sam.asset.buttonConsulta).click(function () {

            var sortOrder;
            var searchString = $(sam.asset.consulta).val();
            var currentFilter;
            var page;

            $(sam.asset.divView).load(sam.path.webroot + "/AssetsOut/GridAssets", { sortOrder: sortOrder, searchString: searchString, currentFilter: currentFilter, page: page }, function () {
                $(sam.asset.consulta).val(searchString);
                $(sam.asset.divGrid).show();

            });

        });
    },

    //loadModalViewDocumento: function () {
    //    $('[data-toggle="tooltip"]').tooltip();
    //    //sam.asset.DocumentoShow();
    //},

    CriarDivGrid: function (_nome) {
        var htmlBuilder = [];

        htmlBuilder.push('<div class="modal fade modalViewGrid" id="modalViewGrid" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel">');
        htmlBuilder.push('  <div class="modal-dialog" role="document" style="width: 80%">');
        htmlBuilder.push('    <div class="modal-content">');
        htmlBuilder.push('        <div class="modal-header">');
        //        htmlBuilder.push('            <button type="button" id="closemodal" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>');
        htmlBuilder.push('            <h4 class="modal-title" id="exampleModalLabel"> Pesquisar ' + _nome + '</h4>');
        htmlBuilder.push('        </div>');
        htmlBuilder.push('        <div class="modal-body">');
        htmlBuilder.push('          <div class="form-group divViewGrid" id="partialView">');
        htmlBuilder.push('          </div>');
        htmlBuilder.push('        </div>');
        htmlBuilder.push('        <div class="modal-footer">');
        htmlBuilder.push('            <button type="button" id="save-btn-modal" class="btn submit-comentario btn-success btn-salvar">Ok</button>');
        htmlBuilder.push('            <button type="button" id="fecharModalViewGrid" class="btn btn-primary" data-dismiss="modal">Cancelar</button>');
        htmlBuilder.push('        </div>');
        htmlBuilder.push('    </div>');
        htmlBuilder.push('  </div>');
        htmlBuilder.push('</div>');
        htmlBuilder.join("")

        $(document.body).append(htmlBuilder.join(""));
    },
    limparModalDivGrid: function () {
        $('.modalViewGrid').empty();
        $('.divViewGrid').empty();
        $('.modalViewGrid').remove();
        if ($('#Empenho') != null) {
            $('#Empenho').val('');
        }
    },

    ConsultarDocumento: function () {
        $('#btnConsultarDocumento').click(function () {
            sam.asset.limparModalDivGrid();
            sam.asset.DocumentoShow();
        });
    },
    DocumentoShow: function () {
        sam.asset.CriarDivGrid("Documento");
        let tipoTransferencia = $("#EnumMovimentoAPesquisar").val();
        $('.divViewGrid').load(sam.path.webroot + "/Assets/GridDocumento", { codigoUGE: $("#ManagerUnitId").val(), tipoTransferencia: tipoTransferencia }, function () {
            $('.modalViewGrid').modal('show');
            sam.asset.paginacaoGridDocumento(tipoTransferencia);
            $('#save-btn-modal').click(function () {
                sam.asset.LerDocumentoInformado(tipoTransferencia);
            });
            $('#fecharModalViewGrid').click(function () {
                $('.modalViewGrid').modal('hide');
                //Carrega novamente pois a modal mata o evento dos checkbox da pagina principal
                sam.asset.CarregaCheckBox();
            });
        });
    },
    paginacaoGridDocumento: function (tipoTransferencia) {
        $('.pagination li a').click(function () {
            var sortOrder;
            var searchString = $('.search-tables').val();
            var currentFilter;

            var href = $(this).attr('href');
            var page = parseInt(sam.utils.getURLParameter(href, 'page'));

            $('.divViewGrid').load(sam.path.webroot + "/Assets/GridDocumento", { codigoUGE: $("#ManagerUnitId").val(), tipoTransferencia: tipoTransferencia, sortOrder: sortOrder, searchString: searchString, currentFilter: currentFilter, page: page }, function () {
                $(sam.asset.consulta).val(searchString);
                $('.divViewGrid').show();
                $('#save-btn-modal').click(function () { sam.asset.LerDocumentoInformado(tipoTransferencia); });
                sam.asset.paginacaoGridDocumento(tipoTransferencia);
            });

            return false;
        });
    },
    LerDocumentoInformado: function (tipoTransferencia) {
        var numerodocumento = $('#result-tables tbody tr td').find('.radioSelecionado:checked')[0];

        if (numerodocumento != undefined) {
            var chapa = $($('#result-tables tbody tr td').find('.radioSelecionado:checked')[0].closest('tr')).children('.Chapaclass')[0].innerText;
            var AssetId = $($('#result-tables tbody tr td').find('.radioSelecionado:checked')[0].closest('tr')).children('.IdAsset')[0].innerText;
            $("#NumberDoc").val(numerodocumento.id);
            sam.asset.ConsultaDocumentoInformado(tipoTransferencia, chapa, AssetId);
            $('.modalViewGrid').modal('hide');
            //Carrega novamente pois a modal mata o evento dos checkbox da pagina principal
            sam.asset.CarregaCheckBox();
        }
    },
    ConsultaDocumentoInformado: function (tipoTransferencia, chapa, AssetId) {
        //if ($("#numeroDocumento").val() != "") {
        if ($("#ManagerUnitId").val() != "0") {
            $.blockUI({ message: $('#modal-loading') });
            $.get(sam.path.webroot + "Assets/PesquisaPorDocumento", { numeroDocumento: $("#NumberDoc").val(), chapa: chapa, codigoUGE: $("#ManagerUnitId").val(), tipoTransferencia: tipoTransferencia, AssetId: AssetId }, function (data) {

                try {
                    if (data[0].Mensagem == undefined) {
                        var retorno = JSON.parse(data);
                        $("#MaterialGroupNaoCadastrado").val('');
                        $("#numeroDocumento").val('');

                        // Dados do Movimento Patrimonio
                        $('#StateConservationId').val(retorno.StateConservationId);
                        $('#AuxiliaryAccountId').val(retorno.AuxiliaryAccountId);
                        $("#OrigemManagerUnit").val(retorno.OrigemManagerUnit);
                        $("#AssetsIdTransferencia").val(retorno.AssetsIdTransferencia);

                        // Dados do Patrimonio
                        $("#NumberDoc").val(retorno.NumberDoc);
                        if ($("#NumeroDocumentoAtual") != null) {
                            $("#NumeroDocumentoAtual").val(retorno.NumeroDocumentoAtual);
                        }

                        $("#MaterialGroupCode").val(retorno.MaterialGroupCode);
                        $("#checkflagAnimalAServico").val(retorno.checkflagAnimalAServico);

                        if (retorno.MaterialGroupCode == 88) {
                            $("#msgAnimalAServico").css("display", "block");
                            if (retorno.checkflagAnimalAServico) {
                                sam.materialItem.alteraTextoAnimalServico("Animal está a serviço do estado");
                            } else { 
                                sam.materialItem.alteraTextoAnimalServico("Animal <b>não</b> está a serviço do estado");
                            }
                        } else {
                            $("#msgAnimalAServico").css("display", "none");
                            sam.materialItem.alteraTextoAnimalServico("");
                        }

                        $("#MaterialItemCode").val(retorno.MaterialItemCode);
                        $("#MaterialItemDescription").val(retorno.MaterialItemDescription);
                        $("#MaterialGroupDescription").val(retorno.MaterialGroupDescription);
                        $("#ShortDescription").val(retorno.ShortDescription);
                        $('#LifeCycle').val(retorno.LifeCycle);
                        $('#LifeCycle').val($('#LifeCycle').val().replace('.', ','));
                        $('#ResidualValue').val(retorno.ResidualValue);
                        $('#ResidualValue').val($('#ResidualValue').val().replace('.', ','));
                        $('#RateDepreciationMonthly').val(retorno.RateDepreciationMonthly);
                        $('#RateDepreciationMonthly').val($('#RateDepreciationMonthly').val().replace('.', ','));
                        $("#checkDepreciacao").val(retorno.AceleratedDepreciation);
                        if (retorno.checkDepreciacao) {
                            $('#checkDepreciacao')[0].checked = true;
                            $('#checkDepreciacao').prop('checked', true).iCheck('update');
                        }
                        else {
                            $('#checkDepreciacao')[0].checked = false;
                            $('#checkDepreciacao').prop('checked', false).iCheck('update');
                        }
                        if (retorno.checkFlagDecretoSefaz || retorno.checkFlagAcervo || retorno.checkFlagTerceiro) {
                            if (retorno.checkFlagDecretoSefaz) {
                                document.getElementById('TipoFlag').innerHTML = "BEM PATRIMONIAL - TIPO DECRETO";
                                $('#checkFlagDecretoSefaz').val(true);
                                $('#checkFlagAcervo').val(false);
                                $('#checkFlagTerceiro').val(false);
                            }
                            if (retorno.checkFlagAcervo) {
                                document.getElementById('TipoFlag').innerHTML = "BEM PATRIMONIAL - TIPO ACERVO";
                                $('#checkFlagAcervo').val(true);
                                $('#checkFlagDecretoSefaz').val(false);
                                $('#checkFlagTerceiro').val(false);
                                $('#div_Depreciacao').css('display', 'none');
                                sam.asset.CamposDepreciacao(0);
                            }
                            if (retorno.checkFlagTerceiro) {
                                document.getElementById('TipoFlag').innerHTML = "BEM PATRIMONIAL - TIPO TERCEIRO";
                                $('#checkFlagTerceiro').val(true);
                                $('#checkFlagAcervo').val(false);
                                $('#checkFlagDecretoSefaz').val(false);
                                $('#div_Depreciacao').css('display', 'none');
                                sam.asset.CamposDepreciacao(0);
                            }
                        }
                        else {
                            $('#checkFlagDecretoSefaz').val(false);
                            $('#checkFlagAcervo').val(false);
                            $('#checkFlagTerceiro').val(false);
                            document.getElementById('TipoFlag').innerHTML = "";
                        }

                        $('#OldNumberIdentification').val(retorno.OldNumberIdentification);
                        $('#DiferenciacaoChapaAntiga').val(retorno.DiferenciacaoChapaAntiga);
                        $('#OldInitial').val(retorno.OldInitial);
                        $('#OldInitialDescription').val(retorno.OldInitialDescription);
                        $('#ValueAcquisitionModel').val(retorno.ValueAcquisition);
                        $('#ValueAcquisitionModel').val($('#ValueAcquisitionModel').val().replace('.', ','));
                        document.getElementById('ValueAcquisitionModel').readOnly = true;
                        $('#ValueUpdateModel').val(retorno.ValueUpdate);
                        $('#ValueUpdateModel').val($('#ValueUpdateModel').val().replace('.', ','));
                        //$('#ValueUpdateModel').val(retorno.ValueUpdate);
                        //$('#ValueUpdateModel').val($('#ValueUpdateModel').val().replace('.', ','));
                        $('#AcquisitionDate').val(sam.commun.FormatarDataJson(retorno.AcquisitionDate));
                        document.getElementById('AcquisitionDate').readOnly = true;
                        $('#Empenho').val(retorno.Empenho);
                        $('#EmpenhoResultado').val(retorno.Empenho);
                        $('#hiddenEmpenho').val(retorno.Empenho);
                        // Dados Complementares
                        if (retorno.checkComplemento) {
                            $('#checkComplemento')[0].checked = true;
                            $('#checkComplemento').prop('checked', true).iCheck('update');
                        }
                        else {
                            $('#checkComplemento')[0].checked = false;
                            $('#checkComplemento').prop('checked', false).iCheck('update');
                        }
                        sam.asset.CarregaCheckBox();
                        $('#SerialNumber').val(retorno.SerialNumber);
                        $('#ManufactureDate').val(sam.commun.FormatarDataJson(retorno.ManufactureDate));
                        $('#DateGuarantee').val(sam.commun.FormatarDataJson(retorno.DateGuarantee));
                        $('#ChassiNumber').val(retorno.ChassiNumber);
                        $('#Brand').val(retorno.Brand);
                        $('#Model').val(retorno.Model);
                        $('#NumberPlate').val(retorno.NumberPlate);
                        $('#AdditionalDescription').val(retorno.AdditionalDescription);

                        if (retorno.checkFlagAcervo == false && retorno.checkFlagTerceiro == false) {
                            sam.materialItem.consultaContaContabilPorGrupoMaterial(retorno.MaterialGroupCode);
                        } else {
                            sam.materialItem.consultaContaContabilPorTipo(retorno.TipoBP);
                        }

                        $.unblockUI({ message: $('#modal-loading') });
                    }
                    else {
                        var retorno2 = (data[0].Mensagem.length < 1);

                        //Limpa os campos que são carregados de acordo com o documento informado
                        sam.asset.limpaCamposCarregados();

                        $('#modalMaterialGroupNaoCadastrado').modal('show');
                        $('#mensagemmodal').html(data[0].Mensagem);
                        $("#MaterialGroupNaoCadastrado").val('true');

                        if ($('#EmpenhoResultado').length > 0) {
                            $('#EmpenhoResultado').val(retorno.Empenho);
                        }
                        $.unblockUI({ message: $('#modal-loading') });
                        return retorno2;
                    }
                }
                catch (e) {
                    // Possivelmente o perfil de usuário esta sem permisão de acesso.
                    $.unblockUI({ message: $('#modal-loading') });
                    $('#modalMaterialGroupNaoCadastrado').modal('show');
                    $('#mensagemmodal').html("1: Não foi possivel realizar a pesquisa do Documento, favor contatar o administrador responsável!");
                }
            }).fail(function () {
                $.unblockUI({ message: $('#modal-loading') });
                $('#modalMaterialGroupNaoCadastrado').modal('show');
                $('#mensagemmodal').html("2: Não foi possivel realizar a pesquisa do Documento, favor contatar o administrador responsável!");
            });
        }
        else {
            $('#modalMaterialGroupNaoCadastrado').modal('show');
            $('#mensagemmodal').html("Favor informar a UGE antes de realizar a pesquisa!");
        }
        //}
        //else
        //{
        //    sam.asset.limparModalDivGrid();
        //    sam.asset.DocumentoShow(tipoTransferencia);
        //}
    },

    ConsultarFornecedor: function () {
        $('#btnConsultarFornecedor').click(function () {
            sam.asset.limparModalDivGrid();
            sam.asset.FornecedorShow();
        });
    },
    FornecedorShow: function () {
        sam.asset.CriarDivGrid("Fornecedor");
        $('.divViewGrid').load(sam.path.webroot + "/Assets/GridFornecedor", function () {
            $('.modalViewGrid').modal('show');
            sam.asset.paginacaoGridFornecedor();
            $('#save-btn-modal').click(function () {
                sam.asset.LerFornecedorInformado();
            });
            $('#fecharModalViewGrid').click(function () {
                $('.modalViewGrid').modal('hide');
                //Carrega novamente pois a modal mata o evento dos checkbox da pagina principal
                sam.asset.CarregaCheckBox();
            });
        });
    },
    paginacaoGridFornecedor: function () {
        $('.pagination li a').click(function () {
            var sortOrder;
            var searchString = $('.search-tables').val();
            var currentFilter;

            var href = $(this).attr('href');
            var page = parseInt(sam.utils.getURLParameter(href, 'page'));

            $('.divViewGrid').load(sam.path.webroot + "/Assets/GridFornecedor", { sortOrder: sortOrder, searchString: searchString, currentFilter: currentFilter, page: page }, function () {
                $(sam.asset.consulta).val(searchString);
                $('.divViewGrid').show();
                sam.asset.paginacaoGridFornecedor();
            });

            return false;
        });
    },
    LerFornecedorInformado: function () {
        var cpfCnpjFornecedor = $('#result-tables tbody tr td').find('.radioSelecionado:checked')[0];

        if (cpfCnpjFornecedor != undefined) {
            $("#SupplierCPFCNPJ").val(cpfCnpjFornecedor.id);
            sam.asset.ConsultaFornecedorInformado(cpfCnpjFornecedor.id);
            $('.modalViewGrid').modal('hide');
            //Carrega novamente pois a modal mata o evento dos checkbox da pagina principal
            sam.asset.CarregaCheckBox();
        }
    },
    ConsultaFornecedorInformado: function (cpfCnpjFornecedor) {
        $.blockUI({ message: $('#modal-loading') });
        $.get(sam.path.webroot + "Assets/PesquisaPorFornecedor", { FornecedorPesquisa: cpfCnpjFornecedor }, function (data) {

            try {
                if (data[0].Mensagem == undefined) {
                    var retorno = JSON.parse(data);
                    $("#MaterialGroupNaoCadastrado").val('');
                    $('#SupplierCPFCNPJ').val(retorno.SupplierCPFCNPJ);
                    $('#SupplierName').val(retorno.SupplierName);
                    $('#SupplierId').val(retorno.SupplierId);
                    $('#fornecedorpesquisa').text('');


                    $.unblockUI({ message: $('#modal-loading') });
                }
                else {
                    var retorno2 = (data[0].Mensagem.length < 1);

                    $('#SupplierCPFCNPJ').val('');
                    $('#SupplierName').val('');
                    $('#SupplierId').val('');

                    $('#modalMaterialGroupNaoCadastrado').modal('show');
                    $('#mensagemmodal').html(data[0].Mensagem);
                    $("#MaterialGroupNaoCadastrado").val('true');

                    $.unblockUI({ message: $('#modal-loading') });
                    return retorno2;
                }
            }
            catch (e) {
                // Possivelmente o perfil de usuário esta sem permisão de acesso.
                $.unblockUI({ message: $('#modal-loading') });
                $('#modalMaterialGroupNaoCadastrado').modal('show');
                $('#mensagemmodal').html("1: Não foi possivel realizar a pesquisa do Fornecedor, favor contatar o administrador responsável!");
            }
        }).fail(function () {
            $.unblockUI({ message: $('#modal-loading') });
            $('#modalMaterialGroupNaoCadastrado').modal('show');
            $('#mensagemmodal').html("2: Não foi possivel realizar a pesquisa do Fornecedor, favor contatar o administrador responsável!");
        });
    },

    ConsultarTerceiro: function () {
        $('#btnConsultarTerceiro').click(function () {
            sam.asset.limparModalDivGrid();
            sam.asset.TerceiroShow();
        });
    },
    TerceiroShow: function () {
        sam.asset.CriarDivGrid("Terceiro");
        $('.divViewGrid').load(sam.path.webroot + "/Assets/GridTerceiro", { InstitutionId: $("#InstitutionId").val(), BudgetUnitId: $("#BudgetUnitId").val() }, function () {
            $('.modalViewGrid').modal('show');
            sam.asset.paginacaoGridTerceiro();
            $('#save-btn-modal').click(function () {
                sam.asset.LerTerceiroInformado();
            });
            $('#fecharModalViewGrid').click(function () {
                $('.modalViewGrid').modal('hide');
                //Carrega novamente pois a modal mata o evento dos checkbox da pagina principal
                sam.asset.CarregaCheckBox();
            });
        });
    },
    paginacaoGridTerceiro: function () {
        $('.pagination li a').click(function () {
            var sortOrder;
            var searchString = $('.search-tables').val();
            var currentFilter;

            var href = $(this).attr('href');
            var page = parseInt(sam.utils.getURLParameter(href, 'page'));

            $('.divViewGrid').load(sam.path.webroot + "/Assets/GridTerceiro", { InstitutionId: $("#InstitutionId").val(), BudgetUnitId: $("#BudgetUnitId").val() }, function () {
                $(sam.asset.consulta).val(searchString);
                $('.divViewGrid').show();
                $('#save-btn-modal').click(function () { sam.asset.LerTerceiroInformado(); });
                sam.asset.paginacaoGridTerceiro();
            });

            return false;
        });
    },
    LerTerceiroInformado: function () {
        var cpfCnpjTerceiro = $('#result-tables tbody tr td').find('.radioSelecionado:checked')[0];

        if (cpfCnpjTerceiro != undefined) {
            sam.asset.ConsultaTerceiroInformado(cpfCnpjTerceiro.id);
            $('.modalViewGrid').modal('hide');
            //Carrega novamente pois a modal mata o evento dos checkbox da pagina principal
            sam.asset.CarregaCheckBox();
        }
    },
    ConsultaTerceiroInformado: function (cpfCnpjTerceiro) {
        $.blockUI({ message: $('#modal-loading') });
        $.get(sam.path.webroot + "Assets/PesquisaPorTerceiro", { OutSourcedPesquisa: cpfCnpjTerceiro }, function (data) {

            try {
                if (data.Erro != undefined) {
                    $('#CPFCNPJDoTerceiro').val('');
                    $('#NomeDoTerceiro').val('');
                    $('#OutSourcedId').val('');

                    $('#modalMaterialGroupNaoCadastrado').modal('show');
                    $('#mensagemmodal').html(data.Erro);
                    $("#MaterialGroupNaoCadastrado").val('true');

                } else {
                    var retorno = JSON.parse(data);
                    $("#MaterialGroupNaoCadastrado").val('');
                    $('#CPFCNPJDoTerceiro').val(retorno.CPFCNPJ);
                    $('#NomeDoTerceiro').val(retorno.Name);
                    $('#OutSourcedId').val(retorno.Id);
                    $('#Terceiropesquisa').text('');
                    sam.asset.mascaraCPFCNPJDoTerceiro();
                }

                $.unblockUI({ message: $('#modal-loading') });
            }
            catch (e) {
                // Possivelmente o perfil de usuário esta sem permisão de acesso.
                $.unblockUI({ message: $('#modal-loading') });
                $('#modalMaterialGroupNaoCadastrado').modal('show');
                $('#mensagemmodal').html("1: Não foi possivel realizar a pesquisa do Terceiro, favor contatar o administrador responsável!");
            }
        }).fail(function () {
            $.unblockUI({ message: $('#modal-loading') });
            $('#modalMaterialGroupNaoCadastrado').modal('show');
            $('#mensagemmodal').html("2: Não foi possivel realizar a pesquisa do Terceiro, favor contatar o administrador responsável!");
        });
    },

    BuscarEmpenho: function () {
        $("#btnConsultarEmpenho").click(function () {
            if ($("#numeroEmpenho").val() != "") {
                if ($("#ManagerUnitId").val() != "0") {
                    $.blockUI({ message: $('#modal-loading') });
                    sam.asset.limparModalDivGrid();
                    sam.asset.EmpenhoShow();
                }
                else {
                    $('#modalMaterialGroupNaoCadastrado').modal('show');
                    $('#mensagemmodal').html("Favor informar a UGE antes de realizar a pesquisa!");
                }
            }
            else {
                $('#modalMaterialGroupNaoCadastrado').modal('show');
                $('#mensagemmodal').html("Favor informar o Empenho para realizar a pesquisa!");
            }
        });
    },


    BuscarEmpenhoRestosAPagar: function () {
        $("#btnConsultarEmpenhoRestosAPagar").click(function () {
            if ($("#numeroEmpenho").val() != "") {
                if ($("#ManagerUnitId").val() != "0") {

                    $.unblockUI({ message: $('#modal-loading') });

                    $.get(sam.path.webroot + "Assets/GetEmpenhoRestosAPagar", { codigoUGE: $("#ManagerUnitId").val(), codigoGestao: $("#Ug").val(), numeroEmpenho: $("#numeroEmpenho").val() }, function (data) {

                        if (data[0].Mensagem == undefined) {
                            var _empenho = JSON.parse(data);

                            $("#EmpenhoResultado").val(_empenho.Empenho);
                            $("#hiddenEmpenho").val(_empenho.Empenho);
                            $("#Empenho").val(_empenho.Empenho);

                            //Bloqueia o campo empenho da view Create
                            $('#Empenho').addClass("Empenho");
                            $('#Empenho').attr("readonly", "readonly");
                        }
                        else {
                            $("#EmpenhoResultado").val('');
                            $("#hiddenEmpenho").val('');
                            $("#Empenho").val('');


                            $('#mensagemmodal').html(data[0].Mensagem);
                            $('#modalMaterialGroupNaoCadastrado').modal('show');
                        }

                        $.unblockUI({ message: $('#modal-loading') });
                    });
                }
                else {
                    $('#modalMaterialGroupNaoCadastrado').modal('show');
                    $('#mensagemmodal').html("Favor informar a UGE antes de realizar a pesquisa!");
                }
            }
            else {
                $('#modalMaterialGroupNaoCadastrado').modal('show');
                $('#mensagemmodal').html("Favor informar o Empenho para realizar a pesquisa!");
            }
        });
    },
    EmpenhoShow: function () {

        $.get(sam.path.webroot + "Assets/GetEmpenhoMaterial", { codigoUGE: $("#ManagerUnitId").val(), codigoGestao: $("#Ug").val(), numeroEmpenho: $("#numeroEmpenho").val(), }, function (data) {

            if (data[0].Mensagem == undefined) {
                var _empenho = JSON.parse(data);
                sam.asset.CriarDivGrid("Itens Materiais");
                $.unblockUI({ message: $('#modal-loading') });
                $('.divViewGrid').load(sam.path.webroot + "Assets/GridMaterialEmpenho", { empenho: _empenho }, function (data) {
                    $('.modalViewGrid').modal('show');
                    $('#save-btn-modal').click(function () {
                        sam.asset.LerEmpenhoInformado(_empenho);
                    });
                    $('#fecharModalViewGrid').click(function () {
                        $('.modalViewGrid').modal('hide');
                        //Carrega novamente pois a modal mata o evento dos checkbox da pagina principal
                        sam.asset.CarregaCheckBox();
                    });
                });
            }
            else {
                var retorno2 = (data[0].Mensagem.length < 1);
                $('#Empenho').val('');
                $('#EmpenhoResultado').val('');
                $('#MaterialItemCode').val('');
                $('#MaterialItemDescription').val('');
                $('#MaterialGroupCode').val('');
                $('#MaterialGroupDescription').val('');
                $('#LifeCycle').val('');
                $('#RateDepreciationMonthly').val('');
                $('#ResidualValue').val('');
                $('#ShortDescription').val('');
                $('#SupplierId').val('');
                $('#SupplierName').val('');

                $('#modalMaterialGroupNaoCadastrado').modal('show');
                $('#mensagemmodal').html(data[0].Mensagem);
                $("#MaterialGroupNaoCadastrado").val('true');
                $.unblockUI({ message: $('#modal-loading') });
                return retorno2;
            }
        });
    },
    LerEmpenhoInformado: function (_empenho) {
        var ItemMaterial = $('#result-tables tbody tr td').find('.radioSelecionado:checked')[0];

        if (ItemMaterial != undefined) {
            sam.asset.ConsultaItemMaterialEmpenho(ItemMaterial.id, _empenho);
            $('.modalViewGrid').modal('hide');
            //Carrega novamente pois a modal mata o evento dos checkbox da pagina principal
            sam.asset.CarregaCheckBox();
        }
    },
    ConsultaItemMaterialEmpenho: function (ItemMaterial, _empenho) {
        if (ItemMaterial != "") {
            $.blockUI({ message: $('#modal-loading') });
            $.get(sam.path.webroot + "Assets/GetItemMaterial", { materialItemCode: ItemMaterial }, function (data) {

                try {
                    if (data[0].Mensagem == undefined) {
                        var retorno = JSON.parse(data);

                        $("#MaterialGroupNaoCadastrado").val('');
                        $('#MaterialItemCode').val(retorno.MaterialItemCode);
                        $('#MaterialItemDescription').val(retorno.MaterialItemDescription);
                        $('#MaterialGroupCode').val(retorno.MaterialGroupCode);
                        $('#MaterialGroupDescription').val(retorno.MaterialGroupDescription);
                        $('#LifeCycle').val(retorno.LifeCycle);
                        $('#LifeCycle').val($('#LifeCycle').val().replace('.', ','));
                        $('#RateDepreciationMonthly').val(retorno.RateDepreciationMonthly);
                        $('#RateDepreciationMonthly').val($('#RateDepreciationMonthly').val().replace('.', ','));
                        $('#ResidualValue').val(retorno.ResidualValue);
                        $('#ResidualValue').val($('#ResidualValue').val().replace('.', ','));

                        sam.materialItem.VerificarDescricaoResumida(retorno.ShortDescription);
                        $('#Empenho').val(_empenho.Empenho);
                        $('#hiddenEmpenho').val(_empenho.Empenho);
                        $('#EmpenhoResultado').val(_empenho.Empenho);
                        
                        //Bloqueia o campo empenho da view Create
                        $('#Empenho').addClass("Empenho");
                        $('#Empenho').attr("readonly", "readonly");
        
                        sam.materialItem.consultaContaContabilPorGrupoMaterial(retorno.MaterialGroupCode);
                        sam.materialItem.VerificarFornecedor(_empenho.CgcCpf, _empenho.DescricaoFornecedor);

                        $.unblockUI({ message: $('#modal-loading') });
                    }
                    else {
                        var retorno2 = (data[0].Mensagem.length < 1);
                        $('#Empenho').val('');
                        $('#EmpenhoResultado').val('');
                        $('#hiddenEmpenho').val('');
                        $('#MaterialItemCode').val('');
                        $('#MaterialGroupCode').val('');
                        $('#MaterialGroupDescription').val('');
                        $('#LifeCycle').val('');
                        $('#RateDepreciationMonthly').val('');
                        $('#ResidualValue').val('');
                        $('#ShortDescription').val('');
                        $('#SupplierId').val('');
                        $('#SupplierName').val('');

                        $('#modalMaterialGroupNaoCadastrado').modal('show');
                        $('#mensagemmodal').html(data[0].Mensagem);
                        $("#MaterialGroupNaoCadastrado").val('true');
                        $("#AuxiliaryAccountId").empty().append('<option class="filter-option pull-left" value> -- Selecione -- </option>');
                        $.unblockUI({ message: $('#modal-loading') });
                        return retorno2;
                    }
                }
                catch (e) {
                    // Possivelmente o perfil de usuário esta sem permisão de acesso.
                    $.unblockUI({ message: $('#modal-loading') });
                    $('#modalMaterialGroupNaoCadastrado').modal('show');
                    $('#mensagemmodal').html("1: Não foi possivel realizar a pesquisa do Item material, favor contatar o administrador responsável!");
                }
            }).fail(function () {
                $.unblockUI({ message: $('#modal-loading') });
                $('#modalMaterialGroupNaoCadastrado').modal('show');
                $('#mensagemmodal').html("2: Não foi possivel realizar a pesquisa do Item material, favor contatar o administrador responsável!");
            });
        }
        else {
            $('#modalMaterialGroupNaoCadastrado').modal('show');
            $('#mensagemmodal').html("Favor informar o código do Item Material para realizar a pesquisa!");
        }
    },

    FiltraPesquisaFornecedores: function () {
        var sortOrder;
        var searchString = $('#search-tables-filtro').val();
        var currentFilter;
        var page = 1;

        $('.divViewGrid').load(sam.path.webroot + "/Assets/GridFornecedor", { sortOrder: sortOrder, searchString: searchString, currentFilter: currentFilter, page: page }, function () {
            $(sam.asset.consulta).val(searchString);
            $('.divViewGrid').show();
            sam.asset.paginacaoGridFornecedor();
        });
    },

    FiltraPesquisaTerceiros: function () {
        var sortOrder;
        var searchString = $('#search-tables-filtro').val();
        var currentFilter;
        var page = 1;

        $('.divViewGrid').load(sam.path.webroot + "/Assets/GridTerceiro", { sortOrder: sortOrder, searchString: searchString, currentFilter: currentFilter, page: page }, function () {
            $(sam.asset.consulta).val(searchString);
            $('.divViewGrid').show();
            $('#save-btn-modal').click(function () { sam.asset.LerTerceiroInformado(); });
            sam.asset.paginacaoGridTerceiro();
        });
    },

    FiltraPesquisaDocumento: function () {

        var sortOrder;
        var searchString = $('#search-tables-filtro').val();
        var currentFilter;
        var page = 1;
        var tipoTransferencia = $("#EnumMovimentoAPesquisar").val();

        $('.divViewGrid').load(sam.path.webroot + "/Assets/GridDocumento", { tipoTransferencia: tipoTransferencia, sortOrder: sortOrder, searchString: searchString, currentFilter: currentFilter, page: page }, function () {
            $(sam.asset.consulta).val(searchString);
            $('.divViewGrid').show();
            $('#save-btn-modal').click(function () { sam.asset.LerDocumentoInformado(tipoTransferencia); });
            sam.asset.paginacaoGridDocumento(tipoTransferencia);
        });
    },

    AdicionaMarscaraUpdateValue: function () {
        $("input[type=text][class~=sam_moeda_updatevalue]:not([class~=mascaraImplementada_updatevalue])").maskMoney({ showSymbol: false, symbol: "R$", decimal: ",", thousands: "." });
        $('.sam_moeda_updatevalue').addClass('mascaraImplementada_updatevalue');
    },

    EventoTravaGridsMovimentacaoInventario: function () {
        var origemPaginaInventario = $('#OrigemInventario').val();

        if (origemPaginaInventario != null && origemPaginaInventario == "True") {
            $('#MovementTypeId').attr('disabled', 'disabled');
            $('#btnFiltrar').attr('disabled', 'disabled');
            $('#Observation').attr('disabled', 'disabled');
            $('.addBem').attr('disabled', 'disabled');
            $('.addTodos').attr('disabled', 'disabled');
            $('.removerBem').attr('disabled', 'disabled');
            $('.removerTodos').attr('disabled', 'disabled');
        }
    },

    setHiddenValuesOnLoad: function () {
        if ($('#hiddenEmpenho') != null)
            $('#hiddenEmpenho').val('');
    },
    verificaBloqueioCampoEmpenho: function () {
        if ($('#hiddenEmpenho').val() != "") {
            $('#Empenho').addClass("Empenho");
            $('#Empenho').attr("readonly", "readonly");
        }
    },
    CriarDivTesteXML: function (xml) {

        if (xml != undefined && xml != '') {

                var htmlBuilder = [];

                htmlBuilder.push('<div class="modal fade" id="modalViewTeste" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel">');
                htmlBuilder.push('<div class="modal-dialog" role="document">');
                htmlBuilder.push('    <div class="modal-content">');
                htmlBuilder.push('        <div class="modal-header">');
                htmlBuilder.push('            <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>');
                htmlBuilder.push('            <h4 class="modal-title" id="exampleModalLabel">Consultar Item de Material</h4>');
                htmlBuilder.push('        </div>');
                htmlBuilder.push('        <div class="modal-body">');
                htmlBuilder.push('          <div class="form-group divView" id="partialView">' + xml);
                htmlBuilder.push('          </div>');
                htmlBuilder.push('        </div>');
                htmlBuilder.push('        <div class="modal-footer">');
                htmlBuilder.push('            <button type="button"  data-toggle="tooltip" data-placement="top" class="btn btn-primary buttonClose" onclick="sam.asset.FechaELimpaModalTeste()" data-dismiss="modal">Fechar</button>');
                htmlBuilder.push('        </div>');
                htmlBuilder.push('    </div>');
                htmlBuilder.push('</div>');
                htmlBuilder.push('</div>');
                htmlBuilder.join("");


                $(document.body).append(htmlBuilder.join(""));

            $("#modalViewTeste").modal('show');
        }
    },
    FechaELimpaModalTeste: function () {
        $('#partialView').innerHTML = '';
    },
    Sucesso: function (msg) {
        if (msg != null && msg != undefined && msg != '') {
            alert(msg);
            location.href = "/Patrimonio/Movimento";
        }
    },
    CarregaCamposMudancaRevalorizacao: function() {
        //Acervo e Terceiro - Quando recarrega a pagina 
        if ($('#checkFlagAcervo').is(':checked')) {
            $('#div_Acervo').css('display', 'block');
            $('#checkFlagTerceiro').iCheck('uncheck');
            $('#div_Depreciacao').css('display', 'none');
        }
        else if ($('#checkFlagTerceiro').is(':checked')) {
            $('#div_Acervo').css('display', 'block');
            $('#checkFlagAcervo').iCheck('uncheck');
            $('#div_Depreciacao').css('display', 'none');
        }
        else {
            $('#div_Acervo').css('display', 'none');
            $('#div_Material').css('display', 'block');
            $('#div_Depreciacao').css('display', 'block');
        }

        //Acervo - Evento de click no check box
        $('#checkFlagAcervo').on('ifChanged', function (event) {
            if (this.checked) {
                $('#div_Acervo').css('display', 'block');
                $('#div_pesquisaItemMaterial').css('display', 'none');
                sam.commun.limpar('#div_Acervo');
                $('#checkFlagTerceiro').iCheck('uncheck');
                $('#div_Depreciacao').css('display', 'none');
                $('#msgAnimalAServico').css("display", "none");
                sam.asset.CamposDepreciacao(0);
                sam.asset.PreencheItemMaterialParaAcervo();
                sam.materialItem.consultaContaContabilPorTipo(1);
            }
            else if (!$('#checkFlagTerceiro')[0].checked) {
                $('#div_Acervo').css('display', 'none');
                $('#div_pesquisaItemMaterial').css('display', 'block');
                $('#ShortDescription').val('');
                $('span[data-valmsg-for="ShortDescription"]').text('');
                $('#msgAnimalAServico').css("display", "none");
                sam.commun.limpar('#div_Acervo');
                $('#div_Depreciacao').css('display', 'block ');
                sam.asset.CamposDepreciacao('');
                sam.asset.LimpaCamposItemMaterial('');
            }
        });

        //Terceiro - Evento de click no check box
        $('#checkFlagTerceiro').on('ifChanged', function (event) {
            if (this.checked) {
                $('#div_Acervo').css('display', 'block');
                $('#div_pesquisaItemMaterial').css('display', 'none');
                sam.commun.limpar('#div_Acervo');
                $('#checkFlagAcervo').iCheck('uncheck');
                $('#div_Depreciacao').css('display', 'none');
                $('#msgAnimalAServico').css("display", "none");
                sam.asset.CamposDepreciacao(0);
                sam.asset.PreencheItemMaterialParaTerceiro();
                sam.materialItem.consultaContaContabilPorTipo(2);
            }
            else if (!$('#checkFlagAcervo')[0].checked) {
                $('#div_Acervo').css('display', 'none');
                $('#div_pesquisaItemMaterial').css('display', 'block');
                $('#ShortDescription').val('');
                $('span[data-valmsg-for="ShortDescription"]').text('');
                $('#msgAnimalAServico').css("display", "none");
                sam.commun.limpar('#div_Acervo');
                $('#div_Depreciacao').css('display', 'block ');
                sam.asset.CamposDepreciacao('');
                sam.asset.LimpaCamposItemMaterial('');
            }
        });
    },
    CarregaCamposInventarioInicial: function () {
        //Acervo e Terceiro - Quando recarrega a pagina 
        if ($('#checkFlagAcervo').is(':checked')) {
            $('#div_Acervo').css('display', 'block');
            $('#checkFlagTerceiro').iCheck('uncheck');
            $('#checkFlagDecretoSefaz').iCheck('uncheck');
            $('#div_Depreciacao').css('display', 'none');
        }
        else if ($('#checkFlagTerceiro').is(':checked')) {
            $('#div_Acervo').css('display', 'block');
            $('#checkFlagAcervo').iCheck('uncheck');
            $('#checkFlagDecretoSefaz').iCheck('uncheck');
            $('#div_Depreciacao').css('display', 'none');
        } else if ($('#checkFlagDecretoSefaz').is(':checked')) {
            //parte cima
            $('#div_Acervo').css('display', 'block');

            //parte baixo
            $('#div_pesquisaItemMaterial').css('display', 'block');
            $('#div_Depreciacao').css('display', 'block');

            $('#checkFlagAcervo').iCheck('uncheck');
            $('#checkFlagTerceiro').iCheck('uncheck');
        }
        else {
            $('#div_Acervo').css('display', 'none');
            $('#div_pesquisaItemMaterial').css('display', 'block');
            $('#div_Depreciacao').css('display', 'block');
        }

        //Acervo - Evento de click no check box
        $('#checkFlagAcervo').on('ifChanged', function (event) {
            if (this.checked) {
                $('#div_Acervo').css('display', 'block');
                $('#div_pesquisaItemMaterial').css('display', 'none');
                sam.commun.limpar('#div_Acervo');
                $('#checkFlagTerceiro').iCheck('uncheck');
                $('#checkFlagDecretoSefaz').iCheck('uncheck');
                $('#div_Depreciacao').css('display', 'none');
                $('#msgAnimalAServico').css("display", "none");
                sam.asset.CamposDepreciacao(0);
                sam.asset.PreencheItemMaterialParaAcervo();
                sam.materialItem.consultaContaContabilPorTipo(1);
            }
            else if (!$('#checkFlagTerceiro')[0].checked && !$('#checkFlagDecretoSefaz')[0].checked) {
                $('#div_Acervo').css('display', 'none');
                $('#div_pesquisaItemMaterial').css('display', 'block');
                $('#ShortDescription').val('');
                $('span[data-valmsg-for="ShortDescription"]').text('');
                sam.commun.limpar('#div_Acervo');
                $('#div_Depreciacao').css('display', 'block ');
                $('#msgAnimalAServico').css("display", "none");
                sam.asset.CamposDepreciacao('');
                sam.asset.LimpaCamposItemMaterial('');
            } else if ($('#checkFlagDecretoSefaz')[0].checked) {
                sam.asset.LimpaCamposItemMaterial('');
                $('#ShortDescription').val('');
            }
        });

        //Terceiro - Evento de click no check box
        $('#checkFlagTerceiro').on('ifChanged', function (event) {
            if (this.checked) {
                $('#div_Acervo').css('display', 'block');
                $('#div_pesquisaItemMaterial').css('display', 'none');
                sam.commun.limpar('#div_Acervo');
                $('#checkFlagAcervo').iCheck('uncheck');
                $('#checkFlagDecretoSefaz').iCheck('uncheck');
                $('#div_Depreciacao').css('display', 'none');
                $('#msgAnimalAServico').css("display", "none");
                sam.asset.CamposDepreciacao(0);
                sam.asset.PreencheItemMaterialParaTerceiro();
                sam.materialItem.consultaContaContabilPorTipo(2);
            }
            else if (!$('#checkFlagAcervo')[0].checked && !$('#checkFlagDecretoSefaz')[0].checked) {
                $('#div_Acervo').css('display', 'none');
                $('#div_pesquisaItemMaterial').css('display', 'block');
                $('#ShortDescription').val('');
                $('span[data-valmsg-for="ShortDescription"]').text('');
                sam.commun.limpar('#div_Acervo');
                $('#div_Depreciacao').css('display', 'block ');
                $('#msgAnimalAServico').css("display", "none");
                sam.asset.CamposDepreciacao('');
                sam.asset.LimpaCamposItemMaterial('');
            } else if ($('#checkFlagDecretoSefaz')[0].checked) {
                sam.asset.LimpaCamposItemMaterial('');
                $('#ShortDescription').val('');
            }
        });

        //Decreto - Evento de click no check box
        $('#checkFlagDecretoSefaz').on('ifChanged', function (event) {
            if (this.checked) {
                //parte cima
                $('#div_Acervo').css('display', 'block');

                //parte baixo
                $('#div_pesquisaItemMaterial').css('display', 'block');
                $('#div_Depreciacao').css('display', 'block');

                //checkboxes
                $('#checkFlagAcervo').iCheck('uncheck');
                $('#checkFlagTerceiro').iCheck('uncheck');

                if ($("#MaterialGroupCode").val() != 88) {
                    $('#msgAnimalAServico').css("display", "none");
                }

            } else if (!$('#checkFlagAcervo')[0].checked && !$('#checkFlagTerceiro')[0].checked) {
                $('#div_Acervo').css('display', 'none');
                $('#div_pesquisaItemMaterial').css('display', 'block');
                $('#ShortDescription').val('');
                $('span[data-valmsg-for="ShortDescription"]').text('');
                sam.commun.limpar('#div_Acervo');
                $('#div_Depreciacao').css('display', 'block ');
                $('#msgAnimalAServico').css("display", "none");
                sam.asset.CamposDepreciacao('');
                sam.asset.LimpaCamposItemMaterial('');
            }
        });
    },
    CarregaCampoLoteChapa: function () {
        //Sequencia Chapa
        if ($('#checkLoteChapa').is(':checked')) {
            document.getElementById('EndNumberIdentification').readOnly = false;
        }
        else {
            document.getElementById('EndNumberIdentification').readOnly = true;
            $("#EndNumberIdentification").val('');
            $("#EndNumberIdentification").text('');
            $('span[data-valmsg-for="EndNumberIdentification"]').text('');
        }

        $('#checkLoteChapa').on('ifChanged', function (event) {
            if (this.checked) {
                document.getElementById('EndNumberIdentification').readOnly = false;
            }
            else {
                document.getElementById('EndNumberIdentification').readOnly = true;
                $("#EndNumberIdentification").val('');
                $("#EndNumberIdentification").text('');
                $('span[data-valmsg-for="EndNumberIdentification"]').text('');
            }
        });
    },
    BloqueiaCampoLoteChapa: function () {
        $('#checkLoteChapa').attr('disabled', 'disabled');
        document.getElementById('EndNumberIdentification').readOnly = true;
        $("#EndNumberIdentification").val('');
        $("#EndNumberIdentification").text('');
        $('span[data-valmsg-for="EndNumberIdentification"]').text('');
    },
    UGEIntegradaSemLoginSiafem: function () {
        $('#SaveLoginSiafem').click(function () {

            if ($('#CPFSIAFEMModal').val().length != 11) {
                alert('Digite os 11 números do CPF');
                return false;
            }

            if ($('#LoginSiafem').val() == "" || $('#SenhaSiafem').val() == "") {
                $('#LoginSiafem').val($('#CPFSIAFEMModal').val());
                $('#SenhaSiafem').val($('#SenhaSIAFEMModal').val());

                if ($('#LoginSiafem').val() == "" || $('#SenhaSiafem').val() == "") {
                    return false;
                }
            }
        });

        if ($('#CPFSIAFEMModal').val() == "" || $('#SenhaSIAFEMModal').val() == "") {
            $('[data-toggle="tooltip"]').tooltip();
            $('#modalLoginSiafem').modal({ keyboard: false, backdrop: 'static', show: true });
        }
    },
    MostraExtratoSIAFEM: function (ids) {
        $("#modalExtratoSIAFEM").modal({ keyboard: false, backdrop: 'static', show: true });
        sam.asset.AbortarSIAFEM($("#Ids").val());
        sam.asset.ProsseguirSIAFEM($("#Ids").val());
    },
    ProsseguirSIAFEM: function (numero) {
        $("#prosseguir").click(function (e) {
            e.target.disabled = true;
            $.blockUI({ message: $('#modal-loading') });
            $.post(sam.path.webroot + "Assets/Prosseguir",
            { auditorias: numero, LoginSiafem: $("#LoginSiafem").val(), SenhaSiafem: $("#SenhaSiafem").val() },
            function () { }).done(function (res) {
                $.unblockUI({ message: $('#modal-loading') });
                $("#retornoSIAFEM").html(res);
                $("#modalRetornoSIAFEM").modal({ keyboard: false, backdrop: 'static', show: true });
                sam.asset.GerarPendenciaSIAFEM(numero, $("#LoginSiafem").val(), $("#SenhaSiafem").val());
                sam.asset.AbortarAposProsseguirSIAFEM(numero);
                $("#ok").click(function () {
                    window.location.href = "/Patrimonio/Movimento";
                });
            });
        });
    },
    GerarPendenciaSIAFEM: function (numero, login, senha) {
        $("#gerar_pendencias").click(function (e) {
            e.target.disabled = true;
            $.blockUI({ message: $('#modal-loading') });
            $.post(sam.path.webroot + "Assets/GerarPendenciaSIAFEM",
                {
                    auditorias: numero, LoginSiafem: login, SenhaSiafem: senha
                }, function () { }).done(function (res) {
                    $.unblockUI({ message: $('#modal-loading') });
                window.location.href = "/Patrimonio/Movimento";
                }).fail(function () {
                    window.location.href = "/Patrimonio/Movimento";
                });

        });
    },
    AbortarSIAFEM: function (numero) {
        $("#abortar").click(function (e) {
            e.target.disabled = true;
            $.blockUI({ message: $('#modal-loading') });
            $.post(sam.path.webroot + "Movimento/AbortarIncorporacao", { auditorias: numero }, function () { }).done(function (res) {
                $.unblockUI({ message: $('#modal-loading') });
                window.location.href = "/Patrimonio/Movimento";
            }).fail(function () {
                $.unblockUI({ message: $('#modal-loading') });
                alert('Houve um erro ao abortar o processo.Estorne a movimentação pela tela Visão Geral');
                window.location.href = "/Patrimonio/Movimento";
            });

        });
    },
    AbortarAposProsseguirSIAFEM: function (numero) {
        $("#abortarApos").click(function (e) {
            e.target.disabled = true;
            $.blockUI({ message: $('#modal-loading') });
            $.post(sam.path.webroot + "Movimento/AbortarIncorporacao", { auditorias: numero }, function () { }).done(function (res) {
                $.unblockUI({ message: $('#modal-loading') });
                window.location.href = "/Patrimonio/Movimento";
            }).fail(function () {
                $.unblockUI({ message: $('#modal-loading') });
                alert('Houve um erro ao abortar o processo.Estorne a movimentação pela tela Visão Geral');
                window.location.href = "/Patrimonio/Movimento";
            });

        });
    },
    mascaraCPFCNPJDoTerceiro: function(){
        try {
            $("#CPFCNPJDoTerceiro").unmask();
            var tamanho = $("#CPFCNPJDoTerceiro").val().replace(/\D/g, '').length;
            if (tamanho == 11) {
                $("#CPFCNPJDoTerceiro").mask("999.999.999-99");
            } else if (tamanho == 14) {
                $("#CPFCNPJDoTerceiro").mask("99.999.999/9999-99");
            }
        } catch (e) {

        }
    },
    Edicao: {
        load: function (numero) {
            this.pesquisaItemMaterial(numero);
            this.submitForm();
            this.mascaraCPFCNPJDoTerceiro();
            $("#scriptEdicao").remove();
        },
        pesquisaItemMaterial: function (numero) {
            if ($("#btnConsultar").length > 0) {
                $("#btnConsultar").click(function (e) {
                    e.target.disabled = true;
                    $("#btnSalvarSubmit").attr('disabled', 'disabled');
                    $.get(sam.path.webroot + "Assets/BuscaItemMaterialParaAtualizacao", { numero: numero, item: $("#materialItemPesquisa").val() })
                     .success(function (res) {
                         if (res.erro !== undefined) {
                             alert(res.mensagem);
                         } else {
                             $("#MaterialItemCode").val(res.codigo);
                             $("#MaterialItemDescription").val(res.descricao);
                         }

                         $("#materialItemPesquisa").val("");
                         e.target.disabled = false;
                         $("#btnSalvarSubmit").removeAttr('disabled');
                     }).fail(function () {
                         alert('Ocorreu um imprevisto ao pesquisar o item material. Por gentileza, tente novamente mais tarde');
                     });
                });
            }
        },
        submitForm: function () {
            $("#formEdicaoBemPatrimonial").submit(function () {
                $("#MaterialItemCode").removeAttr("disabled");
                $("#MaterialItemDescription").removeAttr("disabled");
            })
        },
        mascaraCPFCNPJDoTerceiro: function () {
            if ($("#CPFCNPJDoTerceiro").length > 0) {
                sam.asset.mascaraCPFCNPJDoTerceiro();
            }
        }
    }
}