sam.utils = {
    init: function () {
        sam.utils.masks();
        sam.utils.controlSearchInSelectsElements();
        sam.utils.controlDisabeldElement();
        sam.utils.somenteNumeros();
        sam.utils.formatarMoeda();
        sam.utils.somenteNumeroskeypress();
    },
    masks: function () {
        $(".mascara-telefone").mask("(99) 9999?9-9999");
        $(".mascara-telefone").on("blur", function () {
            var last = $(this).val().substr($(this).val().indexOf("-") + 1);

            if (last.length == 3) {
                var move = $(this).val().substr($(this).val().indexOf("-") - 1, 1);
                var lastfour = move + last;
                var first = $(this).val().substr(0, 9);

                $(this).val(first + '-' + lastfour);
            }
        });

        $(".mascara-cpf").mask("999.999.999.99");
        $(".mascara-cnpj").mask("99.999.999/9999-99");
        $(".mascara-cep").mask("99999-999");
    },
    controlSearchInSelectsElements: function () {
        //Create input text box for search in selects elements.
        $(".chosen-select").chosen();
    },
    controlDisabeldElement: function () {
        $(".disabledElement").attr('readonly', 'readonly');
    },
    getWebRoot: function (root) {
        //for define route in localhost environment and production environment /SAM/Patrimonio/
        if (root == '/')
            sam.path.webroot = '';
        else
            sam.path.webroot = root;
    },
    somenteNumeros: function () {
        $('.somenteNumerosInicial').keypress(function (event) {
            var tecla = (window.event) ? event.keyCode : event.which;
            if ((tecla > 47 && tecla < 58)) return true;
            else {
                if (tecla != 8) return false;
                else return true;
            }
        });
        $('.somenteNumerosFinal').keypress(function (event) {
            var tecla = (window.event) ? event.keyCode : event.which;
            if ((tecla > 47 && tecla < 58)) return true;
            else {
                if (tecla != 8) return false;
                else return true;
            }
        });
    },
    formatarMoeda: function () {
        //$('.sam-moeda').maskMoney({ showSymbol: false, symbol: "R$", decimal: ",", thousands: "." });
        //$('.sam-moeda').addClass('mascaraImplementada');

        //Atribui as propriedades maskMoney somente para quem não tem a classe "mascaraImplementada", para evitar erro de duplicação de máscara
        $("input[type=text][class~=sam-moeda]:not([class~=mascaraImplementada])").maskMoney({ showSymbol: false, symbol: "R$", decimal: ",", thousands: "." });
        $('.sam-moeda').addClass('mascaraImplementada');
    },
    getURLParameter: function (url, name) {
        return (RegExp(name + '=' + '(.+?)(&|$)').exec(url) || [, null])[1];
    },

    somenteNumeroskeypress: function () {
        $('.somenteNumerosKeypress').keypress(function (event) {
            var tecla = (window.event) ? event.keyCode : event.which;
            if ((tecla > 47 && tecla < 58)) return true;
            else {
                if (tecla != 8) return false;
                else return true;
            }
        });
    },
    unformatNumber: function (pNum) {
        return String(pNum).replace(/\D/g, "").replace(/^0+/, "");
    },
    isCpf: function (cpf) {
        var soma;
        var resto;
        var i;

        if ((cpf.length != 11) ||
        (cpf == "00000000000") || (cpf == "11111111111") ||
        (cpf == "22222222222") || (cpf == "33333333333") ||
        (cpf == "44444444444") || (cpf == "55555555555") ||
        (cpf == "66666666666") || (cpf == "77777777777") ||
        (cpf == "88888888888") || (cpf == "99999999999")) {
            return false;
        }

        soma = 0;

        for (i = 1; i <= 9; i++) {
            soma += Math.floor(cpf.charAt(i - 1)) * (11 - i);
        }

        resto = 11 - (soma - (Math.floor(soma / 11) * 11));

        if ((resto == 10) || (resto == 11)) {
            resto = 0;
        }

        if (resto != Math.floor(cpf.charAt(9))) {
            return false;
        }

        soma = 0;

        for (i = 1; i <= 10; i++) {
            soma += cpf.charAt(i - 1) * (12 - i);
        }

        resto = 11 - (soma - (Math.floor(soma / 11) * 11));

        if ((resto == 10) || (resto == 11)) {
            resto = 0;
        }

        if (resto != Math.floor(cpf.charAt(10))) {
            return false;
        }

        return true;
    },

    isCnpj: function (s) {
        var i;
        var c = s.substr(0, 12);
        var dv = s.substr(12, 2);
        var d1 = 0;

        for (i = 0; i < 12; i++) {
            d1 += c.charAt(11 - i) * (2 + (i % 8));
        }

        if (d1 == 0) return false;

        d1 = 11 - (d1 % 11);

        if (d1 > 9) d1 = 0;
        if (dv.charAt(0) != d1) {
            return false;
        }

        d1 *= 2;

        for (i = 0; i < 12; i++) {
            d1 += c.charAt(11 - i) * (2 + ((i + 1) % 8));
        }

        d1 = 11 - (d1 % 11);

        if (d1 > 9) d1 = 0;
        if (dv.charAt(1) != d1) {
            return false;
        }

        return true;
    },

    isCpfCnpj: function (valor) {
        var retorno = false;
        var numero = valor;

        numero = sam.utils.unformatNumber(numero);
        if (numero.length > 11) {
            if (sam.utils.isCnpj(numero)) {
                retorno = true;
            }
        } else {
            if (sam.utils.isCpf(numero)) {
                retorno = true;
            }
        }

        return retorno;
    },

    somenteNumerosPartial: function () {
        $('.somenteNumerosPartial').keypress(function (event) {
            var tecla = (window.event) ? event.keyCode : event.which;
            if ((tecla > 47 && tecla < 58)) return true;
            else {
                if (tecla != 8) return false;
                else return true;
            }
        });
    },

    maskcpfcnpj: function () {
        $(".maskcpfcnpj").unmask();
        $(".maskcpfcnpj").focusout(function () {
            $(".maskcpfcnpj").unmask();
            var tamanho = $(".maskcpfcnpj").val().replace(/\D/g, '').length;
            if (tamanho == 11) {
                $(".maskcpfcnpj").mask("999.999.999-99");
            } else if (tamanho == 14) {
                $(".maskcpfcnpj").mask("99.999.999/9999-99");
            }
        });
        $(".maskcpfcnpj").focusin(function () {
            $(".maskcpfcnpj").unmask();
        });
    },
     getDateFromAspNetFormat:function(date) {
        const re = /-?\d+/;
        const m = re.exec(date);
        return parseInt(m[0], 10);
    }
}
