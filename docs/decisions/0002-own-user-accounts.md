# ADR 0002 — Kimlik doğrulama: uygulamanın kendi kullanıcı hesapları (ASP.NET Core Identity)

- Status: Accepted
- Date: 2026-10-21

## Context
Çalışanların sisteme giriş yöntemi ve personel verisinin kaynağı seçilmeliydi. Seçenekler:
Microsoft Entra ID / AD SSO, uygulamanın kendi hesapları veya mevcut bir İK/bordro sistemi.

## Decision
Uygulama **kendi kullanıcı hesaplarını** ASP.NET Core Identity ile yönetir. Hesapları İK açar
(self-servis kayıt yok); personel kaydı ve yönetici ilişkisi uygulamada İK tarafından tutulur.

## Consequences
- Kazanç: dış sisteme bağımlılık yok; kurulum ve test basit.
- Maliyet: parola politikası, kilitleme, parola sıfırlama ve işten çıkışta hesabın kapatılması bizim
  sorumluluğumuz (`docs/security.md`). Çalışanlar ayrı bir parola hatırlamak zorunda.
- Maliyet: personel verisi başka sistemlerle elle senkron tutulur.

## Alternatives considered
- **Entra ID / AD SSO:** ayrı parola yok, işten çıkışta erişim otomatik kesilir — fakat bootstrap'ta
  tercih edilmedi; dış kimlik sağlayıcı kurulumu ve bağımlılığı getirir.
- **Mevcut İK/bordro sistemi:** kullanılabilir bir kaynak belirtilmedi.

## Revisit triggers
- Şirket Microsoft 365 / Entra ID kullanıyorsa ve parola yönetimi yük olursa.
- Bordro veya İK sistemiyle entegrasyon kapsama girerse.
