using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cloud9_2.Models
{
    [Table("Items")]
    public class Item
    {
        [Key]
        public int ItemId { get; set; }

        [Required]
        public int TenantId { get; set; }

        [Required, MaxLength(100)]
        public string Code { get; set; } = null!;   // SKU / Service kód

        [Required, MaxLength(200)]
        public string Name { get; set; } = null!;

        [MaxLength(1000)]
        public string? Description { get; set; }

        // DB lookup
        [Required]
        public int ItemTypeId { get; set; }

        [ForeignKey(nameof(ItemTypeId))]
        public ItemTypeLookup ItemType { get; set; } = null!;

        // Készlet / gyártás flag-ek (DB-ben tárolva)
        public bool IsStockManaged { get; set; } = true;
        public bool IsManufactured { get; set; } = false;

        // Defaultok bizonylat sorhoz
        public int? DefaultUnitId { get; set; }      // UnitOfMeasurement FK (ha van)
        public int? DefaultTaxCodeId { get; set; }   // TaxCode FK (ha van)

        [Column(TypeName = "decimal(18,4)")]
        public decimal? DefaultSalesPrice { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? DefaultPurchasePrice { get; set; }

        public bool IsActive { get; set; } = true;

        // Audit
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(450)]
        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [MaxLength(450)]
        public string? UpdatedBy { get; set; }
public ICollection<ItemPrice> ItemPrices { get; set; } = new List<ItemPrice>();

    }

    // DTO-k (ha kéred, mehet ugyanebbe a fájlba)
    public class ItemDto
    {
        public int ItemId { get; set; }
        public int TenantId { get; set; }

        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public int ItemTypeId { get; set; }

        public bool IsStockManaged { get; set; }
        public bool IsManufactured { get; set; }

        public int? DefaultUnitId { get; set; }
        public int? DefaultTaxCodeId { get; set; }

        public decimal? DefaultSalesPrice { get; set; }
        public decimal? DefaultPurchasePrice { get; set; }

        public bool IsActive { get; set; }
    }

    public class ItemCreateDto
    {
        [Required]
        public int TenantId { get; set; }

        [Required, MaxLength(100)]
        public string Code { get; set; } = null!;

        [Required, MaxLength(200)]
        public string Name { get; set; } = null!;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public int ItemTypeId { get; set; }

        public bool IsStockManaged { get; set; } = true;
        public bool IsManufactured { get; set; } = false;

        public int? DefaultUnitId { get; set; }
        public int? DefaultTaxCodeId { get; set; }

        public decimal? DefaultSalesPrice { get; set; }
        public decimal? DefaultPurchasePrice { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class ItemUpdateDto
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = null!;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public int ItemTypeId { get; set; }

        public bool IsStockManaged { get; set; }
        public bool IsManufactured { get; set; }

        public int? DefaultUnitId { get; set; }
        public int? DefaultTaxCodeId { get; set; }

        public decimal? DefaultSalesPrice { get; set; }
        public decimal? DefaultPurchasePrice { get; set; }

        public bool IsActive { get; set; }
    }
}
