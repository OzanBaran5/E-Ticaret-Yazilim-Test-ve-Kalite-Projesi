# 📊 E-Ticaret Sistemi — Final Test Raporu

**Proje:** ECommerceApp — C# / NUnit / .NET 9  
**Tarih:** Haziran 2026  
**Test Çerçevesi:** NUnit 4.6.1 + NUnit3TestAdapter  

---

## 📌 Test Özeti

| Metrik | Değer |
|---|---|
| Toplam Test Senaryosu | 20 |
| ✅ Başarılı (PASS) | 13 |
| ❌ Başarısız (FAIL) | 7 |
| Tespit Edilen Bug Sayısı | 6 |

---

## 🔁 1. STLC — Yazılım Test Yaşam Döngüsü

### Aşama 1: Requirement (Gereksinim Analizi)
E-ticaret sistemi aşağıdaki işlevleri yerine getirmelidir:
- Ürün stoğu yönetimi (stok yoksa sipariş verilmemeli)
- Sepete ürün ekleme / çıkarma
- Discount (indirim) uygulaması
- %18 KDV hesaplama
- Minimum sipariş tutarı kontrolü (50 TL)
- Ödeme işlemi ve sipariş oluşturma

### Aşama 2: Test Plan
- **Kapsam:** Core sınıflar (Product, Cart, OrderService)
- **Test Türleri:** Unit (White/Black/Gray Box) + Integration
- **Teknikler:** Equivalence Partitioning (EP) + Boundary Value Analysis (BVA)
- **Araçlar:** NUnit 4.6.1, Visual Studio 2022, dotnet test CLI

### Aşama 3: Test Design
- Toplam 20 test senaryosu tasarlandı
- Her gereksinim için geçerli (valid), geçersiz (invalid) ve sınır (boundary) değerler belirlendi
- 6 adet kasıtlı hata (bug) sisteme enjekte edildi
- `[Category("White Box")]` gibi NUnit attribute'ları ile testler sınıflandırıldı

### Aşama 4: Test Execution
```
dotnet test --verbosity minimal
```
> Sonuç: **Başarısız: 7 | Başarılı: 13 | Toplam: 20**

### Aşama 5: Test Result & Reporting
Test sonuçları aşağıdaki bölümlerde raporlanmıştır. Başarısız testlerin her biri için köklü nedeni (Root Cause) ve bağlı olduğu bug ID'si belirtilmiştir.

---

## 🐛 2. Hata Kavramları — Error, Fault, Failure, Defect/Bug

| Kavram | Tanım | Projeden Örnek |
|---|---|---|
| **Error** | Yazılımcının kodlama sırasında yaptığı hatalı zihinsel karar. | Geliştiricinin KDV'yi `+` yerine `-` yazması düşüncesi. |
| **Fault (Defect/Bug)** | Error'ın kaynak koda yansıması; koddaki somut yanlış satır. | `Cart.cs` içinde `return afterDiscount - tax;` satırı. |
| **Failure** | Fault'un çalışma zamanında (runtime) ortaya çıkması; sistemin beklenen davranışı sergilememesi. | `CalculateTotal()` metodu 212.4 yerine 147.6 döndürür. |
| **Defect/Bug** | Fault ve Failure'ı birlikte tanımlayan genel terim; raporlama sürecinde kullanılır. | Bug #1: KDV hesaplama hatası, Bug #3: Eksi stok zafiyeti. |

---

## 🧪 3. Test Türleri

### White Box Testing
Testeri kodun iç yapısını (if/else dalları, private alanlar, döngüler) bilir. Bu projede `CalculateTotal()` metodunun KDV dallanması ve `ApplyDiscount()` metodunun `_discountRate` alanını nasıl güncellediği test edilmiştir.

### Black Box Testing
Tester yalnızca girdi-çıktı ilişkisine bakar; iç kodu bilmez. Bu projede EP ve BVA tekniklerine göre stok azaltma, ürün ekleme ve sipariş tutarı sınır değerleri test edilmiştir.

### Gray Box Testing
Kısmi iç bilgi kullanılır. Bu projede sipariş sonrası `cart.Status` durumu ve hatalı ödemeden sonra `cart.Items` koleksiyonunun korunup korunmadığı incelenmiştir.

### Integration Testing
Birden fazla modülün (Cart + Product + OrderService) birlikte doğru çalışıp çalışmadığı test edilmiştir. Bug #2 (discount) + Bug #1 (KDV) zincirleme etkisi entegrasyon testinde yakalanmıştır.

---

## 📐 4. Test Case Dizayn Teknikleri

### Equivalence Partitioning (EP)
Girdiler "geçerli" ve "geçersiz" sınıflara bölünür; her sınıftan bir temsili test değeri seçilir.

| Sınıf | Örnek Değer | Test |
|---|---|---|
| Geçerli Stok | Stock = 5 | `ProductIsInStock_ReturnsTrueWhenStockPositive` |
| Geçersiz Stok (0) | Stock = 0 | `ProductIsInStock_ReturnsFalseWhenStockZero` |
| Geçersiz Stok (<0) | Stock = -1 | `DecreaseStock_WhenStockIsZero_ShouldThrowException` |
| Geçerli Ödeme | 1000 TL | `PlaceOrder_ValidOrder_SetsCartToCheckoutStatus` |
| Geçersiz Ödeme | 1 TL | `PlaceOrder_FailedPayment_CartItemsShouldBePreserved` |

### Boundary Value Analysis (BVA)
Değerlerin sınır noktaları (min, min+1, max, max-1) test edilir.

| Sınır Değeri | Test Senaryosu |
|---|---|
| Stock = 0 (min sınır) | `DecreaseStock_WhenStockIsZero_ShouldThrowException` |
| Stock = 1 (min+1) | `DecreaseStock_ByOne_ReducesCorrectly` |
| Fiyat = 100 TL (oto-indirim sınırı) | `CartCalculateTotal_ExactlyAt100_NoAutoDiscount` |
| Fiyat = 20 TL (min tutar altı) | `PlaceOrder_BelowMinimumAmount_ShouldThrowException` |
| Discount = 0.5 (%50 sınır) | `CartApplyDiscount_HalfPriceDiscount_ReducesPriceCorrectly` |

---

## 📈 5. Test Stratejileri

### Agile Testing
Test aktiviteleri geliştirme ile paralel yürütülür; her yeni özellik (stok kontrolü, discount, minimum tutar) eklendiğinde anında test yazılır ve CI ortamında `dotnet test` ile koşturulur.

### Risk-Based Testing
En kritik ve yüksek riskli bileşenler öncelikli test edilir. Bu projede en büyük risk: ödeme hesaplama hataları (Bug #1, #2) ve veri kaybı (Bug #6). Bu bileşenler hem unit hem integration seviyesinde test edilmiştir.

### Regression Testing
Yeni özellikler eklendiğinde (discount, minimum tutar, stok kontrolü) önceki testlerin bozulup bozulmadığı kontrol edilir. `dotnet test` tüm test suite'ini tek komutla çalıştırarak regresyon güvencesi sağlar.

---

## 🔴 6. Başarısız (FAIL) Testler ve Nedenleri

| # | Test Adı | Kategori | Beklenen | Gerçekleşen | Bug |
|---|---|---|---|---|---|
| 1 | `CartCalculateTotal_CorrectlyAppliesVAT` | White Box | 212.4m | 147.6m | #1 |
| 2 | `CartApplyDiscount_HalfPriceDiscount_ReducesPriceCorrectly` | White Box | 0.5m | 0.05m | #2 |
| 3 | `DecreaseStock_WhenStockIsZero_ShouldThrowException` | Black Box | ArgumentException | null | #3 |
| 4 | `PlaceOrder_WithZeroStockProduct_ShouldBeRejected` | Black Box | InvalidOperationException | null | #4 |
| 5 | `PlaceOrder_FailedPayment_CartItemsShouldBePreserved` | Gray Box | Items.Count = 1 | Items.Count = 0 | #6 |
| 6 | `FullFlow_MultipleProducts_WithDiscount_TotalShouldBeCorrect` | Integration | 118m | 140.22m | #1+#2 |
| 7 | `FullFlow_OutOfStockProduct_OrderShouldBeRejected` | Integration | InvalidOperationException | null | #4 |

### Detaylı Bug Listesi

#### Bug #1 — KDV Hesaplama Hatası *(Fault: Kodlama Hatası)*
- **Dosya:** `Core/Cart.cs` — `CalculateTotal()` metodu
- **Hatalı Satır:** `return afterDiscount - tax;`
- **Doğrusu:** `return afterDiscount + tax;`
- **Etki:** Tüm sepet tutarları yanlış. Ödeme tutarları eksik hesaplanıyor.
- **Error:** Geliştirici operatörü yanlış yazmış.
- **Failure:** 212.4 beklenen yerde 147.6 dönüyor.

#### Bug #2 — Discount Rate Hatası *(Fault: Mantık Hatası)*
- **Dosya:** `Core/Cart.cs` — `ApplyDiscount()` metodu
- **Hatalı Satır:** `_discountRate = rate / 10m;`
- **Doğrusu:** `_discountRate = rate;`
- **Etki:** %50 indirim girilince %5 uygulanıyor. Müşteri fazla ödüyor.
- **Error:** Geliştirici 10'a bölme formülünü yanlış yazmış.

#### Bug #3 — Eksi Stok Zafiyeti *(Fault: Eksik Validasyon)*
- **Dosya:** `Core/Product.cs` — `DecreaseStock()` metodu
- **Eksik Kod:** `if (amount > Stock) throw new ArgumentException(...)`
- **Etki:** Stok 0 iken ürün satışı onaylanıyor; stok negatif değerlere iniyor.

#### Bug #4 — Stok Kontrolü Eksikliği *(Fault: Eksik İş Kuralı)*
- **Dosya:** `Core/OrderService.cs` — `PlaceOrder()` metodu
- **Eksik Kod:** Her ürün için `IsInStock()` kontrolü yapılmıyor.
- **Etki:** Stokta olmayan ürünler siparişe giriyor; envanter tutarsızlaşıyor.

#### Bug #5 — Minimum Tutar Sınır Hatası *(Fault: Boundary Hatası)*
- **Dosya:** `Core/OrderService.cs` — `PlaceOrder()` metodu
- **Hatalı Satır:** `if (total >= Cart.MinimumOrderAmount == false)`
- **Açıklama:** `>=` operatörü nedeniyle eşit değer de (50 TL) reddediliyor.
- **Doğrusu:** `if (total > Cart.MinimumOrderAmount == false)` veya `total < MinimumOrderAmount`

#### Bug #6 — Hatalı Ödemede Sepet Veri Kaybı *(Fault: Kritik Mantık Hatası)*
- **Dosya:** `Core/OrderService.cs` — `PlaceOrder()` metodu
- **Hatalı Satır:** `cart.Items.Clear();` (exception öncesi)
- **Etki:** Yetersiz ödemede müşterinin sepeti tamamen siliniyor. Kritik veri kaybı.
- **Failure:** Hatalı ödemeden sonra Items.Count = 0 oluyor; kullanıcı sepetini kaybediyor.

---

## 🟢 7. Başarılı (PASS) Testler

| # | Test Adı | Kategori |
|---|---|---|
| 1 | `CartCalculateTotal_ExactlyAt100_NoAutoDiscount` | White Box |
| 2 | `CartRemoveProduct_ReducesItemCount` | White Box |
| 3 | `ProductIsInStock_ReturnsTrueWhenStockPositive` | White Box |
| 4 | `ProductIsInStock_ReturnsFalseWhenStockZero` | White Box |
| 5 | `AddProduct_ValidProduct_IncreasesCartCount` | Black Box |
| 6 | `DecreaseStock_ByOne_ReducesCorrectly` | Black Box |
| 7 | `PlaceOrder_BelowMinimumAmount_ShouldThrowException` | Black Box |
| 8 | `PlaceOrder_WithExactMinimumAmount_ShouldSucceed` | Black Box |
| 9 | `PlaceOrder_ValidOrder_SetsCartToCheckoutStatus` | Gray Box |
| 10 | `PlaceOrder_AfterSuccess_StockDecreasesByOne` | Gray Box |
| 11 | `Cart_WithNoDiscount_DiscountRateRemainsZero` | Gray Box |
| 12 | `FullFlow_AddProduct_PlaceOrder_StockDecreases` | Integration |
| 13 | `FullFlow_BelowMinimumOrder_ShouldBeRejected` | Integration |
