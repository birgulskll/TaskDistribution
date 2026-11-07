namespace TaskDistribution.Web.Helpers;

/// <summary>
/// Sayfa yönlendirmelerinden sonra kullanıcıya gösterilecek mesajların TempData anahtarları.
/// Anahtarları tek yerde tutuyoruz ki controller ile view'da yazım hatası olmasın.
/// </summary>
public static class Bildirim
{
    public const string Basari = "Bildirim.Basari";
    public const string Uyari = "Bildirim.Uyari";
    public const string Hata = "Bildirim.Hata";
}
