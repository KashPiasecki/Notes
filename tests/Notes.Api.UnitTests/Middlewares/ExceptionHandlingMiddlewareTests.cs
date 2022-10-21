using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Notes.Api.Middlewares;
using Notes.Application.Common.Exceptions;
using TddXt.AnyRoot;
using TddXt.AnyRoot.Strings;

namespace Notes.Api.UnitTests.Middlewares;

public class ExceptionHandlingMiddlewareTests
{
    [Test]
    public async Task InvokeAsync_CalledWithoutAnError_CallsNext()
    {
        // Arrange
        var response = Any.String();
        var defaultContext = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };
        var logger = Any.Instance<ILogger<ExceptionHandlingMiddleware>>();

        // Act
        var exceptionHandlingMiddleware = new ExceptionHandlingMiddleware(logger);
        await exceptionHandlingMiddleware.InvokeAsync(defaultContext, innerHttpContext =>
        {
            innerHttpContext.Response.WriteAsync(response);
            return Task.CompletedTask;
        });

        // Assert
        defaultContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(defaultContext.Response.Body).ReadToEndAsync();
        body.Should().Be(response);
    }
    
    [Test]
    public async Task InvokeAsync_CalledWithUnexpectedException_Returns500InternalServerError()
    {
        // Arrange
        var defaultContext = new DefaultHttpContext();
        var logger = Any.Instance<ILogger<ExceptionHandlingMiddleware>>();

        // Act
        var exceptionHandlingMiddleware = new ExceptionHandlingMiddleware(logger);
        var expectedException = Any.Exception();
        await exceptionHandlingMiddleware.InvokeAsync(defaultContext, _ => Task.FromException(expectedException));
        
        // Assert
        var result = (HttpStatusCode)defaultContext.Response.StatusCode;
        result.Should().Be(HttpStatusCode.InternalServerError);
    }
    
    [Test]
    public async Task InvokeAsync_CalledWithNotFoundException_Returns404NotFound()
    {
        // Arrange
        var defaultContext = new DefaultHttpContext();
        var logger = Any.Instance<ILogger<ExceptionHandlingMiddleware>>();

        // Act
        var exceptionHandlingMiddleware = new ExceptionHandlingMiddleware(logger);
        var expectedException = Any.Instance<NotFoundException>();
        await exceptionHandlingMiddleware.InvokeAsync(defaultContext, _ => Task.FromException(expectedException));
        
        // Assert
        var result = (HttpStatusCode)defaultContext.Response.StatusCode;
        result.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Test]
    public async Task InvokeAsync_CalledWithValidationException_Returns422UnprocessableEntity()
    {
        // Arrange
        var defaultContext = new DefaultHttpContext();
        var logger = Any.Instance<ILogger<ExceptionHandlingMiddleware>>();

        // Act
        var exceptionHandlingMiddleware = new ExceptionHandlingMiddleware(logger);
        var expectedException = Any.Instance<ValidationException>();
        await exceptionHandlingMiddleware.InvokeAsync(defaultContext, _ => Task.FromException(expectedException));
        // Assert
        var result = (HttpStatusCode)defaultContext.Response.StatusCode;
        result.Should().Be(HttpStatusCode.UnprocessableEntity);
    }
    
}