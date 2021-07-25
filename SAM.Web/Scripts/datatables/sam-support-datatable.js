SupportDataTable = (function () {
    let tableName = '#tableSupports';
    let actionEditar = '.action-editar';
    let actionConsulta = '#btnFiltrar';
    let institution = "#InstitutionIdFake";
    let unidadeOrcamentaria = "#BudgetUnitIdFake";
    let unidadeGerencial = "#ManagerUnitIdFake";
    let statusProdesp = "#SupportStatusProdespsFake";
    let statusUsuario = "#SupportStatusUserIdFake";
    let nChamados = "#SupportIdFake";
    let aguardando = "#Aguardando";
    let ultimoAtenProdesp = "#UltimoAtenProdesp";
    let ultimoAtenUsuario = "#UltimoAtenUsuario";
    let qualAguardo = ".qualAguardo";
    let primeiraColunaVisivel = false;
    var _form;
    SupportDataTable.prototype.Load = function (form) {
        _form = form;
        criaTabelaSupports(form);
        consultaSupports();
        eventoBotao();
    };
    let consultaSupports = function () {
        $(actionConsulta).click(function () {
            $('#InstitutionId').val($('#InstitutionIdFake').val());
            $('#BudgetUnitId').val($('#BudgetUnitIdFake').val());
            $('#ManagerUnitId').val($('#ManagerUnitIdFake').val());

            $('#SupportStatusProdespId').val($('#SupportStatusProdespsFake').val());
            $('#SupportStatusUserId').val($('#SupportStatusUserIdFake').val());
            $('#SupportId').val($('#SupportIdFake').val());

            $(tableName).DataTable().clear();
            $(tableName).DataTable().destroy();
            criaTabelaSupports(_form);
        });
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
    let clickEnterDataTableSupport = function () {
        var keycode;
        if (window.event)
            keycode = window.event.keyCode;

        //Caso o botão pressionado seja o enter
        if (keycode == 13) {
            $("#btnFiltrar").click();
            return false;
        }
        else {
            return true;
        }
    };
    //eventos
    let editarClickHandler = function (user) {
        window.location = './Support/Edit/' + user.Id;
    };
    //criação
    let criaTabelaSupports = function (form) {
        let csrfToken = $('input[name="__RequestVerificationToken"]', $(form)).val();
        let table = $(tableName).DataTable({
            "bPaginate": true,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": true,
            "bInfo": true,
            "bMaxLength" : 10,
            "bAutoWidth": false,
            "columnsDefs": [
                { "targets": [7], "searchable": false, "orderable": false, "visible": true }
            ],
            "processing": true,
            "serverSide": true,
            "ajax": {
                "url": "Support/IndexJSONResult",
                "type": "POST",
                "headers": {
                    "XSRF-TOKEN": csrfToken
                },
                "dataType": "JSON",
                "data": parametros()
            },
            "columns": [
                {
                    "data": "Id",
                    "render": function (data, type, row) {
                        return "<input class='chamado' type='checkbox' value='" + data +"'/>";
                    },
                  "visible": false},
                { "data": "Id" },
                { "data": "NameManagerReduced" },
                { "data": "BudgetUnitCode"},
                { "data": "ManagerUnitCode" },
                { "data": "UserDescription" },
                {
                    "data": "InclusionDate",
                    "render": function (data, type, row) {
                        return formatarData(new Date(parseInt(data.substr(6))));
                    }
                },
                {
                    "data": "LastModifyDate",
                    "render": function (data, type, row) {
                        return formatarData(new Date(parseInt(data.substr(6))));
                    }
                },
                { "data": "UserCPF" },
                { "data": "Functionanality" },
                { "data": "SupportTypeDescription" },
                { "data": "SupportStatusProdespDescription" },
                { "data": "SupportStatusUserDescription" },
                { "data": "Responsavel" },
                {
                    "data": "CloseDate",
                    "render": function (data, type, row) {
                        if (data == null || data == undefined) {
                            return "";
                        } else {
                            return formatarData(new Date(parseInt(data.substr(6))));
                        }
                    }
                },
                {
                    "defaultContent": `<div class="form-group action-editar" style="margin-left: -1%;"><a class="btnEditar" title="Editar">
                                       <i class="glyphicon glyphicon-pencil editarclass"></i><span class="sr-only">Editar</span></a></div>`,
                    "class": "action text-center"
                }
            ],
            "drawCallback": function () {
                $('.dataTables_paginate').attr('class', 'pagination-container pagination-datatable');
                $("#SupportIdFake").attr('onKeyPress', 'return sam.search.clickEnterDataTableComParametro(this,event,"' + actionConsulta + '")');
                acaoLote();
                inicializaCampoAguardo();
            },
            "rowCallback": function (row, data, Object, index) {

                let posicao = "10";

                if ($(row).find("td").length > 15) {
                    posicao = "11";
                }

                if ($(aguardando).val() == data.SupportStatusProdespId && $(ultimoAtenProdesp).val() == data.UltimoAtendimento) {
                    $(row).find("td:nth("+ posicao+")").css({ "color": "red", "font-weight": "bold" });
                }else{ 
                    if ($(aguardando).val() == data.SupportStatusProdespId && $(ultimoAtenUsuario).val() == data.UltimoAtendimento) {
                        $(row).find("td:nth(" + posicao + ")").css({ "color": "green", "font-weight": "bold" });
                    }else {
                        $(row).find("td:nth("+ posicao+")").css({ "color": "", "font-weight": "" });
                    }
                }

                $(actionEditar, row).bind('click', () => {
                    editarClickHandler(data);
                });

                $(actionEditar, row).bind('mouseover', () => {
                    $(actionEditar, row).css('cursor', 'pointer');
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
    let parametros = function () {
        let Aguardo = 0;
        if ($(statusProdesp).val() == 3 && $(qualAguardo).length > 0) {
            if ($("#daProdesp").is(":checked") != $("#doUsuario").is(":checked")) {
                if ($("#doUsuario").is(":checked")) {
                    Aguardo = 1;
                }
                else {
                    Aguardo = 2;
                }
            }
        }

        let palavraChave = '';
        if ($("#palavraChave").length > 0) {
            palavraChave = $("#palavraChave").val();
        }

        return {
            institution: $(institution).val(),
            unidadeOrcamentaria: $(unidadeOrcamentaria).val(),
            unidadeGerencial: $(unidadeGerencial).val(),
            statusProdesp: $(statusProdesp).val(),
            statusUsuario: $(statusUsuario).val(),
            nChamados: $(nChamados).val(),
            DataInclusao: $("#DataInclusao").val(),
            ultimaResposta: Aguardo,
            historicoContenha: palavraChave,
            currentHier: sam.commun.Organiza()
        };
    };
    let eventoBotao = function () {
        if ($("#btnPorLote").length > 0) {
            $("#btnPorLote").click(function () {
                primeiraColunaVisivel = !$("#tableSupports").DataTable().column(0).visible();
                acaoLote();
                if (primeiraColunaVisivel) {
                    $("#btnPorLote span").removeClass('glyphicon-list-alt');
                    $("#btnPorLote span").addClass('glyphicon-remove');
                    $("#btnResponderLote").show();
                } else {
                    $("#btnPorLote span").removeClass('glyphicon-remove');
                    $("#btnPorLote span").addClass('glyphicon-list-alt');
                    $("#btnResponderLote").hide();
                }
            });
            
        }
    };
    let acaoLote = function () {
        $("#tableSupports").DataTable().column(0).visible(primeiraColunaVisivel);

        if (primeiraColunaVisivel == true) {
            let inicaliza = $('#listaLote').val();
            let listaInicaliza = JSON.parse(inicaliza);
            $(".chamado").each(function () {
                if (jQuery.inArray(parseInt(this.value), listaInicaliza) > -1) {
                    this.checked = true;
                }
            });
            //quando a coluna está invisível, o icheck não molda os radio, por isso
            //inicializa aqui
            $(".chamado").iCheck({ checkboxClass: 'icheckbox_square-blue' });

            $(".chamado").each(function () {

                $(this).on('ifChanged', function () {
                    let valorCampo = $('#listaLote').val();
                    let lista = JSON.parse(valorCampo);

                    if (this.checked) {
                        lista.push(parseInt(this.value))
                    } else {
                        lista = lista.filter(e => e != parseInt(this.value));
                    }

                    $('#listaLote').val(JSON.stringify(lista));
                });
            });
        }
    };
    let inicializaCampoAguardo = function(){
        if ($(statusProdesp).val() == 3 && $(qualAguardo).length > 0) {
            $(qualAguardo).show();
        }
    }
});