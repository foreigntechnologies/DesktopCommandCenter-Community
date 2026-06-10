using DesktopCommandCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DesktopCommandCenter.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Note> Notes { get; set; }
    public DbSet<ClipboardItem> ClipboardItems { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Note>().Property(n => n.Title).IsRequired().HasMaxLength(200);
    }
}

