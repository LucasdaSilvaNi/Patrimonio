CalendarDataTable = (function () {
    let tableName = '#tableCalendar';
    //let actionCriar = '.action-criar';
    let actionEditar = '.action-editar';
    let actionDetalhar = '.action-detalhar';
    let actionExcluir = '.action-excluir';
    let actionConsulta = '#spanPesquisa';
    //let cbStatus = '.cb-status';
    let consultaText = '#search-tables';
    let temPermissao = '#temPermissao';
    //let perfilOperador = '#perfilOperador';
    let _form;

    CalendarDataTable.prototype.Load = function (form) {
        _form = form;
        createTableCalendar(form);
        //filtrarStatus();
        consultarCalendar();
    };

    //let filtrarStatus = function () {
    //    $('#cbStatus').change(function () {
    //        $(tableName).DataTable().clear();
    //        $(tableName).DataTable().destroy();
    //        criarTableAuxiliary(_form);
    //    });
    //};

    let consultarCalendar = function () {
        $(actionConsulta).click(function () {
            $(tableName).DataTable().clear();
            $(tableName).DataTable().destroy();
            createTableCalendar(_form);
        });
    };

    let formatarData = function (data) {
        var dia = data.getDate();
        if (dia < 10) dia = '0' + dia;

        var mes = data.getMonth() + 1;
        if (mes < 10) mes = '0' + mes;

        //var horas = data.getHours();
        //if (horas < 10) horas = '0' + horas;

        //var minutos = data.getMinutes();
        //if (minutos < 10) minutos = '0' + minutos;

        //var segundos = data.getSeconds();
        //if (segundos < 10) segundos = '0' + segundos;

        return dia + '/' + mes + '/' + data.getFullYear();

        //return dia + '/' + mes + '/' + data.getFullYear() + ' ' + horas + ":" + minutos + ":" + segundos;
    };

    let formatarDataReferencia = function (data) {
              
        var mes = data.getMonth() + 1;
        if (mes < 10) mes = '0' + mes;
        

        return  mes + '/' + data.getFullYear();


    };

    let clickEnterDataTableCalendar = function () {
        var keycode;
        if (window.event)
            keycode = window.event.keyCode;

        if (keycode = 9) {
            $("BtnFiltrar").click();
            return false;
        }
        else {
            return true;
        }
    };

    //var monthly = {

    //    Load: function () {
    //        monthly.InicializaDatePicker();
    //    },


    //    InicializaDatePicker: function () {

    //        var _managerUnitId = $('#ManagerUnitId').val();

    //        $.get(sam.path.webroot + "Movimento/RecuperaMesAnoReferenciaPorUGE", { managerUnitId: _managerUnitId }, function () {
    //        }).done(function (data) {

    //            var mes = data.mes;
    //            var ano = data.ano;
    //            var dataInicial = '01' + '-' + mes + '-' + ano;

    //            var dataAtual = new Date();

    //            var ultimoDiaDoMes = new Date(ano, mes, 0).getDate();
    //            var dataFinalDoMes = new Date(parseInt(ano), (parseInt(mes) - 1), ultimoDiaDoMes);
    //            var dataFinal = '';

    //            if (dataFinalDoMes > dataAtual) {
    //                dataFinal = dataAtual.getDate + '-' + (dataAtual.getMonth + 1) + '-' + dataAtual.getFullYear;
    //            }
    //            else {
    //                dataFinal = dataFinalDoMes.getDate() + '-' + (dataFinalDoMes.getMonth() + 1) + '-' + dataFinalDoMes.getFullYear();
    //            }

    //            $('.datepicker').datepicker('remove');
    //            $('.datepicker').val('');
    //            $('.datepicker').datepicker({ language: 'pt-BR', todayHighlight: true, autoclose: true, orientation: "bottom", startDate: dataInicial, endDate: dataFinal });

    //        }).fail(function () {
    //            $.unblockUI({ message: $('#modal-loading') });
    //            alert('Erro na rotina InicializaDatePicker.');
    //        });
    //    }
    //}
    


    //let editarClickHandler = function (user) {
    //    window.location = './Auxiliary/Create/' + user.Id;
    //};

    //Eventos Crud 
    let editarClickHandler = function (user) {
        window.location = './SiafemCalendar/Edit/' + user.Id;
    };

    let detalharClickHandler = function (user) {
        window.location = './SiafemCalendar/Details/' + user.Id;
    };

    let excluirClickHandler = function (user) {
        window.location = './SiafemCalendar/Delete/' + user.Id;
    };


    //Criando tabela Auxiliar
    createTableCalendar = function (form) {
        let csrfToken = $('input[name="__RequestVerificationToken"]', $(form)).val();
        let table = $(tableName).DataTable({
            "bPaginate": true,
            "bLengthChange": true,
            "pageLength": 10,
            "bFilter": false,
            "bSort": true,
            "bInfo": true,
            "bAutoWidt": false,
            "columnDefs": [
                { "targets": [3], "searchable": false, "orderable": false, "visible": true }
            ],

            "processing": true,
            "serverSide": true,
            "ajax":
            {
                "url": "SiafemCalendar/IndexJSONResult",
                "type": "POST",
                "headers": {
                    "XSRF-TOKEN": csrfToken
                },
                "datatype": "JSON",
                "data": {
                    "currentFilter": $(consultaText).val()
                }
            },

            "columns": [
                { "data": "FiscalYear" },
                //{ "data": "ReferenceMonth" },
                //{ "data": "DateClosing" },
    
            {
                 "data": "ReferenceMonth",
                 "render": function (data, type, row) {
                     return data.toString().substr(4) + "/" + data.toString().substr(0, 4);
                }
            },
            
        //$('.datepicker').datepicker('remove');
        //$('.datepicker').val('');
        //$('.datepicker').datepicker({
        //    language: 'pt-BR', todayHighlight: true, autoclose: true, orientation: "bottom",
        //    startDate: new Date(dataInicial),
        //    endDate: new Date(dataFinal),
        //    format: "mm/yyyy",
        //    viewMode: "months",
        //    minViewMode: "months"
        //}).on('changeDate', function (ev) {

                {
                    "data": "DateClosing",
                    "render": function (data, type, row) {
                        return formatarData(new Date(parseInt(data.substr(6))));
                    }
                },

                {
                    "defaultContent": `<div class= 'form-group'>
                    <a class='action-editar' title='Editar'><i class='glyphicon glyphicon-pencil editarclass'></i><span class='sr-only'>Editar</span></a>
                    <a class='action-detalhar' title='Detalhes'><i class='glyphicon glyphicon-search detalheclass'></i><span class='sr-only'>Detalhes</span></a>
                    <a class='action-excluir' title='Excluir'><i class='glyphicon glyphicon-remove deleteclass'></i><span class='sr-only'>Excluir</span></a></div>`,
                    "className": "text-center"
                }

            ],

            "drawCallback": function () {
                $('.dataTables_paginate').attr('class', 'pagination-container pagination-datatable');
                $("#search-tables").attr('onKeyPress', 'return sam.search.clickEnterDataTable(this,event)');
            },
            //"fnInfoCallback": function (oSettings, iStart, iEnd, iMax, iTotal, sPre) {
            //    console.log(oSettings);
            //},
            "rowCallback": function (row, data, Object, index) {
                $(actionDetalhar, row).bind('click', () => {
                    detalharClickHandler(data);
                });

                $(actionDetalhar, row).bind('mouseover', () => {
                    $(actionDetalhar, row).css('cursor', 'pointer');
                });

                if ($(temPermissao).val() === '1') {
                    $(actionEditar, row).bind('click', () => {
                        editarClickHandler(data);
                });

                $(actionEditar, row).bind('mouseover', () => {
                    $(actionEditar, row).css('cursor', 'pointer');
                });

                $(actionExcluir, row).bind('click', () => {
                    excluirClickHandler(data);
                });
                $(actionExcluir, row).bind('mouseover', () => {
                    $(actionExcluir, row).css('cursor', 'pointer');
                });

                } else {
                    $(actionEditar, row).attr('title', 'Sem permissão para editar');
                    $(actionEditar, row).find('i').addClass('icon-desactived');
                    $(actionExcluir, row).attr('title', 'Sem permissão para excluir');
                    $(actionExcluir, row).find('i').addClass('icon-desactived');
                            

                //if ($(perfilOperador).val() != '1') {
                //    $(actionEditar, row).bind('click', () => {
                //        editarClickHandler(data);
                //    });

                //$(actionDetalhar, row).bind('click', () => {
                //    detalharClickHandler(data);
                //});

                //$(actionDetalhar, row).bind('mouseover', () => {
                //    $(actionDetalhar, row).css('cursor', 'pointer');
                    //});
                }
            },
            "oLanguage": {
                "sProcessing": $('#modal-loading').show(),
                "sThousands": ".",
                "sLengthMenu": "",
                //"sLengthMenu": "Mostrar _MENU_ registros",
                "sZeroRecords": "Não foram encontrados resultados",
                "sInfo": "Página _PAGE_ de _PAGES_",
                "sInfoEmpty": "Mostrando de 0 até 0 de 0 registros",
                "sInfoFiltered": "",
                "sInfoPostFix": "",
                "sSearch": "Buscar:",
                "sUrl": "",
                "oPaginate": {
                    "sFirst": "Primeiro",
                    "sPrevious": "Anterior",
                    "sNext": "Próximo",
                    "sLast": "Último"

                }
            }
        });
    };
});


//sam.auxiliaryAccount = {
//    formSubmit: '.formSubmit',

//    Submit: function () {
//        $(sam.auxiliaryAccount.formSubmit).submit(function () {
//            sam.perfilLogin.habilitarCombosHierarquia();
//        });
//    }
//}