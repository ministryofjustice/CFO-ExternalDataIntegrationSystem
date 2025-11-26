using Infrastructure.Entities.Offloc;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public partial class OfflocContext : DbContext
{
    public OfflocContext()
    {
    }

    public OfflocContext(DbContextOptions<OfflocContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Activity> Activities { get; set; }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Agency> Agencies { get; set; }

    public virtual DbSet<Assessment> Assessments { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Employment> Employments { get; set; }

    public virtual DbSet<Flag> Flags { get; set; }

    public virtual DbSet<Identifier> Identifiers { get; set; }

    public virtual DbSet<IncentiveLevel> IncentiveLevels { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<MainOffence> MainOffences { get; set; }

    public virtual DbSet<Movement> Movements { get; set; }

    public virtual DbSet<OffenderAgency> OffenderAgencies { get; set; }

    public virtual DbSet<OffenderStatus> OffenderStatuses { get; set; }

    public virtual DbSet<OtherOffence> OtherOffences { get; set; }

    public virtual DbSet<PersonalDetail> PersonalDetails { get; set; }

    public virtual DbSet<Pnc> Pncs { get; set; }

    public virtual DbSet<PreviousPrisonNumber> PreviousPrisonNumbers { get; set; }

    public virtual DbSet<ProcessedFile> ProcessedFiles { get; set; }

    public virtual DbSet<SentenceInformation> SentenceInformations { get; set; }

    public virtual DbSet<SexOffender> SexOffenders { get; set; }

    public virtual DbSet<StandardisationReference> StandardisationReferences { get; set; }

    public virtual DbSet<VeteranFlagLog> VeteranFlagLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Activity>(entity =>
        {
            entity.HasKey(e => new { e.NomsNumber, e.Activity1, e.Location, e.StartHour, e.StartMin, e.EndHour, e.EndMin });

            entity
                .ToTable("Activities", "OfflocRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("Activities", "OfflocTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.NomsNumber)
                .HasMaxLength(7)
                .HasColumnName("NOMSnumber");
            entity.Property(e => e.Activity1)
                .HasMaxLength(50)
                .HasColumnName("Activity");
            entity.Property(e => e.Location).HasMaxLength(50);

            entity.HasOne(e => e.PersonalDetail)
                .WithMany(e => e.Activities)
                .HasForeignKey(e => e.NomsNumber);
        });

        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => new { e.NomsNumber, e.AddressType });

            entity
                .ToTable("Addresses", "OfflocRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("Addresses", "OfflocTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.NomsNumber)
                .HasMaxLength(7)
                .HasColumnName("NOMSnumber");
            entity.Property(e => e.AddressType).HasMaxLength(15);
            entity.Property(e => e.Address1).HasMaxLength(100);
            entity.Property(e => e.Address2).HasMaxLength(100);
            entity.Property(e => e.Address3).HasMaxLength(100);
            entity.Property(e => e.Address4).HasMaxLength(100);
            entity.Property(e => e.Address5).HasMaxLength(100);
            entity.Property(e => e.Address6).HasMaxLength(100);
            entity.Property(e => e.Address7).HasMaxLength(100);
            entity.Property(e => e.AddressRelationship).HasMaxLength(60);
            entity.Property(e => e.NominatedNok)
                .HasMaxLength(100)
                .HasColumnName("NominatedNOK");

            entity.HasOne(e => e.PersonalDetail)
                .WithMany(e => e.Addresses)
                .HasForeignKey(e => e.NomsNumber);
        });

        modelBuilder.Entity<Agency>(entity =>
        {
            entity.HasKey(e => e.EstablishmentCode);

            entity
                .ToTable("Agencies", "OfflocRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("Agencies", "OfflocTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.EstablishmentCode).HasMaxLength(3);
            entity.Property(e => e.Establishment).HasMaxLength(40);
        });

        modelBuilder.Entity<Assessment>(entity =>
        {
            entity.HasKey(e => e.NomsNumber);

            entity
                .ToTable("Assessments", "OfflocRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("Assessments", "OfflocTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.NomsNumber)
                .HasMaxLength(7)
                .HasColumnName("NOMSnumber");
            entity.Property(e => e.SecurityCategory).HasMaxLength(30);

            entity.HasOne(e => e.PersonalDetail)
                .WithMany(e => e.Assessments)
                .HasForeignKey(e => e.NomsNumber);
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => new { e.NomsNumber, e.PrisonNumber });

            entity
                .ToTable("Bookings", "OfflocRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("Bookings", "OfflocTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.NomsNumber)
                .HasMaxLength(7)
                .HasColumnName("NOMSnumber");
            entity.Property(e => e.PrisonNumber).HasMaxLength(6);

            entity.HasOne(e => e.PersonalDetail)
                .WithMany(e => e.Bookings)
                .HasForeignKey(e => e.NomsNumber);
        });

        modelBuilder.Entity<Employment>(entity =>
        {
            entity.HasKey(e => e.NomsNumber);

            entity
                .ToTable("Employment", "OfflocRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("Employment", "OfflocTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.NomsNumber)
                .HasMaxLength(7)
                .HasColumnName("NOMSnumber");
            entity.Property(e => e.Employed)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.EmploymentStatusDischarge).HasMaxLength(40);
            entity.Property(e => e.EmploymentStatusReception).HasMaxLength(40);

            entity.HasOne(e => e.PersonalDetail)
                .WithMany(e => e.Employments)
                .HasForeignKey(e => e.NomsNumber);
        });

        modelBuilder.Entity<Flag>(entity =>
        {
            entity.HasKey(e => new { e.NomsNumber, e.Details });

            entity
                .ToTable("Flags", "OfflocRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("Flags", "OfflocTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.NomsNumber)
                .HasMaxLength(7)
                .HasColumnName("NOMSnumber");
            entity.Property(e => e.Details).HasMaxLength(15);

            entity.HasOne(e => e.PersonalDetail)
                .WithMany(e => e.Flags)
                .HasForeignKey(e => e.NomsNumber);
        });

        modelBuilder.Entity<Identifier>(entity =>
        {
            entity.HasKey(e => e.NomsNumber);

            entity
                .ToTable("Identifiers", "OfflocRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("Identifiers", "OfflocTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.NomsNumber)
                .HasMaxLength(7)
                .HasColumnName("NOMSnumber");
            entity.Property(e => e.Crono)
                .HasMaxLength(35)
                .HasColumnName("CROno");

            entity.HasOne(e => e.PersonalDetail)
                .WithMany(e => e.Identifiers)
                .HasForeignKey(e => e.NomsNumber);
        });

        modelBuilder.Entity<IncentiveLevel>(entity =>
        {
            entity.HasKey(e => e.NomsNumber);

            entity
                .ToTable("IncentiveLevel", "OfflocRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("IncentiveLevel", "OfflocTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.NomsNumber)
                .HasMaxLength(7)
                .HasColumnName("NOMSnumber");
            entity.Property(e => e.IncentiveBand).HasMaxLength(10);

            entity.HasOne(e => e.PersonalDetail)
                .WithMany(e => e.IncentiveLevels)
                .HasForeignKey(e => e.NomsNumber);
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.NomsNumber);

            entity
                .ToTable("Locations", "OfflocRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("Locations", "OfflocTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.NomsNumber)
                .HasMaxLength(7)
                .HasColumnName("NOMSnumber");
            entity.Property(e => e.Location1)
                .HasMaxLength(20)
                .HasColumnName("Location");

            entity.HasOne(e => e.PersonalDetail)
                .WithMany(e => e.Locations)
                .HasForeignKey(e => e.NomsNumber);
        });

        modelBuilder.Entity<MainOffence>(entity =>
        {
            entity.HasKey(e => e.NomsNumber);

            entity
                .ToTable("MainOffence", "OfflocRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("MainOffence", "OfflocTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.NomsNumber)
                .HasMaxLength(7)
                .HasColumnName("NOMSnumber");
            entity.Property(e => e.MainOffence1)
                .HasMaxLength(20)
                .HasColumnName("MainOffence");

            entity.HasOne(e => e.PersonalDetail)
                .WithMany(e => e.MainOffences)
                .HasForeignKey(e => e.NomsNumber);
        });

        modelBuilder.Entity<Movement>(entity =>
        {
            entity.HasKey(e => e.NomsNumber);

            entity
                .ToTable("Movements", "OfflocRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("Movements", "OfflocTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.NomsNumber)
                .HasMaxLength(7)
                .HasColumnName("NOMSnumber");
            entity.Property(e => e.MovementCode).HasMaxLength(10);
            entity.Property(e => e.MovementEstabComponent).HasMaxLength(38);
            entity.Property(e => e.TransferReason).HasMaxLength(35);

            entity.HasOne(e => e.PersonalDetail)
                .WithMany(e => e.Movements)
                .HasForeignKey(e => e.NomsNumber);
        });

        modelBuilder.Entity<OffenderAgency>(entity =>
        {
            entity.HasKey(e => e.NomsNumber);

            entity
                .ToTable("OffenderAgencies", "OfflocRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("OffenderAgencies", "OfflocTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.NomsNumber)
                .HasMaxLength(7)
                .HasColumnName("NOMSnumber");
            entity.Property(e => e.EstablishmentCode).HasMaxLength(5);

            entity.HasOne(e => e.PersonalDetail)
                .WithMany(e => e.OffenderAgencies)
                .HasForeignKey(e => e.NomsNumber);
        });

        modelBuilder.Entity<OffenderStatus>(entity =>
        {
            entity.HasKey(e => e.NomsNumber);

            entity
                .ToTable("OffenderStatus", "OfflocRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("OffenderStatus", "OfflocTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.NomsNumber)
                .HasMaxLength(7)
                .HasColumnName("NOMSnumber");
            entity.Property(e => e.CustodyStatus).HasMaxLength(20);
            entity.Property(e => e.InmateStatus).HasMaxLength(50);

            entity.HasOne(e => e.PersonalDetail)
                .WithMany(e => e.Statuses)
                .HasForeignKey(e => e.NomsNumber);
        });

        modelBuilder.Entity<OtherOffence>(entity =>
        {
            entity.HasKey(e => new { e.NomsNumber, e.Details });

            entity
                .ToTable("OtherOffences", "OfflocRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("OtherOffences", "OfflocTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.NomsNumber)
                .HasMaxLength(7)
                .HasColumnName("NOMSnumber");
            entity.Property(e => e.Details).HasMaxLength(15);

            entity.HasOne(e => e.PersonalDetail)
                .WithMany(e => e.OtherOffences)
                .HasForeignKey(e => e.NomsNumber);
        });

        modelBuilder.Entity<PersonalDetail>(entity =>
        {
            entity.HasKey(e => e.NomsNumber);

            entity
                .ToTable("PersonalDetails", "OfflocRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("PersonalDetails", "OfflocTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.NomsNumber)
                .HasMaxLength(7)
                .HasColumnName("NOMSnumber");
            entity.Property(e => e.DateOfBirth).HasColumnName("DOB");
            entity.Property(e => e.EthnicGroup).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasColumnName("Forename1").HasMaxLength(50);
            entity.Property(e => e.SecondName).HasColumnName("Forename2").HasMaxLength(50);
            entity.Property(e => e.Gender).HasMaxLength(30);
            entity.Property(e => e.MaritalStatus).HasMaxLength(50);
            entity.Property(e => e.MaternityStatus).HasMaxLength(100);
            entity.Property(e => e.Nationality).HasMaxLength(50);
            entity.Property(e => e.Religion).HasMaxLength(50);
            entity.Property(e => e.Surname).HasMaxLength(50);
        });

        modelBuilder.Entity<Pnc>(entity =>
        {
            entity.HasKey(e => new { e.NomsNumber, e.Details });

            entity
                .ToTable("PNC", "OfflocRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("PNC", "OfflocTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.NomsNumber)
                .HasMaxLength(7)
                .HasColumnName("NOMSnumber");
            entity.Property(e => e.Details).HasMaxLength(15);

            entity.HasOne(e => e.PersonalDetail)
                .WithMany(e => e.Pncs)
                .HasForeignKey(e => e.NomsNumber);
        });

        modelBuilder.Entity<PreviousPrisonNumber>(entity =>
        {
            entity.HasKey(e => new { e.NomsNumber, e.Details });

            entity
                .ToTable("PreviousPrisonNumbers", "OfflocRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("PreviousPrisonNumbers", "OfflocTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.NomsNumber)
                .HasMaxLength(7)
                .HasColumnName("NOMSnumber");
            entity.Property(e => e.Details).HasMaxLength(8);

            entity.HasOne(e => e.PersonalDetail)
                .WithMany(e => e.PreviousPrisonNumbers)
                .HasForeignKey(e => e.NomsNumber);
        });

        modelBuilder.Entity<ProcessedFile>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ProcessedFiles", "OfflocRunningPicture");

            entity.Property(e => e.FileName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SentenceInformation>(entity =>
        {
            entity.HasKey(e => e.NomsNumber);

            entity
                .ToTable("SentenceInformation", "OfflocRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("SentenceInformation", "OfflocTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.NomsNumber)
                .HasMaxLength(7)
                .HasColumnName("NOMSnumber");
            entity.Property(e => e.Crd)
                .HasMaxLength(10)
                .HasColumnName("crd");
            entity.Property(e => e.Hdcad)
                .HasMaxLength(10)
                .HasColumnName("hdcad");
            entity.Property(e => e.Hdced)
                .HasMaxLength(10)
                .HasColumnName("hdced");
            entity.Property(e => e.Led)
                .HasMaxLength(10)
                .HasColumnName("led");
            entity.Property(e => e.Npd)
                .HasMaxLength(10)
                .HasColumnName("npd");
            entity.Property(e => e.Ped)
                .HasMaxLength(10)
                .HasColumnName("ped");
            entity.Property(e => e.Sed)
                .HasMaxLength(10)
                .HasColumnName("sed");
            entity.Property(e => e.Tused).HasColumnName("tused");

            entity.HasOne(e => e.PersonalDetail)
                .WithMany(e => e.SentenceInformation)
                .HasForeignKey(e => e.NomsNumber);
        });

        modelBuilder.Entity<SexOffender>(entity =>
        {
            entity.HasKey(e => e.NomsNumber);

            entity
                .ToTable("SexOffenders", "OfflocRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("SexOffenders", "OfflocTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.NomsNumber)
                .HasMaxLength(7)
                .HasColumnName("NOMSnumber");
            entity.Property(e => e.Schedule1Sexoffender).HasMaxLength(50);

            entity.HasOne(e => e.PersonalDetail)
                .WithMany(e => e.SexOffenders)
                .HasForeignKey(e => e.NomsNumber);
        });

        modelBuilder.Entity<StandardisationReference>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("StandardisationReference", "OfflocRunningPicture");

            entity.Property(e => e.CleanedData).HasMaxLength(100);
            entity.Property(e => e.RawData).HasMaxLength(100);
            entity.Property(e => e.Source).HasMaxLength(200);
            entity.Property(e => e.Type).HasMaxLength(100);
        });

        modelBuilder.Entity<VeteranFlagLog>(entity =>
        {
            entity.HasKey(e => e.NomsNumber);

            entity
                .ToTable("VeteranFlagLog", "OfflocRunningPicture")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("VeteranFlagLog", "OfflocTemporal");
                        ttb
                            .HasPeriodStart("ValidFrom")
                            .HasColumnName("ValidFrom");
                        ttb
                            .HasPeriodEnd("ValidTo")
                            .HasColumnName("ValidTo");
                    }));

            entity.Property(e => e.NomsNumber)
                .HasColumnName("NomisNumber")
                .HasMaxLength(7);
                //.IsUnicode(false)
                //.IsFixedLength();

            entity.HasOne(e => e.PersonalDetail)
                .WithMany(e => e.VeteranFlagLogs)
                .HasForeignKey(e => e.NomsNumber);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
