using System;

namespace ECommerceApp.Core;

/// <summary>
/// Sipariş servisi. Stok kontrolü, minimum tutar ve ödeme işlemleri burada yapılır.
/// </summary>
public class OrderService
{
    public class Order
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// Sipariş oluşturur.
    /// INTENTIONAL BUG #4: Stok kontrolü eksik — stokta olmayan ürün sipariş ediliyor.
    /// INTENTIONAL BUG #5: Minimum tutar kontrolü '>=' ile yapılıyor,
    ///   50 TL'ye eşit siparişler de reddediliyor (olmamalı).
    /// INTENTIONAL BUG #6: Hatalı ödemede sepet temizleniyor (veri kaybı).
    /// </summary>
    public Order PlaceOrder(Cart cart, decimal paymentAmount)
    {
        // BUG #4: Stok kontrolü YOK — stokta olmayan ürünler siparişe giriyor
        // Doğrusu: foreach item => if (!item.IsInStock()) throw new InvalidOperationException(...)

        decimal total = cart.CalculateTotal();

        // BUG #5: '>=' kullanılmış, '>' olmalı.
        // Minimum tutar SINIR değerinde (50 TL) olan sipariş reddediliyor.
        if (total >= Cart.MinimumOrderAmount == false)
        {
            throw new InvalidOperationException(
                $"Minimum sipariş tutarı {Cart.MinimumOrderAmount} TL olmalıdır. Mevcut: {total} TL");
        }

        if (paymentAmount < total)
        {
            // BUG #6: Exception öncesi sepet temizleniyor — veri kaybı
            cart.Items.Clear();
            throw new ArgumentException("Yetersiz ödeme tutarı!");
        }

        // Stok düşür ve durumu güncelle
        foreach (var item in cart.Items)
            item.DecreaseStock(1);

        cart.Status = "Checkout";

        return new Order { IsSuccessful = true, Message = "Sipariş başarıyla oluşturuldu." };
    }
}
