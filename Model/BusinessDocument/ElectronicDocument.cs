using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cloud9_2.Models
{
    [Table("ElectronicDocument")]
    public class ElectronicDocument
    {
        [Key]
        public int ElectronicDocumentId { get; set; }

        public int? ParentId { get; set; }
        public int? CreatorUserId { get; set; }

        public DateTime? CreationDate { get; set; }

        public string? Name { get; set; }
        public string? Text { get; set; }

        public int? VoteMaxValue { get; set; }
        public int? VoteSplitValue { get; set; }

        public bool? isVoteHalfValueEnabled { get; set; }
    }

    // =========================================================
    // DTO-k
    // =========================================================

    /// <summary>
    /// Lista / detail DTO
    /// </summary>
    public class ElectronicDocumentDto
    {
        public int ElectronicDocumentId { get; set; }

        public int? ParentId { get; set; }
        public int? CreatorUserId { get; set; }

        public DateTime? CreationDate { get; set; }

        public string? Name { get; set; }
        public string? Text { get; set; }

        public int? VoteMaxValue { get; set; }
        public int? VoteSplitValue { get; set; }

        public bool? IsVoteHalfValueEnabled { get; set; }
    }

    /// <summary>
    /// Create DTO
    /// </summary>
    public class ElectronicDocumentCreateDto
    {
        public int? ParentId { get; set; }

        public int? CreatorUserId { get; set; }

        public string? Name { get; set; }

        public string? Text { get; set; }

        public int? VoteMaxValue { get; set; }

        public int? VoteSplitValue { get; set; }

        public bool? IsVoteHalfValueEnabled { get; set; }
    }

    /// <summary>
    /// Update DTO
    /// </summary>
    public class ElectronicDocumentUpdateDto
    {
        public string? Name { get; set; }

        public string? Text { get; set; }

        public int? VoteMaxValue { get; set; }

        public int? VoteSplitValue { get; set; }

        public bool? IsVoteHalfValueEnabled { get; set; }
    }
}
