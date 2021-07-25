sam.repair = {
    btnConsultaConserto: '.btnConsulta-conserto',
    btnConsultaAssets: '.btnConsulta-assets',
    limparNumero: '.limpar-numero',
    submit:'.submit',
    inputDatetime: '.input-datetime',
    inputDatetimeObrigatorio: '.inputDatetimeObrigatorio',
    checkSelecionados: '.control-checkSelecionados',
    checkSelecionar: '.check-selecionar',
    tableAsset: '.table-asset',
    assetConsuta:'.asset-consuta',
    init:function(){
        $('[data-toggle="tooltip"]').tooltip();
    },
    load: function () {
        sam.repair.limparNumerosNoLoadDaPagina();
        sam.repair.maxlengthParaTipoNumber();
        sam.repair.checkedAssets();
        sam.repair.consultaAsset();
        sam.repair.Submit();
    },
    limparNumerosNoLoadDaPagina: function () {
        var elements = $(sam.repair.limparNumero);
        elements.each(function (value, key) {
            if (key.value == '0') {
                key.value = '';
            }
        })
        
    },
    limparDatas:function(){
        $(sam.repair.inputDatetime).val('');
    },
    Submit: function () {
        $(sam.repair.submit).submit(function () {
            //return sam.repair.dataTimeObrigatorioConsertoRetorno();
            $('.sam-moeda').val($('.sam-moeda').val().replace('.', ''));
        });
    },
    maxlengthParaTipoNumber: function () {
        var value = [];
        $(".somenteNumeros").each(function (i) {
            value[i] = this.defaultValue;
            $(this).data("idx", i);
        });
        $(".somenteNumeros").on("keyup", function (e) {
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
    dataTimeObrigatorioConsertoRetorno: function () {
        if($(sam.repair.dataTimeObrigatorio).val(''))
        {
            $('span[data-valmsg-for="ReturnDate"]').text('Número da chapa final menor que chapa inicial');
            return false;
        }
    },
    checkedAssets: function () {
        $(sam.repair.checkSelecionar).click(function () {
            var ckecked = $(this).prop('checked');
            var assetId = $(this).attr('data-id');

            if (ckecked == true)
                sam.repair.adicionarAsset(assetId);
            else
                sam.repair.removerAsset(assetId);
        })
    },
    adicionarAsset:function(assetId){
        var element =  $(sam.repair.checkSelecionados);
        var value = element.val();

        if(value.length < 1)
            value= assetId;
        else
            value = value + ':' + assetId;

        element.val(value);
        
    },
    removerAsset:function(assetId){
        var element =  $(sam.repair.checkSelecionados);
        var value;

        var assetsId = element.val().split(':');
        for(var i =0; i <= assetsId.length - 1; i++){
            if(assetsId[i] != assetId){
                if (value == undefined || value =='')
                    value= assetId;
                else
                    value = value + ':' + assetId;
            }
        }

        element.val(value);
    },

    consultaAsset: function () {
        $(sam.repair.btnConsultaAssets).click(function () {
            var nomeDoItemMaterial = $(sam.repair.assetConsuta).val();
            var AssetId = 0
            $.post(sam.path.webroot + "/Repair/GridAsset", { nomeDoItemMaterial: nomeDoItemMaterial }, function (data) {
                var htmlBuilderAsset = [];
                $('.tr-assets').remove();
                $.each(data, function (i, asset) {
                    if (AssetId != asset.AssetId) {
                        htmlBuilderAsset.push('<tr class="tr tr-assets">');
                        htmlBuilderAsset.push('<td>');
                        htmlBuilderAsset.push('<input type="checkbox" id="checkAsset" class="checkbox check-selecionar" data-id="' + asset.AssetId + '" />');
                        htmlBuilderAsset.push('</td>');
                        htmlBuilderAsset.push('<td>' + asset.NumberIdentification + '</td>');
                        htmlBuilderAsset.push('<td>' + asset.MaterialDescricao + '</td>');
                        htmlBuilderAsset.push('<td>' + asset.UaId + '</td>');
                        htmlBuilderAsset.push('<td>' + asset.ValorAtual + '</td>');
                        htmlBuilderAsset.push('</tr>');

                        $(sam.repair.tableAsset).append(htmlBuilderAsset.join(""));
                        htmlBuilderAsset = [];
                    }
                });
                sam.repair.checkedAssets();
            });
     
        });
        
    }
           
}