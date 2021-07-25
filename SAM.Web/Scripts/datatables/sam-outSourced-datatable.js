OutSourcedsDataTable = (function (){
    let tableName = '#tableOutSourceds';
    let actionEditar = '.action-editar';
    let actionDetalhar = '.action-detalhar';
    let actionExcluir = '.action-excluir';
    let actionConsulta = '#spanPesquisa';
    let consultaText = '#search-tables';
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
                "currentFilter": $(consultaText).val()
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
                $(actionEditar, row).bind('click', () => {
                    editarClickHandler(data);
                });           

                $(actionEditar, row).bind('mouseover', () => {
                    $(actionEditar, row).css('cursor', 'pointer');
                });

                $(actionDetalhar, row).bind('click', () => {
                    detalharClickHandler(data);
                });

                $(actionDetalhar, row).bind('mouseover', () => {
                    $(actionDetalhar, row).css('cursor', 'pointer');
                });

                $(actionExcluir, row).bind('click', () => {
                    excluirClickHandler(data);
                });

                $(actionExcluir, row).bind('mouseover', () => {
                    $(actionExcluir, row).css('cursor', 'pointer');
                });
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



//sam.outsourced = {
//    formSubmit: '.formSubmit',
//    cnpjCpf: '.cnpj-cpf',
//    load:function(){
//        sam.outsourced.validarCnpjCpf();
//    },
//    Submit: function () {
//        $(sam.outsourced.formSubmit).submit(function () {
//            sam.perfilLogin.habilitarCombosHierarquia();
//        });
//    },
//    validarCnpjCpf: function () {
//        $(sam.outsourced.cnpjCpf).blur(function () {
//            var retorno = sam.utils.isCpfCnpj($(this).val());

//            if (retorno == false) {
//                $('span[data-valmsg-for="CPFCNPJ"]').text('Cnpj/Cpf ínvalido.');
//                return false;
//            }
//        })
//    }
//}