# Task Distribution

Kullanıcıların oluşturduğu görevleri, seçilen işlemin zorluk seviyesine (1–5) göre
personellere **otomatik ve adil** şekilde dağıtan bir ASP.NET Core MVC uygulaması.

> Proje geliştirme aşamasındadır. Ayrıntılı dokümantasyon ve atama algoritmasının
> akış şeması son aşamada eklenecektir.

## Teknolojiler

- C# / .NET 8
- ASP.NET Core MVC
- Entity Framework Core 8 + SQLite
- Bootstrap 5
- xUnit

## Proje Yapısı

| Proje | Görevi | Bağımlı olduğu proje |
|---|---|---|
| `TaskDistribution.Domain` | Entity sınıfları (Personel, Islem, TaskItem) | — |
| `TaskDistribution.Data` | EF Core, DbContext, repository'ler | Domain |
| `TaskDistribution.Business` | İş kuralları ve atama algoritması | Data |
| `TaskDistribution.Web` | MVC arayüzü (Controller ve View'lar) | Business |
| `TaskDistribution.Tests` | xUnit testleri | Domain, Data, Business |

Bağımlılıklar tek yönlüdür: `Web → Business → Data → Domain`

## Çalıştırma

```bash
dotnet build
dotnet test
dotnet run --project src/TaskDistribution.Web
```
