using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cloud9_2.Models
{
    [Table("ItemTypes")]
    public class ItemTypeLookup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ItemTypeId { get; set; }

        [Required, MaxLength(50)]
        public string Code { get; set; } = null!;   // PRODUCT, SERVICE, RAW_MATERIAL, FINISHED_GOOD

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;   // Termék, Szolgáltatás, Alapanyag...

        public bool IsActive { get; set; } = true;
    }

    // DTO (ha kell)
    public class ItemTypeDto
    {
        public int ItemTypeId { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
