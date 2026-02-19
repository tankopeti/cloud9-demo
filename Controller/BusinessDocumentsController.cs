using AutoMapper;
using Cloud9_2.Data;
using Cloud9_2.Models;
using Cloud9_2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Cloud9_2.Controllers
{
    [Authorize]
    [Route("api/business-documents")]
    [ApiController]
    public class BusinessDocumentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly BusinessDocumentService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<BusinessDocumentsController> _logger;

        public BusinessDocumentsController(
            ApplicationDbContext context,
            BusinessDocumentService service,
            IMapper mapper,
            ILogger<BusinessDocumentsController> logger)
        {
            _context = context;
            _service = service;
            _mapper = mapper;
            _logger = logger;
        }

        private string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        // ----------------------------------------------------
        // Lookups (DTO)
        // ----------------------------------------------------

        [HttpGet("lookups/types")]
        public async Task<IActionResult> GetTypes()
        {
            var items = await _context.BusinessDocumentTypes
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.BusinessDocumentTypeId)
                .ToListAsync();

            return Ok(_mapper.Map<BusinessDocumentTypeDto[]>(items));
        }

        [HttpGet("lookups/statuses")]
        public async Task<IActionResult> GetStatuses()
        {
            var items = await _context.BusinessDocumentStatuses
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.BusinessDocumentStatusId)
                .ToListAsync();

            return Ok(_mapper.Map<BusinessDocumentStatusDto[]>(items));
        }

        [HttpGet("lookups/party-roles")]
        public async Task<IActionResult> GetPartyRoles()
        {
            var items = await _context.BusinessDocumentPartyRoles
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.BusinessDocumentPartyRoleId)
                .ToListAsync();

            return Ok(_mapper.Map<BusinessDocumentPartyRoleDto[]>(items));
        }

        [HttpGet("lookups/relation-types")]
        public async Task<IActionResult> GetRelationTypes()
        {
            var items = await _context.BusinessDocumentRelationTypes
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.BusinessDocumentRelationTypeId)
                .ToListAsync();

            return Ok(_mapper.Map<BusinessDocumentRelationTypeDto[]>(items));
        }

        // ----------------------------------------------------
        // List / Search
        // ----------------------------------------------------

        // GET api/business-documents?typeId=1&statusId=2&docNo=ABC&skip=0&take=50
        [HttpGet]
        public async Task<IActionResult> Search(
            [FromQuery] int? typeId,
            [FromQuery] int? statusId,
            [FromQuery] string? docNo,
            [FromQuery] string? subject,
            [FromQuery] DateTime? issueFrom,
            [FromQuery] DateTime? issueTo,
            [FromQuery] int? buyerPartnerId,
            [FromQuery] int? buyerRoleId,
            [FromQuery] int? skip,
            [FromQuery] int? take)
        {
            var req = new BusinessDocumentSearchRequest
            {
                BusinessDocumentTypeId = typeId,
                BusinessDocumentStatusId = statusId,
                DocumentNo = docNo,
                Subject = subject,
                IssueDateFrom = issueFrom,
                IssueDateTo = issueTo,
                BuyerPartnerId = buyerPartnerId,
                BuyerRoleId = buyerRoleId ?? 1,
                Skip = skip,
                Take = take
            };

            var docs = await _service.SearchAsync(req); // List<BusinessDocumentDto>
            return Ok(docs);
        }

        // ----------------------------------------------------
        // Get by id
        // ----------------------------------------------------
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id, [FromQuery] bool details = true)
        {
            var doc = await _service.GetByIdAsync(id, includeDetails: details); // BusinessDocumentDto
            if (doc == null) return NotFound();
            return Ok(doc);
        }

        // ----------------------------------------------------
        // Create (DTO)
        // ----------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BusinessDocumentCreateDto req)
        {
            if (req == null) return BadRequest();

            var id = await _service.CreateAsync(req, CurrentUserId);
            return CreatedAtAction(nameof(Get), new { id }, new { BusinessDocumentId = id });
        }

        // ----------------------------------------------------
        // Update header (DTO)
        // ----------------------------------------------------
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateHeader(int id, [FromBody] BusinessDocumentUpdateDto req)
        {
            if (req == null) return BadRequest();

            await _service.UpdateHeaderAsync(id, req, CurrentUserId);
            return NoContent();
        }

        // ----------------------------------------------------
        // Replace lines (DTO)
        // ----------------------------------------------------
        [HttpPut("{id:int}/lines")]
        public async Task<IActionResult> ReplaceLines(int id, [FromBody] BusinessDocumentLineCreateDto[] lines)
        {
            await _service.ReplaceLinesAsync(id, lines?.ToList() ?? new(), CurrentUserId);
            return NoContent();
        }

        // ----------------------------------------------------
        // Set parties (DTO)
        // ----------------------------------------------------
        [HttpPut("{id:int}/parties")]
        public async Task<IActionResult> SetParties(int id, [FromBody] BusinessDocumentPartyCreateDto[] parties)
        {
            await _service.SetPartiesAsync(id, parties?.ToList() ?? new(), CurrentUserId);
            return NoContent();
        }

        // ----------------------------------------------------
        // Add relation (DTO)
        // ----------------------------------------------------
        [HttpPost("{id:int}/relations")]
        public async Task<IActionResult> AddRelation(int id, [FromBody] BusinessDocumentRelationCreateDto req)
        {
            if (req == null) return BadRequest();
            if (req.ToBusinessDocumentId <= 0 || req.BusinessDocumentRelationTypeId <= 0)
                return BadRequest("Missing ToBusinessDocumentId or BusinessDocumentRelationTypeId.");

            // FromBusinessDocumentId-t a route paraméter adja
            req.FromBusinessDocumentId = id;

            await _service.AddRelationAsync(req, CurrentUserId);
            return NoContent();
        }

        // ----------------------------------------------------
        // Add attachment (DTO)
        // ----------------------------------------------------
        [HttpPost("{id:int}/attachments")]
        public async Task<IActionResult> AddAttachment(int id, [FromBody] BusinessDocumentAttachmentCreateDto req)
        {
            if (req == null) return BadRequest();
            if (req.DocumentId <= 0) return BadRequest("Missing DocumentId.");

            // BusinessDocumentId-t a route paraméter adja
            req.BusinessDocumentId = id;

            await _service.AddAttachmentAsync(req, CurrentUserId);
            return NoContent();
        }

        // ----------------------------------------------------
        // Change status
        // ----------------------------------------------------
        public class ChangeStatusRequest
        {
            public int NewStatusId { get; set; }
        }

        [HttpPost("{id:int}/status")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeStatusRequest req)
        {
            if (req == null || req.NewStatusId <= 0)
                return BadRequest("Missing NewStatusId.");

            await _service.ChangeStatusAsync(id, req.NewStatusId, CurrentUserId);
            return NoContent();
        }

        // ----------------------------------------------------
        // Soft delete
        // ----------------------------------------------------
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.SoftDeleteAsync(id, CurrentUserId);
            return NoContent();
        }
    }

    // ----------------------------------------------------------------
    // ElectronicDocument API (DTO)
    // ----------------------------------------------------------------
    [Authorize]
    [Route("api/electronic-documents")]
    [ApiController]
    public class ElectronicDocumentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ElectronicDocumentsController> _logger;

        public ElectronicDocumentsController(
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<ElectronicDocumentsController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // GET api/electronic-documents?query=invoice&skip=0&take=30
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string? query, [FromQuery] int? skip, [FromQuery] int? take)
        {
            IQueryable<ElectronicDocument> q = _context.Set<ElectronicDocument>().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query))
                q = q.Where(x => x.Name != null && x.Name.Contains(query));

            q = q.OrderByDescending(x => x.ElectronicDocumentId);

            if (skip.HasValue) q = q.Skip(skip.Value);
            if (take.HasValue) q = q.Take(take.Value);

            var items = await q.ToListAsync();
            return Ok(_mapper.Map<ElectronicDocumentDto[]>(items));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _context.Set<ElectronicDocument>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ElectronicDocumentId == id);

            if (item == null) return NotFound();
            return Ok(_mapper.Map<ElectronicDocumentDto>(item));
        }
    }
}
