using Cloud9_2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Cloud9_2.Interceptors
{
    public class GenericAuditInterceptor : SaveChangesInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GenericAuditInterceptor> _logger;

        private static readonly Dictionary<Type, string> AuditedEntities = new()
        {
            { typeof(Partner), "Partner" },
            { typeof(TaskPM), "TaskPM" },

            { typeof(CustomerCommunication), "CustomerCommunication" },
            { typeof(CommunicationResponsible), "CommunicationResponsible" },

            { typeof(Document), "Document" }
        };

        private static readonly HashSet<string> ExcludedProperties = new()
        {
            "CreatedDate", "ModifiedDate", "IsActive", "RowVersion",
            "UpdatedDate" // ✅ zaj csökkentése
        };

        public GenericAuditInterceptor(IHttpContextAccessor httpContextAccessor, ILogger<GenericAuditInterceptor> logger)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User
                ?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
        }

        private async Task<string> GetUserNameAsync(DbContext context, string? userId)
        {
            if (string.IsNullOrEmpty(userId)) return "Rendszer";

            var user = await context.Set<ApplicationUser>()
                .Where(u => u.Id == userId)
                .Select(u => u.UserName)
                .FirstOrDefaultAsync();

            return user ?? "Ismeretlen felhasználó";
        }

        private async Task<string> GetPartnerNameAsync(DbContext context, int? partnerId)
        {
            if (!partnerId.HasValue) return "—";

            var name = await context.Set<Partner>()
                .Where(p => p.PartnerId == partnerId.Value)
                .Select(p => p.CompanyName ?? p.Name ?? "")
                .FirstOrDefaultAsync();

            return string.IsNullOrWhiteSpace(name) ? $"#{partnerId.Value}" : name;
        }

        private async Task<string> GetSiteNameAsync(DbContext context, int? siteId)
        {
            if (!siteId.HasValue) return "—";

            var name = await context.Set<Site>()
                .Where(s => s.SiteId == siteId.Value)
                .Select(s => s.SiteName ?? "")
                .FirstOrDefaultAsync();

            return string.IsNullOrWhiteSpace(name) ? $"#{siteId.Value}" : name;
        }

        private async Task<string> GetCommunicationTypeNameAsync(DbContext context, int typeId)
        {
            if (typeId <= 0) return "—";

            var name = await context.Set<CommunicationType>()
                .Where(t => t.CommunicationTypeId == typeId)
                .Select(t => t.Name)
                .FirstOrDefaultAsync();

            return string.IsNullOrWhiteSpace(name) ? $"#{typeId}" : name;
        }

        private async Task<string> GetCommunicationStatusNameAsync(DbContext context, int statusId)
        {
            if (statusId <= 0) return "—";

            var name = await context.Set<CommunicationStatus>()
                .Where(s => s.StatusId == statusId)
                .Select(s => s.Name)
                .FirstOrDefaultAsync();

            return string.IsNullOrWhiteSpace(name) ? $"#{statusId}" : name;
        }

        private async Task<string> GetDocumentTypeNameAsync(DbContext context, int? typeId)
        {
            if (!typeId.HasValue || typeId.Value <= 0) return "—";

            var name = await context.Set<DocumentType>()
                .Where(t => t.DocumentTypeId == typeId.Value)
                .Select(t => t.Name)
                .FirstOrDefaultAsync();

            return string.IsNullOrWhiteSpace(name) ? $"#{typeId.Value}" : name;
        }

        // ✅ TaskPM Priority név
        private async Task<string> GetTaskPriorityNameAsync(DbContext context, int? priorityId)
        {
            if (!priorityId.HasValue || priorityId.Value <= 0) return "—";

            var name = await context.Set<TaskPriorityPM>()
                .Where(x => x.TaskPriorityPMId == priorityId.Value)
                .Select(x => x.Name)
                .FirstOrDefaultAsync();

            return string.IsNullOrWhiteSpace(name) ? $"#{priorityId.Value}" : name;
        }

        private async Task<string> GetTaskTypeNameAsync(DbContext context, int? typeId)
        {
            if (!typeId.HasValue || typeId.Value <= 0) return "—";

            var name = await context.Set<TaskTypePM>() // <-- ha nálad más a típus neve, ezt cseréld
                .Where(x => x.TaskTypePMId == typeId.Value)
                .Select(x => x.TaskTypePMName)
                .FirstOrDefaultAsync();

            return string.IsNullOrWhiteSpace(name) ? $"#{typeId.Value}" : name;
        }

        // ✅ TaskPM Status név
        private async Task<string> GetTaskStatusNameAsync(DbContext context, int? statusId)
        {
            if (!statusId.HasValue || statusId.Value <= 0) return "—";

            var name = await context.Set<TaskStatusPM>()
                .Where(x => x.TaskStatusPMId == statusId.Value)
                .Select(x => x.Name)
                .FirstOrDefaultAsync();

            return string.IsNullOrWhiteSpace(name) ? $"#{statusId.Value}" : name;
        }

        // ✅ Helper: string ("null"/""/whitespace) -> int?
        private static int? ToNullableInt(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;

            s = s.Trim();
            if (string.Equals(s, "null", StringComparison.OrdinalIgnoreCase)) return null;

            return int.TryParse(s, out var n) ? n : (int?)null;
        }

// ✅ TaskPM kommunikációs mód név (TaskPMcomMethodID) -> TaskPMcomMethod
private async Task<string> GetTaskPmComMethodNameAsync(DbContext context, int? methodId)
{
    if (!methodId.HasValue || methodId.Value <= 0) return "—";

    var name = await context.Set<TaskPMcomMethod>()
        .AsNoTracking()
        .Where(m => m.TaskPMcomMethodID == methodId.Value)
        .Select(m => m.Nev)
        .FirstOrDefaultAsync();

    return string.IsNullOrWhiteSpace(name) ? $"#{methodId.Value}" : name;
}



        private async Task AuditChangesAsync(DbContext context)
        {
            var userId = GetCurrentUserId();
            var userName = await GetUserNameAsync(context, userId);
            var now = DateTime.UtcNow;

                // 🔥 Cache-ek (egy mentésen belül ne kérdezzen le mindent 10x)
    var taskPmComMethodCache = new Dictionary<int, string>();

    async Task<string> GetTaskPmComMethodNameCached(int? id)
    {
        if (!id.HasValue || id.Value <= 0) return "—";

        if (taskPmComMethodCache.TryGetValue(id.Value, out var cached))
            return cached;

        var name = await GetTaskPmComMethodNameAsync(context, id);
        taskPmComMethodCache[id.Value] = name;

        return name;
    }


            var auditEntries = new List<AuditLog>();

            foreach (var entry in context.ChangeTracker.Entries())
            {
                var clrType = entry.Metadata.ClrType; // ✅ proxy-biztos
                if (!AuditedEntities.TryGetValue(clrType, out var entityTypeName))
                    continue;

                int entityId = GetEntityId(entry);

                switch (entry.State)
                {
                    case EntityState.Added:
                    {
                        if (entry.Entity is Partner)
                        {
                            auditEntries.Add(new AuditLog
                            {
                                EntityType = entityTypeName,
                                EntityId = entityId,
                                Action = "Created",
                                ChangedById = userId,
                                ChangedByName = userName,
                                ChangedAt = now,
                                Changes = "Új partner létrehozva."
                            });
                            break;
                        }

                        // ✅ TaskPM Created (olvasható)
                        if (entry.Entity is TaskPM tNew)
                        {
                            var assignedName = await GetUserNameAsync(context, tNew.AssignedToId);
                            var prioName = await GetTaskPriorityNameAsync(context, tNew.TaskPriorityPMId);
                            var statusName = await GetTaskStatusNameAsync(context, tNew.TaskStatusPMId);

                            auditEntries.Add(new AuditLog
                            {
                                EntityType = entityTypeName,
                                EntityId = entityId,
                                Action = "Created",
                                ChangedById = userId,
                                ChangedByName = userName,
                                ChangedAt = now,
                                Changes =
                                    $"Új feladat létrehozva. " +
                                    $"Cím: {tNew.Title ?? "—"}; Felelős: {assignedName}; Prioritás: {prioName}; Státusz: {statusName}"
                            });
                            break;
                        }

                        if (entry.Entity is CustomerCommunication ccNew)
                        {
                            var typeName = await GetCommunicationTypeNameAsync(context, ccNew.CommunicationTypeId);
                            var statusName = await GetCommunicationStatusNameAsync(context, ccNew.StatusId);
                            var partnerName = await GetPartnerNameAsync(context, ccNew.PartnerId);
                            var siteName = await GetSiteNameAsync(context, ccNew.SiteId);

                            auditEntries.Add(new AuditLog
                            {
                                EntityType = entityTypeName,
                                EntityId = entityId,
                                Action = "Created",
                                ChangedById = userId,
                                ChangedByName = userName,
                                ChangedAt = now,
                                Changes =
                                    $"Új kommunikáció létrehozva. " +
                                    $"Típus: {typeName}; Státusz: {statusName}; Partner: {partnerName}; Telephely: {siteName}; " +
                                    $"Tárgy: {ccNew.Subject ?? "—"}"
                            });
                            break;
                        }

                        if (entry.Entity is CommunicationResponsible crNew)
                        {
                            var responsibleName = await GetUserNameAsync(context, crNew.ResponsibleId);
                            auditEntries.Add(new AuditLog
                            {
                                EntityType = entityTypeName,
                                EntityId = entityId,
                                Action = "Created",
                                ChangedById = userId,
                                ChangedByName = userName,
                                ChangedAt = now,
                                Changes = $"Felelős kiosztva. Kommunikáció: #{crNew.CustomerCommunicationId}; Felelős: {responsibleName}"
                            });
                            break;
                        }

                        if (entry.Entity is Document dNew)
                        {
                            var partnerName = await GetPartnerNameAsync(context, dNew.PartnerId);
                            var siteName = await GetSiteNameAsync(context, dNew.SiteId);
                            var docTypeName = await GetDocumentTypeNameAsync(context, dNew.DocumentTypeId);
                            var statusText = dNew.Status.ToString();

                            auditEntries.Add(new AuditLog
                            {
                                EntityType = entityTypeName,
                                EntityId = entityId,
                                Action = "Created",
                                ChangedById = userId,
                                ChangedByName = userName,
                                ChangedAt = now,
                                Changes =
                                    $"Új dokumentum létrehozva. " +
                                    $"Fájlnév: {dNew.FileName ?? "—"}; Típus: {docTypeName}; Státusz: {statusText}; " +
                                    $"Partner: {partnerName}; Telephely: {siteName}"
                            });
                            break;
                        }

                        auditEntries.Add(new AuditLog
                        {
                            EntityType = entityTypeName,
                            EntityId = entityId,
                            Action = "Created",
                            ChangedById = userId,
                            ChangedByName = userName,
                            ChangedAt = now,
                            Changes = "Új rekord létrehozva."
                        });
                        break;
                    }

                    case EntityState.Modified:
                    {
                        var changes = new List<string>();

                        foreach (var prop in entry.Properties)
                        {
                            if (ExcludedProperties.Contains(prop.Metadata.Name))
                                continue;

                            var oldValue = entry.OriginalValues[prop.Metadata]?.ToString() ?? "null";
                            var newValue = entry.CurrentValues[prop.Metadata]?.ToString() ?? "null";

                            if (oldValue == newValue)
                                continue;

                            if (entry.Entity is Partner)
                            {
                                string displayName = prop.Metadata.Name switch
                                {
                                    "Name" => "Név",
                                    "CompanyName" => "Cégnév",
                                    "TaxId" => "Adószám",
                                    "Email" => "E-mail",
                                    "Phone" => "Telefon",
                                    "Website" => "Weboldal",
                                    "StatusId" => "Státusz",
                                    "PartnerTypeId" => "Partner típus",
                                    "BillingAddress" => "Számlázási cím",
                                    "BillingName" => "Számlázási név",
                                    "BillingTaxId" => "Számlázási adószám",
                                    "Notes" => "Jegyzetek",
                                    _ => prop.Metadata.Name
                                };

                                changes.Add($"{displayName}: {oldValue} → {newValue}");
                                continue;
                            }

                            // ✅ TaskPM Modified: ID-k -> nevek
                            if (entry.Entity is TaskPM)
                            {
                                string displayName = prop.Metadata.Name switch
                                {
                                    "Title" => "Cím",
                                    "Description" => "Leírás",
                                    "DueDate" => "Határidő",
                                    "PartnerId" => "Partner",
                                    "SiteId" => "Telephely",
                                    "AssignedToId" => "Felelős",
                                    "TaskPriorityPMId" => "Prioritás",
                                    "TaskStatusPMId" => "Státusz",
                                    "TaskTypePMId" => "Feladat típusa",
                                    "TaskPMcomMethodID" => "Kommunikáció mód",
                                    _ => prop.Metadata.Name
                                };

                                if (prop.Metadata.Name == "TaskTypePMId")
                                {
                                    var oldId = ToNullableInt(oldValue);
                                    var newId = ToNullableInt(newValue);

                                    var oldName = await GetTaskTypeNameAsync(context, oldId);
                                    var newName = await GetTaskTypeNameAsync(context, newId);

                                    changes.Add($"{displayName}: {oldName} → {newName}");
                                    continue;
                                }

                                if (prop.Metadata.Name == "AssignedToId")
                                {
                                    var oldName = await GetUserNameAsync(context, oldValue == "null" ? null : oldValue);
                                    var newName = await GetUserNameAsync(context, newValue == "null" ? null : newValue);
                                    changes.Add($"{displayName}: {oldName} → {newName}");
                                    continue;
                                }

                                if (prop.Metadata.Name == "TaskPriorityPMId")
                                {
                                    var oldId = ToNullableInt(oldValue);
                                    var newId = ToNullableInt(newValue);

                                    var oldName = await GetTaskPriorityNameAsync(context, oldId);
                                    var newName = await GetTaskPriorityNameAsync(context, newId);

                                    changes.Add($"{displayName}: {oldName} → {newName}");
                                    continue;
                                }

                                if (prop.Metadata.Name == "TaskStatusPMId")
                                {
                                    var oldId = ToNullableInt(oldValue);
                                    var newId = ToNullableInt(newValue);

                                    var oldName = await GetTaskStatusNameAsync(context, oldId);
                                    var newName = await GetTaskStatusNameAsync(context, newId);

                                    changes.Add($"{displayName}: {oldName} → {newName}");
                                    continue;
                                }

                                // ✅ ÚJ: PartnerId -> Partner név
                                if (prop.Metadata.Name == "PartnerId")
                                {
                                    var oldId = ToNullableInt(oldValue);
                                    var newId = ToNullableInt(newValue);

                                    var oldName = await GetPartnerNameAsync(context, oldId);
                                    var newName = await GetPartnerNameAsync(context, newId);

                                    changes.Add($"{displayName}: {oldName} → {newName}");
                                    continue;
                                }

                                // ✅ ÚJ: SiteId -> Telephely név
                                if (prop.Metadata.Name == "SiteId")
                                {
                                    var oldId = ToNullableInt(oldValue);
                                    var newId = ToNullableInt(newValue);

                                    var oldName = await GetSiteNameAsync(context, oldId);
                                    var newName = await GetSiteNameAsync(context, newId);

                                    changes.Add($"{displayName}: {oldName} → {newName}");
                                    continue;
                                }

                                // ✅ ÚJ: TaskPMcomMethodID -> kommunikáció mód név
if (prop.Metadata.Name == "TaskPMcomMethodID")
{
    var oldId = ToNullableInt(oldValue);
    var newId = ToNullableInt(newValue);

    var oldName = await GetTaskPmComMethodNameCached(oldId);
    var newName = await GetTaskPmComMethodNameCached(newId);

    changes.Add($"{displayName}: {oldName} → {newName}");
    continue;
}


                                changes.Add($"{displayName}: {oldValue} → {newValue}");
                                continue;
                            }

                            if (entry.Entity is CustomerCommunication)
                            {
                                string displayName = prop.Metadata.Name switch
                                {
                                    "Subject" => "Tárgy",
                                    "Note" => "Tartalom",
                                    "Metadata" => "Megjegyzések",
                                    "Date" => "Dátum",
                                    "PartnerId" => "Partner",
                                    "SiteId" => "Telephely",
                                    "CommunicationTypeId" => "Típus",
                                    "StatusId" => "Státusz",
                                    "AgentId" => "Ügyintéző",
                                    _ => prop.Metadata.Name
                                };

                                if (prop.Metadata.Name == "PartnerId")
                                {
                                    var oldP = ToNullableInt(oldValue);
                                    var newP = ToNullableInt(newValue);

                                    var oldName = await GetPartnerNameAsync(context, oldP);
                                    var newName = await GetPartnerNameAsync(context, newP);

                                    changes.Add($"{displayName}: {oldName} → {newName}");
                                    continue;
                                }

                                if (prop.Metadata.Name == "SiteId")
                                {
                                    var oldS = ToNullableInt(oldValue);
                                    var newS = ToNullableInt(newValue);

                                    var oldName = await GetSiteNameAsync(context, oldS);
                                    var newName = await GetSiteNameAsync(context, newS);

                                    changes.Add($"{displayName}: {oldName} → {newName}");
                                    continue;
                                }

                                if (prop.Metadata.Name == "CommunicationTypeId")
                                {
                                    var oldT = ToNullableInt(oldValue) ?? 0;
                                    var newT = ToNullableInt(newValue) ?? 0;

                                    var oldName = await GetCommunicationTypeNameAsync(context, oldT);
                                    var newName = await GetCommunicationTypeNameAsync(context, newT);

                                    changes.Add($"{displayName}: {oldName} → {newName}");
                                    continue;
                                }

                                if (prop.Metadata.Name == "StatusId")
                                {
                                    var oldSt = ToNullableInt(oldValue) ?? 0;
                                    var newSt = ToNullableInt(newValue) ?? 0;

                                    var oldName = await GetCommunicationStatusNameAsync(context, oldSt);
                                    var newName = await GetCommunicationStatusNameAsync(context, newSt);

                                    changes.Add($"{displayName}: {oldName} → {newName}");
                                    continue;
                                }

                                if (prop.Metadata.Name == "AgentId")
                                {
                                    var oldName = await GetUserNameAsync(context, oldValue == "null" ? null : oldValue);
                                    var newName = await GetUserNameAsync(context, newValue == "null" ? null : newValue);
                                    changes.Add($"{displayName}: {oldName} → {newName}");
                                    continue;
                                }

                                changes.Add($"{displayName}: {oldValue} → {newValue}");
                                continue;
                            }

                            if (entry.Entity is CommunicationResponsible && prop.Metadata.Name == "ResponsibleId")
                            {
                                var oldName = await GetUserNameAsync(context, oldValue == "null" ? null : oldValue);
                                var newName = await GetUserNameAsync(context, newValue == "null" ? null : newValue);
                                changes.Add($"Felelős: {oldName} → {newName}");
                                continue;
                            }

                            if (entry.Entity is Document)
                            {
                                string displayName = prop.Metadata.Name switch
                                {
                                    "FileName" => "Fájlnév",
                                    "DocumentTypeId" => "Dokumentumtípus",
                                    "Status" => "Státusz",
                                    "PartnerId" => "Partner",
                                    "SiteId" => "Telephely",
                                    "UploadDate" => "Feltöltés dátuma",
                                    _ => prop.Metadata.Name
                                };

                                if (prop.Metadata.Name == "PartnerId")
                                {
                                    var oldP = ToNullableInt(oldValue);
                                    var newP = ToNullableInt(newValue);

                                    var oldName = await GetPartnerNameAsync(context, oldP);
                                    var newName = await GetPartnerNameAsync(context, newP);

                                    changes.Add($"{displayName}: {oldName} → {newName}");
                                    continue;
                                }

                                if (prop.Metadata.Name == "SiteId")
                                {
                                    var oldS = ToNullableInt(oldValue);
                                    var newS = ToNullableInt(newValue);

                                    var oldName = await GetSiteNameAsync(context, oldS);
                                    var newName = await GetSiteNameAsync(context, newS);

                                    changes.Add($"{displayName}: {oldName} → {newName}");
                                    continue;
                                }

                                if (prop.Metadata.Name == "DocumentTypeId")
                                {
                                    var oldT = ToNullableInt(oldValue);
                                    var newT = ToNullableInt(newValue);

                                    var oldName = await GetDocumentTypeNameAsync(context, oldT);
                                    var newName = await GetDocumentTypeNameAsync(context, newT);

                                    changes.Add($"{displayName}: {oldName} → {newName}");
                                    continue;
                                }

                                if (prop.Metadata.Name == "Status")
                                {
                                    changes.Add($"{displayName}: {oldValue} → {newValue}");
                                    continue;
                                }

                                changes.Add($"{displayName}: {oldValue} → {newValue}");
                                continue;
                            }

                            changes.Add($"{prop.Metadata.Name}: {oldValue} → {newValue}");
                        }

                        if (changes.Any())
                        {
                            auditEntries.Add(new AuditLog
                            {
                                EntityType = entityTypeName,
                                EntityId = entityId,
                                Action = "Updated",
                                ChangedById = userId,
                                ChangedByName = userName,
                                ChangedAt = now,
                                Changes = string.Join("; ", changes)
                            });
                        }
                        break;
                    }

                    case EntityState.Deleted:
                    {
                        if (entry.Entity is Partner)
                        {
                            auditEntries.Add(new AuditLog
                            {
                                EntityType = entityTypeName,
                                EntityId = entityId,
                                Action = "Deleted",
                                ChangedById = userId,
                                ChangedByName = userName,
                                ChangedAt = now,
                                Changes = "Partner törölve (soft delete)."
                            });
                            break;
                        }

                        if (entry.Entity is CustomerCommunication)
                        {
                            auditEntries.Add(new AuditLog
                            {
                                EntityType = entityTypeName,
                                EntityId = entityId,
                                Action = "Deleted",
                                ChangedById = userId,
                                ChangedByName = userName,
                                ChangedAt = now,
                                Changes = "Kommunikáció törölve."
                            });
                            break;
                        }

                        if (entry.Entity is CommunicationResponsible crDel)
                        {
                            var responsibleName = await GetUserNameAsync(context, crDel.ResponsibleId);
                            auditEntries.Add(new AuditLog
                            {
                                EntityType = entityTypeName,
                                EntityId = entityId,
                                Action = "Deleted",
                                ChangedById = userId,
                                ChangedByName = userName,
                                ChangedAt = now,
                                Changes = $"Felelős törölve. Kommunikáció: #{crDel.CustomerCommunicationId}; Felelős: {responsibleName}"
                            });
                            break;
                        }

                        if (entry.Entity is Document)
                        {
                            auditEntries.Add(new AuditLog
                            {
                                EntityType = entityTypeName,
                                EntityId = entityId,
                                Action = "Deleted",
                                ChangedById = userId,
                                ChangedByName = userName,
                                ChangedAt = now,
                                Changes = "Dokumentum törölve."
                            });
                            break;
                        }

                        auditEntries.Add(new AuditLog
                        {
                            EntityType = entityTypeName,
                            EntityId = entityId,
                            Action = "Deleted",
                            ChangedById = userId,
                            ChangedByName = userName,
                            ChangedAt = now,
                            Changes = "Rekord törölve."
                        });
                        break;
                    }
                }
            }

            if (auditEntries.Any())
            {
                context.Set<AuditLog>().AddRange(auditEntries);
                _logger.LogInformation("Audit: {Count} bejegyzés létrehozva {User} által.", auditEntries.Count, userName);
            }
        }

        private static int GetEntityId(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
        {
            var pk = entry.Metadata.FindPrimaryKey();
            if (pk == null) return 0;

            var pkProp = pk.Properties.FirstOrDefault();
            if (pkProp == null) return 0;

            var val = entry.Property(pkProp.Name).CurrentValue ?? entry.Property(pkProp.Name).OriginalValue;
            return val is int intId ? intId : 0;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            AuditChangesAsync(eventData.Context!).GetAwaiter().GetResult();
            return base.SavingChanges(eventData, result);
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            await AuditChangesAsync(eventData.Context!);
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
