# ADR 0001 — Modüler monolit: .NET 10 + Blazor + PostgreSQL, şirket içi barındırma

- Status: Superseded by ADR-0003
- Date: 2026-10-21
- Revised: 2026-10-21 — mimari test aracının adı NetArchTest → ArchUnitNET (plan 0001, R-3); karar değişmedi.

## Context
Polsan'ın 50–100 çalışanı için izin yönetim sistemi kurulacak. Geliştirme ekibi 5 kişi. Kurallar
(4857 md.53 hak ediş, iş günü hesabı, 0,5 gün) tarih ve ondalık hassasiyeti gerektiriyor. Veriler
KVKK kapsamında kişisel veri; bir kısmı sağlık verisi. Geliştirme makinesinde .NET 10 SDK kurulu,
Docker kurulu değil.

## Decision
Tek deploy edilebilir **modüler monolit** kuruyoruz: ASP.NET Core (.NET 10 LTS) + Blazor arayüz,
EF Core + **PostgreSQL**, modüller (`Staff`, `Policy`, `Requests`, `Balance`, `Audit`) ayrı projeler
ve yalnızca `Contracts` üzerinden konuşuyor; sınırlar ArchUnitNET ile korunuyor. Uygulama
**şirket içi sunucuda**, yalnızca iç ağdan erişilebilir şekilde çalışıyor.

## Consequences
- Kazanç: tek dil ve tek araç zinciri (C#), tek deploy, tek transaction ile tutarlılık; domain
  kuralları saf C# ile hızlı test edilir; kişisel veri şirket dışına çıkmaz.
- Kazanç: modül sınırları baştan çizildiği için ileride bir modülü ayırmak mümkün kalır.
- Maliyet: Blazor'da React ekosistemi kadar hazır bileşen yok; Interactive Server modu sunucuyla
  sürekli bağlantı (SignalR) ister.
- Maliyet: entegrasyon testleri için yerelde PostgreSQL kurulumu gerekir (Docker yok).
- Maliyet: iç ağ dışından (evden) erişim için VPN gerekir.

## Alternatives considered
- **Klasik katmanlı tek proje:** başlangıçta daha hızlı, fakat modül sınırları zamanla bulanıklaşır
  ve mimari test ile korunamaz.
- **.NET Web API + React SPA:** daha esnek UI, fakat 5 kişilik ekip için iki araç zinciri, iki test
  dünyası ve ayrı build.
- **Node/TypeScript full-stack:** tek dil, fakat decimal ve tarih kuralları için daha zayıf tip desteği.
- **SQL Server:** şirkette lisans/DBA avantajı yoksa maliyet; **SQLite:** eşzamanlı yazma ve
  yedekleme zayıf.
- **Bulut barındırma:** uzaktan erişim kolay, fakat KVKK yurt dışı aktarım değerlendirmesi gerekir.

## Revisit triggers
- Kullanıcı sayısı 1000'i aşarsa veya birden fazla şirket/tenant gerekirse.
- Bordro / İK sistemi entegrasyonu veya mobil uygulama ihtiyacı doğarsa.
- Uzaktan erişim VPN olmadan zorunlu hale gelirse (bulut değerlendirmesi).
