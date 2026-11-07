using Microsoft.AspNetCore.Mvc;
using TaskDistribution.Business.Services;

namespace TaskDistribution.Web.Controllers;

/// <summary>
/// İşlemler sadece seed data ile geldiği için bu sayfa salt okunurdur.
/// </summary>
public class IslemController : Controller
{
    private readonly IIslemService _islemService;

    public IslemController(IIslemService islemService)
    {
        _islemService = islemService;
    }

    // GET /Islem
    public async Task<IActionResult> Index()
    {
        var islemler = await _islemService.TumunuGetirAsync();
        return View(islemler);
    }
}
