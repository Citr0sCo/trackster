using Microsoft.EntityFrameworkCore;
using Trackster.Api.Data.Records;
using Trackster.Api.Features.Sessions.Types;

namespace Trackster.Api.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public DbSet<SessionRecord> Sessions { get; set; }
    public DbSet<UserRecord> Users { get; set; }
    public DbSet<MovieUserRecord> MovieUserLinks { get; set; }
    public DbSet<EpisodeUserRecord> EpisodeUserLinks { get; set; }
    public DbSet<MovieRecord> Movies { get; set; }
    public DbSet<ShowRecord> Shows { get; set; }
    public DbSet<SeasonRecord> Seasons { get; set; }
    public DbSet<EpisodeRecord> Episodes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        Directory.CreateDirectory("assets");
        optionsBuilder.UseSqlite("Data Source=assets/trackster.db");
        
        var value = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (value == "Development")
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}