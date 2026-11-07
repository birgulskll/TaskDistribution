namespace TaskDistribution.Web.Helpers;

/// <summary>
/// Zorluk seviyesinin ekranda nasıl görüneceği (yazı ve Bootstrap rengi).
/// Sadece görünümle ilgili olduğu için Business'ta değil Web katmanında duruyor.
/// </summary>
public static class ZorlukGorunumu
{
    public static string Etiket(int zorluk) => zorluk switch
    {
        1 => "Çok kolay",
        2 => "Kolay",
        3 => "Orta",
        4 => "Zor",
        5 => "Çok zor",
        _ => "Bilinmiyor"
    };

    // Not: Şablondaki Bootstrap 5.1 sürümünde "text-bg-*" sınıfları yok (5.2'de geldi).
    // Bu yüzden "bg-*" kullanıyoruz; açık zeminlerde yazı okunsun diye "text-dark" ekliyoruz.
    public static string CssSinifi(int zorluk) => zorluk switch
    {
        1 => "bg-success",
        2 => "bg-info text-dark",
        3 => "bg-warning text-dark",
        4 => "bg-danger",
        5 => "bg-dark",
        _ => "bg-secondary"
    };
}
