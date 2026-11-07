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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
