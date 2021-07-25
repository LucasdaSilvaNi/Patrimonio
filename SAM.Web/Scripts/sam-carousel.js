//Antigo jeito de usar o carousel na página inicial do Patrimonio
sam.carousel = {
    init: function () {
        sam.carousel.bind();
    },
    bind: function () {
        $(".owl-carousel").owlCarousel({
            autoPlay: 3000, //Set AutoPlay to 3 seconds

            items: 1,
            itemsDesktop: [768, 3], //abaixo de 768 de largura
            itemsDesktopSmall: [640, 3] //abaixo de 640 de largura
        });
    }
}