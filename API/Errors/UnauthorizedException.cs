namespace API.Errors;

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message)
    {
        
    }
}

public class InvalidTokenException : UnauthorizedException
{
    public InvalidTokenException() : base("Invalid token")
    {
        
    }
}