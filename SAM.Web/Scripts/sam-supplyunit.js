sam.supplyUnit = {
    formSubmit: '.formSubmit',

    Submit: function () {
        $(sam.supplyUnit.formSubmit).submit(function () {
            sam.perfilLogin.habilitarCombosHierarquia();
        });
    }
}