using System.Text;
using AutoFixture;

namespace Notes.Api.IntegrationTests.Utility;

public static class FixtureExtension
{
    public static string CreateValidPassword(this IFixture fixture)
    {
        var stringBuilder = new StringBuilder();
        var upperCase = string.Join("", fixture.CreateMany<char>(10)).ToUpper();
        var lowerCase = string.Join("", fixture.CreateMany<char>(10)).ToLower();
        stringBuilder.Append(upperCase).Append(lowerCase);
        for (var i = 0; i < 10; i++)
        {
            stringBuilder.Append(fixture.CreateInt(0, 9));
        }
        return stringBuilder.ToString();
    }
    
    public static int CreateInt(this IFixture fixture, int min, int max)
    {
        return fixture.Create<int>() % (max - min + 1) + min;
    }
}