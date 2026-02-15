using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cloud9_2.Models
{
    [Table("DocumentStatuses")]
    public class DocumentStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // mert nem IDENTITY
        public int DocumentStatusId { get; set; }

        [MaxLength(100)]
        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? StatusColor { get; set; }   // pl. "#28a745" vagy "success"

        // Navigáció: egy státuszhoz több dokumentum tartozhat
        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}
