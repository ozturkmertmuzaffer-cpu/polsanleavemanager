# Conventions

Buradaki her kural ya araçla zorlanır (tercih edilen) ya da review'da kontrol edilir.

## Language
- Chat language (interviews, sessions): Turkish
- Document language (docs/, specs, plans, ADRs, review and verify reports): Turkish
- Always English (protocol, not prose): ANEW core files, template headings and field labels,
  `Status:` values (Draft / Approved / In progress / Shipped), `Approved by / on:`, `Source:`.
- Code identifiers, branch names, commit messages: English. Kullanıcıya görünen metinler
  Türkçedir ve `.resx` kaynak dosyalarında tutulur — kod içine gömülmez.
- Agent messages (handoff summaries, refusals, `Next:` lines): chat language; command names, file
  paths and protocol fields verbatim.

## Language & framework versions
- .NET 10 (LTS), C# 14 — SDK sürümü `global.json` ile sabitlenir.
- ASP.NET Core 10 + Blazor (Interactive Server), EF Core 10 + Npgsql, PostgreSQL 16+.
- ASP.NET Core Identity (kimlik doğrulama).
- Test: xUnit, Shouldly, bUnit, NetArchTest, Playwright for .NET.
- Paket sürümleri `Directory.Packages.props` ile merkezi yönetilir (Central Package Management).

## Naming
- .NET standart adlandırma: PascalCase tipler/metotlar, `_camelCase` private alanlar, `I` önekli arayüzler.
- Projeler: `Polsan.Leave.<Modül>.<Katman>` (ör. `Polsan.Leave.Requests.Domain`); testler
  `Polsan.Leave.<Modül>.Tests`, `Polsan.Leave.Architecture.Tests`, `Polsan.Leave.E2E.Tests`.
- Test metotları: `<Metot>_<Durum>_<Beklenen>` (ör. `Calculate_SeniorityExactlyFiveYears_Returns14Days`).
- İş kuralı testleri kuralın kimliğini taşır: `[Trait("Rule", "BR-1")]`.
- Branch ve commit adlandırması: `docs/git.md`.

## Error handling
- İş kuralı ihlalleri (yetersiz bakiye, çakışan talep, yetkisiz işlem…) exception değil,
  `Result` / `Result<T>` olarak döner; hata kodu + kullanıcıya gösterilecek Türkçe mesaj anahtarı taşır.
- Exception yalnızca beklenmeyen durumlar içindir; global handler loglar, kullanıcı genel bir
  mesaj + hata kimliği (correlation id) görür.
- Kullanıcıya asla stack trace, SQL, iç tip adı veya kişisel veri gösterilmez.
- `catch` bloğu hatayı yutmaz: ya işler ya loglayıp yeniden fırlatır.

## Data rules
- İzin günü ve bakiye: `decimal` — `double` / `float` yasak (0,5 gün).
- İzin tarihleri: `DateOnly`. Zaman damgaları: `DateTimeOffset`, UTC saklanır; arayüzde
  `Europe/Istanbul`'a çevrilir.
- Şimdiki zaman yalnızca `TimeProvider` üzerinden alınır (`docs/architecture.md`, kural 4).
- Kimlikler: `Guid`.
- Bakiye bir defterdir (ledger): hareketler eklenir, güncellenmez/silinmez (`docs/domain.md`).
- Para veya oran alanı gerekirse: `decimal`.

## Enforced by tooling
- `Directory.Build.props`: `Nullable=enable`, `TreatWarningsAsErrors=true`, `AnalysisLevel=latest-recommended`.
- `.editorconfig` + `dotnet format --verify-no-changes` (`scripts/check`, format adımı).
- Mimari kurallar: NetArchTest testleri (`docs/architecture.md`, Forbidden dependencies).
- Güvenlik açığı olan paket: `dotnet list package --vulnerable --include-transitive` (`scripts/check`).
