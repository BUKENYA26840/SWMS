using Microsoft.EntityFrameworkCore;
using Practical_Assignment.Models;

namespace Practical_Assignment.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Wallet)
                .WithOne(w => w.Student)
                .HasForeignKey<Wallet>(w => w.StudentId);

            modelBuilder.Entity<Wallet>()
                .HasMany(w => w.Transactions)
                .WithOne(t => t.Wallet)
                .HasForeignKey(t => t.WalletId);

            // Seed initial data
            var student1 = new Student { StudentId = "STU001", FullName = "Alice Mutoni", PIN = "1234", IsLocked = false, FailedAttempts = 0 };
            var student2 = new Student { StudentId = "STU002", FullName = "Bob Nkurunziza", PIN = "5678", IsLocked = false, FailedAttempts = 0 };
            var student3 = new Student { StudentId = "STU003", FullName = "Claire Uwase", PIN = "9999", IsLocked = false, FailedAttempts = 0 };

            modelBuilder.Entity<Student>().HasData(student1, student2, student3);

            var wallet1 = new Wallet { WalletId = Guid.Parse("11111111-1111-1111-1111-111111111111"), StudentId = "STU001", Balance = 50000, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) };
            var wallet2 = new Wallet { WalletId = Guid.Parse("22222222-2222-2222-2222-222222222222"), StudentId = "STU002", Balance = 30000, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) };
            var wallet3 = new Wallet { WalletId = Guid.Parse("33333333-3333-3333-3333-333333333333"), StudentId = "STU003", Balance = 75000, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) };

            modelBuilder.Entity<Wallet>().HasData(wallet1, wallet2, wallet3);
        }
    }
}
