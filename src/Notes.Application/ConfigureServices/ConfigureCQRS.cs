using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Notes.Application.Behaviors;
using Notes.Application.CQRS.Note.Commands.Create;

namespace Notes.Application.ConfigureServices;

public static class ConfigureCQRS
{
    public static void AddCQRS(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMediatR(Assembly.GetExecutingAssembly());
        serviceCollection.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        serviceCollection.AddValidatorsFromAssemblyContaining<CreateNoteCommandValidator>();
    }

}