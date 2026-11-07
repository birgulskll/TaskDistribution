using TaskDistribution.Business.Services;
using TaskDistribution.Domain.Entities;
using static TaskDistribution.Tests.Business.AtamaTestVerisi;

namespace TaskDistribution.Tests.Business;

public class AtamaServiceTests
{
    private readonly AtamaService _servis = new();

    // ---------- Aktif personel kuralı ----------

    [Fact]
    public void HicPersonelYoksa_BasarisizSonucVeAnlamliMesajDoner()
    {
        var sonuc = _servis.PersonelSec(3, new List<Personel>());

        Assert.False(sonuc.Basarili);
        Assert.Null(sonuc.PersonelId);
        Assert.Contains("aktif personel bulunmuyor", sonuc.Mesaj);
    }

    [Fact]
    public void SadecePasifPersonelVarsa_BasarisizSonucDoner()
    {
        var personeller = new List<Personel> { Personel(1, aktifMi: false), Personel(2, aktifMi: false) };

        var sonuc = _servis.PersonelSec(3, personeller);

        Assert.False(sonuc.Basarili);
    }

    [Fact]
    public void PasifPersonel_HicGoreviOlmasaBileSecilmez()
    {
        // Arrange: pasif olan hiç görev almamış, aktif olanın ise iki görevi var
        var pasif = Personel(1, aktifMi: false);
        var aktif = Personel(2);
        GorevEkle(aktif, zorluk: 1, gun: 1);
        GorevEkle(aktif, zorluk: 2, gun: 2);

        // Act
        var sonuc = _servis.PersonelSec(4, new List<Personel> { pasif, aktif });

        // Assert
        Assert.Equal(2, sonuc.PersonelId);
    }

    // ---------- Ardışık zorluk kuralı ----------

    [Fact]
    public void SonGoreviAyniZorluktaOlan_Elenir()
    {
        // Arrange
        // Personel 1: 3. zorlukta sadece 1 görevi var (normalde avantajlı) ama SON görevi 3.
        var p1 = Personel(1);
        GorevEkle(p1, zorluk: 3, gun: 1);

        // Personel 2: 3. zorlukta 2 görevi var ama son görevi 1.
        var p2 = Personel(2);
        GorevEkle(p2, zorluk: 3, gun: 1);
        GorevEkle(p2, zorluk: 3, gun: 2);
        GorevEkle(p2, zorluk: 1, gun: 3);

        // Act
        var sonuc = _servis.PersonelSec(3, new List<Personel> { p1, p2 });

        // Assert: Personel 1 elendiği için Personel 2 seçilmeli
        Assert.Equal(2, sonuc.PersonelId);
        Assert.False(sonuc.KuralEsnetildi);
    }

    [Fact]
    public void SonGorev_ListeSirasinaDegilTariheGoreBelirlenir()
    {
        // Personel 1'in görevleri listeye "karışık" sırada eklendi:
        // listedeki son kayıt 2. zorlukta, ama TARİHÇE en son görevi (5 Kasım) 3. zorlukta.
        var p1 = Personel(1);
        GorevEkle(p1, zorluk: 3, gun: 5);
        GorevEkle(p1, zorluk: 2, gun: 1);

        var p2 = Personel(2);
        GorevEkle(p2, zorluk: 3, gun: 2);
        GorevEkle(p2, zorluk: 3, gun: 3);
        GorevEkle(p2, zorluk: 1, gun: 4);

        var sonuc = _servis.PersonelSec(3, new List<Personel> { p1, p2 });

        // Personel 1'in son görevi (tarihe göre) 3 olduğu için elenmeli
        Assert.Equal(2, sonuc.PersonelId);
    }

    // ---------- Sıralama kuralları ----------

    [Fact]
    public void IlgiliZorluktaEnAzGorevAlan_ToplamGorevuFazlaOlsaBileSecilir()
    {
        // Personel 1: toplam 2 görev, ama 2. zorlukta 1 görevi var
        var p1 = Personel(1);
        GorevEkle(p1, zorluk: 2, gun: 1);
        GorevEkle(p1, zorluk: 1, gun: 2);

        // Personel 2: toplam 3 görev, ama 2. zorlukta hiç görevi yok
        var p2 = Personel(2);
        GorevEkle(p2, zorluk: 4, gun: 1);
        GorevEkle(p2, zorluk: 4, gun: 2);
        GorevEkle(p2, zorluk: 5, gun: 3);

        var sonuc = _servis.PersonelSec(2, new List<Personel> { p1, p2 });

        Assert.True(sonuc.Basarili);
        Assert.Equal(2, sonuc.PersonelId);
        Assert.False(sonuc.KuralEsnetildi);
    }

    [Fact]
    public void Esitlikte_ToplamGorevSayisiAzOlanSecilir()
    {
        // İkisinin de 5. zorlukta görevi yok (eşit). Toplam: P1=2, P2=1
        var p1 = Personel(1);
        GorevEkle(p1, zorluk: 1, gun: 1);
        GorevEkle(p1, zorluk: 2, gun: 2);

        var p2 = Personel(2);
        GorevEkle(p2, zorluk: 1, gun: 3);

        var sonuc = _servis.PersonelSec(5, new List<Personel> { p1, p2 });

        Assert.Equal(2, sonuc.PersonelId);
    }

    [Fact]
    public void Esitlikte_SonAtanmaTarihiEnEskiOlanSecilir()
    {
        // İkisinin de 4. zorlukta görevi yok ve toplam 1'er görevi var (eşit).
        // P1'in son görevi 5 Kasım, P2'nin son görevi 3 Kasım → P2 daha uzun süredir boşta.
        var p1 = Personel(1);
        GorevEkle(p1, zorluk: 1, gun: 5);

        var p2 = Personel(2);
        GorevEkle(p2, zorluk: 2, gun: 3);

        var sonuc = _servis.PersonelSec(4, new List<Personel> { p1, p2 });

        Assert.Equal(2, sonuc.PersonelId);
    }

    [Fact]
    public void TumKriterlerEsitse_IdsiEnKucukOlanSecilir()
    {
        // Üçünün de hiç görevi yok; liste bilerek karışık sırada verildi.
        var personeller = new List<Personel> { Personel(3), Personel(1), Personel(2) };

        var sonuc = _servis.PersonelSec(1, personeller);

        Assert.Equal(1, sonuc.PersonelId);
    }

    // ---------- Kuralın esnetilmesi ----------

    [Fact]
    public void TekAktifPersonelinSonGoreviAyniZorluktaysa_KuralEsnetilirVeUyariVerilir()
    {
        var p1 = Personel(1);
        GorevEkle(p1, zorluk: 3, gun: 1);

        var sonuc = _servis.PersonelSec(3, new List<Personel> { p1 });

        Assert.True(sonuc.Basarili);
        Assert.True(sonuc.KuralEsnetildi);
        Assert.Equal(1, sonuc.PersonelId);
        Assert.Contains("esnetilerek", sonuc.Mesaj);
    }

    [Fact]
    public void KuralEsnetilince_ToplamGorevSayisiEnAzOlanSecilir()
    {
        // İkisinin de son görevi 3. zorlukta → ikisi de elenir.
        // P1: 3. zorlukta 1 görev ama toplam 3 görev
        var p1 = Personel(1);
        GorevEkle(p1, zorluk: 1, gun: 1);
        GorevEkle(p1, zorluk: 1, gun: 2);
        GorevEkle(p1, zorluk: 3, gun: 3);

        // P2: 3. zorlukta 2 görev ama toplam 2 görev → esnetmede toplam sayıya bakılır
        var p2 = Personel(2);
        GorevEkle(p2, zorluk: 3, gun: 1);
        GorevEkle(p2, zorluk: 3, gun: 2);

        var sonuc = _servis.PersonelSec(3, new List<Personel> { p1, p2 });

        Assert.True(sonuc.KuralEsnetildi);
        Assert.Equal(2, sonuc.PersonelId);
    }

    // ---------- Geçersiz girdi ----------

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void GecersizZorluk_HataFirlatir(int zorluk)
    {
        var personeller = new List<Personel> { Personel(1) };

        Assert.Throws<ArgumentOutOfRangeException>(() => _servis.PersonelSec(zorluk, personeller));
    }
}
