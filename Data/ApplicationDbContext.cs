using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Cloud9_2.Models;
using Microsoft.AspNetCore.Identity;
using Cloud9_2.Services.Tenancy;

namespace Cloud9_2.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        private readonly ITenantProvider? _tenantProvider;

        // EF filter ezt fogja használni
        public int? CurrentTenantId { get; private set; }

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            ITenantProvider? tenantProvider = null)
            : base(options)
        {
            _tenantProvider = tenantProvider;
            CurrentTenantId = _tenantProvider?.GetTenantId();
        }


        public DbSet<Partner> Partners { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<PartnerType> PartnerTypes { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<AccessPermission> AccessPermissions { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }
        public DbSet<ColumnVisibility> ColumnVisibilities { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<LeadSource> LeadSources { get; set; }
        public DbSet<Lead> Leads { get; set; }
        public DbSet<LeadHistory> LeadHistories { get; set; }
        public DbSet<UnitOfMeasurement> UnitsOfMeasurement { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<QuoteHistory> QuoteHistories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<CommunicationType> CommunicationTypes { get; set; }
        public DbSet<CustomerCommunication> CustomerCommunications { get; set; }
        public DbSet<CommunicationStatus> CommunicationStatuses { get; set; }
        public DbSet<CommunicationPost> CommunicationPosts { get; set; }
        public DbSet<CommunicationResponsible> CommunicationResponsibles { get; set; }
        public DbSet<VatType> VatTypes { get; set; }
        public DbSet<PartnerGroup> PartnerGroups { get; set; }
        public DbSet<TaskTypePM> TaskTypePMs { get; set; }
        public DbSet<TaskPM> TaskPMs { get; set; }
        public DbSet<TaskCommentPM> TaskCommentsPMs { get; set; }
        public DbSet<TaskAttachmentPM> TaskAttachmentsPMs { get; set; }
        public DbSet<TaskStatusPM> TaskStatusesPM { get; set; }
        public DbSet<TaskPriorityPM> TaskPrioritiesPM { get; set; }
        public DbSet<ProjectPM> ProjectPMs { get; set; }
        public DbSet<ProjectStatusPM> ProjectStatusPMs { get; set; }
        public DbSet<OrderItemDiscount> OrderItemDiscounts { get; set; } // Added for CS1061
        public DbSet<DocumentMetadata> DocumentMetadata { get; set; }
        public DbSet<DocumentLink> DocumentLinks { get; set; }
        public DbSet<DocumentStatusHistory> DocumentStatusHistory { get; set; }
        public DbSet<Status> PartnerStatuses { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<OrderShippingMethod> OrderShippingMethods { get; set; }
        public DbSet<PaymentTerm> PaymentTerms { get; set; }
        public DbSet<Employees> Employees { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<EmploymentStatus> EmploymentStatuses { get; set; }
        public DbSet<Salary> Salaries { get; set; }
        public DbSet<EmployeeShift> EmployeeShifts { get; set; }
        public DbSet<PartnerShift> PartnerShifts { get; set; }
        public DbSet<SiteShift> SiteShifts { get; set; }
        public DbSet<EmployeeHistory> EmployeeHistories { get; set; }
        public DbSet<Vacation> Vacations { get; set; }
        public DbSet<VacationBalance> VacationBalances { get; set; }
        public DbSet<OrderStatusType> OrderStatusTypes { get; set; }
        public DbSet<JobTitle> JobTitles { get; set; }
        public DbSet<ResourceType> ResourceTypes { get; set; }
        public DbSet<ResourceStatus> ResourceStatuses { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<ResourceHistory> ResourceHistories { get; set; }
        public DbSet<TaskResourceAssignment> TaskResourceAssignments { get; set; }
        public DbSet<TaskEmployeeAssignment> TaskEmployeeAssignments { get; set; }
        public DbSet<TaskDocumentLink> TaskDocumentLinks { get; set; }
        public DbSet<TaskHistory> TaskHistories { get; set; }
        public DbSet<TaskPMcomMethod> TaskPMcomMethods { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;
        public DbSet<GFO> GFOs { get; set; } = null!;

        public DbSet<BusinessDocument> BusinessDocuments { get; set; }
        public DbSet<BusinessDocumentLine> BusinessDocumentLines { get; set; }
        public DbSet<BusinessDocumentParty> BusinessDocumentParties { get; set; }
        public DbSet<BusinessDocumentPartyRole> BusinessDocumentPartyRoles { get; set; }
        public DbSet<BusinessDocumentType> BusinessDocumentTypes { get; set; }
        public DbSet<BusinessDocumentStatus> BusinessDocumentStatuses { get; set; }
        public DbSet<BusinessDocumentRelation> BusinessDocumentRelations { get; set; }
        public DbSet<BusinessDocumentRelationType> BusinessDocumentRelationTypes { get; set; }
        public DbSet<BusinessDocumentAttachment> BusinessDocumentAttachments { get; set; }
        public DbSet<AttachmentCategoryLookup> AttachmentCategories { get; set; }
        public DbSet<BusinessDocumentStatusHistory> BusinessDocumentStatusHistories { get; set; }
        public DbSet<ElectronicDocument> ElectronicDocuments { get; set; }
        public DbSet<PriceType> PriceTypes { get; set; } = null!;
        public DbSet<ItemPrice> ItemPrices { get; set; } = null!;



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<PriceType>()
                .HasIndex(x => x.Code)
                .IsUnique();

            modelBuilder.Entity<ItemPrice>()
                .HasIndex(x => new { x.TenantId, x.ItemId });

            modelBuilder.Entity<ItemPrice>()
                .HasIndex(x => new { x.TenantId, x.PriceTypeId });

            modelBuilder.Entity<ItemPrice>()
                .HasIndex(x => new { x.TenantId, x.ItemId, x.PriceTypeId, x.CurrencyId, x.IsActive, x.ValidFrom, x.ValidTo });

            // opcionális, de nagyon hasznos:
            // egyszerre 1 aktív "current" ár (ValidTo IS NULL) adott item+currency+type-ra
            modelBuilder.Entity<ItemPrice>()
                .HasIndex(x => new { x.TenantId, x.ItemId, x.CurrencyId, x.PriceTypeId })
                .IsUnique()
                .HasFilter("[IsActive] = 1 AND [ValidTo] IS NULL");


            var tenantId = _tenantProvider?.GetTenantId();

            // Csak runtime-ban legyen filter.
            // Migráció készítésnél tenantId == null lehet -> akkor nem szűrünk.
            if (tenantId != null)
            {
                modelBuilder.Entity<BusinessDocument>()
                    .HasQueryFilter(x => !x.IsDeleted && x.TenantId == tenantId.Value);

                modelBuilder.Entity<BusinessDocumentLine>()
                    .HasQueryFilter(x => x.TenantId == tenantId.Value);

                modelBuilder.Entity<BusinessDocumentParty>()
                    .HasQueryFilter(x => x.TenantId == tenantId.Value);

                modelBuilder.Entity<BusinessDocumentRelation>()
                    .HasQueryFilter(x => x.TenantId == tenantId.Value);

                modelBuilder.Entity<BusinessDocumentAttachment>()
                    .HasQueryFilter(x => x.TenantId == tenantId.Value);

                modelBuilder.Entity<BusinessDocumentStatusHistory>()
                    .HasQueryFilter(x => x.TenantId == tenantId.Value);

                // Lookup tábláknál általában nincs tenant (globális).
                // Ha nálad tenant-specifikusak lesznek, akkor oda is mehet.
            }

            modelBuilder.Entity<PriceType>()
    .HasIndex(x => x.Code)
    .IsUnique();

            modelBuilder.Entity<ItemPrice>()
                .HasIndex(x => new { x.TenantId, x.ItemId });

            modelBuilder.Entity<ItemPrice>()
                .HasIndex(x => new { x.TenantId, x.PriceTypeId });

            modelBuilder.Entity<ItemPrice>()
                .HasIndex(x => new { x.TenantId, x.ItemId, x.PriceTypeId, x.CurrencyId, x.IsActive, x.ValidFrom, x.ValidTo });

            // opcionális: “current price” uniqueness (ValidTo IS NULL és aktív)
            modelBuilder.Entity<ItemPrice>()
                .HasIndex(x => new { x.TenantId, x.ItemId, x.CurrencyId, x.PriceTypeId })
                .IsUnique()
                .HasFilter("[IsActive] = 1 AND [ValidTo] IS NULL");

            modelBuilder.Entity<BusinessDocumentRelation>(entity =>
{
    entity.HasKey(x => x.BusinessDocumentRelationId);

    entity.HasOne(x => x.FromBusinessDocument)
          .WithMany(d => d.FromRelations)
          .HasForeignKey(x => x.FromBusinessDocumentId)
          .OnDelete(DeleteBehavior.Restrict);

    entity.HasOne(x => x.ToBusinessDocument)
          .WithMany(d => d.ToRelations)
          .HasForeignKey(x => x.ToBusinessDocumentId)
          .OnDelete(DeleteBehavior.Restrict);

    entity.HasIndex(x => new { x.TenantId, x.FromBusinessDocumentId, x.ToBusinessDocumentId, x.BusinessDocumentRelationTypeId })
          .IsUnique();
});



            modelBuilder.Entity<Partner>().ToTable("Partners");

            modelBuilder.Entity<TaskPM>()
                .HasOne(t => t.Partner)
                .WithMany()
                .HasForeignKey(t => t.PartnerId)
                .OnDelete(DeleteBehavior.Restrict);




            // ----------------------------------------------------------------
            modelBuilder.Entity<TaskPM>()
                .HasOne(t => t.TaskPMcomMethod)
                .WithMany() // lookup, nincs visszanavigáció
                .HasForeignKey(t => t.TaskPMcomMethodID)
                .OnDelete(DeleteBehavior.Restrict);

            // (opcionális) tábla nevek fixálása, ha kell
            modelBuilder.Entity<TaskPMcomMethod>()
                .ToTable("TaskPMcomMethod");

            // (opcionális) string hosszok / indexek
            modelBuilder.Entity<TaskPMcomMethod>()
                .Property(x => x.Nev)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasIndex(e => e.EntityType);
                entity.HasIndex(e => e.ChangedAt);
                entity.HasIndex(e => e.ChangedById);
                entity.Property(e => e.Changes).HasMaxLength(2000); // vagy nagyobb, ha kell
            });

            modelBuilder.Entity<Partner>(entity =>
            {
                // Computed columnok – EF ne próbálja frissíteni/insertelni őket
                entity.Property(p => p.CompanyNameTrim)
                    .HasComputedColumnSql("CASE WHEN [CompanyName] IS NULL THEN NULL ELSE CONVERT([nvarchar](450), LEFT(LTRIM(RTRIM([CompanyName])), 450)) END PERSISTED")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(p => p.NameTrim)
                    .HasComputedColumnSql("CASE WHEN [Name] IS NULL THEN NULL ELSE CONVERT([nvarchar](450), LEFT(LTRIM(RTRIM([Name])), 450)) END PERSISTED")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(p => p.TaxIdTrim)
                    .HasComputedColumnSql("CASE WHEN [TaxId] IS NULL THEN NULL ELSE CONVERT([nvarchar](50), LEFT(LTRIM(RTRIM([TaxId])), 50)) END PERSISTED")
                    .ValueGeneratedOnAddOrUpdate();
            });


            modelBuilder.Entity<ResourceHistory>().ToTable("ResourceHistory");

            modelBuilder.Entity<TaskHistory>().ToTable("TaskHistory");

            modelBuilder.Entity<GFO>().ToTable("GFO");

            modelBuilder.Entity<ResourceHistory>(entity =>
            {
                entity.HasKey(e => e.ResourceHistoryId);

                entity.Property(e => e.ModifiedDate)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.ServicePrice)
                    .HasColumnType("decimal(18,2)");

                entity.HasOne(e => e.Resource)
                    .WithMany(r => r.ResourceHistories) // if you have inverse
                    .HasForeignKey(e => e.ResourceId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ModifiedBy)
                    .WithMany()
                    .HasForeignKey(e => e.ModifiedById)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<TaskDocumentLink>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Task)
                    .WithMany(t => t.TaskDocuments)
                    .HasForeignKey(e => e.TaskId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Document)
                    .WithMany(d => d.TaskDocuments)
                    .HasForeignKey(e => e.DocumentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.LinkedBy)
                    .WithMany()
                    .HasForeignKey(e => e.LinkedById)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Partner>()
            .HasMany(p => p.Sites)
            .WithOne(s => s.Partner)
            .HasForeignKey(s => s.PartnerId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Partner>()
                .HasMany(p => p.Contacts)
                .WithOne(c => c.Partner)
                .HasForeignKey(c => c.PartnerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Partner>()
                .HasMany(p => p.Documents)
                .WithOne(d => d.Partner)
                .HasForeignKey(d => d.PartnerId)
                .OnDelete(DeleteBehavior.Cascade);

            // TaskPM
            modelBuilder.Entity<TaskPM>(entity =>
            {
                entity.ToTable("TaskPM");
                entity.Property(t => t.AssignedToId).HasColumnName("AssignedTo");
                entity.Property(t => t.CreatedById).HasColumnName("CreatedBy");

                entity.HasOne(t => t.AssignedTo)
                      .WithMany()
                      .HasForeignKey(t => t.AssignedToId)
                      .HasPrincipalKey(u => u.Id);

                entity.HasOne(t => t.CreatedBy)
                      .WithMany()
                      .HasForeignKey(t => t.CreatedById)
                      .HasPrincipalKey(u => u.Id);
            });

            // ProjectPM
            modelBuilder.Entity<ProjectPM>(entity =>
            {
                entity.ToTable("ProjectPM");
                entity.HasKey(p => p.ProjectPMId);
                // Add other config if needed
            });




            modelBuilder.Entity<TaskResourceAssignment>().ToTable("TaskResourceAssignment");

            modelBuilder.Entity<TaskEmployeeAssignment>().ToTable("TaskEmployeeAssignment");

            modelBuilder.Entity<TaskPM>().ToTable("TaskPM");

            modelBuilder.Entity<Resource>().ToTable("Resource");

            modelBuilder.Entity<ResourceType>().ToTable("ResourceType");

            modelBuilder.Entity<ResourceStatus>().ToTable("ResourceStatus");

            modelBuilder.Entity<TaskPriorityPM>().ToTable("TaskPriorityPM");

            modelBuilder.Entity<TaskStatusPM>().ToTable("TaskStatusPM");

            // Map TaskTypePM
            modelBuilder.Entity<TaskTypePM>().ToTable("TaskTypePM");

            modelBuilder.Entity<TaskTypePM>()
            .Property(t => t.CreatedAt)
            .HasColumnName("CreatedAt");

            modelBuilder.Entity<TaskTypePM>()
                .Property(t => t.UpdatedAt)
                .HasColumnName("UpdatedAt");

            // HR
            // Map Employee to singular table name
            modelBuilder.Entity<Employees>().ToTable("Employee");

            modelBuilder.Entity<Employees>().HasQueryFilter(e => e.IsActive);

            // Map EmploymentStatus to its table
            modelBuilder.Entity<EmploymentStatus>().ToTable("EmploymentStatus");

            // Map StatusName to Name column in EmploymentStatus
            modelBuilder.Entity<EmploymentStatus>()
                .Property(es => es.StatusName)
                .HasColumnName("Name");

            // Map created_at and updated_at columns
            modelBuilder.Entity<EmploymentStatus>()
                .Property(es => es.CreatedAt)
                .HasColumnName("created_at");

            modelBuilder.Entity<EmploymentStatus>()
                .Property(es => es.UpdatedAt)
                .HasColumnName("updated_at");

            // Configure Employee -> EmploymentStatus relationship
            modelBuilder.Entity<Employees>()
                .HasOne(e => e.Status)
                .WithMany(es => es.Employees)
                .HasForeignKey(e => e.StatusId)
                .OnDelete(DeleteBehavior.SetNull);

            // Map JobTitle to its table
            modelBuilder.Entity<JobTitle>().ToTable("JobTitle");
            modelBuilder.Entity<JobTitle>()
                .Property(jt => jt.CreatedAt)
                .HasColumnName("CreatedAt");
            modelBuilder.Entity<JobTitle>()
                .Property(jt => jt.UpdatedAt)
                .HasColumnName("UpdatedAt");


            modelBuilder.Entity<EmployeeShift>()
                .HasKey(es => new { es.EmployeeId, es.ShiftId });

            modelBuilder.Entity<PartnerShift>()
                .HasKey(ps => new { ps.PartnerId, ps.ShiftId });

            modelBuilder.Entity<SiteShift>()
                .HasKey(ss => new { ss.SiteId, ss.ShiftId });

            modelBuilder.Entity<Site>().HasQueryFilter(s => s.IsActive);


            modelBuilder.Entity<Site>()
            .HasMany(s => s.CustomerCommunications)
            .WithOne(c => c.Site)
            .HasForeignKey(c => c.SiteId)
            .OnDelete(DeleteBehavior.SetNull);

            // Order configuration
            modelBuilder.Entity<Order>(entity =>
                    {
                        entity.HasKey(e => e.OrderId);

                        entity.Property(e => e.OrderNumber)
                            .HasMaxLength(100);

                        entity.Property(e => e.OrderDate)
                            .HasColumnType("date");

                        entity.Property(e => e.Deadline)
                            .HasColumnType("date");

                        entity.Property(e => e.Description)
                            .HasMaxLength(500);

                        entity.Property(e => e.TotalAmount)
                            .HasColumnType("decimal(18,2)");

                        entity.Property(e => e.SalesPerson)
                            .HasMaxLength(100);

                        entity.Property(e => e.DeliveryDate)
                            .HasColumnType("date");

                        entity.Property(e => e.PlannedDelivery)
                            .HasColumnType("datetime");

                        entity.Property(e => e.DiscountPercentage)
                            .HasColumnType("decimal(5,2)");

                        entity.Property(e => e.DiscountAmount)
                            .HasColumnType("decimal(18,2)");

                        entity.Property(e => e.CompanyName)
                            .HasMaxLength(100);

                        entity.Property(e => e.Subject)
                            .HasMaxLength(200);

                        entity.Property(e => e.CreatedBy)
                            .HasMaxLength(100)
                            .HasDefaultValue("System");

                        entity.Property(e => e.CreatedDate)
                            .HasColumnType("datetime")
                            .HasDefaultValueSql("GETUTCDATE()");

                        entity.Property(e => e.ModifiedBy)
                            .HasMaxLength(100)
                            .HasDefaultValue("System");

                        entity.Property(e => e.ModifiedDate)
                            .HasColumnType("datetime")
                            .HasDefaultValueSql("GETUTCDATE()");

                        entity.Property(e => e.Status)
                            .HasMaxLength(50)
                            .HasDefaultValue("Pending");

                        entity.Property(e => e.ReferenceNumber)
                            .HasMaxLength(100);

                        // Relationships
                        entity.HasOne(e => e.Partner)
                            .WithMany()
                            .HasForeignKey(e => e.PartnerId)
                            .OnDelete(DeleteBehavior.Restrict);

                        entity.HasOne(e => e.Site)
                            .WithMany()
                            .HasForeignKey(e => e.SiteId)
                            .OnDelete(DeleteBehavior.SetNull);

                        entity.HasOne(e => e.Currency)
                            .WithMany()
                            .HasForeignKey(e => e.CurrencyId)
                            .OnDelete(DeleteBehavior.Restrict);

                        entity.HasOne(e => e.Quote)
                            .WithMany()
                            .HasForeignKey(e => e.QuoteId)
                            .OnDelete(DeleteBehavior.SetNull);

                        entity.HasOne(e => e.ShippingMethod)
                            .WithMany(sm => sm.Orders)
                            .HasForeignKey(e => e.ShippingMethodId)
                            .OnDelete(DeleteBehavior.SetNull);

                        entity.HasOne(e => e.PaymentTerm)
                            .WithMany(pt => pt.Orders)
                            .HasForeignKey(e => e.PaymentTermId)
                            .OnDelete(DeleteBehavior.SetNull);

                    });

            // OrderItemDiscount configuration
            modelBuilder.Entity<OrderItemDiscount>(entity =>
            {
                entity.HasKey(e => e.OrderItemDiscountId);

                entity.Property(e => e.DiscountPercentage)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.DiscountAmount)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.BasePrice)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.PartnerPrice)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.VolumePrice)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.ListPrice)
                    .HasColumnType("decimal(18,2)");

            });

            // OrderShippingMethod configuration
            modelBuilder.Entity<OrderShippingMethod>(entity =>
            {
                entity.HasKey(e => e.ShippingMethodId);

                entity.Property(e => e.MethodName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Description)
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .HasDefaultValue("System");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(100)
                    .HasDefaultValue("System");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasIndex(e => e.MethodName)
                    .IsUnique();
            });

            // PaymentTerm configuration
            modelBuilder.Entity<PaymentTerm>(entity =>
            {
                entity.HasKey(e => e.PaymentTermId);

                entity.Property(e => e.TermName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Description)
                    .HasMaxLength(500);

                entity.Property(e => e.DaysDue);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .HasDefaultValue("System");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(100)
                    .HasDefaultValue("System");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasIndex(e => e.TermName)
                    .IsUnique();
            });


            modelBuilder.Entity<Status>().ToTable("PartnerStatuses");
            modelBuilder.Entity<Status>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<PartnerDto>()
                .HasOne(p => p.Status)
                .WithMany()
                .HasForeignKey(p => p.StatusId);

            modelBuilder.Entity<Site>()
                .Property(s => s.PartnerId)
                .IsRequired();

            modelBuilder.Entity<Document>()
                .HasOne(d => d.Partner)
                .WithMany(p => p.Documents)
                .HasForeignKey(d => d.PartnerId)
                .OnDelete(DeleteBehavior.Restrict); // vagy Cascade, lásd lent


            modelBuilder.Entity<Document>()
                .HasMany(d => d.DocumentMetadata)
                .WithOne(m => m.Document)
                .HasForeignKey(m => m.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Document>()
                .HasMany(d => d.DocumentLinks)
                .WithOne(l => l.Document)
                .HasForeignKey(l => l.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Document>()
                .HasMany(d => d.StatusHistory)
                .WithOne(s => s.Document)
                .HasForeignKey(s => s.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Document>()
                .Property(d => d.Status)
                .HasConversion<int>();

            modelBuilder.Entity<DocumentStatusHistory>()
                .Property(h => h.OldStatus)
                .HasConversion<int>();
            modelBuilder.Entity<DocumentStatusHistory>()
                .Property(h => h.NewStatus)
                .HasConversion<int>();



            modelBuilder.Entity<TaskPM>(entity =>
            {
                entity.Property(t => t.IsActive).HasDefaultValue(true);

                // Relationships
                entity.HasOne(t => t.TaskTypePM)
                    .WithMany()
                    .HasForeignKey(t => t.TaskTypePMId);

                // entity.HasOne(t => t.ProjectPM)
                //     .WithMany(p => p.Tasks)
                //     .HasForeignKey(t => t.ProjectPMId);

            });

            // modelBuilder.Entity<TaskPM>()
            //     .HasOne(t => t.ProjectPM)
            //     .WithMany(p => p.Tasks)
            //     .HasForeignKey(t => t.ProjectPMId)
            //     .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<TaskTypePM>(entity =>
                {
                    entity.HasKey(tt => tt.TaskTypePMId);
                    entity.Property(tt => tt.TaskTypePMName).IsRequired().HasMaxLength(50);
                });

            modelBuilder.Entity<TaskPM>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Title).IsRequired().HasMaxLength(100);

                entity.HasOne(t => t.TaskTypePM)
                      .WithMany(tt => tt.Tasks)
                      .HasForeignKey(t => t.TaskTypePMId);

                // entity.HasOne(t => t.Project)
                //       .WithMany(p => p.Tasks)
                //       .HasForeignKey(t => t.ProjectId);

                entity.HasOne(t => t.CreatedBy)
                      .WithMany()
                      .HasForeignKey(t => t.CreatedById);

                entity.HasOne(t => t.AssignedTo)
                      .WithMany()
                      .HasForeignKey(t => t.AssignedToId);
            });

            modelBuilder.Entity<TaskCommentPM>(entity =>
            {
                entity.HasKey(tc => tc.TaskCommentPMId);

                entity.HasOne(tc => tc.TaskPM)
                      .WithMany(t => t.Comments)
                      .HasForeignKey(tc => tc.TaskPMId);

                entity.HasOne(tc => tc.CreatedBy)
                      .WithMany()
                      .HasForeignKey(tc => tc.CreatedById);
            });

            modelBuilder.Entity<TaskAttachmentPM>(entity =>
            {
                entity.HasKey(ta => ta.TaskAttachmentPMId);

                entity.HasOne(ta => ta.TaskPM)
                      .WithMany(t => t.Attachments)
                      .HasForeignKey(ta => ta.TaskPMId);

                entity.HasOne(ta => ta.UploadedBy)
                      .WithMany()
                      .HasForeignKey(ta => ta.UploadedById);
            });


            // Configure PartnerGroup relationship
            modelBuilder.Entity<Partner>()
                .HasOne(p => p.PartnerGroup)
                .WithMany(pg => pg.Partners)
                .HasForeignKey(p => p.PartnerGroupId)
                .OnDelete(DeleteBehavior.SetNull); // Optional: Set PartnerGroupId to null if group is deleted


            // Configure unique indexes
            modelBuilder.Entity<VatType>()
                .HasIndex(v => v.TypeName)
                .IsUnique();

            //prices
            modelBuilder.Entity<VatType>().HasData(
                new VatType { VatTypeId = 1, TypeName = "27%", Rate = 27.00m }
            );

            modelBuilder.Entity<PartnerGroup>().HasData(
                new PartnerGroup { PartnerGroupId = 1, PartnerGroupName = "VIP Customers", DiscountPercentage = 5.00m }
            );

            // CustomerCommunication
            modelBuilder.Entity<CustomerCommunication>()
                .HasKey(c => c.CustomerCommunicationId);

            modelBuilder.Entity<CustomerCommunication>()
                .HasOne(c => c.CommunicationType)
                .WithMany(ct => ct.CustomerCommunications)
                .HasForeignKey(c => c.CommunicationTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CustomerCommunication>()
                .HasOne(c => c.Status)
                .WithMany(cs => cs.CustomerCommunications)
                .HasForeignKey(c => c.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CustomerCommunication>()
                .HasOne(c => c.Contact)
                .WithMany(c => c.CustomerCommunications)
                .HasForeignKey(c => c.ContactId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CustomerCommunication>()
                .HasOne(c => c.Agent)
                .WithMany(u => u.CustomerCommunications)
                .HasForeignKey(c => c.AgentId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<CustomerCommunication>()
                .HasOne(c => c.Partner)
                .WithMany(p => p.Communications)
                .HasForeignKey(c => c.PartnerId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<CustomerCommunication>()
                .HasOne(c => c.Lead)
                .WithMany(l => l.Communications)
                .HasForeignKey(c => c.LeadId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<CustomerCommunication>()
                .HasOne(c => c.Quote)
                .WithMany(q => q.Communications)
                .HasForeignKey(c => c.QuoteId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<CustomerCommunication>()
                .HasOne(c => c.Order)
                .WithMany(o => o.Communications)
                .HasForeignKey(c => c.OrderId)
                .OnDelete(DeleteBehavior.SetNull);

            // CommunicationResponsible
            modelBuilder.Entity<CommunicationResponsible>()
                .HasKey(cr => cr.CommunicationResponsibleId);

            modelBuilder.Entity<CommunicationResponsible>()
                .HasOne(cr => cr.CustomerCommunication)
                .WithMany(c => c.ResponsibleHistory)
                .HasForeignKey(cr => cr.CustomerCommunicationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CommunicationResponsible>()
                .HasOne(cr => cr.Responsible)
                .WithMany()
                .HasForeignKey(cr => cr.ResponsibleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CommunicationResponsible>()
                .HasOne(cr => cr.AssignedBy)
                .WithMany()
                .HasForeignKey(cr => cr.AssignedById)
                .OnDelete(DeleteBehavior.Restrict);

            // CommunicationPost
            modelBuilder.Entity<CommunicationPost>()
                .HasKey(p => p.CommunicationPostId);

            modelBuilder.Entity<CommunicationPost>()
                .HasOne(p => p.CustomerCommunication)
                .WithMany(c => c.Posts)
                .HasForeignKey(p => p.CustomerCommunicationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CommunicationPost>()
                .HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Disable auto-include
            modelBuilder.Entity<CustomerCommunication>()
                .Navigation(c => c.Posts)
                .AutoInclude(false);

            modelBuilder.Entity<CustomerCommunication>()
                .Navigation(c => c.ResponsibleHistory)
                .AutoInclude(false);

            // Order configuration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderId);
                entity.Property(e => e.OrderNumber)
                    .HasMaxLength(100);
                entity.Property(e => e.OrderDate)
                    .HasColumnType("date");
                entity.Property(e => e.Deadline)
                    .HasColumnType("date");
                entity.Property(e => e.Description)
                    .HasMaxLength(500);
                entity.Property(e => e.TotalAmount)
                    .HasColumnType("decimal(18,2)");
                entity.Property(e => e.SalesPerson)
                    .HasMaxLength(100);
                entity.Property(e => e.DeliveryDate)
                    .HasColumnType("date");
                entity.Property(e => e.DiscountPercentage)
                    .HasColumnType("decimal(5,2)");
                entity.Property(e => e.DiscountAmount)
                    .HasColumnType("decimal(18,2)");
                entity.Property(e => e.CompanyName)
                    .HasMaxLength(100);
                entity.Property(e => e.Subject)
                    .HasMaxLength(200);
                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .HasDefaultValue("System");
                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(100)
                    .HasDefaultValue("System");
                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasDefaultValue("Pending");
                entity.Property(e => e.ReferenceNumber)
                    .HasMaxLength(100);
                // entity.Property(e => e.PaymentTerms)
                //     .HasMaxLength(100);
                // entity.Property(e => e.ShippingMethod)
                //     .HasMaxLength(100);

                // Relationships
                entity.HasOne(e => e.Partner)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(e => e.PartnerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Site)
                    .WithMany(s => s.Orders)
                    .HasForeignKey(e => e.SiteId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Currency)
                    .WithMany(c => c.Orders)
                    .HasForeignKey(e => e.CurrencyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Quote)
                    .WithMany(q => q.Orders)
                    .HasForeignKey(e => e.QuoteId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Index
                entity.HasIndex(e => e.OrderNumber).IsUnique();
            });

            // Configure Partner-Lead one-to-many relationship
            modelBuilder.Entity<Lead>()
                .HasOne(l => l.Partner)
                .WithMany(p => p.Leads)
                .HasForeignKey(l => l.PartnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Lead>().HasQueryFilter(l => l.IsActive);

            modelBuilder.Entity<Quote>(entity =>
            {
                entity.Property(e => e.QuoteNumber).IsRequired(false);
                entity.Property(e => e.Description).IsRequired(false);
                entity.Property(e => e.SalesPerson).IsRequired(false);
                entity.Property(e => e.Subject).IsRequired(false);
                entity.Property(e => e.DetailedDescription).IsRequired(false);
                entity.Property(e => e.CompanyName).IsRequired(false);
                entity.Property(e => e.CreatedBy).IsRequired(false);
                entity.Property(e => e.ModifiedBy).IsRequired(false);
                entity.Property(e => e.Status).IsRequired(false);
            });


            modelBuilder.Entity<Quote>().HasQueryFilter(q => q.IsActive);

            modelBuilder.Entity<Quote>()
            .HasMany(q => q.QuoteHistories)
            .WithOne(qi => qi.Quote)
            .HasForeignKey(qi => qi.QuoteId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Quote>()
                .HasIndex(q => q.QuoteNumber)
                .IsUnique();

            modelBuilder.Entity<Contact>()
            .HasOne(c => c.Partner)
            .WithMany(p => p.Contacts)
            .HasForeignKey(c => c.PartnerId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Contact>().HasQueryFilter(c => c.IsActive);

            modelBuilder.Entity<Quote>()
                .HasOne(q => q.Partner)
                .WithMany(p => p.Quotes)
                .HasForeignKey(q => q.PartnerId);

            modelBuilder.Entity<Partner>()
                .HasMany(p => p.Quotes)
                .WithOne(q => q.Partner)
                .HasForeignKey(q => q.PartnerId);

            modelBuilder.Entity<Partner>().HasQueryFilter(p => p.IsActive);

            modelBuilder.Entity<UnitOfMeasurement>()
            .HasOne(u => u.Creator)
            .WithMany()
            .HasForeignKey(u => u.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UnitOfMeasurement>()
            .HasOne(u => u.LastModifier)
            .WithMany()
            .HasForeignKey(u => u.LastModifiedBy)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Currency>()
            .HasOne(c => c.Creator)
            .WithMany()
            .HasForeignKey(c => c.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Currency>()
            .HasOne(c => c.LastModifier)
            .WithMany()
            .HasForeignKey(c => c.LastModifiedBy)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Warehouse>()
            .HasOne(w => w.Creator)
            .WithMany()
            .HasForeignKey(w => w.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Warehouse>()
            .HasOne(w => w.LastModifier)
            .WithMany()
            .HasForeignKey(w => w.LastModifiedBy)
            .OnDelete(DeleteBehavior.NoAction);
        }

        public override int SaveChanges()
        {
            ApplyTenantId();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyTenantId();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyTenantId()
        {
            if (_tenantProvider == null) return;

            int? tenantId = _tenantProvider.GetTenantId();
            if (tenantId == null) return; // nincs tenant (pl. migráció/model build), ne írjunk semmit

            foreach (var entry in ChangeTracker.Entries<ITenantEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    // UI ne tudjon "más tenant"-et beadni
                    entry.Entity.TenantId = tenantId.Value;
                }
                else if (entry.State == EntityState.Modified)
                {
                    // Extra védelem: ne lehessen TenantId-t átírni
                    entry.Property(nameof(ITenantEntity.TenantId)).IsModified = false;
                }
            }
        }

    }
}