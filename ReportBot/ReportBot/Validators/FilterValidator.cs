using FluentValidation;
using ReportBot.Common.Requests;

namespace ReportBot.Validators;

public class FilterValidator : AbstractValidator<FilterRequest>
{
    public FilterValidator()
    {
        RuleFor(x => x.ToDate)
            .GreaterThan(x => x.FromDate)
            .WithMessage("To Date must be greater than From Date");
    }
}
