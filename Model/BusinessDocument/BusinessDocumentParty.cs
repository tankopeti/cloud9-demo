using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cloud9_2.Models
{
    // =========================================================
    // 3) Partner szerepek a bizonylaton (Buyer/Seller/ShipTo/BillTo stb.)
    // =========================================================
    [Table("BusinessDocumentParties")]
    public class BusinessDocumentParty
    {
        [Key]
        public int BusinessDocumentPartyId { get; set; }

        [Required]
        public int TenantId { get; set; }

        [ForeignKey(nameof(TenantId))]
        public Tenant Tenant { get; set; } = null!;

        [Required]
        public int BusinessDocumentId { get; set; }

        [ForeignKey(nameof(BusinessDocumentId))]
        public BusinessDocument BusinessDocument { get; set; } = null!;

        [Required]
        public int BusinessDocumentPartyRoleId { get; set; }

        [ForeignKey(nameof(BusinessDocumentPartyRoleId))]
        public BusinessDocumentPartyRole BusinessDocumentPartyRole { get; set; } = null!;

        public int? PartnerId { get; set; }
        [ForeignKey(nameof(PartnerId))]
        public Partner? Partner { get; set; }

        public int? SiteId { get; set; }
        [ForeignKey(nameof(SiteId))]
        public Site? Site { get; set; }

        public int? ContactId { get; set; }
        [ForeignKey(nameof(ContactId))]
        public Contact? Contact { get; set; }

        [MaxLength(200)]
        public string? DisplayNameSnapshot { get; set; }

        public string? AddressSnapshot { get; set; }

        [MaxLength(50)]
        public string? TaxNumberSnapshot { get; set; }
    }

    // =========================================================
    // DTO-k
    // =========================================================

    /// <summary>
    /// Lista / detail DTO (gridhez vagy bizonylat részletezéshez)
    /// </summary>
    public class BusinessDocumentPartyDto
    {
        public int BusinessDocumentPartyId { get; set; }

        public int TenantId { get; set; }

        public int BusinessDocumentId { get; set; }

        public int BusinessDocumentPartyRoleId { get; set; }

        public int? PartnerId { get; set; }

        public int? SiteId { get; set; }

        public int? ContactId { get; set; }

        public string? DisplayNameSnapshot { get; set; }

        public string? AddressSnapshot { get; set; }

        public string? TaxNumberSnapshot { get; set; }
    }

    /// <summary>
    /// Create DTO – új partner szerep hozzáadása bizonylathoz
    /// </summary>
    public class BusinessDocumentPartyCreateDto
    {
        [Required]
        public int TenantId { get; set; }

        [Required]
        public int BusinessDocumentId { get; set; }

        [Required]
        public int BusinessDocumentPartyRoleId { get; set; }

        public int? PartnerId { get; set; }

        public int? SiteId { get; set; }

        public int? ContactId { get; set; }

        public string? DisplayNameSnapshot { get; set; }

        public string? AddressSnapshot { get; set; }

        public string? TaxNumberSnapshot { get; set; }
    }

    /// <summary>
    /// Update DTO – szerepkör / partner / snapshot módosításhoz
    /// </summary>
    public class BusinessDocumentPartyUpdateDto
    {
        [Required]
        public int BusinessDocumentPartyRoleId { get; set; }

        public int? PartnerId { get; set; }

        public int? SiteId { get; set; }

        public int? ContactId { get; set; }

        public string? DisplayNameSnapshot { get; set; }

        public string? AddressSnapshot { get; set; }

        public string? TaxNumberSnapshot { get; set; }
    }
}
