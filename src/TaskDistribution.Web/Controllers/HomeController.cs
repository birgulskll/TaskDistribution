using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TaskDistribution.Business.Services;
using TaskDistribution.Web.Models;

namespace TaskDistribution.Web.Controllers;

public class HomeController : Controller
{
    private readonly IOzetService _ozetService;

    public HomeController(IOzetService ozetService)
    {
        _ozetService = ozetService;
    }

    // GET / → görev dağılımı özet tablosu
    public async Task<IActionResult> Index()
    {
        var ozetler = await _ozetService.PersonelOzetleriniGetirAsync();
        return View(ozetler);
    }

    // GET /Home/Durum/404 → Program.cs'teki UseStatusCodePagesWithReExecute buraya yönlendirir
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Durum(int id)
    {
        return View(id);   // Sayfa, durum koduna (404, 400...) göre mesaj seçer
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
