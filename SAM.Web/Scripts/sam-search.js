sam.search = {
    cont: 0,
    init: function () {
        sam.search.bind();
    },
    bind: function () {
        sam.search.eventoClickPesquisa();
        sam.search.eventoClickSalvar();
    },
    eventoClickPesquisa: function () {
        $('#spanPesquisa').on('click', function () {
            sam.search.submetePesquisa();
        });
    },
    submetePesquisa: function () {
        //Armazena no campo oculto a string de pesquisa, para que ela possa ser submetida
        $('#searchString').val($('#search-tables').val());

        //Para a tela de ItemInventario
        if ($('#InventarioId') != null)
            $('#InventarioId').val($('#numeroDoInventario').val());

        //Efetua o submit
        $('#consultaForm').submit();
    },
    clickEnter: function () {
        var keycode;
        if (window.event)
            keycode = window.event.keyCode;

        //Caso o botão pressionado seja o enter
        if (keycode == 13) {
            sam.search.submetePesquisa();
            return false;
        }
        else {
            return true;
        }
    },
    clickEnterDataTable: function () {
        var keycode;
        if (window.event)
            keycode = window.event.keyCode;

        //Caso o botão pressionado seja o enter
        if (keycode == 13) {
            $("#spanPesquisa").click();
            return false;
        }
        else {
            return true;
        }
    },
    clickEnterDataTableComParametro: function (a,b,id) {
        var keycode;
        if (window.event)
            keycode = window.event.keyCode;

        //Caso o botão pressionado seja o enter
        if (keycode == 13) {
            $(id).click();
            return false;
        }
        else {
            return true;
        }
    },
    clickEnterSubmit: function () {
        var keycode;
        if (window.event)
            keycode = window.event.keyCode;

        //Caso o botão pressionado seja o enter
        if (keycode == 13 || keycode == 8) {
            $("#btnSalvarSubmit").attr("readonly", "readonly");

            if (sam.search.cont > 0)
                event.preventDefault();

            sam.search.cont = 1;
        }
        else {
            return true;
        }
    },

    eventoClickSalvar: function () {
        $("#btnSalvarSubmit").on("mousedown", function (event) {
           $("#btnSalvarSubmit").attr("readonly", "readonly");
            if (sam.search.cont > 0)
                event.preventDefault();

            sam.search.cont = 1;
        });
    }
}