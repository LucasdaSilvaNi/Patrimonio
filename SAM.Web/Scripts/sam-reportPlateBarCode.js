var reportPlateBarCode = {

    Load: function () {
        $(document).ready(function () {
            $(window).load(function () {
                reportPlateBarCode.EventoTogglePanel();
                reportPlateBarCode.InicializaDatePicker();
            });
        });
    },
    InicializaDatePicker: function () {
        $('.datepicker').datepicker({ language: 'pt-BR', todayHighlight: true, autoclose: true, orientation: 'bottom' });
    },
    CarregaDadosGrid: function () {

        if (reportPlateBarCode.ValidaGeracaoDeReport()) {
            var url = sam.path.webroot + "/PlateBarCode/LoadDataReportPlateBarCode";

            var _institutionId = $('#InstitutionId').val();
            var _budgetUnitId = $('#BudgetUnitId').val();
            var _managerUnitId = $('#ManagerUnitId').val();
            var _contenha = $('#Contenha').val();
            var _chapaInicioRef = $('#numChapaInicioRef').val();
            var _chapaFimRef = $('#numChapaFimRef').val();

            //Bloqueia a tela
            $.blockUI({ message: $('#modal-loading') });

            $.get(url, {
                institutionId: _institutionId, budgetUnitId: _budgetUnitId, managerUnitId: _managerUnitId, contenha: _contenha, chapaInicioRef: _chapaInicioRef, chapaFimRef: _chapaFimRef
            }, function () {
            }).done(function (data) {

                $('#partialDataReportPlateBarCode').html(data);

                //Desbloqueia a tela
                $.unblockUI({ message: $('#modal-loading') });

            }).fail(function () {

                //Desbloqueia a tela
                $.unblockUI({ message: $('#modal-loading') });

                alert('Erro ao carregar Grid.');
            });
        }
    },
    EventoSelecionaRegistro: function () {

        $('input.checkPatrimonio').change(function () {

            var checkRegisters = $('#result-tables-patrimonio').find('.checkPatrimonio:checked');
            var assetsChecks = [];

            for (var i = 0; i < checkRegisters.length; i++) {
                assetsChecks.push($(checkRegisters[i]).data('id'));
            }
            $('input[name="ListAssetsSelected"]').val(JSON.stringify(assetsChecks));
        });
    },
    EventoSelecionaTodosRegistros: function () {

        $('input.checkTodosPatrimonio').change(function () {

            var checkRegisters = $('#result-tables-patrimonio').find('.checkPatrimonio');
            var assetsChecks = [];

            if ($(this).is(":checked")) {

                for (var i = 0; i < checkRegisters.length; i++) {
                    $(checkRegisters[i]).prop('checked', true);

                    assetsChecks.push($(checkRegisters[i]).data('id'));
                }
                $('input[name="ListAssetsSelected"]').val(JSON.stringify(assetsChecks));
            }
            else {
                for (var i = 0; i < checkRegisters.length; i++) {
                    $(checkRegisters[i]).prop('checked', false);
                }
                $('input[name="ListAssetsSelected"]').val(JSON.stringify(assetsChecks));
            }
        });
    },
    EventoTogglePanel: function () {
        $('#btnFiltrar').click(function () {
            var h = $(window).height();
            var hr;
            var hd;

            if ($(".panel-filter").is(":visible")) {

                hr = 290;
                hd = (h - hr) + 'px';
            }
            else {

                hr = 700;
                hd = (h - hr) + 'px';
            }

            $('.divGrid').css('height', hd);
            $(".panel-filter").toggle("blind", 500);
        });
    },
    AbreModalGeraRelatorio: function () {
        $('#modal').modal('show');
    },
    SubmitForm: function () {
        $('#formGenerateReportPlateBarCode').submit();
    },
    ValidaGeracao: function () {

        if ($('#geraNovasChapas').is(':checked')) {
            if (reportPlateBarCode.ValidaCamposParaGeracaoChapas()) {
                reportPlateBarCode.ValidaGeracaoChapas(function () {

                    reportPlateBarCode.SubmitForm();
                });
            }
        }
        else {
            reportPlateBarCode.SubmitForm();
        }
    },
    ValidaGeracaoChapas: function (callBack) {

        var url = sam.path.webroot + "/PlateBarCode/ValidaGeracaoChapas";

        var _idUge = $('#ManagerUnitId').val();
        var _chapaInicioGerar = $('#numChapaInicioGerar').val();
        var _chapaFimGerar = $('#numChapaFimGerar').val();

        $.get(url, {
            idUge: _idUge,
            chapaInicioGerar: _chapaInicioGerar,
            chapaFimGerar: _chapaFimGerar
        }, function () {
        }).done(function (data) {

            if (data.tipo == 'erro') {

                $("#errorMessageGeraChapas").show();
                window.setTimeout(function () {
                    $("#errorMessageGeraChapas").fadeOut(3000, function () {
                        $("#errorMessageGeraChapas").hide();
                    });
                }, 3000);

                $('#errorMessageGeraChapas').find('div').text(data.Mensagem);
            }
            else {
                callBack();
            }

        }).fail(function () {
            alert('Erro ao carregar Grid.');
        });
    },
    ValidaCamposParaGeracaoChapas: function () {
        if ($('#numChapaInicioGerar').val() == '' || $('#numChapaFimGerar').val() == '') {
            $("#errorMessageGeraChapas").show();
            window.setTimeout(function () {
                $("#errorMessageGeraChapas").fadeOut(3000, function () {
                    $("#errorMessageGeraChapas").hide();
                });
            }, 3000);

            $('#errorMessageGeraChapas').find('div').text('por favor, preecha todos os campos para geração de novas chapas.');

            return false;
        }
        else {
            return true;
        }
    },
    ValidaGeracaoDeReport: function () {
        var valido = true;

        $('#spanInstitution').text('');
        $('#spanBudgetUnits').text('');
        $('#spanManagerUnits').text('');

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
    }
}
