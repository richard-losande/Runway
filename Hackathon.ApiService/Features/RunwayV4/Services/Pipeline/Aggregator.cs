using Hackathon.ApiService.Features.RunwayV4.Models;

namespace Hackathon.ApiService.Features.RunwayV4.Services.Pipeline;

public interface IAggregator
{
    AggregationResult Aggregate(List<Transaction> transactions);
}

public class Aggregator : IAggregator
{
    private static readonly Dictionary<CategoryKey, CategoryTier> TierMap = new()
    {
        [CategoryKey.FoodDining] = CategoryTier.Discretionary,
        [CategoryKey.Groceries] = CategoryTier.Essential,
        [CategoryKey.BillsUtilities] = CategoryTier.Essential,
        [CategoryKey.Transport] = CategoryTier.Essential,
        [CategoryKey.Shopping] = CategoryTier.Discretionary,
        [CategoryKey.HealthWellness] = CategoryTier.Essential,
        [CategoryKey.Housing] = CategoryTier.Essential,
        [CategoryKey.Transfers] = CategoryTier.Committed,
        [CategoryKey.EntertainmentSubs] = CategoryTier.Discretionary,
        [CategoryKey.Misc] = CategoryTier.Discretionary,
    };

    public AggregationResult Aggregate(List<Transaction> transactions)
    {
        if (transactions.Count == 0)
        {
            return new AggregationResult();
        }

        // Split credits (income) from debits (expenses)
        var debitTransactions = transactions.Where(t => !t.IsCredit).ToList();
        var creditTransactions = transactions.Where(t => t.IsCredit).ToList();

        // Determine month boundaries
        var months = transactions
            .Select(t => new DateTime(t.Date.Year, t.Date.Month, 1))
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        if (months.Count == 0) months.Add(new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1));

        // Compute monthly credits (income)
        var monthlyCreditAmounts = months.Select(month =>
            creditTransactions
                .Where(t => t.Date.Year == month.Year && t.Date.Month == month.Month)
                .Sum(t => t.Amount)
        ).ToList();
        var monthlyCredits = monthlyCreditAmounts.Count > 0
            ? Math.Round(monthlyCreditAmounts.Average(), 2)
            : 0;

        var categories = new Dictionary<CategoryKey, CategoryBreakdownEntry>();
        var correctionCandidates = new List<CorrectionCandidate>();

        // Only aggregate debit transactions into expense categories
        foreach (CategoryKey catKey in Enum.GetValues<CategoryKey>())
        {
            var catTransactions = debitTransactions
                .Where(t => t.Category == catKey)
                .ToList();

            // Compute monthly amounts
            var monthlyAmounts = new List<decimal>();
            foreach (var month in months)
            {
                var monthTotal = catTransactions
                    .Where(t => t.Date.Year == month.Year && t.Date.Month == month.Month)
                    .Sum(t => t.Amount);
                monthlyAmounts.Add(Math.Round(monthTotal, 2));
            }

            var monthlyAverage = monthlyAmounts.Count > 0
                ? Math.Round(monthlyAmounts.Average(), 2)
                : 0;

            // Top merchants (top 3 by total spend)
            var topMerchants = catTransactions
                .Where(t => !string.IsNullOrEmpty(t.Merchant))
                .GroupBy(t => t.Merchant!)
                .Select(g => new MerchantSummary
                {
                    Name = g.Key,
                    MonthlyAvg = months.Count > 0
                        ? Math.Round(g.Sum(t => t.Amount) / months.Count, 2)
                        : 0,
                })
                .OrderByDescending(m => m.MonthlyAvg)
                .Take(3)
                .ToList();

            categories[catKey] = new CategoryBreakdownEntry
            {
                MonthlyAverage = monthlyAverage,
                MonthlyAmounts = monthlyAmounts,
                Tier = TierMap.GetValueOrDefault(catKey, CategoryTier.Discretionary),
                TopMerchants = topMerchants,
                TransactionCount = catTransactions.Count,
            };
        }

        // Build correction candidates
        foreach (var tx in transactions)
        {
            string? reason = null;

            if (tx.Confidence is double conf && conf < 0.7)
            {
                reason = conf == 0 ? "misc_fallback" : "llm_low_conf";
            }
            else if (tx.Confidence is string s && s != "high")
            {
                reason = "llm";
            }

            // ATM withdrawals always flagged
            if (tx.Merchant == "ATM Withdrawal" || tx.NormDesc.Contains("ATM", StringComparison.OrdinalIgnoreCase))
            {
                reason ??= "atm_withdrawal";
            }

            // Large transfers to individuals
            if (tx.Category == CategoryKey.Transfers && tx.Amount > 5000)
            {
                reason ??= "large_transfer";
            }

            if (reason != null)
            {
                var confidenceScore = tx.Confidence switch
                {
                    double d => (decimal)d,
                    string s when s == "high" => 1.0m,
                    _ => 0.5m,
                };

                correctionCandidates.Add(new CorrectionCandidate
                {
                    Transaction = tx,
                    AssignedCategory = tx.Category ?? CategoryKey.Misc,
                    ConfidenceScore = confidenceScore,
                    Reason = reason,
                });
            }
        }

        // Cap at 15 candidates, sorted by confidence ascending
        correctionCandidates = correctionCandidates
            .OrderBy(c => c.ConfidenceScore)
            .Take(15)
            .ToList();

        var monthlyBurn = categories.Values.Sum(c => c.MonthlyAverage);

        return new AggregationResult
        {
            Categories = categories,
            MonthlyBurn = monthlyBurn,
            MonthlyCredits = monthlyCredits,
            CorrectionCandidates = correctionCandidates,
            AllTransactions = transactions,
        };
    }
}
