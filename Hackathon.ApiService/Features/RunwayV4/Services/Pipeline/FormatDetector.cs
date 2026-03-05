using Hackathon.ApiService.Features.RunwayV4.Models;

namespace Hackathon.ApiService.Features.RunwayV4.Services.Pipeline;

public interface IFormatDetector
{
    FormatDetectionResult Detect(string[] headerColumns);
}

public class FormatDetector : IFormatDetector
{
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

        // Check for separate Credit/Debit columns first
        int creditIdx = Array.FindIndex(cols, c => c.Contains("CREDIT") && !c.Contains("DEBIT"));
        int debitIdx = Array.FindIndex(cols, c => c.Contains("DEBIT"));
        if (creditIdx >= 0 && debitIdx >= 0)
        {
            result.HasSeparateCreditDebit = true;
            result.CreditColumn = creditIdx;
            result.DebitColumn = debitIdx;
        }
        else
        {
            // Fallback: single amount column
            result.AmountColumn = Array.FindIndex(cols, c =>
                c.Contains("AMOUNT") || c.Contains("DEBIT") || c.Contains("WITHDRAWAL"));
            if (result.AmountColumn < 0)
                result.AmountColumn = cols.Length > 2 ? 2 : cols.Length - 1;
        }

        // Heuristic: description column
        var usedColumns = new HashSet<int> { result.DateColumn };
        if (result.HasSeparateCreditDebit)
        {
            usedColumns.Add(result.CreditColumn);
            usedColumns.Add(result.DebitColumn);
        }
        else
        {
            usedColumns.Add(result.AmountColumn);
        }
        // Exclude balance and reference columns
        int balanceIdx = Array.FindIndex(cols, c => c.Contains("BALANCE"));
        if (balanceIdx >= 0) usedColumns.Add(balanceIdx);
        int refIdx = Array.FindIndex(cols, c => c.Contains("REFERENCE") || c.Contains("REF NO"));
        if (refIdx >= 0) usedColumns.Add(refIdx);

        // Prefer a column explicitly named DESCRIPTION or REMARKS
        var descIdx = Array.FindIndex(cols, c => c == "DESCRIPTION" || c.Contains("REMARKS"));
        if (descIdx >= 0 && !usedColumns.Contains(descIdx))
        {
            result.DescriptionColumn = descIdx;
        }
        else
        {
            // Fallback: longest remaining header name
            result.DescriptionColumn = Enumerable.Range(0, cols.Length)
                .Where(i => !usedColumns.Contains(i))
                .OrderByDescending(i => cols[i].Length)
                .FirstOrDefault();
        }

        result.DateFormat = "MM/dd/yyyy";
        return result;
    }
}
