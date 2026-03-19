// Services/TaskPMService.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Cloud9_2.Models;
using Cloud9_2.Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace Cloud9_2.Services
{
    public class TaskPMService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<TaskPMService> _logger;

        public TaskPMService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<TaskPMService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region === Helpers ===

        // ✅ Safe: can return null if user missing
        private static string? FullName(ApplicationUser? user) => user?.UserName;

        // ✅ Base query for listing: AsNoTracking is ok, but keep Includes consistent with MapToDto
        private static IQueryable<TaskPM> BaseQuery(ApplicationDbContext ctx) =>
            ctx.TaskPMs
                .AsNoTracking()
                .Include(t => t.TaskTypePM)
                .Include(t => t.TaskStatusPM)
                .Include(t => t.TaskPriorityPM)
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .Include(t => t.Partner)
                .Include(t => t.RelatedPartner)
                .Include(t => t.Site)
                .Include(t => t.Contact)
                .Include(t => t.TaskPMcomMethod)
                .Include(t => t.Quote)
                .Include(t => t.Order)
                .Include(t => t.CustomerCommunication)
                .Include(t => t.CommunicationType)
                .Include(t => t.TaskResourceAssignments).ThenInclude(ra => ra.Resource)
                .Include(t => t.TaskEmployeeAssignments).ThenInclude(ea => ea.Employee)
                .Include(t => t.TaskHistories).ThenInclude(th => th.ModifiedBy);

        #endregion

        // -----------------------------------------------------------------
        // GET ALL
        // -----------------------------------------------------------------
        public async Task<List<TaskPMDto>> GetAllTasksAsync()
        {
            return await BaseQuery(_context)
                .Where(t => t.IsActive)
                .Select(t => MapToDto(t))
                .ToListAsync();
        }

        // -----------------------------------------------------------------
        // GET BY ID
        // -----------------------------------------------------------------
        public async Task<TaskPMDto?> GetTaskByIdAsync(int id)
        {
            // ❗️FONTOS: itt nem töltünk TaskDocuments/Document/LinkedBy navigációkat entity materializálással,
            // mert egy NULL string mező SqlNullValueException-t okozhat. Attachments-et projectionnel töltjük.
            var task = await _context.TaskPMs
                .Include(t => t.TaskTypePM)
                .Include(t => t.TaskStatusPM)
                .Include(t => t.TaskPriorityPM)
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .Include(t => t.Partner)
                .Include(t => t.RelatedPartner)
                .Include(t => t.Site)
                .Include(t => t.Contact)
                .Include(t => t.Quote)
                .Include(t => t.TaskPMcomMethod)
                .Include(t => t.Order)
                .Include(t => t.CustomerCommunication)
                .Include(t => t.CommunicationType)
                .Include(t => t.TaskResourceAssignments).ThenInclude(ra => ra.Resource)
                .Include(t => t.TaskEmployeeAssignments).ThenInclude(ea => ea.Employee)
                .Include(t => t.TaskHistories).ThenInclude(th => th.ModifiedBy)
                .FirstOrDefaultAsync(t => t.Id == id && t.IsActive);

            if (task == null) return null;

            // ✅ DTO alap (attachments itt üresre jön)
            var dto = MapToDto(task);

            // ✅ Attachments: TaskDocumentLinks → PROJECTION (nem Include + entity materializálás)
            dto.Attachments = await _context.TaskDocumentLinks
                .AsNoTracking()
                .Where(x => x.TaskId == id)
                .OrderByDescending(x => x.LinkedDate)
                .Select(x => new TaskDocumentDto
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,

                    // ✅ DB oldali COALESCE jellegű védelem: ne legyen SqlNullValueException
                    FileName = (x.Document != null ? x.Document.FileName : null) ?? "",
                    FilePath = (x.Document != null ? x.Document.FilePath : null) ?? "",

                    LinkedDate = x.LinkedDate,
                    LinkedByName = x.LinkedBy != null ? x.LinkedBy.UserName : null,
                    Note = x.Note
                })
                .ToListAsync();

            return dto;
        }

        // -----------------------------------------------------------------
        // CREATE
        // -----------------------------------------------------------------
        public async Task<TaskPMDto> CreateTaskAsync(TaskCreateDto dto, string currentUserId)
        {
            _logger.LogInformation("CreateTaskAsync started - User: {UserId}, DTO: {@DTO}", currentUserId, dto);

            // 1) VALIDATION
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ValidationException("A cím megadása kötelező.");

            if (dto.TaskTypePMId == null)
                throw new ValidationException("A feladat típusának kiválasztása kötelező.");

            var taskTypeExists = await _context.TaskTypePMs
                .AnyAsync(t => t.TaskTypePMId == dto.TaskTypePMId.Value);

            if (!taskTypeExists)
            {
                _logger.LogError("Invalid TaskTypePMId provided: {Id}", dto.TaskTypePMId.Value);
                throw new ValidationException("A kiválasztott feladat típus nem létezik.");
            }

            // Site validation
            if (dto.SiteId.HasValue && dto.SiteId.Value != 0)
            {
                if (dto.PartnerId == null || dto.PartnerId.Value == 0)
                    throw new ValidationException("Helyszín megadásakor Partner kiválasztása kötelező.");

                var siteExists = await _context.Sites.AnyAsync(s =>
                    s.SiteId == dto.SiteId.Value && s.PartnerId == dto.PartnerId);

                if (!siteExists)
                    throw new ValidationException("A Site nem tartozik a kiválasztott Partnerhez vagy nem létezik.");
            }

            // Map DTO to Entity
            var task = new TaskPM
            {
                Title = dto.Title,
                Description = dto.Description,
                Description2 = dto.Description2,
                IsActive = dto.IsActive,

                TaskTypePMId = dto.TaskTypePMId.Value,
                TaskStatusPMId = dto.TaskStatusPMId ?? 1,
                TaskPriorityPMId = dto.TaskPriorityPMId ?? 2,

                DueDate = dto.DueDate,
                EstimatedHours = dto.EstimatedHours,
                ActualHours = dto.ActualHours,
                AssignedToId = dto.AssignedToId,
                ScheduledDate = dto.ScheduledDate,
                OptionalDate1 = dto.OptionalDate1,
                OptionalDate2 = dto.OptionalDate2,
                CommunicationTypeId = dto.CommunicationTypeId,
                CommunicationDescription = dto.CommunicationDescription,
                TaskPMcomMethodID = dto.TaskPMcomMethodID,

                PartnerId = dto.PartnerId == 0 ? null : dto.PartnerId,
                RelatedPartnerId = dto.RelatedPartnerId == 0 ? null : dto.RelatedPartnerId,
                SiteId = dto.SiteId == 0 ? null : dto.SiteId,
                ContactId = dto.ContactId == 0 ? null : dto.ContactId,
                QuoteId = dto.QuoteId == 0 ? null : dto.QuoteId,
                OrderId = dto.OrderId == 0 ? null : dto.OrderId,
                CustomerCommunicationId = dto.CustomerCommunicationId == 0 ? null : dto.CustomerCommunicationId,

                CreatedById = currentUserId,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Adding task to context - Title: {Title}", task.Title);
                _context.TaskPMs.Add(task);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Task saved - Id: {Id}", task.Id);

                // --- Documents attach (TaskDocumentLinks) ---
                if (dto.AttachedDocumentIds?.Any() == true)
                {
                    var docIds = dto.AttachedDocumentIds
                        .Where(x => x > 0)
                        .Distinct()
                        .ToList();

                    // csak létező dokumentumokat engedünk
                    var existingDocIds = await _context.Documents
                        .Where(d => docIds.Contains(d.DocumentId))
                        .Select(d => d.DocumentId)
                        .ToListAsync();

                    if (existingDocIds.Count > 0)
                    {
                        // ne duplázzunk
                        var already = await _context.TaskDocumentLinks
                            .Where(x => x.TaskId == task.Id && existingDocIds.Contains(x.DocumentId))
                            .Select(x => x.DocumentId)
                            .ToListAsync();

                        var alreadySet = new HashSet<int>(already);

                        var links = existingDocIds
                            .Where(id => !alreadySet.Contains(id))
                            .Select(id => new TaskDocumentLink
                            {
                                TaskId = task.Id,
                                DocumentId = id,
                                LinkedDate = DateTime.UtcNow,
                                LinkedById = currentUserId
                            })
                            .ToList();

                        if (links.Count > 0)
                            _context.TaskDocumentLinks.AddRange(links);
                    }
                }

                // --- Resources Assignment ---
                if (dto.ResourceIds?.Any() == true)
                {
                    var validResourceIds = await _context.Resources
                        .Where(r => dto.ResourceIds.Contains(r.ResourceId))
                        .Select(r => r.ResourceId)
                        .ToListAsync();

                    if (validResourceIds.Any())
                    {
                        var res = validResourceIds.Select(rid => new TaskResourceAssignment
                        {
                            TaskPMId = task.Id,
                            ResourceId = rid
                        });

                        _logger.LogInformation("Adding {Count} valid resources", res.Count());
                        _context.TaskResourceAssignments.AddRange(res);
                    }
                }

                // --- Employees Assignment ---
                if (dto.EmployeeIds?.Any() == true)
                {
                    var validEmployeeIds = await _context.Employees
                        .Where(e => dto.EmployeeIds.Contains(e.EmployeeId))
                        .Select(e => e.EmployeeId)
                        .ToListAsync();

                    if (validEmployeeIds.Any())
                    {
                        var emp = validEmployeeIds.Select(eid => new TaskEmployeeAssignment
                        {
                            TaskPMId = task.Id,
                            EmployeeId = eid
                        });

                        _logger.LogInformation("Adding {Count} valid employees", emp.Count());
                        _context.TaskEmployeeAssignments.AddRange(emp);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Task with assignments saved successfully - Id: {Id}", task.Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "DATABASE ERROR in CreateTaskAsync for Task: {Title} by User: {UserId}.", task.Title, currentUserId);
                throw;
            }

            return await GetTaskByIdAsync(task.Id)
                   ?? throw new InvalidOperationException("Failed to retrieve the newly created task.");
        }

        // -----------------------------------------------------------------
        // UPDATE
        // -----------------------------------------------------------------
        public async Task<TaskPMDto> UpdateTaskAsync(TaskUpdateDto dto, string currentUserId)
        {
            _logger.LogInformation(
                "UpdateTaskAsync STARTED - TaskId: {TaskId}, User: {UserId}",
                dto.Id, currentUserId
            );

            var task = await _context.TaskPMs
                .Include(t => t.TaskResourceAssignments)
                .Include(t => t.TaskEmployeeAssignments)
                .FirstOrDefaultAsync(t => t.Id == dto.Id && t.IsActive);

            if (task == null)
                throw new KeyNotFoundException($"Task {dto.Id} not found.");

            // -------------------------------------------------
            // FIELD UPDATES
            // -------------------------------------------------
            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Description2 = dto.Description2;
            task.TaskTypePMId = dto.TaskTypePMId;

            if (dto.TaskStatusPMId.HasValue)
                task.TaskStatusPMId = dto.TaskStatusPMId.Value;

            if (dto.TaskPriorityPMId.HasValue)
                task.TaskPriorityPMId = dto.TaskPriorityPMId.Value;

            task.AssignedToId = dto.AssignedToId;

            task.DueDate = dto.DueDate;
            task.EstimatedHours = dto.EstimatedHours;
            task.ActualHours = dto.ActualHours;

            task.PartnerId = dto.PartnerId;
            task.RelatedPartnerId = dto.RelatedPartnerId;
            task.SiteId = dto.SiteId;
            task.ContactId = dto.ContactId;
            task.QuoteId = dto.QuoteId;
            task.OrderId = dto.OrderId;

            task.TaskPMcomMethodID = dto.TaskPMcomMethodID;
            task.CommunicationTypeId = dto.CommunicationTypeId;
            task.CommunicationDescription = dto.CommunicationDescription;

            task.ScheduledDate = dto.ScheduledDate;
            task.OptionalDate1 = dto.OptionalDate1;
            task.OptionalDate2 = dto.OptionalDate2;
            task.CustomerCommunicationId = dto.CustomerCommunicationId;

            task.UpdatedDate = DateTime.UtcNow;

            // -------------------------------------------------
            // 🔗 DOCUMENT LINKS SYNC (EDIT) - EGYSZER!
            // -------------------------------------------------
            if (dto.AttachedDocumentIds != null)
            {
                var existingLinks = await _context.TaskDocumentLinks
                    .Where(x => x.TaskId == task.Id)
                    .ToListAsync();

                var incomingIds = dto.AttachedDocumentIds
                    .Where(x => x > 0)
                    .Distinct()
                    .ToList();

                // 🗑️ TÖRLÉS – ami DB-ben van, de már nincs a payloadban
                var toRemove = existingLinks
                    .Where(x => !incomingIds.Contains(x.DocumentId))
                    .ToList();

                if (toRemove.Any())
                {
                    _context.TaskDocumentLinks.RemoveRange(toRemove);
                }

                // ➕ HOZZÁADÁS – ami újonnan jött
                var existingDocIds = existingLinks
                    .Select(x => x.DocumentId)
                    .ToHashSet();

                var toAdd = incomingIds
                    .Where(docId => !existingDocIds.Contains(docId))
                    .Select(docId => new TaskDocumentLink
                    {
                        TaskId = task.Id,
                        DocumentId = docId,
                        LinkedDate = DateTime.UtcNow,
                        LinkedById = currentUserId
                    })
                    .ToList();

                if (toAdd.Any())
                {
                    _context.TaskDocumentLinks.AddRange(toAdd);
                }
            }

task.ScheduledDate = dto.ScheduledDate;
task.CustomerCommunicationId = dto.CustomerCommunicationId;

task.UpdatedDate = DateTime.UtcNow;

// -------------------------------------------------
// COMPLETED DATE LOGIC
// -------------------------------------------------
if (dto.ReopenTask)
{
    task.CompletedDate = null;
}
else if (dto.CompletedDate.HasValue && !task.CompletedDate.HasValue)
{
    task.CompletedDate = dto.CompletedDate.Value;
}

            // -------------------------------------------------
            // RESOURCES (ADD ONLY)
            // -------------------------------------------------
            if (dto.ResourceIds?.Any() == true)
            {
                var validResourceIds = await _context.Resources
                    .Where(r => dto.ResourceIds.Contains(r.ResourceId))
                    .Select(r => r.ResourceId)
                    .ToListAsync();

                var existing = new HashSet<int>(
                    task.TaskResourceAssignments.Select(x => x.ResourceId)
                );

                var toAdd = validResourceIds
                    .Where(id => !existing.Contains(id))
                    .Select(id => new TaskResourceAssignment
                    {
                        TaskPMId = task.Id,
                        ResourceId = id
                    });

                _context.TaskResourceAssignments.AddRange(toAdd);
            }

            // -------------------------------------------------
            // EMPLOYEES (REPLACE)
            // -------------------------------------------------
            if (dto.EmployeeIds != null)
            {
                _context.TaskEmployeeAssignments.RemoveRange(task.TaskEmployeeAssignments);

                if (dto.EmployeeIds.Any())
                {
                    var validEmployeeIds = await _context.Employees
                        .Where(e => dto.EmployeeIds.Contains(e.EmployeeId))
                        .Select(e => e.EmployeeId)
                        .ToListAsync();

                    var newEmp = validEmployeeIds.Select(eid => new TaskEmployeeAssignment
                    {
                        TaskPMId = task.Id,
                        EmployeeId = eid
                    });

                    _context.TaskEmployeeAssignments.AddRange(newEmp);
                }
            }

            // -------------------------------------------------
            // SAVE
            // -------------------------------------------------

            _logger.LogInformation(
                "Before save TaskId={TaskId}, OptionalDate1={OptionalDate1}, OptionalDate2={OptionalDate2}",
                task.Id, task.OptionalDate1, task.OptionalDate2
            );

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "UpdateTaskAsync FINISHED - TaskId: {TaskId}",
                task.Id
            );

            return await GetTaskByIdAsync(task.Id)
                ?? throw new ValidationException("Failed to retrieve updated task.");
        }

        // -----------------------------------------------------------------
        // SOFT DELETE
        // -----------------------------------------------------------------
        public async Task DeleteTaskAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Invalid task id.");

            _logger.LogWarning("SoftDelete START id={Id}", id);

            var task = await _context.TaskPMs
                .FirstOrDefaultAsync(t => t.Id == id && t.IsActive)
                ?? throw new KeyNotFoundException($"Task {id} not found.");

            task.IsActive = false;
            task.UpdatedDate = DateTime.UtcNow;

            var affected = await _context.SaveChangesAsync();
            var changed = await _context.TaskPMs.CountAsync(t => !t.IsActive);
            _logger.LogWarning("Inactive count after delete: {Count}", changed);

            _logger.LogWarning("SoftDelete DONE id={Id} SaveChanges={Affected}", id, affected);
        }

        // -----------------------------------------------------------------
        // PAGED + FILTER + SEARCH + SORT
        // -----------------------------------------------------------------
        public async Task<PagedResult<TaskPMDto>> GetPagedTasksAsync(
            int page = 1,
            int pageSize = 10,
            string? searchTerm = null,
            string? sort = null,
            string? order = "desc",
            int? statusId = null,
            int? priorityId = null,
            int? taskTypeId = null,
            int? partnerId = null,
            int? siteId = null,
            int? relatedPartnerId = null,
            string? assignedToId = null,
            DateTime? dueDateFrom = null,
            DateTime? dueDateTo = null,
            DateTime? createdDateFrom = null,
            DateTime? createdDateTo = null,
            TaskDisplayType? displayType = null
        )
        {
            page = Math.Max(1, page);
            pageSize = Math.Max(1, Math.Min(100, pageSize));
            sort ??= "Id";
            order ??= "desc";

            // Base
            var query = BaseQuery(_context)
                .Where(t => t.IsActive);

            // -------------------------------------------------
            // ✅ DisplayType filter (Bejelentés/Intézkedés)
            // Mindkettő egyezzen: Status.DisplayType és Type.DisplayType
            // -------------------------------------------------
            if (displayType.HasValue)
            {
                var dt = (int)displayType.Value;

                query = query.Where(t =>
                    t.TaskStatusPM != null && t.TaskStatusPM.DisplayType == dt &&
                    t.TaskTypePM != null && t.TaskTypePM.DisplayType == dt
                );
            }

            // -------------------------------------------------
            // Filters
            // -------------------------------------------------
            if (statusId.HasValue)
                query = query.Where(t => t.TaskStatusPMId == statusId.Value);

            if (priorityId.HasValue)
                query = query.Where(t => t.TaskPriorityPMId == priorityId.Value);

            if (taskTypeId.HasValue)
                query = query.Where(t => t.TaskTypePMId == taskTypeId.Value);

            if (partnerId.HasValue)
                query = query.Where(t => t.PartnerId == partnerId.Value);

            if (relatedPartnerId.HasValue)
                query = query.Where(t => t.RelatedPartnerId == relatedPartnerId.Value);


            if (siteId.HasValue)
                query = query.Where(t => t.SiteId == siteId.Value);

            if (!string.IsNullOrWhiteSpace(assignedToId))
                query = query.Where(t => t.AssignedToId == assignedToId);

            // -------------------------------------------------
            // Date ranges
            // -------------------------------------------------
            if (dueDateFrom.HasValue)
                query = query.Where(t => t.DueDate >= dueDateFrom.Value.Date);

            if (dueDateTo.HasValue)
                query = query.Where(t => t.DueDate <= dueDateTo.Value.Date.AddDays(1).AddSeconds(-1));

            if (createdDateFrom.HasValue)
                query = query.Where(t => t.CreatedDate >= createdDateFrom.Value.Date);

            if (createdDateTo.HasValue)
                query = query.Where(t => t.CreatedDate <= createdDateTo.Value.Date.AddDays(1).AddSeconds(-1));

            // -------------------------------------------------
            // Search
            // (EF oldalon: ToLower().Contains -> működik, de nagy adaton drágább,
            // később érdemes FullText/Computed column/ILIKE jellegű optimalizálás)
            // -------------------------------------------------
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();

                query = query.Where(t =>
                    t.Title.ToLower().Contains(term) ||
                    (t.Description != null && t.Description.ToLower().Contains(term)) ||
                    (t.RelatedPartner != null && t.RelatedPartner.Name.ToLower().Contains(term)) ||
                    (t.AssignedTo != null && t.AssignedTo.UserName.ToLower().Contains(term)) ||
                    (t.CreatedBy != null && t.CreatedBy.UserName.ToLower().Contains(term)) ||
                    (t.Site != null && t.Site.SiteName.ToLower().Contains(term)) ||
                    (t.Site != null && t.Site.City.ToLower().Contains(term))
                );
            }

            // -------------------------------------------------
            // TotalCount
            // -------------------------------------------------
            var totalCount = await query.CountAsync();

            // -------------------------------------------------
            // Sorting
            // -------------------------------------------------
            query = sort.ToLowerInvariant() switch
            {
                "title" => order == "desc" ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title),
                "duedate" => order == "desc" ? query.OrderByDescending(t => t.DueDate) : query.OrderBy(t => t.DueDate),
                "status" => order == "desc" ? query.OrderByDescending(t => t.TaskStatusPM!.Name) : query.OrderBy(t => t.TaskStatusPM!.Name),
                "priority" => order == "desc" ? query.OrderByDescending(t => t.TaskPriorityPM!.Name) : query.OrderBy(t => t.TaskPriorityPM!.Name),
                "assignedto" => order == "desc" ? query.OrderByDescending(t => t.AssignedTo!.UserName) : query.OrderBy(t => t.AssignedTo!.UserName),
                "createddate" => order == "desc" ? query.OrderByDescending(t => t.CreatedDate) : query.OrderBy(t => t.CreatedDate),
                "partner" => order == "desc" ? query.OrderByDescending(t => t.Partner!.Name) : query.OrderBy(t => t.Partner!.Name),
                "relatedpartner" => order == "desc"
                                                ? query.OrderByDescending(t => t.RelatedPartner!.Name)
                                                : query.OrderBy(t => t.RelatedPartner!.Name),
                "site" => order == "desc" ? query.OrderByDescending(t => t.Site!.SiteName) : query.OrderBy(t => t.Site!.SiteName),
                "tasktype" => order == "desc" ? query.OrderByDescending(t => t.TaskTypePM!.TaskTypePMName) : query.OrderBy(t => t.TaskTypePM!.TaskTypePMName),
                _ => order == "desc" ? query.OrderByDescending(t => t.Id) : query.OrderBy(t => t.Id)
            };

            // -------------------------------------------------
            // Paging + DTO
            // -------------------------------------------------
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => MapToDto(t))
                .ToListAsync();

            return new PagedResult<TaskPMDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        // -----------------------------------------------------------------
        // MAP ENTITY → DTO
        // -----------------------------------------------------------------
        private static TaskPMDto MapToDto(TaskPM task)
        {
            return new TaskPMDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Description2 = task.Description2,
                IsActive = task.IsActive,

                TaskTypePMId = task.TaskTypePMId,
                TaskTypePMName = task.TaskTypePM?.TaskTypePMName,

                TaskStatusPMId = task.TaskStatusPMId,
                TaskStatusPMName = task.TaskStatusPM?.Name,
                ColorCode = task.TaskStatusPM?.ColorCode,

                // ✅ KOMMUNIKÁCIÓS MÓD (LOOKUP)
                TaskPMcomMethodID = task.TaskPMcomMethodID,
                TaskPMcomMethodName = task.TaskPMcomMethod != null ? task.TaskPMcomMethod.Nev : null,

                TaskPriorityPMId = task.TaskPriorityPMId,
                TaskPriorityPMName = task.TaskPriorityPM?.Name,
                PriorityColorCode = task.TaskPriorityPM?.PriorityColorCode,

                CommunicationTypeId = task.CommunicationTypeId,
                CommunicationTypeName = task.CommunicationType?.Name,
                CommunicationDescription = task.CommunicationDescription,

                DueDate = task.DueDate,
                EstimatedHours = task.EstimatedHours,
                ActualHours = task.ActualHours,
                ScheduledDate = task.ScheduledDate,
                OptionalDate1 = task.OptionalDate1,
                OptionalDate2 = task.OptionalDate2,

                CreatedById = task.CreatedById,
                CreatedByName = FullName(task.CreatedBy),

                CreatedDate = task.CreatedDate,

                AssignedToId = task.AssignedToId,
                AssignedToName = FullName(task.AssignedTo),

                UpdatedDate = task.UpdatedDate,
                CompletedDate = task.CompletedDate,

                PartnerId = task.PartnerId,
                PartnerName = task.Partner?.Name,

                RelatedPartnerId = task.RelatedPartnerId,
                RelatedPartnerName = task.RelatedPartner?.Name,

                SiteId = task.SiteId,
                SiteName = task.Site?.SiteName,
                City = task.Site?.City,

                ContactId = task.ContactId,
                ContactName = task.Contact != null
                    ? $"{task.Contact.FirstName} {task.Contact.LastName}".Trim()
                    : null,

                QuoteId = task.QuoteId,
                QuoteNumber = task.Quote?.QuoteNumber,

                OrderId = task.OrderId,
                OrderNumber = task.Order?.OrderNumber,

                CustomerCommunicationId = task.CustomerCommunicationId,
                CustomerCommunicationSubject = task.CustomerCommunication?.Subject,

                ResourceIds = task.TaskResourceAssignments.Select(ra => ra.ResourceId).ToList(),
                EmployeeIds = task.TaskEmployeeAssignments.Select(ea => ea.EmployeeId).ToList(),

                TaskHistories = task.TaskHistories
                    .Select(th => new TaskHistoryDto
                    {
                        TaskHistoryId = th.TaskHistoryId,
                        TaskPMId = th.TaskPMId,
                        ModifiedById = th.ModifiedById,
                        ModifiedByName = FullName(th.ModifiedBy),
                        ModifiedDate = th.ModifiedDate,
                        ChangeDescription = th.ChangeDescription
                    })
                    .OrderByDescending(th => th.ModifiedDate)
                    .ToList(),

                // ✅ Attachments itt üres, GetTaskByIdAsync tölti fel projectionnel (biztonságosan)
                Attachments = new List<TaskDocumentDto>()
            };
        }

        // -----------------------------------------------------------------
        // Attach existing document to task (MEGMARAD – ha már van DocumentId)
        // -----------------------------------------------------------------
        public async Task AttachDocumentAsync(int taskId, int documentId, string currentUserId, string? note = null)
        {
            if (!await _context.TaskPMs.AnyAsync(t => t.Id == taskId && t.IsActive))
                throw new KeyNotFoundException($"Task {taskId} not found.");

            if (!await _context.Documents.AnyAsync(d => d.DocumentId == documentId))
                throw new KeyNotFoundException($"Document {documentId} not found.");

            var alreadyAttached = await _context.TaskDocumentLinks
                .AnyAsync(x => x.TaskId == taskId && x.DocumentId == documentId);

            if (alreadyAttached)
                return;

            var link = new TaskDocumentLink
            {
                TaskId = taskId,
                DocumentId = documentId,
                LinkedDate = DateTime.UtcNow,
                LinkedById = currentUserId,
                Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim()
            };

            _context.TaskDocumentLinks.Add(link);
            await _context.SaveChangesAsync();
        }

        // -----------------------------------------------------------------
        // ✅ NEW: Attach LINK to task (Document rekord csak meta+FilePath, NINCS upload)
        // -----------------------------------------------------------------
        public async Task AttachLinkAsync(
            int taskId,
            string filePath,
            string currentUserId,
            string? fileName = null,
            string? note = null)
        {
            if (!await _context.TaskPMs.AnyAsync(t => t.Id == taskId && t.IsActive))
                throw new KeyNotFoundException($"Task {taskId} not found.");

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ValidationException("FilePath is required.");

            var normalizedPath = filePath.Trim();

            // FileName ha nincs megadva: próbáljuk a path végéből kinyerni
            var name = (fileName ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                try
                {
                    name = Path.GetFileName(normalizedPath.TrimEnd('\\', '/'));
                }
                catch { /* ignore */ }

                if (string.IsNullOrWhiteSpace(name))
                    name = "Hivatkozás";
            }

            // 🔎 opcionális: ha ugyanaz a FilePath már létezik a Documents-ben, akkor azt reuse-oljuk
            // (ha nem akarod: töröld ezt a blokkot, és mindig új Document rekord jön létre)
            var existingDoc = await _context.Documents
                .FirstOrDefaultAsync(d => d.FilePath == normalizedPath && d.FileName == name);

            var doc = existingDoc;
            if (doc == null)
            {
                doc = new Document
                {
                    FileName = name,
                    FilePath = normalizedPath
                };

                _context.Documents.Add(doc);
                await _context.SaveChangesAsync(); // doc.DocumentId kell
            }

            // ne duplázzunk taskon belül
            var alreadyAttached = await _context.TaskDocumentLinks
                .AnyAsync(x => x.TaskId == taskId && x.DocumentId == doc.DocumentId);

            if (alreadyAttached)
                return;

            var link = new TaskDocumentLink
            {
                TaskId = taskId,
                DocumentId = doc.DocumentId,
                LinkedDate = DateTime.UtcNow,
                LinkedById = currentUserId,
                Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim()
            };

            _context.TaskDocumentLinks.Add(link);
            await _context.SaveChangesAsync();
        }

        // -----------------------------------------------------------------
        // Remove document link from task (MEGMARAD)
        // -----------------------------------------------------------------
        public async Task RemoveDocumentAsync(int taskId, int documentLinkId, string currentUserId)
        {
            var link = await _context.TaskDocumentLinks
                .FirstOrDefaultAsync(x => x.Id == documentLinkId && x.TaskId == taskId);

            if (link == null)
                throw new KeyNotFoundException($"Attachment {documentLinkId} not found on task {taskId}");

            _context.TaskDocumentLinks.Remove(link);
            await _context.SaveChangesAsync();
        }

        // -----------------------------------------------------------------
        // Optional: Attach multiple existing DocumentIds at once (MEGMARAD)
        // -----------------------------------------------------------------
        public async Task AttachDocumentsAsync(int taskId, List<int> documentIds, string currentUserId, string? note = null)
        {
            foreach (var docId in (documentIds ?? new List<int>()).Distinct())
            {
                await AttachDocumentAsync(taskId, docId, currentUserId, note);
            }
        }

        // -----------------------------------------------------------------
        // ✅ Optional: Attach multiple LINKS at once
        // -----------------------------------------------------------------
        public async Task AttachLinksAsync(int taskId, List<string> filePaths, string currentUserId, string? note = null)
        {
            foreach (var p in (filePaths ?? new List<string>()))
            {
                if (string.IsNullOrWhiteSpace(p)) continue;
                await AttachLinkAsync(taskId, p, currentUserId, fileName: null, note: note);
            }
        }

    }

    // -----------------------------------------------------------------
    // PAGED RESULT
    // -----------------------------------------------------------------
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
