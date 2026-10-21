# ADR 0003 — Veritabanı: PostgreSQL yerine SQLite (kurulum gerektirmeyen, gömülü)

- Status: Accepted
- Date: 2026-10-21

ADR 0001'in yerini alır. ADR 0001'in veritabanı dışındaki tüm kararları (modüler monolit, .NET 10 +
Blazor, modüller ve Contracts sınırları, şirket içi barındırma) **aynen geçerlidir** ve burada
yeniden kayıt altına alınır; değişen yalnızca veritabanıdır.

## Context
Spec 0001'in yapımı sırasında full check, makinede PostgreSQL kurulu olmadığı için kırmızı kaldı.
İstek sahibi, geliştirici makinelerine, CI'a ve sunucuya **kurulum gerektirmeyen** bir veritabanı
istedi. SQL Server LocalDB bu makinede kurulu, fakat ekipteki diğer makinelerde olacağı garanti
değildir. Ölçek: 50–100 kullanıcı, günde düzinelerle ifade edilen yazma; tek uygulama sunucusu.

## Decision
Uygulama **SQLite** kullanır (EF Core SQLite sağlayıcısı, `Microsoft.Data.Sqlite`). Veritabanı tek
bir dosyadır; motor NuGet paketiyle gelir. WAL modu açılır. İzin günleri veritabanında **yarım gün
birimi cinsinden tamsayı** olarak saklanır (1,5 gün = 3); kodda tip `decimal` kalır, dönüşümü EF
value converter yapar. Mimari, dil, arayüz ve barındırma kararları ADR 0001'deki gibidir.

## Consequences
- Kazanç: hiçbir ortamda kurulum yok (geliştirici, CI, sunucu); CI'da servis container'ı gerekmez;
  entegrasyon testleri her test için geçici bir dosya veritabanı kullanır ve iz bırakmaz.
- Kazanç: tamsayı yarım gün saklama, SQL tarafında toplama/sıralamayı tam ve yuvarlamasız yapar.
- Kazanç: yazmalar sıraya girdiği için eşzamanlı onayda bakiyenin eksiye düşmesi daha kolay önlenir.
- Maliyet: aynı anda tek yazıcı — bu ölçekte sorun değil, büyümede sınırdır.
- Maliyet: tek uygulama sunucusuna bağlıyız; veritabanı dosyası ağ paylaşımına konmaz.
- Maliyet: yedekleme bizim sorumluluğumuzda (`VACUUM INTO` ile düzenli kopya; canlıya geçiş runbook'u).
- Maliyet: EF Core'un SQLite sağlayıcısında bazı migration işlemleri (ör. sütun değiştirme) tablo
  yeniden oluşturmayla yapılır.

## Alternatives considered
- **PostgreSQL:** güçlü, fakat her ortamda kurulum ve servis gerektirir — isteğin tam tersi.
- **SQL Server (LocalDB / Express):** EF desteği olgun; fakat LocalDB yalnızca Windows'ta ve ayrı
  kurulumla gelir, üretimde Express kurulumu gerekir.
- **EF Core InMemory:** gerçek bir veritabanı değil; transaction ve kısıt davranışı yok — test kanıtı zayıf.

## Revisit triggers
- Kullanıcı sayısı birkaç yüzü aşarsa veya yazma kilidi beklemeleri ölçülebilir hale gelirse.
- Birden fazla uygulama sunucusu (yük dengeleme) veya yüksek erişilebilirlik gerekirse.
- Raporlama ihtiyaçları SQLite'ın SQL yeteneklerini aşarsa.
