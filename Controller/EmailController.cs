using System;
using System.Collections.Generic;
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
    public class EmailController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;
        private readonly ILogger<EmailController> _logger;

        public EmailController(
            ApplicationDbContext context,
            EmailService emailService,
            ILogger<EmailController> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        // =========================================================
        // EMAIL TEMPLATE ENDPOINTS
        // =========================================================

        [HttpGet("templates")]
        public async Task<ActionResult<IEnumerable<EmailTemplateDto>>> GetTemplates(
            [FromQuery] string? search = null,
            [FromQuery] bool? isActive = null)
        {
            var query = _context.EmailTemplates.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(x =>
                    x.Name.Contains(search) ||
                    x.Subject.Contains(search) ||
                    (x.Description != null && x.Description.Contains(search)));
            }

            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
            }

var result = await query
    .OrderBy(x => x.Name)
    .Select(x => new EmailTemplateDto
    {
        Id = x.Id,
        Name = x.Name,
        Subject = x.Subject,
        Body = x.Body,
        IsActive = x.IsActive,
        Description = x.Description,
        Variables = x.Variables
            .OrderBy(v => v.SortOrder)
            .ThenBy(v => v.VariableName)
            .Select(v => new EmailTemplateVariableDto
            {
                Id = v.Id,
                EmailTemplateId = v.EmailTemplateId,
                VariableName = v.VariableName,
                Description = v.Description,
                ExampleValue = v.ExampleValue,
                SortOrder = v.SortOrder
            })
            .ToList()
    })
    .ToListAsync();

            return Ok(result);
        }

        [HttpGet("templates/{id:int}")]
        public async Task<ActionResult<EmailTemplateDto>> GetTemplate(int id)
        {
            var entity = await _context.EmailTemplates.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return NotFound();

            var dto = new EmailTemplateDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Subject = entity.Subject,
                Body = entity.Body,
                IsActive = entity.IsActive,
                Description = entity.Description
            };

            return Ok(dto);
        }

        [HttpPost("templates")]
        public async Task<ActionResult<EmailTemplateDto>> CreateTemplate([FromBody] CreateEmailTemplateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var exists = await _context.EmailTemplates
                .AnyAsync(x => x.Name == dto.Name);

            if (exists)
            {
                return BadRequest(new { message = $"Email template already exists with name: {dto.Name}" });
            }

            var userName = User?.Identity?.Name ?? "System";

            var entity = new EmailTemplate
            {
                Name = dto.Name.Trim(),
                Subject = dto.Subject,
                Body = dto.Body,
                IsActive = dto.IsActive,
                Description = dto.Description,
                CreatedBy = userName,
                CreatedDate = DateTime.UtcNow
            };

            _context.EmailTemplates.Add(entity);
            await _context.SaveChangesAsync();

            var result = new EmailTemplateDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Subject = entity.Subject,
                Body = entity.Body,
                IsActive = entity.IsActive,
                Description = entity.Description
            };

            return CreatedAtAction(nameof(GetTemplate), new { id = entity.Id }, result);
        }

[HttpPut("templates/{id:int}")]
public async Task<ActionResult<EmailTemplateDto>> UpdateTemplate(int id, [FromBody] UpdateEmailTemplateDto dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var entity = await _context.EmailTemplates.FirstOrDefaultAsync(x => x.Id == id);

    if (entity == null)
        return NotFound();

    var userName = User?.Identity?.Name ?? "System";

    entity.Name = dto.Name.Trim();
    entity.Subject = dto.Subject;
    entity.Body = dto.Body;
    entity.IsActive = dto.IsActive;
    entity.Description = dto.Description;
    entity.ModifiedBy = userName;
    entity.ModifiedDate = DateTime.UtcNow;

    await _context.SaveChangesAsync();

    var result = new EmailTemplateDto
    {
        Id = entity.Id,
        Name = entity.Name,
        Subject = entity.Subject,
        Body = entity.Body,
        IsActive = entity.IsActive,
        Description = entity.Description
    };

    return Ok(result);
}

        [HttpDelete("templates/{id:int}")]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            var entity = await _context.EmailTemplates.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return NotFound();

            _context.EmailTemplates.Remove(entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =========================================================
        // EMAIL QUEUE ENDPOINTS
        // =========================================================

        [HttpGet("queue")]
        public async Task<ActionResult<IEnumerable<EmailQueueDto>>> GetQueue(
            [FromQuery] string? search = null,
            [FromQuery] string? status = null,
            [FromQuery] string? templateName = null,
            [FromQuery] string? relatedEntityType = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 50;
            if (pageSize > 200) pageSize = 200;

            var query = _context.EmailQueues.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(x =>
                    x.ToEmail.Contains(search) ||
                    x.Subject.Contains(search) ||
                    (x.TemplateName != null && x.TemplateName.Contains(search)) ||
                    (x.RelatedEntityType != null && x.RelatedEntityType.Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(x => x.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(templateName))
            {
                query = query.Where(x => x.TemplateName == templateName);
            }

            if (!string.IsNullOrWhiteSpace(relatedEntityType))
            {
                query = query.Where(x => x.RelatedEntityType == relatedEntityType);
            }

            var totalCount = await query.CountAsync();

            var result = await query
                .OrderByDescending(x => x.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new EmailQueueDto
                {
                    Id = x.Id,
                    ToEmail = x.ToEmail,
                    Subject = x.Subject,
                    Status = x.Status,
                    CreatedDate = x.CreatedDate,
                    SentDate = x.SentDate,
                    RetryCount = x.RetryCount
                })
                .ToListAsync();

            return Ok(new
            {
                totalCount,
                page,
                pageSize,
                items = result
            });
        }

        [HttpGet("queue/{id:long}")]
        public async Task<ActionResult<EmailQueue>> GetQueueItem(long id)
        {
            var entity = await _context.EmailQueues
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return NotFound();

            return Ok(entity);
        }

        [HttpPost("queue")]
        public async Task<ActionResult> QueueRawEmail([FromBody] QueueEmailRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userName = User?.Identity?.Name ?? "System";

            var id = await _emailService.QueueRawEmailAsync(
                toEmail: dto.ToEmail,
                subject: dto.Subject,
                body: dto.Body,
                ccEmail: dto.CcEmail,
                bccEmail: dto.BccEmail,
                relatedEntityType: dto.RelatedEntityType,
                relatedEntityId: dto.RelatedEntityId,
                createdBy: userName);

            return Ok(new
            {
                message = "Email queued successfully.",
                emailQueueId = id
            });
        }

        [HttpPost("queue/template")]
        public async Task<ActionResult> QueueTemplateEmail(
            [FromQuery] string templateName,
            [FromQuery] string toEmail,
            [FromBody] Dictionary<string, string> placeholders)
        {
            if (string.IsNullOrWhiteSpace(templateName))
                return BadRequest(new { message = "templateName is required." });

            if (string.IsNullOrWhiteSpace(toEmail))
                return BadRequest(new { message = "toEmail is required." });

            var userName = User?.Identity?.Name ?? "System";

            var id = await _emailService.QueueByTemplateAsync(
                templateName: templateName,
                toEmail: toEmail,
                placeholders: placeholders ?? new Dictionary<string, string>(),
                createdBy: userName);

            if (id == null)
            {
                return BadRequest(new { message = $"Template not found or inactive: {templateName}" });
            }

            return Ok(new
            {
                message = "Template email queued successfully.",
                emailQueueId = id
            });
        }

        [HttpGet("queue/{id:long}/logs")]
        public async Task<ActionResult<IEnumerable<EmailLogDto>>> GetQueueLogs(long id)
        {
            var exists = await _context.EmailQueues.AnyAsync(x => x.Id == id);
            if (!exists)
                return NotFound(new { message = "Email queue item not found." });

            var logs = await _context.EmailLogs
                .Where(x => x.EmailQueueId == id)
                .OrderByDescending(x => x.EventDate)
                .Select(x => new EmailLogDto
                {
                    Id = x.Id,
                    EmailQueueId = x.EmailQueueId,
                    EventType = x.EventType,
                    ToEmail = x.ToEmail,
                    Subject = x.Subject,
                    EventDate = x.EventDate,
                    Message = x.Message
                })
                .ToListAsync();

            return Ok(logs);
        }

        // =========================================================
        // EMAIL LOG ENDPOINTS
        // =========================================================

        [HttpGet("logs")]
        public async Task<ActionResult<IEnumerable<EmailLogDto>>> GetLogs(
            [FromQuery] string? search = null,
            [FromQuery] string? eventType = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 50;
            if (pageSize > 200) pageSize = 200;

            var query = _context.EmailLogs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(x =>
                    (x.ToEmail != null && x.ToEmail.Contains(search)) ||
                    (x.Subject != null && x.Subject.Contains(search)) ||
                    (x.Message != null && x.Message.Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(eventType))
            {
                query = query.Where(x => x.EventType == eventType);
            }

            var totalCount = await query.CountAsync();

            var result = await query
                .OrderByDescending(x => x.EventDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new EmailLogDto
                {
                    Id = x.Id,
                    EmailQueueId = x.EmailQueueId,
                    EventType = x.EventType,
                    ToEmail = x.ToEmail,
                    Subject = x.Subject,
                    EventDate = x.EventDate,
                    Message = x.Message
                })
                .ToListAsync();

            return Ok(new
            {
                totalCount,
                page,
                pageSize,
                items = result
            });
        }

        // =========================================================
        // OPTIONAL ADMIN ACTIONS
        // =========================================================

        [HttpPost("queue/{id:long}/retry")]
        public async Task<IActionResult> RetryQueueItem(long id)
        {
            var entity = await _context.EmailQueues.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return NotFound();

            entity.Status = "Pending";
            entity.ErrorMessage = null;
            entity.ScheduledAt = DateTime.UtcNow;
            entity.LastAttemptDate = null;

            await _context.SaveChangesAsync();

            _context.EmailLogs.Add(new EmailLog
            {
                EmailQueueId = entity.Id,
                EventType = "Retried",
                ToEmail = entity.ToEmail,
                Subject = entity.Subject,
                Message = "Email re-queued manually.",
                PerformedBy = User?.Identity?.Name ?? "System",
                EventDate = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return Ok(new { message = "Email re-queued successfully." });
        }

        [HttpPost("queue/{id:long}/cancel")]
        public async Task<IActionResult> CancelQueueItem(long id)
        {
            var entity = await _context.EmailQueues.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return NotFound();

            entity.Status = "Cancelled";

            await _context.SaveChangesAsync();

            _context.EmailLogs.Add(new EmailLog
            {
                EmailQueueId = entity.Id,
                EventType = "Cancelled",
                ToEmail = entity.ToEmail,
                Subject = entity.Subject,
                Message = "Email cancelled manually.",
                PerformedBy = User?.Identity?.Name ?? "System",
                EventDate = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return Ok(new { message = "Email cancelled successfully." });
        }

        [HttpGet("templates/{id:int}/variables")]
public async Task<ActionResult<IEnumerable<EmailTemplateVariableDto>>> GetTemplateVariables(int id)
{
    var exists = await _context.EmailTemplates.AnyAsync(x => x.Id == id);
    if (!exists)
        return NotFound(new { message = "Template not found." });

    var result = await _context.EmailTemplateVariables
        .Where(x => x.EmailTemplateId == id)
        .OrderBy(x => x.SortOrder)
        .ThenBy(x => x.VariableName)
        .Select(x => new EmailTemplateVariableDto
        {
            Id = x.Id,
            EmailTemplateId = x.EmailTemplateId,
            VariableName = x.VariableName,
            Description = x.Description,
            ExampleValue = x.ExampleValue,
            SortOrder = x.SortOrder
        })
        .ToListAsync();

    return Ok(result);
}

[HttpPost("templates/{id:int}/variables")]
public async Task<ActionResult<EmailTemplateVariableDto>> CreateTemplateVariable(int id, [FromBody] EmailTemplateVariableDto dto)
{
    var template = await _context.EmailTemplates.FirstOrDefaultAsync(x => x.Id == id);
    if (template == null)
        return NotFound(new { message = "Template not found." });

    var exists = await _context.EmailTemplateVariables
        .AnyAsync(x => x.EmailTemplateId == id && x.VariableName == dto.VariableName);

    if (exists)
        return BadRequest(new { message = $"Variable already exists: {dto.VariableName}" });

    var entity = new EmailTemplateVariable
    {
        EmailTemplateId = id,
        VariableName = dto.VariableName.Trim(),
        Description = dto.Description,
        ExampleValue = dto.ExampleValue,
        SortOrder = dto.SortOrder
    };

    _context.EmailTemplateVariables.Add(entity);
    await _context.SaveChangesAsync();

    dto.Id = entity.Id;
    dto.EmailTemplateId = entity.EmailTemplateId;

    return Ok(dto);
}

[HttpDelete("variables/{id:int}")]
public async Task<IActionResult> DeleteTemplateVariable(int id)
{
    var entity = await _context.EmailTemplateVariables.FirstOrDefaultAsync(x => x.Id == id);
    if (entity == null)
        return NotFound();

    _context.EmailTemplateVariables.Remove(entity);
    await _context.SaveChangesAsync();

    return NoContent();
}

[HttpPut("variables/{id:int}")]
public async Task<ActionResult<EmailTemplateVariableDto>> UpdateTemplateVariable(int id, [FromBody] EmailTemplateVariableDto dto)
{
    var entity = await _context.EmailTemplateVariables.FirstOrDefaultAsync(x => x.Id == id);
    if (entity == null)
        return NotFound(new { message = "Variable not found." });

    var exists = await _context.EmailTemplateVariables
        .AnyAsync(x => x.Id != id &&
                       x.EmailTemplateId == entity.EmailTemplateId &&
                       x.VariableName == dto.VariableName);

    if (exists)
        return BadRequest(new { message = $"Variable already exists: {dto.VariableName}" });

    entity.VariableName = dto.VariableName.Trim();
    entity.Description = dto.Description;
    entity.ExampleValue = dto.ExampleValue;
    entity.SortOrder = dto.SortOrder;

    await _context.SaveChangesAsync();

    return Ok(new EmailTemplateVariableDto
    {
        Id = entity.Id,
        EmailTemplateId = entity.EmailTemplateId,
        VariableName = entity.VariableName,
        Description = entity.Description,
        ExampleValue = entity.ExampleValue,
        SortOrder = entity.SortOrder
    });
}

    }
}