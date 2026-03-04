using Refit;

namespace Hackathon.ApiService.Integrations.SproutPayroll;

// ── Sprout API response models (match external API shape) ──

public class SproutPayrollResponse
{
    public SproutPagination Pagination { get; set; } = new();
    public List<SproutPayrollEntry> Data { get; set; } = [];
}

public class SproutPagination
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    public int PageSize { get; set; }
}

public class SproutPayrollEntry
{
    public SproutEmployeeInfo EmployeeInformation { get; set; } = new();
    public List<SproutAdjustment>? Adjustments { get; set; }
    public SproutGovernmentDeductions GovernmentStatutoryDeductions { get; set; } = new();
    public decimal BasicSalary { get; set; }
    public decimal Tax { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal NetAmount { get; set; }
    public int PayrollYear { get; set; }
    public int PayrollMonth { get; set; }
    public int PayrollPeriod { get; set; }
}

public class SproutEmployeeInfo
{
    public string Id { get; set; } = string.Empty;
    public string EmployeeIdNumber { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string? Department { get; set; }
}

public class SproutAdjustment
{
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool Taxable { get; set; }
}

public class SproutGovernmentDeductions
{
    public decimal Sssee { get; set; }
    public decimal Ssser { get; set; }
    public decimal Phee { get; set; }
    public decimal Pher { get; set; }
    public decimal Hdmfee { get; set; }
    public decimal Hdmfer { get; set; }
    public decimal HdmfAdditional { get; set; }
}

// ── Refit interface ──

[Headers("Accept: application/json")]
public interface ISproutPayrollClient
{
    [Get("/api/v1/payrolls/entries/{entryId}/summary")]
    Task<SproutPayrollResponse> GetPayrollSummaryAsync(
        int entryId,
        [Query] string search,
        CancellationToken cancellationToken = default);
}
