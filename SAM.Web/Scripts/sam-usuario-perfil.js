var usuarioPerfil = {
    listaPerfilsUsuarios: [],
    perfilSelecionado: null,

    Init: function () {
        usuarioPerfil.Load();
        usuarioPerfil.EventoTogglePanel();
    },

    Load: function () {

        var tipoForm = $('#tipoForm').val();

        if (tipoForm == 'I') {
            if ($('#relationshipUserProfileInstitution').val() != '')
                usuarioPerfil.listaPerfilsUsuarios = usuarioPerfil.preparaListaPerfilsUsuarios(JSON.parse($('#relationshipUserProfileInstitution').val()));

            usuarioPerfil.carregaGridPerfisRelacionadosUsuario();
        }
        else if (tipoForm == 'A') {
            if ($('#relationshipUserProfileInstitution').val() != '')
                usuarioPerfil.listaPerfilsUsuarios = usuarioPerfil.preparaListaPerfilsUsuarios(JSON.parse($('#relationshipUserProfileInstitution').val()));

            usuarioPerfil.carregaGridPerfisRelacionadosUsuario();
        }

        usuarioPerfil.limpaMensagensDeValidacoes();
        usuarioPerfil.eventoSubmitForm();
        usuarioPerfil.tratamentoCombosPorPerfil();
    },
    EventoTogglePanel: function () {
        $('#btnFiltrar').click(function () {
            if ($(".panel-filter").is(":visible") != true) {
                $('.divGrid').css('height', '15em');
                $(".panel-filter").toggle("blind", 235);
            } else {

                $('.divGrid').css('height', '360px');
                $(".panel-filter").toggle("blind", 235);
            }
        });
    },

    carregaGridPerfisRelacionadosUsuario: function () {

        var data = usuarioPerfil.listaPerfilsUsuarios;
        var row = '';
        var institutionCode = '';
        var budgetUnitCode = '';
        var managerUnitCode = '';
        var administrativeUnitCode = '';
        var sectionCode = '';

        //Limpa as tr do corpo da table
        $('#tabPerfilRelUsu').find('tbody tr').remove();

        $.each(data, function (i, item) {

            institutionCode = item.InstitutionCode == null ? '' : item.InstitutionCode;
            budgetUnitCode = item.BudgetUnitCode == null ? '' : item.BudgetUnitCode;
            managerUnitCode = item.ManagerUnitCode == null ? '' : item.ManagerUnitCode;
            administrativeUnitCode = item.AdministrativeUnitCode == null ? '' : item.AdministrativeUnitCode;
            sectionCode = item.SectionCode == null ? '' : item.SectionCode;

            row += '<tr id="' + item.Id + '">';

            row += '  <td>' + item.DescProfile + '</td>';
            row += '  <td class="centraliza">' + institutionCode + '</td>';
            row += '  <td class="centraliza">' + budgetUnitCode + '</td>';
            row += '  <td class="centraliza">' + managerUnitCode + '</td>';
            row += '  <td class="centraliza">' + administrativeUnitCode + '</td>';
            row += '  <td class="centraliza">' + sectionCode + '</td>';

            row += '  <td class="centraliza"></td>';

            if (item.ProfilePadrao)
                row += '  <td class="centraliza" style="cursor:pointer;"> <i class="glyphicon glyphicon glyphicon-ok table-text-color-blue"></i> </td>';
            else
                row += '  <td class="centraliza" style="cursor:pointer;" onclick="usuarioPerfil.setaPerfilPadraoParaUsuario(\'' + String(item.Id) + '\')"> <i class="glyphicon glyphicon glyphicon-ok icon-desactived"></i> </td>';

            row += '  <td class="centraliza" style="cursor:pointer;" onclick="usuarioPerfil.removePerfilDoUsuario(\'' + String(item.Id) + '\')"> <i class="glyphicon glyphicon-remove table-text-color-red"></i> </td>';

            row += '</tr>';
        });

        $("#tabPerfilRelUsu tbody").append(row);
    },

    removePerfilDoUsuario: function (id) {

        if (usuarioPerfil.listaPerfilsUsuarios != null && usuarioPerfil.listaPerfilsUsuarios.length > 0)
            usuarioPerfil.listaPerfilsUsuarios = sam.commun.RemoveElementoArrayPorId(id, usuarioPerfil.listaPerfilsUsuarios);

        $('#tabPerfilRelUsu').find('tr[id=' + id + ']').remove();


        //Se após a remoção do tr existir somente mais um perfil, setá-lo como padrão
        var qtdPerfilsDoUsuario = $('#tabPerfilRelUsu').find('tbody tr').length;
        if (qtdPerfilsDoUsuario == 1) {
            var idRelPerfilDoUsuario = $('#tabPerfilRelUsu').find('tbody tr:first')[0].id;
            usuarioPerfil.setaPerfilPadraoParaUsuario(idRelPerfilDoUsuario);
        }
    },

    setaPerfilPadraoParaUsuario: function (id) {

        $.each(usuarioPerfil.listaPerfilsUsuarios, function (i, item) {

            if (String(item.Id).trim() == String(id).trim())
                item.ProfilePadrao = true;
            else
                item.ProfilePadrao = false;
        });

        usuarioPerfil.carregaGridPerfisRelacionadosUsuario();
    },

    adicionaPerfilDoUsuario: function () {

        var institutionId = $('#Institutions').val() == null || $('#Institutions').val() == '' ? '0' : $('#Institutions').val();
        var budgetUnitId = $('#BudgetUnits').val() == null || $('#BudgetUnits').val() == '' ? '0' : $('#BudgetUnits').val();
        var managerUnitId = $('#ManagerUnits').val() == null || $('#ManagerUnits').val() == '' ? '0' : $('#ManagerUnits').val();
        var administrativeUnitId = $('#AdministrativeUnits').val() == null || $('#AdministrativeUnits').val() == '' ? '0' : $('#AdministrativeUnits').val();
        var sectionId = $('#Sections').val() == null || $('#Sections').val() == '' ? '0' : $('#Sections').val();
        var profileId = $('#Profiles').val() == '' ? '0' : $('#Profiles').val();

        var institutionCode = $('#Institutions').val() != null && $('#Institutions').val() != '0' && $('#Institutions').val() != '' ? $('#Institutions :selected').text().split('-')[0].trim() : '';
        var budgetUnitCode = $('#BudgetUnits').val() != null && $('#BudgetUnits').val() != '0' && $('#BudgetUnits').val().trim() != '' ? $('#BudgetUnits :selected').text().split('-')[0].trim() : '';
        var managerUnitCode = $('#ManagerUnits').val() != null && $('#ManagerUnits').val() != '0' && $('#ManagerUnits').val() != '' ? $('#ManagerUnits :selected').text().split('-')[0].trim() : '';
        var administrativeUnitCode = $('#AdministrativeUnits').val() != null && $('#AdministrativeUnits').val() != '0' && $('#AdministrativeUnits').val() != '' ? $('#AdministrativeUnits :selected').text().split('-')[0].trim() : '';
        var sectionCode = $('#Sections').val() != null && $('#Sections').val() != '0' && $('#Sections').val() != '' ? $('#Sections :selected').text().split('-')[0].trim() : '';

        if (usuarioPerfil.validaPerfilDoUsuario(institutionId, budgetUnitId, managerUnitId, administrativeUnitId, sectionId, profileId)) {

            //Define o prefixo
            var prefixo = 'reg_';
            //Recupera o maior index do novo registro de acordo com seu prefixo
            var maiorId = sam.commun.maiorIndexNovoRegistro(usuarioPerfil.listaPerfilsUsuarios, prefixo);

            var relPerfilDoUsuario = {

                Id: prefixo + (maiorId + 1),

                InstitutionId: institutionId,
                BudgetUnitId: budgetUnitId,
                ManagerUnitId: managerUnitId,
                AdministrativeUnitId: administrativeUnitId,
                SectionId: sectionId,

                InstitutionCode: institutionCode,
                BudgetUnitCode: budgetUnitCode,
                ManagerUnitCode: managerUnitCode,
                AdministrativeUnitCode: administrativeUnitCode,
                SectionCode: sectionCode,

                ProfileId: $('#Profiles').val(),
                DescProfile: $('#Profiles :selected').text().split('-')[0].trim(),

                ManagerId: null,
                ProfilePadrao: false,
                RelationshipUserProfileId: null,
                Status: true
            };

            usuarioPerfil.listaPerfilsUsuarios.push(relPerfilDoUsuario);
            usuarioPerfil.carregaGridPerfisRelacionadosUsuario();
        }
    },

    validaPerfilDoUsuario: function (institutionId, budgetUnitId, managerUnitId, administrativeUnitId, sectionId, profileId) {

        var valido = true;

        if ($('#Profiles').val() == null || $('#Profiles').val() == '0' || $('#Profiles').val().trim() == '') {
            $('#spanProfile').text('Por favor, informe o Perfil');
            valido = false;
        }

        if ($('#Institutions').val() == null || $('#Institutions').val() == '0' || $('#Institutions').val().trim() == '') {
            $('#spanInstitution').text('Por favor, informe o Órgão');
            valido = false;
        }

        if (usuarioPerfil.perfilSelecionado == 3) {
            if ($('#BudgetUnits').val() == null || $('#BudgetUnits').val() == '0' || $('#BudgetUnits').val().trim() == '') {
                $('#spanBudgetUnits').text('Por favor, informe a UO');
                valido = false;
            }

            if ($('#ManagerUnits').val() == null || $('#ManagerUnits').val() == '0' || $('#ManagerUnits').val().trim() == '') {
                $('#spanManagerUnits').text('Por favor, informe a UGE');
                valido = false;
            }

            if ($('#AdministrativeUnits').val() == null || $('#AdministrativeUnits').val() == '0' || $('#AdministrativeUnits').val().trim() == '') {
                $('#spanAdministrativeUnits').text('Por favor, informe a UA');
                valido = false;
            }
        }

        if (usuarioPerfil.perfilSelecionado == 5) {
            if ($('#BudgetUnits').val() == null || $('#BudgetUnits').val() == '0' || $('#BudgetUnits').val().trim() == '') {
                $('#spanBudgetUnits').text('Por favor, informe a UO');
                valido = false;
            }
        }

        if (usuarioPerfil.perfilSelecionado == 6) {
            if ($('#BudgetUnits').val() == null || $('#BudgetUnits').val() == '0' || $('#BudgetUnits').val().trim() == '') {
                $('#spanBudgetUnits').text('Por favor, informe a UO');
                valido = false;
            }

            if ($('#ManagerUnits').val() == null || $('#ManagerUnits').val() == '0' || $('#ManagerUnits').val().trim() == '') {
                $('#spanManagerUnits').text('Por favor, informe a UGE');
                valido = false;
            }
        }
        if (usuarioPerfil.perfilSelecionado == 7) {
            if ($('#BudgetUnits').val() == null || $('#BudgetUnits').val() == '0' || $('#BudgetUnits').val().trim() == '') {
                $('#spanBudgetUnits').text('Por favor, informe a UO');
                valido = false;
            }
        }

        if (valido == true && sam.commun.ExisteElementoArrayPorFiltros(institutionId, budgetUnitId, managerUnitId, administrativeUnitId, sectionId, profileId, usuarioPerfil.listaPerfilsUsuarios)) {
            sam.commun.CriarAlertDinamico('Sinto muito, mas já existe um Perfil de Usuário com estas configurações.')
            valido = false;
        }

        return valido;
    },

    validaExistenciaDeUmPerfilPadrao: function () {
        var existe = false;

        var data = usuarioPerfil.listaPerfilsUsuarios;

        $.each(data, function (i, item) {
            if (item.ProfilePadrao == true)
                existe = true;
        });

        if (!existe) {
            sam.commun.CriarAlertDinamico('Por favor, informe um perfil padrão.')
        }

        return existe;
    },

    limpaMensagensDeValidacoes: function () {

        $('#Profiles').change(function () {
            $('#spanProfile').text('');
        });

        $('#Institutions').change(function () {
            $('#spanInstitution').text('');
        });

        $('#BudgetUnits').change(function () {
            $('#spanBudgetUnits').text('');
        });

        $('#ManagerUnits').change(function () {
            $('#spanManagerUnits').text('');
        });

        $('#AdministrativeUnits').change(function () {
            $('#spanAdministrativeUnits').text('');
        });
    },

    removeIdsGeradosParaNovosRegistros: function () {

        var data = usuarioPerfil.listaPerfilsUsuarios;

        $.each(data, function (i, item) {
            if (String(item.Id).indexOf("reg_") >= 0)
                item.Id = 0
        });
    },

    eventoSubmitForm: function () {

        //Atribui um id ao botão de submit
        $('button[type=submit]').attr('id', 'submit');
        //Remove atributo de submit do botão
        $('button[type=submit]').removeAttr('type');
        $('#submit').attr('type', 'button');

        $('#submit').click(function () {

            if (usuarioPerfil.validaExistenciaDeUmPerfilPadrao()) {

                usuarioPerfil.removeIdsGeradosParaNovosRegistros();

                //Atualiza campo com a lista de perfils de usuário
                var relacionamento = JSON.stringify(usuarioPerfil.listaPerfilsUsuarios);
                $('#relationshipUserProfileInstitution').val(relacionamento);

                //Adiciona o atributo de submit do botão
                $('#submit').removeAttr('type');
                $('#submit').attr('type', 'submit');
                //Submete o formulário
                $('#submit').submit();
            }
        });
    },

    preparaListaPerfilsUsuarios: function (listaPerfilsUsuarios) 
    {        
        var cont = 0;        
        $.each(listaPerfilsUsuarios, function (i, item) {                          
            if (item.Id == 0){
                cont++;                
                item.Id = 'reg_' + cont;            
            }        
        });        
        return listaPerfilsUsuarios;    
    },

    tratamentoCombosPorPerfil: function () {
        $('#Profiles').change(function (e) {
            usuarioPerfil.perfilSelecionado = eval($(e.currentTarget).val());

            $('#spanBudgetUnits').text('');
            $('#spanManagerUnits').text('');
            $('#spanAdministrativeUnits').text('');

            $('#Institutions').val($('#Institutions option:first').val());
            $('#BudgetUnits').val($('#BudgetUnits option:first').val());
            $('#ManagerUnits').val($('#ManagerUnits option:first').val());
            $('#AdministrativeUnits').val($('#AdministrativeUnits option:first').val());
            $('#Sections').val($('#Sections option:first').val());

            if (usuarioPerfil.perfilSelecionado == 2) {
                $('#omite_div_por_orgao').css('display', 'block');
                $('#omite_div_por_uo').css('display', 'block');
                $('#omite_div_por_responsavel').css('display', 'none');

                $('#BudgetUnits').val($('#BudgetUnits option:first').val());
                $('#ManagerUnits').val($('#ManagerUnits option:first').val());

                $('#AdministrativeUnits').val('');
                $('#Sections').val('');
            }
            else if (usuarioPerfil.perfilSelecionado == 3) {
                $('#omite_div_por_orgao').css('display', 'block');
                $('#omite_div_por_uo').css('display', 'block');
                $('#omite_div_por_responsavel').css('display', 'block');

                $('#BudgetUnits').val($('#BudgetUnits option:first').val());
                $('#ManagerUnits').val($('#ManagerUnits option:first').val());

                $('#AdministrativeUnits').val($('#AdministrativeUnits option:first').val());
                $('#Sections').val($('#Sections option:first').val());
            }
            else if (usuarioPerfil.perfilSelecionado == 4) {
                $('#omite_div_por_orgao').css('display', 'none');
                $('#omite_div_por_responsavel').css('display', 'none');

                $('#BudgetUnits').val('');
                $('#ManagerUnits').val('');

                $('#AdministrativeUnits').val('');
                $('#Sections').val('');
            }
            else if (usuarioPerfil.perfilSelecionado == 5) {
                $('#omite_div_por_orgao').css('display', 'block');
                $('#omite_div_por_uo').css('display', 'none');
                $('#omite_div_por_responsavel').css('display', 'none');

                $('#BudgetUnits').val($('#BudgetUnits option:first').val());

                $('#ManagerUnits').val('');

                $('#AdministrativeUnits').val('');
                $('#Sections').val('');
            }
            else if (usuarioPerfil.perfilSelecionado == 6) {
                $('#omite_div_por_orgao').css('display', 'block');
                $('#omite_div_por_uo').css('display', 'block');
                $('#omite_div_por_responsavel').css('display', 'none');

                $('#BudgetUnits').val($('#BudgetUnits option:first').val());
                $('#ManagerUnits').val($('#ManagerUnits option:first').val());

                $('#AdministrativeUnits').val('');
                $('#Sections').val('');
            }
            else if (usuarioPerfil.perfilSelecionado == 7) {
                $('#omite_div_por_orgao').css('display', 'block');
                $('#omite_div_por_uo').css('display', 'none');
                $('#omite_div_por_responsavel').css('display', 'none');

                $('#BudgetUnits').val($('#BudgetUnits option:first').val());

                $('#ManagerUnits').val('');
                $('#AdministrativeUnits').val('');
                $('#Sections').val('');
            }
            else {
                $('#omite_div_por_responsavel').css('display', 'block');
                $('#omite_div_por_orgao').css('display', 'block');
                $('#omite_div_por_uo').css('display', 'block');
            }
        });
    }
}