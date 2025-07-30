namespace ProjectManagementSystemBackend.Common.CustomExceptions
{
    public class ConflictException : Exception
    {
        public ConflictException() { }
        public ConflictException(string message) : base(message) { }
    }
}
