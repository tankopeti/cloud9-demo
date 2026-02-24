using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cloud9_2.Models
{
    [Table("PriceTypes")]
    public class PriceType
    {
        [Key]
        public int PriceTypeId { get; set; }

        [Required, MaxLength(30)]
        public string Code { get; set; } = null!;   // LIST / SALES / PURCHASE

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public ICollection<ItemPrice> ItemPrices { get; set; } = new List<ItemPrice>();
    }
}
