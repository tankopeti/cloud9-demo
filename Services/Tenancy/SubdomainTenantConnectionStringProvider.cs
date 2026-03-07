using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;

namespace Cloud9_2.Services.Tenancy
{
    public class SubdomainTenantConnectionStringProvider : ITenantConnectionStringProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;

        public SubdomainTenantConnectionStringProvider(IHttpContextAccessor httpContextAccessor, IConfiguration config)
        {
            _httpContextAccessor = httpContextAccessor;
            _config = config;
        }

public string GetTenantKey()
{
    var http = _httpContextAccessor.HttpContext;
    var host = http?.Request?.Host.Host;

    // nincs host (startup) → default
    if (string.IsNullOrWhiteSpace(host))
        return (_config["Tenancy:DefaultTenant"] ?? "nyugalom").ToLowerInvariant();

    host = host.ToLowerInvariant();

    // ✅ sima localhost / 127.0.0.1 → default tenant
    if (host == "localhost" || host == "127.0.0.1")
        return (_config["Tenancy:DefaultTenant"] ?? "nyugalom").ToLowerInvariant();

    var subdomain = host.Split('.')[0];
    if (string.IsNullOrWhiteSpace(subdomain))
        return (_config["Tenancy:DefaultTenant"] ?? "nyugalom").ToLowerInvariant();

    return subdomain;
}

        public string GetConnectionString()
        {
            var tenantKey = GetTenantKey();

            // appsettings.json: Tenants:{tenantKey}:ConnectionStringEnv
            var envKey = _config[$"Tenants:{tenantKey}:ConnectionStringEnv"];
            if (string.IsNullOrWhiteSpace(envKey))
                throw new InvalidOperationException($"Unknown tenant '{tenantKey}' (no config at Tenants:{tenantKey}).");

            var cs = Environment.GetEnvironmentVariable(envKey);
            if (string.IsNullOrWhiteSpace(cs))
                throw new InvalidOperationException($"Tenant connection string env var not set: {envKey}");

            return cs;
        }
    }
}