using Hackathon.ApiService.Features.RunwayV4.Models;

namespace Hackathon.ApiService.Features.RunwayV4.Services.Pipeline;

public class FormatDetectionResult
{
    public BankSource Source { get; set; } = BankSource.Unknown;
    public int DateColumn { get; set; } = -1;
    public int AmountColumn { get; set; } = -1;
    public int CreditColumn { get; set; } = -1;
    public int DebitColumn { get; set; } = -1;
    public int DescriptionColumn { get; set; } = -1;
    public string DateFormat { get; set; } = string.Empty;
    public bool HasSeparateCreditDebit { get; set; }
    public int HeaderRowIndex { get; set; }
}

public class MerchantMatch
{
    public CategoryKey Category { get; set; }
    public string MerchantName { get; set; } = string.Empty;
    public string Confidence { get; set; } = "high";
}

public class AggregationResult
{
    public Dictionary<CategoryKey, CategoryBreakdownEntry> Categories { get; set; } = new();
    public decimal MonthlyBurn { get; set; }
    public decimal MonthlyCredits { get; set; }
    public List<CorrectionCandidate> CorrectionCandidates { get; set; } = new();
    public List<Transaction> AllTransactions { get; set; } = new();
}
