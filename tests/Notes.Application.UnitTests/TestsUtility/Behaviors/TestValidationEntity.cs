using FluentValidation;
using MediatR;

namespace Notes.Application.UnitTests.TestsUtility.Behaviors;

public record TestValidationEntity(string Test) : IRequest<Unit>;

public class TestValidationEntityValidator : AbstractValidator<TestValidationEntity>
{
    public TestValidationEntityValidator()
    {
        RuleFor(x => x.Test).NotEmpty();
    }
}
