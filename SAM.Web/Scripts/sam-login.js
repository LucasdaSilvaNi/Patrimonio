samLogin = (function () {
    let form = '#formLogin';


    samLogin.prototype.submit = function () {
        $(form).submit(function () {
            $.get('./AssetPending/ImportarIntegracao', {}, function (data) {
                console.info("LoginIntegracao", data);
            });
        });
    };
});