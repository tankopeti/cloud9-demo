using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Cloud9_2.Data;
using Cloud9_2.Models;
using Cloud9_2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cloud9_2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeService _employeeService;
        private readonly DocumentService _documentService;
        private readonly ILogger<EmployeeController> _logger;
        private readonly ApplicationDbContext _context;

        // DocumentLinks.ModuleID ehhez a modulhoz
        private const string ModuleEmployee = "Employee";

        public EmployeeController(
            EmployeeService employeeService,
            DocumentService documentService,
            ApplicationDbContext context,
            ILogger<EmployeeController> logger)
        {
            _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
            _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // -----------------------------
        // INDEX (AJAX): /api/employee/index?page=1&pageSize=50&searchTerm=...&quickFilter=all
        // -----------------------------
        [HttpGet("index")]
        public async Task<IActionResult> Index(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 30,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? quickFilter = "all",

            // ===== Advanced filter (új, opcionális) =====
            [FromQuery] string? filterText = null,
            [FromQuery] string? filterPhone = null,
            [FromQuery] int? filterWorkerTypeId = null,
            [FromQuery] int? filterPartnerId = null,
            [FromQuery] int? filterStatusId = null,
            [FromQuery] int? filterSiteId = null,
            [FromQuery] bool? filterActiveOnly = null
        )
        {
            try
            {
                // van-e advanced filter aktív?
                var hasAdvanced =
                    !string.IsNullOrWhiteSpace(filterText) ||
                    !string.IsNullOrWhiteSpace(filterPhone) ||
                    filterWorkerTypeId.HasValue ||
                    filterPartnerId.HasValue ||
                    filterStatusId.HasValue ||
                    filterSiteId.HasValue ||
                    filterActiveOnly.HasValue;

                if (!hasAdvanced)
                {
                    // 100% kompatibilis: a meglévő működő hívás
                    var result = await _employeeService.GetEmployeesIndexAsync(page, pageSize, searchTerm, quickFilter);
                    return Ok(result);
                }
                else
                {
                    // ÚJ: advanced
                    var advanced = new EmployeeAdvancedFilterDto
                    {
                        Text = filterText,
                        Phone = filterPhone,
                        WorkerTypeId = filterWorkerTypeId,
                        PartnerId = filterPartnerId,
                        StatusId = filterStatusId,
                        SiteId = filterSiteId,
                        ActiveOnly = filterActiveOnly
                    };

                    var result = await _employeeService.GetEmployeesIndexAdvancedAsync(
                        page, pageSize, searchTerm, quickFilter, advanced);

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EmployeeController.Index failed.");
                return StatusCode(500, new { message = "Hiba történt a dolgozók betöltése közben." });
            }
        }

        // -----------------------------
        // READ (View/Edit modal): /api/employee/{id}
        // -----------------------------
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var item = await _employeeService.GetByIdAsync(id);
                if (item == null) return NotFound(new { message = "A dolgozó nem található." });
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EmployeeController.Get failed. EmployeeId={EmployeeId}", id);

#if DEBUG
                return StatusCode(500, new { message = ex.Message, detail = ex.ToString() });
#else
                return StatusCode(500, new { message = "Hiba történt a dolgozó lekérése közben." });
#endif
            }
        }

        // -----------------------------
        // CREATE: /api/employee
        // -----------------------------
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmployeesCreateDto dto)
        {
            if (dto == null) return BadRequest(new { message = "Hiányzó kérés tartalom." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _employeeService.CreateAsync(dto);
                if (!result.Success)
                {
#if DEBUG
                    return BadRequest(new { message = result.Error ?? "Nem sikerült a dolgozó létrehozása.", debug = result });
#else
                    return BadRequest(new { message = result.Error ?? "Nem sikerült a dolgozó létrehozása." });
#endif
                }

                // Option 1: 200 OK
                return Ok(new { employeeId = result.Data });

                // Option 2 (recommended REST): 201 Created
                // return CreatedAtAction(nameof(Get), new { id = result.Data }, new { employeeId = result.Data });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EmployeeController.Create failed.");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, detail = ex.ToString() });
#else
                return StatusCode(500, new { message = "Hiba történt a dolgozó létrehozása közben." });
#endif
            }
        }

        // -----------------------------
        // UPDATE: /api/employee/{id}
        // -----------------------------
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] EmployeesUpdateDto dto)
        {
            if (dto == null) return BadRequest("dto is null");

            dto.EmployeeId = id; // <- KÖTELEZŐ, nehogy 0 legyen vagy rossz id

            var result = await _employeeService.UpdateAsync(dto);
            if (!result.Success) return BadRequest(new { message = result.Error });

            return Ok(result);
        }

        // -----------------------------
        // DELETE (soft): /api/employee/{id}
        // -----------------------------
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _employeeService.SoftDeleteAsync(id);
                if (!result.Success)
                    return BadRequest(new { message = result.Error ?? "Nem sikerült a dolgozó törlése." });

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EmployeeController.Delete failed. EmployeeId={EmployeeId}", id);
                return StatusCode(500, new { message = "Hiba történt a dolgozó törlése közben." });
            }
        }

        // ============================================================
        // EMPLOYEE DOCUMENTS (NEW)
        // ============================================================

        // GET: /api/employee/{id}/documents?includeInactive=false&skip=0&take=50
        [HttpGet("{id:int}/documents")]
        public async Task<IActionResult> GetEmployeeDocuments(
            [FromRoute] int id,
            [FromQuery] bool includeInactive = false,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 50)
        {
            take = Math.Clamp(take, 1, 200);
            if (skip < 0) skip = 0;

            var exists = await _context.Employees.AnyAsync(e => e.EmployeeId == id);
            if (!exists) return NotFound(new { message = "A dolgozó nem található." });

            var query =
                from l in _context.DocumentLinks.AsNoTracking()
                join d in _context.Documents.AsNoTracking() on l.DocumentId equals d.DocumentId
                where l.ModuleID == ModuleEmployee
                      && l.RecordID == id
                select new
                {
                    d.DocumentId,
                    d.DocumentName,
                    d.DocumentDescription,

                    d.FileName,
                    d.OriginalFileName,
                    d.StoredFileName,
                    d.FileExtension,
                    d.ContentType,
                    d.FileSizeBytes,

                    d.UploadDate,
                    d.UploadedBy,
                    d.Status,

                    d.IsDeleted,
                    d.VersionNumber,
                    d.IsLatestVersion,
                    d.ParentDocumentId
                };

            if (!includeInactive)
                query = query.Where(x => x.IsDeleted == false);

            var items = await query
                .OrderByDescending(x => x.UploadDate)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return Ok(items);
        }

        // POST: /api/employee/{id}/documents  (multipart/form-data: file + payloadJson)
        [HttpPost("{id:int}/documents")]
        [RequestSizeLimit(200_000_000)]
        public async Task<IActionResult> UploadEmployeeDocument(
            [FromRoute] int id,
            [FromForm] IFormFile file,
            [FromForm] string payloadJson)
        {
            var exists = await _context.Employees.AnyAsync(e => e.EmployeeId == id);
            if (!exists) return NotFound(new { message = "A dolgozó nem található." });

            if (file == null || file.Length == 0)
                return BadRequest(new { message = "File is required" });

            if (string.IsNullOrWhiteSpace(payloadJson))
                return BadRequest(new { message = "payloadJson is required" });

            CreateDocumentPayload payload;
            try
            {
                payload = System.Text.Json.JsonSerializer.Deserialize<CreateDocumentPayload>(
                    payloadJson,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? throw new Exception("payload deserialized null");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Invalid payloadJson", detail = ex.Message });
            }

            if (!Enum.TryParse<DocumentStatusEnum>(payload.Status, true, out var statusEnum))
                return BadRequest(new { message = "Invalid Status enum value", status = payload.Status });

            // 1) file mentés (most még a meglévő helyre)
            var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "documents");
            Directory.CreateDirectory(uploadsRoot);

            var safeFileName = Path.GetFileName(file.FileName);
            var storedFileName = $"{Guid.NewGuid():N}_{safeFileName}";
            var fullPath = Path.Combine(uploadsRoot, storedFileName);

            await using (var stream = System.IO.File.Create(fullPath))
            {
                await file.CopyToAsync(stream);
            }

            var fi = new FileInfo(fullPath);
            var relativePath = $"/uploads/documents/{storedFileName}";
            var sha256Hex = await ComputeSha256HexAsync(fullPath);

            // 2) doc rekord (service-en keresztül)
            var dto = new CreateDocumentDto
            {
                FileName = string.IsNullOrWhiteSpace(payload.FileName) ? safeFileName : payload.FileName.Trim(),
                FilePath = relativePath,
                DocumentTypeId = payload.DocumentTypeId,
                PartnerId = payload.PartnerId,
                SiteId = payload.SiteId,
                ContactId = payload.ContactId,
                Status = statusEnum,
                CustomMetadata = payload.CustomMetadata ?? new List<MetadataEntry>(),

                OriginalFileName = safeFileName,
                StoredFileName = storedFileName,
                FileExtension = Path.GetExtension(safeFileName),
                ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? payload.ContentType : file.ContentType,
                FileSizeBytes = fi.Exists ? fi.Length : file.Length,
                StorageProvider = "FileSystem",
                StorageKey = relativePath,

                DocumentName = payload.DocumentName,
                DocumentDescription = payload.DocumentDescription,

                HashAlgorithm = "SHA256",
                FileHash = sha256Hex
            };

            var created = await _documentService.CreateDocumentAsync(dto);

            // 3) link Employee-hez (DocumentLinks)
            _context.DocumentLinks.Add(new DocumentLink
            {
                DocumentId = created.DocumentId,
                ModuleID = ModuleEmployee,
                RecordID = id
            });

            await _context.SaveChangesAsync();

            return Created($"/api/documents/{created.DocumentId}", new
            {
                created.DocumentId,
                created.DocumentName,
                created.FileName,
                created.OriginalFileName,
                created.FileExtension,
                created.ContentType,
                created.FileSizeBytes,
                created.UploadDate,
                created.UploadedBy,
                created.Status
            });
        }

        // ============================================================
        // LOOKUPS (TomSelect / dropdownok)
        // ============================================================

        // /api/employee/lookups/workertypes
        [HttpGet("lookups/workertypes")]
        public async Task<IActionResult> WorkerTypes()
        {
            try
            {
                var data = await _context.WorkerTypes
                    .AsNoTracking()
                    .Where(x => x.IsActive) // ha nincs IsActive, vedd ki
                    .OrderBy(x => x.WorkerTypeId)
                    .Select(x => new
                    {
                        id = x.WorkerTypeId,
                        name = x.Name,
                        code = x.Code // ha nincs, vedd ki
                    })
                    .ToListAsync();

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EmployeeController.WorkerTypes failed.");
                return StatusCode(500, new { message = "Nem sikerült a WorkerType lista betöltése." });
            }
        }

        // /api/employee/lookups/partners
        [HttpGet("lookups/partners")]
        public async Task<IActionResult> Partners()
        {
            try
            {
                var data = await _context.Partners
                    .AsNoTracking()
                    .OrderBy(p => p.Name)
                    .Select(p => new
                    {
                        id = p.PartnerId,
                        name = p.Name
                    })
                    .ToListAsync();

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EmployeeController.Partners failed.");
                return StatusCode(500, new { message = "Nem sikerült a Partner lista betöltése." });
            }
        }

        // /api/employee/lookups/jobtitles
        [HttpGet("lookups/jobtitles")]
        public async Task<IActionResult> JobTitles()
        {
            try
            {
                var data = await _context.JobTitles
                    .AsNoTracking()
                    .OrderBy(j => j.TitleName)
                    .Select(j => new
                    {
                        id = j.JobTitleId,
                        name = j.TitleName
                    })
                    .ToListAsync();

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EmployeeController.JobTitles failed.");
                return StatusCode(500, new { message = "Nem sikerült a Pozíció (JobTitle) lista betöltése." });
            }
        }

        // /api/employee/lookups/employmentstatus
        [HttpGet("lookups/employmentstatus")]
        public async Task<IActionResult> EmploymentStatuses()
        {
            try
            {
                var data = await _context.EmploymentStatuses
                    .AsNoTracking()
                    .OrderBy(s => s.StatusName) // ha nálad Name, akkor OrderBy(s => s.Name)
                    .Select(s => new
                    {
                        id = s.StatusId,
                        name = s.StatusName // ha nálad Name, akkor name = s.Name
                    })
                    .ToListAsync();

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EmployeeController.EmploymentStatuses failed.");
                return StatusCode(500, new { message = "Nem sikerült a státusz lista betöltése." });
            }
        }

        // /api/employee/lookups/sites
        [HttpGet("lookups/sites")]
        public async Task<IActionResult> Sites()
        {
            try
            {
                var data = await _context.Sites
                    .AsNoTracking()
                    .OrderBy(s => s.SiteName) // ha nálad Name, akkor OrderBy(s => s.Name)
                    .Select(s => new
                    {
                        id = s.SiteId,
                        name = s.SiteName, // ha nálad Name, akkor name = s.Name
                        partnerId = s.PartnerId
                    })
                    .ToListAsync();

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EmployeeController.Sites failed.");
                return StatusCode(500, new { message = "Nem sikerült a telephely lista betöltése." });
            }
        }

        [HttpGet("{id}/history")]
        public async Task<IActionResult> GetEmployeeHistory(int id)
        {
            var items = await _context.AuditLogs
                .Where(x => x.EntityType == "Employee" && x.EntityId == id)
                .OrderByDescending(x => x.ChangedAt)
                .Select(x => new
                {
                    x.ChangedAt,
                    x.ChangedByName,
                    x.Action,
                    x.Changes
                })
                .ToListAsync();

            return Ok(items);
        }

        // GET: /api/employee/{id}/sites
        [HttpGet("{id:int}/sites")]
        public async Task<IActionResult> GetEmployeeSites(int id)
        {
            var exists = await _context.Employees.AnyAsync(e => e.EmployeeId == id);
            if (!exists) return NotFound(new { message = "A dolgozó nem található." });

            var siteIds = await _context.EmployeeSites
                .AsNoTracking()
                .Where(x => x.EmployeeId == id)
                .Select(x => x.SiteId)
                .OrderBy(x => x)
                .ToListAsync();

            return Ok(new { siteIds });
        }

        // PUT: /api/employee/{id}/sites
        [HttpPut("{id:int}/sites")]
        public async Task<IActionResult> UpdateEmployeeSites(int id, [FromBody] EmployeeSitesDto dto)
        {
            if (dto == null) return BadRequest(new { message = "Hiányzó kérés tartalom." });

            var exists = await _context.Employees.AnyAsync(e => e.EmployeeId == id);
            if (!exists) return NotFound(new { message = "A dolgozó nem található." });

            // opcionális: valid site ellenőrzés (csak aktív / létező)
            var desired = (dto.SiteIds ?? new List<int>())
                .Where(x => x > 0)
                .Distinct()
                .ToList();

            // ✅ Diff-alapú csere (SaveChanges NÉLKÜL itt vagy a végén 1x)
            // Ha már megírtad service-ben: _employeeService.ReplaceSitesAsync(id, desired)
            await _employeeService.ReplaceSitesAsync(id, desired);

            // 1 db mentés a végén (a ReplaceSitesAsync ne mentsen!)
            await _context.SaveChangesAsync();

            return Ok(new { success = true, siteIds = desired });
        }

        // ---------- helpers ----------
        private static async Task<string> ComputeSha256HexAsync(string filePath)
        {
            await using var stream = System.IO.File.OpenRead(filePath);
            using var sha = SHA256.Create();
            var hash = await sha.ComputeHashAsync(stream);
            return Convert.ToHexString(hash); // .NET 5+
        }
    }
}