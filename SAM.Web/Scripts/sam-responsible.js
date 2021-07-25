sam.responsible = {
    formSubmit: '.formSubmit',

    Submit: function () {
        $(sam.responsible.formSubmit).submit(function () {
            sam.perfilLogin.habilitarCombosHierarquia();
        });
    }
}