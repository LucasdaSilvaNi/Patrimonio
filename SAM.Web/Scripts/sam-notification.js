var notification = {

    Load: function () {
        notification.EventoEditorDeTexto();
        notification.EventoSubmit();
        notification.EventoCheckMostrar();
    },

    EventoEditorDeTexto: function(){
        $('.summernote').summernote({
            height: 450,
            focus: true,
            tabsize: 2,
            codemirror: {
                mode: 'text/html',
                htmlMode: true,
                lineNumbers: true,
                theme: 'monokai'
            }
        });
    },

    EventoSubmit: function () {
        $("#formNotificacao").submit(function () {
            var corpoMensagemHtml = $('#CorpoMensagem').code();
            $('#CorpoMensagem').val(corpoMensagemHtml);
        });
    },
    EventoCheckMostrar: function () {
        $('input#Status').on('ifChanged', function (event) {
            if ($(this).is(":checked")) {
                $('#Status').val('True');
            }
            else {
                $('#Status').val('False');
            }
        });
    }
}