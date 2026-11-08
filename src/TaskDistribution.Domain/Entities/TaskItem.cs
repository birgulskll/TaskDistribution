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
    /// İsteğe bağlıdır.
    /// </summary>
    public string? Aciklama { get; set; }

    /// <summary>
    /// Görevin ait olduğu işlem (foreign key).
    /// </summary>
    public int IslemId { get; set; }

    /// <summary>
    /// İlişkili işlem. EF Core tarafından doldurulur.
    /// </summary>
    public Islem Islem { get; set; } = null!;

    public int AtananPersonelId { get; set; }

    public Personel AtananPersonel { get; set; } = null!;

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
