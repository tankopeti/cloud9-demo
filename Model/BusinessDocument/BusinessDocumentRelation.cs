using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cloud9_2.Models
{
    // =========================================================
    // 5) Bizonylatok közti származtatás
    // =========================================================
    [Table("BusinessDocumentRelations")]
    public class BusinessDocumentRelation
    {
        [Key]
        public int BusinessDocumentRelationId { get; set; }

        [Required]
        public int TenantId { get; set; }

        [ForeignKey(nameof(TenantId))]
        public Tenant Tenant { get; set; } = null!;

        [Required]
        public int FromBusinessDocumentId { get; set; }

        [ForeignKey(nameof(FromBusinessDocumentId))]
        public BusinessDocument FromBusinessDocument { get; set; } = null!;

        [Required]
        public int ToBusinessDocumentId { get; set; }

        [ForeignKey(nameof(ToBusinessDocumentId))]
        public BusinessDocument ToBusinessDocument { get; set; } = null!;

        [Required]
        public int BusinessDocumentRelationTypeId { get; set; }

        [ForeignKey(nameof(BusinessDocumentRelationTypeId))]
        public BusinessDocumentRelationType BusinessDocumentRelationType { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(450)]
        public string? CreatedBy { get; set; }
    }

    // =========================================================
    // DTO-k
    // =========================================================

    /// <summary>
    /// Lista / detail DTO – dokumentum kapcsolatok lekérdezéséhez
    /// </summary>
    public class BusinessDocumentRelationDto
    {
        public int BusinessDocumentRelationId { get; set; }

        public int TenantId { get; set; }

        public int FromBusinessDocumentId { get; set; }

        public int ToBusinessDocumentId { get; set; }

        public int BusinessDocumentRelationTypeId { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? CreatedBy { get; set; }
    }

    /// <summary>
    /// Create DTO – új dokumentum kapcsolat létrehozásához
    /// </summary>
    public class BusinessDocumentRelationCreateDto
    {
        [Required]
        public int TenantId { get; set; }

        [Required]
        public int FromBusinessDocumentId { get; set; }

        [Required]
        public int ToBusinessDocumentId { get; set; }

        [Required]
        public int BusinessDocumentRelationTypeId { get; set; }
    }
}
