using System;
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

        [MaxLength(100)]
        public string? TaxNumber { get; set; }

        [MaxLength(100)]
        public string? EuTaxNumber { get; set; }

        [MaxLength(100)]
        public string? CompanyRegistrationNumber { get; set; }

        [MaxLength(100)]
        public string? BankAccountNumber { get; set; }

        public int? CurrencyId { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(50)]
        public string? PostalCode { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(250)]
        public string? AddressLine1 { get; set; }

        [MaxLength(250)]
        public string? AddressLine2 { get; set; }

        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(50)]
        public string? Phone { get; set; }

        [MaxLength(200)]
        public string? Website { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? CreatedAt { get; set; }

        [MaxLength(450)]
        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [MaxLength(450)]
        public string? UpdatedBy { get; set; }

        [ForeignKey(nameof(CurrencyId))]
        public Currency? Currency { get; set; }
    }
}