using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloud9_2.Data;
using Cloud9_2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cloud9_2.Services
{
    public class EmployeeService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(
            ApplicationDbContext context,
            ILogger<EmployeeService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private IQueryable<Employees> ApplyEmployeeIndexFilters(
            IQueryable<Employees> q,
            EmployeeIndexQueryDto f)
        {
            // 1) Top search (existing)
            if (!string.IsNullOrWhiteSpace(f.SearchTerm))
            {
                var st = f.SearchTerm.Trim();
                q = q.Where(e =>
                    (e.FirstName != null && EF.Functions.Like(e.FirstName, $"%{st}%")) ||
                    (e.LastName != null && EF.Functions.Like(e.LastName, $"%{st}%")) ||
                    (e.Email != null && EF.Functions.Like(e.Email, $"%{st}%")) ||
                    (e.PhoneNumber != null && EF.Functions.Like(e.PhoneNumber, $"%{st}%")) ||
                    (e.TaxId != null && EF.Functions.Like(e.TaxId, $"%{st}%")) ||
                    (e.TajNumber != null && EF.Functions.Like(e.TajNumber, $"%{st}%"))
                );
            }

            // 2) Quick filter (existing)
            switch ((f.QuickFilter ?? "all").ToLowerInvariant())
            {
                case "active":
                    q = q.Where(e => e.IsActive);
                    break;
                case "internal":
                    q = q.Where(e => e.WorkerTypeId == 1);
                    break;
                case "external":
                    q = q.Where(e => e.WorkerTypeId == 2);
                    break;
            }

            // 3) Advanced text (Name/Email)
            if (!string.IsNullOrWhiteSpace(f.Text))
            {
                var t = f.Text.Trim();
                q = q.Where(e =>
                    (e.FirstName != null && EF.Functions.Like(e.FirstName, $"%{t}%")) ||
                    (e.LastName != null && EF.Functions.Like(e.LastName, $"%{t}%")) ||
                    (e.Email != null && EF.Functions.Like(e.Email, $"%{t}%"))
                );
            }

            // 4) Phone contains
            if (!string.IsNullOrWhiteSpace(f.Phone))
            {
                var p = f.Phone.Trim();
                q = q.Where(e => e.PhoneNumber != null && EF.Functions.Like(e.PhoneNumber, $"%{p}%"));
            }

            // 5) WorkerType / Partner
            if (f.WorkerTypeId.HasValue)
                q = q.Where(e => e.WorkerTypeId == f.WorkerTypeId.Value);

            if (f.PartnerId.HasValue)
                q = q.Where(e => e.PartnerId == f.PartnerId.Value);

            // 6) ActiveOnly (modal) – ha true, akkor IsActive
            if (f.ActiveOnly == true)
                q = q.Where(e => e.IsActive);

            // 7) Status filter (EXISTS)
            if (f.StatusId.HasValue)
            {
                var sid = f.StatusId.Value;
                q = q.Where(e =>
                    _context.EmployeeEmploymentStatuses.Any(x =>
                        x.EmployeeId == e.EmployeeId &&
                        x.IsCurrent &&
                        x.StatusId == sid
                    )
                );
            }

            // 8) Site filter (EXISTS)
            if (f.SiteId.HasValue)
            {
                var siteId = f.SiteId.Value;
                q = q.Where(e =>
                    _context.EmployeeSites.Any(x =>
                        x.EmployeeId == e.EmployeeId &&
                        x.SiteId == siteId
                    )
                );
            }

            return q;
        }
        // -----------------------------
        // AJAX index (pagination + search + quick filter)
        // -----------------------------
        public async Task<EmployeeIndexResult> GetEmployeesIndexAsync(
            int page,
            int pageSize,
            string? searchTerm,
            string? quickFilter // all | active | internal | external
        )
        {
            if (page < 1) page = 1;
            if (pageSize < 10) pageSize = 30;
            if (pageSize > 200) pageSize = 200;

            // 1) Alap query (csak Employees tábla)
            IQueryable<Employees> baseQ = _context.Employees.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var st = searchTerm.Trim();
                baseQ = baseQ.Where(e =>
                    (e.FirstName != null && EF.Functions.Like(e.FirstName, $"%{st}%")) ||
                    (e.LastName != null && EF.Functions.Like(e.LastName, $"%{st}%")) ||
                    (e.Email != null && EF.Functions.Like(e.Email, $"%{st}%")) ||
                    (e.PhoneNumber != null && EF.Functions.Like(e.PhoneNumber, $"%{st}%")) ||
                    (e.TaxId != null && EF.Functions.Like(e.TaxId, $"%{st}%")) ||
                    (e.TajNumber != null && EF.Functions.Like(e.TajNumber, $"%{st}%"))
                );
            }

            switch ((quickFilter ?? "all").ToLowerInvariant())
            {
                case "active":
                    baseQ = baseQ.Where(e => e.IsActive);
                    break;
                case "internal":
                    baseQ = baseQ.Where(e => e.WorkerTypeId == 1);
                    break;
                case "external":
                    baseQ = baseQ.Where(e => e.WorkerTypeId == 2);
                    break;
                case "all":
                default:
                    break;
            }

            var total = await baseQ.CountAsync();

            // 2) Oldalnyi employee alapadat + név mezők JOIN-nal (nem Include)
            var pageItems = await (
            from e in baseQ
                .OrderByDescending(x => x.CreatedAt ?? DateTime.MinValue)
                .ThenByDescending(x => x.EmployeeId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)

            join wt in _context.WorkerTypes.AsNoTracking()
                on e.WorkerTypeId equals wt.WorkerTypeId into wtj
            from wt in wtj.DefaultIfEmpty()

            join p in _context.Partners.AsNoTracking()
                on e.PartnerId equals p.PartnerId into pj
            from p in pj.DefaultIfEmpty()

            join jt in _context.JobTitles.AsNoTracking()
                on e.JobTitleId equals jt.JobTitleId into jtj
            from jt in jtj.DefaultIfEmpty()

            join ds in _context.Sites.AsNoTracking()
                on e.DefaultSiteId equals ds.SiteId into dsj
            from ds in dsj.DefaultIfEmpty()

            select new
            {
                e.EmployeeId,
                e.FirstName,
                e.LastName,
                e.Email,
                e.PhoneNumber,
                e.WorkerTypeId,
                e.DefaultSiteId,
                DefaultSiteName = ds != null ? ds.SiteName : null,
                WorkerTypeName = wt != null ? wt.Name : null,
                e.PartnerId,
                PartnerName = p != null ? p.Name : null,
                e.JobTitleId,
                JobTitleName = jt != null ? jt.TitleName : null,
                e.IsActive
            }
            ).ToListAsync();

            var employeeIds = pageItems.Select(x => x.EmployeeId).ToList();

            if (employeeIds.Count == 0)
            {
                return new EmployeeIndexResult
                {
                    Items = new List<EmployeeIndexRowDto>(),
                    TotalRecords = total,
                    CurrentPage = page,
                    PageSize = pageSize
                };
            }

            // 3) Státusz nevek
            var statusByEmployee = await (
                from ees in _context.EmployeeEmploymentStatuses.AsNoTracking()
                join s in _context.EmploymentStatuses.AsNoTracking()
                    on ees.StatusId equals s.StatusId
                where employeeIds.Contains(ees.EmployeeId) && ees.IsCurrent
                orderby ees.AssignedAt descending
                select new
                {
                    ees.EmployeeId,
                    StatusName = s.StatusName
                }
            )
            .ToListAsync();

            var statusDict = statusByEmployee
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.StatusName).Distinct().ToList()
                );

            // 4) Site nevek
            var siteByEmployee = await (
                from es in _context.EmployeeSites.AsNoTracking()
                join s in _context.Sites.AsNoTracking()
                    on es.SiteId equals s.SiteId
                where employeeIds.Contains(es.EmployeeId)
                orderby es.IsPrimary descending
                select new
                {
                    es.EmployeeId,
                    SiteName = s.SiteName
                }
            )
            .ToListAsync();

            var siteDict = siteByEmployee
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.SiteName).Distinct().ToList()
                );

            // 5) DTO összeállítás
            var items = pageItems.Select(x => new EmployeeIndexRowDto
            {
                EmployeeId = x.EmployeeId,
                FullName = ((x.LastName ?? "") + " " + (x.FirstName ?? "")).Trim(),
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                WorkerTypeId = x.WorkerTypeId,
                WorkerTypeName = x.WorkerTypeName,
                PartnerId = x.PartnerId,
                PartnerName = x.PartnerName,
                JobTitleId = x.JobTitleId,
                JobTitleName = x.JobTitleName,
                IsActive = x.IsActive,
                StatusNames = statusDict.TryGetValue(x.EmployeeId, out var st) ? st : new List<string>(),
                SiteNames = siteDict.TryGetValue(x.EmployeeId, out var si) ? si : new List<string>(),
                DefaultSiteId = x.DefaultSiteId,
                DefaultSiteName = x.DefaultSiteName
            }).ToList();

            return new EmployeeIndexResult
            {
                Items = items,
                TotalRecords = total,
                CurrentPage = page,
                PageSize = pageSize
            };
        }

        private IQueryable<Employees> ApplyAdvancedFilters(
    IQueryable<Employees> q,
    EmployeeAdvancedFilterDto f)
        {
            if (f == null) return q;

            if (!string.IsNullOrWhiteSpace(f.Text))
            {
                var t = f.Text.Trim();
                q = q.Where(e =>
                    (e.FirstName != null && EF.Functions.Like(e.FirstName, $"%{t}%")) ||
                    (e.LastName != null && EF.Functions.Like(e.LastName, $"%{t}%")) ||
                    (e.Email != null && EF.Functions.Like(e.Email, $"%{t}%"))
                );
            }

            if (!string.IsNullOrWhiteSpace(f.Phone))
            {
                var p = f.Phone.Trim();
                q = q.Where(e => e.PhoneNumber != null && EF.Functions.Like(e.PhoneNumber, $"%{p}%"));
            }

            if (f.WorkerTypeId.HasValue)
                q = q.Where(e => e.WorkerTypeId == f.WorkerTypeId.Value);

            if (f.PartnerId.HasValue)
                q = q.Where(e => e.PartnerId == f.PartnerId.Value);

            if (f.ActiveOnly == true)
                q = q.Where(e => e.IsActive);

            // Státusz filter: EXISTS az EmployeeEmploymentStatuses-ra (IsCurrent)
            if (f.StatusId.HasValue)
            {
                var sid = f.StatusId.Value;
                q = q.Where(e =>
                    _context.EmployeeEmploymentStatuses.Any(x =>
                        x.EmployeeId == e.EmployeeId &&
                        x.IsCurrent &&
                        x.StatusId == sid
                    )
                );
            }

            // Telephely filter: EXISTS az EmployeeSites-ra
            if (f.SiteId.HasValue)
            {
                var siteId = f.SiteId.Value;
                q = q.Where(e =>
                    _context.EmployeeSites.Any(x =>
                        x.EmployeeId == e.EmployeeId &&
                        x.SiteId == siteId
                    )
                );
            }

            return q;
        }

        public async Task<EmployeeIndexResult> GetEmployeesIndexAdvancedAsync(
            int page,
            int pageSize,
            string? searchTerm,
            string? quickFilter,
            EmployeeAdvancedFilterDto advanced
        )
        {
            if (page < 1) page = 1;
            if (pageSize < 10) pageSize = 30;
            if (pageSize > 200) pageSize = 200;

            // 1) Alap query (csak Employees tábla)
            IQueryable<Employees> baseQ = _context.Employees.AsNoTracking();

            // --- ugyanaz a keresés, mint a régi metódusban
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var st = searchTerm.Trim();
                baseQ = baseQ.Where(e =>
                    (e.FirstName != null && EF.Functions.Like(e.FirstName, $"%{st}%")) ||
                    (e.LastName != null && EF.Functions.Like(e.LastName, $"%{st}%")) ||
                    (e.Email != null && EF.Functions.Like(e.Email, $"%{st}%")) ||
                    (e.PhoneNumber != null && EF.Functions.Like(e.PhoneNumber, $"%{st}%")) ||
                    (e.TaxId != null && EF.Functions.Like(e.TaxId, $"%{st}%")) ||
                    (e.TajNumber != null && EF.Functions.Like(e.TajNumber, $"%{st}%"))
                );
            }

            // --- ugyanaz a quick filter, mint a régi metódusban
            switch ((quickFilter ?? "all").ToLowerInvariant())
            {
                case "active":
                    baseQ = baseQ.Where(e => e.IsActive);
                    break;
                case "internal":
                    baseQ = baseQ.Where(e => e.WorkerTypeId == 1);
                    break;
                case "external":
                    baseQ = baseQ.Where(e => e.WorkerTypeId == 2);
                    break;
                case "all":
                default:
                    break;
            }

            // --- ADVANCED FILTER (új)
            baseQ = ApplyAdvancedFilters(baseQ, advanced);

            var total = await baseQ.CountAsync();

            // 2) Oldalnyi employee alapadat + név mezők JOIN-nal (nem Include)
            var pageItems = await (
                from e in baseQ
                    .OrderByDescending(x => x.CreatedAt ?? DateTime.MinValue)
                    .ThenByDescending(x => x.EmployeeId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)

                join wt in _context.WorkerTypes.AsNoTracking()
                    on e.WorkerTypeId equals wt.WorkerTypeId into wtj
                from wt in wtj.DefaultIfEmpty()

                join p in _context.Partners.AsNoTracking()
                    on e.PartnerId equals p.PartnerId into pj
                from p in pj.DefaultIfEmpty()

                join jt in _context.JobTitles.AsNoTracking()
                    on e.JobTitleId equals jt.JobTitleId into jtj
                from jt in jtj.DefaultIfEmpty()

                join ds in _context.Sites.AsNoTracking()
                    on e.DefaultSiteId equals ds.SiteId into dsj
                from ds in dsj.DefaultIfEmpty()

                select new
                {
                    e.EmployeeId,
                    e.FirstName,
                    e.LastName,
                    e.Email,
                    e.PhoneNumber,
                    e.WorkerTypeId,
                    e.DefaultSiteId,
                    DefaultSiteName = ds != null ? ds.SiteName : null,
                    WorkerTypeName = wt != null ? wt.Name : null,
                    e.PartnerId,
                    PartnerName = p != null ? p.Name : null,
                    e.JobTitleId,
                    JobTitleName = jt != null ? jt.TitleName : null,
                    e.IsActive
                }
            ).ToListAsync();

            var employeeIds = pageItems.Select(x => x.EmployeeId).ToList();

            if (employeeIds.Count == 0)
            {
                return new EmployeeIndexResult
                {
                    Items = new System.Collections.Generic.List<EmployeeIndexRowDto>(),
                    TotalRecords = total,
                    CurrentPage = page,
                    PageSize = pageSize
                };
            }

            // 3) Státusz nevek (ugyanaz)
            var statusByEmployee = await (
                from ees in _context.EmployeeEmploymentStatuses.AsNoTracking()
                join s in _context.EmploymentStatuses.AsNoTracking()
                    on ees.StatusId equals s.StatusId
                where employeeIds.Contains(ees.EmployeeId) && ees.IsCurrent
                orderby ees.AssignedAt descending
                select new
                {
                    ees.EmployeeId,
                    StatusName = s.StatusName
                }
            ).ToListAsync();

            var statusDict = statusByEmployee
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.StatusName).Distinct().ToList()
                );

            // 4) Site nevek (ugyanaz)
            var siteByEmployee = await (
                from es in _context.EmployeeSites.AsNoTracking()
                join s in _context.Sites.AsNoTracking()
                    on es.SiteId equals s.SiteId
                where employeeIds.Contains(es.EmployeeId)
                orderby es.IsPrimary descending
                select new
                {
                    es.EmployeeId,
                    SiteName = s.SiteName
                }
            ).ToListAsync();

            var siteDict = siteByEmployee
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.SiteName).Distinct().ToList()
                );

            // 5) DTO összeállítás (ugyanaz)
            var items = pageItems.Select(x => new EmployeeIndexRowDto
            {
                EmployeeId = x.EmployeeId,
                FullName = ((x.LastName ?? "") + " " + (x.FirstName ?? "")).Trim(),
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                WorkerTypeId = x.WorkerTypeId,
                WorkerTypeName = x.WorkerTypeName,
                PartnerId = x.PartnerId,
                PartnerName = x.PartnerName,
                JobTitleId = x.JobTitleId,
                JobTitleName = x.JobTitleName,
                IsActive = x.IsActive,
                StatusNames = statusDict.TryGetValue(x.EmployeeId, out var st) ? st : new System.Collections.Generic.List<string>(),
                SiteNames = siteDict.TryGetValue(x.EmployeeId, out var si) ? si : new System.Collections.Generic.List<string>(),
                DefaultSiteId = x.DefaultSiteId,
                DefaultSiteName = x.DefaultSiteName
            }).ToList();

            return new EmployeeIndexResult
            {
                Items = items,
                TotalRecords = total,
                CurrentPage = page,
                PageSize = pageSize
            };
        }

        // -----------------------------
        // READ (view modal) - MINDEN CREATE mező + MINDENHEZ NÉV (nem csak ID)
        // -----------------------------
        public async Task<EmployeeDetailsDto?> GetByIdAsync(int employeeId)
        {
            // 1) Alap employee + name mezők (WorkerType, Partner, JobTitle, DefaultSite)
            var dto = await (
                from e in _context.Employees.AsNoTracking()
                where e.EmployeeId == employeeId

                join wt in _context.WorkerTypes.AsNoTracking()
                    on e.WorkerTypeId equals wt.WorkerTypeId into wtj
                from wt in wtj.DefaultIfEmpty()

                join p in _context.Partners.AsNoTracking()
                    on e.PartnerId equals p.PartnerId into pj
                from p in pj.DefaultIfEmpty()

                join jt in _context.JobTitles.AsNoTracking()
                    on e.JobTitleId equals jt.JobTitleId into jtj
                from jt in jtj.DefaultIfEmpty()

                join ds in _context.Sites.AsNoTracking()
                    on e.DefaultSiteId equals ds.SiteId into dsj
                from ds in dsj.DefaultIfEmpty()

                select new EmployeeDetailsDto
                {
                    // ---- azonosítók
                    EmployeeId = e.EmployeeId,

                    // ---- alap adatok
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    FullName = ((e.LastName ?? "") + " " + (e.FirstName ?? "")).Trim(),

                    Email = e.Email,
                    Email2 = e.Email2,
                    PhoneNumber = e.PhoneNumber,
                    PhoneNumber2 = e.PhoneNumber2,

                    DateOfBirth = e.DateOfBirth,
                    Address = e.Address,
                    HireDate = e.HireDate,

                    DepartmentId = e.DepartmentId,
                    // DepartmentName: ha van Department tábla, itt joinold fel és töltsd (lásd lent a TODO-t)

                    JobTitleId = e.JobTitleId,
                    JobTitleName = jt != null ? jt.TitleName : null,

                    IsActive = e.IsActive,

                    DefaultSiteId = e.DefaultSiteId,
                    DefaultSiteName = ds != null ? ds.SiteName : null,

                    WorkingTime = e.WorkingTime,
                    IsContracted = e.IsContracted,

                    FamilyData = e.FamilyData,
                    Comment1 = e.Comment1,
                    Comment2 = e.Comment2,

                    VacationDays = e.VacationDays,
                    FullVacationDays = e.FullVacationDays,

                    TaxId = e.TaxId,
                    TajNumber = e.TajNumber,

                    BirthName = e.BirthName,
                    MotherBirthName = e.MotherBirthName,
                    BirthPlace = e.BirthPlace,

                    NationalityCode = e.NationalityCode,
                    FeorCode = e.FeorCode,

                    EmploymentEndDate = e.EmploymentEndDate,

                    PermanentAddress = e.PermanentAddress,
                    MailingAddress = e.MailingAddress,

                    BankAccountIban = e.BankAccountIban,

                    WorkerTypeId = e.WorkerTypeId,
                    WorkerTypeName = wt != null ? wt.Name : null,

                    PartnerId = e.PartnerId,
                    PartnerName = p != null ? p.Name : null,

                    // listák majd lent töltődnek:
                    StatusIds = new List<int>(),
                    StatusNames = new List<string>(),
                    Sites = new List<EmployeeSiteDto>()
                }
            ).FirstOrDefaultAsync();

            if (dto == null)
                return null;

            // 2) Státuszok: ID + Név + AssignedAt (mert ezt is "látni akarod")
            var statuses = await (
                from ees in _context.EmployeeEmploymentStatuses.AsNoTracking()
                join s in _context.EmploymentStatuses.AsNoTracking()
                    on ees.StatusId equals s.StatusId
                where ees.EmployeeId == employeeId && ees.IsCurrent
                orderby ees.AssignedAt descending
                select new EmployeeStatusDto
                {
                    StatusId = ees.StatusId,
                    StatusName = s.StatusName,
                    AssignedAt = ees.AssignedAt
                }
            ).ToListAsync();

            dto.StatusIds = statuses.Select(x => x.StatusId).Distinct().ToList();
            dto.StatusNames = statuses.Select(x => x.StatusName).Distinct().ToList();
            dto.Statuses = statuses;

            // 3) Telephelyek: ID + Név + IsPrimary
            var sites = await (
                from es in _context.EmployeeSites.AsNoTracking()
                join s in _context.Sites.AsNoTracking()
                    on es.SiteId equals s.SiteId
                where es.EmployeeId == employeeId
                orderby es.IsPrimary descending, s.SiteName
                select new EmployeeSiteDto
                {
                    SiteId = es.SiteId,
                    SiteName = s.SiteName,
                    IsPrimary = es.IsPrimary
                }
            ).ToListAsync();

            dto.Sites = sites;

            // convenience listák (ha frontendnek kell egyszerűen)
            dto.SiteIds = sites.Select(x => x.SiteId).Distinct().ToList();
            dto.SiteNames = sites.Select(x => x.SiteName).Distinct().ToList();

            // 4) TODO DepartmentName (csak ha van ilyen táblád)
            // Ha van pl. _context.Departments: joinold fel ugyanígy az 1) query-ben:
            // join d in _context.Departments ... -> DepartmentName = d.Name

            return dto;
        }

        // -----------------------------
        // CREATE
        // -----------------------------
        public async Task<ServiceResult<int>> CreateAsync(EmployeesCreateDto dto)
        {
            try
            {
                // üzleti szabály: külsős => PartnerId kötelező, belsős => null
                NormalizeWorkerTypePartner(dto.WorkerTypeId, ref dto);

                var entity = new Employees
                {
                    FirstName = dto.FirstName?.Trim(),
                    LastName = dto.LastName?.Trim(),
                    Email = dto.Email?.Trim(),
                    Email2 = dto.Email2?.Trim(),
                    PhoneNumber = dto.PhoneNumber?.Trim(),
                    PhoneNumber2 = dto.PhoneNumber2?.Trim(),
                    DateOfBirth = dto.DateOfBirth,
                    Address = dto.Address?.Trim(),
                    HireDate = dto.HireDate,
                    DepartmentId = dto.DepartmentId,
                    JobTitleId = dto.JobTitleId,
                    IsActive = dto.IsActive,
                    DefaultSiteId = dto.DefaultSiteId,
                    WorkingTime = dto.WorkingTime,
                    IsContracted = dto.IsContracted ?? (byte)0,
                    FamilyData = dto.FamilyData,
                    Comment1 = dto.Comment1,
                    Comment2 = dto.Comment2,
                    VacationDays = dto.VacationDays,
                    FullVacationDays = dto.FullVacationDays,
                    TaxId = dto.TaxId,
                    TajNumber = dto.TajNumber,
                    BirthName = dto.BirthName,
                    MotherBirthName = dto.MotherBirthName,
                    BirthPlace = dto.BirthPlace,
                    NationalityCode = dto.NationalityCode,
                    FeorCode = dto.FeorCode,
                    EmploymentEndDate = dto.EmploymentEndDate,
                    PermanentAddress = dto.PermanentAddress,
                    MailingAddress = dto.MailingAddress,
                    BankAccountIban = dto.BankAccountIban,
                    WorkerTypeId = dto.WorkerTypeId,
                    PartnerId = dto.PartnerId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Employees.Add(entity);
                await _context.SaveChangesAsync(); // EmployeeId

                // státuszok mentése (csak ha van mit)
                if (dto.StatusIds != null && dto.StatusIds.Count > 0)
                {
                    await ReplaceStatusesAsync(entity.EmployeeId, dto.StatusIds);
                    await _context.SaveChangesAsync();
                }

                // telephelyek mentése (ha create-ben van SiteIds, akkor itt is érdemes)
                if (dto.SiteIds != null && dto.SiteIds.Count > 0)
                {
                    await ReplaceSitesAsync(entity.EmployeeId, dto.SiteIds, dto.DefaultSiteId);
                    await _context.SaveChangesAsync();
                }

                return ServiceResult<int>.Ok(entity.EmployeeId);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "EmployeeService.CreateAsync DbUpdateException.");
#if DEBUG
                return ServiceResult<int>.Fail($"DB hiba: {dbEx.InnerException?.Message ?? dbEx.Message}");
#else
                return ServiceResult<int>.Fail("Adatbázis hiba történt a dolgozó létrehozásakor.");
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EmployeeService.CreateAsync failed.");
#if DEBUG
                return ServiceResult<int>.Fail($"CreateAsync hiba: {ex.Message}");
#else
                return ServiceResult<int>.Fail("Nem sikerült a dolgozó létrehozása.");
#endif
            }
        }

        // -----------------------------
        // UPDATE
        // -----------------------------
        public async Task<ServiceResult> UpdateAsync(EmployeesUpdateDto dto)
        {
            try
            {
                var entity = await _context.Employees
                    .FirstOrDefaultAsync(e => e.EmployeeId == dto.EmployeeId);

                if (entity == null)
                    return ServiceResult.Fail("A dolgozó nem található.");

                // üzleti szabály: külsős => PartnerId kötelező, belsős => null
                NormalizeWorkerTypePartner(dto.WorkerTypeId, ref dto);

                entity.FirstName = dto.FirstName?.Trim();
                entity.LastName = dto.LastName?.Trim();
                entity.Email = dto.Email?.Trim();
                entity.Email2 = dto.Email2?.Trim();
                entity.PhoneNumber = dto.PhoneNumber?.Trim();
                entity.PhoneNumber2 = dto.PhoneNumber2?.Trim();
                entity.DateOfBirth = dto.DateOfBirth;
                entity.Address = dto.Address?.Trim();
                entity.HireDate = dto.HireDate;
                entity.DepartmentId = dto.DepartmentId;
                entity.JobTitleId = dto.JobTitleId;
                entity.DefaultSiteId = dto.DefaultSiteId;
                entity.WorkingTime = dto.WorkingTime;
                entity.IsContracted = dto.IsContracted ?? (byte)0;
                entity.FamilyData = dto.FamilyData;
                entity.Comment1 = dto.Comment1;
                entity.Comment2 = dto.Comment2;
                entity.TaxId = dto.TaxId;
                entity.TajNumber = dto.TajNumber;
                entity.BirthName = dto.BirthName;
                entity.MotherBirthName = dto.MotherBirthName;
                entity.BirthPlace = dto.BirthPlace;
                entity.NationalityCode = dto.NationalityCode;
                entity.FeorCode = dto.FeorCode;
                entity.EmploymentEndDate = dto.EmploymentEndDate;
                entity.PermanentAddress = dto.PermanentAddress;
                entity.MailingAddress = dto.MailingAddress;
                entity.BankAccountIban = dto.BankAccountIban;
                entity.VacationDays = dto.VacationDays;
                entity.FullVacationDays = dto.FullVacationDays;
                entity.WorkerTypeId = dto.WorkerTypeId;
                entity.PartnerId = dto.PartnerId;
                entity.IsActive = dto.IsActive;
                entity.UpdatedAt = DateTime.UtcNow;

                // státusz join frissítés
                await ReplaceStatusesAsync(entity.EmployeeId, dto.StatusIds);

                // telephely join frissítés (ha van dto.SiteIds)
                if (dto.SiteIds != null)
                    await ReplaceSitesAsync(entity.EmployeeId, dto.SiteIds, dto.DefaultSiteId);

                await _context.SaveChangesAsync();
                return ServiceResult.Ok();
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "EmployeeService.UpdateAsync DbUpdateException.");
#if DEBUG
                var inner = dbEx.InnerException?.Message;
                return ServiceResult.Fail($"DB hiba: {inner ?? dbEx.Message}");
#else
    return ServiceResult.Fail("Adatbázis hiba történt a mentés során.");
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EmployeeService.UpdateAsync failed.");
#if DEBUG
                return ServiceResult.Fail($"UpdateAsync hiba: {ex.Message}");
#else
    return ServiceResult.Fail("Nem sikerült a dolgozó mentése.");
#endif
            }
        }

        // -----------------------------
        // DELETE (soft-delete: IsActive=false)
        // -----------------------------
        public async Task<ServiceResult> SoftDeleteAsync(int employeeId)
        {
            try
            {
                var entity = await _context.Employees
                    .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

                if (entity == null)
                    return ServiceResult.Fail("A dolgozó nem található.");

                entity.IsActive = false;
                entity.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return ServiceResult.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EmployeeService.SoftDeleteAsync failed.");
                return ServiceResult.Fail("Nem sikerült a dolgozó törlése.");
            }
        }

        // -----------------------------
        // Helper: státuszok cseréje (IsCurrent=true)
        // -----------------------------
private async Task ReplaceStatusesAsync(int employeeId, List<int>? statusIds)
{
    statusIds ??= new List<int>();
    var desired = statusIds.Distinct().ToHashSet();

    // aktuális join sorok
    var current = await _context.EmployeeEmploymentStatuses
        .Where(x => x.EmployeeId == employeeId)
        .ToListAsync();

    var currentIds = current.Select(x => x.StatusId).ToHashSet();

    // törlendők: ami most van, de nem kell
    var toRemove = current.Where(x => !desired.Contains(x.StatusId)).ToList();
    if (toRemove.Count > 0)
        _context.EmployeeEmploymentStatuses.RemoveRange(toRemove);

    // hozzáadandók: ami kell, de még nincs
    var toAddIds = desired.Except(currentIds).ToList();
    if (toAddIds.Count > 0)
    {
        var now = DateTime.UtcNow;
        foreach (var statusId in toAddIds)
        {
            _context.EmployeeEmploymentStatuses.Add(new EmployeeEmploymentStatus
            {
                EmployeeId = employeeId,
                StatusId = statusId,
                AssignedAt = now,
                IsCurrent = true
            });
        }
    }

    // opcionális: IsCurrent/AssignedAt frissítés, ha nálad ez tényleg számít.
    // Ha nincs history és minden sor "current", akkor ez így oké.
}
        // -----------------------------
        // Helper: telephelyek cseréje
        // (create/update-hoz hasznos, és view-ban is ezt látod)
        // -----------------------------
public async Task ReplaceSitesAsync(int employeeId, List<int> siteIds, int? defaultSiteId = null)
{
    siteIds ??= new List<int>();

    var desiredList = siteIds
        .Where(x => x > 0)
        .Distinct()
        .ToList();

    var desired = desiredList.ToHashSet();

    var currentLinks = await _context.EmployeeSites
        .Where(x => x.EmployeeId == employeeId)
        .ToListAsync();

    var currentSiteIds = currentLinks.Select(x => x.SiteId).ToHashSet();

    // remove
    var toRemove = currentLinks.Where(x => !desired.Contains(x.SiteId)).ToList();
    if (toRemove.Count > 0)
        _context.EmployeeSites.RemoveRange(toRemove);

    // add
    var toAdd = desired.Except(currentSiteIds).ToList();
    foreach (var siteId in toAdd)
    {
        _context.EmployeeSites.Add(new EmployeeSite
        {
            EmployeeId = employeeId,
            SiteId = siteId,
            IsPrimary = false
        });
    }

    // primary: frissítsük az összes megmaradó/linkelt site-ra
    // (currentLinks-ben még benne van a toRemove is, ezért újra leképezzük a "maradók" halmazát)
    var remainingSiteIds = currentSiteIds.Intersect(desired).ToList();
    remainingSiteIds.AddRange(toAdd);
    remainingSiteIds = remainingSiteIds.Distinct().ToList();

    if (remainingSiteIds.Count == 0)
        return;

    int primaryId;

    if (defaultSiteId.HasValue && desired.Contains(defaultSiteId.Value))
        primaryId = defaultSiteId.Value;
    else
        primaryId = remainingSiteIds[0]; // fallback

    // meglévők frissítése
    foreach (var link in currentLinks.Where(x => desired.Contains(x.SiteId)))
    {
        link.IsPrimary = (link.SiteId == primaryId);
    }

    // frissen hozzáadott linkeknél EF még nem adja vissza objektumként, ezért külön kezeljük:
    // (egyszerűen hozzáadjuk még egyszer a primary-t, vagy a toAdd loopban állítjuk)
    // -> egyszerűbb: a toAdd loopban állítsuk:
    //   IsPrimary = (siteId == primaryId)

    // Ha ezt választod, akkor a fenti toAdd loopot módosítsd így:
    // IsPrimary = (siteId == primaryId)
}

        // -----------------------------
        // Business rule helpers
        // -----------------------------
        private static void NormalizeWorkerTypePartner(int workerTypeId, ref EmployeesCreateDto dto)
        {
            // 1=INTERNAL, 2=EXTERNAL
            if (workerTypeId == 1)
            {
                dto.PartnerId = null;
                return;
            }

            if (workerTypeId == 2 && dto.PartnerId == null)
                throw new InvalidOperationException("Külsős dolgozónál kötelező PartnerId.");
        }

        private static void NormalizeWorkerTypePartner(int workerTypeId, ref EmployeesUpdateDto dto)
        {
            if (workerTypeId == 1)
            {
                dto.PartnerId = null;
                return;
            }

            if (workerTypeId == 2 && dto.PartnerId == null)
                throw new InvalidOperationException("Külsős dolgozónál kötelező PartnerId.");
        }
    }

    // ---------------------------------
    // Result + DTO-k az AJAX-hoz
    // ---------------------------------
    public class EmployeeIndexResult
    {
        public List<EmployeeIndexRowDto> Items { get; set; } = new();
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }

    public class EmployeeIndexRowDto
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; } = "";
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public int WorkerTypeId { get; set; }
        public string? WorkerTypeName { get; set; }
        public int? PartnerId { get; set; }
        public string? PartnerName { get; set; }
        public int? JobTitleId { get; set; }
        public string? JobTitleName { get; set; }
        public bool IsActive { get; set; }
        public List<string> StatusNames { get; set; } = new();
        public List<string> SiteNames { get; set; } = new();
        public int? DefaultSiteId { get; set; }
        public string? DefaultSiteName { get; set; }
    }

    // ---------------------------------
    // DETAILS DTO - minden create mező + mindenhez név
    // ---------------------------------
    public class EmployeeDetailsDto
    {
        public int EmployeeId { get; set; }

        // Basic
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }

        public string? Email { get; set; }
        public string? Email2 { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PhoneNumber2 { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public DateTime? HireDate { get; set; }

        // Department
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; } // ha van Department tábla: töltsd

        // JobTitle
        public int? JobTitleId { get; set; }
        public string? JobTitleName { get; set; }

        public bool IsActive { get; set; }

        // Default site
        public int? DefaultSiteId { get; set; }
        public string? DefaultSiteName { get; set; }

        // Contract
        public decimal? WorkingTime { get; set; }
        public byte? IsContracted { get; set; }

        // Other create fields
        public string? FamilyData { get; set; }
        public string? Comment1 { get; set; }
        public string? Comment2 { get; set; }
        public int? VacationDays { get; set; }
        public int? FullVacationDays { get; set; }
        public string? TaxId { get; set; }
        public string? TajNumber { get; set; }
        public string? BirthName { get; set; }
        public string? MotherBirthName { get; set; }
        public string? BirthPlace { get; set; }
        public string? NationalityCode { get; set; }
        public string? FeorCode { get; set; }
        public DateTime? EmploymentEndDate { get; set; }
        public string? PermanentAddress { get; set; }
        public string? MailingAddress { get; set; }
        public string? BankAccountIban { get; set; }

        // WorkerType + Partner
        public int WorkerTypeId { get; set; }
        public string? WorkerTypeName { get; set; }

        public int? PartnerId { get; set; }
        public string? PartnerName { get; set; }

        // Status + Sites
        public List<int> StatusIds { get; set; } = new();
        public List<string> StatusNames { get; set; } = new();
        public List<EmployeeStatusDto> Statuses { get; set; } = new();

        public List<int> SiteIds { get; set; } = new();
        public List<string> SiteNames { get; set; } = new();
        public List<EmployeeSiteDto> Sites { get; set; } = new();
    }

    public class EmployeeStatusDto
    {
        public int StatusId { get; set; }
        public string StatusName { get; set; } = "";
        public DateTime? AssignedAt { get; set; }
    }

    public class EmployeeSiteDto
    {
        public int SiteId { get; set; }
        public string SiteName { get; set; } = "";
        public bool IsPrimary { get; set; }
    }

    public class EmployeeIndexQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 30;

        public string? SearchTerm { get; set; }      // a felső kereső
        public string? QuickFilter { get; set; }     // all|active|internal|external

        // Advanced modal mezők:
        public string? Text { get; set; }            // Név/Email contains
        public string? Phone { get; set; }           // Telefon contains
        public int? WorkerTypeId { get; set; }
        public int? PartnerId { get; set; }
        public int? StatusId { get; set; }           // single select (később lehet List<int>)
        public int? SiteId { get; set; }             // single select (később lehet List<int>)
        public bool? ActiveOnly { get; set; }        // Csak aktív
    }

    public class EmployeeAdvancedFilterDto
    {
        public string? Text { get; set; }         // Név/Email contains
        public string? Phone { get; set; }        // Telefon contains
        public int? WorkerTypeId { get; set; }
        public int? PartnerId { get; set; }
        public int? StatusId { get; set; }        // single select
        public int? SiteId { get; set; }          // single select
        public bool? ActiveOnly { get; set; }     // Csak aktív
    }

    // ---------------------------------
    // ServiceResult
    // ---------------------------------
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }

        public static ServiceResult Ok() => new() { Success = true };
        public static ServiceResult Fail(string error) => new() { Success = false, Error = error };
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; set; }

        public static ServiceResult<T> Ok(T data) => new() { Success = true, Data = data };
        public new static ServiceResult<T> Fail(string error) => new() { Success = false, Error = error };
    }
}