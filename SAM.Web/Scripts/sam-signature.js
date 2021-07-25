sam.signature = {
    formSubmit: '.formSubmit',

    Submit: function () {
        $(sam.signature.formSubmit).submit(function () {
            sam.perfilLogin.habilitarCombosHierarquia();
        });
    }

}