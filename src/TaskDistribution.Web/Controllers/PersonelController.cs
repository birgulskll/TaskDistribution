using Microsoft.AspNetCore.Mvc;
using TaskDistribution.Business.Services;
using TaskDistribution.Web.Helpers;
using TaskDistribution.Web.ViewModels;

namespace TaskDistribution.Web.Controllers;

/// <summary>
/// Personel CRUD sayfaları. Yalnızca IPersonelService kullanır.
/// </summary>
public class PersonelController : Controller
{
    private readonly IPersonelService _personelService;

    public PersonelController(IPersonelService personelService)
    {
        _personelService = personelService;
    }

    // GET /Personel
    public async Task<IActionResult> Index()
    {
        var personeller = await _personelService.TumunuGetirAsync();
        return View(personeller);
    }

    // GET /Personel/Ekle
    public IActionResult Ekle()
    {
        return View(new PersonelFormViewModel());
    }

    // POST /Personel/Ekle
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Ekle(PersonelFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var sonuc = await _personelService.EkleAsync(model.Ad, model.Soyad);

        if (!sonuc.Basarili)
        {
            ModelState.AddModelError(string.Empty, sonuc.Mesaj);
            return View(model);
        }

        // Post-Redirect-Get: mesaj TempData ile listeye taşınır
        TempData[Bildirim.Basari] = sonuc.Mesaj;
        return RedirectToAction(nameof(Index));
    }

    // GET /Personel/Duzenle/5
    public async Task<IActionResult> Duzenle(int id)
    {
        var personel = await _personelService.GetirAsync(id);
        if (personel is null)
        {
            return NotFound();
        }

        var model = new PersonelFormViewModel
        {
            Id = personel.Id,
            Ad = personel.Ad,
            Soyad = personel.Soyad
        };

        return View(model);
    }

    // POST /Personel/Duzenle/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Duzenle(int id, PersonelFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var sonuc = await _personelService.GuncelleAsync(id, model.Ad, model.Soyad);

        if (!sonuc.Basarili)
        {
            ModelState.AddModelError(string.Empty, sonuc.Mesaj);
            return View(model);
        }

        TempData[Bildirim.Basari] = sonuc.Mesaj;
        return RedirectToAction(nameof(Index));
    }

    // GET /Personel/Sil/5 (onay sayfası)
    public async Task<IActionResult> Sil(int id)
    {
        var personel = await _personelService.GetirAsync(id);
        if (personel is null)
        {
            return NotFound();
        }

        return View(personel);
    }

    // POST /Personel/Sil/5
    // Aynı imzalı iki "Sil" metodu olamayacağı için adı SilOnayla; [ActionName] ile adres değişmez.
    [HttpPost]
    [ActionName("Sil")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SilOnayla(int id)
    {
        var sonuc = await _personelService.SilAsync(id);

        TempData[sonuc.Basarili ? Bildirim.Basari : Bildirim.Hata] = sonuc.Mesaj;
        return RedirectToAction(nameof(Index));
    }

    // POST /Personel/AktiflikDegistir/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AktiflikDegistir(int id)
    {
        var sonuc = await _personelService.AktiflikDurumunuDegistirAsync(id);

        TempData[sonuc.Basarili ? Bildirim.Basari : Bildirim.Hata] = sonuc.Mesaj;
        return RedirectToAction(nameof(Index));
    }
}
