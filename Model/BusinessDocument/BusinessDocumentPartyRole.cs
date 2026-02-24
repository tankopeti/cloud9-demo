using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cloud9_2.Models
{
    [Table("BusinessDocumentPartyRoles")]
    public class BusinessDocumentPartyRole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BusinessDocumentPartyRoleId { get; set; }

        [Required, MaxLength(50)]
        public string Code { get; set; } = null!; // pl. BUYER, SELLER, SHIP_TO, BILL_TO, PAYER

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!; // admin felületen szerkeszthető

        public bool IsActive { get; set; } = true;
    }

    // =========================================================
    // DTO-k
    // =========================================================

    /// <summary>
    /// Lista / lookup DTO
    /// </summary>
    public class BusinessDocumentPartyRoleDto
    {
        public int BusinessDocumentPartyRoleId { get; set; }

        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Create DTO – új szerepkör létrehozásához
    /// </summary>
    public class BusinessDocumentPartyRoleCreateDto
    {
        [Required]
        public int BusinessDocumentPartyRoleId { get; set; }

        [Required, MaxLength(50)]
        public string Code { get; set; } = null!;

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Update DTO – név / aktív státusz módosításához
    /// (Code-ot általában nem engedjük módosítani ERP-ben)
    /// </summary>
    public class BusinessDocumentPartyRoleUpdateDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        public bool IsActive { get; set; }
    }
}
