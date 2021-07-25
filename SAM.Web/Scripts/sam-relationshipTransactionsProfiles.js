var relationshipTransactionsProfiles = {
    init: function () {
        relationshipTransactionsProfiles.load();
    },

    load: function () {
        relationshipTransactionsProfiles.carregaSortable();
        relationshipTransactionsProfiles.eventoTrocaPerfil();
    },

    carregaSortable: function () {
        $(".source, .target").sortable({
            connectWith: ".connected"
        });
    },

    eventoTrocaPerfil: function () {
        $('#ProfileId').change(function (e) {
            var _profileId = $(e.currentTarget).val();

            if (_profileId != '' && _profileId != '0')
                relationshipTransactionsProfiles.recuperaDados(_profileId);
        });
    },

    recuperaDados: function (_profileId) {
        var url = sam.path.webroot + "/RelationshipTransactionsProfiles/RecuperaDados";

        $.get(url, { profileId: _profileId }, function () {
        }).done(function (data) {

            relationshipTransactionsProfiles.montaListaDasTransacoesPorPerfilColunaEsquerda(data.trasacoesNaoRelacionadasAoPerfil);
            relationshipTransactionsProfiles.montaListaDasTransacoesPorPerfilColunaDireita(data.transacoesRelacionadasAoPerfil);

        }).fail(function () {
            sam.commun.CriarAlertDinamico('Erro ao carregar a rotina recuperaDados');
        });
    },

    montaListaDasTransacoesPorPerfilColunaEsquerda: function (data) {

        var row = '';
        var ul = $('#sideBySide .left ul:first');

        ul.find('li').remove();

        $.each(data, function (i, item) {
            row += '<li id="' + item.Id + '" class="ui-sortable-handle"> ' + item.Path + ' - ' + item.Initial +' </li>';
        });

        ul.append(row);
    },

    montaListaDasTransacoesPorPerfilColunaDireita: function (data) {

        var row = '';
        var ul = $('#sideBySide .right ul:first');

        ul.find('li').remove();

        $.each(data, function (i, item) {
            row += '<li id="' + item.Id + '" class="ui-sortable-handle"> ' + item.Path + ' - ' + item.Initial + ' </li>';
        });

        ul.append(row);
    },

    gravaTransacoesPorPerfil: function () {
        var _transacoesPorPerfil = [];

        var _data = $('#sideBySide .right ul:first li');
        var _profileId = $('#ProfileId').val();

        $.each(_data, function (i, item) {
            _transacoesPorPerfil.push(item.id);
        });

        var _arrayTransactionPorPerfil = JSON.stringify($.makeArray(_transacoesPorPerfil));
        var url = sam.path.webroot + "/RelationshipTransactionsProfiles/GravaTransacoesPorpefil";

        $.post(url, { profileId: _profileId, arrayTransactionPorPerfil: _arrayTransactionPorPerfil }, function () {
        }).done(function (data) {

            sam.commun.CriarAlertDinamico(data.mensagem);

            var _profileId = $('#ProfileId').val();

            if (_profileId != '' && _profileId != '0')
                relationshipTransactionsProfiles.recuperaDados(_profileId);

        }).fail(function () {
            sam.commun.CriarAlertDinamico('Erro ao carregar a rotina gravaTransacoesPorPerfil.');
        });
    }
}