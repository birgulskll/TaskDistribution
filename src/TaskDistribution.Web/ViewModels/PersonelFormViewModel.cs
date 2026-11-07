using System.ComponentModel.DataAnnotations;
using TaskDistribution.Domain.Constants;

namespace TaskDistribution.Web.ViewModels;

/// <summary>
/// Personel ekleme ve düzenleme formunun taşıdığı veri.
/// Entity (Personel) yerine bunu kullanıyoruz; böylece formdan sadece Ad ve Soyad
/// gelebilir. Kötü niyetli biri forma "AktifMi=false" ekleyip gönderse bile dikkate alınmaz.
/// </summary>
public class PersonelFormViewModel
{
    /// <summary>Ekleme formunda 0, düzenleme formunda personelin Id'si.</summary>
    public int Id { get; set; }

    [Display(Name = "Ad")]
    [Required(ErrorMessage = "Ad boş olamaz.")]
    [StringLength(AlanUzunluklari.PersonelAd, ErrorMessage = "Ad en fazla {1} karakter olabilir.")]
    public string? Ad { get; set; }

    [Display(Name = "Soyad")]
    [Required(ErrorMessage = "Soyad boş olamaz.")]
    [StringLength(AlanUzunluklari.PersonelSoyad, ErrorMessage = "Soyad en fazla {1} karakter olabilir.")]
    public string? Soyad { get; set; }
}
