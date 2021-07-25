sam.costCenter = {
    formSubmit: '.formSubmit',

    Submit: function () {
        $(sam.costCenter.formSubmit).submit(function () {
            sam.perfilLogin.habilitarCombosHierarquia();
        });
    }
}