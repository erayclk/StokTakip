using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace stkTakip.Models
{
    public class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        [Key]
        public int CategoryId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string CategoryName { get; set; }
        
        [StringLength(200)]
        public string Description { get; set; }
        
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
