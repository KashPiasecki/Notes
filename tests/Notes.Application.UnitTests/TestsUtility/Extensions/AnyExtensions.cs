using System.Text;
using TddXt.AnyExtensibility;
using TddXt.AnyGenerators.Root;

namespace Notes.Application.UnitTests.TestsUtility.Extensions;

public static class AnyExtensions
{
    public static string LowerCaseString(this BasicGenerator basicGenerator, int length) =>
        basicGenerator.InstanceOf(InlineGenerators.AlphaString(length)).ToLower();
    
    public static string UpperCaseString(this BasicGenerator basicGenerator, int length) =>
        basicGenerator.InstanceOf(InlineGenerators.AlphaString(length)).ToUpper();

    public static string MixedCaseString(this BasicGenerator basicGenerator, int length)
    {
        var mixedList = new List<char>();
        var upperAmount = length / 2;
        var lowerAmount = length / 2 + length % 2;

        for (var i = upperAmount - 1; i >= 0; i--)
        {
            mixedList.Add(basicGenerator.InstanceOf(InlineGenerators.UpperCaseAlphaChar()));
        }
        for (var i = lowerAmount - 1; i >= 0; i--)
        {
            mixedList.Add(basicGenerator.InstanceOf(InlineGenerators.LowerCaseAlphaChar()));
        }
        var random = new Random();
        var mixedCaseString = new string(mixedList.OrderBy(_ => random.Next()).ToArray());
        return mixedCaseString;
    }
    
    public static string ValidPassword(this BasicGenerator basicGenerator)
    {
        var stringBuilder = new StringBuilder();
        var upperCaseString = Any.LowerCaseString( 5);
        var lowerCaseString = Any.UpperCaseString(5);
        var numbersString = basicGenerator.InstanceOf(InlineGenerators.NumericString(5));
        return stringBuilder
            .Append(upperCaseString)
            .Append(lowerCaseString)
            .Append(numbersString)
            .ToString();
    }

    public static string Email(this BasicGenerator basicGenerator)
    {
        var stringBuilder = new StringBuilder();
        var address = basicGenerator.InstanceOf(InlineGenerators.LowercaseAlphaString());
        const string domain = "@example.com";
        return stringBuilder
            .Append(address)
            .Append(domain)
            .ToString();
    }
}