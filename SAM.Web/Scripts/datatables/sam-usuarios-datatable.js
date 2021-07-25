UsuarioDataTable = (function () {
    let tableName = '#tableUsuarios';
    let actionEditar = '.action-editar';
    let actionDetalhar = '.action-detalhar';
    let actionExcluir = '.action-excluir';
    let actionConsulta = '#spanPesquisa';
    let consultaText = '#search-tables';
    var _form;
    UsuarioDataTable.prototype.Load = function (form) {
        _form = form;
        criarTabelaUsuarios(form);
        consultarUsuarios();
    };
    let formatarData = function (data) {
        var dia = data.getDate();
        if (dia < 10) dia = '0' + dia;

        var mes = data.getMonth() + 1;
        if (mes < 10) mes = '0' + mes;

        var horas = data.getHours();
        if (horas < 10) horas = '0' + horas;

        var minutos = data.getMinutes();
        if (minutos < 10) minutos = '0' + minutos;

        var segundos = data.getSeconds();
        if (segundos < 10) segundos = '0' + segundos;

        return dia + '/' + mes + '/' + data.getFullYear() + ' ' + horas + ":" + minutos + ":" + segundos;
    };
    let consultarUsuarios = function () {
        $(actionConsulta).click(function () {
            $(tableName).DataTable().clear();
            $(tableName).DataTable().destroy();
            criarTabelaUsuarios(_form);
        });
    };

    //eventos
    let editarClickHandler = function (user) {
        window.location = './Users/Edit/' + user.Id;
    };

    let detalharClickHandler = function (user) {
        window.location = './Users/Details/' + user.Id;
    };

    let excluirClickHandler = function (user) {
        window.location = './Users/Delete/' + user.Id;
    };
    //criação
    criarTabelaUsuarios = function (form) {
        let csrfToken = $('input[name="__RequestVerificationToken"]', $(form)).val();
        let table = $(tableName).DataTable({
            "bPaginate": true,
            "bLengthChange": true,
            "bFilter": false,
            "bSort": true,
            "bInfo": true,
            "bAutoWidth": false,
            "columnDefs": [
                { "targets": [7], "searchable": false, "orderable": false, "visible": true },
                { "targets": [8], "searchable": false, "orderable": false, "visible": true }
            ],
            "processing": true,
            "serverSide": true,
            "ajax":
            {
                "url": "Users/IndexJSONResult",
                "type": "POST",
                "headers": {
                    "XSRF-TOKEN": csrfToken
                },
                "dataType": "JSON",
                "data": {
                    "currentFilter": $(consultaText).val()
                }

            },
            "columns": [
                { "data": "CPF" },
                { "data": "Name" },
                { "data": "Orgao", "className": "text-center" },
                { "data": "Uo" },
                { "data": "Uge" },
                { "data": "Email" },
                { "data": "AddedDate" },
                {
                    "data": "Status",
                    "defaultContent": `<div class="icheckbox_square-blue checked disabled" style="position: relative;">
                                      <input checked="checked" class="check-box" disabled="disabled" type="checkbox" style="position: absolute; opacity: 0;">
                                      <ins class="iCheck-helper" style="position: absolute; top: 0%; left: 0%; display: block; width: 100%; height: 100%; margin: 0px; padding: 0px; background: rgb(255, 255, 255); border: 0px; opacity: 0;">
                                      </ins></div>`,
                    "className": "text-center"
                },
                {
                    "defaultContent": `<div class='form-group' style='margin-left: -1%;'><a class='action-editar' title='Editar'><i class='glyphicon glyphicon-pencil editarclass'></i><span class='sr-only'>Editar</span></a>
                                       <a class='action-detalhar' title='Detalhes'><i class='glyphicon glyphicon-search detalheclass'></i><span class='sr-only'>Detalhes</span></a>
                                       <a class ='action-excluir' title='Excluir'><i class ='glyphicon glyphicon-remove deleteclass'></i><span class='sr-only'>Excluir</span></a></div>`,
                    "className": "text-center"
                }
            ],
            "drawCallback": function () {
                $('.dataTables_paginate').attr('class', 'pagination-container pagination-datatable');
                $("#search-tables").attr('onKeyPress', 'return sam.search.clickEnterDataTable(this,event)');
                $('.sorting_asc').removeClass("sorting_asc");
            },
            "rowCallback": function (row, data, Object, index) {
                $(row).find('td:nth-child(7)').text(formatarData(new Date(parseInt(data.AddedDate.substr(6)))));

                if (data.Status == 1) {
                    $(row).find('td:nth-child(8)').html(`<div class="icheckbox_square-blue checked disabled" style="position: relative;">
                                                     <input checked="checked" class="check-box" disabled="disabled" type="checkbox" style="position: absolute; opacity: 0;">
                                                     <ins class="iCheck-helper" style="position: absolute; top: 0%; left: 0%; display: block; width: 100%; height: 100%; margin: 0px; padding: 0px; background: rgb(255, 255, 255); border: 0px; opacity: 0;">
                                                     </ins></div>`);
                } else {
                    $(row).find('td:nth-child(8)').html(`<div class="icheckbox_square-blue disabled" style="position: relative;">
                                                     <input class="check-box" disabled="disabled" type="checkbox" style="position: absolute; opacity: 0;">
                                                     <ins class="iCheck-helper" style="position: absolute; top: 0%; left: 0%; display: block; width: 100%; height: 100%; margin: 0px; padding: 0px; background: rgb(255, 255, 255); border: 0px; opacity: 0;">
                                                     </ins></div>`);
                }

                $(actionEditar, row).bind('click', () => {
                    editarClickHandler(data);
                });

                $(actionDetalhar, row).bind('click', () => {
                    detalharClickHandler(data);
                });

                $(actionEditar, row).bind('mouseover', () => {
                    $(actionEditar, row).css('cursor', 'pointer');
                });

                $(actionDetalhar, row).bind('mouseover', () => {
                    $(actionDetalhar, row).css('cursor', 'pointer');
                });

                if (data.CPF != $("#cpf").val()) {
                    $(actionExcluir, row).bind('click', () => {
                        excluirClickHandler(data);
                    });
                    $(actionExcluir, row).bind('mouseover', () => {
                        $(actionExcluir, row).css('cursor', 'pointer');
                    });
                } else {
                    $(actionExcluir, row).attr("title", "Sem permissão para excluir");
                    $(actionExcluir, row).find("i").removeClass("deleteclass");
                    $(actionExcluir, row).find("i").addClass("icon-desactived");
                }
                return row;
            },
            "oLanguage": {
                "sProcessing": $('#modal-loading').show(),
                "sThousands": ".",
                "sLengthMenu": "Mostrar _MENU_ registros",
                "sZeroRecords": "Não foram encontrados resultados",
                "sInfo": "Página _PAGE_ de _PAGES_",
                "sInfoEmpty": "",
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