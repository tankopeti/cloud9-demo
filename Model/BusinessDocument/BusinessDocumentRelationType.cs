using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cloud9_2.Models
{
    [Table("BusinessDocumentRelationTypes")]
    public class BusinessDocumentRelationType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BusinessDocumentRelationTypeId { get; set; }

        [Required, MaxLength(50)]
        public string Code { get; set; } = null!; // CONVERTED_FROM, DERIVED_FROM, BASED_ON, REPLACES...

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
    public class BusinessDocumentRelationTypeDto
    {
        public int BusinessDocumentRelationTypeId { get; set; }

        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Create DTO – új relation type létrehozásához
    /// </summary>
    public class BusinessDocumentRelationTypeCreateDto
    {
        [Required]
        public int BusinessDocumentRelationTypeId { get; set; }

        [Required, MaxLength(50)]
        public string Code { get; set; } = null!;

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Update DTO – név / aktív státusz módosításhoz
    /// (Code módosítása ERP-ben általában tiltott)
    /// </summary>
    public class BusinessDocumentRelationTypeUpdateDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        public bool IsActive { get; set; }
    }
}
