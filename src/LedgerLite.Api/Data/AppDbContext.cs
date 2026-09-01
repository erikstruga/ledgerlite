using LedgerLite.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LedgerLite.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<JournalLine> JournalLines => Set<JournalLine>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Invoice>().HasIndex(x => x.Number).IsUnique();
        builder.Entity<Account>().HasIndex(x => x.Code).IsUnique();
        builder.Entity<Invoice>().Property(x => x.Amount).HasPrecision(18, 2);
        builder.Entity<JournalLine>().Property(x => x.Debit).HasPrecision(18, 2);
        builder.Entity<JournalLine>().Property(x => x.Credit).HasPrecision(18, 2);
    }
}

