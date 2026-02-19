using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cloud9_2.Models
{
    // =========================================================
    // 1) Bizonylat törzs (Header)
    // =========================================================
    [Table("BusinessDocuments")]
    public class BusinessDocument
    {
        [Key]
        public int BusinessDocumentId { get; set; }

        // Multi-tenant
        [Required]
        public int TenantId { get; set; }

        [ForeignKey(nameof(TenantId))]
        public Tenant Tenant { get; set; } = null!;

        // Típus + státusz (lookup táblák)
        [Required]
        public int BusinessDocumentTypeId { get; set; }

        [ForeignKey(nameof(BusinessDocumentTypeId))]
        public BusinessDocumentType BusinessDocumentType { get; set; } = null!;

        [Required]
        public int BusinessDocumentStatusId { get; set; }

        [ForeignKey(nameof(BusinessDocumentStatusId))]
        public BusinessDocumentStatus BusinessDocumentStatus { get; set; } = null!;

        // Azonosítók / számozás
        [MaxLength(50)]
        public string? DocumentNo { get; set; } // tenant + type + series szerint egyedi

        public DateTime? IssueDate { get; set; }          // kelte
        public DateTime? FulfillmentDate { get; set; }    // teljesítés
        public DateTime? DueDate { get; set; }            // fizetési határidő

        // Pénzügy (egyszerű alapmezők)
        public int? CurrencyId { get; set; }              // ha van Currency táblád
        [Column(TypeName = "decimal(18,6)")]
        public decimal? ExchangeRate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? NetTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TaxTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? GrossTotal { get; set; }

        // Rövid szövegek (nem UI-fordítás, üzleti tartalom)
        [MaxLength(200)]
        public string? Subject { get; set; }

        public string? Notes { get; set; }

        // Audit + soft delete
        public DateTime? CreatedAt { get; set; }
        [MaxLength(450)]
        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
        [MaxLength(450)]
        public string? UpdatedBy { get; set; }

        public DateTime? DeletedAt { get; set; }
        [MaxLength(450)]
        public string? DeletedBy { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Navigációk
        public ICollection<BusinessDocumentLine> Lines { get; set; } = new List<BusinessDocumentLine>();
        public ICollection<BusinessDocumentParty> Parties { get; set; } = new List<BusinessDocumentParty>();
        public ICollection<BusinessDocumentRelation> FromRelations { get; set; } = new List<BusinessDocumentRelation>();
        public ICollection<BusinessDocumentRelation> ToRelations { get; set; } = new List<BusinessDocumentRelation>();
        public ICollection<BusinessDocumentAttachment> Attachments { get; set; } = new List<BusinessDocumentAttachment>();
        public ICollection<BusinessDocumentStatusHistory> StatusHistory { get; set; } = new List<BusinessDocumentStatusHistory>();
    }

    // =========================================================
    // DTO-k (ugyanebben a fájlban)
    // =========================================================

    /// <summary>
    /// Lista / grid / egyszerű részletek DTO (nincs navigáció, csak ID-k + alapmezők).
    /// </summary>
    public class BusinessDocumentDto
    {
        public int BusinessDocumentId { get; set; }

        public int TenantId { get; set; }

        public int BusinessDocumentTypeId { get; set; }
        public int BusinessDocumentStatusId { get; set; }

        public string? DocumentNo { get; set; }

        public DateTime? IssueDate { get; set; }
        public DateTime? FulfillmentDate { get; set; }
        public DateTime? DueDate { get; set; }

        public int? CurrencyId { get; set; }
        public decimal? ExchangeRate { get; set; }

        public decimal? NetTotal { get; set; }
        public decimal? TaxTotal { get; set; }
        public decimal? GrossTotal { get; set; }

        public string? Subject { get; set; }
        public string? Notes { get; set; }

        public bool IsDeleted { get; set; }
    }

    /// <summary>
    /// Create DTO: amit a kliens beküld új bizonylat létrehozásához.
    /// (Audit mezők és soft delete NINCSENEK benne.)
    /// </summary>
    public class BusinessDocumentCreateDto
    {
        [Required]
        public int TenantId { get; set; }

        [Required]
        public int BusinessDocumentTypeId { get; set; }

        [Required]
        public int BusinessDocumentStatusId { get; set; }

        [MaxLength(50)]
        public string? DocumentNo { get; set; }

        public DateTime? IssueDate { get; set; }
        public DateTime? FulfillmentDate { get; set; }
        public DateTime? DueDate { get; set; }

        public int? CurrencyId { get; set; }
        public decimal? ExchangeRate { get; set; }

        public decimal? NetTotal { get; set; }
        public decimal? TaxTotal { get; set; }
        public decimal? GrossTotal { get; set; }

        [MaxLength(200)]
        public string? Subject { get; set; }

        public string? Notes { get; set; }
    }

    /// <summary>
    /// Update DTO: szerkesztéshez.
    /// Tipikusan nem engedjük TenantId módosítását, és az audit mezőket is a backend kezeli.
    /// </summary>
    public class BusinessDocumentUpdateDto
    {
        [Required]
        public int BusinessDocumentTypeId { get; set; }

        [Required]
        public int BusinessDocumentStatusId { get; set; }

        [MaxLength(50)]
        public string? DocumentNo { get; set; }

        public DateTime? IssueDate { get; set; }
        public DateTime? FulfillmentDate { get; set; }
        public DateTime? DueDate { get; set; }

        public int? CurrencyId { get; set; }
        public decimal? ExchangeRate { get; set; }

        public decimal? NetTotal { get; set; }
        public decimal? TaxTotal { get; set; }
        public decimal? GrossTotal { get; set; }

        [MaxLength(200)]
        public string? Subject { get; set; }

        public string? Notes { get; set; }
    }
}
