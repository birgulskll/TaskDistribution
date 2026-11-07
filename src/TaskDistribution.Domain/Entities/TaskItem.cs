using TaskDistribution.Domain.Constants;

namespace TaskDistribution.Domain.Entities;

/// <summary>
/// Bir personele atanmış görev.
/// Adı "Task" değil "TaskItem", çünkü .NET'te zaten System.Threading.Tasks.Task sınıfı var.
/// </summary>
public class TaskItem
{
    private int _zorluk = ZorlukSeviyesi.EnDusuk;

    public int Id { get; set; }

    public string Baslik { get; set; } = string.Empty;

    /// <summary>
    /// İsteğe bağlıdır, bu yüzden boş (null) olabilir.
    /// </summary>
    public string? Aciklama { get; set; }

    // --- İşlem ilişkisi ---

    /// <summary>
    /// Görevin hangi işleme ait olduğunu tutan yabancı anahtar (foreign key).
    /// </summary>
    public int IslemId { get; set; }

    /// <summary>
    /// İlişkili işlem nesnesi (navigation property). Veritabanından EF Core tarafından doldurulur.
    /// </summary>
    public Islem Islem { get; set; } = null!;

    // --- Personel ilişkisi ---

    public int AtananPersonelId { get; set; }

    public Personel AtananPersonel { get; set; } = null!;

    // --- Diğer bilgiler ---

    /// <summary>
    /// Görev oluşturulduğu andaki işlem zorluğunun kopyası.
    /// İşlemin zorluğu ileride değişse bile eski görevlerin zorluğu bozulmaz.
    /// </summary>
    public int Zorluk
    {
        get => _zorluk;
        set => _zorluk = ZorlukSeviyesi.Dogrula(value);
    }

    /// <summary>
    /// Görevin oluşturulduğu (ve aynı anda atandığı) tarih.
    /// </summary>
    public DateTime OlusturmaTarihi { get; set; }
}
