AuxiliaryDataTable = (function () {
    let tableName = '#tableAuxiliary';
    let actionEditar = '.action-editar';
    let actionDetalhar = '.action-detalhar';
    let actionExcluir = '.action-excluir';
    let actionConsulta = '#spanPesquisa';
    let consultaText = '#search-tables';
    let temPermissao = '#temPermissao';
    let _form;

    AuxiliaryDataTable.prototype.Load = function (form) {
        _form = form;
        criarTableAuxiliary(form);
        //filtrarStatus();
        consultarAuxiliary();
    };
    let consultarAuxiliary = function () {
        $(actionConsulta).click(function () {
            $(tableName).DataTable().clear();
            $(tableName).DataTable().destroy();
            criarTableAuxiliary(_form);
        });
    };

    //Eventos Crud 
    let editarClickHandler = function (user) {
        window.location = './AuxiliaryAccounts/Edit/' + user.Id;
    };

    let detalharClickHandler = function (user) {
        window.location = './AuxiliaryAccounts/Details/' + user.Id;
    };

    let excluirClickHandler = function (user) {
        window.location = './AuxiliaryAccounts/Delete/' + user.Id;
    };
    //outros métodos
    let verificaNulidade = function (data) {
        if (data === null)
            return "";
        else
            return data;
    };
    let verificaNegatividade = function (data) {
        if (data === null || parseInt(data) < 0)
            return "";
        else
            return data;
    };
    //Criando tabela Auxiliar
      criarTableAuxiliary = function (form) {
        let csrfToken = $('input[name="__RequestVerificationToken"]', $(form)).val();
        let table = $(tableName).DataTable({
            "bPaginate": true,
            "bLengthChange": true,
            "bFilter": false,
            "bSort": true,
            "bInfo": true,
            "bAutoWidt": false,
            "columnDefs": [
                { "targets": [4], "searchable": false, "orderable": false, "visible": true }
            ],

            "processing": true,
            "serverSide": true,
            "ajax":
            {
                "url": "AuxiliaryAccounts/IndexJSONResult",
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
                { "data": "Description" },
                {
                    "data": "CodigoContaContabil",
                    "className": "text-center"
                },
                {
                    "data": "CodigoContaDepreciada",
                    "render": function (data, type, row) {
                        return verificaNegatividade(data);
                    },
                    "className": "text-center"
                },
                {
                    "data": "DescricaoContaDepreciada",
                    "render": function (data, type, row) {
                        return verificaNulidade(data);
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
                }
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