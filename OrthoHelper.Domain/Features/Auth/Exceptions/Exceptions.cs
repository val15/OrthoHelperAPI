namespace OrthoHelper.Domain.Features.TextCorrection.Exceptions
{
    public class InvalidUserNameException : Exception
    {
        public InvalidUserNameException(string message) : base(message) { }
    }
}
