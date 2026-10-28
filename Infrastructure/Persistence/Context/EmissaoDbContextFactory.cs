using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence.Context;

public sealed class EmissaoDbContextFactory : IDesignTimeDbContextFactory<EmissaoDbContext>
{
    public EmissaoDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__Default")
            ?? "Server=localhost,1433;Database=EmissaoCartao;User Id=sa;Password=Your_strong_Password123;TrustServerCertificate=True;Encrypt=False";

        var options = new DbContextOptionsBuilder<EmissaoDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new EmissaoDbContext(options);
    }
}
