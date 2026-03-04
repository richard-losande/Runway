using Hackathon.ApiService.Features.RunwayV4.Models;

namespace Hackathon.ApiService.Features.RunwayV4.Services.Pipeline;

public interface IFormatDetector
{
<<<<<<< Updated upstream
    FormatDetectionResult Detect(string[] headerRow);
=======
    FormatDetectionResult Detect(string[] headerColumns);
>>>>>>> Stashed changes
}

public class FormatDetector : IFormatDetector
{
<<<<<<< Updated upstream
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

=======
    public FormatDetectionResult Detect(string[] headerColumns)
    {
        var upper = headerColumns.Select(h => h.Trim().ToUpperInvariant()).ToArray();

        if (TryDetectGCash(upper, out var result)) return result;
        if (TryDetectBDO(upper, out result)) return result;
        if (TryDetectBPI(upper, out result)) return result;

        return DetectUnknown(upper);
    }

    private static bool TryDetectGCash(string[] cols, out FormatDetectionResult result)
    {
        result = new FormatDetectionResult();
        int refIdx = Array.FindIndex(cols, c => c.Contains("REFERENCE NO"));
        int remarksIdx = Array.FindIndex(cols, c => c.Contains("REMARKS"));
        if (refIdx < 0 || remarksIdx < 0) return false;

        result.Source = BankSource.GCash;
        result.DescriptionColumn = remarksIdx;
        result.DateColumn = Array.FindIndex(cols, c => c.Contains("DATE"));
        result.DateFormat = "MM/dd/yyyy";

        int creditIdx = Array.FindIndex(cols, c => c.Contains("CREDIT"));
        int debitIdx = Array.FindIndex(cols, c => c.Contains("DEBIT"));
        if (creditIdx >= 0 && debitIdx >= 0)
        {
            result.HasSeparateCreditDebit = true;
            result.CreditColumn = creditIdx;
            result.DebitColumn = debitIdx;
        }
        else
        {
            result.AmountColumn = Array.FindIndex(cols, c => c.Contains("AMOUNT"));
        }
        return true;
    }

    private static bool TryDetectBDO(string[] cols, out FormatDetectionResult result)
    {
        result = new FormatDetectionResult();
        int detailsIdx = Array.FindIndex(cols, c => c.Contains("TRANSACTION DETAILS"));
        int balanceIdx = Array.FindIndex(cols, c => c.Contains("RUNNING BALANCE"));
        if (detailsIdx < 0 || balanceIdx < 0) return false;

        result.Source = BankSource.BDO;
        result.DescriptionColumn = detailsIdx;
        result.DateColumn = Array.FindIndex(cols, c => c.Contains("DATE") || c.Contains("POSTING"));
        result.AmountColumn = Array.FindIndex(cols, c => c.Contains("AMOUNT"));
        result.DateFormat = "MM/dd/yyyy";
        return true;
    }

    private static bool TryDetectBPI(string[] cols, out FormatDetectionResult result)
    {
        result = new FormatDetectionResult();
        int refIdx = Array.FindIndex(cols, c => c.Contains("REFERENCE NUMBER"));
        int descIdx = Array.FindIndex(cols, c => c == "DESCRIPTION" || c.Contains("DESCRIPTION"));
        if (refIdx < 0 || descIdx < 0) return false;

        result.Source = BankSource.BPI;
        result.DescriptionColumn = descIdx;
        result.DateColumn = Array.FindIndex(cols, c => c.Contains("DATE"));
        result.DateFormat = "MM-dd-yyyy";

        int creditIdx = Array.FindIndex(cols, c => c.Contains("CREDIT AMOUNT"));
        int debitIdx = Array.FindIndex(cols, c => c.Contains("DEBIT AMOUNT"));
        if (creditIdx >= 0 && debitIdx >= 0)
        {
            result.HasSeparateCreditDebit = true;
            result.CreditColumn = creditIdx;
            result.DebitColumn = debitIdx;
        }
        else
        {
            result.AmountColumn = Array.FindIndex(cols, c => c.Contains("AMOUNT"));
        }
        return true;
    }

    private static FormatDetectionResult DetectUnknown(string[] cols)
    {
        var result = new FormatDetectionResult { Source = BankSource.Unknown };

        // Heuristic: date column = first column containing "date"
        result.DateColumn = Array.FindIndex(cols, c => c.Contains("DATE"));
        if (result.DateColumn < 0) result.DateColumn = 0;

        // Heuristic: amount = column containing "amount" or "debit"
        result.AmountColumn = Array.FindIndex(cols, c =>
            c.Contains("AMOUNT") || c.Contains("DEBIT") || c.Contains("WITHDRAWAL"));
        if (result.AmountColumn < 0)
            result.AmountColumn = cols.Length > 2 ? 2 : cols.Length - 1;

        // Heuristic: description = longest header name column
        result.DescriptionColumn = Enumerable.Range(0, cols.Length)
            .Where(i => i != result.DateColumn && i != result.AmountColumn)
            .OrderByDescending(i => cols[i].Length)
            .FirstOrDefault();

        result.DateFormat = "MM/dd/yyyy";
>>>>>>> Stashed changes
        return result;
    }
}
