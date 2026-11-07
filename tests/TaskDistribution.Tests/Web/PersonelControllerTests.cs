using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using TaskDistribution.Business.Models;
using TaskDistribution.Domain.Entities;
using TaskDistribution.Web.Controllers;
using TaskDistribution.Web.Helpers;
using TaskDistribution.Web.ViewModels;

namespace TaskDistribution.Tests.Web;

public class PersonelControllerTests
{
    private readonly SahtePersonelService _servis = new();
    private readonly PersonelController _controller;

    public PersonelControllerTests()
    {
        _controller = new PersonelController(_servis)
        {
            // Testte gerçek bir web isteği olmadığı için TempData'yı elle hazırlıyoruz.
            TempData = new TempDataDictionary(new DefaultHttpContext(), new BosTempDataProvider())
        };
    }

    [Fact]
    public async Task Index_PersonelListesiniViewaGonderir()
    {
        _servis.Personeller.Add(new Personel { Id = 1, Ad = "Ayşe", Soyad = "Yılmaz" });
        _servis.Personeller.Add(new Personel { Id = 2, Ad = "Ali", Soyad = "Kaya" });

        var sonuc = await _controller.Index();

        var view = Assert.IsType<ViewResult>(sonuc);
        var model = Assert.IsType<List<Personel>>(view.Model);
        Assert.Equal(2, model.Count);
    }

    [Fact]
    public async Task EklePost_FormGecersizse_ServisiCagirmadanFormuGeriGosterir()
    {
        // Arrange: [Required] kuralının bozulduğunu taklit et
        _controller.ModelState.AddModelError("Ad", "Ad boş olamaz.");
        var model = new PersonelFormViewModel { Soyad = "Kaya" };

        // Act
        var sonuc = await _controller.Ekle(model);

        // Assert
        var view = Assert.IsType<ViewResult>(sonuc);
        Assert.Same(model, view.Model);
        Assert.Equal(0, _servis.EkleCagrilmaSayisi);
    }

    [Fact]
    public async Task EklePost_ServisHataDonerse_HatayiFormdaGosterir()
    {
        _servis.DonecekSonuc = ServisSonucu.Hata("Ad en fazla 50 karakter olabilir.");

        var sonuc = await _controller.Ekle(new PersonelFormViewModel { Ad = "x", Soyad = "y" });

        Assert.IsType<ViewResult>(sonuc);
        Assert.False(_controller.ModelState.IsValid);
        Assert.Contains("50 karakter", _controller.ModelState[string.Empty]!.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task EklePost_Basariliysa_MesajBirakipListeyeYonlendirir()
    {
        _servis.DonecekSonuc = ServisSonucu.Basari("Deniz Ak eklendi.");

        var sonuc = await _controller.Ekle(new PersonelFormViewModel { Ad = "Deniz", Soyad = "Ak" });

        var yonlendirme = Assert.IsType<RedirectToActionResult>(sonuc);
        Assert.Equal(nameof(PersonelController.Index), yonlendirme.ActionName);
        Assert.Equal("Deniz Ak eklendi.", _controller.TempData[Bildirim.Basari]);
    }

    [Fact]
    public async Task DuzenleGet_OlmayanPersonel_404Doner()
    {
        var sonuc = await _controller.Duzenle(999);

        Assert.IsType<NotFoundResult>(sonuc);
    }

    [Fact]
    public async Task AktiflikDegistir_ServisHataDonerse_HataMesajiBirakir()
    {
        _servis.DonecekSonuc = ServisSonucu.Hata("Personel bulunamadı.");

        var sonuc = await _controller.AktiflikDegistir(999);

        Assert.IsType<RedirectToActionResult>(sonuc);
        Assert.Equal("Personel bulunamadı.", _controller.TempData[Bildirim.Hata]);
        Assert.Null(_controller.TempData[Bildirim.Basari]);
    }
}
