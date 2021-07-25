hierarquiaIntegradaSiafem = {
    bind: function () {

        $('.comboinstitutionIntegradoSIAFEM').change(function () {
            $('.comboBudgetUnitIntegradoSIAFEM').empty();
            var options = '';
            var orgaoId = $('.comboinstitutionIntegradoSIAFEM').val();
            $.get(sam.path.webroot + "/Hierarquia/GetUos", { orgaoId: orgaoId }, function (data) {
                var options = '';

                $.each(data, function (key, value) {
                    options += '<option value="' + value.Id + '">' + value.Description + '</option>';
                    $('.comboBudgetUnitIntegradoSIAFEM').append(options);
                    options = '';
                });
            });
            $('.comboBudgetUnitIntegradoSIAFEM').empty().append('<option value="">Selecione a UO</option>');
            $('.comboManagerUnit').empty().append('<option value="">Selecione a UGE</option>');
        });

        $('.comboBudgetUnitIntegradoSIAFEM').change(function () {
            $('.comboManagerUnitIntegradoSIAFEM').empty();
            var options = '';
            var uoId = $('.comboBudgetUnitIntegradoSIAFEM').val();
            $.get(sam.path.webroot + "/Hierarquia/GetUgesIntegradasAoSIAFEM", { uoId: uoId }, function (data) {
                var options = '';
                $('.comboManagerUnit').empty().append('<option value="">Selecione a UGE</option>');
                $.each(data, function (key, value) {
                    options += '<option value="' + value.Id + '">' + value.Description + '</option>';
                    $('.comboManagerUnit').append(options);
                    options = '';
                });
            });
        });
    }
}