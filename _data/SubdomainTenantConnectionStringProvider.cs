using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;

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

            var defaultTenant = (_config["Tenancy:DefaultTenant"] ?? "nyugalom").ToLowerInvariant();

            // nincs host (startup) → default
            if (string.IsNullOrWhiteSpace(host))
                return defaultTenant;

            host = host.ToLowerInvariant();

            // localhost → default tenant
            if (host == "localhost")
                return defaultTenant;

            // bármilyen IP cím → default tenant
            if (IPAddress.TryParse(host, out _))
                return defaultTenant;

            var subdomain = host.Split('.')[0];
            if (string.IsNullOrWhiteSpace(subdomain))
                return defaultTenant;

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