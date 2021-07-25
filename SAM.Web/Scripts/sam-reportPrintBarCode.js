var reportPrintBarCode = {
    Load: function () {
        $(document).ready(function () {
            $(window).load(function () {
                reportPrintBarCode.EventoTogglePanel();
                reportPrintBarCode.InicializaDatePicker();
            });
        });
    },
    InicializaDatePicker: function(){
        $('.datepicker').datepicker({ language: 'pt-BR', todayHighlight: true, autoclose: true, orientation: 'bottom' });
    },
    CarregaDadosGrid: function () {

        if (reportPrintBarCode.ValidaGeracaoDeReport()) {
            var url = sam.path.webroot + "/PrintBarCode/LoadDataReportPrintBarCode";

            var _institutionId = $('#InstitutionId').val();
            var _budgetUnitId = $('#BudgetUnitId').val();
            var _managerUnitId = $('#ManagerUnitId').val();
            var _contenha = $('#Contenha').val();
            var _numberIdentification = $('#NumberIdentification').val();
            var _chapaInicioRef = $('#numChapaInicioRef').val();
            var _chapaFimRef = $('#numChapaFimRef').val();
            //var _typeReading = parseInt($('input[type=radio][name="TypeReading"]:checked').val());

            //Bloqueia a tela
            $.blockUI({ message: $('#modal-loading') });

            $.get(url, { institutionId: _institutionId, budgetUnitId: _budgetUnitId, managerUnitId: _managerUnitId, contenha: _contenha, chapaInicioRef: _chapaInicioRef, chapaFimRef: _chapaFimRef }, function () {
            }).done(function (data) {

                //Desbloqueia a tela
                $.unblockUI({ message: $('#modal-loading') });

                $('#partialDataReportPrintBarCode').html(data);

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
    },
    SubmitForm: function () {
        $('#formGenerateReportPrintBarCode').submit();
    }
}