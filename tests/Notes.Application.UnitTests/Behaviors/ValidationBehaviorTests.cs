using MediatR;
using Notes.Application.Behaviors;
using Notes.Application.Common.Exceptions;
using Notes.Application.UnitTests.TestsUtility.Behaviors;
using TddXt.AnyRoot.Builder;
using TddXt.AnyRoot.Collections;

namespace Notes.Application.UnitTests.Behaviors;

public class ValidationBehaviorTests
{
    [Test]
    public async Task Handle_WithoutValidatorsPassed_CallsNext()
    {
        // Arrange
        var validators = Any.Enumerable<TestValidationEntityValidator>(0);
        var requestHandlerDelegate = Substitute.For<RequestHandlerDelegate<Unit>>();

        var validationBehavior = new ValidationBehavior<TestValidationEntity, Unit>(validators);
        // Act
        await validationBehavior.Handle(Any.Instance<TestValidationEntity>(), Any.Instance<CancellationToken>(), requestHandlerDelegate);

        // Assert
        await requestHandlerDelegate.Received(1).Invoke();
    }
    
    [Test]
    public async Task Handle_WithoutValidationErrors_CallsNext()
    {
        // Arrange
        var validators = Any.Enumerable<TestValidationEntityValidator>(1);
        var requestHandlerDelegate = Substitute.For<RequestHandlerDelegate<Unit>>();

        var validationBehavior = new ValidationBehavior<TestValidationEntity, Unit>(validators);
        var testValidationEntity = Any.Instance<TestValidationEntity>();
        
        // Act
        await validationBehavior.Handle(testValidationEntity, Any.Instance<CancellationToken>(), requestHandlerDelegate);

        // Assert
        await requestHandlerDelegate.Received(1).Invoke();
    }
    
    [Test]
    public async Task Handle_WithValidationErrors_ThrowsValidationException()
    {
        // Arrange
        var validators = Any.Enumerable<TestValidationEntityValidator>(1);
        var requestHandlerDelegate = Substitute.For<RequestHandlerDelegate<Unit>>();

        var validationBehavior = new ValidationBehavior<TestValidationEntity, Unit>(validators);
        var testValidationEntity = Any.Instance<TestValidationEntity>()
            .WithProperty(x => x.Test, string.Empty);
        
        // Act
        Func<Task> act = () => validationBehavior.Handle(testValidationEntity, Any.Instance<CancellationToken>(), requestHandlerDelegate);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}