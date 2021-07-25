confrontoContabil = {
    load: function () {
        confrontoContabil.CarregaComboMesReferencia();
        confrontoContabil.GeraExcel();
    },
    CarregaComboMesReferencia: function () {
        if ($("#InstitutionId").val() != null &&
           $("#InstitutionId").val() != 0) {
            var _url = sam.path.webroot + "Institutions/CarregaComboPeriodos";

            $('#MesRef option').remove();

            let orgao = $("#InstitutionId").val();
            let logado = $("#institutionIdCurrent").val();

            $.get(_url, { orgaoId: orgao, orgaoLogadoId: logado }, function (data) {
                var options = '';

                try {
                    $.each(data, function (key, elem) {
                        if (elem.Value != "0") {
                            options += '<option value="' + elem.Value + '">' + elem.Text + '</option>';
                            $('#MesRef').prepend(options);
                        }
                        options = '';
                    });

                    options += '<option value="0">Mês Atual</option>';
                    $('#MesRef').prepend(options);
                    $('#MesRef').val("0");
                    options = '';
                } catch (e) {
                    alert('ERRo');
                }
            });
        }
    },
    GeraExcel: function () {
        $(".btnExcel").click(function () {
            $('span[data-valmsg-for="ManagerUnitId"]').text('');
            $('#msg').text('Buscando as informações no SIAFEM. Por favor, aguarde...');
            $("#formConfrontoContabil").submit();
        });
    }
}