using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cloud9_2.Models
{
    [Table("AttachmentCategories")]
    public class AttachmentCategoryLookup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AttachmentCategoryId { get; set; }

        [Required, MaxLength(50)]
        public string Code { get; set; } = null!; // SIGNED_CONTRACT, DELIVERY_PROOF, INVOICE_PDF...

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;
    }

    // -------------------- DTO --------------------

    public class AttachmentCategoryDto
    {
        public int AttachmentCategoryId { get; set; }

        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;
    }
}
