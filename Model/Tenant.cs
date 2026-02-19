using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Cloud9_2.Models
{
    [Table("Tenants")]
    public class Tenant
    {
        [Key]
        public int TenantId { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = null!;

        [MaxLength(50)]
        public string? Code { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
