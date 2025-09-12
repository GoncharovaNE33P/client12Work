using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace client.Models;

public partial class PostgresContext : DbContext
{
    public PostgresContext()
    {
    }

    public PostgresContext(DbContextOptions<PostgresContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Hotel> Hotels { get; set; }

    public virtual DbSet<Tour> Tours { get; set; }

    public virtual DbSet<ToursType> ToursTypes { get; set; }

    public virtual DbSet<Type> Types { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source CountryCode. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=123456");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.CountryCode).HasName("country_pk");

            entity.ToTable("country");

            entity.Property(e => e.CountryCode).HasColumnName("country_code");
            entity.Property(e => e.NameCountry).HasColumnName("name_country");
        });

        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.HasKey(e => e.IdHotel).HasName("hotel_pk");

            entity.ToTable("hotel");

            entity.Property(e => e.IdHotel).HasColumnName("id_hotel");
            entity.Property(e => e.CountOfStars).HasColumnName("count_of_stars");
            entity.Property(e => e.CountryCode).HasColumnName("country_code");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.NameHotel).HasColumnName("name_hotel");

            entity.HasOne(d => d.CountryCodeNavigation).WithMany(p => p.Hotels)
                .HasForeignKey(d => d.CountryCode)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("hotel_country_fk");
        });

        modelBuilder.Entity<Tour>(entity =>
        {
            entity.HasKey(e => e.IdTour).HasName("tour_pk");

            entity.ToTable("tour");

            entity.Property(e => e.IdTour).HasColumnName("id_tour");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ImagePreview).HasColumnName("image_preview");
            entity.Property(e => e.IsActual).HasColumnName("is_actual");
            entity.Property(e => e.NameTour).HasColumnName("name_tour");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.TicketCount).HasColumnName("ticket_count");
        });

        modelBuilder.Entity<ToursType>(entity =>
        {
            entity.HasKey(e => e.IdToursType).HasName("tours_type_pk");

            entity.ToTable("tours_type");

            entity.Property(e => e.IdToursType).HasColumnName("id_tours_type");
            entity.Property(e => e.IdTour)
                .ValueGeneratedOnAdd()
                .HasColumnName("id_tour");
            entity.Property(e => e.IdType)
                .ValueGeneratedOnAdd()
                .HasColumnName("id_type");

            entity.HasOne(d => d.IdTourNavigation).WithMany(p => p.ToursTypes)
                .HasForeignKey(d => d.IdTour)
                .HasConstraintName("tours_type_tour_fk");

            entity.HasOne(d => d.IdTypeNavigation).WithMany(p => p.ToursTypes)
                .HasForeignKey(d => d.IdType)
                .HasConstraintName("tours_type_type_fk");
        });

        modelBuilder.Entity<Type>(entity =>
        {
            entity.HasKey(e => e.IdType).HasName("type_pk");

            entity.ToTable("type");

            entity.Property(e => e.IdType).HasColumnName("id_type");
            entity.Property(e => e.NameType).HasColumnName("name_type");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
