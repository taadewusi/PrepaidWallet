using Microsoft.EntityFrameworkCore;
using PrepaidWallet.Models;

namespace PrepaidWallet.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<PrepaidUser> PrepaidUsers => Set<PrepaidUser>();
    public DbSet<BalanceTransaction> BalanceTransactions => Set<BalanceTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── PrepaidUser ────────────────────────────────────────────────────
        modelBuilder.Entity<PrepaidUser>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.FullName)
                  .IsRequired()
                  .HasMaxLength(150);

            entity.Property(e => e.PhoneNumber)
                  .IsRequired()
                  .HasMaxLength(20);

            entity.HasIndex(e => e.PhoneNumber)
                  .IsUnique();

            entity.Property(e => e.Email)
                  .HasMaxLength(150);

            entity.HasIndex(e => e.Email)
                  .IsUnique()
                  .HasFilter("[Email] IS NOT NULL AND [Email] <> ''");

            entity.Property(e => e.Balance)
                  .HasColumnType("decimal(18,2)")
                  .HasDefaultValue(0.00m);

            entity.Property(e => e.IsActive)
                  .HasDefaultValue(true);

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("GETUTCDATE()");

            entity.ToTable("PrepaidUsers");
        });

        // ── BalanceTransaction ────────────────────────────────────────────
        modelBuilder.Entity<BalanceTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Amount)
                  .HasColumnType("decimal(18,2)");

            entity.Property(e => e.BalanceBefore)
                  .HasColumnType("decimal(18,2)");

            entity.Property(e => e.BalanceAfter)
                  .HasColumnType("decimal(18,2)");

            entity.Property(e => e.OperatorName)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(e => e.Remark)
                  .HasMaxLength(500);

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(e => e.PrepaidUser)
                  .WithMany()
                  .HasForeignKey(e => e.PrepaidUserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.ToTable("BalanceTransactions");
        });

        // ── Seed data ─────────────────────────────────────────────────────
        modelBuilder.Entity<PrepaidUser>().HasData(
            new PrepaidUser
            {
                Id = 1,
                FullName = "Adaeze Okonkwo",
                PhoneNumber = "08012345678",
                Email = "adaeze.okonkwo@email.com",
                Balance = 5000.00m,
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new PrepaidUser
            {
                Id = 2,
                FullName = "Emeka Chukwu",
                PhoneNumber = "08098765432",
                Email = "emeka.chukwu@email.com",
                Balance = 12500.00m,
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new PrepaidUser
            {
                Id = 3,
                FullName = "Fatima Bello",
                PhoneNumber = "07031122334",
                Email = "fatima.bello@email.com",
                Balance = 750.50m,
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
