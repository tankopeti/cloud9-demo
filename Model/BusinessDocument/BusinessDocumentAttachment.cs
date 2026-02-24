using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cloud9_2.Models
{
    // =========================================================
    // 6) Csatolmányok: összekötés a meglévő Document (fájl) táblával
    // =========================================================
    [Table("BusinessDocumentAttachments")]
    public class BusinessDocumentAttachment
    {
        [Key]
        public int BusinessDocumentAttachmentId { get; set; }

        [Required]
        public int TenantId { get; set; }

        [ForeignKey(nameof(TenantId))]
        public Tenant Tenant { get; set; } = null!;

        [Required]
        public int BusinessDocumentId { get; set; }

        [ForeignKey(nameof(BusinessDocumentId))]
        public BusinessDocument BusinessDocument { get; set; } = null!;

        // Ez a TE meglévő Document táblád (fájl meta + hash + verzió, stb.)
        [Required]
        public int DocumentId { get; set; }

        [ForeignKey(nameof(DocumentId))]
        public Document Document { get; set; } = null!;

        // kategória lookup (nincs enum)
        public int? AttachmentCategoryId { get; set; }

        [ForeignKey(nameof(AttachmentCategoryId))]
        public AttachmentCategoryLookup? AttachmentCategory { get; set; }

        public bool IsPrimary { get; set; } = false;
    }

    // =========================================================
    // DTO-k (ugyanebben a fájlban)
    // =========================================================

    /// <summary>
    /// Lista / grid DTO (csak az azonosítók + pár flag).
    /// </summary>
    public class BusinessDocumentAttachmentDto
    {
        public int BusinessDocumentAttachmentId { get; set; }

        public int TenantId { get; set; }

        public int BusinessDocumentId { get; set; }

        public int DocumentId { get; set; }

        public int? AttachmentCategoryId { get; set; }

        public bool IsPrimary { get; set; }
    }

    /// <summary>
    /// Create DTO: új csatolmány kapcsolás a bizonylathoz.
    /// </summary>
    public class BusinessDocumentAttachmentCreateDto
    {
        [Required]
        public int TenantId { get; set; }

        [Required]
        public int BusinessDocumentId { get; set; }

        [Required]
        public int DocumentId { get; set; }

        public int? AttachmentCategoryId { get; set; }

        public bool IsPrimary { get; set; } = false;
    }

    /// <summary>
    /// Update DTO: tipikusan kategória/primary módosítás (DocumentId-t sokszor nem engedjük cserélni).
    /// </summary>
    public class BusinessDocumentAttachmentUpdateDto
    {
        public int? AttachmentCategoryId { get; set; }

        public bool IsPrimary { get; set; }
    }
}
