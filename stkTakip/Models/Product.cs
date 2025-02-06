using System;
using System.ComponentModel.DataAnnotations;

namespace stkTakip.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }
        
        [StringLength(50)]
        public string SKU { get; set; }
        
        [StringLength(20)]
        public string Barcode { get; set; }
        
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        
        [Required]
        public decimal UnitPrice { get; set; }
        
        public int MinimumStockLevel { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
