using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cloud9_2.Models
{
    [Table("ItemPrices")]
    public class ItemPrice
    {
        [Key]
        public long ItemPriceId { get; set; }

        [Required]
        public int TenantId { get; set; }

        [Required]
        public int ItemId { get; set; }

        [Required]
        public int CurrencyId { get; set; }

        [Required]
        public int PriceTypeId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal UnitPrice { get; set; }

        // SQL: DATE
        [Column(TypeName = "date")]
        public DateTime? ValidFrom { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ValidTo { get; set; }

        public bool IsActive { get; set; } = true;

        // Audit (Items-hez illesztve)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(450)]
        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [MaxLength(450)]
        public string? UpdatedBy { get; set; }

        // Navigation
        [ForeignKey(nameof(ItemId))]
        public Item Item { get; set; } = null!;

        [ForeignKey(nameof(CurrencyId))]
        public Currency Currency { get; set; } = null!;

        [ForeignKey(nameof(PriceTypeId))]
        public PriceType PriceType { get; set; } = null!;
    }
}
