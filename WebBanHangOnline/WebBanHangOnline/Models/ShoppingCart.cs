using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBanHangOnline.Models
{
    public class ShoppingCart
    {
        public List<ShoppingCartItem> Items { get; set; }
        public int PromotionId { get; set; } // Thêm thuộc tính PromotionId
        public string PromotionCode { get; set; } // Thêm thuộc tính PromotionCode
        public decimal DiscountAmount { get; set; } // Thêm thuộc tính PromotionCode
        public int TypePromotion { get; set; } // Thêm thuộc tính PromotionCode

        public ShoppingCart() 
        {
            this.Items = new List<ShoppingCartItem>();
        }

        public void AddToCart(ShoppingCartItem item,int quantity)
        {
            var checkExists = Items.FirstOrDefault(x => x.ProductId == item.ProductId);
            if (checkExists != null)
            {
                checkExists.Quantity += quantity;
                checkExists.TotalPrice = checkExists.Price * checkExists.Quantity;
            }
            else
            {
                Items.Add(item);
            }
        }

        public void Remove(int id)
        {
            var checkExists = Items.SingleOrDefault(x=> x.ProductId == id);
            if (checkExists != null)
            {
                Items.Remove(checkExists);
            }
        }

        public void UpdateQuantity(int id,int quantity)
        {
            var checkExists = Items.SingleOrDefault(x=>x.ProductId == id);
            if(checkExists != null)
            {
                checkExists.Quantity = quantity;
                checkExists.TotalPrice = checkExists.Price * checkExists.Quantity;
            }
        }

        public decimal GetTotalPrice()
        {
            return Items.Sum(x => x.TotalPrice);
        }

        public decimal GetTotalQuantity()
        {
            return Items.Sum(x => x.Quantity);
        }

        public void ClearCart()
        {
            Items.Clear();
            PromotionId = 0; 
            PromotionCode = null; 
            DiscountAmount = 0; 
            TypePromotion = 0;
        }
    }

    public class ShoppingCartItem
    {
        public int ProductId { get; set; }
        public int ProductStoreID { get; set; }
        public string ProductName { get; set; }
        public string Alias { get; set; }
        public string CategoryName { get; set; }
        public string ProductImg { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public string PromotionCode { get; set; }
        public decimal DiscountAmount { get; set; }
        public int PromotionId { get; set; }
        public decimal TotalAmount { get; set; }
        public int TypePromotion { get; internal set; }
        public decimal PhiShip { get; set; }
    }
}