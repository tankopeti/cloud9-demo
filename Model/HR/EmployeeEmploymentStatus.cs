using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cloud9_2.Models
{
    public class EmployeeEmploymentStatus
    {
        public int EmployeeId { get; set; }
        public Employees Employee { get; set; } = null!;

        public int StatusId { get; set; }
        public EmploymentStatus Status { get; set; } = null!;

        // opcionális, de nagyon hasznos mezők:
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public bool IsCurrent { get; set; } = true;
    }
}