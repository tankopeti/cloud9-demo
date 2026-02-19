using AutoMapper;
using Cloud9_2.Data;
using Cloud9_2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloud9_2.Services
{
    // CRUD + műveleti service a BusinessDocument modulhoz (DTO-alapú)
    public class BusinessDocumentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<BusinessDocumentService> _logger;

        public BusinessDocumentService(
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<BusinessDocumentService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // ----------------------------
        // GET
        // ----------------------------

        // Header + (opcionálisan) detail betöltés, majd DTO-ra map
        public async Task<BusinessDocumentDto?> GetByIdAsync(int id, bool includeDetails = true)
        {
            IQueryable<BusinessDocument> q = _context.BusinessDocuments.AsNoTracking();

            if (includeDetails)
            {
                q = q
                    .Include(x => x.BusinessDocumentType)
                    .Include(x => x.BusinessDocumentStatus)
                    .Include(x => x.Lines)
                    .Include(x => x.Parties)
                        .ThenInclude(p => p.BusinessDocumentPartyRole)
                    .Include(x => x.Attachments)
                    .Include(x => x.FromRelations)
                    .Include(x => x.ToRelations)
                    .Include(x => x.StatusHistory);
            }

            var entity = await q.FirstOrDefaultAsync(x => x.BusinessDocumentId == id);
            if (entity == null) return null;

            return _mapper.Map<BusinessDocumentDto>(entity);
        }

        public async Task<List<BusinessDocumentDto>> SearchAsync(BusinessDocumentSearchRequest req)
        {
            IQueryable<BusinessDocument> q = _context.BusinessDocuments.AsNoTracking();

            if (req.BusinessDocumentTypeId.HasValue)
                q = q.Where(x => x.BusinessDocumentTypeId == req.BusinessDocumentTypeId.Value);

            if (req.BusinessDocumentStatusId.HasValue)
                q = q.Where(x => x.BusinessDocumentStatusId == req.BusinessDocumentStatusId.Value);

            if (req.IssueDateFrom.HasValue)
                q = q.Where(x => x.IssueDate >= req.IssueDateFrom.Value);

            if (req.IssueDateTo.HasValue)
                q = q.Where(x => x.IssueDate <= req.IssueDateTo.Value);

            if (!string.IsNullOrWhiteSpace(req.DocumentNo))
                q = q.Where(x => x.DocumentNo != null && x.DocumentNo.Contains(req.DocumentNo));

            if (!string.IsNullOrWhiteSpace(req.Subject))
                q = q.Where(x => x.Subject != null && x.Subject.Contains(req.Subject));

            if (req.BuyerPartnerId.HasValue)
            {
                q = q.Where(d =>
                    d.Parties.Any(p =>
                        p.BusinessDocumentPartyRoleId == req.BuyerRoleId &&
                        p.PartnerId == req.BuyerPartnerId.Value));
            }

            q = q.OrderByDescending(x => x.BusinessDocumentId);

            if (req.Skip.HasValue) q = q.Skip(req.Skip.Value);
            if (req.Take.HasValue) q = q.Take(req.Take.Value);

            var entities = await q.ToListAsync();
            return _mapper.Map<List<BusinessDocumentDto>>(entities);
        }

        // ----------------------------
        // CREATE / UPDATE (HEADER)
        // ----------------------------

        // DTO -> Entity AutoMapper-rel + audit mezők
        public async Task<int> CreateAsync(BusinessDocumentCreateDto req, string? userId)
        {
            var now = DateTime.UtcNow;

            var doc = _mapper.Map<BusinessDocument>(req);

            doc.CreatedAt = now;
            doc.CreatedBy = userId;

            // ha tenant nálad kötelező, akkor create DTO-ban van; ha mégsem, itt állítsd
            // doc.TenantId = req.TenantId;

            // Lines
            if (req is not null)
            {
                // BusinessDocumentCreateDto nálunk nem tartalmazott Lines/Parties mezőt.
                // Ha nálad mégis van (kibővítetted), akkor az AutoMapper is tudja map-elni.
                // Ha nincs, akkor a sorok/partyk külön metódusokkal mennek.
            }

            _context.BusinessDocuments.Add(doc);
            await _context.SaveChangesAsync();

            return doc.BusinessDocumentId;
        }

        // Patch jellegű update: csak ami a DTO-ban ki van töltve
        public async Task UpdateHeaderAsync(int id, BusinessDocumentUpdateDto req, string? userId)
        {
            var doc = await _context.BusinessDocuments.FirstOrDefaultAsync(x => x.BusinessDocumentId == id);
            if (doc == null) throw new InvalidOperationException($"BusinessDocument not found: {id}");

            // A BusinessDocumentUpdateDto nálunk kötelező TypeId/StatusId-vel lett. Ha nálad patch kell,
            // akkor csinálj külön Patch DTO-t nullable mezőkkel.
            _mapper.Map(req, doc);

            doc.UpdatedAt = DateTime.UtcNow;
            doc.UpdatedBy = userId;

            await _context.SaveChangesAsync();
        }

        // ----------------------------
        // LINES
        // ----------------------------

        // Egyszerű replace (törlés + újra felvétel) DTO-val
        public async Task ReplaceLinesAsync(int businessDocumentId, List<BusinessDocumentLineCreateDto> lines, string? userId)
        {
            var doc = await _context.BusinessDocuments
                .Include(d => d.Lines)
                .FirstOrDefaultAsync(d => d.BusinessDocumentId == businessDocumentId);

            if (doc == null) throw new InvalidOperationException($"BusinessDocument not found: {businessDocumentId}");

            _context.BusinessDocumentLines.RemoveRange(doc.Lines);

            int lineNo = 1;
            foreach (var l in lines ?? new List<BusinessDocumentLineCreateDto>())
            {
                var entity = _mapper.Map<BusinessDocumentLine>(l);

                // biztosítsuk a kapcsolódást + default LineNo
                entity.BusinessDocumentId = businessDocumentId;
                entity.TenantId = doc.TenantId;

                if (entity.LineNo <= 0) entity.LineNo = lineNo++;
                doc.Lines.Add(entity);
            }

            doc.UpdatedAt = DateTime.UtcNow;
            doc.UpdatedBy = userId;

            await _context.SaveChangesAsync();
        }

        // ----------------------------
        // PARTIES
        // ----------------------------

        public async Task SetPartiesAsync(int businessDocumentId, List<BusinessDocumentPartyCreateDto> parties, string? userId)
        {
            var doc = await _context.BusinessDocuments
                .Include(d => d.Parties)
                .FirstOrDefaultAsync(d => d.BusinessDocumentId == businessDocumentId);

            if (doc == null) throw new InvalidOperationException($"BusinessDocument not found: {businessDocumentId}");

            _context.BusinessDocumentParties.RemoveRange(doc.Parties);

            foreach (var p in parties ?? new List<BusinessDocumentPartyCreateDto>())
            {
                var entity = _mapper.Map<BusinessDocumentParty>(p);

                entity.BusinessDocumentId = businessDocumentId;
                entity.TenantId = doc.TenantId;

                doc.Parties.Add(entity);
            }

            doc.UpdatedAt = DateTime.UtcNow;
            doc.UpdatedBy = userId;

            await _context.SaveChangesAsync();
        }

        // ----------------------------
        // RELATIONS
        // ----------------------------

        public async Task AddRelationAsync(BusinessDocumentRelationCreateDto req, string? userId)
        {
            if (req.FromBusinessDocumentId == req.ToBusinessDocumentId)
                throw new InvalidOperationException("FromBusinessDocumentId and ToBusinessDocumentId cannot be the same.");

            bool exists = await _context.BusinessDocumentRelations.AnyAsync(r =>
                r.TenantId == req.TenantId &&
                r.FromBusinessDocumentId == req.FromBusinessDocumentId &&
                r.ToBusinessDocumentId == req.ToBusinessDocumentId &&
                r.BusinessDocumentRelationTypeId == req.BusinessDocumentRelationTypeId);

            if (exists) return;

            var entity = _mapper.Map<BusinessDocumentRelation>(req);
            entity.CreatedAt = DateTime.UtcNow;
            entity.CreatedBy = userId;

            _context.BusinessDocumentRelations.Add(entity);
            await _context.SaveChangesAsync();
        }

        // ----------------------------
        // ATTACHMENTS
        // ----------------------------

        public async Task AddAttachmentAsync(BusinessDocumentAttachmentCreateDto req, string? userId)
        {
            // optional: ha primary, akkor régi primary-k lekapcsolása
            if (req.IsPrimary)
            {
                var existingPrimary = await _context.BusinessDocumentAttachments
                    .Where(x => x.TenantId == req.TenantId && x.BusinessDocumentId == req.BusinessDocumentId && x.IsPrimary)
                    .ToListAsync();

                foreach (var a in existingPrimary)
                    a.IsPrimary = false;
            }

            bool exists = await _context.BusinessDocumentAttachments.AnyAsync(a =>
                a.TenantId == req.TenantId &&
                a.BusinessDocumentId == req.BusinessDocumentId &&
                a.DocumentId == req.DocumentId);

            if (exists) return;

            var entity = _mapper.Map<BusinessDocumentAttachment>(req);
            _context.BusinessDocumentAttachments.Add(entity);

            await _context.SaveChangesAsync();
        }

        // ----------------------------
        // STATUS CHANGE + HISTORY
        // ----------------------------

        public async Task ChangeStatusAsync(int businessDocumentId, int newStatusId, string? userId)
        {
            var doc = await _context.BusinessDocuments.FirstOrDefaultAsync(x => x.BusinessDocumentId == businessDocumentId);
            if (doc == null) throw new InvalidOperationException($"BusinessDocument not found: {businessDocumentId}");

            if (doc.BusinessDocumentStatusId == newStatusId) return;

            var oldStatusId = doc.BusinessDocumentStatusId;

            doc.BusinessDocumentStatusId = newStatusId;
            doc.UpdatedAt = DateTime.UtcNow;
            doc.UpdatedBy = userId;

            var historyDto = new BusinessDocumentStatusHistoryCreateDto
            {
                TenantId = doc.TenantId,
                BusinessDocumentId = businessDocumentId,
                OldBusinessDocumentStatusId = oldStatusId,
                NewBusinessDocumentStatusId = newStatusId
            };

            var history = _mapper.Map<BusinessDocumentStatusHistory>(historyDto);
            history.ChangeDate = DateTime.UtcNow;
            history.ChangedBy = userId;

            _context.BusinessDocumentStatusHistories.Add(history);

            await _context.SaveChangesAsync();
        }

        // ----------------------------
        // SOFT DELETE
        // ----------------------------

        public async Task SoftDeleteAsync(int businessDocumentId, string? userId)
        {
            var doc = await _context.BusinessDocuments.FirstOrDefaultAsync(x => x.BusinessDocumentId == businessDocumentId);
            if (doc == null) return;

            doc.IsDeleted = true;
            doc.DeletedAt = DateTime.UtcNow;
            doc.DeletedBy = userId;

            await _context.SaveChangesAsync();
        }
    }

    // ----------------------------------------
    // Search request maradhat külön (nem EF entity)
    // ----------------------------------------

    public class BusinessDocumentSearchRequest
    {
        public int? BusinessDocumentTypeId { get; set; }
        public int? BusinessDocumentStatusId { get; set; }
        public DateTime? IssueDateFrom { get; set; }
        public DateTime? IssueDateTo { get; set; }
        public string? DocumentNo { get; set; }
        public string? Subject { get; set; }

        public int? BuyerPartnerId { get; set; }
        public int BuyerRoleId { get; set; } = 1;

        public int? Skip { get; set; }
        public int? Take { get; set; }
    }
}
