var reportAnaliticoDeBemPatrimonial = {

    Load: function () {
        $(document).ready(function () {
            $(window).load(function () {
                reportAnaliticoDeBemPatrimonial.EventoTogglePanel();
                reportAnaliticoDeBemPatrimonial.InicializaDatePicker();
            });
        });
    },
    InicializaDatePicker: function () {
        $('.datepicker').datepicker({ language: 'pt-BR', todayHighlight: true, autoclose: true, orientation: 'bottom' });
    },
    CarregaDadosGrid: function () {

        if (reportAnaliticoDeBemPatrimonial.ValidaGeracaoDeReport()) {

            var url = sam.path.webroot + "Assets/LoadDataAnaliticoDeBemPatrimonial";

            var _institutionId = $('#InstitutionId').val();
            var _budgetUnitId = $('#BudgetUnitId').val();
            var _managerUnitId = $('#ManagerUnitId').val();
            var _administrativeUnitId = $('#AdministrativeUnitId').val();
            var _sectionId = $('#SectionId').val();
            var _statusAsset = eval($('#StatusAsset').val());
            var _numberIdentification = $('#NumberIdentification').val();

            //Bloqueia a tela
            $.blockUI({ message: $('#modal-loading') });

            $.get(url, {
                institutionId: _institutionId,
                budgetUnitId: _budgetUnitId,
                managerUnitId: _managerUnitId,
                administrativeUnitId: _administrativeUnitId,
                sectionId: _sectionId,
                statusAsset: _statusAsset,
                numberIdentification: _numberIdentification
            }, function () {
            }).done(function (data) {

                //Desbloqueia a tela
                $.unblockUI({ message: $('#modal-loading') });

                $('#partialDataReportAnaliticoDeBemPatrimonial').html(data);

            }).fail(function () {
                alert('Erro na rotina CarregaDadosGrid.');
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
        $(".btnGerar").click(function () {
            if ($(".checkPatrimonio:checked").length == 0) {
                alert("Selecione um BP para o relatório!");
                return false;
            } else {
                return true;
            }
        });
        
    }
}
