using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Cloud9_2.Models
{
    public class Document
    {
        [Key]
        public int DocumentId { get; set; }

        // Fájl meta
        [Required]
        [MaxLength(450)]
        public string FileName { get; set; } = null!;             // nálad kötelező a DB-ben

        public string? FilePath { get; set; }                     // legacy / opcionális

        [MaxLength(450)]
        public string? OriginalFileName { get; set; }

        [MaxLength(450)]
        public string? StoredFileName { get; set; }

        [MaxLength(20)]
        public string? FileExtension { get; set; }                // ".pdf"

        [MaxLength(255)]
        public string? ContentType { get; set; }                  // "application/pdf"

        public long? FileSizeBytes { get; set; }

        [MaxLength(50)]
        public string? StorageProvider { get; set; }              // FileSystem / AzureBlob / S3

        [MaxLength(1024)]
        public string? StorageKey { get; set; }                   // blob key / relatív út

        // Üzleti leíró mezők
        public string? DocumentName { get; set; }
        public string? DocumentDescription { get; set; }

        // Kapcsolatok
        public int? DocumentTypeId { get; set; }
        public DocumentType? DocumentType { get; set; }

        public int? SiteId { get; set; }
        public Site? Site { get; set; }

        public int? PartnerId { get; set; }
        [ForeignKey(nameof(PartnerId))]
        public Partner? Partner { get; set; }

        public int? ContactId { get; set; }
        public Contact? Contact { get; set; }

        // Legacy / egyéb
        public int? employee_id { get; set; }

        public DateTime? UploadDate { get; set; }
        public string? UploadedBy { get; set; }

        public bool? isActive { get; set; }                       // legacy (később kiváltható IsDeleted-del)

        // Dokumentum státusz (nálad enum)
        public DocumentStatusEnum Status { get; set; }            // DB-ben int (Status)
        public int? StatusId { get; set; }                        // PartnerStatuses FK (legacy/üzleti okból)
        public int? DocumentStatusId { get; set; }                // külön táblád van rá
        public DocumentStatus? DocumentStatus { get; set; }

        // Integritás (hash)
        [MaxLength(20)]
        public string? HashAlgorithm { get; set; }                // "SHA256"

        [Column(TypeName = "char(64)")]
        public string? FileHash { get; set; }                     // SHA256 hex

        // Audit + soft delete
        public DateTime? CreatedAt { get; set; }

        [MaxLength(450)]
        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [MaxLength(450)]
        public string? UpdatedBy { get; set; }

        public DateTime? DeletedAt { get; set; }

        [MaxLength(450)]
        public string? DeletedBy { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Verziózás
        public int? VersionNumber { get; set; }
        public bool? IsLatestVersion { get; set; }

        public int? ParentDocumentId { get; set; }
        public Document? ParentDocument { get; set; }

        // Navigációk
        public ICollection<DocumentMetadata> DocumentMetadata { get; set; } = new List<DocumentMetadata>();
        public ICollection<DocumentLink> DocumentLinks { get; set; } = new List<DocumentLink>();
        public ICollection<DocumentStatusHistory> StatusHistory { get; set; } = new List<DocumentStatusHistory>();
        public virtual ICollection<TaskDocumentLink> TaskDocuments { get; set; } = new List<TaskDocumentLink>();
    }

    public class DocumentDto
    {
        [Key]
        public int DocumentId { get; set; }

        // Fájl meta
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? OriginalFileName { get; set; }
        public string? StoredFileName { get; set; }
        public string? FileExtension { get; set; }
        public string? ContentType { get; set; }
        public long? FileSizeBytes { get; set; }
        public string? StorageProvider { get; set; }
        public string? StorageKey { get; set; }

        // Üzleti leíró mezők
        public string? DocumentName { get; set; }
        public string? DocumentDescription { get; set; }

        // Kapcsolatok
        public int? DocumentTypeId { get; set; }
        public string? DocumentTypeName { get; set; }

        public DateTime? UploadDate { get; set; }
        public string? UploadedBy { get; set; }

        public int? SiteId { get; set; }

        public int? PartnerId { get; set; }
        public string? PartnerName { get; set; }

        public int? ContactId { get; set; }

        // Státuszok
        public DocumentStatusEnum Status { get; set; }
        public int? DocumentStatusId { get; set; }
        public int? StatusId { get; set; }

        public bool? isActive { get; set; }
        public bool IsDeleted { get; set; }

        // Hash
        public string? HashAlgorithm { get; set; }
        public string? FileHash { get; set; }

        // Audit
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        // Verziózás
        public int? VersionNumber { get; set; }
        public bool? IsLatestVersion { get; set; }
        public int? ParentDocumentId { get; set; }

        public List<DocumentLinkDto>? DocumentLinks { get; set; }
        public List<DocumentStatusHistoryDto>? StatusHistory { get; set; } = new List<DocumentStatusHistoryDto>();
        public static IDictionary<string, string> StatusDisplayNames { get; } = GetStatusDisplayNames();

        public List<MetadataEntry> CustomMetadata { get; set; } = new();

        private static IDictionary<string, string> GetStatusDisplayNames()
        {
            return Enum.GetValues(typeof(DocumentStatusEnum))
                .Cast<DocumentStatusEnum>()
                .ToDictionary(
                    e => e.ToString(),
                    e => e switch
                    {
                        DocumentStatusEnum.Beérkezett => "Beérkezett",
                        DocumentStatusEnum.Függőben => "Függőben",
                        DocumentStatusEnum.Elfogadott => "Elfogadott",
                        DocumentStatusEnum.Lezárt => "Lezárt",
                        DocumentStatusEnum.Jóváhagyandó => "Jóváhagyandó",
                        _ => e.ToString()
                    });
        }
    }

    public class DocumentStatusHistoryDto
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public DocumentStatusEnum OldStatus { get; set; }
        public DocumentStatusEnum NewStatus { get; set; }
        public DateTime ChangeDate { get; set; }
        public string? ChangedBy { get; set; }
    }

    public class DocumentLinkDto
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public string ModuleId { get; set; } = "";
        public int RecordId { get; set; }
    }

    public class MetadataEntry
    {
        [Key]
        public string Key { get; set; } = "";
        public string? Value { get; set; }
    }

    public class CreateDocumentDto
    {
        [Required]
        public string FileName { get; set; } = null!;

        [Required]
        public string FilePath { get; set; } = null!;

        public int? DocumentTypeId { get; set; }
        public int? SiteId { get; set; }
        public int? PartnerId { get; set; }
        public int? ContactId { get; set; }

        // Új mezők (feltöltéskor gyakran elérhető)
        public string? OriginalFileName { get; set; }
        public string? StoredFileName { get; set; }
        public string? FileExtension { get; set; }
        public string? ContentType { get; set; }
        public long? FileSizeBytes { get; set; }
        public string? StorageProvider { get; set; }
        public string? StorageKey { get; set; }

        public string? DocumentName { get; set; }
        public string? DocumentDescription { get; set; }

        // Hash-t általában szerver számolja, de DTO-ban lehet opcionális mező
        public string? HashAlgorithm { get; set; }
        public string? FileHash { get; set; }

        [Required]
        public DocumentStatusEnum Status { get; set; }

        public List<MetadataEntry>? CustomMetadata { get; set; } = new();
    }

    public class DocumentModalViewModel
    {
        public DocumentDto? Document { get; set; }
        public CreateDocumentDto? CreateDocument { get; set; }
        public List<SelectListItem>? DocumentTypes { get; set; }
        public List<SelectListItem>? Partners { get; set; }
        public List<SelectListItem>? Sites { get; set; }
        public string? NextDocumentNumber { get; set; }
    }

    public class DocumentListItemDto
    {
        public int DocumentId { get; set; }
        public string? FileName { get; set; }
        public string? DocumentName { get; set; }
        public string? DocumentTypeName { get; set; }
        public DateTime? UploadDate { get; set; }
        public string? UploadedBy { get; set; }
        public string? PartnerName { get; set; }
        public int? PartnerId { get; set; }
        public int? SiteId { get; set; }
        public DocumentStatusEnum Status { get; set; }
        public bool IsDeleted { get; set; }
    }
}
