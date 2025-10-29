using Infrastructure.Entities.Audit;

namespace Infrastructure.Contexts;

public class AuditContext : DbContext
{
    public AuditContext() { }

    public AuditContext(DbContextOptions<AuditContext> options)
        : base(options) { }


    public DbSet<AuditEntry> Entries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditEntry>(entity =>
        {
            entity.ToTable("AuditEntries");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.CorrelationId)
                .IsRequired()
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.EntityName)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(e => e.Action)
                .IsRequired()
                .HasMaxLength(32);

            entity.Property(e => e.Timestamp)
                .IsRequired();

            entity.Property(e => e.PerformedBy)
                .HasMaxLength(256);

            entity.HasIndex(e => e.EntityName);
            entity.HasIndex(e => e.Action);

        });

        base.OnModelCreating(modelBuilder);
    }
}
