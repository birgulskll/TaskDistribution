using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TaskDistribution.Domain.Constants;
using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Web.ViewModels;

/// <summary>
/// Görev oluşturma formu.
/// DİKKAT: Burada "Zorluk" alanı BİLEREK yok. Zorluk formdan gelmez,
/// sunucuda seçilen işlemden alınır. Böylece kullanıcı zorluğu değiştiremez.
/// </summary>
public class GorevFormViewModel
{
    [Display(Name = "Başlık")]
    [Required(ErrorMessage = "Görev başlığı boş olamaz.")]
    [StringLength(AlanUzunluklari.GorevBaslik, ErrorMessage = "Başlık en fazla {1} karakter olabilir.")]
    public string? Baslik { get; set; }

    [Display(Name = "Açıklama")]
    [StringLength(AlanUzunluklari.GorevAciklama, ErrorMessage = "Açıklama en fazla {1} karakter olabilir.")]
    public string? Aciklama { get; set; }

    [Display(Name = "İşlem")]
    [Required(ErrorMessage = "Lütfen bir işlem seçin.")]
    public int? IslemId { get; set; }

    /// <summary>
    /// Açılır listede gösterilecek işlemler. Sadece sayfayı çizmek için kullanılır;
    /// [BindNever]: formdan gelen veriyle doldurulmaz. [ValidateNever]: doğrulamaya katılmaz.
    /// </summary>
    [BindNever]
    [ValidateNever]
    public List<Islem> Islemler { get; set; } = new();
}
