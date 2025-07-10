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

    public DbSet<MovieRecord> Movies { get; set; }
    public DbSet<TvShowRecord> TvShows { get; set; }
    public DbSet<TvShowRecord> TvShowEpisodes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        Directory.CreateDirectory("assets");
        optionsBuilder.UseSqlite("Data Source=assets/trackster.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}

public class TvShowRecord
{
    [Key]
    public Guid Identifier { get; set; }
}

public class TvShowEpisodeRecord
{
    [Key]
    public Guid Identifier { get; set; }
    public TvShowRecord TvShow { get; set; }
}

public class MovieRecord
{
    [Key]
    public Guid Identifier { get; set; }
}