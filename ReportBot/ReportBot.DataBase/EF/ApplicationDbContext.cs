using McgTgBotNet.DB.Entities;
using Microsoft.EntityFrameworkCore;


namespace McgTgBotNet.Models;

public partial class ApplicationDbContext : DbContext
{
    public virtual DbSet<User> Users { get; set; } = null!;
    public virtual DbSet<Project> Projects { get; set; } = null!;
    public virtual DbSet<Report> Report { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
}