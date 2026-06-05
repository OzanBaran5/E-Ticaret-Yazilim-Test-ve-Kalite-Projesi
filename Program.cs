using System;

namespace ECommerceApp;

class Program
{
    static void Main(string[] args)
    {
        Console.Title = "E-Ticaret Sistemi — Test Raporu";
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        WriteHeader();
        WriteBugList();
        WritePassedTests();
        WriteFailedTests();
        WriteFooter();

        Console.ReadKey();
    }

    static void WriteHeader()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║         E-TİCARET SİSTEMİ — YAZILIM TEST RAPORU         ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("  Kullanılan Framework : NUnit (.NET 9)");
        Console.WriteLine("  Test Türleri         : White Box | Black Box | Gray Box | Integration");
        Console.WriteLine("  Toplam Test          : 20");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("  Başarılı (PASS)      : 14");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("  Başarısız (FAIL)     : 6");
        Console.ResetColor();
        Console.WriteLine();
    }

    static void WriteBugList()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("══ SİSTEME EKLENEN BİLİNÇLİ HATALAR (BUGS) ══════════════");
        Console.ResetColor();

        var bugs = new[]
        {
            ("BUG #1", "Cart.cs", "KDV (+) yerine (-) ile çıkarılıyor. Defect (Kodlama hatası)"),
            ("BUG #2", "Cart.cs", "ApplyDiscount: rate/10 ile saklanıyor. %50→%5 uygulanıyor."),
            ("BUG #3", "Product.cs", "DecreaseStock: Eksi stok kontrolü yok. Fault (Mantık hatası)"),
            ("BUG #4", "OrderService.cs", "PlaceOrder: Stok 0 iken sipariş geçiyor. Eksik validasyon."),
            ("BUG #5", "OrderService.cs", "Minimum tutar: >= ile yazılmış, = durumunda reddediyor."),
            ("BUG #6", "OrderService.cs", "Hatalı ödemede sepet Clear() ile temizleniyor (veri kaybı)."),
        };

        foreach (var (id, file, desc) in bugs)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"  [{id}] ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"{file,-20} ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(desc);
        }

        Console.ResetColor();
        Console.WriteLine();
    }

    static void WritePassedTests()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("══ BAŞARILI (PASS) TESTLER ════════════════════════════════");
        Console.ResetColor();

        var passed = new[]
        {
            ("[White Box]     ", "CartCalculateTotal_ExactlyAt100_NoAutoDiscount"),
            ("[White Box]     ", "CartRemoveProduct_ReducesItemCount"),
            ("[White Box]     ", "ProductIsInStock_ReturnsTrueWhenStockPositive"),
            ("[White Box]     ", "ProductIsInStock_ReturnsFalseWhenStockZero"),
            ("[Black Box]     ", "AddProduct_ValidProduct_IncreasesCartCount"),
            ("[Black Box]     ", "DecreaseStock_ByOne_ReducesCorrectly"),
            ("[Black Box]     ", "PlaceOrder_BelowMinimumAmount_ShouldThrowException"),
            ("[Gray Box]      ", "PlaceOrder_ValidOrder_SetsCartToCheckoutStatus"),
            ("[Gray Box]      ", "PlaceOrder_AfterSuccess_StockDecreasesByOne"),
            ("[Gray Box]      ", "Cart_WithNoDiscount_DiscountRateRemainsZero"),
            ("[Integration]   ", "FullFlow_AddProduct_PlaceOrder_StockDecreases"),
            ("[Integration]   ", "FullFlow_BelowMinimumOrder_ShouldBeRejected"),
        };

        foreach (var (type, name) in passed)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("  ✓ ");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write(type);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(name);
        }

        Console.ResetColor();
        Console.WriteLine();
    }

    static void WriteFailedTests()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("══ BAŞARISIZ (FAIL) TESTLER — BUG YAKALANDI ══════════════");
        Console.ResetColor();

        var failed = new[]
        {
            ("[White Box]  ", "CartCalculateTotal_CorrectlyAppliesVAT",
             "Bug #1: KDV çıkarılıyor. Beklenen: 212.4 | Gerçek: 147.6"),
            ("[White Box]  ", "CartApplyDiscount_HalfPriceDiscount_ReducesPriceCorrectly",
             "Bug #2: Discount 0.5 yerine 0.05 saklanıyor."),
            ("[Black Box]  ", "DecreaseStock_WhenStockIsZero_ShouldThrowException",
             "Bug #3: Exception fırlatılmıyor, stok -1 oluyor."),
            ("[Black Box]  ", "PlaceOrder_WithZeroStockProduct_ShouldBeRejected",
             "Bug #4: Stok kontrolü yok, stoksuz ürün siparişe giriyor."),
            ("[Black Box]  ", "PlaceOrder_WithExactMinimumAmount_ShouldSucceed",
             "Bug #5: 50 TL sınır değerinde sipariş reddediliyor (>= hatası)."),
            ("[Gray Box]   ", "PlaceOrder_FailedPayment_CartItemsShouldBePreserved",
             "Bug #6: Hatalı ödemede sepet Items.Clear() ile siliniyor."),
        };

        foreach (var (type, name, reason) in failed)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("  ✗ ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(type);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(name);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"    → {reason}");
        }

        Console.ResetColor();
        Console.WriteLine();
    }

    static void WriteFooter()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("══════════════════════════════════════════════════════════");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("  Detaylı rapor: FinalTestReport.md");
        Console.WriteLine("  Visual Studio → Test Explorer ile testlerin tamamını çalıştırabilirsiniz.");
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("  Çıkmak için bir tuşa basın...");
        Console.ResetColor();
    }
}
