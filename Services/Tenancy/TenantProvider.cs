using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace Cloud9_2.Services.Tenancy
{
    public class TenantProvider : ITenantProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? GetTenantId()
        {
            var http = _httpContextAccessor.HttpContext;
            if (http == null) return null; // <-- ez a lényeg

            var claim = http.User?.FindFirst("tenant_id");
            if (claim != null && int.TryParse(claim.Value, out var id))
                return id;

            if (http.Request.Headers.TryGetValue("X-TenantId", out var hv) &&
                int.TryParse(hv.FirstOrDefault(), out var hid))
                return hid;

            return null; // devben lehet 1, de EF model buildnél inkább null
        }
    }
}
