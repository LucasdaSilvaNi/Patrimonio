using System.Web.Optimization;

namespace SAM.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles").Include(
                        "~/Scripts/modernizr-*",
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/methods_pt.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/amazonmenu.css",
                      "~/Content/PagedList.css",
                      "~/Content/sam.css",
                      "~/Content/sam-asset.css",
                      "~/Content/sam-relationshipUserProfile.css",
                      "~/Content/sam-ticket.css",
                      "~/Content/sam-transfer.css",
                      "~/Content/sam-users.css"));

            bundles.Add(new StyleBundle("~/lib/css").Include(
                      "~/lib/chosen-v1.4.2/chosen.css",
                      "~/lib/maxazan-jquery-treegrid-5a0511e/css/jquery.treegrid.css",
                      "~/lib/icheck-1.x/skins/minimal/_all.css",
                      "~/lib/icheck-1.x/skins/square/_all.css",
                      "~/lib/icheck-1.x/skins/flat/_all.css",
                      "~/lib/icheck-1.x/skins/line/_all.css",
                      "~/lib/icheck-1.x/skins/polaris/polaris.css",
                      "~/lib/icheck-1.x/skins/futurico/futurico.css"));

            bundles.Add(new StyleBundle("~/Scripts/css").Include(
                      "~/Scripts/bootstrap-datepicker/dist/css/bootstrap-datepicker.css"));

            bundles.Add(new ScriptBundle("~/Layout_1/js").Include(
                      "~/Scripts/sam-usuario-perfil.js"));

            bundles.Add(new ScriptBundle("~/Layout_2/js").Include(
                      "~/lib/icheck-1.x/icheck.js"));

            bundles.Add(new ScriptBundle("~/Layout_3/js").Include(
                      "~/Scripts/jquery.maskedinput.min.js",
                      "~/Scripts/amazonmenu.js",
                      "~/lib/chosen-v1.4.2/chosen.jquery.js",
                      "~/lib/jquery.blockUI.1.33/jquery.blockUI.js",
                      "~/lib/jquery.inputmask-3.x/dist/inputmask/inputmask.js",
                      "~/lib/jquery.inputmask-3.x/dist/inputmask/jquery.inputmask.js",
                      "~/lib/maxazan-jquery-treegrid-5a0511e/js/jquery.treegrid.js",
                      "~/Scripts/bootstrap-datepicker/dist/js/bootstrap-datepicker.js",
                      "~/Scripts/bootstrap-datepicker/dist/locales/bootstrap-datepicker.pt-BR.min.js",
                      "~/Scripts/bootstrap-colorpicker/dist/js/bootstrap-colorpicker.js",
                      "~/Scripts/jquery-bootpag/lib/jquery.bootpag.min.js",
                      "~/Scripts/sam.js",
                      "~/Scripts/sam-global.js",
                      "~/Scripts/sam-asset.js",
                      "~/Scripts/sam-path.js",
                      "~/Scripts/sam-menu.js",
                      "~/Scripts/sam-relationshipUserProfile.js",
                      "~/Scripts/sam-search.js",
                      "~/Scripts/sam-transaction.js",
                      "~/Scripts/sam-transferencia.js"));


            bundles.Add(new ScriptBundle("~/Layout_4/js").Include(
                      "~/Scripts/sam-mask-money.js",
                      "~/Scripts/sam-material-item.js",
                      "~/Scripts/sam-prototype.js",
                      "~/Scripts/sam-perfil-login.js",
                      "~/Scripts/sam-auxiliary-account.js",
                      "~/Scripts/sam-costcenter.js",
                      "~/Scripts/sam-responsible.js",
                      "~/Scripts/sam-sections.js",
                      "~/Scripts/sam-supplyunit.js",
                      "~/Scripts/sam-signature.js",
                      "~/Scripts/sam-repair.js",
                      "~/Scripts/sam-exchange.js",
                      "~/Scripts/sam-date.js",
                      "~/Scripts/sam-inventario.js",
                      "~/Scripts/sam-item-inventario.js",
                      "~/Scripts/sam-commun.js",
                      "~/Scripts/sam-utils.js"));

            // MUDANCA DE COMPONENTE DROPDOW PARA DROPDOW SEARCH
            bundles.Add(new ScriptBundle("~/Layout_5/js").Include(
                "~/Scripts/bootstrap-select-1.12.4/js/bootstrap-select.js"));

            bundles.Add(new StyleBundle("~/Layout_5/css").Include(
                "~/Scripts/bootstrap-select-1.12.4/dist/css/bootstrap-select.css"));

            bundles.Add(new StyleBundle("~/IncorporacaoEmLote/css").Include(
                "~/Scripts/bootstrap-select-1.12.4/dist/css/bootstrap-select.css",
                "~/Content/datatables/css/jquery.dataTables.min.css",
                "~/Content/datatables/css/dataTables.bootstrap.min.css"));

            bundles.Add(new StyleBundle("~/IncorporacaoEmLote/js").Include(
                "~/Scripts/datatables/js/jquery.dataTables.min.js",
                "~/Scripts/datatables/js/dataTables.bootstrap.min.js",
                "~/Scripts/sam-incorporacao-lote.js"));


            bundles.Add(new ScriptBundle("~/AssetPending/Edit/js").Include(
                "~/Scripts/sam-assetpending.js"));

            //Ao analisar a partialView que carrega esse script, será visto que ela só seria chamada
            // pela partialView de incorporação "Incorporação por Compra Direta", que se enontra inativada (25/11/2019)
            //bundles.Add(new ScriptBundle("~/Assets/_partialBuscarFornecedor/js").Include(
            //    "~/Scripts/sam-search.js"));

            bundles.Add(new ScriptBundle("~/Assets/movimento/js").Include(
                "~/scripts/sam-movimento.js"));

            bundles.Add(new ScriptBundle("~/Assets/ReportAnaliticoDeBemPatrimonial/js").Include(
                "~/Scripts/sam-reportAnaliticoDeBemPatrimonial.js"));

            bundles.Add(new ScriptBundle("~/Assets/Closing/js").Include(
                "~/Scripts/sam-closing.js"));

            bundles.Add(new ScriptBundle("~/ReclassificacaoContabil/js").Include(
                "~/Scripts/sam-reclassificacao-contabil.js"));

            bundles.Add(new ScriptBundle("~/MovimentoBolsa/Movimento/js").Include(
                "~/Scripts/sam-movimento.js"));

            bundles.Add(new ScriptBundle("~/Movimento/Create/js").Include(
                "~/Scripts/sam-movimento.js"));


            bundles.Add(new StyleBundle("~/Movimento/Index_1/css").Include(
                "~/Scripts/notification-menu/notification_menu/css/style_light.css"));

            bundles.Add(new ScriptBundle("~/Movimento/Index_2/js").Include(
                "~/Scripts/sam-movimento.js",
                "~/Scripts/notification-menu/notification_menu/js/ttw-notification-menu.js"));

            bundles.Add(new StyleBundle("~/Notification/Create_1/css").Include(
                "~/Content/sam-notification.css"));

            bundles.Add(new ScriptBundle("~/Notification/Create_2/js").Include(
                "~/Scripts/summernote-develop/dist/summernote.js",
                "~/Scripts/sam-notification.js"));


            bundles.Add(new StyleBundle("~/Notification/Edit_1/css").Include(
                "~/Content/sam-notification.css"));

            bundles.Add(new ScriptBundle("~/Notification/Edit_2/js").Include(
                "~/Scripts/summernote-develop/dist/summernote.js",
                "~/Scripts/sam-notification.js"));


            bundles.Add(new ScriptBundle("~/PlateBarCode/ReportPlateBarCode/js").Include(
                "~/Scripts/sam-reportPlateBarCode.js"));

            bundles.Add(new ScriptBundle("~/Principal/Index/js").Include(
                "~/Scripts/summernote-develop/dist/summernote.js"));


            bundles.Add(new ScriptBundle("~/PrintBarCode/ReportPrintBarCode/js").Include(
                "~/Scripts/sam-reportPrintBarCode.js"));

            bundles.Add(new StyleBundle("~/RelationshipUserProfile/index_1/css").Include(
                "~/Content/sam-relationshipUserProfile.css"));


            bundles.Add(new ScriptBundle("~/RelationshipUserProfile/index_2/js").Include(
                "~/scripts/sam-relationshipTransactionsProfiles.js"));

            bundles.Add(new ScriptBundle("~/Support/js").Include(
                "~/Scripts/sam-support.js"));

            bundles.Add(new ScriptBundle("~/NLComplementar/Fechamento/js").Include(
                "~/Scripts/sam-hierarquiaIntegradaSiafem.js"));

            bundles.Add(new ScriptBundle("~/ConfrontoContabil/js").Include(
                "~/Scripts/sam-hierarquiaIntegradaSiafem.js",
                "~/Scripts/sam-confrontoContabil.js"));

            bundles.Add(new StyleBundle("~/Support/Edit_2/css").Include(
                "~/lib/jquery-ui-1.12.1/jquery-ui.min.css"));

            bundles.Add(new StyleBundle("~/Home/css").Include(
                "~/lib/owl-carousel/owl.carousel.css",
                "~/lib/owl-carousel/owl.theme.css"
                ));

            bundles.Add(new ScriptBundle("~/Home/js").Include(
                "~/lib/owl-carousel/owl.carousel.js"));
        }
    }
}