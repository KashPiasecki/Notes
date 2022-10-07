namespace Notes.Application.Common.Interfaces;

public interface IJsonConverterWrapper
{
    string Serialize<T>(T input);
    T Deserialize<T>(string input);
}