using FastEndpoints;
using Hackathon.Bff.Integrations.SproutPayroll;
using System.Globalization;

namespace Hackathon.Bff.Features.Payroll;

public class PayrollLineItem
{
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class PayrollSummaryResponse
{
    public decimal GrossPay { get; set; }
    public decimal NetPay { get; set; }
    public decimal Tax { get; set; }
    public List<PayrollLineItem> Deductions { get; set; } = [];
    public List<PayrollLineItem> Earnings { get; set; } = [];
    public string EmployeeName { get; set; } = string.Empty;
    public string PayrollPeriod { get; set; } = string.Empty;
}

public class SummaryEndpoint : EndpointWithoutRequest<PayrollSummaryResponse>
{
    private readonly ISproutPayrollClient _sproutClient;

    public SummaryEndpoint(ISproutPayrollClient sproutClient)
    {
        _sproutClient = sproutClient;
    }

    public override void Configure()
    {
        Get("/api/v1/payroll/summary");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var sproutResponse = await _sproutClient.GetPayrollSummaryAsync(
            1002, "LM_Feaure_TestData", ct);

        var entry = sproutResponse.Data.FirstOrDefault();
        if (entry is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var deductions = new List<PayrollLineItem>
        {
            new() { Name = "SSS", Amount = entry.GovernmentStatutoryDeductions.Sssee },
            new() { Name = "PhilHealth", Amount = entry.GovernmentStatutoryDeductions.Phee },
            new() { Name = "Pag-IBIG", Amount = entry.GovernmentStatutoryDeductions.Hdmfee },
            new() { Name = "Withholding Tax", Amount = entry.Tax },
        };

        var earnings = new List<PayrollLineItem>();

        if (entry.Adjustments is not null)
        {
            foreach (var adj in entry.Adjustments)
            {
                if (adj.Amount < 0)
                    deductions.Add(new PayrollLineItem { Name = adj.Name, Amount = Math.Abs(adj.Amount) });
                else if (adj.Amount > 0)
                    earnings.Add(new PayrollLineItem { Name = adj.Name, Amount = adj.Amount });
            }
        }

        var monthName = CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(entry.PayrollMonth);

        await Send.OkAsync(new PayrollSummaryResponse
        {
            GrossPay = entry.BasicSalary,
            NetPay = entry.NetAmount,
            Tax = entry.Tax,
            Deductions = deductions,
            Earnings = earnings,
            EmployeeName = $"{entry.EmployeeInformation.FirstName} {entry.EmployeeInformation.LastName}".Trim(),
            PayrollPeriod = $"{monthName} {entry.PayrollYear} - Period {entry.PayrollPeriod}",
        }, cancellation: ct);
    }
}
