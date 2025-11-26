using Infrastructure.Entities.Clustering;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public partial class ClusteringContext : DbContext
{
    public ClusteringContext()
    {
    }

    public ClusteringContext(DbContextOptions<ClusteringContext> options)
        : base(options)
    {
    }

    public DbSet<Cluster> Clusters { get; set; }

    public DbSet<UPCI2> UPCI2s { get; set; }

    public DbSet<StickyLocation> StickyLocations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cluster>(cluster =>
        {
            cluster.ToTable("Clusters", "output");

            cluster.HasKey(e => e.ClusterId)
                .IsClustered(false)
                .HasName("NonClusteredIndex-20240612-123941");

            cluster.Property(e => e.ClusterId)
                .ValueGeneratedNever();

            cluster.Property(e => e.UPCI)
                .HasMaxLength(9)
                .HasColumnName("UPCI2");

            cluster.HasIndex(x => x.UPCI)
                .HasDatabaseName("ClusteredIndex-20240612-123922")
                .IsClustered(true);

            cluster.OwnsMany(x => x.Members, member =>
            {
                member.WithOwner().HasForeignKey(x => x.ClusterId);
                member.ToTable("ClusterMembership", "output");
                member.HasKey(e => e.NodeKey);

                member.Property(e => e.NodeKey)
                      .HasMaxLength(50)
                      .IsRequired();

                member.OwnsMany(x => x.EdgeProbabilities, edge =>
                {
                    edge.HasKey(e => new { e.SourceKey, e.TargetKey });

                    edge.WithOwner().HasForeignKey(e => e.SourceKey);

                    edge.ToTable("Clusters_EdgeProbabilities", "output");

                    edge.Property(e => e.TempClusterId)
                        .IsRequired();

                    edge.Property(e => e.SourceKey)
                        .HasMaxLength(50)
                        .IsRequired();

                    edge.Property(e => e.SourceName)
                        .HasMaxLength(50)
                        .IsRequired();

                    edge.Property(e => e.TargetKey)
                        .HasMaxLength(50)
                        .IsRequired();

                    edge.Property(e => e.TargetName)
                        .HasMaxLength(50)
                        .IsRequired();

                    edge.Property(e => e.Probability)
                        .HasColumnType("decimal(18, 17)")
                        .IsRequired();
                });
            });

            cluster.OwnsMany(x => x.Attributes, attribute =>
            {
                attribute.WithOwner().HasForeignKey(x => x.ClusterId);

                attribute.ToTable("ClusterAttributes", "search");

                attribute.Property(e => e.UPCI)
                    .HasMaxLength(9)
                    .IsRequired()
                    .HasColumnName("UPCI2");

                attribute.HasIndex(e => e.UPCI)
                    .IsClustered(true);

                attribute.HasKey(e => new
                {
                    e.UPCI,
                    e.RecordSource,
                    e.Identifier
                }).IsClustered(false)
                .HasName("fake_pk");

                attribute.Property(e => e.RecordSource)
                    .HasMaxLength(50)
                    .IsRequired();

                attribute.Property(e => e.Identifier)
                    .HasMaxLength(50)
                    .IsRequired();

                attribute.Property(e => e.PrimaryRecord)
                    .IsRequired();

                attribute.Property(e => e.LastName)
                    .IsRequired(false)
                    .HasMaxLength(100);

                attribute.Property(e => e.DateOfBirth)
                    .IsRequired()
                    .HasColumnName("DOB")
                    .HasColumnType("Date");

                attribute.HasIndex(e => new
                {
                    e.RecordSource,
                    e.LastName,
                    e.DateOfBirth
                }).HasDatabaseName("NonClusteredIndex-20240612-140452");

                attribute.HasIndex(e => new
                {
                    e.RecordSource,
                    e.Identifier
                }).HasDatabaseName("NonClusteredIndex-20240612-140433");

            });
        });

        modelBuilder.Entity<UPCI2>(upci2 =>
        {
            upci2.ToTable("UPCI2", "reference");
            upci2.HasKey(e => e.ClusterId);

            upci2.Property(e => e.Upci)
                .HasMaxLength(9)
                .HasColumnName("UPCI2");
        });
        
        modelBuilder.Entity<StickyLocation>(sl =>
        {
            sl.ToTable("StickyLocations", "reference");
            sl.HasKey(e => e.Upci);

            sl.Property(e => e.Upci)
                .HasMaxLength(9)
                .HasColumnName("Upci2");

            sl.Property(e => e.OrgCode)
                .HasMaxLength(4)
                .IsRequired();
        });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);





}