using System.Text.Json;
using Notes.Application.Common.Interfaces;

namespace Notes.Infrastructure.Utility.Wrappers;

public class JsonConverterWrapper : IJsonConverterWrapper
{
    public string Serialize<TRequest>(TRequest input)
    {
        return JsonSerializer.Serialize(input);
    }

    public TResponse Deserialize<TResponse>(string input)
    {
        var response = JsonSerializer.Deserialize<TResponse>(input);
        if (response is null)
        {
            throw new ArgumentNullException(nameof(Deserialize), "Deserialized object is null");
        }

        return response;
    }
}