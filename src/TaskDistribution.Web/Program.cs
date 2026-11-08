using TaskDistribution.Business;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "appsettings.json içinde 'ConnectionStrings:DefaultConnection' ayarı bulunamadı.");

builder.Services.AddBusinessLayer(connectionString);

var app = builder.Build();

// Veritabanı dosyası yoksa oluşturur, migration'ları ve seed data'yı uygular.
app.Services.VeritabaniniHazirla();

if (!app.Environment.IsDevelopment())
{
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
