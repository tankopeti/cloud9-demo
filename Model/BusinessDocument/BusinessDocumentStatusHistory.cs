using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cloud9_2.Models
{
    // =========================================================
    // 7) Bizonylat státusz history
    // =========================================================
    [Table("BusinessDocumentStatusHistories")]
    public class BusinessDocumentStatusHistory
    {
        [Key]
        public int BusinessDocumentStatusHistoryId { get; set; }

        [Required]
        public int TenantId { get; set; }

        [ForeignKey(nameof(TenantId))]
        public Tenant Tenant { get; set; } = null!;

        [Required]
        public int BusinessDocumentId { get; set; }

        [ForeignKey(nameof(BusinessDocumentId))]
        public BusinessDocument BusinessDocument { get; set; } = null!;

        [Required]
        public int OldBusinessDocumentStatusId { get; set; }

        [Required]
        public int NewBusinessDocumentStatusId { get; set; }

        [ForeignKey(nameof(OldBusinessDocumentStatusId))]
        public BusinessDocumentStatus OldStatus { get; set; } = null!;

        [ForeignKey(nameof(NewBusinessDocumentStatusId))]
        public BusinessDocumentStatus NewStatus { get; set; } = null!;

        public DateTime ChangeDate { get; set; } = DateTime.UtcNow;

        [MaxLength(450)]
        public string? ChangedBy { get; set; }
    }

    // =========================================================
    // DTO-k
    // =========================================================

    /// <summary>
    /// Lista / audit DTO – státusz változások megjelenítéséhez
    /// </summary>
    public class BusinessDocumentStatusHistoryDto
    {
        public int BusinessDocumentStatusHistoryId { get; set; }

        public int TenantId { get; set; }

        public int BusinessDocumentId { get; set; }

        public int OldBusinessDocumentStatusId { get; set; }

        public int NewBusinessDocumentStatusId { get; set; }

        public DateTime ChangeDate { get; set; }

        public string? ChangedBy { get; set; }
    }

    /// <summary>
    /// Create DTO – új státuszváltás rögzítéséhez
    /// (ChangeDate és ChangedBy backend oldalon töltődik)
    /// </summary>
    public class BusinessDocumentStatusHistoryCreateDto
    {
        [Required]
        public int TenantId { get; set; }

        [Required]
        public int BusinessDocumentId { get; set; }

        [Required]
        public int OldBusinessDocumentStatusId { get; set; }

        [Required]
        public int NewBusinessDocumentStatusId { get; set; }
    }
}
