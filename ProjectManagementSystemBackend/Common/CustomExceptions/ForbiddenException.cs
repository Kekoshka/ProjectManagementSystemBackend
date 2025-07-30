namespace ProjectManagementSystemBackend.Common.CustomExceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException() { }
        public ForbiddenException(string message) : base(message) { }
    }
}
