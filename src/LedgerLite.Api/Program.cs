using System.Text.Json.Serialization;
using LedgerLite.Api.Data;
using LedgerLite.Api.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlite(builder.Configuration.GetConnectionString("Ledger") ?? "Data Source=ledgerlite.db"));
builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope()) SeedData.Initialize(scope.ServiceProvider.GetRequiredService<AppDbContext>());

app.MapGet("/api/invoices", async (AppDbContext db) => await db.Invoices.OrderByDescending(x => x.IssueDate).ToListAsync());
app.MapPost("/api/invoices", async (Invoice invoice, AppDbContext db) => {
    db.Invoices.Add(invoice); await db.SaveChangesAsync(); return Results.Created($"/api/invoices/{invoice.Id}", invoice);
});
app.MapPatch("/api/invoices/{id:int}/status", async (int id, InvoiceStatus status, AppDbContext db) => {
    var invoice = await db.Invoices.FindAsync(id); if (invoice is null) return Results.NotFound();
    invoice.Status = status; await db.SaveChangesAsync(); return Results.Ok(invoice);
});

app.MapGet("/api/journal-entries", async (AppDbContext db) => await db.JournalEntries.Include(x => x.Lines).ThenInclude(x => x.Account).OrderByDescending(x => x.EntryDate).ToListAsync());
app.MapPost("/api/journal-entries", async (JournalEntry entry, AppDbContext db) => {
    if (entry.Lines.Count < 2 || entry.Lines.Sum(x => x.Debit) != entry.Lines.Sum(x => x.Credit))
        return Results.ValidationProblem(new Dictionary<string, string[]> { ["lines"] = ["Journal entries must contain at least two lines and balance."] });
    db.JournalEntries.Add(entry); await db.SaveChangesAsync(); return Results.Created($"/api/journal-entries/{entry.Id}", entry);
});

app.MapGet("/api/reports/profit-and-loss", async (DateOnly? from, DateOnly? to, AppDbContext db) => {
    var start = from ?? new DateOnly(DateTime.Today.Year, 1, 1); var end = to ?? DateOnly.FromDateTime(DateTime.Today);
    var reportLines = await db.JournalLines
        .Where(x => x.JournalEntry!.EntryDate >= start && x.JournalEntry.EntryDate <= end && (x.Account!.Type == "Revenue" || x.Account.Type == "Expense"))
        .Select(x => new { x.Account!.Name, x.Account.Type, x.Debit, x.Credit })
        .ToListAsync();
    var rows = reportLines.GroupBy(x => new { x.Name, x.Type })
        .Select(g => new { g.Key.Name, g.Key.Type, Amount = g.Sum(x => x.Type == "Revenue" ? x.Credit - x.Debit : x.Debit - x.Credit) })
        .ToList();
    var revenue = rows.Where(x => x.Type == "Revenue").Select(x => new ProfitAndLossLine(x.Name, x.Amount)).ToList();
    var expenses = rows.Where(x => x.Type == "Expense").Select(x => new ProfitAndLossLine(x.Name, x.Amount)).ToList();
    var totalRevenue = revenue.Sum(x => x.Amount); var totalExpenses = expenses.Sum(x => x.Amount);
    return new ProfitAndLossReport(start, end, revenue, expenses, totalRevenue, totalExpenses, totalRevenue - totalExpenses);
});

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.Run();
public partial class Program { }
