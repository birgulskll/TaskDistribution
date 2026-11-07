using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using TaskDistribution.Business.Models;
using TaskDistribution.Web.Controllers;
using TaskDistribution.Web.Helpers;
using TaskDistribution.Web.ViewModels;

namespace TaskDistribution.Tests.Web;

public class GorevControllerTests
{
    private readonly SahteGorevService _gorevServisi = new();
    private readonly GorevController _controller;

    public GorevControllerTests()
    {
        _controller = new GorevController(_gorevServisi, new SahteIslemService())
        {
            TempData = new TempDataDictionary(new DefaultHttpContext(), new BosTempDataProvider())
        };
    }

    private static GorevFormViewModel GecerliForm() => new() { Baslik = "Rapor hazırla", IslemId = 2 };

    [Fact]
    public async Task OlusturGet_IslemListesiyleBosFormGosterir()
    {
        var sonuc = await _controller.Olustur();

        var view = Assert.IsType<ViewResult>(sonuc);
        var model = Assert.IsType<GorevFormViewModel>(view.Model);
        Assert.Equal(3, model.Islemler.Count);
    }

    [Fact]
    public async Task OlusturPost_FormGecersizse_ServisiCagirmazVeIslemListesiniTekrarDoldurur()
    {
        _controller.ModelState.AddModelError("IslemId", "Lütfen bir işlem seçin.");
        var form = new GorevFormViewModel { Baslik = "Rapor" };

        var sonuc = await _controller.Olustur(form);

        var view = Assert.IsType<ViewResult>(sonuc);
        Assert.Equal(3, ((GorevFormViewModel)view.Model!).Islemler.Count);   // Açılır liste boş kalmamalı
        Assert.Equal(0, _gorevServisi.OlusturCagrilmaSayisi);
    }

    [Fact]
    public async Task OlusturPost_SecilenIslemIdsiniServiseIletir()
    {
        await _controller.Olustur(GecerliForm());

        Assert.Equal(2, _gorevServisi.SonGelenIslemId);
    }

    [Fact]
    public async Task OlusturPost_AktifPersonelYoksa_HatayiFormdaGosterir()
    {
        _gorevServisi.DonecekSonuc = ServisSonucu.Hata("Sistemde aktif personel bulunmuyor. Görev atanamadı.");

        var sonuc = await _controller.Olustur(GecerliForm());

        Assert.IsType<ViewResult>(sonuc);
        Assert.Contains("aktif personel bulunmuyor", _controller.ModelState[string.Empty]!.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task OlusturPost_NormalAtamada_YesilMesajBirakir()
    {
        _gorevServisi.DonecekSonuc = ServisSonucu.Basari("Görev Ali Kaya adlı personele atandı.");

        var sonuc = await _controller.Olustur(GecerliForm());

        Assert.IsType<RedirectToActionResult>(sonuc);
        Assert.Equal("Görev Ali Kaya adlı personele atandı.", _controller.TempData[Bildirim.Basari]);
        Assert.Null(_controller.TempData[Bildirim.Uyari]);
    }

    [Fact]
    public async Task OlusturPost_KuralEsnetildiyse_SariUyariBirakir()
    {
        _gorevServisi.DonecekSonuc = ServisSonucu.Uyari("Ardışık zorluk kuralı esnetilerek atandı.");

        var sonuc = await _controller.Olustur(GecerliForm());

        Assert.IsType<RedirectToActionResult>(sonuc);
        Assert.Equal("Ardışık zorluk kuralı esnetilerek atandı.", _controller.TempData[Bildirim.Uyari]);
        Assert.Null(_controller.TempData[Bildirim.Basari]);
    }

    private sealed class BosTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();

        public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
    }
}
