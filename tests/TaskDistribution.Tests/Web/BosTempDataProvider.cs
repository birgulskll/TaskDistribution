using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace TaskDistribution.Tests.Web;

/// <summary>
/// Controller testlerinde TempData'nın çalışması için gereken, hiçbir şey saklamayan sahte sağlayıcı.
/// Gerçek uygulamada bu iş çerezlerle (cookie) yapılır; testte buna gerek yok.
/// </summary>
internal sealed class BosTempDataProvider : ITempDataProvider
{
    public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();

    public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
}
