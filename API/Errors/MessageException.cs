namespace API.Errors;

public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message)
    {
        
    }
}