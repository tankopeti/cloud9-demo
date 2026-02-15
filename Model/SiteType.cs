using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Cloud9_2.Models
{
    [Table("SiteTypes")]
    public class SiteType
    {
        [Key]
        public int SiteTypeId { get; set; }

        [Required]
        [StringLength(255)]
        public string? Name { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation property – ha egy Site-hoz több SiteType kapcsolódik
        public ICollection<Site>? Sites { get; set; }
    }
}