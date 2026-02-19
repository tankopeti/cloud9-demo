using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cloud9_2.Models
{
    // =========================================================
    // 2) Bizonylat tételek (Lines)
    // =========================================================
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

        public int? ItemId { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal Quantity { get; set; } = 1;

        public int? UnitId { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? UnitPrice { get; set; }

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

    // =========================================================
    // DTO-k (ugyanebben a fájlban)
    // =========================================================

    /// <summary>
    /// Lista / detail DTO (gridhez vagy bizonylat részletezéshez).
    /// </summary>
    public class BusinessDocumentLineDto
    {
        public int BusinessDocumentLineId { get; set; }

        public int TenantId { get; set; }

        public int BusinessDocumentId { get; set; }

        public int LineNo { get; set; }

        public int? ItemId { get; set; }

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

    /// <summary>
    /// Create DTO – új tétel hozzáadásához.
    /// TenantId és BusinessDocumentId kötelező.
    /// </summary>
    public class BusinessDocumentLineCreateDto
    {
        [Required]
        public int TenantId { get; set; }

        [Required]
        public int BusinessDocumentId { get; set; }

        public int LineNo { get; set; } = 1;

        public int? ItemId { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public decimal Quantity { get; set; } = 1;

        public int? UnitId { get; set; }

        public decimal? UnitPrice { get; set; }

        public decimal? DiscountAmount { get; set; }

        public int? TaxCodeId { get; set; }

        public int? WarehouseId { get; set; }
    }

    /// <summary>
    /// Update DTO – tétel szerkesztéséhez.
    /// Általában nem engedjük a TenantId és BusinessDocumentId módosítását.
    /// </summary>
    public class BusinessDocumentLineUpdateDto
    {
        public int LineNo { get; set; }

        public int? ItemId { get; set; }

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
