using Infrastructure.Entities.Delius;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public partial class DeliusContext : DbContext
{
    public DeliusContext()
    {
    }

    public DeliusContext(DbContextOptions<DeliusContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdditionalIdentifier> AdditionalIdentifiers { get; set; }

    public virtual DbSet<AliasDetail> AliasDetails { get; set; }

    public virtual DbSet<Disability> Disabilities { get; set; }

    public virtual DbSet<Disposal> Disposals { get; set; }

    public virtual DbSet<EventDetail> EventDetails { get; set; }

    public virtual DbSet<MainOffence> MainOffences { get; set; }

    public virtual DbSet<Oa> Oas { get; set; }

    public virtual DbSet<Offender> Offenders { get; set; }

    public virtual DbSet<OffenderAddress> OffenderAddresses { get; set; }

    public virtual DbSet<OffenderManager> OffenderManagers { get; set; }

    public virtual DbSet<OffenderManagerBuilding> OffenderManagerBuildings { get; set; }

    public virtual DbSet<OffenderManagerTeam> OffenderManagerTeams { get; set; }

    public virtual DbSet<OffenderToOffenderManagerMapping> OffenderToOffenderManagerMappings { get; set; }

    public virtual DbSet<OffenderTransfer> OffenderTransfers { get; set; }

    public virtual DbSet<PersonalCircumstance> PersonalCircumstances { get; set; }

    public virtual DbSet<ProcessedFile> ProcessedFiles { get; set; }

    public virtual DbSet<Provision> Provisions { get; set; }

    public virtual DbSet<RegistrationDetail> RegistrationDetails { get; set; }

    public virtual DbSet<Requirement> Requirements { get; set; }

    public virtual DbSet<StandardisationReference> StandardisationReferences { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdditionalIdentifier>(entity =>
        {
            entity
                .ToTable("AdditionalIdentifier", "DeliusRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("AdditionalIdentifier", "DeliusTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Deleted).HasMaxLength(1);
            entity.Property(e => e.MilitaryServiceNumber).HasMaxLength(30);
            entity.Property(e => e.OldPnc).HasMaxLength(30);
            entity.Property(e => e.Pnc)
                .HasMaxLength(30)
                .HasColumnName("PNC");
            entity.Property(e => e.Yot)
                .HasMaxLength(30)
                .HasColumnName("YOT");

            entity.HasOne(e => e.Offender)
                .WithMany(e => e.AdditionalIdentifiers)
                .HasForeignKey(e => e.OffenderId);
        });

        modelBuilder.Entity<AliasDetail>(entity =>
        {
            entity
                .ToTable("AliasDetails", "DeliusRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("AliasDetails", "DeliusTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Deleted).HasMaxLength(1);
            entity.Property(e => e.FirstName).HasMaxLength(35);
            entity.Property(e => e.GenderCode).HasMaxLength(100);
            entity.Property(e => e.GenderDescription).HasMaxLength(500);
            entity.Property(e => e.RowNo).ValueGeneratedOnAdd();
            entity.Property(e => e.SecondName).HasMaxLength(35);
            entity.Property(e => e.Surname).HasMaxLength(35);
            entity.Property(e => e.ThirdName).HasMaxLength(35);

            entity.HasOne(e => e.Offender)
                .WithMany(e => e.AliasDetails)
                .HasForeignKey(e => e.OffenderId);
        });

        modelBuilder.Entity<Disability>(entity =>
        {
            entity
                .ToTable("Disability", "DeliusRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("Disability", "DeliusTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Deleted).HasMaxLength(1);
            entity.Property(e => e.TypeCode).HasMaxLength(100);
            entity.Property(e => e.TypeDescription).HasMaxLength(500);

            entity.HasOne(e => e.Offender)
                .WithMany(e => e.Disabilities)
                .HasForeignKey(e => e.OffenderId);
        });

        modelBuilder.Entity<Disposal>(entity =>
        {
            entity
                .ToTable("Disposal", "DeliusRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("Disposal", "DeliusTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Deleted).HasMaxLength(1);
            entity.Property(e => e.DisposalCode).HasMaxLength(30);
            entity.Property(e => e.DisposalDetail).HasMaxLength(128);
            entity.Property(e => e.DisposalTerminationCode).HasMaxLength(100);
            entity.Property(e => e.DisposalTerminationDescription).HasMaxLength(500);
            entity.Property(e => e.Length).HasMaxLength(38);
            entity.Property(e => e.UnitCode).HasMaxLength(100);
            entity.Property(e => e.UnitDescription).HasMaxLength(500);

            entity.HasOne(e => e.Offender)
                .WithMany(e => e.Disposals)
                .HasForeignKey(e => e.OffenderId);
        });

        modelBuilder.Entity<EventDetail>(entity =>
        {
            entity
                .ToTable("EventDetails", "DeliusRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("EventDetails", "DeliusTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Cohort).HasMaxLength(11);
            entity.Property(e => e.Deleted).HasMaxLength(1);

            entity.HasOne(e => e.Offender)
                .WithMany(e => e.EventDetails)
                .HasForeignKey(e => e.OffenderId);
        });

        modelBuilder.Entity<MainOffence>(entity =>
        {
            entity
                .ToTable("MainOffence", "DeliusRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("MainOffence", "DeliusTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Deleted).HasMaxLength(1);
            entity.Property(e => e.OffenceCode).HasMaxLength(5);
            entity.Property(e => e.OffenceDescription).HasMaxLength(300);

            entity.HasOne(e => e.Offender)
                .WithMany(e => e.MainOffences)
                .HasForeignKey(e => e.OffenderId);
        });

        modelBuilder.Entity<Oa>(entity =>
        {
            entity
                .ToTable("OAS", "DeliusRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("OAS", "DeliusTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Deleted).HasMaxLength(1);
            entity.Property(e => e.Roshscore)
                .HasMaxLength(38)
                .HasColumnName("ROSHScore");

            entity.HasOne(e => e.Offender)
                .WithMany(e => e.OAs)
                .HasForeignKey(e => e.OffenderId);
        });

        modelBuilder.Entity<Offender>(entity =>
        {
            entity.HasKey(e => e.OffenderId).IsClustered(false);

            entity
                .ToTable("Offenders", "DeliusRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("Offenders", "DeliusTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.HasIndex(e => e.Nomisnumber, "IX_Offenders").IsClustered();

            entity.Property(e => e.OffenderId).ValueGeneratedNever();
            entity.Property(e => e.Crn)
                .HasMaxLength(7)
                .HasColumnName("CRN");
            entity.Property(e => e.Cro)
                .HasMaxLength(12)
                .HasColumnName("CRO");
            entity.Property(e => e.Deleted).HasMaxLength(1);
            entity.Property(e => e.EthnicityCode).HasMaxLength(100);
            entity.Property(e => e.EthnicityDescription).HasMaxLength(500);
            entity.Property(e => e.FirstName).HasMaxLength(35);
            entity.Property(e => e.GenderCode).HasMaxLength(100);
            entity.Property(e => e.GenderDescription).HasMaxLength(500);
            entity.Property(e => e.ImmigrationNumber).HasMaxLength(20);
            entity.Property(e => e.ImmigrationStatusCode).HasMaxLength(100);
            entity.Property(e => e.ImmigrationStatusDescription).HasMaxLength(500);
            entity.Property(e => e.NationalityCode).HasMaxLength(100);
            entity.Property(e => e.NationalityDescription).HasMaxLength(500);
            entity.Property(e => e.Nino)
                .HasMaxLength(9)
                .HasColumnName("NINO");
            entity.Property(e => e.Nomisnumber)
                .HasMaxLength(7)
                .HasColumnName("NOMISNumber");
            entity.Property(e => e.Pncnumber)
                .HasMaxLength(13)
                .HasColumnName("PNCNumber");
            entity.Property(e => e.PreviousSurname).HasMaxLength(35);
            entity.Property(e => e.PrisonNumber).HasMaxLength(10);
            entity.Property(e => e.SecondName).HasMaxLength(35);
            entity.Property(e => e.SecondNationalityCode).HasMaxLength(100);
            entity.Property(e => e.SecondNationalityDescription).HasMaxLength(500);
            entity.Property(e => e.Surname).HasMaxLength(35);
            entity.Property(e => e.ThirdName).HasMaxLength(35);
            entity.Property(e => e.TitleCode).HasMaxLength(100);
            entity.Property(e => e.TitleDescription).HasMaxLength(500);
        });

        modelBuilder.Entity<OffenderAddress>(entity =>
        {
            entity
                .ToTable("OffenderAddress", "DeliusRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("OffenderAddress", "DeliusTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.BuildingName).HasMaxLength(35);
            entity.Property(e => e.County).HasMaxLength(35);
            entity.Property(e => e.Deleted).HasMaxLength(1);
            entity.Property(e => e.District).HasMaxLength(35);
            entity.Property(e => e.HouseNumber).HasMaxLength(35);
            entity.Property(e => e.NoFixedAbode).HasMaxLength(1);
            entity.Property(e => e.Postcode).HasMaxLength(8);
            entity.Property(e => e.StatusCode).HasMaxLength(100);
            entity.Property(e => e.StatusDescription).HasMaxLength(500);
            entity.Property(e => e.StreetName).HasMaxLength(35);
            entity.Property(e => e.Town).HasMaxLength(35);

            entity.HasOne(e => e.Offender)
                .WithMany(e => e.Addresses)
                .HasForeignKey(e => e.OffenderId);
        });

        modelBuilder.Entity<OffenderManager>(entity =>
        {
            entity.HasKey(e => new { e.OmCode, e.TeamCode });

            entity
                .ToTable("OffenderManager", "DeliusRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("OffenderManager", "DeliusTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.OmCode).HasMaxLength(7);
            entity.Property(e => e.TeamCode).HasMaxLength(6);
            entity.Property(e => e.ContactNo).HasMaxLength(35);
            entity.Property(e => e.OmForename).HasMaxLength(35);
            entity.Property(e => e.OmSurname).HasMaxLength(35);
            entity.Property(e => e.OrgCode).HasMaxLength(3);
        });

        modelBuilder.Entity<OffenderManagerBuilding>(entity =>
        {
            entity.HasKey(e => e.CompositeHash);

            entity
                .ToTable("OffenderManagerBuildings", "DeliusRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("OffenderManagerBuildings", "DeliusTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.BuildingName).HasMaxLength(35);
            entity.Property(e => e.County).HasMaxLength(35);
            entity.Property(e => e.District).HasMaxLength(35);
            entity.Property(e => e.HouseNumber).HasMaxLength(35);
            entity.Property(e => e.PostCode).HasMaxLength(8);
            entity.Property(e => e.Street).HasMaxLength(35);
            entity.Property(e => e.Town).HasMaxLength(35);


        });

        modelBuilder.Entity<OffenderManagerTeam>(entity =>
        {
            entity.HasKey(e => new { e.OrgCode, e.TeamCode, e.CompositeBuildingHash });

            entity
                .ToTable("OffenderManagerTeam", "DeliusRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("OffenderManagerTeam", "DeliusTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.OrgCode).HasMaxLength(3);
            entity.Property(e => e.TeamCode).HasMaxLength(6);
            entity.Property(e => e.CompositeBuildingHash).HasMaxLength(32);
            entity.Property(e => e.OrgDescription).HasMaxLength(60);
            entity.Property(e => e.TeamDescription).HasMaxLength(50);
        });

        modelBuilder.Entity<OffenderToOffenderManagerMapping>(entity =>
        {
            entity.HasKey(e => new { e.OffenderId, e.Id, e.OmCode });

            entity
                .ToTable("OffenderToOffenderManagerMappings", "DeliusRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("OffenderToOffenderManagerMappings", "DeliusTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.OrgCode).HasMaxLength(3);
            entity.Property(e => e.TeamCode).HasMaxLength(6);
            entity.Property(e => e.OmCode).HasMaxLength(7);
            entity.Property(e => e.Deleted).HasMaxLength(1);

            entity.HasOne(e => e.Offender)
                .WithMany(e => e.OffenderToOffenderManagerMappings)
                .HasForeignKey(e => e.OffenderId);
        });

        modelBuilder.Entity<OffenderTransfer>(entity =>
        {
            entity
                .ToTable("OffenderTransfer", "DeliusRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("OffenderTransfer", "DeliusTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Deleted).HasMaxLength(1);
            entity.Property(e => e.ReasonCode).HasMaxLength(100);
            entity.Property(e => e.ReasonDescription).HasMaxLength(500);
            entity.Property(e => e.StatusCode).HasMaxLength(100);
            entity.Property(e => e.StatusDescription).HasMaxLength(500);

            entity.HasOne(e => e.Offender)
                .WithMany(e => e.Transfers)
                .HasForeignKey(e => e.OffenderId);
        });

        modelBuilder.Entity<PersonalCircumstance>(entity =>
        {
            entity
                .ToTable("PersonalCircumstances", "DeliusRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("PersonalCircumstances", "DeliusTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Deleted).HasMaxLength(1);
            entity.Property(e => e.SubType).HasMaxLength(100);
            entity.Property(e => e.SubTypeDescription).HasMaxLength(500);
            entity.Property(e => e.Type).HasMaxLength(100);
            entity.Property(e => e.TypeDescription).HasMaxLength(500);

            entity.HasOne(e => e.Offender)
                .WithMany(e => e.PersonalCircumstances)
                .HasForeignKey(e => e.OffenderId);
        });

        modelBuilder.Entity<ProcessedFile>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ProcessedFiles", "DeliusRunningPicture");

            entity.Property(e => e.FileName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Provision>(entity =>
        {
            entity
                .ToTable("Provision", "DeliusRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("Provision", "DeliusTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Code).HasMaxLength(100);
            entity.Property(e => e.Deleted).HasMaxLength(1);
            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasOne(e => e.Offender)
                .WithMany(e => e.Provisions)
                .HasForeignKey(e => e.OffenderId);
        });

        modelBuilder.Entity<RegistrationDetail>(entity =>
        {
            entity
                .ToTable("RegistrationDetails", "DeliusRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("RegistrationDetails", "DeliusTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CategoryCode).HasMaxLength(100);
            entity.Property(e => e.CategoryDescription).HasMaxLength(500);
            entity.Property(e => e.DeRegistered).HasMaxLength(1);
            entity.Property(e => e.Deleted).HasMaxLength(1);
            entity.Property(e => e.RegisterCode).HasMaxLength(100);
            entity.Property(e => e.RegisterDescription).HasMaxLength(500);
            entity.Property(e => e.TypeCode).HasMaxLength(10);
            entity.Property(e => e.TypeDescription).HasMaxLength(50);

            entity.HasOne(e => e.Offender)
                .WithMany(e => e.RegistrationDetails)
                .HasForeignKey(e => e.OffenderId);
        });

        modelBuilder.Entity<Requirement>(entity =>
        {
            entity
                .ToTable("Requirement", "DeliusRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("Requirement", "DeliusTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CategoryCode).HasMaxLength(20);
            entity.Property(e => e.CategoryDescription).HasMaxLength(200);
            entity.Property(e => e.Deleted).HasMaxLength(1);
            entity.Property(e => e.Length).HasMaxLength(38);
            entity.Property(e => e.SubCategoryCode).HasMaxLength(100);
            entity.Property(e => e.SubCategoryDescription).HasMaxLength(500);
            entity.Property(e => e.TerminationDescription).HasMaxLength(500);
            entity.Property(e => e.TerminationReasonCode).HasMaxLength(100);
            entity.Property(e => e.UnitCode).HasMaxLength(100);
            entity.Property(e => e.UnitDescription).HasMaxLength(500);

            entity.HasOne(e => e.Offender)
                .WithMany(e => e.Requirements)
                .HasForeignKey(e => e.OffenderId);
        });

        modelBuilder.Entity<StandardisationReference>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("StandardisationReference", "DeliusRunningPicture");

            entity.Property(e => e.CleanedData).HasMaxLength(100);
            entity.Property(e => e.RawData).HasMaxLength(100);
            entity.Property(e => e.Source).HasMaxLength(200);
            entity.Property(e => e.Type).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
