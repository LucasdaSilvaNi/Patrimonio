sam.menu = {
    stringMenu: '',
    moduleLevel: 0,
    verticalMenu: '#menu-vertical',
    menuData: null,
    init: function () {
        sam.menu.bind();
    },
    bind: function () {
        $('.menu-botao').on('mouseover', function () {
            if ($(sam.menu.verticalMenu).css('visibility') == 'hidden') {
                $(sam.menu.verticalMenu).css('visibility', 'visible');
            }
        });

        $(sam.menu.verticalMenu).on('mouseleave', function () {
            $(sam.menu.verticalMenu).css('visibility', 'hidden');
        });

        $(sam.menu.verticalMenu).css('visibility', 'hidden');
    },
    getMenu: function (id) {

        sam.menu.stringMenu = '';

        $.blockUI({ message: sam.modal });
        $.post(sam.path.webroot + "Menu/GetMenu", { id: id }, function (data) {
            //console.log(data);

            if (data != null) {

                sam.menu.menuData = data;
                sam.menu.mountMenuSystems(data);
            }
            $.unblockUI({ message: sam.modal });
        });
    },
    mountMenuSystems: function (data) {

        $(sam.menu.verticalMenu).empty();

        sam.menu.stringMenu += '<ul class="vertical-menu">' +
                               '  <li>' +
                               '    <p class="menu-subtitulo">Escolha abaixo qual menu você deseja acessar</p>' +
                               '  </li>';
        //Laço para os sistemas (Atualmente Segurança e Patrimonio)
        $.each(data, function (i, managedSystem) {
            if (managedSystem.Name == "Estrutura Hierárquica") {
                sam.menu.menuData = data;
            }

            sam.menu.stringMenu += '<li>' +
                                   '  <a href="#">' + managedSystem.Name + '</a>' +
                                   '    <ul>' +
                                   '      <li>' +
                                   '        <p class="menu-titulo menu-linha">' + managedSystem.Name + '</p>' +
                                   //'        <p class="menu-subtitulo menu-linha">' + managedSystem.Description + '</p>' +
                                   '      </li>';
            //Modules
            sam.menu.mountMenuModules(managedSystem.ModulesViewModel, managedSystem.Id, null);

            sam.menu.stringMenu += '    </ul>' +
                                   '</li>';
        });

        //sam.menu.stringMenu +=  '<li>                                       ' +
        //                        '  <a href="/Patrimonio/Principal">Home</a> ' +
        //                        '</li>                                      ';

        sam.menu.stringMenu += '</ul>';

        $(sam.menu.verticalMenu).append(sam.menu.stringMenu);

        //Start events to menu
        amazonmenu.init({ menuid: 'menu-vertical' });
    },
    mountMenuModules: function (data, managedSystem, parentId) {

        $.each(data, function (j, module) {

            if (module.ManagedSystemId == managedSystem) {

                //check if module root
                if (parentId != '' && parentId != null) {
                    if (module.ParentId == parentId) {
                        sam.menu.mountModuleHeader(module);
                        sam.menu.stringMenu += '<li><a href="' + module.Path + '">' + module.Name + '</a></li>';
                        sam.menu.mountMenuModules(data, managedSystem, module.Id);
                    }

                } else {
                    //Is root
                    if (module.ParentId == '' || module.ParentId == null) {

                        sam.menu.mountModuleHeader(module);

                        sam.menu.stringMenu += '<li>' +
                                               '  <a href="#">' + module.Name + '</a>' +
                                               '    <ul>' +
                                               '      <li>' +
                                               '        <p class="menu-titulo menu-linha">' + module.Name + '</p>' +
                                               //'        <p class="menu-subtitulo menu-linha">' + module.Description + '</p>' +
                                               '      </li>';

                        sam.menu.mountMenuModules(data, managedSystem, module.Id);

                        sam.menu.stringMenu += '    </ul>' +
                                               '</li>';
                    }
                }
            }
        });
    },
    mountModuleHeader: function (module) {
        //check if contains header for module
        if (module.MenuName != '' && module.MenuName != null) {
            sam.menu.stringMenu += '<li>' +
                                   '  <p class="menu-titulo menu-linha">' + module.MenuName + '</p>' +
                                   //'  <p class="menu-subtitulo menu-linha">' + module.Description + '</p>' +
                                   '</li>';
        }
    }
}