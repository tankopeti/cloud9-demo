using System;
using System.ComponentModel.DataAnnotations;

namespace Cloud9_2.Models
{
    public class EmailQueue
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string ToEmail { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? CcEmail { get; set; }

        [MaxLength(255)]
        public string? BccEmail { get; set; }

        [Required]
        [MaxLength(255)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Body { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        public int RetryCount { get; set; } = 0;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ScheduledAt { get; set; }

        public DateTime? SentDate { get; set; }

        public DateTime? LastAttemptDate { get; set; }

        [MaxLength(2000)]
        public string? ErrorMessage { get; set; }

        [MaxLength(100)]
        public string? TemplateName { get; set; }

        [MaxLength(100)]
        public string? RelatedEntityType { get; set; }

        public int? RelatedEntityId { get; set; }

        [MaxLength(100)]
        public string? CreatedBy { get; set; }
    }


    // =========================
    // DTO-k
    // =========================

    public class EmailQueueDto
    {
        public long Id { get; set; }

        public string ToEmail { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public DateTime? SentDate { get; set; }

        public int RetryCount { get; set; }
    }

    public class QueueEmailRequestDto
    {
        [Required]
        [EmailAddress]
        public string ToEmail { get; set; } = string.Empty;

        public string? CcEmail { get; set; }

        public string? BccEmail { get; set; }

        [Required]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Body { get; set; } = string.Empty;

        public string? TemplateName { get; set; }

        public string? RelatedEntityType { get; set; }

        public int? RelatedEntityId { get; set; }
    }

    public class QueueTemplateEmailRequestDto
    {
        public string TemplateName { get; set; } = string.Empty;
        public string ToEmail { get; set; } = string.Empty;
        public string? CcEmail { get; set; }
        public string? BccEmail { get; set; }
        public string? RelatedEntityType { get; set; }
        public int? RelatedEntityId { get; set; }
        public Dictionary<string, string> Placeholders { get; set; } = new();
    }
}