using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloud9_2.Data;
using Cloud9_2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cloud9_2.Services
{
    public class DocumentService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DocumentService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public DocumentService(
            ApplicationDbContext context,
            ILogger<DocumentService> logger,
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        private string GetCurrentUser() =>
            _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

        private async Task<bool> IsAdminAsync()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext!.User);
            if (user == null) return false;

            return await _userManager.IsInRoleAsync(user, "Admin")
                   || await _userManager.IsInRoleAsync(user, "SuperAdmin");
        }

        private async Task<bool> IsEditorAsync()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext!.User);
            if (user == null) return false;

            return await _userManager.IsInRoleAsync(user, "Editor");
        }

        private async Task LogStatusChangeAsync(int documentId, DocumentStatusEnum oldStatus, DocumentStatusEnum newStatus, bool force = false)
        {
            if (!force && oldStatus == newStatus) return;

            var history = new DocumentStatusHistory
            {
                DocumentId = documentId,
                OldStatus = oldStatus,
                NewStatus = newStatus,
                ChangeDate = DateTime.UtcNow,
                ChangedBy = GetCurrentUser()
            };

            _context.DocumentStatusHistory.Add(history);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Logged status change for document {DocumentId} from {OldStatus} to {NewStatus}",
                documentId, oldStatus, newStatus);
        }

        // ----------------------------
        // SEARCH (külön endpointhez)
        // ----------------------------
        public async Task<List<DocumentSearchResultDto>> SearchDocumentsAsync(string term, int take = 20)
        {
            term = (term ?? "").Trim();
            if (term.Length < 2) return new List<DocumentSearchResultDto>();

            take = Math.Clamp(take, 1, 50);

            // 1) ID-k dokumentum mezők + partner név alapján (LEFT JOIN)
            var docOrPartnerIds = await (
                from d in _context.Documents.AsNoTracking()
                join p in _context.Partners.AsNoTracking() on d.PartnerId equals p.PartnerId into pj
                from p in pj.DefaultIfEmpty()
                where d.IsDeleted == false
                      && (
                          (d.FileName != null && d.FileName.Contains(term)) ||
                          (d.FilePath != null && d.FilePath.Contains(term)) ||
                          (d.DocumentName != null && d.DocumentName.Contains(term)) ||
                          (d.DocumentDescription != null && d.DocumentDescription.Contains(term)) ||
                          (p != null && p.Name != null && p.Name.Contains(term))
                      )
                select d.DocumentId
            )
            .Distinct()
            .Take(1000)
            .ToListAsync();

            // 2) ID-k metaadatok alapján
            var metaIds = await _context.DocumentMetadata
                .AsNoTracking()
                .Where(m => m.Value != null && m.Value.Contains(term))
                .Select(m => m.DocumentId)
                .Distinct()
                .Take(2000)
                .ToListAsync();

            // 3) Union
            var allIds = docOrPartnerIds
                .Concat(metaIds)
                .Distinct()
                .Take(take)
                .ToList();

            if (allIds.Count == 0)
                return new List<DocumentSearchResultDto>();

            // 4) Dokumentumok + PartnerName (rendezve dátum szerint)
            var docs = await (
                from d in _context.Documents.AsNoTracking()
                join p in _context.Partners.AsNoTracking() on d.PartnerId equals p.PartnerId into pj
                from p in pj.DefaultIfEmpty()
                where d.IsDeleted == false && allIds.Contains(d.DocumentId)
                orderby d.UploadDate descending
                select new
                {
                    d.DocumentId,
                    d.FileName,
                    d.FilePath,
                    d.UploadDate,
                    d.PartnerId,
                    PartnerName = p != null ? p.Name : null,
                    d.Status
                }
            )
            .Take(take)
            .ToListAsync();

            var ids = docs.Select(x => x.DocumentId).Distinct().ToList();

            // 5) Metadata lekérés
            var metas = await _context.DocumentMetadata
                .AsNoTracking()
                .Where(m => ids.Contains(m.DocumentId))
                .ToListAsync();

            var metaByDoc = metas
                .GroupBy(m => m.DocumentId)
                .ToDictionary(
                    g => g.Key,
                    g => g.GroupBy(x => x.Key)
                          .ToDictionary(
                              gg => gg.Key,
                              gg => string.Join(", ", gg.Select(x => x.Value))
                          )
                );

            return docs.Select(d => new DocumentSearchResultDto
            {
                DocumentId = d.DocumentId,
                FileName = d.FileName ?? "",
                FilePath = d.FilePath ?? "",
                UploadDate = d.UploadDate,
                PartnerId = d.PartnerId,
                PartnerName = d.PartnerName ?? "",
                Status = d.Status,
                Metadata = metaByDoc.TryGetValue(d.DocumentId, out var md) ? md : new Dictionary<string, string>()
            }).ToList();
        }

        // DTO a searchhöz
        public class DocumentSearchResultDto
        {
            public int DocumentId { get; set; }
            public string FileName { get; set; } = "";
            public string FilePath { get; set; } = "";
            public DateTime? UploadDate { get; set; }
            public int? PartnerId { get; set; }
            public DocumentStatusEnum Status { get; set; }
            public string PartnerName { get; set; } = "";
            public Dictionary<string, string> Metadata { get; set; } = new();
        }

        public class DocumentListItemDto
        {
            public int DocumentId { get; set; }
            public string? FileName { get; set; }
            public string? DocumentName { get; set; }

            public DateTime? UploadDate { get; set; }
            public string? UploadedBy { get; set; }

            public int? PartnerId { get; set; }
            public string? PartnerName { get; set; }

            public int? SiteId { get; set; }

            public int? DocumentTypeId { get; set; }
            public string? DocumentTypeName { get; set; }

            public DocumentStatusEnum Status { get; set; }

            // új soft delete
            public bool IsDeleted { get; set; }

            // legacy kompat
            public bool? IsActive { get; set; }
        }

        public async Task<List<DocumentListItemDto>> GetDocumentsListAsync(
            string? searchTerm,
            int? documentTypeId,
            int? partnerId,
            int? siteId,
            DocumentStatusEnum? status,
            DateTime? dateFrom,
            DateTime? dateTo,
            bool includeInactive,
            string? sortBy,
            string? sortDir,
            int skip,
            int take)
        {
            take = Math.Clamp(take, 1, 200);
            if (skip < 0) skip = 0;

            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext!.User);
            var isAdmin = user != null &&
                          (await _userManager.IsInRoleAsync(user, "Admin") ||
                           await _userManager.IsInRoleAsync(user, "SuperAdmin"));

            // ✅ Könnyű list query (NINCS Include links/history/metadata)
            var query =
                from d in _context.Documents.AsNoTracking()
                join p in _context.Partners.AsNoTracking() on d.PartnerId equals p.PartnerId into pj
                from p in pj.DefaultIfEmpty()
                join dt in _context.DocumentTypes.AsNoTracking() on d.DocumentTypeId equals dt.DocumentTypeId into dtj
                from dt in dtj.DefaultIfEmpty()
                select new DocumentListItemDto
                {
                    DocumentId = d.DocumentId,
                    FileName = d.FileName,
                    DocumentName = d.DocumentName,
                    UploadDate = d.UploadDate,
                    UploadedBy = d.UploadedBy,
                    PartnerId = d.PartnerId,
                    PartnerName = p != null ? p.Name : null,
                    SiteId = d.SiteId,
                    DocumentTypeId = d.DocumentTypeId,
                    DocumentTypeName = dt != null ? dt.Name : null,
                    Status = d.Status,

                    IsDeleted = d.IsDeleted,
                    IsActive = d.isActive
                };

            if (!isAdmin)
                query = query.Where(x => x.UploadedBy == GetCurrentUser());

            // ✅ soft delete: default csak nem törölt
            if (!includeInactive)
                query = query.Where(x => x.IsDeleted == false);

            // ✅ search (könnyű mezőkön)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();
                query = query.Where(x =>
                    (x.FileName != null && x.FileName.Contains(term)) ||
                    (x.DocumentName != null && x.DocumentName.Contains(term)) ||
                    (x.UploadedBy != null && x.UploadedBy.Contains(term)) ||
                    (x.PartnerName != null && x.PartnerName.Contains(term)) ||
                    (x.DocumentTypeName != null && x.DocumentTypeName.Contains(term))
                );
            }

            // ✅ filters
            if (documentTypeId.HasValue) query = query.Where(x => x.DocumentTypeId == documentTypeId);
            if (partnerId.HasValue) query = query.Where(x => x.PartnerId == partnerId);
            if (siteId.HasValue) query = query.Where(x => x.SiteId == siteId);
            if (status.HasValue) query = query.Where(x => x.Status == status.Value);

            // ✅ date range (UploadDate)
            if (dateFrom.HasValue)
            {
                var from = dateFrom.Value.Date;
                query = query.Where(x => x.UploadDate >= from);
            }
            if (dateTo.HasValue)
            {
                var toExclusive = dateTo.Value.Date.AddDays(1);
                query = query.Where(x => x.UploadDate < toExclusive);
            }

            // ✅ sorting
            sortBy = (sortBy ?? "uploaddate").Trim().ToLowerInvariant();
            sortDir = string.Equals(sortDir, "asc", StringComparison.OrdinalIgnoreCase) ? "asc" : "desc";
            bool asc = sortDir == "asc";

            query = sortBy switch
            {
                "filename" => asc ? query.OrderBy(x => x.FileName) : query.OrderByDescending(x => x.FileName),
                "documentname" => asc ? query.OrderBy(x => x.DocumentName) : query.OrderByDescending(x => x.DocumentName),
                "documentid" => asc ? query.OrderBy(x => x.DocumentId) : query.OrderByDescending(x => x.DocumentId),
                "status" => asc ? query.OrderBy(x => x.Status) : query.OrderByDescending(x => x.Status),
                _ => asc ? query.OrderBy(x => x.UploadDate) : query.OrderByDescending(x => x.UploadDate),
            };

            return await query.Skip(skip).Take(take).ToListAsync();
        }

        public async Task<int> GetDocumentsListCountAsync(
            string? searchTerm,
            int? documentTypeId,
            int? partnerId,
            int? siteId,
            DocumentStatusEnum? status,
            DateTime? dateFrom,
            DateTime? dateTo,
            bool includeInactive)
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext!.User);
            var isAdmin = user != null &&
                          (await _userManager.IsInRoleAsync(user, "Admin") ||
                           await _userManager.IsInRoleAsync(user, "SuperAdmin"));

            var query = _context.Documents.AsNoTracking().AsQueryable();

            if (!isAdmin)
                query = query.Where(d => d.UploadedBy == GetCurrentUser());

            if (!includeInactive)
                query = query.Where(d => d.IsDeleted == false);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();
                query = query.Where(d =>
                    (d.FileName != null && d.FileName.Contains(term)) ||
                    (d.DocumentName != null && d.DocumentName.Contains(term)) ||
                    (d.UploadedBy != null && d.UploadedBy.Contains(term))
                );
                // PartnerName/DocumentTypeName kereséshez JOIN kéne -> a list endpoint ezt kezeli.
            }

            if (documentTypeId.HasValue) query = query.Where(d => d.DocumentTypeId == documentTypeId);
            if (partnerId.HasValue) query = query.Where(d => d.PartnerId == partnerId);
            if (siteId.HasValue) query = query.Where(d => d.SiteId == siteId);
            if (status.HasValue) query = query.Where(d => d.Status == status.Value);

            if (dateFrom.HasValue)
            {
                var from = dateFrom.Value.Date;
                query = query.Where(d => d.UploadDate >= from);
            }
            if (dateTo.HasValue)
            {
                var toExclusive = dateTo.Value.Date.AddDays(1);
                query = query.Where(d => d.UploadDate < toExclusive);
            }

            return await query.CountAsync();
        }

        // -----------------------------------
        // GET ONE (View/Edit modalhoz)
        // -----------------------------------
        public async Task<DocumentDto?> GetDocumentAsync(int documentId)
        {
            try
            {
                var isAdmin = await IsAdminAsync();

                var query =
                    _context.Documents
                        .AsNoTracking()
                        .Include(d => d.DocumentType)
                        .Include(d => d.DocumentLinks)
                        .Include(d => d.StatusHistory)
                        .Include(d => d.DocumentMetadata)
                        .GroupJoin(
                            _context.Partners.AsNoTracking(),
                            d => d.PartnerId,
                            p => p.PartnerId,
                            (d, p) => new { Document = d, Partner = p }
                        )
                        .SelectMany(
                            dp => dp.Partner.DefaultIfEmpty(),
                            (d, p) => new DocumentDto
                            {
                                DocumentId = d.Document.DocumentId,

                                FileName = d.Document.FileName,
                                FilePath = d.Document.FilePath,

                                OriginalFileName = d.Document.OriginalFileName,
                                StoredFileName = d.Document.StoredFileName,
                                FileExtension = d.Document.FileExtension,
                                ContentType = d.Document.ContentType,
                                FileSizeBytes = d.Document.FileSizeBytes,
                                StorageProvider = d.Document.StorageProvider,
                                StorageKey = d.Document.StorageKey,

                                DocumentName = d.Document.DocumentName,
                                DocumentDescription = d.Document.DocumentDescription,

                                HashAlgorithm = d.Document.HashAlgorithm,
                                FileHash = d.Document.FileHash,

                                CreatedAt = d.Document.CreatedAt,
                                CreatedBy = d.Document.CreatedBy,
                                UpdatedAt = d.Document.UpdatedAt,
                                UpdatedBy = d.Document.UpdatedBy,
                                DeletedAt = d.Document.DeletedAt,
                                DeletedBy = d.Document.DeletedBy,
                                IsDeleted = d.Document.IsDeleted,

                                VersionNumber = d.Document.VersionNumber,
                                IsLatestVersion = d.Document.IsLatestVersion,
                                ParentDocumentId = d.Document.ParentDocumentId,

                                ContactId = d.Document.ContactId,
                                DocumentStatusId = d.Document.DocumentStatusId,
                                StatusId = d.Document.StatusId,

                                DocumentTypeId = d.Document.DocumentTypeId,
                                DocumentTypeName = d.Document.DocumentType != null ? d.Document.DocumentType.Name : null,

                                UploadDate = d.Document.UploadDate,
                                UploadedBy = d.Document.UploadedBy,
                                SiteId = d.Document.SiteId,
                                PartnerId = d.Document.PartnerId,
                                PartnerName = d.Document.PartnerId.HasValue
                                    ? (p != null ? (p.Name ?? "Unknown") : "Unknown")
                                    : "N/A",

                                Status = d.Document.Status,

                                DocumentLinks = d.Document.DocumentLinks.Select(l => new DocumentLinkDto
                                {
                                    Id = l.ID,
                                    DocumentId = l.DocumentId,
                                    ModuleId = l.ModuleID,
                                    RecordId = l.RecordID
                                }).ToList(),

                                StatusHistory = d.Document.StatusHistory
                                    .OrderByDescending(h => h.ChangeDate)
                                    .Select(h => new DocumentStatusHistoryDto
                                    {
                                        Id = h.Id,
                                        DocumentId = h.DocumentId,
                                        OldStatus = h.OldStatus,
                                        NewStatus = h.NewStatus,
                                        ChangeDate = h.ChangeDate,
                                        ChangedBy = h.ChangedBy
                                    }).ToList(),

                                CustomMetadata = d.Document.DocumentMetadata
                                    .OrderBy(m => m.Key)
                                    .Select(m => new MetadataEntry
                                    {
                                        Key = m.Key,
                                        Value = m.Value
                                    })
                                    .ToList()
                            });

                if (!isAdmin)
                    query = query.Where(d => d.UploadedBy == GetCurrentUser());

                // alapból ne adjunk vissza töröltet
                query = query.Where(d => d.IsDeleted == false);

                return await query.FirstOrDefaultAsync(d => d.DocumentId == documentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching document {DocumentId}", documentId);
                throw;
            }
        }

        // ---------------------------------------------------------
        // LISTA: régi hívások kompatibilitása (NINCS dátum/sortDir)
        // ---------------------------------------------------------
        public Task<List<DocumentDto>> GetDocumentsAsync(
            string? searchTerm,
            int? documentTypeId,
            int? partnerId,
            int? siteId,
            DocumentStatusEnum? status,
            string? sortBy,
            int skip,
            int take)
        {
            return GetDocumentsAsync(
                searchTerm: searchTerm,
                documentTypeId: documentTypeId,
                partnerId: partnerId,
                siteId: siteId,
                status: status,
                dateFrom: null,
                dateTo: null,
                sortBy: sortBy,
                sortDir: "desc",
                skip: skip,
                take: take);
        }

        // ---------------------------------------------------------
        // LISTA: teljes (összetett szűrőhöz) + soft delete + sortDir
        // ---------------------------------------------------------
        public async Task<List<DocumentDto>> GetDocumentsAsync(
            string? searchTerm,
            int? documentTypeId,
            int? partnerId,
            int? siteId,
            DocumentStatusEnum? status,
            DateTime? dateFrom,
            DateTime? dateTo,
            string? sortBy,
            string? sortDir,
            int skip,
            int take)
        {
            try
            {
                take = Math.Clamp(take, 1, 200);
                if (skip < 0) skip = 0;

                var isAdmin = await IsAdminAsync();

                // LISTA: ne include-oljuk a heavy gyűjteményeket
                var baseQuery =
                    from d in _context.Documents.AsNoTracking()
                    join p in _context.Partners.AsNoTracking() on d.PartnerId equals p.PartnerId into pj
                    from p in pj.DefaultIfEmpty()
                    join dt in _context.DocumentTypes.AsNoTracking() on d.DocumentTypeId equals dt.DocumentTypeId into dtj
                    from dt in dtj.DefaultIfEmpty()
                    where d.IsDeleted == false // ✅ soft delete default
                    select new DocumentDto
                    {
                        DocumentId = d.DocumentId,
                        FileName = d.FileName,
                        FilePath = d.FilePath,

                        DocumentName = d.DocumentName,
                        DocumentDescription = d.DocumentDescription,

                        OriginalFileName = d.OriginalFileName,
                        StoredFileName = d.StoredFileName,
                        FileExtension = d.FileExtension,
                        ContentType = d.ContentType,
                        FileSizeBytes = d.FileSizeBytes,
                        StorageProvider = d.StorageProvider,
                        StorageKey = d.StorageKey,

                        HashAlgorithm = d.HashAlgorithm,
                        FileHash = d.FileHash,

                        CreatedAt = d.CreatedAt,
                        CreatedBy = d.CreatedBy,
                        UpdatedAt = d.UpdatedAt,
                        UpdatedBy = d.UpdatedBy,
                        DeletedAt = d.DeletedAt,
                        DeletedBy = d.DeletedBy,
                        IsDeleted = d.IsDeleted,

                        VersionNumber = d.VersionNumber,
                        IsLatestVersion = d.IsLatestVersion,
                        ParentDocumentId = d.ParentDocumentId,

                        ContactId = d.ContactId,
                        DocumentStatusId = d.DocumentStatusId,
                        StatusId = d.StatusId,

                        DocumentTypeId = d.DocumentTypeId,
                        DocumentTypeName = dt != null ? dt.Name : null,
                        UploadDate = d.UploadDate,
                        UploadedBy = d.UploadedBy,
                        SiteId = d.SiteId,
                        PartnerId = d.PartnerId,
                        PartnerName = p != null ? p.Name : "N/A",
                        Status = d.Status,

                        // listán nem kell
                        DocumentLinks = new List<DocumentLinkDto>(),
                        StatusHistory = new List<DocumentStatusHistoryDto>(),
                        CustomMetadata = new List<MetadataEntry>()
                    };

                if (!isAdmin)
                    baseQuery = baseQuery.Where(x => x.UploadedBy == GetCurrentUser());

                // search
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    var term = searchTerm.Trim();
                    baseQuery = baseQuery.Where(x =>
                        (x.FileName != null && x.FileName.Contains(term)) ||
                        (x.DocumentName != null && x.DocumentName.Contains(term)) ||
                        (x.UploadedBy != null && x.UploadedBy.Contains(term)) ||
                        (x.PartnerName != null && x.PartnerName.Contains(term)) ||
                        (x.DocumentTypeName != null && x.DocumentTypeName.Contains(term))
                    );
                }

                // filters
                if (documentTypeId.HasValue) baseQuery = baseQuery.Where(x => x.DocumentTypeId == documentTypeId);
                if (partnerId.HasValue) baseQuery = baseQuery.Where(x => x.PartnerId == partnerId);
                if (siteId.HasValue) baseQuery = baseQuery.Where(x => x.SiteId == siteId);
                if (status.HasValue) baseQuery = baseQuery.Where(x => x.Status == status.Value);

                // date range (UploadDate)
                if (dateFrom.HasValue)
                {
                    var from = dateFrom.Value.Date;
                    baseQuery = baseQuery.Where(x => x.UploadDate >= from);
                }
                if (dateTo.HasValue)
                {
                    var toExclusive = dateTo.Value.Date.AddDays(1);
                    baseQuery = baseQuery.Where(x => x.UploadDate < toExclusive);
                }

                // sorting
                sortBy = (sortBy ?? "uploaddate").Trim().ToLowerInvariant();
                sortDir = string.Equals(sortDir, "asc", StringComparison.OrdinalIgnoreCase) ? "asc" : "desc";
                var asc = sortDir == "asc";

                baseQuery = sortBy switch
                {
                    "filename" => asc ? baseQuery.OrderBy(x => x.FileName) : baseQuery.OrderByDescending(x => x.FileName),
                    "documentname" => asc ? baseQuery.OrderBy(x => x.DocumentName) : baseQuery.OrderByDescending(x => x.DocumentName),
                    "documentid" => asc ? baseQuery.OrderBy(x => x.DocumentId) : baseQuery.OrderByDescending(x => x.DocumentId),
                    "status" => asc ? baseQuery.OrderBy(x => x.Status) : baseQuery.OrderByDescending(x => x.Status),
                    _ => asc ? baseQuery.OrderBy(x => x.UploadDate) : baseQuery.OrderByDescending(x => x.UploadDate)
                };

                return await baseQuery.Skip(skip).Take(take).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching documents");
                throw;
            }
        }

        // -----------------------------------
        // COUNT (paginationhoz)
        // -----------------------------------
        public async Task<int> GetDocumentsCountAsync(
            string? searchTerm,
            int? documentTypeId,
            int? partnerId,
            int? siteId,
            DocumentStatusEnum? status = null,
            DateTime? dateFrom = null,
            DateTime? dateTo = null)
        {
            var isAdmin = await IsAdminAsync();

            var query = _context.Documents.AsNoTracking().Where(d => d.IsDeleted == false);

            if (!isAdmin)
                query = query.Where(d => d.UploadedBy == GetCurrentUser());

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();
                query = query.Where(d =>
                    (d.FileName != null && d.FileName.Contains(term)) ||
                    (d.DocumentName != null && d.DocumentName.Contains(term)) ||
                    (d.UploadedBy != null && d.UploadedBy.Contains(term)));
            }

            if (documentTypeId.HasValue) query = query.Where(d => d.DocumentTypeId == documentTypeId);
            if (partnerId.HasValue) query = query.Where(d => d.PartnerId == partnerId);
            if (siteId.HasValue) query = query.Where(d => d.SiteId == siteId);
            if (status.HasValue) query = query.Where(d => d.Status == status.Value);

            if (dateFrom.HasValue)
            {
                var from = dateFrom.Value.Date;
                query = query.Where(d => d.UploadDate >= from);
            }
            if (dateTo.HasValue)
            {
                var toExclusive = dateTo.Value.Date.AddDays(1);
                query = query.Where(d => d.UploadDate < toExclusive);
            }

            return await query.CountAsync();
        }

        // -----------------------------------
        // CREATE
        // -----------------------------------
        public async Task<DocumentDto> CreateDocumentAsync(CreateDocumentDto documentDto)
        {
            if (documentDto == null)
                throw new ArgumentNullException(nameof(documentDto));

            var isAdmin = await IsAdminAsync();
            var isEditor = await IsEditorAsync();
            if (!isAdmin && !isEditor)
                throw new UnauthorizedAccessException("User lacks permission to create documents.");

            var now = DateTime.UtcNow;
            var userName = GetCurrentUser();

            var doc = new Document
            {
                FileName = documentDto.FileName,
                FilePath = documentDto.FilePath,

                DocumentTypeId = documentDto.DocumentTypeId,
                PartnerId = documentDto.PartnerId,
                SiteId = documentDto.SiteId,
                ContactId = documentDto.ContactId,

                UploadDate = now,
                UploadedBy = userName,

                Status = documentDto.Status,

                // új meta
                OriginalFileName = documentDto.OriginalFileName,
                StoredFileName = documentDto.StoredFileName,
                FileExtension = documentDto.FileExtension,
                ContentType = documentDto.ContentType,
                FileSizeBytes = documentDto.FileSizeBytes,
                StorageProvider = documentDto.StorageProvider,
                StorageKey = documentDto.StorageKey,

                DocumentName = documentDto.DocumentName,
                DocumentDescription = documentDto.DocumentDescription,

                HashAlgorithm = string.IsNullOrWhiteSpace(documentDto.HashAlgorithm) ? "SHA256" : documentDto.HashAlgorithm,
                FileHash = documentDto.FileHash,

                // audit + soft delete
                CreatedAt = now,
                CreatedBy = userName,
                IsDeleted = false,

                // legacy kompat
                isActive = true,

                DocumentMetadata = new List<DocumentMetadata>()
            };

            // custom meta mentése (üreseket kidobjuk)
            if (documentDto.CustomMetadata != null && documentDto.CustomMetadata.Any())
            {
                foreach (var m in documentDto.CustomMetadata)
                {
                    var key = (m?.Key ?? "").Trim();
                    var value = (m?.Value ?? "").Trim();
                    if (string.IsNullOrWhiteSpace(key) && string.IsNullOrWhiteSpace(value)) continue;

                    doc.DocumentMetadata.Add(new DocumentMetadata
                    {
                        Key = key,
                        Value = value
                    });
                }
            }

            _context.Documents.Add(doc);
            await _context.SaveChangesAsync();

            // initial status history (force)
            await LogStatusChangeAsync(doc.DocumentId, doc.Status, doc.Status, force: true);

            return await GetDocumentAsync(doc.DocumentId)
                   ?? throw new Exception("Failed to retrieve created document");
        }

        // -----------------------------------
        // UPDATE
        // -----------------------------------
        public async Task<DocumentDto?> UpdateDocumentAsync(int documentId, DocumentDto documentUpdate)
        {
            if (documentUpdate == null)
                throw new ArgumentNullException(nameof(documentUpdate));

            var isAdmin = await IsAdminAsync();
            var isEditor = await IsEditorAsync();

            var doc = await _context.Documents
                .Include(d => d.DocumentMetadata)
                .FirstOrDefaultAsync(d => d.DocumentId == documentId);

            if (doc == null) return null;

            if (doc.IsDeleted)
                throw new InvalidOperationException("Cannot update a deleted document.");

            if (!isAdmin && !isEditor)
                throw new UnauthorizedAccessException();

            if (isEditor && doc.UploadedBy != GetCurrentUser())
                throw new UnauthorizedAccessException();

            if (doc.Status != documentUpdate.Status)
                await LogStatusChangeAsync(documentId, doc.Status, documentUpdate.Status);

            // alap mezők
            doc.FileName = documentUpdate.FileName ?? doc.FileName;
            doc.FilePath = documentUpdate.FilePath ?? doc.FilePath;
            doc.DocumentTypeId = documentUpdate.DocumentTypeId ?? doc.DocumentTypeId;
            doc.PartnerId = documentUpdate.PartnerId ?? doc.PartnerId;
            doc.SiteId = documentUpdate.SiteId ?? doc.SiteId;
            doc.ContactId = documentUpdate.ContactId ?? doc.ContactId;

            doc.Status = documentUpdate.Status;
            doc.DocumentStatusId = documentUpdate.DocumentStatusId ?? doc.DocumentStatusId;
            doc.StatusId = documentUpdate.StatusId ?? doc.StatusId;

            // új mezők
            doc.DocumentName = documentUpdate.DocumentName ?? doc.DocumentName;
            doc.DocumentDescription = documentUpdate.DocumentDescription ?? doc.DocumentDescription;

            doc.OriginalFileName = documentUpdate.OriginalFileName ?? doc.OriginalFileName;
            doc.StoredFileName = documentUpdate.StoredFileName ?? doc.StoredFileName;
            doc.FileExtension = documentUpdate.FileExtension ?? doc.FileExtension;
            doc.ContentType = documentUpdate.ContentType ?? doc.ContentType;
            doc.FileSizeBytes = documentUpdate.FileSizeBytes ?? doc.FileSizeBytes;
            doc.StorageProvider = documentUpdate.StorageProvider ?? doc.StorageProvider;
            doc.StorageKey = documentUpdate.StorageKey ?? doc.StorageKey;

            doc.HashAlgorithm = documentUpdate.HashAlgorithm ?? doc.HashAlgorithm;
            doc.FileHash = documentUpdate.FileHash ?? doc.FileHash;

            doc.VersionNumber = documentUpdate.VersionNumber ?? doc.VersionNumber;
            doc.IsLatestVersion = documentUpdate.IsLatestVersion ?? doc.IsLatestVersion;
            doc.ParentDocumentId = documentUpdate.ParentDocumentId ?? doc.ParentDocumentId;

            // audit
            doc.UpdatedAt = DateTime.UtcNow;
            doc.UpdatedBy = GetCurrentUser();

            // metadata replace
            var incoming = (documentUpdate.CustomMetadata ?? new List<MetadataEntry>())
                .Select(m => new
                {
                    Key = (m?.Key ?? "").Trim(),
                    Value = (m?.Value ?? "").Trim()
                })
                .Where(x => !(string.IsNullOrWhiteSpace(x.Key) && string.IsNullOrWhiteSpace(x.Value)))
                .ToList();

            _context.DocumentMetadata.RemoveRange(doc.DocumentMetadata);
            doc.DocumentMetadata.Clear();

            foreach (var m in incoming)
            {
                doc.DocumentMetadata.Add(new DocumentMetadata
                {
                    Key = m.Key,
                    Value = m.Value,
                    DocumentId = doc.DocumentId
                });
            }

            await _context.SaveChangesAsync();

            return await GetDocumentAsync(documentId);
        }

        // -----------------------------------
        // DELETE (soft)
        // -----------------------------------
        public async Task<bool> DeleteDocumentAsync(int documentId)
        {
            var isAdmin = await IsAdminAsync();
            if (!isAdmin) return false;

            var doc = await _context.Documents.FirstOrDefaultAsync(d => d.DocumentId == documentId);
            if (doc == null) return false;

            if (doc.IsDeleted) return true; // már törölt

            doc.IsDeleted = true;
            doc.DeletedAt = DateTime.UtcNow;
            doc.DeletedBy = GetCurrentUser();

            // legacy kompat
            doc.isActive = false;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> GetNextDocumentNumberAsync()
        {
            var dayOfYear = DateTime.UtcNow.DayOfYear;
            var random = new Random().Next(100, 1000);
            var count = await _context.Documents.CountAsync();

            return $"TestDocument-{dayOfYear}-{count + 1:D4}-{random}";
        }
    }
}
