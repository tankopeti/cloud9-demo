using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cloud9_2.Models
{
    public class EmailTemplateVariable
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmailTemplateId { get; set; }

        [ForeignKey(nameof(EmailTemplateId))]
        public EmailTemplate EmailTemplate { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string VariableName { get; set; } = string.Empty; // pl. CustomerName

        [StringLength(250)]
        public string? Description { get; set; } // pl. ügyfél neve

        [StringLength(250)]
        public string? ExampleValue { get; set; } // pl. Kovács Péter

        public int SortOrder { get; set; }
    }
    public class EmailTemplateVariableDto
    {
        public int Id { get; set; }
        public int EmailTemplateId { get; set; }
        public string VariableName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ExampleValue { get; set; }
        public int SortOrder { get; set; }
    }

        public class EmailTemplateWithVariablesDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? Description { get; set; }

        public List<EmailTemplateVariableDto> Variables { get; set; } = new();
    }
    
}