using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Trackster.Api.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

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
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}

public class ShowRecord
{
    [Key]
    public Guid Identifier { get; set; }

    public string Title { get; set; }
    public int Year { get; set; }
    public string TMDB { get; set; }
}

public class SeasonRecord
{
    [Key]
    public Guid Identifier { get; set; }
    public int Number { get; set; }
    public ShowRecord Show { get; set; }
}

public class EpisodeRecord
{
    [Key]
    public Guid Identifier { get; set; }
    public int Number { get; set; }
    public SeasonRecord Season { get; set; }
}

public class MovieRecord
{
    [Key]
    public Guid Identifier { get; set; }

    public string Title { get; set; }
    public int Year { get; set; }
    public string TMDB { get; set; }
}

public class UserRecord
{
    [Key]
    public Guid Identifier { get; set; }
    public string Username { get; set; }
}

public class MovieUserRecord
{
    [Key]
    public Guid Identifier { get; set; }

    public UserRecord User { get; set; }
    public MovieRecord Movie { get; set; }
    public DateTime CollectedAt { get; set; }
}

public class EpisodeUserRecord
{
    [Key]
    public Guid Identifier { get; set; }

    public UserRecord User { get; set; }
    public EpisodeRecord Episode { get; set; }
    public DateTime CollectedAt { get; set; }
}