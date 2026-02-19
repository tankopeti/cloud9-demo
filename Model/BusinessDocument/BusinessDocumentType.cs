using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cloud9_2.Models
{
    // =========================================================
    // 4) Bizonylat típusok (lookup tábla)
    // =========================================================
    [Table("BusinessDocumentTypes")]
    public class BusinessDocumentType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BusinessDocumentTypeId { get; set; }

        [Required, MaxLength(50)]
        public string Code { get; set; } = null!; // QUOTE, ORDER, DELIVERY_NOTE, ACCEPTANCE, INVOICE, CONTRACT

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        public bool IsActive { get; set; } = true;
    }

    // =========================================================
    // DTO-k
    // =========================================================

    /// <summary>
    /// Lookup / lista DTO
    /// </summary>
    public class BusinessDocumentTypeDto
    {
        public int BusinessDocumentTypeId { get; set; }

        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Create DTO – új bizonylat típus létrehozásához
    /// </summary>
    public class BusinessDocumentTypeCreateDto
    {
        [Required]
        public int BusinessDocumentTypeId { get; set; }

        [Required, MaxLength(50)]
        public string Code { get; set; } = null!;

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Update DTO – név / aktív státusz módosításához
    /// (Code ERP-ben általában nem módosítható)
    /// </summary>
    public class BusinessDocumentTypeUpdateDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        public bool IsActive { get; set; }
    }
}
