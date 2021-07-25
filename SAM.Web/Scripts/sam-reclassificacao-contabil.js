reclassificacaoContabil = {
    listadosBP: [],
    load: function () {
        function inicializaCampoHierarquias() { 
            $('.comboinstitution').change(function () {
                $('.comboBudgetUnit').empty();
                var options = '';
                var orgaoId = $('.comboinstitution').val();
                $.get(sam.path.webroot + "/Hierarquia/GetUos", { orgaoId: orgaoId }, function (data) {
                    var options = '';

                    $.each(data, function (key, value) {
                        options += '<option value="' + value.Id + '">' + value.Description + '</option>';
                        $('.comboBudgetUnit').append(options);
                        options = '';
                    });
                });
                $('.comboBudgetUnit').empty().append('<option value="">Selecione a UO</option>');
                $('.comboManagerUnit').empty().append('<option value="">Selecione a UGE</option>');
                $("#divGrupoMaterial").hide();
                $("#ListaDosBPs").hide();
            });

            $('.comboBudgetUnit').change(function () {

                $('.comboManagerUnit').empty();
                var options = '';
                var uoId = $('.comboBudgetUnit').val();
                $.get(sam.path.webroot + "/Hierarquia/GetUges", { uoId: uoId }, function (data) {
                    var options = '';
                    $('.comboManagerUnit').empty().append('<option value="">Selecione a UGE</option>');
                    $.each(data, function (key, value) {
                        options += '<option value="' + value.Id + '">' + value.Description + '</option>';
                        $('.comboManagerUnit').append(options);
                        options = '';
                    });
                    $("#divGrupoMaterial").hide();
                    $("#ListaDosBPs").hide();
                });
            });

            if ($(".comboManagerUnit").val() != 0) {
                reclassificacaoContabil.carregaGrupoMaterial($(".comboManagerUnit").val());
            } else {
                $("#divGrupoMaterial").hide();
                $("#ListaDosBPs").hide();
            }

            $(".comboManagerUnit").change(function (e) {
                let valor = e.target.value;
                if (valor != "0" && valor != "") {
                    reclassificacaoContabil.carregaGrupoMaterial(valor);
                } else {
                    $("#divGrupoMaterial").hide();
                    $("#ListaDosBPs").hide();
                }
            });
        }
        function eventoChangeCampoGrupoMaterial(){
            $("#GrupoMaterial").change(function (e) {
                let valor = e.target.value;

                if (valor != "") {
                    $("#ListaDosBPs").show();
                    $("#reclassificar").hide();
                    reclassificacaoContabil.listadosBP = [];
                    reclassificacaoContabil.carregaDataTable();
                } else {
                    $("#ListaDosBPs").hide();
                }
            });
        }
        function eventoClickBotaoPesquisa() {
            $("#spanPesquisa").click(function () {
                reclassificacaoContabil.carregaDataTable();
            });
        }
        function bloqueiaSubmit() {
            $("#formtabela").submit(function (e) {
                e.preventDefault();
            })
        }
        function inicializaBotoes() {
            $("#reclassificarTodos").click(function (e) {
                e.target.disabled = true;
                $.post(sam.path.webroot + "/ReclassificacaoContabil/EscolheContaContabilBPsTodos", { grupoMaterial: $("#GrupoMaterial").val(), numeroUGE: $("#ManagerUnitId").val() }
                       , function (data) {
                           e.target.disabled = false;
                           if (data.naoEncontrado != undefined) {
                               alert('Os BPs provavelmente já foram reclassificados. A página será reiniciada');
                               location.reload();
                           } else if(data.erro != undefined){
                               alert('Não foi possível buscar as conta contábeis para esse grupo agora. Por gentileza, tente novamente mais tarde');
                               location.reload();
                          } else{
                               $(".bodyModalTrocaDeConta").html(data);
                               $('#modalTrocaDeConta').modal({ keyboard: false, backdrop: 'static', show: true });
                               reclassificacaoContabil.configuraBotoesModaisTodos();
                           }
                       });
            });

            $("#reclassificar").click(function (e) {
                e.target.disabled = true;
                $.post(sam.path.webroot + "/ReclassificacaoContabil/EscolheContaContabilBPsEscolhidos", { lista: reclassificacaoContabil.listadosBP }
                       , function (data) {
                           e.target.disabled = false;
                           if (data.naoEncontrado != undefined) {
                               alert('Os BPs provavelmente já foram reclassificados. A página será reiniciada');
                               location.reload();
                           } else if (data.erro != undefined) {
                               alert('Não foi possível buscar as conta contábeis para esse grupo agora. Por gentileza, tente novamente mais tarde');
                               location.reload();
                           } else {
                               $(".bodyModalTrocaDeConta").html(data);
                               $('#modalTrocaDeConta').modal({ keyboard: false, backdrop: 'static', show: true });
                               reclassificacaoContabil.configuraBotoesModaisEscolhidos();
                           }
                       });
            });
        }

        inicializaCampoHierarquias();
        eventoChangeCampoGrupoMaterial();
        eventoClickBotaoPesquisa();
        bloqueiaSubmit();
        inicializaBotoes();
        $("#ListaDosBPs").hide();
    },
    carregaDataTable: function () {
        let csrfToken = $('input[name="__RequestVerificationToken"]', $("#formtabela")).val();
        $("#tableListaBPs").DataTable().clear();
        $("#tableListaBPs").DataTable().destroy();
        $("#tableListaBPs").DataTable(
            {
                "bPaginate": true,
                "bLengthChange": false,
                "bFilter": false,
                "bSort": false,
                "bInfo": false,
                "bAutoWidth": false,
                "processing": true,
                "serverSide": true,
                "bMaxLength": 10,
                "ajax": {
                    "url": "ReclassificacaoContabil/BuscaBPS",
                    "type": "POST",
                    "headers": {
                        "XSRF-TOKEN": csrfToken
                    },
                    "dataType": "JSON",
                    "data": { grupoMaterial: $("#GrupoMaterial").val(), numeroUGE: $("#ManagerUnitId").val(), pesquisa: $("#search-tables").val() }
                },
                "columns": [
                    {
                        "data": "Id",
                        "render": function (data, type, row) {
                            return "<input class='bp' type='checkbox' value='" + data + "'/>";
                        },
                    },
                    { "data": "Sigla" },
                    { "data": "Chapa" },
                    { "data": "Descricao" },
                    { "data": "ContaContabilAtual" }
                ],
                "drawCallback": function () {
                    $('.dataTables_paginate').attr('class', 'pagination-container pagination-datatable');
                    reclassificacaoContabil.inicializaCheckBoxesTabela();
                },
                "oLanguage": {
                    "sProcessing": $('#modal-loading').show(),
                    "sZeroRecords": "Não foram encontrados resultados",
                    "oPaginate": {
                        "sFirst": "Primeiro",
                        "sPrevious": "Anterior",
                        "sNext": "Próximo",
                        "sLast": "Último"
                    }
                }
            });
    },
    carregaGrupoMaterial: function (UGE) {
        $.post(sam.path.webroot + "/ReclassificacaoContabil/CarregaCampoGrupo", { numeroUGE: UGE },
               function (data) {
                   $("#GrupoMaterial").empty();
                   if (data.semGrupos == undefined) {
                       $("#GrupoMaterial").append('<option class="filter-option pull-left" value> -- Selecione -- </option>');
                       $.each(data, function (key, value) {
                           $("#GrupoMaterial").append('<option class="filter-option pull-left" value="' + value.Codigo + '">' + value.Codigo + ' - ' + value.Descricao + '</option>');
                       });
                       $("#divGrupoMaterial").show();
                   } else {
                       alert('Não foram encontrados BPs dessa UGE para serem reclassificados');
                   }
               })
         .fail(function () { alert("Houve um erro ao buscar os grupos materiais do BPs. Por gentileza, tente novamente mais tarde."); });
    },
    inicializaCheckBoxesTabela: function () {
        $(".bp").each(function () {
            if (jQuery.inArray(parseInt(this.value), reclassificacaoContabil.listadosBP) > -1) {
                this.checked = true;
            }

            $(this).iCheck({ checkboxClass: 'icheckbox_square-blue' });

            $(this).on('ifChanged', function () {
                if (this.checked) {
                    reclassificacaoContabil.listadosBP.push(parseInt(this.value));
                    $("#reclassificar").show();
                } else {
                    reclassificacaoContabil.listadosBP = reclassificacaoContabil.listadosBP.filter(e => e != parseInt(this.value));
                    if (reclassificacaoContabil.listadosBP.length == 0) {
                        $("#reclassificar").hide();
                    }
                }
            });
        });
    },
    configuraBotoesModaisEscolhidos: function () {
        function realizaReclassificacao() {
            $.blockUI({ message: $('#modal-loading') });
            $.post(sam.path.webroot + "/ReclassificacaoContabil/RealizaReclassificacao", { lista: reclassificacaoContabil.listadosBP, novaConta: $("#IdContaContabil").val() }
                       , function (data) {
                           $.unblockUI({ message: $('#modal-loading') });
                           if (data.msg != undefined) {
                               $(".bodyModalMsgFinal").html(data.msg);
                               $('#modalMsgFinal').modal({ keyboard: false, backdrop: 'static', show: true });
                               $("#ok").click(function () { location.reload() });
                           }
                       })
            .fail(function () { alert("Não foi possível fazer a reclassificação do BPs no momento. Por gentileza, tente novamente mais tarde."); location.reload() });
        }

        $("#reclassificarComEscolha").click(function () {
            if ($("#IdContaContabil").val() == "") {
                return false;
            } else {
                realizaReclassificacao();
            };
        });

        $("#reclassificarEmDefinitivo").click(function () {
                realizaReclassificacao();
        });
    },
    configuraBotoesModaisTodos: function () {
        function realizaReclassificacao() {
            $.blockUI({ message: $('#modal-loading') });
            $.post(sam.path.webroot + "/ReclassificacaoContabil/RealizaReclassificacaoTodos", { novaConta: $("#IdContaContabil").val(), grupoMaterial: $("#GrupoMaterial").val(), numeroUGE: $("#ManagerUnitId").val() }
                       , function (data) {
                           $.unblockUI({ message: $('#modal-loading') });
                           if (data.msg != undefined) {
                               $(".bodyModalMsgFinal").html(data.msg);
                               $('#modalMsgFinal').modal({ keyboard: false, backdrop: 'static', show: true });
                               $("#ok").click(function () { location.reload() });
                           }
                       })
            .fail(function () { alert("Não foi possível fazer a reclassificação do BPs no momento. Por gentileza, tente novamente mais tarde."); location.reload() });
        }

        $("#reclassificarComEscolha").click(function () {
            if ($("#IdContaContabil").val() == "") {
                return false;
            } else {
                realizaReclassificacao();
            };
        });

        $("#reclassificarEmDefinitivo").click(function (e) {
            realizaReclassificacao();
        });
    }
}