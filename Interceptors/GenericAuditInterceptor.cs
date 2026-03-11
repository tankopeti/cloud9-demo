using Cloud9_2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Globalization;

namespace Cloud9_2.Interceptors
{
    public class GenericAuditInterceptor : SaveChangesInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GenericAuditInterceptor> _logger;

        // ✅ Audited entity types (CLR types)
        private static readonly Dictionary<Type, string> AuditedEntities = new()
        {
            { typeof(Partner), "Partner" },
            { typeof(TaskPM), "TaskPM" },

            { typeof(CustomerCommunication), "CustomerCommunication" },
            { typeof(CommunicationResponsible), "CommunicationResponsible" },

            { typeof(Document), "Document" },

            // HR
            { typeof(Employees), "Employee" },
            { typeof(EmployeeEmploymentStatus), "EmployeeEmploymentStatus" },
            { typeof(EmployeeSite), "EmployeeSite" }
        };

        // ✅ Default exclusions (noise reduction)
        private static readonly HashSet<string> ExcludedProperties = new()
        {
            "CreatedDate", "ModifiedDate", "IsActive", "RowVersion",
            "UpdatedDate", "CreatedAt", "UpdatedAt" // HR Employees zaj csökkentés
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

        // ==========================
        // Audit value normalization (noise reduction)
        // ==========================

        private static string NormalizeString(string? s)
        {
            // null == "" == "   "
            return string.IsNullOrWhiteSpace(s) ? "" : s.Trim();
        }

        private static bool TryDecimal(object? o, out decimal value)
        {
            value = 0m;
            if (o == null) return false;

            if (o is decimal d) { value = d; return true; }
            if (o is int i) { value = i; return true; }
            if (o is long l) { value = l; return true; }
            if (o is double db) { value = (decimal)db; return true; }
            if (o is float f) { value = (decimal)f; return true; }

            var s = o.ToString();
            if (string.IsNullOrWhiteSpace(s)) return false;

            // HU (8,00) + invariant (8.00) + plain (8)
            return decimal.TryParse(s, NumberStyles.Any, new CultureInfo("hu-HU"), out value)
                || decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
        }

        private static bool TryTimeSpanHungarian(object? o, out TimeSpan value)
        {
            value = default;
            if (o == null) return false;

            if (o is TimeSpan ts) { value = ts; return true; }

            // Ha esetleg TimeOnly lenne
#if NET6_0_OR_GREATER
            if (o is TimeOnly to) { value = to.ToTimeSpan(); return true; }
#endif

            var s = NormalizeString(o.ToString());
            if (string.IsNullOrWhiteSpace(s)) return false;

            // Magyar formátum: "8:00", "12:00", "15:35" (H:mm)
            // plusz "08:00" (HH:mm) is ok
            return TimeSpan.TryParseExact(s, new[] { @"h\:mm", @"hh\:mm", @"h\:mm\:ss", @"hh\:mm\:ss" },
                                          CultureInfo.InvariantCulture, out value)
                || TimeSpan.TryParse(s, CultureInfo.InvariantCulture, out value);
        }

        private static bool AreEquivalent(object? oldObj, object? newObj)
        {
            if (oldObj == null && newObj == null) return true;

            // string: null == ""
            if (oldObj is string || newObj is string)
            {
                var a = NormalizeString(oldObj?.ToString());
                var b = NormalizeString(newObj?.ToString());
                return a == b;
            }

            // numeric: 8,00 == 8
            if (TryDecimal(oldObj, out var od) && TryDecimal(newObj, out var nd))
                return od == nd;

            // time: 8:00 == 08:00
            if (TryTimeSpanHungarian(oldObj, out var ot) && TryTimeSpanHungarian(newObj, out var nt))
                return ot == nt;

            // fallback
            return Equals(oldObj, newObj) || (oldObj?.ToString() ?? "null") == (newObj?.ToString() ?? "null");
        }

        private static string FormatForAudit(object? o)
        {
            if (o == null) return "null";

            // string: trim
            if (o is string s) return NormalizeString(s);

            // numeric: egységes (8,00)
            if (TryDecimal(o, out var d))
                return d.ToString("0.##", new CultureInfo("hu-HU"));

            // time: egységes (H:mm)
            if (TryTimeSpanHungarian(o, out var ts))
                return ts.ToString(@"h\:mm");

            return o.ToString() ?? "null";
        }

        // ==========================
        // HR: safe helpers (property-name tolerant)
        // ==========================
        private static int GetIntPropByNames(object obj, params string[] names)
        {
            var t = obj.GetType();

            foreach (var n in names)
            {
                var p = t.GetProperty(n);
                if (p == null) continue;

                var v = p.GetValue(obj);
                if (v == null) continue;

                try
                {
                    // ha int
                    if (v.GetType() == typeof(int))
                        return (int)v;

                    // ha int?
                    var underlying = Nullable.GetUnderlyingType(v.GetType());
                    if (underlying == typeof(int))
                        return (int)Convert.ChangeType(v, typeof(int));

                    // ha string vagy más
                    var s = v.ToString();
                    if (!string.IsNullOrWhiteSpace(s) && int.TryParse(s, out var parsed))
                        return parsed;
                }
                catch
                {
                    // ignore, megyünk tovább
                }
            }

            return 0;
        }

        private async Task<string> GetEmploymentStatusNameAsync(DbContext context, int statusId)
        {
            if (statusId <= 0) return "—";

            var name = await context.Set<EmploymentStatus>()
                .Where(x => x.StatusId == statusId)          // <-- nálad lehet EmploymentStatusId, StatusId, stb.
                .Select(x => x.StatusName)                    // <-- a valós mezőnév
                .FirstOrDefaultAsync();

            return string.IsNullOrWhiteSpace(name) ? $"#{statusId}" : name.Trim();
        }

        private async Task AuditEmployeeStatusLinkAsync(
            DbContext context,
            List<AuditLog> auditEntries,
            EmployeeEmploymentStatus link,
            EntityState state,
            string userId,
            string userName,
            DateTime now)
        {
            if (state != EntityState.Added && state != EntityState.Deleted)
                return;

            // ✅ toleráns: ha nálad StatusId a mező, ez akkor is működik
            var employeeId = GetIntPropByNames(link, "EmployeeId");
            var statusId = GetIntPropByNames(link, "EmploymentStatusId", "StatusId", "EmploymentStatusID", "EmploymentStatusPMId");

            var statusName = await GetEmploymentStatusNameAsync(context, statusId);

            auditEntries.Add(new AuditLog
            {
                EntityType = "Employee",
                EntityId = employeeId,
                Action = "Updated",
                ChangedById = userId,
                ChangedByName = userName,
                ChangedAt = now,
                Changes = state == EntityState.Added
                    ? $"Státusz hozzáadva: {statusName}"
                    : $"Státusz eltávolítva: {statusName}"
            });
        }

        private async Task AuditEmployeeSiteLinkAsync(
            DbContext context,
            List<AuditLog> auditEntries,
            EmployeeSite link,
            EntityState state,
            string userId,
            string userName,
            DateTime now)
        {
            if (state != EntityState.Added && state != EntityState.Deleted)
                return;

            var employeeId = GetIntPropByNames(link, "EmployeeId");
            var siteId = GetIntPropByNames(link, "SiteId");

            var siteName = await GetSiteNameAsync(context, siteId);

            auditEntries.Add(new AuditLog
            {
                EntityType = "Employee",
                EntityId = employeeId,
                Action = "Updated",
                ChangedById = userId,
                ChangedByName = userName,
                ChangedAt = now,
                Changes = state == EntityState.Added
                    ? $"Telephely hozzáadva: {siteName}"
                    : $"Telephely eltávolítva: {siteName}"
            });
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
                // ==========================
                // HR JOIN audit -> Employee history
                // ==========================
                if (entry.Entity is EmployeeEmploymentStatus ees)
                {
                    await AuditEmployeeStatusLinkAsync(context, auditEntries, ees, entry.State, userId, userName, now);
                    continue;
                }

                if (entry.Entity is EmployeeSite es)
                {
                    await AuditEmployeeSiteLinkAsync(context, auditEntries, es, entry.State, userId, userName, now);
                    continue;
                }

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

                            // HR Employees Created – opcionális, de hasznos
                            if (entry.Entity is Employees eNew)
                            {
                                auditEntries.Add(new AuditLog
                                {
                                    EntityType = "Employee",
                                    EntityId = eNew.EmployeeId,
                                    Action = "Created",
                                    ChangedById = userId,
                                    ChangedByName = userName,
                                    ChangedAt = now,
                                    Changes = $"Új dolgozó létrehozva: {(eNew.LastName ?? "").Trim()} {(eNew.FirstName ?? "").Trim()}".Trim()
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
                                var oldObj = entry.OriginalValues[prop.Metadata];
                                var newObj = entry.CurrentValues[prop.Metadata];

                                // ✅ zajcsökkentés: string null/empty + numeric formázás + time (8:00) normalizálva
                                if (AreEquivalent(oldObj, newObj))
                                    continue;

                                var oldValue = FormatForAudit(oldObj);
                                var newValue = FormatForAudit(newObj);

                                // ✅ Employees IsActive: még excluded előtt!
                                if (entry.Entity is Employees && prop.Metadata.Name == "IsActive")
                                {
                                    var text = (newValue.Equals("False", StringComparison.OrdinalIgnoreCase) || newValue == "0")
                                        ? "Dolgozó deaktiválva."
                                        : "Dolgozó újraaktiválva.";

                                    changes.Add(text);
                                    continue;
                                }

                                if (ExcludedProperties.Contains(prop.Metadata.Name))
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

                                // ✅ HR Employees mező mapping (minimál, bővíthető)
                                if (entry.Entity is Employees)
                                {
                                    string displayName = prop.Metadata.Name switch
                                    {
                                        "FirstName" => "Keresztnév",
                                        "LastName" => "Vezetéknév",
                                        "Email" => "E-mail",
                                        "Email2" => "E-mail 2",
                                        "PhoneNumber" => "Telefonszám",
                                        "PhoneNumber2" => "Telefonszám 2",
                                        "Address" => "Cím",
                                        "HireDate" => "Belépés dátuma",
                                        "DepartmentId" => "Osztály",
                                        "JobTitleId" => "Munkakör",
                                        "WorkerTypeId" => "Dolgozó típus",
                                        "PartnerId" => "Partner",
                                        "DefaultSiteId" => "Alap telephely",
                                        "WorkingTime" => "Munkaidő",
                                        "IsContracted" => "Szerződéses",
                                        _ => prop.Metadata.Name
                                    };

                                    // Partner név feloldás
                                    if (prop.Metadata.Name == "PartnerId")
                                    {
                                        var oldId = ToNullableInt(oldValue);
                                        var newId = ToNullableInt(newValue);
                                        var oldName = await GetPartnerNameAsync(context, oldId);
                                        var newName = await GetPartnerNameAsync(context, newId);
                                        changes.Add($"{displayName}: {oldName} → {newName}");
                                        continue;
                                    }

                                    // DefaultSite név feloldás
                                    if (prop.Metadata.Name == "DefaultSiteId")
                                    {
                                        var oldId = ToNullableInt(oldValue);
                                        var newId = ToNullableInt(newValue);
                                        var oldName = await GetSiteNameAsync(context, oldId);
                                        var newName = await GetSiteNameAsync(context, newId);
                                        changes.Add($"{displayName}: {oldName} → {newName}");
                                        continue;
                                    }

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
                                        "RelatedPartnerId" => "Kapcsolt partner",
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

                                    if (prop.Metadata.Name == "PartnerId")
                                    {
                                        var oldId = ToNullableInt(oldValue);
                                        var newId = ToNullableInt(newValue);

                                        var oldName = await GetPartnerNameAsync(context, oldId);
                                        var newName = await GetPartnerNameAsync(context, newId);

                                        changes.Add($"{displayName}: {oldName} → {newName}");
                                        continue;
                                    }

                                    if (prop.Metadata.Name == "RelatedPartnerId")
                                    {
                                        var oldId = ToNullableInt(oldValue);
                                        var newId = ToNullableInt(newValue);

                                        var oldName = await GetPartnerNameAsync(context, oldId);
                                        var newName = await GetPartnerNameAsync(context, newId);

                                        changes.Add($"{displayName}: {oldName} → {newName}");
                                        continue;
                                    }

                                    if (prop.Metadata.Name == "SiteId")
                                    {
                                        var oldId = ToNullableInt(oldValue);
                                        var newId = ToNullableInt(newValue);

                                        var oldName = await GetSiteNameAsync(context, oldId);
                                        var newName = await GetSiteNameAsync(context, newId);

                                        changes.Add($"{displayName}: {oldName} → {newName}");
                                        continue;
                                    }

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