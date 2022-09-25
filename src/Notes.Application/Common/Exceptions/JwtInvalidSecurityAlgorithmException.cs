namespace Notes.Application.Common.Exceptions;

public class JwtInvalidSecurityAlgorithmException : Exception
{
    public JwtInvalidSecurityAlgorithmException()
    {
    }

    public JwtInvalidSecurityAlgorithmException(string message)
        : base(message)
    {
    }

    public JwtInvalidSecurityAlgorithmException(string message, Exception inner)
        : base(message, inner)
    {
    }
}