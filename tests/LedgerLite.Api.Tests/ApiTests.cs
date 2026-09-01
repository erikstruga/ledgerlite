using System.Net;
using System.Net.Http.Json;
using LedgerLite.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace LedgerLite.Api.Tests;

public class ApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    public ApiTests(WebApplicationFactory<Program> factory) => _client = factory.CreateClient();

    [Fact] public async Task Health_ReturnsOk() => Assert.Equal(HttpStatusCode.OK, (await _client.GetAsync("/health")).StatusCode);

    [Fact] public async Task Invoices_ReturnSeededData()
    {
        var invoices = await _client.GetFromJsonAsync<List<Invoice>>("/api/invoices");
        Assert.NotNull(invoices); Assert.NotEmpty(invoices); Assert.Contains(invoices, x => x.Number == "INV-1004");
    }

    [Fact] public async Task UnbalancedJournalEntry_IsRejected()
    {
        var entry = new JournalEntry { EntryDate = new(2026, 8, 1), Memo = "Bad entry", Lines = [new() { AccountId = 1, Debit = 100 }, new() { AccountId = 2, Credit = 90 }] };
        Assert.Equal(HttpStatusCode.BadRequest, (await _client.PostAsJsonAsync("/api/journal-entries", entry)).StatusCode);
    }

    [Fact] public async Task ProfitAndLoss_ComputesNetIncome()
    {
        var report = await _client.GetFromJsonAsync<ProfitAndLossReport>("/api/reports/profit-and-loss?from=2026-01-01&to=2026-12-31");
        Assert.NotNull(report); Assert.Equal(4905m, report.NetIncome);
    }
}
