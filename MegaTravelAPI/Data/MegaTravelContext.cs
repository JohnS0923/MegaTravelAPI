using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MegaTravelAPI.Data
{
    public partial class MegaTravelContext : DbContext
    {
        private readonly IConfiguration _config;
        public MegaTravelContext(IConfiguration config)
        {
            _config = config;

        }

        public MegaTravelContext(DbContextOptions<MegaTravelContext> options, IConfiguration config)
            : base(options)
        {
            _config = config;
        }

        public virtual DbSet<Agent> Agents { get; set; } = null!;
        public virtual DbSet<Login> Logins { get; set; } = null!;
        public virtual DbSet<Trip> Trips { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<TripPayment> TripPayment { get; set; } = null!;



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var databaseConnectionString = _config["MegaTravel:DatabaseConnectionString"];
                optionsBuilder.UseSqlServer(databaseConnectionString);

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agent>(entity =>
            {
                entity.ToTable("Agent");

                entity.Property(e => e.AgentId)
                    .ValueGeneratedNever()
                    .HasColumnName("AgentID");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OfficeLocation)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(10)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Login>(entity =>
            {
                entity.ToTable("Login");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Password).IsUnicode(false);

                entity.Property(e => e.UserType)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.LoginInfo)
                    .HasForeignKey<Login>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Login_Agent");

                entity.HasOne(d => d.Id1)
                    .WithOne(p => p.LoginInfo)
                    .HasForeignKey<Login>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Login_User");
            });

            modelBuilder.Entity<Trip>(entity =>
            {
                entity.ToTable("Trip");

                entity.Property(e => e.TripId)
                    .ValueGeneratedNever()
                    .HasColumnName("TripID");

                entity.Property(e => e.AgentId).HasColumnName("AgentID");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Location)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.TripName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Agent)
                    .WithMany(p => p.Trips)
                    .HasForeignKey(d => d.AgentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Trip_Agent");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Trips)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Trip_User");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.Property(e => e.UserId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("UserID");

                entity.Property(e => e.City)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.State)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Street1)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Street2)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TripPayment>(entity =>
            {
                entity.ToTable("TripPayment");

                entity.Property(e => e.PaymentId)
                .ValueGeneratedOnAdd()
                .HasColumnName("paymentID");

                entity.Property(e => e.TripId)
                .HasColumnName("tripID");

                entity.Property(e => e.PaymentStatus)
                .HasColumnName("paymentStatus");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
