using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cloud9._2.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessPermissions",
                columns: table => new
                {
                    AccessPermissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CanViewPage = table.Column<bool>(type: "bit", nullable: false),
                    ColumnName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CanViewColumn = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessPermissions", x => x.AccessPermissionId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MustChangePassword = table.Column<bool>(type: "bit", nullable: true),
                    Disabled = table.Column<bool>(type: "bit", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttachmentCategories",
                columns: table => new
                {
                    AttachmentCategoryId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachmentCategories", x => x.AttachmentCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChangedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Changes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BusinessDocumentPartyRoles",
                columns: table => new
                {
                    BusinessDocumentPartyRoleId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessDocumentPartyRoles", x => x.BusinessDocumentPartyRoleId);
                });

            migrationBuilder.CreateTable(
                name: "BusinessDocumentRelationTypes",
                columns: table => new
                {
                    BusinessDocumentRelationTypeId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessDocumentRelationTypes", x => x.BusinessDocumentRelationTypeId);
                });

            migrationBuilder.CreateTable(
                name: "BusinessDocumentStatuses",
                columns: table => new
                {
                    BusinessDocumentStatusId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessDocumentStatuses", x => x.BusinessDocumentStatusId);
                });

            migrationBuilder.CreateTable(
                name: "BusinessDocumentTypes",
                columns: table => new
                {
                    BusinessDocumentTypeId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessDocumentTypes", x => x.BusinessDocumentTypeId);
                });

            migrationBuilder.CreateTable(
                name: "ColumnVisibilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PageName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ColumnName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColumnVisibilities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommunicationStatuses",
                columns: table => new
                {
                    StatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunicationStatuses", x => x.StatusId);
                });

            migrationBuilder.CreateTable(
                name: "CommunicationTypes",
                columns: table => new
                {
                    CommunicationTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "NVARCHAR(50)", nullable: false),
                    method_name = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true),
                    sort_order = table.Column<short>(type: "smallint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunicationTypes", x => x.CommunicationTypeId);
                });

            migrationBuilder.CreateTable(
                name: "DocumentStatuses",
                columns: table => new
                {
                    DocumentStatusId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusColor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentStatuses", x => x.DocumentStatusId);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTypes",
                columns: table => new
                {
                    DocumentTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTypes", x => x.DocumentTypeId);
                });

            migrationBuilder.CreateTable(
                name: "ElectronicDocument",
                columns: table => new
                {
                    ElectronicDocumentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    CreatorUserId = table.Column<int>(type: "int", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VoteMaxValue = table.Column<int>(type: "int", nullable: true),
                    VoteSplitValue = table.Column<int>(type: "int", nullable: true),
                    isVoteHalfValueEnabled = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectronicDocument", x => x.ElectronicDocumentId);
                });

            migrationBuilder.CreateTable(
                name: "EmploymentStatus",
                columns: table => new
                {
                    StatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentStatus", x => x.StatusId);
                });

            migrationBuilder.CreateTable(
                name: "GFO",
                columns: table => new
                {
                    GFOId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GFOKod = table.Column<int>(type: "int", nullable: false),
                    GFOName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangeType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GFO", x => x.GFOId);
                });

            migrationBuilder.CreateTable(
                name: "ItemTypes",
                columns: table => new
                {
                    ItemTypeId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemTypes", x => x.ItemTypeId);
                });

            migrationBuilder.CreateTable(
                name: "JobTitle",
                columns: table => new
                {
                    JobTitleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TitleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTitle", x => x.JobTitleId);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemDiscounts",
                columns: table => new
                {
                    OrderItemDiscountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderItemId = table.Column<int>(type: "int", nullable: false),
                    DiscountType = table.Column<int>(type: "int", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PartnerPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    VolumeThreshold = table.Column<int>(type: "int", nullable: true),
                    VolumePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ListPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemDiscounts", x => x.OrderItemDiscountId);
                });

            migrationBuilder.CreateTable(
                name: "OrderShippingMethods",
                columns: table => new
                {
                    ShippingMethodId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MethodName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, defaultValue: "System"),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, defaultValue: "System"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderShippingMethods", x => x.ShippingMethodId);
                });

            migrationBuilder.CreateTable(
                name: "OrderStatusTypes",
                columns: table => new
                {
                    OrderStatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatusTypes", x => x.OrderStatusId);
                });

            migrationBuilder.CreateTable(
                name: "PartnerGroups",
                columns: table => new
                {
                    PartnerGroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartnerGroupName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerGroups", x => x.PartnerGroupId);
                });

            migrationBuilder.CreateTable(
                name: "PartnerStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PartnerTypes",
                columns: table => new
                {
                    PartnerTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartnerTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerTypes", x => x.PartnerTypeId);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTerms",
                columns: table => new
                {
                    PaymentTermId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TermName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DaysDue = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, defaultValue: "System"),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, defaultValue: "System"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTerms", x => x.PaymentTermId);
                });

            migrationBuilder.CreateTable(
                name: "PriceTypes",
                columns: table => new
                {
                    PriceTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceTypes", x => x.PriceTypeId);
                });

            migrationBuilder.CreateTable(
                name: "ProjectStatusPMs",
                columns: table => new
                {
                    ProjectStatusPMId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ColorCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectStatusPMs", x => x.ProjectStatusPMId);
                });

            migrationBuilder.CreateTable(
                name: "ResourceStatus",
                columns: table => new
                {
                    ResourceStatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceStatus", x => x.ResourceStatusId);
                });

            migrationBuilder.CreateTable(
                name: "ResourceType",
                columns: table => new
                {
                    ResourceTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceType", x => x.ResourceTypeId);
                });

            migrationBuilder.CreateTable(
                name: "SiteTypes",
                columns: table => new
                {
                    SiteTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteTypes", x => x.SiteTypeId);
                });

            migrationBuilder.CreateTable(
                name: "StatusDto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusDto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskPMcomMethod",
                columns: table => new
                {
                    TaskPMcomMethodID = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nev = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    kod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    leiras = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    aktiv = table.Column<bool>(type: "bit", nullable: false),
                    sorrend = table.Column<short>(type: "smallint", nullable: false),
                    letrehozva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    modositva = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskPMcomMethod", x => x.TaskPMcomMethodID);
                });

            migrationBuilder.CreateTable(
                name: "TaskPriorityPM",
                columns: table => new
                {
                    TaskPriorityPMId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    PriorityColorCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskPriorityPM", x => x.TaskPriorityPMId);
                });

            migrationBuilder.CreateTable(
                name: "TaskStatusPM",
                columns: table => new
                {
                    TaskStatusPMId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: true),
                    DisplayType = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    ColorCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStatusPM", x => x.TaskStatusPMId);
                });

            migrationBuilder.CreateTable(
                name: "TaskTypePM",
                columns: table => new
                {
                    TaskTypePMId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskTypePMName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayType = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTypePM", x => x.TaskTypePMId);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    TenantId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.TenantId);
                });

            migrationBuilder.CreateTable(
                name: "UserActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoginTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LogoutTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActivities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VatTypes",
                columns: table => new
                {
                    VatTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VatTypes", x => x.VatTypeId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    CurrencyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrencyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Locale = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsBaseCurrency = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.CurrencyId);
                    table.ForeignKey(
                        name: "FK_Currencies_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Currencies_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UnitsOfMeasurement",
                columns: table => new
                {
                    UnitOfMeasurementId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsBaseUnit = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitsOfMeasurement", x => x.UnitOfMeasurementId);
                    table.ForeignKey(
                        name: "FK_UnitsOfMeasurement_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UnitsOfMeasurement_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Warehouses",
                columns: table => new
                {
                    WarehouseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.WarehouseId);
                    table.ForeignKey(
                        name: "FK_Warehouses_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Warehouses_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ItemTypeId = table.Column<int>(type: "int", nullable: false),
                    IsStockManaged = table.Column<bool>(type: "bit", nullable: false),
                    IsManufactured = table.Column<bool>(type: "bit", nullable: false),
                    DefaultUnitId = table.Column<int>(type: "int", nullable: true),
                    DefaultTaxCodeId = table.Column<int>(type: "int", nullable: true),
                    DefaultSalesPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    DefaultPurchasePrice = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_Items_ItemTypes_ItemTypeId",
                        column: x => x.ItemTypeId,
                        principalTable: "ItemTypes",
                        principalColumn: "ItemTypeId");
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email2 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    PhoneNumber2 = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    HireDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    JobTitleId = table.Column<int>(type: "int", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DefaultSiteId = table.Column<int>(type: "int", nullable: true),
                    WorkingTime = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsContracted = table.Column<byte>(type: "tinyint", nullable: true),
                    FamilyData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comment1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comment2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VacationDays = table.Column<int>(type: "int", nullable: true),
                    FullVacationDays = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.EmployeeId);
                    table.ForeignKey(
                        name: "FK_Employee_EmploymentStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "EmploymentStatus",
                        principalColumn: "StatusId");
                    table.ForeignKey(
                        name: "FK_Employee_JobTitle_JobTitleId",
                        column: x => x.JobTitleId,
                        principalTable: "JobTitle",
                        principalColumn: "JobTitleId");
                });

            migrationBuilder.CreateTable(
                name: "Partners",
                columns: table => new
                {
                    PartnerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PartnerCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlternatePhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IntTaxId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IndividualTaxId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Industry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressLine1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressLine2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastContacted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssignedTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillingContactName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillingEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentTerms = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreditLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PreferredCurrency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsTaxExempt = table.Column<bool>(type: "bit", nullable: true),
                    PartnerGroupId = table.Column<int>(type: "int", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true),
                    PartnerTypeId = table.Column<int>(type: "int", nullable: true),
                    GFOId = table.Column<int>(type: "int", nullable: true),
                    Comment1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comment2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyNameTrim = table.Column<string>(type: "nvarchar(max)", nullable: true, computedColumnSql: "CASE WHEN [CompanyName] IS NULL THEN NULL ELSE CONVERT([nvarchar](450), LEFT(LTRIM(RTRIM([CompanyName])), 450)) END PERSISTED"),
                    NameTrim = table.Column<string>(type: "nvarchar(max)", nullable: true, computedColumnSql: "CASE WHEN [Name] IS NULL THEN NULL ELSE CONVERT([nvarchar](450), LEFT(LTRIM(RTRIM([Name])), 450)) END PERSISTED"),
                    TaxIdTrim = table.Column<string>(type: "nvarchar(max)", nullable: true, computedColumnSql: "CASE WHEN [TaxId] IS NULL THEN NULL ELSE CONVERT([nvarchar](50), LEFT(LTRIM(RTRIM([TaxId])), 50)) END PERSISTED")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partners", x => x.PartnerId);
                    table.ForeignKey(
                        name: "FK_Partners_GFO_GFOId",
                        column: x => x.GFOId,
                        principalTable: "GFO",
                        principalColumn: "GFOId");
                    table.ForeignKey(
                        name: "FK_Partners_PartnerGroups_PartnerGroupId",
                        column: x => x.PartnerGroupId,
                        principalTable: "PartnerGroups",
                        principalColumn: "PartnerGroupId");
                    table.ForeignKey(
                        name: "FK_Partners_PartnerStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "PartnerStatuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Partners_PartnerTypes_PartnerTypeId",
                        column: x => x.PartnerTypeId,
                        principalTable: "PartnerTypes",
                        principalColumn: "PartnerTypeId");
                });

            migrationBuilder.CreateTable(
                name: "ProjectPM",
                columns: table => new
                {
                    ProjectPMId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Budget = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProjectStatusPMId = table.Column<int>(type: "int", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectPM", x => x.ProjectPMId);
                    table.ForeignKey(
                        name: "FK_ProjectPM_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjectPM_ProjectStatusPMs_ProjectStatusPMId",
                        column: x => x.ProjectStatusPMId,
                        principalTable: "ProjectStatusPMs",
                        principalColumn: "ProjectStatusPMId");
                });

            migrationBuilder.CreateTable(
                name: "PartnerDto",
                columns: table => new
                {
                    PartnerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AlternatePhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ShortName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PartnerCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OwnId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GFOId = table.Column<int>(type: "int", nullable: true),
                    GFOName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IndividualTaxId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IntTaxId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Industry = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AddressLine1 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AddressLine2 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true),
                    LastContacted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AssignedTo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BillingContactName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BillingEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PaymentTerms = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreditLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PreferredCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Comment1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comment2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsTaxExempt = table.Column<bool>(type: "bit", nullable: true),
                    PartnerGroupId = table.Column<int>(type: "int", nullable: true),
                    PartnerTypeId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerDto", x => x.PartnerId);
                    table.ForeignKey(
                        name: "FK_PartnerDto_StatusDto_StatusId",
                        column: x => x.StatusId,
                        principalTable: "StatusDto",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BusinessDocuments",
                columns: table => new
                {
                    BusinessDocumentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BusinessDocumentTypeId = table.Column<int>(type: "int", nullable: false),
                    BusinessDocumentStatusId = table.Column<int>(type: "int", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FulfillmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CurrencyId = table.Column<int>(type: "int", nullable: true),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    NetTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TaxTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GrossTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessDocuments", x => x.BusinessDocumentId);
                    table.ForeignKey(
                        name: "FK_BusinessDocuments_BusinessDocumentStatuses_BusinessDocumentStatusId",
                        column: x => x.BusinessDocumentStatusId,
                        principalTable: "BusinessDocumentStatuses",
                        principalColumn: "BusinessDocumentStatusId");
                    table.ForeignKey(
                        name: "FK_BusinessDocuments_BusinessDocumentTypes_BusinessDocumentTypeId",
                        column: x => x.BusinessDocumentTypeId,
                        principalTable: "BusinessDocumentTypes",
                        principalColumn: "BusinessDocumentTypeId");
                    table.ForeignKey(
                        name: "FK_BusinessDocuments_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId");
                });

            migrationBuilder.CreateTable(
                name: "ItemPrices",
                columns: table => new
                {
                    ItemPriceId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    PriceTypeId = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "date", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "date", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemPrices", x => x.ItemPriceId);
                    table.ForeignKey(
                        name: "FK_ItemPrices_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId");
                    table.ForeignKey(
                        name: "FK_ItemPrices_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId");
                    table.ForeignKey(
                        name: "FK_ItemPrices_PriceTypes_PriceTypeId",
                        column: x => x.PriceTypeId,
                        principalTable: "PriceTypes",
                        principalColumn: "PriceTypeId");
                });

            migrationBuilder.CreateTable(
                name: "EmployeeHistories",
                columns: table => new
                {
                    HistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    AppUserId = table.Column<int>(type: "int", nullable: true),
                    ChangeTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WhatModified = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AppUserId1 = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeHistories", x => x.HistoryId);
                    table.ForeignKey(
                        name: "FK_EmployeeHistories_AspNetUsers_AppUserId1",
                        column: x => x.AppUserId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EmployeeHistories_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                });

            migrationBuilder.CreateTable(
                name: "Salaries",
                columns: table => new
                {
                    SalaryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    SalaryAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salaries", x => x.SalaryId);
                    table.ForeignKey(
                        name: "FK_Salaries_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                });

            migrationBuilder.CreateTable(
                name: "VacationBalances",
                columns: table => new
                {
                    BalanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    AvailableDays = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacationBalances", x => x.BalanceId);
                    table.ForeignKey(
                        name: "FK_VacationBalances_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                });

            migrationBuilder.CreateTable(
                name: "Vacations",
                columns: table => new
                {
                    VacationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    AppUserId = table.Column<int>(type: "int", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DurationDays = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsApproved = table.Column<byte>(type: "tinyint", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AppUserId1 = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vacations", x => x.VacationId);
                    table.ForeignKey(
                        name: "FK_Vacations_AspNetUsers_AppUserId1",
                        column: x => x.AppUserId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Vacations_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    ContactId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comment2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true),
                    PartnerId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.ContactId);
                    table.ForeignKey(
                        name: "FK_Contacts_PartnerStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "PartnerStatuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Contacts_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "PartnerId");
                });

            migrationBuilder.CreateTable(
                name: "Leads",
                columns: table => new
                {
                    LeadId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastContactDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextFollowUpDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PartnerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leads", x => x.LeadId);
                    table.ForeignKey(
                        name: "FK_Leads_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "PartnerId");
                });

            migrationBuilder.CreateTable(
                name: "LeadSources",
                columns: table => new
                {
                    LeadSourceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeadSourceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PartnerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadSources", x => x.LeadSourceId);
                    table.ForeignKey(
                        name: "FK_LeadSources_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "PartnerId");
                });

            migrationBuilder.CreateTable(
                name: "Sites",
                columns: table => new
                {
                    SiteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddressLine1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressLine2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    ContactPerson1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactPerson2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactPerson3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MobilePhone1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MobilePhone2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MobilePhone3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    messagingApp1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    messagingApp2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    messagingApp3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    eMail1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    eMail2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comment1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comment2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PartnerId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: true),
                    SiteTypeId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sites", x => x.SiteId);
                    table.ForeignKey(
                        name: "FK_Sites_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Sites_AspNetUsers_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Sites_PartnerStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "PartnerStatuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Sites_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "PartnerId");
                    table.ForeignKey(
                        name: "FK_Sites_SiteTypes_SiteTypeId",
                        column: x => x.SiteTypeId,
                        principalTable: "SiteTypes",
                        principalColumn: "SiteTypeId");
                });

            migrationBuilder.CreateTable(
                name: "ContactDto",
                columns: table => new
                {
                    ContactId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comment2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true),
                    PartnerId = table.Column<int>(type: "int", nullable: true),
                    PartnerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PartnerDtoPartnerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactDto", x => x.ContactId);
                    table.ForeignKey(
                        name: "FK_ContactDto_PartnerDto_PartnerDtoPartnerId",
                        column: x => x.PartnerDtoPartnerId,
                        principalTable: "PartnerDto",
                        principalColumn: "PartnerId");
                    table.ForeignKey(
                        name: "FK_ContactDto_PartnerStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "PartnerStatuses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DocumentDto",
                columns: table => new
                {
                    DocumentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OriginalFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StoredFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileExtension = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    StorageProvider = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StorageKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocumentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocumentDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocumentTypeId = table.Column<int>(type: "int", nullable: true),
                    DocumentTypeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UploadedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SiteId = table.Column<int>(type: "int", nullable: true),
                    PartnerId = table.Column<int>(type: "int", nullable: true),
                    PartnerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DocumentStatusId = table.Column<int>(type: "int", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    HashAlgorithm = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VersionNumber = table.Column<int>(type: "int", nullable: true),
                    IsLatestVersion = table.Column<bool>(type: "bit", nullable: true),
                    ParentDocumentId = table.Column<int>(type: "int", nullable: true),
                    PartnerDtoPartnerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentDto", x => x.DocumentId);
                    table.ForeignKey(
                        name: "FK_DocumentDto_PartnerDto_PartnerDtoPartnerId",
                        column: x => x.PartnerDtoPartnerId,
                        principalTable: "PartnerDto",
                        principalColumn: "PartnerId");
                });

            migrationBuilder.CreateTable(
                name: "SiteDto",
                columns: table => new
                {
                    SiteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressLine1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressLine2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    Phone1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MobilePhone1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MobilePhone2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MobilePhone3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    messagingApp1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    messagingApp2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    messagingApp3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    eMail1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    eMail2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactPerson1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactPerson2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactPerson3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comment1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comment2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true),
                    PartnerId = table.Column<int>(type: "int", nullable: false),
                    SiteTypeId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PartnerDtoPartnerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteDto", x => x.SiteId);
                    table.ForeignKey(
                        name: "FK_SiteDto_PartnerDto_PartnerDtoPartnerId",
                        column: x => x.PartnerDtoPartnerId,
                        principalTable: "PartnerDto",
                        principalColumn: "PartnerId");
                });

            migrationBuilder.CreateTable(
                name: "BusinessDocumentLines",
                columns: table => new
                {
                    BusinessDocumentLineId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BusinessDocumentId = table.Column<int>(type: "int", nullable: false),
                    LineNo = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemCodeSnapshot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ItemNameSnapshot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UomSnapshot = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VatRateSnapshot = table.Column<decimal>(type: "decimal(9,4)", nullable: false),
                    UnitPriceSnapshot = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TaxCodeId = table.Column<int>(type: "int", nullable: true),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GrossAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    WarehouseId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessDocumentLines", x => x.BusinessDocumentLineId);
                    table.ForeignKey(
                        name: "FK_BusinessDocumentLines_BusinessDocuments_BusinessDocumentId",
                        column: x => x.BusinessDocumentId,
                        principalTable: "BusinessDocuments",
                        principalColumn: "BusinessDocumentId");
                    table.ForeignKey(
                        name: "FK_BusinessDocumentLines_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId");
                    table.ForeignKey(
                        name: "FK_BusinessDocumentLines_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId");
                });

            migrationBuilder.CreateTable(
                name: "BusinessDocumentRelations",
                columns: table => new
                {
                    BusinessDocumentRelationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    FromBusinessDocumentId = table.Column<int>(type: "int", nullable: false),
                    ToBusinessDocumentId = table.Column<int>(type: "int", nullable: false),
                    BusinessDocumentRelationTypeId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessDocumentRelations", x => x.BusinessDocumentRelationId);
                    table.ForeignKey(
                        name: "FK_BusinessDocumentRelations_BusinessDocumentRelationTypes_BusinessDocumentRelationTypeId",
                        column: x => x.BusinessDocumentRelationTypeId,
                        principalTable: "BusinessDocumentRelationTypes",
                        principalColumn: "BusinessDocumentRelationTypeId");
                    table.ForeignKey(
                        name: "FK_BusinessDocumentRelations_BusinessDocuments_FromBusinessDocumentId",
                        column: x => x.FromBusinessDocumentId,
                        principalTable: "BusinessDocuments",
                        principalColumn: "BusinessDocumentId");
                    table.ForeignKey(
                        name: "FK_BusinessDocumentRelations_BusinessDocuments_ToBusinessDocumentId",
                        column: x => x.ToBusinessDocumentId,
                        principalTable: "BusinessDocuments",
                        principalColumn: "BusinessDocumentId");
                    table.ForeignKey(
                        name: "FK_BusinessDocumentRelations_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId");
                });

            migrationBuilder.CreateTable(
                name: "BusinessDocumentStatusHistories",
                columns: table => new
                {
                    BusinessDocumentStatusHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BusinessDocumentId = table.Column<int>(type: "int", nullable: false),
                    OldBusinessDocumentStatusId = table.Column<int>(type: "int", nullable: false),
                    NewBusinessDocumentStatusId = table.Column<int>(type: "int", nullable: false),
                    ChangeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    BusinessDocumentId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessDocumentStatusHistories", x => x.BusinessDocumentStatusHistoryId);
                    table.ForeignKey(
                        name: "FK_BusinessDocumentStatusHistories_BusinessDocumentStatuses_NewBusinessDocumentStatusId",
                        column: x => x.NewBusinessDocumentStatusId,
                        principalTable: "BusinessDocumentStatuses",
                        principalColumn: "BusinessDocumentStatusId");
                    table.ForeignKey(
                        name: "FK_BusinessDocumentStatusHistories_BusinessDocumentStatuses_OldBusinessDocumentStatusId",
                        column: x => x.OldBusinessDocumentStatusId,
                        principalTable: "BusinessDocumentStatuses",
                        principalColumn: "BusinessDocumentStatusId");
                    table.ForeignKey(
                        name: "FK_BusinessDocumentStatusHistories_BusinessDocuments_BusinessDocumentId",
                        column: x => x.BusinessDocumentId,
                        principalTable: "BusinessDocuments",
                        principalColumn: "BusinessDocumentId");
                    table.ForeignKey(
                        name: "FK_BusinessDocumentStatusHistories_BusinessDocuments_BusinessDocumentId1",
                        column: x => x.BusinessDocumentId1,
                        principalTable: "BusinessDocuments",
                        principalColumn: "BusinessDocumentId");
                    table.ForeignKey(
                        name: "FK_BusinessDocumentStatusHistories_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId");
                });

            migrationBuilder.CreateTable(
                name: "LeadHistories",
                columns: table => new
                {
                    LeadHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeadId = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PartnerId = table.Column<int>(type: "int", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangeType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadHistories", x => x.LeadHistoryId);
                    table.ForeignKey(
                        name: "FK_LeadHistories_Leads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Leads",
                        principalColumn: "LeadId");
                });

            migrationBuilder.CreateTable(
                name: "BusinessDocumentParties",
                columns: table => new
                {
                    BusinessDocumentPartyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BusinessDocumentId = table.Column<int>(type: "int", nullable: false),
                    BusinessDocumentPartyRoleId = table.Column<int>(type: "int", nullable: false),
                    PartnerId = table.Column<int>(type: "int", nullable: true),
                    SiteId = table.Column<int>(type: "int", nullable: true),
                    ContactId = table.Column<int>(type: "int", nullable: true),
                    DisplayNameSnapshot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AddressSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxNumberSnapshot = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessDocumentParties", x => x.BusinessDocumentPartyId);
                    table.ForeignKey(
                        name: "FK_BusinessDocumentParties_BusinessDocumentPartyRoles_BusinessDocumentPartyRoleId",
                        column: x => x.BusinessDocumentPartyRoleId,
                        principalTable: "BusinessDocumentPartyRoles",
                        principalColumn: "BusinessDocumentPartyRoleId");
                    table.ForeignKey(
                        name: "FK_BusinessDocumentParties_BusinessDocuments_BusinessDocumentId",
                        column: x => x.BusinessDocumentId,
                        principalTable: "BusinessDocuments",
                        principalColumn: "BusinessDocumentId");
                    table.ForeignKey(
                        name: "FK_BusinessDocumentParties_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "ContactId");
                    table.ForeignKey(
                        name: "FK_BusinessDocumentParties_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "PartnerId");
                    table.ForeignKey(
                        name: "FK_BusinessDocumentParties_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "SiteId");
                    table.ForeignKey(
                        name: "FK_BusinessDocumentParties_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId");
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    DocumentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OriginalFileName = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    StoredFileName = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    FileExtension = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    StorageProvider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StorageKey = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    DocumentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocumentDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocumentTypeId = table.Column<int>(type: "int", nullable: true),
                    SiteId = table.Column<int>(type: "int", nullable: true),
                    PartnerId = table.Column<int>(type: "int", nullable: true),
                    ContactId = table.Column<int>(type: "int", nullable: true),
                    employee_id = table.Column<int>(type: "int", nullable: true),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UploadedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: true),
                    DocumentStatusId = table.Column<int>(type: "int", nullable: true),
                    HashAlgorithm = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FileHash = table.Column<string>(type: "char(64)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    VersionNumber = table.Column<int>(type: "int", nullable: true),
                    IsLatestVersion = table.Column<bool>(type: "bit", nullable: true),
                    ParentDocumentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.DocumentId);
                    table.ForeignKey(
                        name: "FK_Documents_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "ContactId");
                    table.ForeignKey(
                        name: "FK_Documents_DocumentStatuses_DocumentStatusId",
                        column: x => x.DocumentStatusId,
                        principalTable: "DocumentStatuses",
                        principalColumn: "DocumentStatusId");
                    table.ForeignKey(
                        name: "FK_Documents_DocumentTypes_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentTypes",
                        principalColumn: "DocumentTypeId");
                    table.ForeignKey(
                        name: "FK_Documents_Documents_ParentDocumentId",
                        column: x => x.ParentDocumentId,
                        principalTable: "Documents",
                        principalColumn: "DocumentId");
                    table.ForeignKey(
                        name: "FK_Documents_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "PartnerId");
                    table.ForeignKey(
                        name: "FK_Documents_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "SiteId");
                });

            migrationBuilder.CreateTable(
                name: "Quotes",
                columns: table => new
                {
                    QuoteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuoteNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    QuoteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SalesPerson = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ValidityDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    QuoteDiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalItemDiscounts = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DetailedDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PartnerId = table.Column<int>(type: "int", nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SiteId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotes", x => x.QuoteId);
                    table.ForeignKey(
                        name: "FK_Quotes_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId");
                    table.ForeignKey(
                        name: "FK_Quotes_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "PartnerId");
                    table.ForeignKey(
                        name: "FK_Quotes_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "SiteId");
                });

            migrationBuilder.CreateTable(
                name: "Resource",
                columns: table => new
                {
                    ResourceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ResourceTypeId = table.Column<int>(type: "int", nullable: true),
                    ResourceStatusId = table.Column<int>(type: "int", nullable: true),
                    Serial = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NextService = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateOfPurchase = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WarrantyPeriod = table.Column<int>(type: "int", nullable: true),
                    WarrantyExpireDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ServiceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WhoBuyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    WhoLastServicedId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PartnerId = table.Column<int>(type: "int", nullable: true),
                    SiteId = table.Column<int>(type: "int", nullable: true),
                    ContactId = table.Column<int>(type: "int", nullable: true),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreatedAt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comment1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comment2 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resource", x => x.ResourceId);
                    table.ForeignKey(
                        name: "FK_Resource_AspNetUsers_WhoBuyId",
                        column: x => x.WhoBuyId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Resource_AspNetUsers_WhoLastServicedId",
                        column: x => x.WhoLastServicedId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Resource_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "ContactId");
                    table.ForeignKey(
                        name: "FK_Resource_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_Resource_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "PartnerId");
                    table.ForeignKey(
                        name: "FK_Resource_ResourceStatus_ResourceStatusId",
                        column: x => x.ResourceStatusId,
                        principalTable: "ResourceStatus",
                        principalColumn: "ResourceStatusId");
                    table.ForeignKey(
                        name: "FK_Resource_ResourceType_ResourceTypeId",
                        column: x => x.ResourceTypeId,
                        principalTable: "ResourceType",
                        principalColumn: "ResourceTypeId");
                    table.ForeignKey(
                        name: "FK_Resource_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "SiteId");
                });

            migrationBuilder.CreateTable(
                name: "Shifts",
                columns: table => new
                {
                    ShiftId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartnerId = table.Column<int>(type: "int", nullable: true),
                    SiteId = table.Column<int>(type: "int", nullable: true),
                    ContactId = table.Column<int>(type: "int", nullable: true),
                    ShiftDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    ShiftType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shifts", x => x.ShiftId);
                    table.ForeignKey(
                        name: "FK_Shifts_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "ContactId");
                    table.ForeignKey(
                        name: "FK_Shifts_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "PartnerId");
                    table.ForeignKey(
                        name: "FK_Shifts_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "SiteId");
                });

            migrationBuilder.CreateTable(
                name: "DocumentLinkDto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    ModuleId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecordId = table.Column<int>(type: "int", nullable: false),
                    DocumentDtoDocumentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentLinkDto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentLinkDto_DocumentDto_DocumentDtoDocumentId",
                        column: x => x.DocumentDtoDocumentId,
                        principalTable: "DocumentDto",
                        principalColumn: "DocumentId");
                });

            migrationBuilder.CreateTable(
                name: "DocumentStatusHistoryDto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    OldStatus = table.Column<int>(type: "int", nullable: false),
                    NewStatus = table.Column<int>(type: "int", nullable: false),
                    ChangeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocumentDtoDocumentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentStatusHistoryDto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentStatusHistoryDto_DocumentDto_DocumentDtoDocumentId",
                        column: x => x.DocumentDtoDocumentId,
                        principalTable: "DocumentDto",
                        principalColumn: "DocumentId");
                });

            migrationBuilder.CreateTable(
                name: "MetadataEntry",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocumentDtoDocumentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetadataEntry", x => x.Key);
                    table.ForeignKey(
                        name: "FK_MetadataEntry_DocumentDto_DocumentDtoDocumentId",
                        column: x => x.DocumentDtoDocumentId,
                        principalTable: "DocumentDto",
                        principalColumn: "DocumentId");
                });

            migrationBuilder.CreateTable(
                name: "BusinessDocumentAttachments",
                columns: table => new
                {
                    BusinessDocumentAttachmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BusinessDocumentId = table.Column<int>(type: "int", nullable: false),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    AttachmentCategoryId = table.Column<int>(type: "int", nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessDocumentAttachments", x => x.BusinessDocumentAttachmentId);
                    table.ForeignKey(
                        name: "FK_BusinessDocumentAttachments_AttachmentCategories_AttachmentCategoryId",
                        column: x => x.AttachmentCategoryId,
                        principalTable: "AttachmentCategories",
                        principalColumn: "AttachmentCategoryId");
                    table.ForeignKey(
                        name: "FK_BusinessDocumentAttachments_BusinessDocuments_BusinessDocumentId",
                        column: x => x.BusinessDocumentId,
                        principalTable: "BusinessDocuments",
                        principalColumn: "BusinessDocumentId");
                    table.ForeignKey(
                        name: "FK_BusinessDocumentAttachments_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "DocumentId");
                    table.ForeignKey(
                        name: "FK_BusinessDocumentAttachments_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId");
                });

            migrationBuilder.CreateTable(
                name: "DocumentLinks",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    ModuleID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecordID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentLinks", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DocumentLinks_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "DocumentId");
                });

            migrationBuilder.CreateTable(
                name: "DocumentMetadata",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentMetadata", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DocumentMetadata_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "DocumentId");
                });

            migrationBuilder.CreateTable(
                name: "DocumentStatusHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    OldStatus = table.Column<int>(type: "int", nullable: false),
                    NewStatus = table.Column<int>(type: "int", nullable: false),
                    ChangeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentStatusHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentStatusHistory_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "DocumentId");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OrderDate = table.Column<DateTime>(type: "date", nullable: true),
                    Deadline = table.Column<DateTime>(type: "date", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SalesPerson = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "date", nullable: true),
                    PlannedDelivery = table.Column<DateTime>(type: "datetime", nullable: true),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DetailedDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, defaultValue: "System"),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, defaultValue: "System"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Pending"),
                    PartnerId = table.Column<int>(type: "int", nullable: false),
                    SiteId = table.Column<int>(type: "int", nullable: true),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    ShippingMethodId = table.Column<int>(type: "int", nullable: true),
                    PaymentTermId = table.Column<int>(type: "int", nullable: true),
                    ContactId = table.Column<int>(type: "int", nullable: true),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    QuoteId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    OrderStatusTypes = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "ContactId");
                    table.ForeignKey(
                        name: "FK_Orders_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId");
                    table.ForeignKey(
                        name: "FK_Orders_OrderShippingMethods_ShippingMethodId",
                        column: x => x.ShippingMethodId,
                        principalTable: "OrderShippingMethods",
                        principalColumn: "ShippingMethodId");
                    table.ForeignKey(
                        name: "FK_Orders_OrderStatusTypes_OrderStatusTypes",
                        column: x => x.OrderStatusTypes,
                        principalTable: "OrderStatusTypes",
                        principalColumn: "OrderStatusId");
                    table.ForeignKey(
                        name: "FK_Orders_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "PartnerId");
                    table.ForeignKey(
                        name: "FK_Orders_PaymentTerms_PaymentTermId",
                        column: x => x.PaymentTermId,
                        principalTable: "PaymentTerms",
                        principalColumn: "PaymentTermId");
                    table.ForeignKey(
                        name: "FK_Orders_Quotes_QuoteId",
                        column: x => x.QuoteId,
                        principalTable: "Quotes",
                        principalColumn: "QuoteId");
                    table.ForeignKey(
                        name: "FK_Orders_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "SiteId");
                });

            migrationBuilder.CreateTable(
                name: "QuoteHistories",
                columns: table => new
                {
                    QuoteHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuoteId = table.Column<int>(type: "int", nullable: false),
                    QuoteItemId = table.Column<int>(type: "int", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuoteHistories", x => x.QuoteHistoryId);
                    table.ForeignKey(
                        name: "FK_QuoteHistories_Quotes_QuoteId",
                        column: x => x.QuoteId,
                        principalTable: "Quotes",
                        principalColumn: "QuoteId");
                });

            migrationBuilder.CreateTable(
                name: "ResourceHistory",
                columns: table => new
                {
                    ResourceHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResourceId = table.Column<int>(type: "int", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "GETDATE()"),
                    ChangeDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ServicePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceHistory", x => x.ResourceHistoryId);
                    table.ForeignKey(
                        name: "FK_ResourceHistory_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ResourceHistory_Resource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resource",
                        principalColumn: "ResourceId");
                });

            migrationBuilder.CreateTable(
                name: "EmployeeShifts",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    ShiftId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeShifts", x => new { x.EmployeeId, x.ShiftId });
                    table.ForeignKey(
                        name: "FK_EmployeeShifts_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_EmployeeShifts_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "ShiftId");
                });

            migrationBuilder.CreateTable(
                name: "PartnerShifts",
                columns: table => new
                {
                    PartnerId = table.Column<int>(type: "int", nullable: false),
                    ShiftId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerShifts", x => new { x.PartnerId, x.ShiftId });
                    table.ForeignKey(
                        name: "FK_PartnerShifts_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "PartnerId");
                    table.ForeignKey(
                        name: "FK_PartnerShifts_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "ShiftId");
                });

            migrationBuilder.CreateTable(
                name: "SiteShifts",
                columns: table => new
                {
                    SiteId = table.Column<int>(type: "int", nullable: false),
                    ShiftId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteShifts", x => new { x.SiteId, x.ShiftId });
                    table.ForeignKey(
                        name: "FK_SiteShifts_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "ShiftId");
                    table.ForeignKey(
                        name: "FK_SiteShifts_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "SiteId");
                });

            migrationBuilder.CreateTable(
                name: "CustomerCommunications",
                columns: table => new
                {
                    CustomerCommunicationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommunicationTypeId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AgentId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    AttachmentPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Metadata = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ContactId = table.Column<int>(type: "int", nullable: true),
                    PartnerId = table.Column<int>(type: "int", nullable: true),
                    LeadId = table.Column<int>(type: "int", nullable: true),
                    QuoteId = table.Column<int>(type: "int", nullable: true),
                    OrderId = table.Column<int>(type: "int", nullable: true),
                    SiteId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerCommunications", x => x.CustomerCommunicationId);
                    table.ForeignKey(
                        name: "FK_CustomerCommunications_AspNetUsers_AgentId",
                        column: x => x.AgentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomerCommunications_CommunicationStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "CommunicationStatuses",
                        principalColumn: "StatusId");
                    table.ForeignKey(
                        name: "FK_CustomerCommunications_CommunicationTypes_CommunicationTypeId",
                        column: x => x.CommunicationTypeId,
                        principalTable: "CommunicationTypes",
                        principalColumn: "CommunicationTypeId");
                    table.ForeignKey(
                        name: "FK_CustomerCommunications_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "ContactId");
                    table.ForeignKey(
                        name: "FK_CustomerCommunications_Leads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Leads",
                        principalColumn: "LeadId");
                    table.ForeignKey(
                        name: "FK_CustomerCommunications_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId");
                    table.ForeignKey(
                        name: "FK_CustomerCommunications_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "PartnerId");
                    table.ForeignKey(
                        name: "FK_CustomerCommunications_Quotes_QuoteId",
                        column: x => x.QuoteId,
                        principalTable: "Quotes",
                        principalColumn: "QuoteId");
                    table.ForeignKey(
                        name: "FK_CustomerCommunications_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "SiteId");
                });

            migrationBuilder.CreateTable(
                name: "CommunicationPosts",
                columns: table => new
                {
                    CommunicationPostId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerCommunicationId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunicationPosts", x => x.CommunicationPostId);
                    table.ForeignKey(
                        name: "FK_CommunicationPosts_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunicationPosts_CustomerCommunications_CustomerCommunicationId",
                        column: x => x.CustomerCommunicationId,
                        principalTable: "CustomerCommunications",
                        principalColumn: "CustomerCommunicationId");
                });

            migrationBuilder.CreateTable(
                name: "CommunicationResponsibles",
                columns: table => new
                {
                    CommunicationResponsibleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerCommunicationId = table.Column<int>(type: "int", nullable: false),
                    ResponsibleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AssignedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunicationResponsibles", x => x.CommunicationResponsibleId);
                    table.ForeignKey(
                        name: "FK_CommunicationResponsibles_AspNetUsers_AssignedById",
                        column: x => x.AssignedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunicationResponsibles_AspNetUsers_ResponsibleId",
                        column: x => x.ResponsibleId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunicationResponsibles_CustomerCommunications_CustomerCommunicationId",
                        column: x => x.CustomerCommunicationId,
                        principalTable: "CustomerCommunications",
                        principalColumn: "CustomerCommunicationId");
                });

            migrationBuilder.CreateTable(
                name: "TaskPM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    TaskTypePMId = table.Column<int>(type: "int", nullable: true),
                    TaskStatusPMId = table.Column<int>(type: "int", nullable: true),
                    TaskPriorityPMId = table.Column<int>(type: "int", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstimatedHours = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ActualHours = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CommunicationDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaskPMcomMethodID = table.Column<short>(type: "smallint", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AssignedTo = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PartnerId = table.Column<int>(type: "int", nullable: true),
                    SiteId = table.Column<int>(type: "int", nullable: true),
                    ContactId = table.Column<int>(type: "int", nullable: true),
                    QuoteId = table.Column<int>(type: "int", nullable: true),
                    CommunicationTypeId = table.Column<int>(type: "int", nullable: true),
                    OrderId = table.Column<int>(type: "int", nullable: true),
                    CustomerCommunicationId = table.Column<int>(type: "int", nullable: true),
                    RelatedPartnerId = table.Column<int>(type: "int", nullable: true),
                    ProjectPMId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskPM", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskPM_AspNetUsers_AssignedTo",
                        column: x => x.AssignedTo,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskPM_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskPM_CommunicationTypes_CommunicationTypeId",
                        column: x => x.CommunicationTypeId,
                        principalTable: "CommunicationTypes",
                        principalColumn: "CommunicationTypeId");
                    table.ForeignKey(
                        name: "FK_TaskPM_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "ContactId");
                    table.ForeignKey(
                        name: "FK_TaskPM_CustomerCommunications_CustomerCommunicationId",
                        column: x => x.CustomerCommunicationId,
                        principalTable: "CustomerCommunications",
                        principalColumn: "CustomerCommunicationId");
                    table.ForeignKey(
                        name: "FK_TaskPM_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId");
                    table.ForeignKey(
                        name: "FK_TaskPM_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "PartnerId");
                    table.ForeignKey(
                        name: "FK_TaskPM_Partners_RelatedPartnerId",
                        column: x => x.RelatedPartnerId,
                        principalTable: "Partners",
                        principalColumn: "PartnerId");
                    table.ForeignKey(
                        name: "FK_TaskPM_ProjectPM_ProjectPMId",
                        column: x => x.ProjectPMId,
                        principalTable: "ProjectPM",
                        principalColumn: "ProjectPMId");
                    table.ForeignKey(
                        name: "FK_TaskPM_Quotes_QuoteId",
                        column: x => x.QuoteId,
                        principalTable: "Quotes",
                        principalColumn: "QuoteId");
                    table.ForeignKey(
                        name: "FK_TaskPM_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "SiteId");
                    table.ForeignKey(
                        name: "FK_TaskPM_TaskPMcomMethod_TaskPMcomMethodID",
                        column: x => x.TaskPMcomMethodID,
                        principalTable: "TaskPMcomMethod",
                        principalColumn: "TaskPMcomMethodID");
                    table.ForeignKey(
                        name: "FK_TaskPM_TaskPriorityPM_TaskPriorityPMId",
                        column: x => x.TaskPriorityPMId,
                        principalTable: "TaskPriorityPM",
                        principalColumn: "TaskPriorityPMId");
                    table.ForeignKey(
                        name: "FK_TaskPM_TaskStatusPM_TaskStatusPMId",
                        column: x => x.TaskStatusPMId,
                        principalTable: "TaskStatusPM",
                        principalColumn: "TaskStatusPMId");
                    table.ForeignKey(
                        name: "FK_TaskPM_TaskTypePM_TaskTypePMId",
                        column: x => x.TaskTypePMId,
                        principalTable: "TaskTypePM",
                        principalColumn: "TaskTypePMId");
                });

            migrationBuilder.CreateTable(
                name: "TaskAttachmentsPMs",
                columns: table => new
                {
                    TaskAttachmentPMId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskPMId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileSize = table.Column<int>(type: "int", nullable: true),
                    UploadedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskAttachmentsPMs", x => x.TaskAttachmentPMId);
                    table.ForeignKey(
                        name: "FK_TaskAttachmentsPMs_AspNetUsers_UploadedById",
                        column: x => x.UploadedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskAttachmentsPMs_TaskPM_TaskPMId",
                        column: x => x.TaskPMId,
                        principalTable: "TaskPM",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaskCommentsPMs",
                columns: table => new
                {
                    TaskCommentPMId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskPMId = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskCommentsPMs", x => x.TaskCommentPMId);
                    table.ForeignKey(
                        name: "FK_TaskCommentsPMs_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskCommentsPMs_TaskPM_TaskPMId",
                        column: x => x.TaskPMId,
                        principalTable: "TaskPM",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaskDocumentLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    LinkedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LinkedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskDocumentLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskDocumentLinks_AspNetUsers_LinkedById",
                        column: x => x.LinkedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskDocumentLinks_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "DocumentId");
                    table.ForeignKey(
                        name: "FK_TaskDocumentLinks_TaskPM_TaskId",
                        column: x => x.TaskId,
                        principalTable: "TaskPM",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaskEmployeeAssignment",
                columns: table => new
                {
                    TaskEmployeeAssignmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskPMId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskEmployeeAssignment", x => x.TaskEmployeeAssignmentId);
                    table.ForeignKey(
                        name: "FK_TaskEmployeeAssignment_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_TaskEmployeeAssignment_TaskPM_TaskPMId",
                        column: x => x.TaskPMId,
                        principalTable: "TaskPM",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaskHistory",
                columns: table => new
                {
                    TaskHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskPMId = table.Column<int>(type: "int", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangeDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskHistory", x => x.TaskHistoryId);
                    table.ForeignKey(
                        name: "FK_TaskHistory_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskHistory_TaskPM_TaskPMId",
                        column: x => x.TaskPMId,
                        principalTable: "TaskPM",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaskResourceAssignment",
                columns: table => new
                {
                    TaskResourceAssignmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskPMId = table.Column<int>(type: "int", nullable: false),
                    ResourceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskResourceAssignment", x => x.TaskResourceAssignmentId);
                    table.ForeignKey(
                        name: "FK_TaskResourceAssignment_Resource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resource",
                        principalColumn: "ResourceId");
                    table.ForeignKey(
                        name: "FK_TaskResourceAssignment_TaskPM_TaskPMId",
                        column: x => x.TaskPMId,
                        principalTable: "TaskPM",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "PartnerGroups",
                columns: new[] { "PartnerGroupId", "DiscountPercentage", "PartnerGroupName" },
                values: new object[] { 1, 5.00m, "VIP Customers" });

            migrationBuilder.InsertData(
                table: "VatTypes",
                columns: new[] { "VatTypeId", "Rate", "TypeName" },
                values: new object[] { 1, 27.00m, "27%" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ChangedAt",
                table: "AuditLogs",
                column: "ChangedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ChangedById",
                table: "AuditLogs",
                column: "ChangedById");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityType",
                table: "AuditLogs",
                column: "EntityType");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessDocumentAttachments_AttachmentCategoryId",
                table: "BusinessDocumentAttachments",
                column: "AttachmentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessDocumentAttachments_BusinessDocumentId",
                table: "BusinessDocumentAttachments",
                column: "BusinessDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessDocumentAttachments_DocumentId",
                table: "BusinessDocumentAttachments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessDocumentAttachments_TenantId",
                table: "BusinessDocumentAttachments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessDocumentLines_BusinessDocumentId",
                table: "BusinessDocumentLines",
                column: "BusinessDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessDocumentLines_ItemId",
                table: "BusinessDocumentLines",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessDocumentLines_TenantId",
                table: "BusinessDocumentLines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessDocumentParties_BusinessDocumentId",
                table: "BusinessDocumentParties",
                column: "BusinessDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessDocumentParties_BusinessDocumentPartyRoleId",
                table: "BusinessDocumentParties",
                column: "BusinessDocumentPartyRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessDocumentParties_ContactId",
                table: "BusinessDocumentParties",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessDocumentParties_PartnerId",
                table: "BusinessDocumentParties",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessDocumentParties_SiteId",
                table: "BusinessDocumentParties",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessDocumentParties_TenantId",
                table: "BusinessDocumentParties",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessDocumentRelations_BusinessDocumentRelationTypeId",
                table: "BusinessDocumentRelations",
                column: "BusinessDocumentRelationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessDocumentRelations_FromBusinessDocumentId",
                table: "BusinessDocumentRelations",
                column: "FromBusinessDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessDocumentRelations_TenantId_FromBusinessDocumentId_ToBusinessDocumentId_BusinessDocumentRelationTypeId",
                table: "BusinessDocumentRelations",
                columns: new[] { "TenantId", "FromBusinessDocumentId", "ToBusinessDocumentId", "BusinessDocumentRelationTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessDocumentRelations_ToBusinessDocumentId",
                table: "BusinessDocumentRelations",
                column: "ToBusinessDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessDocuments_BusinessDocumentStatusId",
                table: "BusinessDocuments",
                column: "BusinessDocumentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessDocuments_BusinessDocumentTypeId",
                table: "BusinessDocuments",
                column: "BusinessDocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessDocuments_TenantId",
                table: "BusinessDocuments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessDocumentStatusHistories_BusinessDocumentId",
                table: "BusinessDocumentStatusHistories",
                column: "BusinessDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessDocumentStatusHistories_BusinessDocumentId1",
                table: "BusinessDocumentStatusHistories",
                column: "BusinessDocumentId1");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessDocumentStatusHistories_NewBusinessDocumentStatusId",
                table: "BusinessDocumentStatusHistories",
                column: "NewBusinessDocumentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessDocumentStatusHistories_OldBusinessDocumentStatusId",
                table: "BusinessDocumentStatusHistories",
                column: "OldBusinessDocumentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessDocumentStatusHistories_TenantId",
                table: "BusinessDocumentStatusHistories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunicationPosts_CreatedById",
                table: "CommunicationPosts",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CommunicationPosts_CustomerCommunicationId",
                table: "CommunicationPosts",
                column: "CustomerCommunicationId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunicationResponsibles_AssignedById",
                table: "CommunicationResponsibles",
                column: "AssignedById");

            migrationBuilder.CreateIndex(
                name: "IX_CommunicationResponsibles_CustomerCommunicationId",
                table: "CommunicationResponsibles",
                column: "CustomerCommunicationId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunicationResponsibles_ResponsibleId",
                table: "CommunicationResponsibles",
                column: "ResponsibleId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactDto_PartnerDtoPartnerId",
                table: "ContactDto",
                column: "PartnerDtoPartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactDto_StatusId",
                table: "ContactDto",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_PartnerId",
                table: "Contacts",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_StatusId",
                table: "Contacts",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_CreatedBy",
                table: "Currencies",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_LastModifiedBy",
                table: "Currencies",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCommunications_AgentId",
                table: "CustomerCommunications",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCommunications_CommunicationTypeId",
                table: "CustomerCommunications",
                column: "CommunicationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCommunications_ContactId",
                table: "CustomerCommunications",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCommunications_LeadId",
                table: "CustomerCommunications",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCommunications_OrderId",
                table: "CustomerCommunications",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCommunications_PartnerId",
                table: "CustomerCommunications",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCommunications_QuoteId",
                table: "CustomerCommunications",
                column: "QuoteId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCommunications_SiteId",
                table: "CustomerCommunications",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCommunications_StatusId",
                table: "CustomerCommunications",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDto_PartnerDtoPartnerId",
                table: "DocumentDto",
                column: "PartnerDtoPartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentLinkDto_DocumentDtoDocumentId",
                table: "DocumentLinkDto",
                column: "DocumentDtoDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentLinks_DocumentId",
                table: "DocumentLinks",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentMetadata_DocumentId",
                table: "DocumentMetadata",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ContactId",
                table: "Documents",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_DocumentStatusId",
                table: "Documents",
                column: "DocumentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_DocumentTypeId",
                table: "Documents",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ParentDocumentId",
                table: "Documents",
                column: "ParentDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_PartnerId",
                table: "Documents",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_SiteId",
                table: "Documents",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentStatusHistory_DocumentId",
                table: "DocumentStatusHistory",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentStatusHistoryDto_DocumentDtoDocumentId",
                table: "DocumentStatusHistoryDto",
                column: "DocumentDtoDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_JobTitleId",
                table: "Employee",
                column: "JobTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_StatusId",
                table: "Employee",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeHistories_AppUserId1",
                table: "EmployeeHistories",
                column: "AppUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeHistories_EmployeeId",
                table: "EmployeeHistories",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeShifts_ShiftId",
                table: "EmployeeShifts",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemPrices_CurrencyId",
                table: "ItemPrices",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemPrices_ItemId",
                table: "ItemPrices",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemPrices_PriceTypeId",
                table: "ItemPrices",
                column: "PriceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemPrices_TenantId_ItemId",
                table: "ItemPrices",
                columns: new[] { "TenantId", "ItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_ItemPrices_TenantId_ItemId_CurrencyId_PriceTypeId",
                table: "ItemPrices",
                columns: new[] { "TenantId", "ItemId", "CurrencyId", "PriceTypeId" },
                unique: true,
                filter: "[IsActive] = 1 AND [ValidTo] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ItemPrices_TenantId_ItemId_PriceTypeId_CurrencyId_IsActive_ValidFrom_ValidTo",
                table: "ItemPrices",
                columns: new[] { "TenantId", "ItemId", "PriceTypeId", "CurrencyId", "IsActive", "ValidFrom", "ValidTo" });

            migrationBuilder.CreateIndex(
                name: "IX_ItemPrices_TenantId_PriceTypeId",
                table: "ItemPrices",
                columns: new[] { "TenantId", "PriceTypeId" });

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemTypeId",
                table: "Items",
                column: "ItemTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadHistories_LeadId",
                table: "LeadHistories",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_PartnerId",
                table: "Leads",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadSources_PartnerId",
                table: "LeadSources",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_MetadataEntry_DocumentDtoDocumentId",
                table: "MetadataEntry",
                column: "DocumentDtoDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ContactId",
                table: "Orders",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CurrencyId",
                table: "Orders",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderNumber",
                table: "Orders",
                column: "OrderNumber",
                unique: true,
                filter: "[OrderNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderStatusTypes",
                table: "Orders",
                column: "OrderStatusTypes");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PartnerId",
                table: "Orders",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PaymentTermId",
                table: "Orders",
                column: "PaymentTermId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_QuoteId",
                table: "Orders",
                column: "QuoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShippingMethodId",
                table: "Orders",
                column: "ShippingMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_SiteId",
                table: "Orders",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderShippingMethods_MethodName",
                table: "OrderShippingMethods",
                column: "MethodName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PartnerDto_StatusId",
                table: "PartnerDto",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Partners_GFOId",
                table: "Partners",
                column: "GFOId");

            migrationBuilder.CreateIndex(
                name: "IX_Partners_PartnerGroupId",
                table: "Partners",
                column: "PartnerGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Partners_PartnerTypeId",
                table: "Partners",
                column: "PartnerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Partners_StatusId",
                table: "Partners",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerShifts_ShiftId",
                table: "PartnerShifts",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTerms_TermName",
                table: "PaymentTerms",
                column: "TermName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PriceTypes_Code",
                table: "PriceTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPM_CreatedById",
                table: "ProjectPM",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPM_ProjectStatusPMId",
                table: "ProjectPM",
                column: "ProjectStatusPMId");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteHistories_QuoteId",
                table: "QuoteHistories",
                column: "QuoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_CurrencyId",
                table: "Quotes",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_PartnerId",
                table: "Quotes",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_QuoteNumber",
                table: "Quotes",
                column: "QuoteNumber",
                unique: true,
                filter: "[QuoteNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_SiteId",
                table: "Quotes",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_ContactId",
                table: "Resource",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_EmployeeId",
                table: "Resource",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_PartnerId",
                table: "Resource",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_ResourceStatusId",
                table: "Resource",
                column: "ResourceStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_ResourceTypeId",
                table: "Resource",
                column: "ResourceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_SiteId",
                table: "Resource",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_WhoBuyId",
                table: "Resource",
                column: "WhoBuyId");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_WhoLastServicedId",
                table: "Resource",
                column: "WhoLastServicedId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceHistory_ModifiedById",
                table: "ResourceHistory",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceHistory_ResourceId",
                table: "ResourceHistory",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Salaries_EmployeeId",
                table: "Salaries",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_ContactId",
                table: "Shifts",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_PartnerId",
                table: "Shifts",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_SiteId",
                table: "Shifts",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_SiteDto_PartnerDtoPartnerId",
                table: "SiteDto",
                column: "PartnerDtoPartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_CreatedById",
                table: "Sites",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_LastModifiedById",
                table: "Sites",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_PartnerId",
                table: "Sites",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_SiteTypeId",
                table: "Sites",
                column: "SiteTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_StatusId",
                table: "Sites",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_SiteShifts_ShiftId",
                table: "SiteShifts",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAttachmentsPMs_TaskPMId",
                table: "TaskAttachmentsPMs",
                column: "TaskPMId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAttachmentsPMs_UploadedById",
                table: "TaskAttachmentsPMs",
                column: "UploadedById");

            migrationBuilder.CreateIndex(
                name: "IX_TaskCommentsPMs_CreatedById",
                table: "TaskCommentsPMs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TaskCommentsPMs_TaskPMId",
                table: "TaskCommentsPMs",
                column: "TaskPMId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskDocumentLinks_DocumentId",
                table: "TaskDocumentLinks",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskDocumentLinks_LinkedById",
                table: "TaskDocumentLinks",
                column: "LinkedById");

            migrationBuilder.CreateIndex(
                name: "IX_TaskDocumentLinks_TaskId",
                table: "TaskDocumentLinks",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskEmployeeAssignment_EmployeeId",
                table: "TaskEmployeeAssignment",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskEmployeeAssignment_TaskPMId",
                table: "TaskEmployeeAssignment",
                column: "TaskPMId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskHistory_ModifiedById",
                table: "TaskHistory",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_TaskHistory_TaskPMId",
                table: "TaskHistory",
                column: "TaskPMId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskPM_AssignedTo",
                table: "TaskPM",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_TaskPM_CommunicationTypeId",
                table: "TaskPM",
                column: "CommunicationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskPM_ContactId",
                table: "TaskPM",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskPM_CreatedBy",
                table: "TaskPM",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TaskPM_CustomerCommunicationId",
                table: "TaskPM",
                column: "CustomerCommunicationId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskPM_OrderId",
                table: "TaskPM",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskPM_PartnerId",
                table: "TaskPM",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskPM_ProjectPMId",
                table: "TaskPM",
                column: "ProjectPMId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskPM_QuoteId",
                table: "TaskPM",
                column: "QuoteId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskPM_RelatedPartnerId",
                table: "TaskPM",
                column: "RelatedPartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskPM_SiteId",
                table: "TaskPM",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskPM_TaskPMcomMethodID",
                table: "TaskPM",
                column: "TaskPMcomMethodID");

            migrationBuilder.CreateIndex(
                name: "IX_TaskPM_TaskPriorityPMId",
                table: "TaskPM",
                column: "TaskPriorityPMId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskPM_TaskStatusPMId",
                table: "TaskPM",
                column: "TaskStatusPMId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskPM_TaskTypePMId",
                table: "TaskPM",
                column: "TaskTypePMId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskResourceAssignment_ResourceId",
                table: "TaskResourceAssignment",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskResourceAssignment_TaskPMId",
                table: "TaskResourceAssignment",
                column: "TaskPMId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitsOfMeasurement_CreatedBy",
                table: "UnitsOfMeasurement",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UnitsOfMeasurement_LastModifiedBy",
                table: "UnitsOfMeasurement",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_VacationBalances_EmployeeId",
                table: "VacationBalances",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Vacations_AppUserId1",
                table: "Vacations",
                column: "AppUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Vacations_EmployeeId",
                table: "Vacations",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_VatTypes_TypeName",
                table: "VatTypes",
                column: "TypeName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_CreatedBy",
                table: "Warehouses",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_LastModifiedBy",
                table: "Warehouses",
                column: "LastModifiedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessPermissions");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "BusinessDocumentAttachments");

            migrationBuilder.DropTable(
                name: "BusinessDocumentLines");

            migrationBuilder.DropTable(
                name: "BusinessDocumentParties");

            migrationBuilder.DropTable(
                name: "BusinessDocumentRelations");

            migrationBuilder.DropTable(
                name: "BusinessDocumentStatusHistories");

            migrationBuilder.DropTable(
                name: "ColumnVisibilities");

            migrationBuilder.DropTable(
                name: "CommunicationPosts");

            migrationBuilder.DropTable(
                name: "CommunicationResponsibles");

            migrationBuilder.DropTable(
                name: "ContactDto");

            migrationBuilder.DropTable(
                name: "DocumentLinkDto");

            migrationBuilder.DropTable(
                name: "DocumentLinks");

            migrationBuilder.DropTable(
                name: "DocumentMetadata");

            migrationBuilder.DropTable(
                name: "DocumentStatusHistory");

            migrationBuilder.DropTable(
                name: "DocumentStatusHistoryDto");

            migrationBuilder.DropTable(
                name: "ElectronicDocument");

            migrationBuilder.DropTable(
                name: "EmployeeHistories");

            migrationBuilder.DropTable(
                name: "EmployeeShifts");

            migrationBuilder.DropTable(
                name: "ItemPrices");

            migrationBuilder.DropTable(
                name: "LeadHistories");

            migrationBuilder.DropTable(
                name: "LeadSources");

            migrationBuilder.DropTable(
                name: "MetadataEntry");

            migrationBuilder.DropTable(
                name: "OrderItemDiscounts");

            migrationBuilder.DropTable(
                name: "PartnerShifts");

            migrationBuilder.DropTable(
                name: "QuoteHistories");

            migrationBuilder.DropTable(
                name: "ResourceHistory");

            migrationBuilder.DropTable(
                name: "Salaries");

            migrationBuilder.DropTable(
                name: "SiteDto");

            migrationBuilder.DropTable(
                name: "SiteShifts");

            migrationBuilder.DropTable(
                name: "TaskAttachmentsPMs");

            migrationBuilder.DropTable(
                name: "TaskCommentsPMs");

            migrationBuilder.DropTable(
                name: "TaskDocumentLinks");

            migrationBuilder.DropTable(
                name: "TaskEmployeeAssignment");

            migrationBuilder.DropTable(
                name: "TaskHistory");

            migrationBuilder.DropTable(
                name: "TaskResourceAssignment");

            migrationBuilder.DropTable(
                name: "UnitsOfMeasurement");

            migrationBuilder.DropTable(
                name: "UserActivities");

            migrationBuilder.DropTable(
                name: "VacationBalances");

            migrationBuilder.DropTable(
                name: "Vacations");

            migrationBuilder.DropTable(
                name: "VatTypes");

            migrationBuilder.DropTable(
                name: "Warehouses");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AttachmentCategories");

            migrationBuilder.DropTable(
                name: "BusinessDocumentPartyRoles");

            migrationBuilder.DropTable(
                name: "BusinessDocumentRelationTypes");

            migrationBuilder.DropTable(
                name: "BusinessDocuments");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "PriceTypes");

            migrationBuilder.DropTable(
                name: "DocumentDto");

            migrationBuilder.DropTable(
                name: "Shifts");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Resource");

            migrationBuilder.DropTable(
                name: "TaskPM");

            migrationBuilder.DropTable(
                name: "BusinessDocumentStatuses");

            migrationBuilder.DropTable(
                name: "BusinessDocumentTypes");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropTable(
                name: "ItemTypes");

            migrationBuilder.DropTable(
                name: "PartnerDto");

            migrationBuilder.DropTable(
                name: "DocumentStatuses");

            migrationBuilder.DropTable(
                name: "DocumentTypes");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "ResourceStatus");

            migrationBuilder.DropTable(
                name: "ResourceType");

            migrationBuilder.DropTable(
                name: "CustomerCommunications");

            migrationBuilder.DropTable(
                name: "ProjectPM");

            migrationBuilder.DropTable(
                name: "TaskPMcomMethod");

            migrationBuilder.DropTable(
                name: "TaskPriorityPM");

            migrationBuilder.DropTable(
                name: "TaskStatusPM");

            migrationBuilder.DropTable(
                name: "TaskTypePM");

            migrationBuilder.DropTable(
                name: "StatusDto");

            migrationBuilder.DropTable(
                name: "EmploymentStatus");

            migrationBuilder.DropTable(
                name: "JobTitle");

            migrationBuilder.DropTable(
                name: "CommunicationStatuses");

            migrationBuilder.DropTable(
                name: "CommunicationTypes");

            migrationBuilder.DropTable(
                name: "Leads");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "ProjectStatusPMs");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "OrderShippingMethods");

            migrationBuilder.DropTable(
                name: "OrderStatusTypes");

            migrationBuilder.DropTable(
                name: "PaymentTerms");

            migrationBuilder.DropTable(
                name: "Quotes");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropTable(
                name: "Sites");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Partners");

            migrationBuilder.DropTable(
                name: "SiteTypes");

            migrationBuilder.DropTable(
                name: "GFO");

            migrationBuilder.DropTable(
                name: "PartnerGroups");

            migrationBuilder.DropTable(
                name: "PartnerStatuses");

            migrationBuilder.DropTable(
                name: "PartnerTypes");
        }
    }
}
