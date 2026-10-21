# Spec 0001 — Uygulama iskeleti

- Status: In progress
- Mode: lite
- Plan: `specs/plans/0001-plan.md`

## Intent
Polsan Leave Manager'ı geliştirecek 5 kişilik ekip, iş kodu yazmaya başlamadan önce çalışan boş
bir uygulamaya ve güvenilir bir doğrulama zincirine ihtiyaç duyuyor. Uygulama başlar, Türkçe bir
açılış sayfası gösterir ve sağlık durumunu (veritabanı bağlantısı dahil) bildirir; modül sınırları
ve yasak bağımlılıklar ilk günden otomatik testle korunur; `./scripts/check` yerelde ve CI'da
gerçekten derler, test eder ve geride süreç bırakmaz. Başarı: sonraki özellik (F1) yalnızca iş
kodu yazarak başlayabilir. Bilinçli olarak yapılmayanlar: iş kuralı ve iş verisi, giriş /
kimlik doğrulama (F2), sunucuya kurulum.

## Requirements
- R-1: Uygulama yerelde tek komutla başlatılabilir ve tarayıcıda "Polsan İzin Yönetimi"
  başlıklı Türkçe bir açılış sayfası gösterir.
- R-2: Arayüz mobil uyumlu bir ana yerleşime (başlık + boş menü alanı + içerik) sahiptir; tüm
  sayfalar bu yerleşimi kullanır.
- R-3: Var olmayan bir adres istendiğinde Türkçe "sayfa bulunamadı" sayfası gösterilir.
- R-4: Beklenmeyen bir hata oluştuğunda kullanıcı Türkçe genel bir hata mesajı ve bir hata
  kimliği görür; teknik ayrıntı (stack trace, SQL, iç tip adı) görmez. Hata, aynı hata kimliğiyle
  loglanır.
- R-5: Girişsiz erişilebilen bir sağlık kontrolü vardır; uygulama ve veritabanı erişilebilir ise
  `Healthy`, veritabanına ulaşılamıyorsa `Unhealthy` döner (izleme araçları için standart metin). Yanıt yalnızca bu durumu içerir;
  veritabanı adı, sürüm veya hata ayrıntısı içermez.
- R-6: Beş iş modülü (Personel, İzin Politikası, İzin Talepleri, Bakiye, Denetim) ve Web
  `docs/architecture.md`'deki katman yapısıyla boş olarak mevcuttur.
- R-7: `docs/architecture.md`'deki beş yasak bağımlılık kuralının her biri otomatik bir kontrolle
  (mimari test veya derleme analyzer'ı) doğrulanır ve ihlal edildiğinde `./scripts/check` kırmızı
  olur. Kural 5 (dış ağ) için kanıt kısmidir: iş modüllerinin HTTP istemci tiplerine bağımlılığı
  engellenir; her türlü ağ erişimi yakalanmaz.
- R-8: `./scripts/check` (full) derleme, biçim kontrolü, unit / mimari / bileşen testleri,
  entegrasyon + E2E testleri ve güvenlik açığı olan paket kontrolünü çalıştırır;
  `./scripts/check --fast` yalnızca hızlı adımları çalıştırır. Hızlı ve full-only test kümeleri
  bir selector ve onun tersiyle ayrılır.
- R-9: Gerçek PostgreSQL'e bağlanan en az bir entegrasyon testi vardır; test veritabanı
  bağlantısı tanımlı değilse test başarısız olur, atlanmaz.
- R-10: Açılış sayfasının gerçek tarayıcıda açıldığını doğrulayan bir E2E duman testi vardır;
  uygulamayı ve tarayıcıyı test kendisi başlatır ve (hata / timeout dahil) kapatır.
- R-11: CI her push ve PR'da full check'i .NET 10, PostgreSQL ve Playwright tarayıcısıyla çalıştırır.
- R-12: Kod kuralları (`docs/conventions.md`, Enforced by tooling) derleme düzeyinde zorlanır:
  nullable açık, uyarılar hata, merkezi paket sürüm yönetimi, SDK sürümü sabit.

## User experience
- Flow: geliştirici uygulamayı başlatır → tarayıcıda kök adresi açar → açılış sayfasını görür.
  Var olmayan bir adrese gider → "sayfa bulunamadı" sayfasını görür.
- States:
  - Açılış sayfası — başarılı: "Polsan İzin Yönetimi" başlığı ve kısa bir hoş geldiniz metni.
  - Açılış sayfası — boş: menüde öğe yoktur (bu özellik için beklenen durum).
  - Yükleniyor: Blazor bağlantısı kurulurken sayfa içeriği sunucudan hazır gelir; boş beyaz
    ekran görünmez.
  - Hata: genel Türkçe hata mesajı + hata kimliği.
  - Bulunamadı: Türkçe "sayfa bulunamadı" mesajı + açılış sayfasına dönüş bağlantısı.
  - Yetkisiz: uygulanamaz — bu özellikte giriş yoktur (F2).
- Design reference: none — the flow above is the design.
- Accessibility: sayfa dili `tr` olarak işaretlidir; her sayfanın anlamlı bir başlığı (title)
  vardır; dönüş bağlantısı klavyeyle (Tab + Enter) kullanılabilir; metin kontrastı WCAG AA.
  Yerleşim 360 px genişlikte yatay kaydırma olmadan görüntülenir.

## Constraints & out of scope
- **F2 (kimlik doğrulama) tamamlanmadan uygulama hiçbir sunucuya kurulmaz.** Bu özellikte giriş
  yoktur; varsayılan olarak her şeyi reddetme (default-deny) kuralı F2'de gelir.
- Çalışma zamanında dış ağa çağrı yapılmaz.
- Check geride süreç bırakmaz (`./scripts/residue` ve `--fast` temiz).
- Yeni NuGet paketleri `docs/security.md` lisans politikasına uyar ve planda listelenir.
- Ön koşullar: yerelde PostgreSQL 16+ kurulu ve `LEAVE_TEST_DB` tanımlı; Playwright tarayıcısı
  bir kez kurulmuş. Bunlar yokken full check kırmızıdır — bu beklenen davranıştır.
- Kapsam dışı: iş varlıkları ve tabloları, veritabanı migration'ları ile iş şeması, giriş /
  roller, menü öğeleri, sunucu kurulumu ve runbook, e-posta.

## Acceptance criteria
- [ ] AC-1 — Kök adres istendiğinde yanıt başarılıdır ve sayfa başlığı ile ana başlık
  "Polsan İzin Yönetimi" metnini içerir; sayfanın dili `tr`'dir.
- [ ] AC-2 — Açılış sayfası ana yerleşimi (başlık + menü alanı + içerik) kullanır ve menü alanı
  boştur.
- [ ] AC-3 — Var olmayan bir adres istendiğinde 404 durum kodu ve Türkçe "sayfa bulunamadı"
  mesajı ile açılış sayfasına dönüş bağlantısı döner.
- [ ] AC-4 — Bir istek sırasında beklenmeyen hata oluştuğunda kullanıcı 500 durum kodu, Türkçe
  genel hata mesajı ve bir hata kimliği görür; yanıt gövdesi stack trace, exception tipi veya
  mesajı içermez; aynı hata kimliği log kaydında yer alır. Hata, yalnızca test ortamında var olan
  bir tetikleyiciyle oluşturulur; üretim ortamında bu tetikleyici adresi 404 döner.
- [ ] AC-5 — Veritabanı erişilebilirken sağlık kontrolü girişsiz olarak `Healthy` (HTTP 200) döner.
- [ ] AC-6 — Veritabanına ulaşılamıyorken sağlık kontrolü `Unhealthy` (HTTP 503) döner; her iki
  durumda yanıt gövdesi yalnızca durum metnini içerir (veritabanı adı, bağlantı bilgisi, hata
  ayrıntısı yok).
- [ ] AC-7 — Beş yasak bağımlılık kuralının her biri için otomatik bir kontrol (mimari test veya
  derleme analyzer'ı) vardır. Her kontrolün ihlali yakaladığı, test projesinde bilerek kuralı
  ihlal eden bir örneğe karşı gösterilir; üretim kodu ihlal içermez. `DateTime.Now` /
  `DateTime.UtcNow` / `DateTimeOffset.Now` kullanımı derlemeyi kırar.
- [ ] AC-8 — `docs/architecture.md`'deki beş modül ve Web, belirtilen katman/proje adlarıyla
  çözümde mevcuttur ve derlenir.
- [ ] AC-9 — `LEAVE_TEST_DB` tanımlıyken entegrasyon testi gerçek PostgreSQL'e bağlanıp geçer;
  tanımlı değilken başarısız olur (atlanmış sayılmaz).
- [ ] AC-10 — E2E duman testi headless tarayıcıda açılış sayfasını açar ve başlığı doğrular;
  test bittikten sonra uygulama ve tarayıcı süreci kalmaz.
- [ ] AC-11 — `./scripts/check` ve `./scripts/check --fast` yeşil biter; `./scripts/residue` ve
  `./scripts/residue --fast` temiz biter; hızlı adımın test sayısı + full-only adımın test
  sayısı = toplam test sayısı.
- [ ] AC-12 — Bir uyarı (ör. null olabilecek bir referans) derlemeyi kırar; biçim kuralına
  uymayan bir dosya biçim adımını kırar.
- [ ] AC-13 — CI, bir PR'da `doctor` ve full `check` job'larını çalıştırır ve ikisi de yeşildir.
- [ ] AC-14 — Açılış ve "sayfa bulunamadı" sayfaları 360 px genişlikte yatay kaydırma olmadan
  görüntülenir.
- [ ] AC-15 — Açılış sayfasının başlığı ve metni ilk HTML yanıtında (sunucu ön-render) bulunur;
  JavaScript çalışmadan da görünür ("Yükleniyor" durumu boş ekran değildir).

## Definition of Done
- [ ] Every acceptance criterion mapped to proof (test or reproducible observation)
- [ ] `scripts/check` green
- [ ] Independent review done; real findings fixed, noise rejected with written rationale
- [ ] Docs / ADRs updated if behavior or architecture changed
- [ ] Spec moved to `specs/done/` (it becomes immutable there)

## Scorecard (fill at ship — honest numbers make the process improvable)
| Metric | Value |
|---|---|
| Spec revisions | |
| Fix rounds | |
| Review findings: real / noise | |
| Regressions introduced | |
| Bugs escaped to production | |
