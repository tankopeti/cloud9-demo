using AutoMapper;
using Cloud9_2.Models;

namespace Cloud9_2.Mapping
{
    public class BusinessDocumentProfile : Profile
    {
        public BusinessDocumentProfile()
        {
            // -------------------------
            // LOOKUPS
            // -------------------------
            CreateMap<AttachmentCategoryLookup, AttachmentCategoryDto>().ReverseMap();
            CreateMap<BusinessDocumentPartyRole, BusinessDocumentPartyRoleDto>().ReverseMap();
            CreateMap<BusinessDocumentRelationType, BusinessDocumentRelationTypeDto>().ReverseMap();
            CreateMap<BusinessDocumentStatus, BusinessDocumentStatusDto>().ReverseMap();
            CreateMap<BusinessDocumentType, BusinessDocumentTypeDto>().ReverseMap();

            // -------------------------
            // ELECTRONIC DOCUMENT
            // (figyelj: entity-ben isVoteHalfValueEnabled, DTO-ban IsVoteHalfValueEnabled)
            // -------------------------
            CreateMap<ElectronicDocument, ElectronicDocumentDto>()
                .ForMember(d => d.IsVoteHalfValueEnabled,
                    o => o.MapFrom(s => s.isVoteHalfValueEnabled));

            CreateMap<ElectronicDocumentCreateDto, ElectronicDocument>()
                .ForMember(d => d.isVoteHalfValueEnabled,
                    o => o.MapFrom(s => s.IsVoteHalfValueEnabled));

            CreateMap<ElectronicDocumentUpdateDto, ElectronicDocument>()
                .ForMember(d => d.isVoteHalfValueEnabled,
                    o => o.MapFrom(s => s.IsVoteHalfValueEnabled));

            // -------------------------
            // BUSINESS DOCUMENT (header)
            // -------------------------
            CreateMap<BusinessDocument, BusinessDocumentDto>();

            CreateMap<BusinessDocumentCreateDto, BusinessDocument>()
                // navigációkhoz ne nyúljon
                .ForMember(d => d.Tenant, o => o.Ignore())
                .ForMember(d => d.BusinessDocumentType, o => o.Ignore())
                .ForMember(d => d.BusinessDocumentStatus, o => o.Ignore())
                .ForMember(d => d.Lines, o => o.Ignore())
                .ForMember(d => d.Parties, o => o.Ignore())
                .ForMember(d => d.FromRelations, o => o.Ignore())
                .ForMember(d => d.ToRelations, o => o.Ignore())
                .ForMember(d => d.Attachments, o => o.Ignore())
                .ForMember(d => d.StatusHistory, o => o.Ignore());

            CreateMap<BusinessDocumentUpdateDto, BusinessDocument>()
                // Tenantet nem engedünk módosítani
                .ForMember(d => d.TenantId, o => o.Ignore())
                .ForMember(d => d.Tenant, o => o.Ignore())
                // navigációkhoz ne nyúljon
                .ForMember(d => d.BusinessDocumentType, o => o.Ignore())
                .ForMember(d => d.BusinessDocumentStatus, o => o.Ignore())
                .ForMember(d => d.Lines, o => o.Ignore())
                .ForMember(d => d.Parties, o => o.Ignore())
                .ForMember(d => d.FromRelations, o => o.Ignore())
                .ForMember(d => d.ToRelations, o => o.Ignore())
                .ForMember(d => d.Attachments, o => o.Ignore())
                .ForMember(d => d.StatusHistory, o => o.Ignore());

            // -------------------------
            // LINES
            // -------------------------
            CreateMap<BusinessDocumentLine, BusinessDocumentLineDto>();

            CreateMap<BusinessDocumentLineCreateDto, BusinessDocumentLine>()
                // ezeket a service-ben állítod be (doc.TenantId + businessDocumentId)
                .ForMember(d => d.TenantId, o => o.Ignore())
                .ForMember(d => d.Tenant, o => o.Ignore())
                .ForMember(d => d.BusinessDocumentId, o => o.Ignore())
                .ForMember(d => d.BusinessDocument, o => o.Ignore())
                // számolt mezők (ha később használod)
                .ForMember(d => d.NetAmount, o => o.Ignore())
                .ForMember(d => d.TaxAmount, o => o.Ignore())
                .ForMember(d => d.GrossAmount, o => o.Ignore());

            CreateMap<BusinessDocumentLineUpdateDto, BusinessDocumentLine>()
                .ForMember(d => d.TenantId, o => o.Ignore())
                .ForMember(d => d.Tenant, o => o.Ignore())
                .ForMember(d => d.BusinessDocumentId, o => o.Ignore())
                .ForMember(d => d.BusinessDocument, o => o.Ignore())
                .ForMember(d => d.NetAmount, o => o.Ignore())
                .ForMember(d => d.TaxAmount, o => o.Ignore())
                .ForMember(d => d.GrossAmount, o => o.Ignore());

            // -------------------------
            // PARTIES
            // -------------------------
            CreateMap<BusinessDocumentParty, BusinessDocumentPartyDto>();

            CreateMap<BusinessDocumentPartyCreateDto, BusinessDocumentParty>()
                .ForMember(d => d.TenantId, o => o.Ignore())
                .ForMember(d => d.Tenant, o => o.Ignore())
                .ForMember(d => d.BusinessDocumentId, o => o.Ignore())
                .ForMember(d => d.BusinessDocument, o => o.Ignore())
                .ForMember(d => d.BusinessDocumentPartyRole, o => o.Ignore())
                .ForMember(d => d.Partner, o => o.Ignore())
                .ForMember(d => d.Site, o => o.Ignore())
                .ForMember(d => d.Contact, o => o.Ignore());

            CreateMap<BusinessDocumentPartyUpdateDto, BusinessDocumentParty>()
                .ForMember(d => d.TenantId, o => o.Ignore())
                .ForMember(d => d.Tenant, o => o.Ignore())
                .ForMember(d => d.BusinessDocumentId, o => o.Ignore())
                .ForMember(d => d.BusinessDocument, o => o.Ignore())
                .ForMember(d => d.BusinessDocumentPartyRole, o => o.Ignore())
                .ForMember(d => d.Partner, o => o.Ignore())
                .ForMember(d => d.Site, o => o.Ignore())
                .ForMember(d => d.Contact, o => o.Ignore());

            // -------------------------
            // RELATIONS
            // -------------------------
            CreateMap<BusinessDocumentRelation, BusinessDocumentRelationDto>();

            CreateMap<BusinessDocumentRelationCreateDto, BusinessDocumentRelation>()
                .ForMember(d => d.Tenant, o => o.Ignore())
                .ForMember(d => d.FromBusinessDocument, o => o.Ignore())
                .ForMember(d => d.ToBusinessDocument, o => o.Ignore())
                .ForMember(d => d.BusinessDocumentRelationType, o => o.Ignore())
                // CreatedAt/CreatedBy a service-ben
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                .ForMember(d => d.CreatedBy, o => o.Ignore());

            // -------------------------
            // ATTACHMENTS
            // -------------------------
            CreateMap<BusinessDocumentAttachment, BusinessDocumentAttachmentDto>();

            CreateMap<BusinessDocumentAttachmentCreateDto, BusinessDocumentAttachment>()
                .ForMember(d => d.Tenant, o => o.Ignore())
                .ForMember(d => d.BusinessDocument, o => o.Ignore())
                .ForMember(d => d.Document, o => o.Ignore())
                .ForMember(d => d.AttachmentCategory, o => o.Ignore());

            CreateMap<BusinessDocumentAttachmentUpdateDto, BusinessDocumentAttachment>()
                // ezek nem módosíthatók update-nél
                .ForMember(d => d.TenantId, o => o.Ignore())
                .ForMember(d => d.Tenant, o => o.Ignore())
                .ForMember(d => d.BusinessDocumentId, o => o.Ignore())
                .ForMember(d => d.BusinessDocument, o => o.Ignore())
                .ForMember(d => d.DocumentId, o => o.Ignore())
                .ForMember(d => d.Document, o => o.Ignore())
                .ForMember(d => d.AttachmentCategory, o => o.Ignore());

            // -------------------------
            // STATUS HISTORY
            // -------------------------
            CreateMap<BusinessDocumentStatusHistory, BusinessDocumentStatusHistoryDto>();

            CreateMap<BusinessDocumentStatusHistoryCreateDto, BusinessDocumentStatusHistory>()
                .ForMember(d => d.Tenant, o => o.Ignore())
                .ForMember(d => d.BusinessDocument, o => o.Ignore())
                .ForMember(d => d.OldStatus, o => o.Ignore())
                .ForMember(d => d.NewStatus, o => o.Ignore())
                // ChangeDate/ChangedBy a service-ben
                .ForMember(d => d.ChangeDate, o => o.Ignore())
                .ForMember(d => d.ChangedBy, o => o.Ignore());
        }
    }
}
