using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cloud9_2.Models
{
    [Table("BusinessDocumentLines")]
    public class BusinessDocumentLine
    {
        [Key]
        public int BusinessDocumentLineId { get; set; }

        [Required]
        public int TenantId { get; set; }

        [ForeignKey(nameof(TenantId))]
        public Tenant Tenant { get; set; } = null!;

        [Required]
        public int BusinessDocumentId { get; set; }

        [ForeignKey(nameof(BusinessDocumentId))]
        public BusinessDocument BusinessDocument { get; set; } = null!;

        public int LineNo { get; set; } = 1;

        // =========================
        // ITEM (kötelező)
        // =========================
        [Required]
        public int ItemId { get; set; }

        [ForeignKey(nameof(ItemId))]
        public Item Item { get; set; } = null!;

        // =========================
        // SNAPSHOT mezők (kötelező)
        // =========================
        [Required, MaxLength(100)]
        public string ItemCodeSnapshot { get; set; } = null!;

        [Required, MaxLength(200)]
        public string ItemNameSnapshot { get; set; } = null!;

        [Required, MaxLength(50)]
        public string UomSnapshot { get; set; } = null!;

        // Ha TaxCode-ban nem "rate" van, hanem csak kód, akkor ez maradhat TaxCodeSnapshot is.
        [Column(TypeName = "decimal(9,4)")]
        public decimal VatRateSnapshot { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal UnitPriceSnapshot { get; set; }

        // =========================
        // Sor saját mezők
        // =========================
        [MaxLength(500)]
        public string? Description { get; set; }   // szabad szöveg, de snapshot ettől még él

        [Column(TypeName = "decimal(18,4)")]
        public decimal Quantity { get; set; } = 1;

        // Ha később kell mégis, maradhatnak, de a snapshot az igazság.
        public int? UnitId { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? UnitPrice { get; set; } // legacy / override (opcionális)

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? NetAmount { get; set; }

        public int? TaxCodeId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TaxAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? GrossAmount { get; set; }

        public int? WarehouseId { get; set; }
    }

public class BusinessDocumentLineDto
{
    public int BusinessDocumentLineId { get; set; }
    public int TenantId { get; set; }
    public int BusinessDocumentId { get; set; }
    public int LineNo { get; set; }

    public int ItemId { get; set; }

    public string ItemCodeSnapshot { get; set; } = null!;
    public string ItemNameSnapshot { get; set; } = null!;
    public string UomSnapshot { get; set; } = null!;
    public decimal VatRateSnapshot { get; set; }
    public decimal UnitPriceSnapshot { get; set; }

    public string? Description { get; set; }
    public decimal Quantity { get; set; }

    public int? UnitId { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal? NetAmount { get; set; }
    public int? TaxCodeId { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal? GrossAmount { get; set; }
    public int? WarehouseId { get; set; }
}

public class BusinessDocumentLineCreateDto
{
    [Required]
    public int TenantId { get; set; }

    [Required]
    public int BusinessDocumentId { get; set; }

    public int LineNo { get; set; } = 1;

    [Required]
    public int ItemId { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public decimal Quantity { get; set; } = 1;

    public int? UnitId { get; set; }
    public decimal? UnitPrice { get; set; } // opcionális override
    public decimal? DiscountAmount { get; set; }
    public int? TaxCodeId { get; set; }
    public int? WarehouseId { get; set; }
}

public class BusinessDocumentLineUpdateDto
{
    public int LineNo { get; set; }

    public int ItemId { get; set; }  // ha nem akarod engedni, vedd ki

    [MaxLength(500)]
    public string? Description { get; set; }

    public decimal Quantity { get; set; }
    public int? UnitId { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? DiscountAmount { get; set; }
    public int? TaxCodeId { get; set; }
    public int? WarehouseId { get; set; }
}







}
