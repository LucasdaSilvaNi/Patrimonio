var tipoDispositivoInventariante = {

    recarregaPagina: false,

    init: function () { },

    Load: function () {
        tipoDispositivoInventariante.EventoChangeOperacaoDispositivoInventariante();
        tipoDispositivoInventariante.EventoChangeTipoDispositivoInventariante();
        //tipoDispositivoInventariante.EventoChangeUGE();
        tipoDispositivoInventariante.eventoSubmitForm();
        //tipoDispositivoInventariante.EventoMostraMensagemObrigatorioUGE();
    },

    EnumTipoDispositivoInventariante: {
        Android: 1,
        COMPEX_CPX8000: 2,
        InventarioManual: 3 
    },

    EnumTipoOperacaoDispositivoInventariante: {
        GeracaoArquivos: 1,
        LeituraArquivos: 2
    },

    EventoChangeTipoDispositivoInventariante: function(){
        $('#TipoDispositivoInventarianteID').change(function () {
            var _tipoDispositivoInventarianteID = $('#TipoDispositivoInventarianteID option:selected').val();

            if(_tipoDispositivoInventarianteID != "")
                $('#TipoOperacaoDispositivoInventarianteID').prop('disabled', false);
            else
                $('#TipoOperacaoDispositivoInventarianteID').prop('disabled', true);
        });
    },


    EventoChangeOperacaoDispositivoInventariante: function () {
        $('#TipoOperacaoDispositivoInventarianteID').change(function () {

            var _tipoDispositivoInventarianteID = $('#TipoDispositivoInventarianteID option:selected').val();
            var _tipoOperacaoDispositivoInventarianteID = $('#TipoOperacaoDispositivoInventarianteID option:selected').val();
            if (_tipoOperacaoDispositivoInventarianteID != '') {


                $.get(sam.path.webroot + "OperacoesComColetor/CarregaPartialViewTipoOperacaoDispositivoInventariante", { tipoDispositivoInventarianteID: _tipoDispositivoInventarianteID, tipoOperacaoDispositivoInventariante: _tipoOperacaoDispositivoInventarianteID }, function () {
                    //$.get(sam.path.webroot + "OperacoesComColetor/CarregaPartialViewTipoOperacaoDispositivoInventariante", { tipoDispositivoInventarianteID: _tipoDispositivoInventarianteID, tipoOperacaoDispositivoInventarianteID: _tipoOperacaoDispositivoInventarianteID }, function () {
                }).done(function (data) {

                    //switch (switch_on) {
                    //    default:

                    //}
                    //Carrega a partialView do tipo dd dispositivo inventariante
                    $('#partialViewTipoDispositivoInventariante').html(data);
                    $('#partialViewTipoDispositivoInventariante').css('display', 'block');

                    //Desbloqueia a tela
                    $.unblockUI({ message: $('#modal-loading') });

                    //var instituationDestino = $('#InstituationIdDestino').val();
                    //var budgetUnitIdDestino = $('#BudgetUnitIdDestino').val();
                    //var managerUnitIdDestino = $('#ManagerUnitIdDestino').val();


                    ////Seta os valores de órgão, UO e UGE nos combos de filtros, para recuperar apenas os dados a mesma hierarquia SIAFEM
                    //$('#InstituationId').val(instituationDestino);
                    //$('#BudgetUnitId').val(budgetUnitIdDestino);
                    //$('#ManagerUnitId').val(managerUnitIdDestino);


                    ////Caso seja alterada a UGE de movimentação
                    //$('.comboManagerUnit2').bind('change', function () {

                    //    //Replica a UGE de movimentação, para a UGE a ser filtrada e recarrega os grids
                    //    var managerUnitId = $(this).val();
                    //    $('#ManagerUnitId').val(managerUnitId).change(); //O método change força o navegador a chamar o evento de change para o combo de Id #ManagerUnitId

                    //    movimento.EventoMostraMensagemObrigatorioUGE();
                    //    var ugeAlterada = true;
                    //    {
                    //        //Desativa o evento de change
                    //        $('.comboManagerUnit2').unbind("change");

                    //        //Habilita novamente os combos dos filtros
                    //        $('#InstituationId').removeAttr('disabled');
                    //        $('#BudgetUnitId').removeAttr('disabled');
                    //        $('#ManagerUnitId').removeAttr('disabled');
                    //    }

                }).fail(function () {
                    $.unblockUI({ message: $('#modal-loading') });
                    alert('Erro na rotina EventoChangeTipoDispositivoInventariante.');
                });

            }
            else {
                $('#partialViewTipoDispositivoInventariante').html('');
            }

        });

        if ($('#TipoDispositivoInventariante').val() != "") {
            $('#partialViewTipoDispositivoInventariante').css('display', 'block');
        }
    },

    EventoChangeUGE: function () {
        $('.comboManagerUnitFiltro').change(function () {

            //Recarrega UA
            $('.comboAdministrativeUnit').empty();
            var options = '';
            var ugeId = $('.comboManagerUnitFiltro').val();
            $.get(sam.path.webroot + "/Hierarquia/GetUas", { ugeId: ugeId }, function (data) {
                var options = '';
                $('.selectpicker.comboAdministrativeUnit').empty().append('<option value="">Selecione a UA</option>');
                $.each(data, function (key, value) {
                    options += '<option value="' + value.Id + '">' + value.Description + '</option>';
                    $('.selectpicker.comboAdministrativeUnit').append(options);
                    options = '';
                });
                $('.selectpicker.comboAdministrativeUnit').selectpicker('refresh');

                var ugeAlterada = true;

                tipo.EventoMostraMensagemObrigatorioUGE();

            });
            $('.selectpicker.comboSection').empty().append('<option value="">Selecione a Divisão</option>');
            $('.selectpicker.comboSection').selectpicker('refresh');
        });
    },

    eventoSubmitForm: function () {

        //Atribui um id ao botão de submit
       // $('button[type=submit]').attr('id', 'submit');

        //Remove atributo de submit do botão
        $('button[type=submit]').removeAttr('type');
        $('#submit').attr('type', 'button');

        $('#submit').click(function () {

            //Habilita novamente os combos dos filtros
            $('#InstituationId').removeAttr('disabled');
            $('#BudgetUnitId').removeAttr('disabled');
            $('#ManagerUnitId').removeAttr('disabled');


            //Adiciona o atributo de submit do botão
            $('#submit').removeAttr('type');
            $('#submit').attr('type', 'submit');
            //Submete o formulário
            $('#submit').submit();

        });
    },

    EventoMostraMensagemObrigatorioUGE: function () {
        if ($('#ManagerUnitId').val() == '0') {
            $('#spanManagerUnitFiltro').text('Por favor, informe uma UGE para o processamento dos dados de Bens Patrimonial.');
        }
        else {
            $('#spanManagerUnitFiltro').text('');
        }
    },
}
