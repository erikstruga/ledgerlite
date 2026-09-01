using System.ComponentModel.DataAnnotations;

namespace LedgerLite.Api.Models;

public enum InvoiceStatus { Draft, Sent, Paid, Overdue }

public class Invoice
{
    public int Id { get; set; }
    [Required, MaxLength(120)] public string Customer { get; set; } = "";
    [Required, MaxLength(30)] public string Number { get; set; } = "";
    public DateOnly IssueDate { get; set; }
    public DateOnly DueDate { get; set; }
    [Range(0.01, 1_000_000)] public decimal Amount { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
}

public class Account
{
    public int Id { get; set; }
    [Required, MaxLength(10)] public string Code { get; set; } = "";
    [Required, MaxLength(80)] public string Name { get; set; } = "";
    [Required, MaxLength(20)] public string Type { get; set; } = "";
}

public class JournalEntry
{
    public int Id { get; set; }
    public DateOnly EntryDate { get; set; }
    [Required, MaxLength(200)] public string Memo { get; set; } = "";
    public List<JournalLine> Lines { get; set; } = [];
}

public class JournalLine
{
    public int Id { get; set; }
    public int JournalEntryId { get; set; }
    public JournalEntry? JournalEntry { get; set; }
    public int AccountId { get; set; }
    public Account? Account { get; set; }
    [Range(0, 1_000_000)] public decimal Debit { get; set; }
    [Range(0, 1_000_000)] public decimal Credit { get; set; }
}

public record ProfitAndLossLine(string Account, decimal Amount);
public record ProfitAndLossReport(DateOnly From, DateOnly To, IReadOnlyList<ProfitAndLossLine> Revenue,
    IReadOnlyList<ProfitAndLossLine> Expenses, decimal TotalRevenue, decimal TotalExpenses, decimal NetIncome);
