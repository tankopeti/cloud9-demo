using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Cloud9_2.Models
{
    public class PartnerSiteLink
    {
        [Key]
        public int PartnerSiteLinkId { get; set; }

        public int PartnerId { get; set; }
        public Partner Partner { get; set; }

        public int SiteId { get; set; }
        public Site Site { get; set; }

        public int PartnerTypeId { get; set; }
        public PartnerType PartnerType { get; set; }

        public bool IsPrimary { get; set; } = false;
        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        public string? Comment { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? CreatedById { get; set; }
        public ApplicationUser? CreatedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string? LastModifiedById { get; set; }
        public ApplicationUser? LastModifiedBy { get; set; }
    }

    public class PartnerSiteLinkDto
    {
        [Key]
        public int PartnerSiteLinkId { get; set; }

        public int PartnerId { get; set; }
        public int SiteId { get; set; }
        public int PartnerTypeId { get; set; }

        public string? SiteName { get; set; }
        public string? PartnerTypeName { get; set; }

        public bool IsPrimary { get; set; }
        public bool IsActive { get; set; }

        public string? Comment { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class SitePartnersDto
    {
        public int SiteId { get; set; }
        public List<SitePartnerItemDto>? Partners { get; set; }
    }

    public class SitePartnerItemDto
    {
        public int PartnerId { get; set; }
        public int PartnerTypeId { get; set; }
    }
}