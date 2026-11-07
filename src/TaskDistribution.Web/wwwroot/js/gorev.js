// Görev oluşturma formu: seçilen işlemin zorluğunu ekranda gösterir.
// Bu sadece kullanıcıya bilgi vermek içindir; sunucu zorluğu her zaman veritabanındaki işlemden alır.
(function () {
    const secim = document.getElementById("islemSecimi");
    const gosterim = document.getElementById("zorlukGosterimi");

    if (!secim || !gosterim) {
        return;
    }

    function zorluguGoster() {
        const secilen = secim.options[secim.selectedIndex];
        const zorluk = secilen ? secilen.dataset.zorluk : null;

        if (!zorluk) {
            gosterim.className = "text-muted";
            gosterim.textContent = "İşlem seçilince görünür";
            return;
        }

        gosterim.className = "badge " + secilen.dataset.css;
        gosterim.textContent = zorluk + " · " + secilen.dataset.etiket;
    }

    secim.addEventListener("change", zorluguGoster);

    // Form hatayla geri döndüğünde daha önce seçilmiş işlemin zorluğu da görünsün
    zorluguGoster();
})();
