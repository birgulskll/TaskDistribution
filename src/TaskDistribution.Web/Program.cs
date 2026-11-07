using TaskDistribution.Business;

var builder = WebApplication.CreateBuilder(args);

// ---------- 1. Servisleri kaydet ----------

// MVC: Controller'lar ve View'lar
builder.Services.AddControllersWithViews();

// Veritabanı bağlantı bilgisini appsettings.json dosyasından oku
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "appsettings.json içinde 'ConnectionStrings:DefaultConnection' ayarı bulunamadı.");

// Business ve Data katmanlarının tüm servisleri tek satırla (Aşama 6'da yazdığımız extension method)
builder.Services.AddBusinessLayer(connectionString);

var app = builder.Build();

// ---------- 2. Veritabanını hazırla ----------

// Veritabanı dosyası yoksa oluşturur, migration'ları ve seed data'yı uygular.
app.Services.VeritabaniniHazirla();

// ---------- 3. İstek hattını (middleware) ayarla ----------

if (!app.Environment.IsDevelopment())
{
    // Beklenmeyen bir hata olursa kullanıcıya teknik detay değil, sade bir hata sayfası göster
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// 404 (sayfa bulunamadı) gibi durumlarda boş sayfa yerine /Home/Durum/404 sayfasını göster
app.UseStatusCodePagesWithReExecute("/Home/Durum/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
