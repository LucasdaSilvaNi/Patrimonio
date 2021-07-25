﻿OutSourcedsDataTable = (function (){
    let tableName = '#tableOutSourceds';
    let actionEditar = '.action-editar';
    let actionDetalhar = '.action-detalhar';
    let actionExcluir = '.action-excluir';
    let actionConsulta = '#spanPesquisa';
    let consultaText = '#search-tables';
    let perfilOperador = '#perfilOperador';
    var _form;

    OutSourcedsDataTable.prototype.Load = function (form){
        _form = form;
        criarTabelaOutSourceds(form);
        consultarOutSourceds();
    };
    
    let consultarOutSourceds = function () {
        $(actionConsulta).click(function () {
            $(tableName).DataTable().clear();
            $(tableName).DataTable().destroy();
            criarTabelaOutSourceds(_form); 
        });
    };

    let verificaNulidade = function (data) {
        if (data == null)
            return "";
        else
            return data;
    };

    let editarClickHandler = function (outsourced){
        window.location = './OutSourceds/Edit/' + outsourced.Id;
    };

    let detalharClickHandler = function (outsourced){
        window.location = './OutSourceds/Details/' + outsourced.Id;
    };

    let excluirClickHandler = function (outsourced){
        window.location = './OutSourceds/Delete/' + outsourced.Id;
    };

    //criação
    criarTabelaOutSourceds = function (form) {
        let csrfToken = $('input[name="__RequestVerificationToken"]', $(form)).val();
        let table = $(tableName).DataTable({
            "bPaginate": true,
            "bLengthChange": true,
            "bFilter": false,
            "bSort": true,
            "bInfo": true,
            "bAutoWidth": false,
            "columnDefs": [
               { "targets": [6], "searchable": false, "orderable": false, "visible": true }
            ],

    
           "processing": true,
        "serverSide": true,
        "ajax":
        {
            "url": "OutSourceds/IndexJSONResult",
            "type": "POST",
            "headers": {
                "XSRF-TOKEN": csrfToken
            },
            "dataType": "JSON",
            "data": {
                "currentFilter": $(consultaText).val(),
                "currentHier": sam.commun.Organiza()
            }

        },
        
           "columns": [
             {
                 "data": "InstitutionCode",
                 "render": function (data, type, row) {
                     return verificaNulidade(data);
                 }
             },
             {
                 "data": "InstitutionDescription",
                 "render": function (data, type, row) {
                     return verificaNulidade(data);
                 }
             },
             {
                 "data": "BudgetUnitCode",
                 "render": function (data, type, row) {
                     return verificaNulidade(data);
                 }
             },
             {
                 "data": "BudgetUnitDescription",
                 "render": function (data, type, row) {
                     return verificaNulidade(data);
                 }
             },
            
             {
                "data": "CPFCNPJ",
                "render": function (data, type, row) {
                    return verificaNulidade(data);
                }
             },
            
            {
                 "data": "Name",
                "render": function (data, type, row) {
                return verificaNulidade(data);
            }
             },
             {
                 "defaultContent": `<div class='form-group text-center' style='margin-left: -1%;'>
                                       <a class='action-editar' title='Editar'><i class='glyphicon glyphicon-pencil editarclass'></i><span class='sr-only'>Editar</span></a>
                                       <a class='action-detalhar' title='Detalhes'><i class='glyphicon glyphicon-search detalheclass'></i><span class='sr-only'>Detalhes</span></a>
                                       <a class='action-excluir' title='Excluir'><i class ='glyphicon glyphicon-remove deleteclass'></i><span class='sr-only'>Excluir</span></a></div>`,
                "class": "action text-center"
             }
        ],

        "drawCallback": function () {
            $('.dataTables_paginate').attr('class', 'pagination-container pagination-datatable');
            $("#search-tables").attr('onKeyPress', 'return sam.search.clickEnterDataTable(this,event)');
        },
            "rowCallback": function (row, data, Object, index) {

                if ($(perfilOperador).val() != '1') {
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
                }

                $(actionDetalhar, row).bind('click', () => {
                    detalharClickHandler(data);
                });

                $(actionDetalhar, row).bind('mouseover', () => {
                    $(actionDetalhar, row).css('cursor', 'pointer');
                });

            },
            "oLanguage": {
                "sProcessing": $('#modal-loading').show(),
                "sThousands": ".",
                "sLengthMenu": "Mostrar _MENU_ registros",
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