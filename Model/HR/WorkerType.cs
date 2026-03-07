using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cloud9_2.Models
{
    public class WorkerType
    {
        [Key]
        public int WorkerTypeId { get; set; }

        [Required, StringLength(50)]
        public string Code { get; set; } = null!;   // INTERNAL / EXTERNAL

        [Required, StringLength(100)]
        public string Name { get; set; } = null!;   // Belsős / Külsős

        public bool IsActive { get; set; } = true;

        public ICollection<Employees> Employees { get; set; } = new List<Employees>();
    }
}