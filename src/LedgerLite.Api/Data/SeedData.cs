using LedgerLite.Api.Models;

namespace LedgerLite.Api.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext db)
    {
        db.Database.EnsureCreated();
        if (db.Accounts.Any()) return;

        var accounts = new[] {
            new Account { Code = "1000", Name = "Cash", Type = "Asset" },
            new Account { Code = "1100", Name = "Accounts Receivable", Type = "Asset" },
            new Account { Code = "4000", Name = "Consulting Revenue", Type = "Revenue" },
            new Account { Code = "5000", Name = "Software Expense", Type = "Expense" },
            new Account { Code = "5100", Name = "Office Expense", Type = "Expense" }
        };
        db.Accounts.AddRange(accounts);
        db.Invoices.AddRange(
            new Invoice { Customer = "Northstar Studio", Number = "INV-1004", IssueDate = new(2026, 8, 3), DueDate = new(2026, 9, 2), Amount = 4200, Status = InvoiceStatus.Sent },
            new Invoice { Customer = "Maple & Co.", Number = "INV-1003", IssueDate = new(2026, 7, 12), DueDate = new(2026, 8, 11), Amount = 2850, Status = InvoiceStatus.Overdue },
            new Invoice { Customer = "Aster Labs", Number = "INV-1002", IssueDate = new(2026, 7, 1), DueDate = new(2026, 7, 31), Amount = 5600, Status = InvoiceStatus.Paid }
        );
        db.SaveChanges();

        db.JournalEntries.AddRange(
            Entry(new(2026, 7, 1), "Aster Labs project billed", accounts[1], accounts[2], 5600),
            Entry(new(2026, 7, 31), "Aster Labs payment received", accounts[0], accounts[1], 5600),
            Entry(new(2026, 8, 5), "Design software subscription", accounts[3], accounts[0], 480),
            Entry(new(2026, 8, 18), "Office supplies", accounts[4], accounts[0], 215)
        );
        db.SaveChanges();
    }

    private static JournalEntry Entry(DateOnly date, string memo, Account debit, Account credit, decimal amount) =>
        new() { EntryDate = date, Memo = memo, Lines = [new() { Account = debit, Debit = amount }, new() { Account = credit, Credit = amount }] };
}

