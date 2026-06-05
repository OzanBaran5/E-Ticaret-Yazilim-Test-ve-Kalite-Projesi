using System;

namespace ECommerceApp.Core;

/// <summary>
/// Ürün sınıfı. Stok yönetimi burada yapılır.
/// </summary>
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }

    /// <summary>
    /// Stok kontrolü: ürünün satışa uygun olup olmadığını döner.
    /// </summary>
    public bool IsInStock() => Stock > 0;

    /// <summary>
    /// Stok azaltır.
    /// INTENTIONAL BUG #3: Stock negatife düşse de Exception fırlatılmıyor.
    /// Doğrusu: if (amount > Stock) throw new ArgumentException(...)
    /// </summary>
    public void DecreaseStock(int amount)
    {
        // BUG #3: Eksik validasyon — negatif stoka izin veriliyor
        Stock -= amount;
    }
}
