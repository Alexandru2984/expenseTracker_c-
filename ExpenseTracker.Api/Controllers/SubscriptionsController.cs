using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.Models;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly AppDbContext _db;

    public SubscriptionsController(AppDbContext db)
    {
        _db = db;
    }

    // GET /api/subscriptions
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.Subscriptions.OrderBy(s => s.Name).ToListAsync();
        return Ok(items);
    }

    // GET /api/subscriptions/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var item = await _db.Subscriptions.FindAsync(id);
        if (item is null) return NotFound();
        return Ok(item);
    }

    // POST /api/subscriptions
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SubscriptionItem input)
    {
        input.Id = Guid.NewGuid();
        _db.Subscriptions.Add(input);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = input.Id }, input);
    }

    // PUT /api/subscriptions/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] SubscriptionItem input)
    {
        var existing = await _db.Subscriptions.FindAsync(id);
        if (existing is null) return NotFound();

        existing.Name = input.Name;
        existing.Cost = input.Cost;
        existing.Currency = input.Currency;
        existing.BillingPeriod = input.BillingPeriod;
        existing.NextBillingDate = input.NextBillingDate;
        existing.Category = input.Category;
        existing.IsActive = input.IsActive;

        await _db.SaveChangesAsync();
        return Ok(existing);
    }

    // DELETE /api/subscriptions/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var item = await _db.Subscriptions.FindAsync(id);
        if (item is null) return NotFound();

        _db.Subscriptions.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // GET /api/subscriptions/summary
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var active = await _db.Subscriptions
            .Where(s => s.IsActive)
            .ToListAsync();

        var totalMonthly = active.Sum(s =>
            s.BillingPeriod == BillingPeriod.Yearly ? s.Cost / 12 : s.Cost);

        var byCurrency = active
            .GroupBy(s => s.Currency)
            .Select(g => new
            {
                Currency = g.Key,
                MonthlyTotal = g.Sum(s =>
                    s.BillingPeriod == BillingPeriod.Yearly ? s.Cost / 12 : s.Cost)
            });

        return Ok(new
        {
            TotalMonthlyEquivalent = Math.Round(totalMonthly, 2),
            ByCurrency = byCurrency,
            ActiveSubscriptions = active.Count,
            TotalSubscriptions = await _db.Subscriptions.CountAsync()
        });
    }
}
