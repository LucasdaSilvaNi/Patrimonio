sam.auxiliaryAccount = {
    formSubmit: '.formSubmit',

    Load: function () {
        sam.auxiliaryAccount.Submit();
        sam.auxiliaryAccount.CarregaCheckBox();
    },

    Submit: function () {
        $(sam.auxiliaryAccount.formSubmit).submit(function () {
            sam.perfilLogin.habilitarCombosHierarquia();
        });
    },

    CarregaCheckBox: function () {

        $('#Acervo').on('ifChanged', function (event) {
            if (this.checked) {
                $('#Terceiro').iCheck('uncheck');
                $('#RelacionadoBP').val(1);
                $('#DepreciationAccountId').val($("#DepreciationAccountId option:first").val());
                $('#DepreciationAccountId').attr("disabled","disabled");
            } else {
                if ($('#Terceiro')[0].checked) {
                    $('#RelacionadoBP').val(2);
                } else {
                    $('#RelacionadoBP').val(0);
                    $('#DepreciationAccountId').removeAttr("disabled");
                }
            }
        });

        $('#Terceiro').on('ifChanged', function (event) {
            if (this.checked) {
                $('#Acervo').iCheck('uncheck');
                $('#RelacionadoBP').val(2);
                $('#DepreciationAccountId').val($("#DepreciationAccountId option:first").val());
                $('#DepreciationAccountId').attr("disabled", "disabled");
            } else {
                if ($('#Acervo')[0].checked) {
                    $('#RelacionadoBP').val(1);
                } else {
                    $('#RelacionadoBP').val(0);
                    $('#DepreciationAccountId').removeAttr("disabled");
                }
            }
        });
    },

    InicializaCheckbox: function(valor){
        if (valor == 1)
            $('#Acervo').iCheck('check');
        if (valor == 2)
            $('#Terceiro').iCheck('check');
    }
}