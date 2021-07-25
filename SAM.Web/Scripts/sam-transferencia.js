sam.transferencia = {
    init: function () {
        sam.transferencia.bind();

        $('.selectpicker.comboSection').selectpicker({
            liveSearch: true,
            showSubtext: true
        });

        $('.selectpicker.comboSection2').selectpicker({
            liveSearch: true,
            showSubtext: true
        });

        $('.selectpicker.comboAdministrativeUnit').selectpicker({
            liveSearch: true,
            showSubtext: true
        });

        $('.selectpicker.comboAdministrativeUnit2').selectpicker({
            liveSearch: true,
            showSubtext: true
        });
    },
    bind: function () {
        $('.comboinstitution').change(function () {
            $('.comboBudgetUnit').empty();
            var options = '';
            var orgaoId = $('.comboinstitution').val();
            $.get(sam.path.webroot + "/Hierarquia/GetUos", { orgaoId: orgaoId }, function (data) {
                var options = '';

                $.each(data, function (key, value) {
                    options += '<option value="' + value.Id + '">' + value.Description + '</option>';
                    $('.comboBudgetUnit').append(options);
                    options = '';
                });
            });
            $('.comboBudgetUnit').empty().append('<option value="">Selecione a UO</option>');
            $('.comboManagerUnit').empty().append('<option value="">Selecione a UGE</option>');
            $('.selectpicker.comboAdministrativeUnit').empty().append('<option value="">Selecione a UA</option>');
            $('.selectpicker.comboAdministrativeUnit').selectpicker('refresh');
            $('.comboResponsible').empty().append('<option value="">Selecione o Responsavel</option>');
            //$('.comboSection').empty().append('<option value="">Selecione a Divisão</option>');
            // NOVO COMBO DE PESQUISA
            $('.selectpicker.comboSection').empty().append('<option value="">Selecione a Divisão</option>');
            $('.selectpicker.comboSection').selectpicker('refresh');
        });

        $('.comboinstitution2').change(function () {
            $('.comboBudgetUnit2').empty();
            var options = '';
            var orgaoId = $('.comboinstitution2').val();
            $.get(sam.path.webroot + "/Hierarquia/GetUos", { orgaoId: orgaoId }, function (data) {
                var options = '';

                $.each(data, function (key, value) {
                    options += '<option value="' + value.Id + '">' + value.Description + '</option>';
                    $('.comboBudgetUnit2').append(options);
                    options = '';
                });
            });
            $('.comboBudgetUnit2').empty().append('<option value="">Selecione a UO</option>');
            $('.comboManagerUnit2').empty().append('<option value="">Selecione a UGE</option>');
            $('.selectpicker.comboAdministrativeUnit2').empty().append('<option value="">Selecione a UA</option>');
            $('.selectpicker.comboAdministrativeUnit2').selectpicker('refresh');
            $('.comboResponsible2').empty().append('<option value="">Selecione o Responsavel</option>');
            $('.selectpicker.comboSection2').empty().append('<option value="">Selecione a Divisão</option>');
            $('.selectpicker.comboSection2').selectpicker('refresh');
        });

        $('.comboBudgetUnit').change(function () {

            $('.comboManagerUnit').empty();
            var options = '';
            var uoId = $('.comboBudgetUnit').val();
            $.get(sam.path.webroot + "/Hierarquia/GetUges", { uoId: uoId }, function (data) {
                var options = '';
                $('.comboManagerUnit').empty().append('<option value="">Selecione a UGE</option>');
                $.each(data, function (key, value) {
                    options += '<option value="' + value.Id + '">' + value.Description + '</option>';
                    $('.comboManagerUnit').append(options);
                    options = '';
                });
            });

            $('.selectpicker.comboAdministrativeUnit').empty().append('<option value="">Selecione a UA</option>');
            $('.selectpicker.comboAdministrativeUnit').selectpicker('refresh');
            $('.comboResponsible').empty().append('<option value="">Selecione o Responsavel</option>');
            //$('.comboSection').empty().append('<option value="">Selecione a Divisão</option>');
            // NOVO COMBO DE PESQUISA
            $('.selectpicker.comboSection').empty().append('<option value="">Selecione a Divisão</option>');
            $('.selectpicker.comboSection').selectpicker('refresh');
        });

        $('.comboBudgetUnit2').change(function () {
            $('.comboManagerUnit2').empty();
            var options = '';
            var uoId = $('.comboBudgetUnit2').val();
            $.get(sam.path.webroot + "/Hierarquia/GetUges", { uoId: uoId }, function (data) {
                var options = '';
                $('.comboManagerUnit2').empty().append('<option value="">Selecione a UGE</option>');
                $.each(data, function (key, value) {
                    options += '<option value="' + value.Id + '">' + value.Description + '</option>';
                    $('.comboManagerUnit2').append(options);
                    options = '';

                    //bloqueio de UGE para Movimentação no Mesmo Órgão
                    if ($('#MovementTypeId option:selected').val() == movimento.EnumMovementType.MovTransferenciaMesmoOrgaoPatrimoniado ||
                        $('#MovementTypeId option:selected').val() == movimento.EnumMovementType.MovSaidaInservivelDaUGETransferencia) {

                        if ($('#ManagerUnitId option:selected').length > 0) {
                            if ($('#ManagerUnitId option:selected').val() != '' && $('#ManagerUnitId option:selected').val() != '0') {
                                $(".comboManagerUnit2 option[value='" + $('#ManagerUnitId option:selected').val() + "']").attr('disabled', 'disabled');
                            }
                        } else {
                            $(".comboManagerUnit2 option[value='" + $('#ManagerUnitId').val() + "']").attr('disabled', 'disabled');
                        }
                    }

                });
            });

            $('.selectpicker.comboAdministrativeUnit2').empty().append('<option value="">Selecione a UA</option>');
            $('.selectpicker.comboAdministrativeUnit2').selectpicker('refresh');
            $('.comboResponsible2').empty().append('<option value="">Selecione o Responsavel</option>');
            $('.selectpicker.comboSection2').empty().append('<option value="">Selecione a Divisão</option>');
            $('.selectpicker.comboSection2').selectpicker('refresh');
        });

        $('.comboManagerUnit').change(function () {

            var options = '';
            var orgaoId = $('.comboinstitution').val();
            var uoId = $('.comboBudgetUnit').val();
            var ugeId = $('.comboManagerUnit').val();
            var relatorio = $('.checkRelatorio').val();

            $.get(sam.path.webroot + "/Hierarquia/GetUas", { ugeId: ugeId }, function (data) {
                var options = '';
                if (relatorio) {
                    $('.selectpicker.comboAdministrativeUnit').empty().append('<option value="">-- Todas as UAs --</option>');
                }
                else {
                    $('.selectpicker.comboAdministrativeUnit').empty().append('<option value="">Selecione a UA</option>');
                }
                $.each(data, function (key, value) {
                    options += '<option value="' + value.Id + '">' + value.Description + '</option>';
                    $('.selectpicker.comboAdministrativeUnit').append(options);
                    options = '';
                });
                $('.selectpicker.comboAdministrativeUnit').selectpicker('refresh');
            });
            $('.comboResponsible').empty().append('<option value="">Selecione o Responsavel</option>');
            //$('.comboSection').empty().append('<option value="">Selecione a Divisão</option>');
            // NOVO COMBO DE PESQUISA
            $('.selectpicker.comboSection').empty().append('<option value="">Selecione a Divisão</option>');
            $('.selectpicker.comboSection').selectpicker('refresh');

            $.get(sam.path.webroot + "/Hierarquia/GetInitial", { orgaoId: orgaoId, uoId: uoId, ugeId: ugeId }, function (data) {
                var options = '';
                $('.comboInitial').empty().append('<option value="">Selecione a Sigla</option>');
                $.each(data, function (key, value) {
                    options += '<option value="' + value.Id + '">' + value.Name + '</option>';
                    $('.comboInitial').append(options);
                    options = '';
                });
            });
        });

        $('.comboManagerUnit2').change(function () {

            var options = '';
            var ugeId = $('.comboManagerUnit2').val();
            $.get(sam.path.webroot + "/Hierarquia/GetUas", { ugeId: ugeId }, function (data) {
                var options = '';
                $('.selectpicker.comboAdministrativeUnit2').empty().append('<option value="">Selecione a UA</option>');
                $.each(data, function (key, value) {
                    options += '<option value="' + value.Id + '">' + value.Description + '</option>';
                    $('.selectpicker.comboAdministrativeUnit2').append(options);
                    options = '';
                });
                $('.selectpicker.comboAdministrativeUnit2').selectpicker('refresh');
            });
            $('.comboResponsible2').empty().append('<option value="">Selecione o Responsavel</option>');
            $('.selectpicker.comboSection2').empty().append('<option value="">Selecione a Divisão</option>');
            $('.selectpicker.comboSection2').selectpicker('refresh');
        });

        $('.comboAdministrativeUnit').change(function () {
            var options = '';
            var propertyId = $('.comboAdministrativeUnit')[1].id;
            var uaId = $('#' + propertyId + '').val()
            $.get(sam.path.webroot + "/Hierarquia/GetDivisoes", { uaId: uaId }, function (data) {
                var options = '';
                $('.selectpicker.comboSection').empty().append('<option value="">Selecione a Divisão</option>');
                $.each(data, function (key, value) {
                    options += '<option value="' + value.Id + '">' + value.Description + '</option>';
                    $('.selectpicker.comboSection').append(options);
                    options = '';
                });
                $('.selectpicker.comboSection').selectpicker('refresh');
            });
            $('.comboResponsible').empty();
            var options = '';
            var propertyId = $('.comboAdministrativeUnit')[1].id;
            var uaId = $('#' + propertyId + '').val()
            $.get(sam.path.webroot + "/Hierarquia/GetResponsavel", { uaId: uaId }, function (data) {
                var options = '';
                $('.comboResponsible').empty().append('<option value="">Selecione o Responsavel</option>');
                $.each(data, function (key, value) {
                    options += '<option value="' + value.Id + '">' + value.Name + '</option>';
                    $('.comboResponsible').append(options);
                    options = '';
                });
            });
        });

        $('.comboAdministrativeUnit2').change(function () {
            var options = '';
            var propertyId = $('.comboAdministrativeUnit2')[1].id;
            var uaId = $('#' + propertyId + '').val()
            $.get(sam.path.webroot + "/Hierarquia/GetDivisoes", { uaId: uaId }, function (data) {
                var options = '';
                $('.selectpicker.comboSection2').empty().append('<option value="">Selecione a Divisão</option>');
                $.each(data, function (key, value) {
                    options += '<option value="' + value.Id + '">' + value.Description + '</option>';
                    $('.selectpicker.comboSection2').append(options);
                    options = '';
                });
                $('.selectpicker.comboSection2').selectpicker('refresh');
            });
            $('.comboResponsible2').empty();
            var options = '';
            var propertyId = $('.comboAdministrativeUnit2')[1].id;
            var uaId = $('#' + propertyId + '').val()
            $.get(sam.path.webroot + "/Hierarquia/GetResponsavel", { uaId: uaId }, function (data) {
                var options = '';
                $('.comboResponsible2').empty().append('<option value="">Selecione o Responsavel</option>');
                $.each(data, function (key, value) {
                    options += '<option value="' + value.Id + '">' + value.Name + '</option>';
                    $('.comboResponsible2').append(options);
                    options = '';
                });
            });
        });

        $('.comboSection2').change(function () {
            var options = '';
            var propertyId = $('.comboSection2')[1].id;
            var divisaoId = $('#' + propertyId + '').val()
            $('.comboResponsible2').empty();
            var options = '';
            if (divisaoId == "0" || divisaoId == null || divisaoId == "") {
                var propertyId = $('.comboAdministrativeUnit2')[1].id;
                var uaId = $('#' + propertyId + '').val()
                $.get(sam.path.webroot + "/Hierarquia/GetResponsavel", { uaId: uaId }, function (data) {
                    var options = '';
                    $('.comboResponsible2').empty().append('<option value="">Selecione o Responsavel</option>');
                    $.each(data, function (key, value) {
                        options += '<option value="' + value.Id + '">' + value.Name + '</option>';
                        $('.comboResponsible2').append(options);
                        options = '';
                    });
                });
            }
            else {
                $.get(sam.path.webroot + "/Hierarquia/GetResponsavelPorDivisao", { divisaoId: divisaoId }, function (data) {
                    var options = '';
                    $('.comboResponsible2').empty().append('<option value="">Selecione o Responsavel</option>');
                    $.each(data, function (key, value) {
                        options += '<option value="' + value.Id + '">' + value.Name + '</option>';
                        $('.comboResponsible2').append(options);
                        options = '';
                    });
                });
            }
        });

        // Caso seja informado a Divisão, carrega o Responsavel da divisao, se somente informe UA, carrega responsaveis por UA
        $('.comboSection').change(function () {
            var options = '';
            var propertyId = $('.comboSection')[1].id;
            var divisaoId = $('#' + propertyId + '').val()
            $('.comboResponsible').empty();
            var options = '';
            if (divisaoId == "0" || divisaoId == null || divisaoId == "") {
                var propertyId = $('.comboAdministrativeUnit')[1].id;
                var uaId = $('#' + propertyId + '').val()
                $.get(sam.path.webroot + "/Hierarquia/GetResponsavel", { uaId: uaId }, function (data) {
                    var options = '';
                    $('.comboResponsible').empty().append('<option value="">Selecione o Responsavel</option>');
                    $.each(data, function (key, value) {
                        options += '<option value="' + value.Id + '">' + value.Name + '</option>';
                        $('.comboResponsible').append(options);
                        options = '';
                    });
                });
            }
            else {
                $.get(sam.path.webroot + "/Hierarquia/GetResponsavelPorDivisao", { divisaoId: divisaoId }, function (data) {
                    var options = '';
                    $('.comboResponsible').empty().append('<option value="">Selecione o Responsavel</option>');
                    $.each(data, function (key, value) {
                        options += '<option value="' + value.Id + '">' + value.Name + '</option>';
                        $('.comboResponsible').append(options);
                        options = '';
                    });
                });
            }
        });
    }
}

