using System;
using System.ComponentModel.DataAnnotations;

namespace stkTakip.Models
{
    public class Stock
    {
        [Key]
        public int StockId { get; set; }
        
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        
        public int Quantity { get; set; }
        
        [StringLength(50)]
        public string Location { get; set; }
        
        [StringLength(100)]
        public string BatchNumber { get; set; }
        
        public DateTime? ExpiryDate { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
