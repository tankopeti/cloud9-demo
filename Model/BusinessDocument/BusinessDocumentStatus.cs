using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cloud9_2.Models
{
    [Table("BusinessDocumentStatuses")]
    public class BusinessDocumentStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BusinessDocumentStatusId { get; set; }

        [Required, MaxLength(50)]
        public string Code { get; set; } = null!; // DRAFT, APPROVED, SENT, FULFILLED, INVOICED, CANCELLED...

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(20)]
        public string? Color { get; set; } // ha kell UI-hoz

        public bool IsActive { get; set; } = true;
    }

    // =========================================================
    // DTO-k
    // =========================================================

    /// <summary>
    /// Lookup / lista DTO
    /// </summary>
    public class BusinessDocumentStatusDto
    {
        public int BusinessDocumentStatusId { get; set; }

        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Color { get; set; }

        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Create DTO – új státusz létrehozásához
    /// </summary>
    public class BusinessDocumentStatusCreateDto
    {
        [Required]
        public int BusinessDocumentStatusId { get; set; }

        [Required, MaxLength(50)]
        public string Code { get; set; } = null!;

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(20)]
        public string? Color { get; set; }

        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Update DTO – név / szín / aktív státusz módosításához
    /// (Code ERP-ben tipikusan nem módosítható)
    /// </summary>
    public class BusinessDocumentStatusUpdateDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(20)]
        public string? Color { get; set; }

        public bool IsActive { get; set; }
    }
}
