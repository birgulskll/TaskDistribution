using Microsoft.AspNetCore.Mvc;
using TaskDistribution.Business.Services;
using TaskDistribution.Web.Helpers;
using TaskDistribution.Web.ViewModels;

namespace TaskDistribution.Web.Controllers;

/// <summary>
/// Görev listesi ve görev oluşturma. Atama kararını Business katmanı verir.
/// </summary>
public class GorevController : Controller
{
    private readonly IGorevService _gorevService;
    private readonly IIslemService _islemService;

    public GorevController(IGorevService gorevService, IIslemService islemService)
    {
        _gorevService = gorevService;
        _islemService = islemService;
    }

    // GET /Gorev
    public async Task<IActionResult> Index()
    {
        var gorevler = await _gorevService.TumunuGetirAsync();
        return View(gorevler);
    }

    // GET /Gorev/Olustur
    public async Task<IActionResult> Olustur()
    {
        var model = new GorevFormViewModel();
        await IslemleriDoldurAsync(model);
        return View(model);
    }

    // POST /Gorev/Olustur
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Olustur(GorevFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Form hatalarla geri gösterilecek; açılır liste boş kalmasın diye işlemleri tekrar doldur
            await IslemleriDoldurAsync(model);
            return View(model);
        }

        // Zorluk GÖNDERİLMİYOR: servis onu işlemden kendisi alacak
        var sonuc = await _gorevService.OlusturAsync(model.Baslik, model.Aciklama, model.IslemId!.Value);

        if (!sonuc.Basarili)
        {
            ModelState.AddModelError(string.Empty, sonuc.Mesaj);
            await IslemleriDoldurAsync(model);
            return View(model);
        }

        TempData[sonuc.UyariVar ? Bildirim.Uyari : Bildirim.Basari] = sonuc.Mesaj;
        return RedirectToAction(nameof(Index));
    }

    private async Task IslemleriDoldurAsync(GorevFormViewModel model)
    {
        model.Islemler = await _islemService.TumunuGetirAsync();
    }
}
