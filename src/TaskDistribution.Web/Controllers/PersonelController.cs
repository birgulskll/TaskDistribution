using Microsoft.AspNetCore.Mvc;
using TaskDistribution.Business.Services;
using TaskDistribution.Web.Helpers;
using TaskDistribution.Web.ViewModels;

namespace TaskDistribution.Web.Controllers;

/// <summary>
/// Personel sayfaları. Dikkat: DbContext veya repository YOK; sadece IPersonelService kullanılıyor.
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

    // GET /Personel/Ekle → boş formu göster
    public IActionResult Ekle()
    {
        return View(new PersonelFormViewModel());
    }

    // POST /Personel/Ekle → formdan gelen veriyi kaydet
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Ekle(PersonelFormViewModel model)
    {
        // 1. Formdaki [Required], [StringLength] kuralları sağlanmadıysa formu hatalarla geri göster
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // 2. Asıl iş kuralları Business katmanında
        var sonuc = await _personelService.EkleAsync(model.Ad, model.Soyad);

        if (!sonuc.Basarili)
        {
            ModelState.AddModelError(string.Empty, sonuc.Mesaj);
            return View(model);
        }

        // 3. Başarılı: mesajı TempData'ya koy ve listeye yönlendir (Post-Redirect-Get)
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

    // POST /Personel/AktiflikDegistir/5 → aktif ise pasif, pasif ise aktif yap
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AktiflikDegistir(int id)
    {
        var sonuc = await _personelService.AktiflikDurumunuDegistirAsync(id);

        TempData[sonuc.Basarili ? Bildirim.Basari : Bildirim.Hata] = sonuc.Mesaj;
        return RedirectToAction(nameof(Index));
    }
}
