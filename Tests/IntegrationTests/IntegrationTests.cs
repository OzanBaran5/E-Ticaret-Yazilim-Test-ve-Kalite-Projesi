using System;
using NUnit.Framework;
using ECommerceApp.Core;

namespace ECommerceApp.Tests.IntegrationTests;

[TestFixture]
public class IntegrationTests
{
    // =========================================================
    // INTEGRATION TESTLER (4 adet)
    // Birden fazla modül bir arada (Cart + Product + OrderService) test edilir.
    // =========================================================

    /// <summary>
    /// PASS: Tam akış — ürün ekle, sipariş ver, stok düşmeli.
    /// EP: Geçerli tam akış senaryosu.
    /// </summary>
    [Test]
    [Category("Integration Test")]
    public void FullFlow_AddProduct_PlaceOrder_StockDecreases()
    {
        var product = new Product { Id = 101, Name = "Monitör", Price = 200m, Stock = 3 };
        var cart = new Cart();
        var service = new OrderService();

        cart.AddProduct(product);
        var order = service.PlaceOrder(cart, 1000m);

        Assert.That(order.IsSuccessful, Is.True);
        Assert.That(cart.Status, Is.EqualTo("Checkout"));
        Assert.That(product.Stock, Is.EqualTo(2));
    }

    /// <summary>
    /// FAIL: Bug #2 + Bug #1 zincirleme — discount hatalı uygulanıp KDV de yanlış çıkınca
    /// hesaplanan toplam beklenen değerden çok farklı olur.
    /// BVA: %50 indirim sınır senaryosu.
    /// </summary>
    [Test]
    [Category("Integration Test")]
    public void FullFlow_MultipleProducts_WithDiscount_TotalShouldBeCorrect()
    {
        var p1 = new Product { Id = 102, Name = "Ürün A", Price = 100m, Stock = 5 };
        var p2 = new Product { Id = 103, Name = "Ürün B", Price = 100m, Stock = 5 };
        var cart = new Cart();
        cart.AddProduct(p1);
        cart.AddProduct(p2);

        cart.ApplyDiscount(0.5m); // %50 indirim

        decimal total = cart.CalculateTotal();

        // Doğru hesap: 200 → %50 disc → 100 → 100 eşit sınır, oto-indirim yok → +%18 KDV → 118
        // Bug #2: discount 0.5/10=0.05 → 200*0.95=190 → 190>100 → *0.9=171 → -KDV(Bug#1) → 171-30.78=140.22
        Assert.That(total, Is.EqualTo(118m),
            "İndirim ve KDV doğru uygulandığında toplam 118 TL olmalıdır.");
    }

    /// <summary>
    /// PASS: Minimum tutarın altında sipariş reddedilmeli.
    /// EP: Geçersiz (düşük tutarlı) senaryo.
    /// BVA: 49 TL — minimum sınırın (50) hemen altı.
    /// </summary>
    [Test]
    [Category("Integration Test")]
    public void FullFlow_BelowMinimumOrder_ShouldBeRejected()
    {
        var product = new Product { Id = 104, Name = "Kalem", Price = 40m, Stock = 10 };
        var cart = new Cart();
        var service = new OrderService();
        cart.AddProduct(product);

        // 40 TL ürün → hesaplanan total minimum altında → exception beklenir
        Assert.Throws<InvalidOperationException>(() => service.PlaceOrder(cart, 500m));
    }

    /// <summary>
    /// FAIL: Bug #4 — Sıfır stoktaki ürün siparişe giriyor, reddedilmeli ama reddedilmiyor.
    /// EP: Geçersiz senaryo — stoksuz ürün.
    /// BVA: Stock = 0 sınır değer.
    /// </summary>
    [Test]
    [Category("Integration Test")]
    public void FullFlow_OutOfStockProduct_OrderShouldBeRejected()
    {
        var product = new Product { Id = 105, Name = "Laptop", Price = 500m, Stock = 0 };
        var cart = new Cart();
        var service = new OrderService();
        cart.AddProduct(product);

        // Bug #4: Stok kontrolü yok, exception fırlatılmıyor
        Assert.Throws<InvalidOperationException>(() => service.PlaceOrder(cart, 2000m),
            "Stoksuz ürün siparişi reddedilmelidir.");
    }
}
