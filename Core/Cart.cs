using System;
using System.Collections.Generic;
using System.Linq;

namespace ECommerceApp.Core;

/// <summary>
/// Sepet sınıfı. İndirim, KDV ve minimum tutar hesaplamaları burada yapılır.
/// </summary>
public class Cart
{
    public static readonly decimal MinimumOrderAmount = 50m;
    public List<Product> Items { get; set; } = new();
    public string Status { get; set; } = "Active";

    private decimal _discountRate = 0m; // 0.0 - 1.0 arası

    public void AddProduct(Product product)
    {
        Items.Add(product);
    }

    public void RemoveProduct(Product product)
    {
        Items.Remove(product);
    }

    /// <summary>
    /// İndirim oranı uygular (0.0 ile 1.0 arasında, örneğin %10 için 0.10).
    /// INTENTIONAL BUG #2: İndirim oranı 10 ile bölünüyor, dolayısıyla
    /// %50 (0.5) girildiğinde aslında %5 (0.05) uygulanıyor.
    /// Doğrusu: _discountRate = rate;
    /// </summary>
    public void ApplyDiscount(decimal rate)
    {
        // BUG #2: Discount rate yanlış hesaplanıyor
        _discountRate = rate / 10m;
    }

    public decimal GetDiscountRate() => _discountRate;

    /// <summary>
    /// Sepet toplamını hesaplar. İndirim ve %18 KDV uygulanır.
    /// INTENTIONAL BUG #1: KDV toplamdan çıkartılıyor, eklenmesi lazım.
    /// </summary>
    public decimal CalculateTotal()
    {
        decimal subtotal = Items.Sum(item => item.Price);

        // İndirim uygula
        decimal afterDiscount = subtotal * (1m - _discountRate);

        // %10 otomatik indirim (100 TL üzeri siparişlerde)
        if (afterDiscount > 100m)
            afterDiscount *= 0.9m;

        // BUG #1: KDV çıkarılıyor, eklenmesi lazım
        decimal tax = afterDiscount * 0.18m;
        return afterDiscount - tax; // HATALI: + tax olmalı
    }
}
