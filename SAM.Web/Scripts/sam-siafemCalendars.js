InicializaDatePicker = function () {
    let dataInicio;

    //var _managerUnitId = $('#ManagerUnitId').val();

    //$.get(sam.path.webroot + "Movimento/RecuperaMesAnoReferenciaPorUGE", { managerUnitId: _managerUnitId }, function () {
    //}).done(function (data) {

    //    var mes = data.mes;
    //    var ano = data.ano;
    //    //var dataInicial = '01' + '-' + mes + '-' + ano;
    //    var dataInicial = ano + '-' + mes + '-' + '02';
    //    var diaDataFinal = new Date(ano, parseInt(mes), 1);

    //    var dataAtual = new Date();

    //    var ultimoDiaDoMes = new Date(ano, mes, 0).getDate();
    //    var dataFinalDoMes = new Date(parseInt(ano), (parseInt(mes) - 1), ultimoDiaDoMes);
    //    var dataFinal;


    //    var dataFechamentoInicial = new Date(ano, parseInt(mes), 1)
    //    var dataFechamentoFinal;

    //    if (dataFinalDoMes > dataAtual) {
    //        dataFinal = dataAtual.getFullYear + '-' + (dataAtual.getMonth + 1) + '-' + dataAtual.getDate;
    //        dataFechamentoFinal = dataAtual.getFullYear + '-' + (dataAtual.getMonth + 2) + '-' + diaDataFinal.getDate;
    //    }
    //    else {
    //        dataFinal = dataFinalDoMes.getFullYear() + '-' + (dataFinalDoMes.getMonth() + 1) + '-' + dataFinalDoMes.getDate();
    //        dataFechamentoFinal = dataFinalDoMes.getFullYear() + '-' + (parseInt(mes) + 2) + '-' + '01';

    //        var ultimoDia = new Date(dataFechamentoInicial.getFullYear(), dataFechamentoInicial.getMonth() + 1, 0);
    //        dataFechamentoFinal = new Date(dataFechamentoInicial.getFullYear(), dataFechamentoInicial.getMonth(), ultimoDia.getDate());
    //    }


    //            if ($('.datepicker') != null) {
    //                $('.datepicker').datepicker('remove');
    //                $('.datepicker').val('');
    //                $('.datepicker').datepicker({
    //                    language: 'pt-BR', todayHighlight: true, autoclose: true, orientation: "bottom",
    //                    startDate: new Date(dataInicial),
    //                    endDate: new Date(dataFinal),
    //                    format: "mm/yyyy",
    //                    viewMode: "months",
    //                    minViewMode: "months"
    //                }).on('changeDate', function (ev) {


    //                    $('.datepicker1').datepicker('remove');
    //                    $('.datepicker1').val('');
    //                    $('.datepicker1').datepicker({
    //                        language: 'pt-BR', todayHighlight: true, autoclose: true, orientation: "bottom",
    //                        startDate: dataFechamentoInicial,
    //                        endDate: dataFechamentoFinal,
    //                        format: "dd/mm/yyyy",
    //                        viewMode: "days",
    //                        minViewMode: "days"
    //                    });

    //                    $('.datepicker1')[0].focus();
    //                }).data('datepicker');

    //            } else {
    //                $('.datepicker1').datepicker('remove');
    //                $('.datepicker1').val('');
    //                $('.datepicker1').datepicker({
    //                    language: 'pt-BR', todayHighlight: true, autoclose: true, orientation: "bottom",
    //                    startDate: new Date(dataFechamentoInicial),
    //                    endDate: new Date(dataFechamentoFinal),
    //                    format: "dd/mm/yyyy",
    //                    viewMode: "days",
    //                    minViewMode: "days"
    //                });
    //            }

    //            var checkout = $('.datepicker1').datepicker({
    //                onRender: function (date) {
    //                    return date.valueOf() <= checkin.date.valueOf() ? 'disabled' : '';
    //                },
    //                language: 'pt-BR', todayHighlight: true, autoclose: true, orientation: "bottom",
    //                startDate: new Date(dataFechamentoInicial),
    //                endDate: new Date(dataFechamentoFinal),
    //                format: "dd/mm/yyyy",
    //                viewMode: "days",
    //                minViewMode: "days"
    //            }).on('changeDate', function (ev) {
    //                checkout.hide();
    //            }).data('datepicker');

    //        }).fail(function () {
    //            $.unblockUI({ message: $('#modal-loading') });
    //            alert('Erro na rotina InicializaDatePicker.');
    //        });
    //    }

    $(".datepicker").datepicker({
        language: 'pt-BR',
        //ignoreReadonly: true,
        format: "mm/yyyy",
        todayBtn: true,
        autoclose: true,
        viewMode: "months",
        minViewMode: "months"
    })
      .on("changeDate", function (e) {
          var checkInDate = e.date, $checkOut = $(".datepicker1");
          //checkInDate.setDate(checkInDate.getDate() + 30);
          var data = new Date(checkInDate.getFullYear(), checkInDate.getMonth() + 1, checkInDate.getDate());

          $checkOut.datepicker("setStartDate", data);
          $checkOut.datepicker("setDate", data).focus();
      });

    $(".datepicker1").datepicker({
        language: 'pt-BR',
        todayHighlight: true,
        format: "dd/mm/yyyy",
        todayBtn: true,
        autoclose: true,
        viewMode: "days",
        minViewMode: "days"
    });

    $(".datepicker2").datepicker({
        language: 'pt-BR',
        todayHighlight: true,
        format: "dd/mm/yyyy",
        todayBtn: true,
        autoclose: true,
        viewMode: "days",
        minViewMode: "days"
    });

}

AlteraDatePicker = function () {

    $(".dateload").load({
        language: 'pt-BR',
        //ignoreReadonly: true,
        format: "mm/yyyy",
        todayBtn: true,
        autoclose: true,
        viewMode: "months",
        minViewMode: "months"
    }).on("blur", function (e) {

        //var checkInDate = e.date, $checkOut = $(".dateload1");
        // var data = new Date(checkInDate.getFullYear(), checkInDate.getMonth() + 1, checkInDate.getDate());
        //alert('oi')
        //$checkOut.load("setStartDate", data);
        //alert(e.date);
        //$checkOut.load("setDate", data).focus();
    });
    
     
    $(".dateload1").datepicker({
        language: 'pt-BR',
        todayHighlight: true,
        format: "dd/mm/yyyy",
        todayBtn: true,
        autoclose: true,
        viewMode: "days",
        minViewMode: "days"
    }).on("show", function (e) {

        //var checkInDate = e.date, $checkOut = $(".dateload1");
       // var data = new Date(checkInDate.getFullYear(), checkInDate.getMonth() + 1, checkInDate.getDate());
        //alert('oi')
        //$checkOut.load("setStartDate", data);
        //$checkOut.load("setDate", data).focus();
    });

    $(".dateload2").datepicker({
        language: 'pt-BR',
        todayHighlight: true,
        format: "dd/mm/yyyy",
        todayBtn: true,
        autoclose: true,
        viewMode: "days",
        minViewMode: "days"
    }).on("show", function (e) {

        //var checkInDate = e.date, $checkOut = $(".dateload1");
        // var data = new Date(checkInDate.getFullYear(), checkInDate.getMonth() + 1, checkInDate.getDate());
        //alert('oi')
        //$checkOut.load("setStartDate", data);
        //$checkOut.load("setDate", data).focus();
    });
}

setDataInicio = function (data) {
    $(".dateload1").datepicker({
        language: 'pt-BR',
        todayHighlight: true,
        format: "dd/mm/yyyy",
        todayBtn: true,
        autoclose: true,
        viewMode: "days",
        minViewMode: "days",
        startDate: new Date(data)
    });

    $(".dateload2").datepicker({
        language: 'pt-BR',
        todayHighlight: true,
        format: "dd/mm/yyyy",
        todayBtn: true,
        autoclose: true,
        viewMode: "days",
        minViewMode: "days",
        startDate: new Date(data)
    });
}