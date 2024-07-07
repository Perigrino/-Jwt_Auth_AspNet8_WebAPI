using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Jwt_Auth_AspNet8.Application.Data;

public class AppDbContext :DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    // protected override void OnModelCreating(ModelBuilder modelBuilder)  
    // {
    //     modelBuilder.Entity<Users>()
    //         .HasMany(u => u.ReferralCode)
    //         .WithOne(w => w.Users)
    //         .HasForeignKey(w => w.UserId);
    // }
    
    
}

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // Assuming the appsettings.json file is in the same directory
            .AddJsonFile(@Directory.GetCurrentDirectory() + "/../Jwt_Auth_AspNet8.API/appsettings.json")
            .Build();
        var connectionString = configuration.GetConnectionString("Default");
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}