using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cloud9_2.Models
{
    public class Employees
    {
        [Key]
        public int EmployeeId { get; set; }

        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? Email2 { get; set; }

        [StringLength(15)]
        public string? PhoneNumber { get; set; }

        [StringLength(15)]
        public string? PhoneNumber2 { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        public DateTime? HireDate { get; set; }

        public int? DepartmentId { get; set; }

        public int? JobTitleId { get; set; }

        public bool IsActive { get; set; } = true;

        public int? DefaultSiteId { get; set; }

        [Range(0, 99.99)]
        public decimal? WorkingTime { get; set; } = 8.00m;

        [Range(0, 1)]
        public byte? IsContracted { get; set; } = 0; // 0 = false, 1 = true

        public string? FamilyData { get; set; }

        public string? Comment1 { get; set; }

        public string? Comment2 { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
        public JobTitle? JobTitle { get; set; }
        public int? VacationDays { get; set; }
        public int? FullVacationDays { get; set; }

        // Azonosítók
        [StringLength(10)]
        public string? TaxId { get; set; }          // adóazonosító jel (10)

        [StringLength(9)]
        public string? TajNumber { get; set; }      // TAJ (9)

        [StringLength(100)]
        public string? BirthName { get; set; }      // születési név

        [StringLength(100)]
        public string? MotherBirthName { get; set; } // anyja születési neve

        [StringLength(100)]
        public string? BirthPlace { get; set; }     // születési hely

        [StringLength(2)]
        public string? NationalityCode { get; set; } // pl. "HU"

        // Foglalkoztatás
        [StringLength(10)]
        public string? FeorCode { get; set; }       // FEOR kód

        public DateTime? EmploymentEndDate { get; set; }

        // Címek (ha bontani akarod)
        [StringLength(250)]
        public string? PermanentAddress { get; set; }

        [StringLength(250)]
        public string? MailingAddress { get; set; }

        // Bérfizetés (csak ha kell)
        [StringLength(34)]
        public string? BankAccountIban { get; set; }
        public int WorkerTypeId { get; set; } = 1;          // default: belsős
        public WorkerType WorkerType { get; set; } = null!;

        public int? PartnerId { get; set; }                // csak külsősöknél
        public Partner? Partner { get; set; }              // ha van Partner modelled
        public ICollection<EmployeeSite> EmployeeSites { get; set; } = new List<EmployeeSite>();
        public ICollection<EmployeeEmploymentStatus> EmployeeEmploymentStatuses { get; set; } = new List<EmployeeEmploymentStatus>();

    }

    public class EmployeesCreateDto
    {
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string? FirstName { get; set; }

        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string? LastName { get; set; }

        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }

        [StringLength(100, ErrorMessage = "Secondary email cannot exceed 100 characters.")]
        [EmailAddress(ErrorMessage = "Invalid secondary email address.")]
        public string? Email2 { get; set; }

        [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters.")]
        public string? PhoneNumber { get; set; }

        [StringLength(15, ErrorMessage = "Secondary phone number cannot exceed 15 characters.")]
        public string? PhoneNumber2 { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string? Address { get; set; }

        public DateTime? HireDate { get; set; }

        public int? DepartmentId { get; set; }

        public int? JobTitleId { get; set; }

        public bool IsActive { get; set; } = true;

        public int? DefaultSiteId { get; set; }

        [Range(0, 99.99, ErrorMessage = "Working time must be between 0 and 99.99 hours.")]
        public decimal? WorkingTime { get; set; } = 8.00m;

        [Range(0, 1, ErrorMessage = "IsContracted must be 0 (false) or 1 (true).")]
        public byte? IsContracted { get; set; } = 0;

        public string? FamilyData { get; set; }

        public string? Comment1 { get; set; }

        // Azonosítók
        [StringLength(10)]
        public string? TaxId { get; set; }          // adóazonosító jel (10)

        [StringLength(9)]
        public string? TajNumber { get; set; }      // TAJ (9)

        [StringLength(100)]
        public string? BirthName { get; set; }      // születési név

        [StringLength(100)]
        public string? MotherBirthName { get; set; } // anyja születési neve

        [StringLength(100)]
        public string? BirthPlace { get; set; }     // születési hely

        [StringLength(2)]
        public string? NationalityCode { get; set; } // pl. "HU"

        // Foglalkoztatás
        [StringLength(10)]
        public string? FeorCode { get; set; }       // FEOR kód

        public DateTime? EmploymentEndDate { get; set; }

        // Címek (ha bontani akarod)
        [StringLength(250)]
        public string? PermanentAddress { get; set; }

        [StringLength(250)]
        public string? MailingAddress { get; set; }

        // Bérfizetés (csak ha kell)
        [StringLength(34)]
        public string? BankAccountIban { get; set; }

        public List<int> StatusIds { get; set; } = new();

        public string? Comment2 { get; set; }
        public int? VacationDays { get; set; }
        public int? FullVacationDays { get; set; }
        public int WorkerTypeId { get; set; } = 1;  // 1=INTERNAL, 2=EXTERNAL (seed alapján)
        public int? PartnerId { get; set; }         // külsősöknél kötelező
        public List<int> SiteIds { get; set; } = new();
    }

    public class EmployeesUpdateDto
    {
        [Required(ErrorMessage = "Employee ID is required.")]
        public int EmployeeId { get; set; }

        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string? FirstName { get; set; }

        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string? LastName { get; set; }

        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }

        [StringLength(100, ErrorMessage = "Secondary email cannot exceed 100 characters.")]
        [EmailAddress(ErrorMessage = "Invalid secondary email address.")]
        public string? Email2 { get; set; }

        [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters.")]
        public string? PhoneNumber { get; set; }

        [StringLength(15, ErrorMessage = "Secondary phone number cannot exceed 15 characters.")]
        public string? PhoneNumber2 { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string? Address { get; set; }

        public DateTime? HireDate { get; set; }

        public int? DepartmentId { get; set; }

        public int? JobTitleId { get; set; }

        public int? DefaultSiteId { get; set; }

        [Range(0, 99.99, ErrorMessage = "Working time must be between 0 and 99.99 hours.")]
        public decimal? WorkingTime { get; set; }

        [Range(0, 1, ErrorMessage = "IsContracted must be 0 (false) or 1 (true).")]
        public byte? IsContracted { get; set; }

        public string? FamilyData { get; set; }

        public string? Comment1 { get; set; }

        public string? Comment2 { get; set; }

        // Azonosítók
        [StringLength(10)]
        public string? TaxId { get; set; }          // adóazonosító jel (10)

        [StringLength(9)]
        public string? TajNumber { get; set; }      // TAJ (9)

        [StringLength(100)]
        public string? BirthName { get; set; }      // születési név

        [StringLength(100)]
        public string? MotherBirthName { get; set; } // anyja születési neve

        [StringLength(100)]
        public string? BirthPlace { get; set; }     // születési hely

        [StringLength(2)]
        public string? NationalityCode { get; set; } // pl. "HU"

        // Foglalkoztatás
        [StringLength(10)]
        public string? FeorCode { get; set; }       // FEOR kód

        public DateTime? EmploymentEndDate { get; set; }

        // Címek (ha bontani akarod)
        [StringLength(250)]
        public string? PermanentAddress { get; set; }

        [StringLength(250)]
        public string? MailingAddress { get; set; }

        // Bérfizetés (csak ha kell)
        [StringLength(34)]
        public string? BankAccountIban { get; set; }
        public List<int> StatusIds { get; set; } = new();
        public int? VacationDays { get; set; }
        public int? FullVacationDays { get; set; }
        public int WorkerTypeId { get; set; } = 1;
        public int? PartnerId { get; set; }
        public List<int> SiteIds { get; set; } = new();
        public bool IsActive { get; set; } = true;
    }

    public class EmployeeSitesDto
    {
        public List<int> SiteIds { get; set; } = new();
    }

}