using System.Globalization;
using System.Text.RegularExpressions;
using Hackathon.ApiService.Features.RunwayV4.Models;

namespace Hackathon.ApiService.Features.RunwayV4.Services.Pipeline;

public interface ITransactionNormalizer
{
    List<Transaction> Normalize(List<string[]> rows, FormatDetectionResult format);
}

public partial class TransactionNormalizer : ITransactionNormalizer
{
    [GeneratedRegex(@"[^A-Z0-9\s*&]", RegexOptions.Compiled)]
    private static partial Regex SpecialCharsRegex();

    [GeneratedRegex(@"\s{2,}", RegexOptions.Compiled)]
    private static partial Regex MultiSpaceRegex();

    [GeneratedRegex(@"\b(REFUND|REVERSAL|RETURN|REF)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex RefundRegex();

    [GeneratedRegex(@"\b(SALARY|PAYROLL)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex SalaryRegex();

    [GeneratedRegex(@"\b(LOAN PAYMENT|CREDIT CARD|BILL PAYMENT)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex LoanPaymentRegex();

    [GeneratedRegex(@"\b(ATM|WITHDRAWAL)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex AtmRegex();

    private static readonly string[] DateFormats =
    [
        "MM/dd/yyyy", "MM-dd-yyyy", "MM/dd/yyyy HH:mm", "yyyy-MM-dd",
        "yyyy-MM-dd HH:mm:ss", "dd/MM/yyyy", "MM/dd/yy", "M/d/yyyy",
        "M/d/yy", "M-d-yyyy"
    ];

    public List<Transaction> Normalize(List<string[]> rows, FormatDetectionResult format)
    {
        var transactions = new List<Transaction>();
        int txCounter = 0;

        foreach (var row in rows)
        {
            if (row.Length <= Math.Max(format.DescriptionColumn, Math.Max(format.DateColumn, 0)))
                continue;

            var rawDesc = SafeGet(row, format.DescriptionColumn);
            if (string.IsNullOrWhiteSpace(rawDesc)) continue;

            // Parse amount
            decimal amount = 0;
            bool isCredit = false;

            if (format.HasSeparateCreditDebit)
            {
                var debitStr = SafeGet(row, format.DebitColumn);
                var creditStr = SafeGet(row, format.CreditColumn);
                decimal debit = ParseAmount(debitStr);
                decimal credit = ParseAmount(creditStr);

                if (debit > 0)
                {
                    amount = debit;
                }
                else if (credit > 0)
                {
                    amount = credit;
                    isCredit = true;
                }
                else
                {
                    continue; // No amount
                }
            }
            else
            {
                var amountStr = SafeGet(row, format.AmountColumn);
                amount = ParseAmount(amountStr);
                if (amount == 0) continue;

                // Negative amounts = debit (outflow), positive = credit in some formats
                if (amount < 0)
                {
                    amount = Math.Abs(amount);
                }
                else if (format.Source == BankSource.BDO || format.Source == BankSource.Unknown)
                {
                    // BDO: positive amounts in the Amount column are debits
                }
                else
                {
                    isCredit = true;
                }
            }

            var normDesc = NormalizeDescription(rawDesc);

            // Exclusion: refunds/reversals
            if (RefundRegex().IsMatch(normDesc)) continue;

            // Parse date
            DateTime date = DateTime.UtcNow;
            var dateStr = SafeGet(row, format.DateColumn);
            if (!string.IsNullOrWhiteSpace(dateStr))
            {
                if (DateTime.TryParseExact(dateStr.Trim(), DateFormats,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
                {
                    date = parsed;
                }
                else if (DateTime.TryParse(dateStr.Trim(), CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var parsed2))
                {
                    date = parsed2;
                }
            }

            txCounter++;
            var tx = new Transaction
            {
                Id = $"tx_{txCounter:D4}",
                Date = date,
                Amount = amount,
                RawDesc = rawDesc,
                NormDesc = normDesc,
                Source = format.Source,
                IsCredit = isCredit,
            };

            // Special handling: ATM → Misc
            if (AtmRegex().IsMatch(normDesc))
            {
                tx.Category = CategoryKey.Misc;
                tx.Merchant = "ATM Withdrawal";
                tx.Confidence = "high";
            }

            // Special handling: Loan/bill payments → BillsUtilities
            if (LoanPaymentRegex().IsMatch(normDesc))
            {
                tx.Category = CategoryKey.BillsUtilities;
                tx.Merchant = "Loan/Bill Payment";
                tx.Confidence = "high";
            }

            transactions.Add(tx);
        }

        return transactions;
    }

    public static string NormalizeDescription(string raw)
    {
        var upper = raw.ToUpperInvariant();
        upper = SpecialCharsRegex().Replace(upper, " ");
        upper = MultiSpaceRegex().Replace(upper, " ");
        return upper.Trim();
    }

    private static decimal ParseAmount(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return 0;

        // Remove currency symbols, commas, parentheses
        var cleaned = value.Trim()
            .Replace("₱", "").Replace("PHP", "").Replace("P", "")
            .Replace(",", "").Replace("(", "-").Replace(")", "")
            .Trim();

        if (decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            return result;

        return 0;
    }

    private static string SafeGet(string[] row, int index)
    {
        if (index < 0 || index >= row.Length) return string.Empty;
        return row[index]?.Trim() ?? string.Empty;
    }

    public static List<string[]> ParseCsvLines(string csvContent)
    {
        var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var result = new List<string[]>();

        foreach (var line in lines)
        {
            result.Add(ParseCsvLine(line.TrimEnd('\r')));
        }

        return result;
    }

    private static string[] ParseCsvLine(string line)
    {
        var fields = new List<string>();
        bool inQuotes = false;
        var current = new System.Text.StringBuilder();

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++; // Skip escaped quote
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(current.ToString().Trim());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        fields.Add(current.ToString().Trim());
        return fields.ToArray();
    }
}
