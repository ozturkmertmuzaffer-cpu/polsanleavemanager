# Architecture

## System overview
Polsan Leave Manager, Polsan Teknoloji çalışanlarının (50–100 kişi, tek şirket / single-tenant)
izin taleplerini, yönetici onaylarını ve izin bakiyelerini yöneten, şirket içi ağda çalışan bir
web uygulamasıdır. Stil: **modüler monolit** — tek deploy edilebilir ASP.NET Core (.NET 10)
uygulaması, Blazor arayüz, tek SQLite veritabanı (gömülü, kurulum gerektirmez). Modüller derleme düzeyinde ayrı projelerdir
ve birbirine yalnızca `Contracts` projeleri üzerinden erişir. Gerekçe: bu ölçekte dağıtık
mimarinin maliyeti faydasını aşar; sınırlar ise mimari testlerle korunur
(ADR `0001-modular-monolith-dotnet-blazor-postgres.md`; veritabanı ADR `0003-sqlite-instead-of-postgresql.md`). Kimlik doğrulama uygulamanın kendi
hesaplarıyla yapılır (ADR `0002-own-user-accounts.md`).

Proje adlandırması: `Polsan.Leave.<Modül>.<Katman>` — katmanlar `Domain` (saf C#),
`Application` (use-case servisleri), `Infrastructure` (EF Core), `Contracts` (public arayüz).
Host: `Polsan.Leave.Web` (Blazor + composition root).

## Modules / components and ownership

| Module | Single responsibility | Owns |
|---|---|---|
| Personel (`Staff`) | Çalışan kaydı, yönetici ilişkisi, roller, kimlik doğrulama | Çalışan, kullanıcı hesabı, rol, işe giriş / çıkış ve doğum tarihi |
| İzin Politikası (`Policy`) | İzin türleri, resmî tatil takvimi, 4857 md.53 hak ediş hesabı, iş günü hesabı | İzin türü tanımları, tatil takvimi, ek gün tanımları |
| İzin Talepleri (`Requests`) | Talep oluşturma, onay, red, iptal akışı | İzin talebi ve durum geçmişi |
| Bakiye (`Balance`) | Hak, kullanım, iptal ve düzeltme hareketlerinin defteri (ledger), kalan bakiye | Bakiye hareketleri |
| Denetim (`Audit`) | Kim, ne zaman, ne yaptı kaydı (onay, red, iptal, bakiye düzeltme) | Denetim kayıtları |
| Web (`Web`) | Blazor arayüz, kimlik doğrulama uçları, composition root | Hiçbir iş verisi — yalnızca sunum |

## Communication rules
- Modüller arası çağrı **senkron**dur ve yalnızca hedef modülün `Contracts` arayüzü üzerinden
  yapılır (ör. `Requests` → `IBalanceService.Reserve/Consume/Release`).
- Birden fazla modülü değiştiren bir işlem (ör. onay + bakiye düşümü + denetim kaydı) **tek
  veritabanı transaction'ı** içinde yürür.
- MVP'de mesaj kuyruğu, event bus veya arka plan iş kuyruğu yoktur.
- UI yalnızca `Application` servislerini çağırır.

## Forbidden dependencies (make them testable)
Mimari testler (ArchUnitNET, `tests/Polsan.Leave.Architecture.Tests`) ve derleme analyzer'ı
`scripts/check` içinde her kuralı doğrular; her kuralın ihlali yakaladığı öz-testlerle kanıtlanır:
1. Bir modül başka bir modülün `Domain`, `Application` veya `Infrastructure` projesine referans
   vermez; yalnızca `Contracts` projesine. Bir modül başka bir modülün tablolarına erişmez.
2. `*.Domain` projeleri EF Core, ASP.NET Core veya Blazor'a referans vermez.
3. `Polsan.Leave.Web` içindeki Blazor bileşenleri `DbContext` (EF Core), `Microsoft.Data.Sqlite` veya
   `*.Infrastructure` tiplerini kullanmaz.
4. `DateTime.Now`, `DateTime.UtcNow`, `DateTime.Today`, `DateTimeOffset.Now`, `DateTimeOffset.UtcNow`
   kullanımı yasaktır — zaman `TimeProvider` ile enjekte edilir (BannedApiAnalyzers, `BannedSymbols.txt`;
   ihlal derlemeyi `RS0030` ile kırar).
5. Çalışma zamanında dış ağa (internete) çağrı yapılmaz (MVP). Kanıt kısmidir: iş modüllerinin
   `System.Net.Http` bağımlılığı engellenir.

## Deliberately out of scope
- Raporlama ve Excel dışa aktarım, ekip izin takvimi.
- Bordro / İK sistemi entegrasyonu, Active Directory / Entra ID SSO.
- İki adımlı (yönetici + İK) onay, vekil onaylayıcı.
- Saatlik izin, avans (eksi) izin.
- Native mobil uygulama, e-posta / bildirim gönderimi.
- Sağlık raporu dosyası saklama.
- Çok şirketli (multi-tenant) yapı, bulut barındırma.
