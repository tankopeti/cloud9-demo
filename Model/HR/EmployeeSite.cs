using System;
using System.ComponentModel.DataAnnotations;

namespace Cloud9_2.Models
{
    public class EmployeeSite
    {
        public int EmployeeId { get; set; }
        public Employees Employee { get; set; } = null!;

        public int SiteId { get; set; }
        public Site Site { get; set; } = null!;

        // opcionális, de hasznos:
        public bool IsPrimary { get; set; } = false; // dolgozó "alap" telephelye
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

        public class SiteEmployeesDto
    {
        public List<int> EmployeeIds { get; set; } = new();
    }
}