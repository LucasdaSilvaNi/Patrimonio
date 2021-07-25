sam.sections = {
    formSubmit: '.formSubmit',

    Load: function () {
        $(document).ready(function () {
            $(window).load(function () {
                sam.sections.CriarDivMensagemModal();
            });
        });
    },
    EventoTrocaStatusRetorno: function () {
        $('#cbStatus').change(function () {
            $('#spanPesquisa').click()
        });
    },

    Submit: function () {
        $('.comboResponsible').change(function () {
            $('#MensagemModal').val('False');
        });

        sam.sections.submitForm();

        $(document).ready(function () {
            $(window).load(function () {
                sam.sections.CriarDivMensagemModal();
            });
        });

        $(sam.sections.formSubmit).submit(function () {
            sam.perfilLogin.habilitarCombosHierarquia();
        });

        var value = [];

        $(".somenteNumerosMax").each(function (i) {
            value[i] = this.defaultValue;
            $(this).data("idx", i);
        });
        $(".somenteNumerosMax").on("keyup", function (e) {
            var $field = $(this),
                val = this.value,
                $thisIndex = parseInt($field.data("idx"), $field.attr("maxlength"));

            if (this.validity && this.validity.badInput || isNaN(val) || $field.is(":invalid")) {
                this.value = value[$thisIndex];
                return;
            }
            if (val.length > Number($field.attr("maxlength"))) {
                val = val.slice(0, 5);
                $field.val(val);
            }
            value[$thisIndex] = val;
        });
    },

    PesquisarBps: function (_UGE, _Ua, _Divisao) {
        $.get(sam.path.webroot + "Sections/VerificarBPsResponsavel", {UGE: _UGE, Ua: _Ua, Divisao: _Divisao }, function (data) {
            try {
                if (data[0].Id != 0) {
                    $('#modal').modal('show');
                    if (data[0].Id == 1) {
                        $('#save-btn-modal').show();
                        $('#save-btn-modal').click(function () { $('.modalView').modal('hide'); $('#formSection').submit(); });
                        $('#fecharModal').text('Cancelar');
                    } else {
                        $('#save-btn-modal').hide();
                        $('#fecharModal').text('Fechar');
                    }
                    $('#MensagemModal').val(true);
                    //$('#save-btn-modal').click(function () { $('.modalView').modal('hide'); $('#formSection').submit(); });
                    $('#fecharModal').click(function () { $('.modalView').modal('hide'); $('#MensagemModal').val('False'); });
                    $('#mensagemmodal').html(data[0].Mensagem);
                    $('#modal').on('hidden.bs.modal', function () {
                        $('#MensagemModal').val('False');
                    });
                }else{
                    $('#MensagemModal').val(true)
                    $('#formSection').submit();
                }
            }
            catch (e) {
                // Possivelmente o perfil de usuário esta sem permisão de acesso.
                $('#modal').modal('show');
                $('#save-btn-modal').hide();
                $('#mensagemmodal').html("1: Não foi possivel realizar a pesquisa dos Bps do responsável, favor contatar o administrador responsável!");
            }
        });
    },
    submitForm: function () {
        $("#formSection").submit(function () {
            if ($('#MensagemModal').val() == "False") {
                var ResponsavelId = $('#ResponsibleId').val();
                var ResponsavelIdAux = $('#ResponsibleIdAux').val();

                if (ResponsavelId != ResponsavelIdAux) {
                    var Ua = $('#AdministrativeUnitId').val();
                    var UGE = $('#ManagerUnitId').val();
                    var Divisao = $('#Id').val();
                    sam.sections.PesquisarBps(UGE, Ua, Divisao);
                    $.unblockUI({ message: $('#modal-loading') });
                    return false;
                }
            }
        });
    },

    CriarDivMensagemModal: function () {
        var htmlBuilder = [];

        htmlBuilder.push('<div class="modal fade modalView" id="modal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel">');
        htmlBuilder.push('<div class="modal-dialog" role="document">');
        htmlBuilder.push('    <div class="modal-content">');
        htmlBuilder.push('        <div class="modal-header">');
        //htmlBuilder.push('            <button type="button" id="closemodal" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>');
        htmlBuilder.push('            <h4 class="modal-title" id="exampleModalLabel">Divisão</h4>');
        htmlBuilder.push('        </div>');
        htmlBuilder.push('        <div class="modal-body">');
        htmlBuilder.push('                    <div class="form-group">');
        htmlBuilder.push('                <label for="recipient-name" id="mensagemmodal" class="control-label"></label>');
        htmlBuilder.push('            </div>');
        htmlBuilder.push('        </div>');
        htmlBuilder.push('        <div class="modal-footer">');
        htmlBuilder.push('            <button type="button" id="save-btn-modal" class="btn submit-comentario btn-success btn-salvar">Alterar</button>');
        htmlBuilder.push('            <button type="button" id="fecharModal" class="btn btn-primary buttonClose" data-dismiss="modal">Cancelar</button>');
        htmlBuilder.push('        </div>');
        htmlBuilder.push('    </div>');
        htmlBuilder.push('</div>');
        htmlBuilder.push('</div>');
        htmlBuilder.join("")

        $(document.body).append(htmlBuilder.join(""));
    }
}