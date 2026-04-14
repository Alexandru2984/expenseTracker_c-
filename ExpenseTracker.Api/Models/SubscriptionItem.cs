namespace ExpenseTracker.Api.Models;

public enum BillingPeriod
{
    Monthly,
    Yearly
}

public class SubscriptionItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public decimal Cost { get; set; }

    public string Currency { get; set; } = "RON";

    public BillingPeriod BillingPeriod { get; set; } = BillingPeriod.Monthly;

    public DateTime NextBillingDate { get; set; }

    public string Category { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
