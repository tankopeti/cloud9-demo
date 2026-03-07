using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Cloud9_2.Data;   // <-- ez kell

namespace Cloud9_2.Infrastructure;

public class DesignTimeDbContextFactory 
    : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=localhost,1433;Database=Cloud9_Nyugalom;User Id=sa;Password=JELSZO;TrustServerCertificate=True");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}