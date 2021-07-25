sam.perfilLogin = {
    comboInstitution: '.comboinstitution',
    comboManager: '.comboManager',
    comboBudgetUnit: '.comboBudgetUnit',
    comboManagerUnit: '.comboManagerUnit',
    comboAdministrativeUnit: '.comboAdministrativeUnit',
    comboSection: '.comboSection',
    habilitarCombosHierarquia: function () {
        $(sam.perfilLogin.comboInstitution).prop('disabled', false);
        $(sam.perfilLogin.comboManager).prop('disabled', false);
        $(sam.perfilLogin.comboBudgetUnit).prop('disabled', false);
        $(sam.perfilLogin.comboManagerUnit).prop('disabled', false);
        $(sam.perfilLogin.comboAdministrativeUnit).prop('disabled', false);
        $(sam.perfilLogin.comboSection).prop('disabled', false);
    },
    importarIntegracao: function () {
        $.get(sam.path.webroot + 'AssetPending/ImportarIntegracao', {}, function (data) {
            console.info("LoginIntegracao", data);
        });
    }
}