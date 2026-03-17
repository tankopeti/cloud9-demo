using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cloud9_2.Models
{
    public class EmailLog
    {
        [Key]
        public long Id { get; set; }

        public long? EmailQueueId { get; set; }

        [ForeignKey(nameof(EmailQueueId))]
        public EmailQueue? EmailQueue { get; set; }

        [Required]
        [MaxLength(50)]
        public string EventType { get; set; } = string.Empty;

        [MaxLength(255)]
        [EmailAddress]
        public string? ToEmail { get; set; }

        [MaxLength(255)]
        public string? Subject { get; set; }

        public DateTime EventDate { get; set; } = DateTime.UtcNow;

        [MaxLength(2000)]
        public string? Message { get; set; }

        [MaxLength(100)]
        public string? PerformedBy { get; set; }
    }


    // =========================
    // DTO-k
    // =========================

    public class EmailLogDto
    {
        public long Id { get; set; }

        public long? EmailQueueId { get; set; }

        public string EventType { get; set; } = string.Empty;

        public string? ToEmail { get; set; }

        public string? Subject { get; set; }

        public DateTime EventDate { get; set; }

        public string? Message { get; set; }
    }
}