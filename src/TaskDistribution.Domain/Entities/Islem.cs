using TaskDistribution.Domain.Constants;

namespace TaskDistribution.Domain.Entities;

/// <summary>
/// Görev oluştururken seçilen iş türü. Görevin zorluğu buradan alınır.
/// </summary>
public class Islem
{
    // Zorluk değerini saklayan gizli alan (backing field).
    private int _zorluk = ZorlukSeviyesi.EnDusuk;

    public int Id { get; set; }

    public string Ad { get; set; } = string.Empty;

    /// <summary>
    /// 1 ile 5 arasında olmak zorundadır; dışında bir değer verilirse hata fırlatılır.
    /// </summary>
    public int Zorluk
    {
        get => _zorluk;
        set => _zorluk = ZorlukSeviyesi.Dogrula(value);
    }

    /// <summary>
    /// Bu işlem seçilerek oluşturulmuş görevler.
    /// </summary>
    public ICollection<TaskItem> Gorevler { get; set; } = new List<TaskItem>();
}
