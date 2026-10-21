# Domain

Kodda İngilizce karşılık parantez içinde verilir; konuşmada ve spec'lerde Türkçe terim kullanılır.

## Ubiquitous language

| Term | Meaning | Notes / not to be confused with |
|---|---|---|
| Çalışan (`Employee`) | Sisteme hesabı olan Polsan personeli. İşe giriş tarihi, doğum tarihi ve (varsa) işten çıkış tarihi tutulur. | Kullanıcı hesabı (`UserAccount`) kimlik doğrulama içindir; çalışan iş varlığıdır. |
| Yönetici (`Manager`) | Bir çalışanın izin taleplerini onaylayan doğrudan amir. Her çalışanın en fazla bir doğrudan yöneticisi vardır. | Bir rol değil, ilişkidir; yöneticinin de kendi yöneticisi olabilir. |
| İK (`HR`) | Çalışanları, izin türlerini, tatil takvimini ve bakiye düzeltmelerini yöneten rol. | — |
| Rol (`Role`) | `Employee`, `Manager`, `HR`. Bir kişide birden fazla rol olabilir. | — |
| İzin türü (`LeaveType`) | Yıllık, mazeret (evlilik, ölüm, babalık vb.), ücretsiz, hastalık/rapor gibi tanımlar. Bakiyeden düşüp düşmediği türde tanımlıdır. | Yalnızca yıllık izin md.53 hak edişine tabidir. |
| İzin talebi (`LeaveRequest`) | Çalışanın tür + başlangıç/bitiş tarihi (+ yarım gün bilgisi) ile oluşturduğu talep. | Onaylanınca "izin" olur; ayrı bir varlık değildir. |
| Talep durumu (`RequestStatus`) | `Pending` (Beklemede), `Approved` (Onaylandı), `Rejected` (Reddedildi), `Cancelled` (İptal). | — |
| İzin günü (`LeaveDays`) | Bir talebin iş günü cinsinden süresi; 0,5 katlarıdır. | Takvim günü değildir (BR-2). |
| İş günü (`WorkingDay`) | Hafta tatili (Cumartesi, Pazar) ve resmî tatil olmayan gün. | — |
| Resmî tatil (`PublicHoliday`) | İK'nın yıllık olarak girdiği ulusal bayram ve genel tatil günleri (yarım gün arifeler dahil). | — |
| Hak ediş yılı (`EntitlementYear`) | İşe giriş yıldönümünden başlayan bir yıllık dönem. | Takvim yılı değildir. |
| Hak (`Entitlement`) | Bir hak ediş yılında kazanılan yıllık izin günü: md.53 yasal gün + İK ek günü. | — |
| Bakiye (`Balance`) | Bakiye hareketlerinin toplamından türetilen kalan gün. Saklanan tek bir sayı değildir. | — |
| Bakiye hareketi (`BalanceEntry`) | Defterdeki bir satır: `Accrual` (hak), `Usage` (kullanım), `Reversal` (iptal iadesi), `Adjustment` (İK düzeltmesi). | Silinmez, güncellenmez. |
| Kıdem (`Seniority`) | İşe giriş tarihinden hak ediş tarihine kadar geçen tam yıl. | — |

## Business rules
- **BR-1:** Yıllık izin hakkı 4857 sayılı İş Kanunu md.53'e göre otomatik hesaplanır: kıdem
  kıdem 1 yıldan 5 yıla kadar (5 yıl dahil) → 14 gün; 5 yıldan fazla, 15 yıldan az → 20 gün;
  15 yıl (dahil) ve üzeri → 26 gün. 18 yaş ve altı veya 50 yaş ve üzeri çalışanlara en az 20 gün
  verilir. Sınır değerler (tam 5 yıl, tam 15 yıl, 18/50 yaş) testle sabitlenir. Bir yılını doldurmamış
  çalışanın yıllık izin hakkı yoktur. İK yasal günün üzerine ek gün tanımlayabilir, altına inemez.
- **BR-2:** İzin süresi iş günü olarak sayılır; hafta tatili ve İK'nın girdiği resmî tatiller
  izin süresinden düşülmez (md.56).
- **BR-3:** En küçük izin birimi 0,5 gündür (sabah veya öğleden sonra).
- **BR-4:** Kullanılmayan yıllık izin yanmaz; süresiz olarak sonraki hak ediş yılına devreder.
- **BR-5:** Bakiyeden düşen bir türde, kalan bakiyeyi aşan talep oluşturulamaz ve onaylanamaz
  (eksi bakiye yoktur). Bekleyen talepler kalan bakiyeden ayrılmış (rezerve) sayılır.
- **BR-6:** Talebi çalışanın doğrudan yöneticisi onaylar veya reddeder. Doğrudan yöneticisi
  olmayan çalışanın talebini İK onaylar. Kimse kendi talebini onaylayamaz.
- **BR-7:** Çalışan `Pending` veya başlangıç tarihi henüz gelmemiş `Approved` talebini iptal
  edebilir; iptal edilen gün bakiyeye `Reversal` hareketiyle geri döner.
- **BR-8:** Bir çalışanın aynı güne (aynı yarım güne) düşen `Pending` veya `Approved` iki talebi
  olamaz.
- **BR-9:** Red gerekçesi zorunludur.
- **BR-10:** İşten çıkış tarihi geçmiş çalışanın hesabı pasiftir; yeni talep oluşturamaz.

## Key domain invariants
- Bakiye her zaman bakiye hareketlerinin toplamıdır; hareketler değiştirilmez veya silinmez —
  düzeltme yeni bir `Adjustment` hareketidir.
- Kalan yıllık izin bakiyesi hiçbir zaman sıfırın altına düşmez.
- Yasal md.53 hakkının altında bir yıllık hak oluşmaz.
- Her durum değişikliği (onay, red, iptal) ve bakiye düzeltmesi denetim kaydı üretir; ikisi aynı
  transaction'da yazılır.
- `Approved`, `Rejected`, `Cancelled` son durumlardır; yalnızca `Approved` → `Cancelled`
  geçişine (BR-7) izin verilir.
