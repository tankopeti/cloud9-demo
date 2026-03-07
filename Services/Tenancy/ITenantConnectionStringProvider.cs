namespace Cloud9_2.Services.Tenancy
{
    public interface ITenantConnectionStringProvider
    {
        string GetTenantKey();              // pl. "nyugalom"
        string GetConnectionString();        // a TENANT_* env varból
    }
}