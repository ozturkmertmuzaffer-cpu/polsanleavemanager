# Security

İzin kayıtları KVKK kapsamında kişisel veridir; hastalık / rapor izinleri özel nitelikli (sağlık)
veri sayılır. Uygulama yalnızca şirket içi ağda, şirket içi sunucuda çalışır.

## Secrets
- Secrets never enter the repo, specs, prompts, or chat. `.env` is gitignored; provide `.env.example`.
- Agents never print secret values, even when debugging.
- Geliştirmede `dotnet user-secrets`, sunucuda ortam değişkenleri.

## Input & output
- Tüm girdiler sınırda (form modeli / servis girişi) doğrulanır; sunucu tarafı doğrulama esastır,
  istemci doğrulaması yalnızca kullanıcı deneyimi içindir.
- Veritabanı erişimi EF Core ile; ham SQL gerekiyorsa yalnızca parametreli.
- Blazor'un otomatik HTML encoding'i korunur; `MarkupString` kullanımı yasaktır.
- Hata mesajlarında ve loglarda kişisel veri (ad, tarih aralığı, izin türü gerekçesi), SQL veya
  stack trace yer almaz; çalışan loglarda yalnızca ID ile anılır.
- Hastalık / rapor izninde yalnızca tarih ve tür tutulur; tanı veya rapor dosyası saklanmaz (MVP).

## AuthN / AuthZ
- ASP.NET Core Identity; kendi parola / hash kodumuz yazılmaz. Parola en az 12 karakter.
- 5 başarısız denemede hesap 15 dakika kilitlenir.
- Self-servis kayıt yoktur: hesabı İK açar, çalışan ilk girişte parolasını belirler.
- İşten çıkış tarihi geçen çalışanın hesabı pasiftir, oturum açamaz.
- **Default-deny:** global `RequireAuthenticatedUser` politikası; anonim erişim yalnızca giriş
  ve parola belirleme sayfalarında, açıkça işaretlenerek.
- Roller: `Employee`, `Manager`, `HR`.
- **Kayıt düzeyinde yetki servis katmanında** uygulanır (UI'da değil): çalışan yalnızca kendi
  taleplerini, yönetici yalnızca doğrudan bağlı çalışanlarının taleplerini, İK tümünü görür.
  Kimse kendi talebini onaylayamaz (BR-6). Her yetki kuralı testle korunur.
- HTTPS zorunlu + HSTS (iç ağda da); anti-forgery token açık.
- Onay, red, iptal ve bakiye düzeltmeleri denetim kaydına yazılır (kim, ne zaman, ne).
- SQLite veritabanı dosyası (ADR 0003) yalnızca uygulama hesabının erişebildiği yerel bir dizinde
  durur; ağ paylaşımına konmaz, web köküne konmaz ve düzenli yedeklenir (`VACUUM INTO`).

## Dependencies
- Yeni bir NuGet paketi eklemek insan onayı ister (plan içinde gerekçesiyle belirtilir).
- Lisans: MIT, Apache-2.0 veya BSD; aktif bakımı olan paketler.
- `dotnet list package --vulnerable --include-transitive` `scripts/check` içinde çalışır;
  bilinen açık bulunursa check kırmızı olur.

## Review lens
Security is a mandatory dimension of every independent review (see `prompts/review.md`), not a
separate afterthought phase.
