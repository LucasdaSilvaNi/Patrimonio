sam.commun = {

    init: function () {
        sam.commun.InicializaICheck();
    },

    CriarAlertDinamico: function (mensagem, callback) {

        sam.commun.removeModaConfirm();

        var htmlBuilder = [];

        htmlBuilder.push('<div class="modal fade modalAlertDinamico" id="modalAlertDinamico" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel">');
        htmlBuilder.push('<div class="modal-dialog" role="document">');
        htmlBuilder.push('    <div class="modal-content">');
        htmlBuilder.push('        <div class="modal-header">');
        htmlBuilder.push('            <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>');
        htmlBuilder.push('            <h4 class="modal-title" id="exampleModalLabel">Alerta</h4>');
        htmlBuilder.push('        </div>');
        htmlBuilder.push('        <div class="modal-body">');
        htmlBuilder.push('                    <div class="form-group">');
        htmlBuilder.push('                <p for="recipient-name" >' + mensagem + '</p>');
        htmlBuilder.push('            </div>');
        htmlBuilder.push('        </div>');
        htmlBuilder.push('        <div class="modal-footer">');
        htmlBuilder.push('            <button type="button" class="btn btn-primary" data-dismiss="modal" onclick="sam.commun.unloadAlertDinamico(' + callback + ')">Fechar</button>');
        htmlBuilder.push('        </div>');
        htmlBuilder.push('    </div>');
        htmlBuilder.push('</div>');
        htmlBuilder.push('</div>');
        htmlBuilder.join("")

        $(document.body).append(htmlBuilder.join(""));
        $('#modalAlertDinamico').modal('show');
    },

    removeModaConfirm: function () {
        $('#modalAlertDinamico').remove();
    },

    unloadAlertDinamico: function (callback) {
        $('#modalAlertDinamico').modal('hide');
        $('#modalAlertDinamico').empty();

        if (callback != undefined)
        {
            callback();
        }
    },

    RemoveElementoArrayPorId: function (id, array) {
        var arrayFiltrado = array.filter(function (item) {
            return item.Id != id;
        });

        return arrayFiltrado;
    },

    maiorIndexNovoRegistro: function (lista, prefixo) {
        var maiorId = 0;
        var listaNovosIds = [];

        if (lista.length > 0) {

            //Recupera novos registros
            var padrao = new RegExp(prefixo, 'i');
            var listaNovosregistros = lista.filter(function (item) {
                return padrao.test(item.Id);
            });

            for (var i = 0; i < listaNovosregistros.length; i++) {
                listaNovosIds.push(eval(listaNovosregistros[i].Id.substring(prefixo.length, listaNovosregistros[i].Id.length)));
            }

            if (listaNovosIds.length > 0) {
                maiorId = Math.max.apply(null, listaNovosIds);
            }
        }

        return maiorId;
    },

    ExisteElementoArrayPorFiltros: function (institutionId, budgetUnitId, managerUnitId, administrativeUnitId, sectionId, profileId, array) {
        var elemFiltrado = array.filter(function (item) {
            return String(institutionId).trim() == (item.InstitutionId == null ? '0' : String(item.InstitutionId).trim()) &&
                   String(budgetUnitId).trim() == (item.BudgetUnitId == null ? '0' : String(item.BudgetUnitId).trim()) &&
                   String(managerUnitId).trim() == (item.ManagerUnitId == null ? '0' : String(item.ManagerUnitId).trim()) && 
                   String(administrativeUnitId).trim() == (item.AdministrativeUnitId == null ? '0' : String(item.AdministrativeUnitId).trim()) && 
                   String(sectionId).trim() == (item.SectionId == null ? '0' : String(item.SectionId).trim()) && 
                   String(profileId).trim() == (item.ProfileId == null ? '0' : String(item.ProfileId).trim())
        });

        return elemFiltrado.length > 0;
    },

    limpar: function (div) {
        var texts = $(div).find("input[type=text]");
        for (var i = 0; i < texts.length; i++) {
            texts[i].value = '';
        }
        var dates = $(div).find("input[type=date]");
        for (var i = 0; i < dates.length; i++) {
            dates[i].value = '';
        }
        var dates = $(div).find("select");
        for (var i = 0; i < dates.length; i++) {
            dates[i].value = '';
        }
        var dates = $(div).find("input[type=number]");
        for (var i = 0; i < dates.length; i++) {
            dates[i].value = '';
        }
    },

    InicializaICheck: function () {
        $('input[type=checkbox], input[type=radio]').iCheck({
            checkboxClass: 'icheckbox_square-blue',
            radioClass: 'iradio_square-blue'
        });
    },

    CancelaPostPartialView: function () {
        $(document).keydown(function (event) {
            if (event.keyCode == 13) {
                event.preventDefault();
                return false;
            }
        });
    },

    FormatarDataJson: function (data) {

        if (data == null)
            return data;

        var dateString = data.substr(6);
        var currentTime = new Date(parseInt(dateString));
        var month = ("0" + (currentTime.getMonth() + 1)).slice(-2);
        var day = ("0" + currentTime.getDate()).slice(-2);
        var year = currentTime.getFullYear();
        var date = year + '-' + month + '-' + day;
        return date
    },

    RemoveItemArray: function (array, value) {
        array = $.grep(array, function (item) {
            return item != value;
        });

        return array;
    },

    EventoComboPeriodoPorUGE: function () {
        sam.commun.CarregaComboPeriodoPorUGE();

        $('#ManagerUnitId').change(function () {
            sam.commun.CarregaComboPeriodoPorUGE();
        });
    },

    CarregaComboPeriodoPorUGE: function () {
        var _url = sam.path.webroot + "Assets/CarregaComboPeriodosPorUGE";

        var _budgetUnitId = $('#BudgetUnitId').val();
        var _managerUnitId = $('#ManagerUnitId').val();
        $('#MesRef option').remove();

        $.get(_url, { budgetUnitId: _budgetUnitId, managerUnitId: _managerUnitId }, function (data) {
            var options = '';

            $.each(data, function (key, elem) {
                options += '<option value="' + elem.Value + '">' + elem.Text + '</option>';
                $('#MesRef').append(options);
                options = '';
            });
        });
    },
    CarregaComboPeriodoPorOrgao: function () {
        var _url = sam.path.webroot + "Institutions/CarregaComboPeriodos";

        $('#MesRef option').remove();

        let orgao = $("#InstitutionId").val();
        let logado = $("#institutionIdCurrent").val();

        $.get(_url, { orgaoId: orgao, orgaoLogadoId: logado }, function (data) {
            var options = '';

            try {
                $.each(data, function (key, elem) {
                    options += '<option value="' + elem.Value + '">' + elem.Text + '</option>';
                    $('#MesRef').append(options);
                    options = '';
                });
            } catch (e) {
                alert('Houve algum erro inesperado ao carregar os meses de referência. Por gentileza, tente novamente mais tarde.');
            }
        });
    },
    Organiza: function () {
        return $("#institutionIdCurrent").val() + "," + $("#budgetUnitIdCurrent").val() + "," + $("#managerUnitIdCurrent").val() + ","
               + $("#administrativeUnitIdCurrent").val() + "," + $("#sectionUnitIdCurrent").val();
    }
}