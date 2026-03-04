using Hackathon.ApiService.Features.RunwayV4.Models;

namespace Hackathon.ApiService.Features.RunwayV4.Services.Pipeline;

public interface IFormatDetector
{
    FormatDetectionResult Detect(string[] headerRow);
}

public class FormatDetector : IFormatDetector
{
    private static readonly Dictionary<string, BankSource> BankHints = new(StringComparer.OrdinalIgnoreCase)
    {
        ["GCash"] = BankSource.GCash,
        ["BDO"] = BankSource.BDO,
        ["BPI"] = BankSource.BPI,
        ["UnionBank"] = BankSource.UnionBank,
        ["Maya"] = BankSource.Maya,
        ["RCBC"] = BankSource.RCBC,
    };

    public FormatDetectionResult Detect(string[] headerRow)
    {
        var result = new FormatDetectionResult();
        var headers = headerRow.Select(h => h.Trim()).ToArray();

        // Try to detect bank source from header content
        var joined = string.Join(" ", headers);
        foreach (var (hint, source) in BankHints)
        {
            if (joined.Contains(hint, StringComparison.OrdinalIgnoreCase))
            {
                result.Source = source;
                break;
            }
        }

        // Map columns by header name
        for (int i = 0; i < headers.Length; i++)
        {
            var h = headers[i].ToUpperInvariant();

            if (h.Contains("DATE") || h.Contains("TRANS DATE") || h.Contains("POSTING DATE"))
                result.DateColumn = i;
            else if (h.Contains("DESCRIPTION") || h.Contains("PARTICULARS") || h.Contains("REMARKS") || h.Contains("DETAILS") || h.Contains("MEMO"))
                result.DescriptionColumn = i;
            else if (h == "AMOUNT" || h == "TRANSACTION AMOUNT")
                result.AmountColumn = i;
            else if (h.Contains("DEBIT") || h.Contains("WITHDRAWAL"))
                result.DebitColumn = i;
            else if (h.Contains("CREDIT") || h.Contains("DEPOSIT"))
                result.CreditColumn = i;
        }

        // Determine if separate credit/debit columns
        result.HasSeparateCreditDebit = result.DebitColumn >= 0 && result.CreditColumn >= 0;

        // If no single amount column found but we have credit/debit, that's fine
        if (result.AmountColumn < 0 && !result.HasSeparateCreditDebit)
        {
            // Fallback: look for any numeric-looking column
            for (int i = 0; i < headers.Length; i++)
            {
                var h = headers[i].ToUpperInvariant();
                if (h.Contains("AMT") || h.Contains("TOTAL") || h.Contains("VALUE"))
                {
                    result.AmountColumn = i;
                    break;
                }
            }
        }

        // Default date format
        result.DateFormat = "MM/dd/yyyy";

        // Bank-specific overrides
        switch (result.Source)
        {
            case BankSource.BDO:
                result.DateFormat = "MM/dd/yyyy";
                break;
            case BankSource.BPI:
                result.DateFormat = "yyyy-MM-dd";
                break;
            case BankSource.GCash:
                result.DateFormat = "MM/dd/yyyy HH:mm";
                break;
        }

        return result;
    }
}
