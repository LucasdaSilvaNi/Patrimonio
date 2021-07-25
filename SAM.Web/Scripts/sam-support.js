var support = {
    LoadIndex: function () {
        support.SubmitFiltrar();
        support.EventoTogglePanel();
        support.InicializaDataConsulta();
        support.MudaStatusProdesp();
        support.InicializaListaLote();
        support.LinkParaVideo();
    },
    Load: function () {
        support.EventoUpload();
        support.GetAttachments();
        support.GetHistorics();
        support.DesabilitaCampos();
        support.SubmitForm();
    },

    EventoUpload: function(){
        $('#btnUpload').click(function () {

            var arquivo = $('#evidenceFile').get(0).files[0];

            if (arquivo == undefined) {
                sam.commun.CriarAlertDinamico('Por favor, selecione um anexo');
                return false;
            }

            if (arquivo.size > 10000000) { //10MB
                sam.commun.CriarAlertDinamico('Sinto muito, mas o limite de upload de anexo é de 10 MB.');
                return false;
            }

            var formData = new FormData();
            formData.append("evidenceFile", $('#evidenceFile').get(0).files[0]);

            var _url = sam.path.webroot + "Support/Upload";
            
            $.ajax({
                url: _url,
                type: "POST",
                contentType: false,
                processData: false,
                data: formData,
                dataType: "html",
                //mimeType: "multipart/form-data",
                success: function (data) {
                    
                    var dados = JSON.parse(data);

                    if (dados.tipo == 'sucesso') {
                        support.GetAttachments();
                    }
                    else if (dados.tipo == 'erro') {
                        sam.commun.CriarAlertDinamico(dados.mensagem);
                    }
                },
                error: function (xhr, status, p3, p4) {
                    sam.commun.CriarAlertDinamico('Erro ao carregar a rotina EventoUpload.');
                }
            });
        });
    },

    EventoTogglePanel: function () {
        $('#btnOcultaFiltro').click(function () {
            var h = $(window).height();
            var hr;
            var hd;

            if ($(".panel-filter").is(":visible")) {

                hr = 295;
                hd = (h - hr) + 'px';
            }
            else {

                hr = 610;
                hd = (h - hr) + 'px';
            }

            $(".panel-filter").toggle("blind", 500);
        });
    },

    GetAttachments: function () {
        var url = sam.path.webroot + "/Support/GetAttachments";

        $.get(url, {
        }, function () {
        }).done(function (data) {
            $('#partialViewAttachment').html(data);

        }).fail(function () {
            sam.commun.CriarAlertDinamico('Erro ao carregar a rotina CarregaDadosPatrimoniosDisponiveisGrid.');
        });
    },

    GetHistorics: function(){
        var url = sam.path.webroot + "/Support/GetHistoric";
        var _supportId = $('#Id').val();

        $.get(url, {
            supportId: _supportId
        }, function () {
        }).done(function (data) {
            $('#partialViewHistoric').html(data);

        }).fail(function () {
            sam.commun.CriarAlertDinamico('Erro ao carregar a rotina GetHistorics.');
        });
    },

    //Rotinas usadas para submeter valores que se encontram desabilitados na tela -----------------------------------
    HabilitaCampos: function(){
    
        $('#UserDescription').removeAttr('disabled');
        $('#UserCPF').removeAttr('disabled');
        $('#UserPerfil').removeAttr('disabled');
    },

    DesabilitaCampos: function () {
        $('#UserDescription').attr('disabled', 'disabled');
        $('#UserCPF').attr('disabled', 'disabled');
        $('#UserPerfil').attr('disabled', 'disabled');
    },

    SubmitFiltrar: function(){
        $('#btnFiltrar').click(function () {
            
            $('#InstitutionId').val($('#InstitutionIdFake').val());
            $('#BudgetUnitId').val($('#BudgetUnitIdFake').val());
            $('#ManagerUnitId').val($('#ManagerUnitIdFake').val());

            $('#SupportStatusProdespId').val($('#SupportStatusProdespsFake').val());
            $('#SupportStatusUserId').val($('#SupportStatusUserIdFake').val());
            $('#SupportId').val($('#SupportIdFake').val());

            $('#consultaForm').submit();
        });
    },

    SubmitForm: function () {
        $('#formSuporte').submit(function () {
            support.HabilitaCampos();
        });
    },
    InicializaDataConsulta: function () {
        let dataAtual = new Date();
        $('.datepicker').datepicker({ language: 'pt-BR', todayHighlight: true, autoclose: true, orientation: "bottom", endDate: dataAtual });
        $('.apagaData').click(function () {
            $('#DataInclusao').val("");
        });
    },
    MudaStatusProdesp: function () {
        $("#SupportStatusProdespsFake").change(function () {
            if ($(".qualAguardo").length > 0) {
                if ($(this).val() == 3) {
                    $(".qualAguardo").show();
                } else {
                    $(".qualAguardo").hide();
                }
            }
            return true;
        });
    },
    InicializaListaLote: function () {
        if ($("#btnPorLote").length > 0) {
            $('#listaLote').val(JSON.stringify([]));
            $("#btnResponderLote").hide();
            $("#btnResponderLote").click(function () {
                let valorCampo = $('#listaLote').val();
                let lista = JSON.parse(valorCampo);

                if (lista.length > 0) {
                    $("#partialView").empty();
                    $("#partialView").load(sam.path.webroot + "Support/ResponderLote",
                        {
                          listaLoteEscolhidos: lista,
                          listaString: $('#listaLote').val()
                        },
                    function () {
                        $("#modalViewGrid").modal('show');
                    });
                } else {
                    alert('Selecione pelo menos 1 chamado para responder!')
                }
            });
        }
    },
    LoteSalvoComSucesso: function () {
        $("#modalViewGrid").modal('hide');
        $("#btnPorLote").click();
        $("#btnFiltrar").click();
    },
    LinkParaVideo: function () {
        $("#chamadaVideo").click(function () {
            $.post(sam.path.webroot + "Principal/Video", { video : 1 }, function () { })
            .done(function (data) {
                $("#corpo").html(data);
                $("#modalVideoSobreChamado").modal({ keyboard: false, backdrop: 'static', show: true });
            });
        });
    }
}